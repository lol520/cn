using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace SAwareness
{
    internal class AutoSmite
    {
        public enum SpellType
        {
            Target,
            Skillshot,
            Active
        }

        private readonly String[] _monsters =
        {
            "GreatWraith", "Wraith", "AncientGolem", "GiantWolf", "LizardElder",
            "Golem", "Worm", "Dragon", "Wight", "TT_Spiderboss", "TTNGolem", "TTNWolf", "TTNWraith",
            "SRU_Baron", "SRU_Dragon", "SRU_Blue", "SRU_Red", "SRU_Gromp", "SRU_Krug", "SRU_Murkwolf", "SRU_Razorbeak" //Need to check
        };

        private readonly String[] _usefulMonsters = { "AncientGolem", "LizardElder", "Worm", "Dragon", "TT_Spiderboss", "SRU_Baron", "SRU_Dragon", "SRU_Blue", "SRU_Red" };

        public AutoSmite()
        {
            Game.OnGameUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        ~AutoSmite()
        {
            Game.OnGameUpdate -= Game_OnGameUpdate;
            Drawing.OnDraw -= Drawing_OnDraw;
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if (!IsActive())
                return;
            foreach (Obj_AI_Minion minion in ObjectManager.Get<Obj_AI_Minion>())
            {
                int smiteDamage = GetSmiteDamage();
                if (Vector3.Distance(ObjectManager.Player.ServerPosition, minion.ServerPosition) < 1500)
                {
                    foreach (string monster in _monsters)
                    {
                        if (minion.SkinName == monster && minion.IsVisible)
                        {
                            Vector2 pos = Drawing.WorldToScreen(minion.ServerPosition);
                            String health = minion.Health != 0 ? (((int) minion.Health - smiteDamage)).ToString() : "";
                            health =
                                !Menu.AutoSmite.GetMenuItem("SAwarenessAutoSmiteKeyActive")
                                    .GetValue<KeyBind>()
                                    .Active
                                    ? health + " Disabled"
                                    : health;
                            Drawing.DrawText(pos[0], pos[1], Color.SkyBlue, health);
                            if(minion.IsDead)
                                continue;
                            Vector2 hpBarPos = minion.HPBarPosition;
                            hpBarPos.X += 45;
                            hpBarPos.Y += 18;
                            float hpXPos = hpBarPos.X + (63 * ((float)(GetSmiteDamage() + (GetExtraDamage(minion) != null 
                                ? GetExtraDamage(minion).Damage 
                                : 0)) / minion.MaxHealth));
                            Drawing.DrawLine(hpXPos, hpBarPos.Y, hpXPos, hpBarPos.Y + 5, 2, System.Drawing.Color.SkyBlue);
                        }
                    }
                }
            }
        }

        public bool IsActive()
        {
            return Menu.Activator.GetActive() && Menu.AutoSmite.GetActive();
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (!IsActive() || !Menu.AutoSmite.GetMenuItem("SAwarenessAutoSmiteKeyActive").GetValue<KeyBind>().Active)
                return;
            foreach (Obj_AI_Minion minion in ObjectManager.Get<Obj_AI_Minion>())
            {
                List<Obj_AI_Minion> min = ObjectManager.Get<Obj_AI_Minion>().ToList();
                if (Vector3.Distance(ObjectManager.Player.ServerPosition, minion.ServerPosition) < 750 &&
                    minion.Health > 0 && minion.IsVisible)
                {
                    int smiteDamage = GetSmiteDamage();
                    ExtraDamage extraDamageInfo = null;
                    int extraDamage = 0;
                    if (Menu.AutoSmite.GetMenuItem("SAwarenessAutoSmiteAutoSpell").GetValue<bool>())
                    {
                        extraDamageInfo = GetExtraDamage(minion);
                        if (extraDamageInfo != null)
                            extraDamage = (int) extraDamageInfo.Damage;
                    }
                    if (minion.Health <= smiteDamage)
                    {
                        if (!Menu.AutoSmite.GetMenuItem("SAwarenessAutoSmiteSmallCampsActive").GetValue<bool>())
                        {
                            foreach (string monster in _usefulMonsters)
                            {
                                if (minion.SkinName == monster)
                                {
                                    SmiteIt(minion);
                                }
                            }
                        }
                        else
                        {
                            foreach (string monster in _monsters)
                            {
                                if (minion.SkinName == monster)
                                {
                                    SmiteIt(minion);
                                }
                            }
                        }
                    }
                    else if (extraDamageInfo != null && minion.Health <= smiteDamage + extraDamage)
                    {
                        if (Vector3.Distance(ObjectManager.Player.ServerPosition, minion.ServerPosition) <
                            extraDamageInfo.Range + minion.BoundingRadius)
                        {
                            foreach (string monster in _usefulMonsters)
                            {
                                if (minion.SkinName == monster)
                                {
                                    SmiteIt(minion, extraDamageInfo);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void SmiteIt(Obj_AI_Base minion, ExtraDamage extraDamageInfo = null)
        {
            SpellSlot smiteSlot = GetSmiteSlot();
            if (smiteSlot != SpellSlot.Unknown)
            {
                if (extraDamageInfo == null)
                {
                    GamePacket gPacketT =
                        Packet.C2S.Cast.Encoded(new Packet.C2S.Cast.Struct(minion.NetworkId, smiteSlot));
                    gPacketT.Send();
                    ObjectManager.Player.SummonerSpellbook.CastSpell(smiteSlot, minion);
                }
                else
                {
                    GamePacket gPacketT;
                    switch (extraDamageInfo.Type)
                    {
                        case SpellType.Active:
                            gPacketT =
                                Packet.C2S.Cast.Encoded(new Packet.C2S.Cast.Struct(minion.NetworkId,
                                    extraDamageInfo.Slot));
                            gPacketT.Send();
                            ObjectManager.Player.SummonerSpellbook.CastSpell(smiteSlot, minion);
                            break;

                        case SpellType.Skillshot:
                            gPacketT =
                                Packet.C2S.Cast.Encoded(new Packet.C2S.Cast.Struct(0, extraDamageInfo.Slot, -1, 0, 0,
                                    minion.ServerPosition.X, minion.ServerPosition.Y));
                            gPacketT.Send();
                            ObjectManager.Player.SummonerSpellbook.CastSpell(smiteSlot, minion);
                            break;

                        case SpellType.Target:
                            gPacketT =
                                Packet.C2S.Cast.Encoded(new Packet.C2S.Cast.Struct(minion.NetworkId,
                                    extraDamageInfo.Slot));
                            ObjectManager.Player.SummonerSpellbook.CastSpell(smiteSlot, minion);
                            gPacketT.Send();
                            break;
                    }
                    Utility.DelayAction.Add(
                        (int) (Game.Time /*+ (extraTimeForCast/1000)*(sender.ServerPosition.Distance(endPos)/1000)*/+
                               (ObjectManager.Player.ServerPosition.Distance(minion.ServerPosition)/1000)),
                        () =>
                            Packet.C2S.Cast.Encoded(new Packet.C2S.Cast.Struct(minion.NetworkId, smiteSlot))
                                .Send());
                    ObjectManager.Player.SummonerSpellbook.CastSpell(smiteSlot, minion);
                    //gPacketT = Packet.C2S.Cast.Encoded(new Packet.C2S.Cast.Struct(minion.NetworkId, (SpellSlot)slot));
                    //gPacketT.Send();
                }
            }
        }

        private ExtraDamage GetExtraDamage(Obj_AI_Base minion)
        {
            Obj_AI_Hero player = ObjectManager.Player;
            switch (ObjectManager.Player.ChampionName)
            {
                case "Chogath":
                    return player.Spellbook.CanUseSpell(SpellSlot.R) == SpellState.Ready
                        ? new ExtraDamage(player.GetSpellDamage(minion, SpellSlot.R), 175, SpellType.Target, SpellSlot.R)
                        : null;
                case "Elise":
                    return player.Spellbook.CanUseSpell(SpellSlot.Q) == SpellState.Ready
                        ? new ExtraDamage(player.GetSpellDamage(minion, SpellSlot.Q), 475, SpellType.Target, SpellSlot.Q)
                        : null;
                case "Fizz":
                    return player.Spellbook.CanUseSpell(SpellSlot.Q) == SpellState.Ready
                        ? new ExtraDamage(player.GetSpellDamage(minion, SpellSlot.Q), 550, SpellType.Target, SpellSlot.Q)
                        : null;
                case "Kayle":
                    return player.Spellbook.CanUseSpell(SpellSlot.Q) == SpellState.Ready
                        ? new ExtraDamage(player.GetSpellDamage(minion, SpellSlot.Q), 650, SpellType.Target, SpellSlot.Q)
                        : null;
                case "KhaZix":
                    return player.Spellbook.CanUseSpell(SpellSlot.Q) == SpellState.Ready
                        ? new ExtraDamage(player.GetSpellDamage(minion, SpellSlot.Q, 
                            (MinionManager.GetMinions(minion.ServerPosition, 500f, MinionTypes.All, MinionTeam.Neutral)).Count > 0 
                            ? ObjectManager.Player.HasBuff("khazixqevo", true) ? 2 : 0
                            : ObjectManager.Player.HasBuff("khazixqevo", true) ? 3 : 1), 325, SpellType.Target, SpellSlot.Q)
                        : null;
                case "LeeSin":
                    return player.Spellbook.CanUseSpell(SpellSlot.Q) == SpellState.Ready && minion.HasBuff("BlindMonkQOne", true)
                        ? new ExtraDamage(player.GetSpellDamage(minion, SpellSlot.R, 1) > 400 ? 400 : player.GetSpellDamage(minion, SpellSlot.R, 1), 
                        1200, SpellType.Skillshot, SpellSlot.Q) : null;
                case "Lux":
                    return player.Spellbook.CanUseSpell(SpellSlot.R) == SpellState.Ready
                        ? new ExtraDamage(player.GetSpellDamage(minion, SpellSlot.R), 3340, SpellType.Skillshot,
                            SpellSlot.R)
                        : null;
                case "MasterYi":
                    return player.Spellbook.CanUseSpell(SpellSlot.Q) == SpellState.Ready
                        ? new ExtraDamage(player.GetSpellDamage(minion, SpellSlot.Q), 600, SpellType.Target, SpellSlot.Q)
                        : null;
                case "MonkeyKing":
                    return player.Spellbook.CanUseSpell(SpellSlot.Q) == SpellState.Ready
                        ? new ExtraDamage(player.GetSpellDamage(minion, SpellSlot.Q), 100, SpellType.Active, SpellSlot.Q)
                        : null;
                case "Nasus":
                    return player.Spellbook.CanUseSpell(SpellSlot.Q) == SpellState.Ready
                        ? new ExtraDamage(player.GetSpellDamage(minion, SpellSlot.Q), 100, SpellType.Active, SpellSlot.Q)
                        : null; //TODO: Check for nasus stacks
                case "Nunu":
                    return player.Spellbook.CanUseSpell(SpellSlot.Q) == SpellState.Ready
                        ? new ExtraDamage(player.GetSpellDamage(minion, SpellSlot.Q), 155, SpellType.Target, SpellSlot.Q)
                        : null;
                case "Olaf":
                    return player.Spellbook.CanUseSpell(SpellSlot.E) == SpellState.Ready
                        ? new ExtraDamage(player.GetSpellDamage(minion, SpellSlot.E), 325, SpellType.Target, SpellSlot.E)
                        : null;
                case "Pantheon":
                    return player.Spellbook.CanUseSpell(SpellSlot.Q) == SpellState.Ready
                        ? new ExtraDamage(player.GetSpellDamage(minion, SpellSlot.Q), 600, SpellType.Target, SpellSlot.Q)
                        : null;
                case "Rammus":
                    return player.Spellbook.CanUseSpell(SpellSlot.Q) == SpellState.Ready
                        ? new ExtraDamage(player.GetSpellDamage(minion, SpellSlot.Q), 100, SpellType.Active, SpellSlot.Q)
                        : null;
                case "Rengar":
                    return player.Spellbook.CanUseSpell(SpellSlot.Q) == SpellState.Ready
                        ? new ExtraDamage(player.GetSpellDamage(minion, SpellSlot.Q), 100, SpellType.Active, SpellSlot.Q)
                        : null;
                case "Shaco":
                    return player.Spellbook.CanUseSpell(SpellSlot.E) == SpellState.Ready
                        ? new ExtraDamage(player.GetSpellDamage(minion, SpellSlot.E), 625, SpellType.Target, SpellSlot.E)
                        : null;
                case "Twitch":
                    return player.Spellbook.CanUseSpell(SpellSlot.E) == SpellState.Ready
                        ? new ExtraDamage(player.GetSpellDamage(minion, SpellSlot.E), 1200, SpellType.Active,
                            SpellSlot.E) : null;
                case "Udyr":
                    return player.Spellbook.CanUseSpell(SpellSlot.R) == SpellState.Ready
                        ? new ExtraDamage(player.GetSpellDamage(minion, SpellSlot.R), 100, SpellType.Active,
                            SpellSlot.R) : null;
                case "Veigar":
                    return player.Spellbook.CanUseSpell(SpellSlot.Q) == SpellState.Ready
                        ? new ExtraDamage(player.GetSpellDamage(minion, SpellSlot.Q), 650, SpellType.Active,
                            SpellSlot.Q) : null;
                case "Vi":
                    return player.Spellbook.CanUseSpell(SpellSlot.E) == SpellState.Ready
                        ? new ExtraDamage(player.GetSpellDamage(minion, SpellSlot.E), 600, SpellType.Target, SpellSlot.E)
                        : null;
                case "Volibear":
                    return player.Spellbook.CanUseSpell(SpellSlot.W) == SpellState.Ready
                        ? new ExtraDamage(player.GetSpellDamage(minion, SpellSlot.W), 400, SpellType.Target, SpellSlot.W)
                        : null;
                case "Warwick":
                    return player.Spellbook.CanUseSpell(SpellSlot.Q) == SpellState.Ready
                        ? new ExtraDamage(player.GetSpellDamage(minion, SpellSlot.Q), 400, SpellType.Target, SpellSlot.Q)
                        : null;
                case "Xerath":
                    return player.Spellbook.CanUseSpell(SpellSlot.R) == SpellState.Ready
                        ? new ExtraDamage(player.GetSpellDamage(minion, SpellSlot.R), 3200 + 1200 * player.Spellbook.GetSpell(SpellSlot.R).Level, SpellType.Target, SpellSlot.R)
                        : null;
                default:
                    return null;
            }
        }

        private int GetSmiteDamage()
        {
            int level = ObjectManager.Player.Level;
            int smiteDamage = 390 +
                              (level < 5
                                  ? 20*(level - 1)
                                  : (level < 10
                                      ? 60 + 30*(level - 4)
                                      : (level < 15
                                          ? 210 + 40*(level - 9)
                                          : 410 + 50*(level - 14))));
            return smiteDamage;
        }

        private SpellSlot GetSmiteSlot()
        {
            foreach (SpellDataInst spell in ObjectManager.Player.SummonerSpellbook.Spells)
            {
                if (spell.Name.ToLower().Contains("smite") && spell.State == SpellState.Ready)
                    return spell.Slot;
            }
            return SpellSlot.Unknown;
        }

        public class ExtraDamage
        {
            public double Damage;
            public int Range;
            public SpellSlot Slot;
            public SpellType Type;

            public ExtraDamage(double damage, int range, SpellType type, SpellSlot slot)
            {
                Damage = AdjustDamage(damage);
                Range = range;
                Type = type;
                Slot = slot;
            }

            private double AdjustDamage(double damage)
            {
                const int spiritStone = 1080;
                const int spiritElderLizard = 3209;
                const int spiritSpectralWraith = 3206;

                if (Items.HasItem(spiritStone))
                    return damage + (damage*0.2);
                if (Items.HasItem(spiritElderLizard))
                    return damage + (damage*0.2);
                if (Items.HasItem(spiritSpectralWraith))
                    return damage + (damage*0.3);
                return damage;
            }
        }
    }
}
