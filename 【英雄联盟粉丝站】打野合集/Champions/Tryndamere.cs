using System;
using System.Linq;
using Color = System.Drawing.Color;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using LX_Orbwalker;

namespace Master
{
    class Tryndamere : Program
    {
        private const String Version = "1.0.5";

        public Tryndamere()
        {
            SkillQ = new Spell(SpellSlot.Q, 320);
            SkillW = new Spell(SpellSlot.W, 750);
            SkillE = new Spell(SpellSlot.E, 660);
            SkillR = new Spell(SpellSlot.R, 400);
            SkillE.SetSkillshot(SkillE.Instance.SData.SpellCastTime, SkillE.Instance.SData.LineWidth, SkillE.Instance.SData.MissileSpeed, false, SkillshotType.SkillshotLine);

            Config.AddSubMenu(new Menu("杩炴嫑/楠氭壈", "csettings"));
            Config.SubMenu("csettings").AddItem(new MenuItem(Name + "qusage", "浣跨敤 Q").SetValue(true));
            Config.SubMenu("csettings").AddItem(new MenuItem(Name + "autoqusage", "|浣跨敤Q濡傛灉HP鍦▅").SetValue(new Slider(40, 1)));
            Config.SubMenu("csettings").AddItem(new MenuItem(Name + "wusage", "浣跨敤 W").SetValue(true));
            Config.SubMenu("csettings").AddItem(new MenuItem(Name + "eusage", "|浣跨敤E鍦ㄨ繛鎷泑").SetValue(true));
            Config.SubMenu("csettings").AddItem(new MenuItem(Name + "autoeusage", "|浣跨敤E濡傛灉HP浠ヤ笂|").SetValue(new Slider(20, 1)));
            Config.SubMenu("csettings").AddItem(new MenuItem(Name + "ignite", "濡傛灉鍙嚮鏉€鑷姩浣跨敤鐐圭噧").SetValue(true));
            Config.SubMenu("csettings").AddItem(new MenuItem(Name + "iusage", "浣跨敤椤圭洰").SetValue(true));

            Config.AddSubMenu(new Menu("娓呯嚎/娓呴噹", "LaneJungClear"));
            Config.SubMenu("LaneJungClear").AddItem(new MenuItem(Name + "useClearE", "浣跨敤 E").SetValue(true));

            Config.AddSubMenu(new Menu("鏉傞」", "miscs"));
            Config.SubMenu("miscs").AddItem(new MenuItem(Name + "killstealE", "鑷姩E鍋锋潃").SetValue(true));
            Config.SubMenu("miscs").AddItem(new MenuItem(Name + "surviveQ", "灏濊瘯鐢≦鐢熷瓨").SetValue(true));
            Config.SubMenu("miscs").AddItem(new MenuItem(Name + "surviveR", "灏濊瘯鐢≧鐢熷瓨").SetValue(true));
            Config.SubMenu("miscs").AddItem(new MenuItem(Name + "SkinID", "|鎹㈢毊鑲").SetValue(new Slider(4, 0, 6))).ValueChanged += SkinChanger;
            Config.SubMenu("miscs").AddItem(new MenuItem(Name + "packetCast", "浣跨敤灏佸寘").SetValue(true));

            Config.AddSubMenu(new Menu("鎶€鑳借寖鍥撮€夐」", "DrawSettings"));
            Config.SubMenu("DrawSettings").AddItem(new MenuItem(Name + "DrawW", "W 鑼冨洿").SetValue(true));
            Config.SubMenu("DrawSettings").AddItem(new MenuItem(Name + "DrawE", "E 鑼冨洿").SetValue(true));
Config.AddSubMenu(new Menu("鑻遍泟鑱旂洘绮変笣绔檌", "LOL520.CC"));
				Config.SubMenu("LOL520.CC").AddItem(new MenuItem("qunhao", "浜ゆ祦缇わ細21422249"));
				Config.SubMenu("LOL520.CC").AddItem(new MenuItem("qunhao2", "浜ゆ祦2缇わ細397773763"));

            Game.OnGameUpdate += OnGameUpdate;
            Drawing.OnDraw += OnDraw;
            GameObject.OnCreate += OnCreate;
            Game.PrintChat("<font color = \"#33CCCC\">Master of {0}</font> <font color = \"#00ff00\">v{1}</font>", Name, Version);
        }

