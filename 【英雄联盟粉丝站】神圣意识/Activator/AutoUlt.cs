using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace SAwareness
{
    class AutoUlt
    {
        private static Ult _ult;

        public AutoUlt()
        {
            if (_ult == null)
            {
                Init();
            }
            Game.OnGameUpdate += Game_OnGameUpdate;
        }

        ~AutoUlt()
        {
            Game.OnGameUpdate -= Game_OnGameUpdate;
        }

        public bool IsActive()
        {
            return Menu.Activator.GetActive() && Menu.ActivatorAutoHeal.GetActive();
        }

        private static void Init()
        {
            switch (ObjectManager.Player.ChampionName)
            {
                case "Kayle":
                    _ult = new Ult(UltType.Invincible, SpellSlot.E, 900);
                    break;

                case "Lissandra":
                    _ult = new Ult(UltType.Invincible, SpellSlot.R, 0);
                    break;

                case "Lulu":
                    _ult = new Ult(UltType.Invincible, SpellSlot.R, 900);
                    break;

                case "Shen":
                    _ult = new Ult(UltType.Global, SpellSlot.R, 90000);
                    break;

                case "Soraka":
                    _ult = new Ult(UltType.Global, SpellSlot.R, 90000);
                    break;

                case "Tryndamere":
                    _ult = new Ult(UltType.Invincible, SpellSlot.R, 0, false);
                    break;

                case "Yorick":
                    _ult = new Ult(UltType.Invincible, SpellSlot.R, 900);
                    break;

                case "Zilean":
                    _ult = new Ult(UltType.Invincible, SpellSlot.R, 900);
                    break;

                default:
                    return;
            }
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (!IsActive() || _ult == null || ObjectManager.Player.Spellbook.GetSpell(_ult.SpellSlot).State != SpellState.Ready)
                return;

            var tempDamages =
                new Dictionary<Obj_AI_Hero, List<Activator.IncomingDamage>>(Activator.Damages);
            foreach (var damage in Activator.Damages)
            {
                Obj_AI_Hero hero = damage.Key;

                if (!Menu.AutoShield.GetMenuItem("SAwarenessAutoShieldAlly").GetValue<bool>())
                    if (hero.NetworkId != ObjectManager.Player.NetworkId)
                        continue;
            }

            foreach (var damage in tempDamages)
            {
                if (Activator.CalcMaxDamage(damage.Key) > damage.Key.Health &&
                    (damage.Key.Distance(ObjectManager.Player.ServerPosition) < _ult.Range))
                {
                    if (!Menu.ActivatorAutoUlt.GetMenuItem("SAwarenessActivatorAutoUltAlly").GetValue<bool>() &&
                        damage.Key.NetworkId != ObjectManager.Player.NetworkId)
                    {
                        continue;
                    }
                    if (_ult.Target)
                    {
                        Packet.C2S.Cast.Encoded(new Packet.C2S.Cast.Struct(damage.Key.NetworkId, _ult.SpellSlot)).Send();
                        return;
                    }
                    if (damage.Key.NetworkId == ObjectManager.Player.NetworkId)
                    {
                        Packet.C2S.Cast.Encoded(new Packet.C2S.Cast.Struct(damage.Key.NetworkId, _ult.SpellSlot)).Send();
                        return;
                    }
                }
            }
        }

        enum UltType
        {
            Invincible,
            Global
        }

        private class Ult
        {
            public UltType UltType;
            public SpellSlot SpellSlot;
            public double Range;
            public bool Target;

            public Ult(UltType ultType, SpellSlot spellSlot, double range, bool target = true)
            {
                UltType = ultType;
                SpellSlot = spellSlot;
                Range = range;
                Target = target;
            }
        }
    }
}