        private void OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead) return;
            PacketCast = Config.Item(Name + "packetCast").GetValue<bool>();
            switch (LXOrbwalker.CurrentMode)
            {
                case LXOrbwalker.Mode.Combo:
                    NormalCombo(false);
                    break;
                case LXOrbwalker.Mode.Harass:
                    NormalCombo(true);
                    break;
                case LXOrbwalker.Mode.LaneClear:
                    LaneJungClear();
                    break;
                case LXOrbwalker.Mode.LaneFreeze:
                    LaneJungClear();
                    break;
                case LXOrbwalker.Mode.Flee:
                    if (SkillE.IsReady()) SkillE.Cast(Game.CursorPos, PacketCast);
                    break;
            }
            if (Config.Item(Name + "killstealE").GetValue<bool>()) KillSteal();
        }

        private void OnDraw(EventArgs args)
        {
            if (Player.IsDead) return;
            if (Config.Item(Name + "DrawW").GetValue<bool>() && SkillW.Level > 0) Utility.DrawCircle(Player.Position, SkillW.Range, SkillW.IsReady() ? Color.Green : Color.Red);
            if (Config.Item(Name + "DrawE").GetValue<bool>() && SkillE.Level > 0) Utility.DrawCircle(Player.Position, SkillE.Range, SkillE.IsReady() ? Color.Green : Color.Red);
        }

        private void OnCreate(GameObject sender, EventArgs args)
        {
            if (!(sender is Obj_SpellMissile)) return;
            var missle = (Obj_SpellMissile)sender;
            var caster = missle.SpellCaster;
            if (caster.IsEnemy)
            {
                if (Config.Item(Name + "surviveQ").GetValue<bool>() && SkillQ.IsReady())
                {
                    var HealthBuff = (Player.Mana == 100) ? new Int32[] { 80, 135, 190, 245, 300 }[SkillQ.Level - 1] + 1.5 * Player.FlatMagicDamageMod : (new Int32[] { 30, 40, 50, 60, 70 }[SkillQ.Level - 1] + 0.3 * Player.FlatMagicDamageMod + new double[] { 0.5, 0.95, 1.4, 1.85, 2.3 }[SkillQ.Level - 1] + 0.012 * Player.FlatMagicDamageMod) * Player.Mana;
                    if (missle.SData.Name.Contains("BasicAttack"))
                    {
                        if (missle.Target.IsMe && Player.Health <= caster.GetAutoAttackDamage(Player, true) && Player.Health + HealthBuff > caster.GetAutoAttackDamage(Player, true)) SkillQ.Cast();
                    }
                    else if (missle.Target.IsMe || missle.EndPosition.Distance(Player.Position) <= 130)
                    {
                        if (missle.SData.Name == "summonerdot")
                        {
                            if (Player.Health <= (caster as Obj_AI_Hero).GetSummonerSpellDamage(Player, Damage.SummonerSpell.Ignite) && Player.Health + HealthBuff > (caster as Obj_AI_Hero).GetSummonerSpellDamage(Player, Damage.SummonerSpell.Ignite)) SkillQ.Cast();
                        }
                        else if (Player.Health <= (caster as Obj_AI_Hero).GetSpellDamage(Player, (caster as Obj_AI_Hero).GetSpellSlot(missle.SData.Name, false), 1) && Player.Health + HealthBuff > (caster as Obj_AI_Hero).GetSpellDamage(Player, (caster as Obj_AI_Hero).GetSpellSlot(missle.SData.Name, false), 1)) SkillQ.Cast();
                    }
                }
                if (Config.Item(Name + "surviveR").GetValue<bool>() && SkillR.IsReady())
                {
                    if (missle.SData.Name.Contains("BasicAttack"))
                    {
                        if (missle.Target.IsMe && Player.Health <= caster.GetAutoAttackDamage(Player, true)) SkillR.Cast();
                    }
                    else if (missle.Target.IsMe || missle.EndPosition.Distance(Player.Position) <= 130)
                    {
                        if (missle.SData.Name == "summonerdot")
                        {
                            if (Player.Health <= (caster as Obj_AI_Hero).GetSummonerSpellDamage(Player, Damage.SummonerSpell.Ignite)) SkillR.Cast();
                        }
                        else if (Player.Health <= (caster as Obj_AI_Hero).GetSpellDamage(Player, (caster as Obj_AI_Hero).GetSpellSlot(missle.SData.Name, false), 1)) SkillR.Cast();
                    }
                }
            }
        }

        private void NormalCombo(bool IsHarass)
        {
            if (targetObj == null) return;
            if (Config.Item(Name + "qusage").GetValue<bool>() && SkillQ.IsReady() && Player.Health * 100 / Player.MaxHealth <= Config.Item(Name + "autoqusage").GetValue<Slider>().Value && Player.CountEnemysInRange(800) >= 1) SkillQ.Cast(PacketCast);
            if (Config.Item(Name + "wusage").GetValue<bool>() && SkillW.IsReady() && SkillW.InRange(targetObj.Position))
            {
                if (Utility.IsBothFacing(Player, targetObj, 300))
                {
                    if (Player.GetAutoAttackDamage(targetObj) < targetObj.GetAutoAttackDamage(Player) || Player.Health < targetObj.Health) SkillW.Cast(PacketCast);
                }
                else if (Player.IsFacing(targetObj) && !targetObj.IsFacing(Player) && !LXOrbwalker.InAutoAttackRange(targetObj)) SkillW.Cast(PacketCast);
            }
            if (!IsHarass && Config.Item(Name + "eusage").GetValue<bool>() && SkillE.IsReady() && SkillE.InRange(targetObj.Position) && !LXOrbwalker.InAutoAttackRange(targetObj))
            {
                if (Player.Health * 100 / Player.MaxHealth >= Config.Item(Name + "autoeusage").GetValue<Slider>().Value)
                {
                    SkillE.Cast(targetObj.Position + Vector3.Normalize(targetObj.Position - Player.Position) * 200, PacketCast);
                }
                else if (SkillR.IsReady() || (Player.Mana >= 70 && SkillQ.IsReady())) SkillE.Cast(targetObj.Position + Vector3.Normalize(targetObj.Position - Player.Position) * 200, PacketCast);
            }
            if (Config.Item(Name + "iusage").GetValue<bool>()) UseItem(targetObj);
            if (Config.Item(Name + "ignite").GetValue<bool>()) CastIgnite(targetObj);
        }

        private void LaneJungClear()
        {
            var minionObj = MinionManager.GetMinions(Player.Position, SkillE.Range, MinionTypes.All, MinionTeam.NotAlly);
            if (minionObj.Count == 0) return;
            if (Config.Item(Name + "useClearE").GetValue<bool>() && SkillE.IsReady()) SkillE.Cast(SkillE.GetLineFarmLocation(minionObj.ToList()).Position, PacketCast);
        }

        private void KillSteal()
        {
            var target = ObjectManager.Get<Obj_AI_Hero>().FirstOrDefault(i => i.IsValidTarget(SkillE.Range) && CanKill(i, SkillE) && i != targetObj);
            if (target != null && SkillE.IsReady()) SkillE.Cast(targetObj.Position + Vector3.Normalize(targetObj.Position - Player.Position) * 200, PacketCast);
        }

        private void UseItem(Obj_AI_Hero target)
        {
            if (Items.CanUseItem(Bilge) && Player.Distance(target) <= 450) Items.UseItem(Bilge, target);
            if (Items.CanUseItem(Blade) && Player.Distance(target) <= 450) Items.UseItem(Blade, target);
            if (Items.CanUseItem(Tiamat) && Player.CountEnemysInRange(350) >= 1) Items.UseItem(Tiamat);
            if (Items.CanUseItem(Hydra) && (Player.CountEnemysInRange(350) >= 2 || (Player.GetAutoAttackDamage(target) < target.Health && Player.CountEnemysInRange(350) == 1))) Items.UseItem(Hydra);
            if (Items.CanUseItem(Rand) && Player.CountEnemysInRange(450) >= 1) Items.UseItem(Rand);
            if (Items.CanUseItem(Youmuu) && Player.CountEnemysInRange(350) >= 1) Items.UseItem(Youmuu);
        }
    }
}