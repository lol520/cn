using System;
using System.Linq;
using Color = System.Drawing.Color;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using LX_Orbwalker;

namespace Master
{
    class Jax : Program
    {
        private const String Version = "1.0.0";
        private Int32 Sheen = 3057, Trinity = 3075;
        private bool WardCasted = false;

        public Jax()
        {
            SkillQ = new Spell(SpellSlot.Q, 700);
            SkillW = new Spell(SpellSlot.W, 300);
            SkillE = new Spell(SpellSlot.E, 187.5f);
            SkillR = new Spell(SpellSlot.R, 100);
            SkillQ.SetTargetted(SkillQ.Instance.SData.SpellCastTime, SkillQ.Instance.SData.MissileSpeed);

            Config.AddSubMenu(new Menu("杩炴嫑", "csettings"));
            Config.SubMenu("csettings").AddItem(new MenuItem(Name + "qusage", "浣跨敤 Q").SetValue(true));
            Config.SubMenu("csettings").AddItem(new MenuItem(Name + "wusage", "浣跨敤 W").SetValue(true));
            Config.SubMenu("csettings").AddItem(new MenuItem(Name + "wuseMode", "W妯″紡").SetValue(new StringList(new[] { "|AA鍚巪", "|R鍚巪" })));
            Config.SubMenu("csettings").AddItem(new MenuItem(Name + "eusage", "浣跨敤 E").SetValue(true));
            Config.SubMenu("csettings").AddItem(new MenuItem(Name + "rusage", "浣跨敤 R").SetValue(true));
            Config.SubMenu("csettings").AddItem(new MenuItem(Name + "ruseMode", "R妯″紡").SetValue(new StringList(new[] { "|鐜╁HP|", "|鏁屼汉鏁伴噺|" })));
            Config.SubMenu("csettings").AddItem(new MenuItem(Name + "ruseHp", "濡傛灉HP鍦ㄤ娇鐢≧").SetValue(new Slider(50, 1)));
            Config.SubMenu("csettings").AddItem(new MenuItem(Name + "ruseEnemy", "濡傛灉浠ヤ笂鐨勬晫浜轰娇鐢≧").SetValue(new Slider(2, 1, 4)));
            Config.SubMenu("csettings").AddItem(new MenuItem(Name + "ignite", "濡傛灉鍙嚮鏉€鑷姩浣跨敤鐐圭噧").SetValue(true));
            Config.SubMenu("csettings").AddItem(new MenuItem(Name + "iusage", "浣跨敤椤圭洰").SetValue(true));

            Config.AddSubMenu(new Menu("楠氭壈", "hsettings"));
            Config.SubMenu("hsettings").AddItem(new MenuItem(Name + "useHarQ", "浣跨敤 Q").SetValue(true));
            Config.SubMenu("hsettings").AddItem(new MenuItem(Name + "harMode", "浣跨敤Q濡傛灉HP浠ヤ笂").SetValue(new Slider(20, 1)));
            Config.SubMenu("hsettings").AddItem(new MenuItem(Name + "useHarW", "浣跨敤 W").SetValue(true));
            Config.SubMenu("hsettings").AddItem(new MenuItem(Name + "useHarE", "浣跨敤 E").SetValue(true));

            Config.AddSubMenu(new Menu("娓呯嚎/娓呴噹", "LaneJungClear"));
            Config.SubMenu("LaneJungClear").AddItem(new MenuItem(Name + "useClearQ", "浣跨敤 Q").SetValue(true));
            Config.SubMenu("LaneJungClear").AddItem(new MenuItem(Name + "useClearW", "浣跨敤 W").SetValue(true));
            Config.SubMenu("LaneJungClear").AddItem(new MenuItem(Name + "useClearWMode", "W妯″紡").SetValue(new StringList(new[] { "|AA鍚巪", "|R鍚巪" })));
            Config.SubMenu("LaneJungClear").AddItem(new MenuItem(Name + "useClearE", "浣跨敤 E").SetValue(true));

            Config.AddSubMenu(new Menu("鏉傞」", "miscs"));
            Config.SubMenu("miscs").AddItem(new MenuItem(Name + "lasthitW", "浣跨敤W鏈€鍚庝竴鍑粅").SetValue(true));
            Config.SubMenu("miscs").AddItem(new MenuItem(Name + "killstealWQ", "浣跨敤WQ鍋稡UFF").SetValue(true));
            Config.SubMenu("miscs").AddItem(new MenuItem(Name + "useAntiE", "浣跨敤e鎺ヨ繎").SetValue(true));
            Config.SubMenu("miscs").AddItem(new MenuItem(Name + "useInterE", "浣跨敤e鎵撴柇").SetValue(true));
            Config.SubMenu("miscs").AddItem(new MenuItem(Name + "SkinID", "|鎹㈢毊鑲").SetValue(new Slider(8, 0, 8))).ValueChanged += SkinChanger;
            Config.SubMenu("miscs").AddItem(new MenuItem(Name + "packetCast", "浣跨敤灏佸寘").SetValue(true));

            Config.AddSubMenu(new Menu("鎶€鑳借寖鍥撮€夐」", "DrawSettings"));
            Config.SubMenu("DrawSettings").AddItem(new MenuItem(Name + "DrawQ", "Q 鑼冨洿").SetValue(true));
            Config.SubMenu("DrawSettings").AddItem(new MenuItem(Name + "DrawE", "E 鑼冨洿").SetValue(true));
Config.AddSubMenu(new Menu("鑻遍泟鑱旂洘绮変笣绔檌", "LOL520.CC"));
				Config.SubMenu("LOL520.CC").AddItem(new MenuItem("qunhao", "浜ゆ祦缇わ細21422249"));
				Config.SubMenu("LOL520.CC").AddItem(new MenuItem("qunhao2", "浜ゆ祦2缇わ細397773763"));
            Game.OnGameUpdate += OnGameUpdate;
            Drawing.OnDraw += OnDraw;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            Interrupter.OnPossibleToInterrupt += OnPossibleToInterrupt;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            LXOrbwalker.AfterAttack += AfterAttack;
            Game.PrintChat("<font color = \"#33CCCC\">Master of {0}</font> <font color = \"#00ff00\">v{1}</font>", Name, Version);
        }

        private void OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead) return;
            PacketCast = Config.Item(Name + "packetCast").GetValue<bool>();
            switch (LXOrbwalker.CurrentMode)
            {
                case LXOrbwalker.Mode.Combo:
                    NormalCombo();
                    break;
                case LXOrbwalker.Mode.Harass:
                    Harass();
                    break;
                case LXOrbwalker.Mode.LaneClear:
                    LaneJungClear();
                    break;
                case LXOrbwalker.Mode.LaneFreeze:
                    LaneJungClear();
                    break;
                case LXOrbwalker.Mode.Lasthit:
                    if (Config.Item(Name + "lasthitW").GetValue<bool>()) LastHit();
                    break;
                case LXOrbwalker.Mode.Flee:
                    WardJump(Game.CursorPos);
                    break;
            }
            if (Config.Item(Name + "killstealWQ").GetValue<bool>()) KillSteal();
        }

        private void OnDraw(EventArgs args)
        {
            if (Player.IsDead) return;
            if (Config.Item(Name + "DrawQ").GetValue<bool>() && SkillQ.Level > 0) Utility.DrawCircle(Player.Position, SkillQ.Range, SkillQ.IsReady() ? Color.Green : Color.Red);
            if (Config.Item(Name + "DrawE").GetValue<bool>() && SkillE.Level > 0) Utility.DrawCircle(Player.Position, SkillE.Range, SkillE.IsReady() ? Color.Green : Color.Red);
        }

        private void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!Config.Item(Name + "useAntiE").GetValue<bool>()) return;
            if (gapcloser.Sender.IsValidTarget(SkillE.Range) && SkillE.IsReady()) SkillE.Cast(PacketCast);
        }

        private void OnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if (!Config.Item(Name + "useInterE").GetValue<bool>()) return;
            if (SkillQ.IsReady() && SkillE.IsReady() && !SkillE.InRange(unit.Position) && unit.IsValidTarget(SkillQ.Range)) SkillQ.CastOnUnit(unit, PacketCast);
            if (unit.IsValidTarget(SkillE.Range) && SkillE.IsReady()) SkillE.Cast(PacketCast);
        }

        private void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe) return;
            if (args.SData.Name == "jaxrelentlessattack" && SkillW.IsReady() && (args.Target as Obj_AI_Base).IsValidTarget(LXOrbwalker.GetAutoAttackRange(Player, (Obj_AI_Base)args.Target)))
            {
                switch (LXOrbwalker.CurrentMode)
                {
                    case LXOrbwalker.Mode.Combo:
                        if (Config.Item(Name + "wusage").GetValue<bool>() && Config.Item(Name + "wuseMode").GetValue<StringList>().SelectedIndex == 1) SkillW.Cast(PacketCast);
                        break;
                    case LXOrbwalker.Mode.LaneClear:
                        if (Config.Item(Name + "useClearW").GetValue<bool>() && Config.Item(Name + "useClearWMode").GetValue<StringList>().SelectedIndex == 1) SkillW.Cast(PacketCast);
                        break;
                    case LXOrbwalker.Mode.LaneFreeze:
                        if (Config.Item(Name + "useClearW").GetValue<bool>() && Config.Item(Name + "useClearWMode").GetValue<StringList>().SelectedIndex == 1) SkillW.Cast(PacketCast);
                        break;
                }
            }
        }

        private void AfterAttack(Obj_AI_Base unit, Obj_AI_Base target)
        {
            if (unit.IsMe)
            {
                if (SkillW.IsReady() && target.IsValidTarget(LXOrbwalker.GetAutoAttackRange(Player, target)))
                {
                    switch (LXOrbwalker.CurrentMode)
                    {
                        case LXOrbwalker.Mode.Combo:
                            if (Config.Item(Name + "wusage").GetValue<bool>() && Config.Item(Name + "wuseMode").GetValue<StringList>().SelectedIndex == 0) SkillW.Cast(PacketCast);
                            break;
                        case LXOrbwalker.Mode.Harass:
                            if (Config.Item(Name + "useHarW").GetValue<bool>() && !Config.Item(Name + "useHarQ").GetValue<bool>()) SkillW.Cast(PacketCast);
                            break;
                        case LXOrbwalker.Mode.LaneClear:
                            if (Config.Item(Name + "useClearW").GetValue<bool>() && Config.Item(Name + "useClearWMode").GetValue<StringList>().SelectedIndex == 0) SkillW.Cast(PacketCast);
                            break;
                        case LXOrbwalker.Mode.LaneFreeze:
                            if (Config.Item(Name + "useClearW").GetValue<bool>() && Config.Item(Name + "useClearWMode").GetValue<StringList>().SelectedIndex == 0) SkillW.Cast(PacketCast);
                            break;
                    }
                }
            }
        }

        private void NormalCombo()
        {
            if (targetObj == null) return;
            if (Config.Item(Name + "eusage").GetValue<bool>() && SkillE.IsReady())
            {
                if (!Player.HasBuff("JaxCounterStrike", true))
                {
                    if ((Config.Item(Name + "qusage").GetValue<bool>() && SkillQ.InRange(targetObj.Position) && SkillQ.IsReady()) || SkillE.InRange(targetObj.Position)) SkillE.Cast(PacketCast);
                }
                else if (SkillE.InRange(targetObj.Position) && !targetObj.IsValidTarget(SkillE.Range - 3.5f)) SkillE.Cast(PacketCast);
            }
            if (Config.Item(Name + "qusage").GetValue<bool>() && SkillQ.IsReady() && SkillQ.InRange(targetObj.Position))
            {
                if ((Config.Item(Name + "eusage").GetValue<bool>() && SkillE.IsReady() && Player.HasBuff("JaxCounterStrike", true) && !SkillE.InRange(targetObj.Position)) || (!LXOrbwalker.InAutoAttackRange(targetObj) && Player.Distance(targetObj) > 425)) SkillQ.CastOnUnit(targetObj, PacketCast);
            }
            if (Config.Item(Name + "rusage").GetValue<bool>() && SkillR.IsReady())
            {
                switch (Config.Item(Name + "ruseMode").GetValue<StringList>().SelectedIndex)
                {
                    case 0:
                        if (Player.Health * 100 / Player.MaxHealth < Config.Item(Name + "ruseHp").GetValue<Slider>().Value) SkillR.Cast(PacketCast);
                        break;
                    case 1:
                        if (Player.CountEnemysInRange(600) >= Config.Item(Name + "ruseEnemy").GetValue<Slider>().Value) SkillR.Cast(PacketCast);
                        break;
                }
            }
            if (Config.Item(Name + "iusage").GetValue<bool>()) UseItem(targetObj);
            if (Config.Item(Name + "ignite").GetValue<bool>()) CastIgnite(targetObj);
        }

        private void Harass()
        {
            if (targetObj == null) return;
            if (Config.Item(Name + "useHarW").GetValue<bool>() && SkillW.IsReady())
            {
                if (Config.Item(Name + "useHarQ").GetValue<bool>() && SkillQ.InRange(targetObj.Position) && SkillQ.IsReady()) SkillW.Cast(PacketCast);
            }
            if (Config.Item(Name + "useHarE").GetValue<bool>() && SkillE.IsReady())
            {
                if (!Player.HasBuff("JaxCounterStrike", true))
                {
                    if ((Config.Item(Name + "useHarQ").GetValue<bool>() && SkillQ.InRange(targetObj.Position) && SkillQ.IsReady()) || SkillE.InRange(targetObj.Position)) SkillE.Cast(PacketCast);
                }
                else if (SkillE.InRange(targetObj.Position) && !targetObj.IsValidTarget(SkillE.Range - 3.5f)) SkillE.Cast(PacketCast);
            }
            if (Config.Item(Name + "useHarQ").GetValue<bool>() && SkillQ.IsReady() && SkillQ.InRange(targetObj.Position) && Player.Health * 100 / Player.MaxHealth >= Config.Item(Name + "harMode").GetValue<Slider>().Value)
            {
                if ((Config.Item(Name + "useHarE").GetValue<bool>() && SkillE.IsReady() && Player.HasBuff("JaxCounterStrike", true) && !SkillE.InRange(targetObj.Position)) || (!LXOrbwalker.InAutoAttackRange(targetObj) && Player.Distance(targetObj) > 425)) SkillQ.CastOnUnit(targetObj, PacketCast);
            }
        }

        private void LaneJungClear()
        {
            var minionObj = MinionManager.GetMinions(Player.Position, SkillQ.Range, MinionTypes.All, MinionTeam.NotAlly).FirstOrDefault();
            if (minionObj == null) return;
            if (Config.Item(Name + "useClearE").GetValue<bool>() && SkillE.IsReady())
            {
                if (!Player.HasBuff("JaxCounterStrike", true))
                {
                    if ((Config.Item(Name + "useClearQ").GetValue<bool>() && SkillQ.InRange(minionObj.Position) && SkillQ.IsReady()) || SkillE.InRange(minionObj.Position)) SkillE.Cast(PacketCast);
                }
                else if (SkillE.InRange(minionObj.Position) && !minionObj.IsValidTarget(SkillE.Range - 3.5f)) SkillE.Cast(PacketCast);
            }
            if (Config.Item(Name + "useClearQ").GetValue<bool>() && SkillQ.IsReady() && SkillQ.InRange(minionObj.Position))
            {
                if ((Config.Item(Name + "useClearE").GetValue<bool>() && SkillE.IsReady() && Player.HasBuff("JaxCounterStrike", true) && !SkillE.InRange(minionObj.Position)) || (!LXOrbwalker.InAutoAttackRange(minionObj) && Player.Distance(minionObj) > 425)) SkillQ.CastOnUnit(minionObj, PacketCast);
            }
        }

        private void LastHit()
        {
            var minionObj = (Obj_AI_Base)ObjectManager.Get<Obj_AI_Turret>().FirstOrDefault(i => i.IsValidTarget(LXOrbwalker.GetAutoAttackRange(Player, i)) && i.Health <= GetBonusDmg(i));
            if (minionObj == null) minionObj = MinionManager.GetMinions(Player.Position, LXOrbwalker.GetAutoAttackRange(), MinionTypes.All, MinionTeam.NotAlly).FirstOrDefault(i => i.Health <= GetBonusDmg(i));
            if (minionObj != null && (SkillW.IsReady() || Player.HasBuff("JaxEmpowerTwo", true)))
            {
                LXOrbwalker.SetAttack(false);
                if (!Player.HasBuff("JaxEmpowerTwo", true)) SkillW.Cast(PacketCast);
                if (Player.HasBuff("JaxEmpowerTwo", true)) Player.IssueOrder(GameObjectOrder.AttackUnit, minionObj);
                LXOrbwalker.SetAttack(true);
            }
        }

        private void WardJump(Vector3 Pos)
        {
            if (!SkillQ.IsReady()) return;
            bool IsWard = false;
            var posJump = (Player.Distance(Pos) > SkillQ.Range) ? Player.Position + Vector3.Normalize(Pos - Player.Position) * 600 : Pos;
            foreach (var jumpObj in ObjectManager.Get<Obj_AI_Base>().Where(i => !i.IsMe && i.IsAlly && !(i is Obj_AI_Turret) && i.Distance(Player) <= SkillQ.Range + i.BoundingRadius && i.Distance(posJump) <= 230))
            {
                if (jumpObj.Name.ToLower().Contains("ward")) IsWard = true;
                SkillQ.CastOnUnit(jumpObj, PacketCast);
                if (!jumpObj.Name.ToLower().Contains("ward")) return;
            }
            if (!IsWard && GetWardSlot() != null && !WardCasted)
            {
                GetWardSlot().UseItem(posJump);
                WardCasted = true;
                Utility.DelayAction.Add(1000, () => WardCasted = false);
            }
        }

        private void KillSteal()
        {
            var target = ObjectManager.Get<Obj_AI_Hero>().FirstOrDefault(i => i.IsValidTarget(SkillQ.Range) && i.Health <= SkillQ.GetDamage(i) + GetBonusDmg(i) && i != targetObj);
            if (target != null && Player.Mana >= SkillQ.Instance.ManaCost)
            {
                if (SkillW.IsReady()) SkillW.Cast(PacketCast);
                if (SkillQ.IsReady() && Player.HasBuff("JaxEmpowerTwo", true)) SkillQ.CastOnUnit(target, PacketCast);
            }
        }

        private void UseItem(Obj_AI_Hero target)
        {
            if (Items.CanUseItem(Bilge) && Player.Distance(target) <= 450) Items.UseItem(Bilge, target);
            if (Items.CanUseItem(Blade) && Player.Distance(target) <= 450) Items.UseItem(Blade, target);
            if (Items.CanUseItem(Rand) && Player.CountEnemysInRange(450) >= 1) Items.UseItem(Rand);
        }

        private double GetBonusDmg(Obj_AI_Base target)
        {
            double DmgItem = 0;
            if (Items.HasItem(Sheen) && (Items.CanUseItem(Sheen) || Player.HasBuff("sheen", true)) && Player.BaseAttackDamage > DmgItem) DmgItem = Player.BaseAttackDamage;
            if (Items.HasItem(Trinity) && (Items.CanUseItem(Trinity) || Player.HasBuff("sheen", true)) && Player.BaseAttackDamage * 2 > DmgItem) DmgItem = Player.BaseAttackDamage * 2;
            return SkillW.GetDamage(target) + Player.CalcDamage(target, Damage.DamageType.Physical, Player.BaseAttackDamage + Player.FlatPhysicalDamageMod + DmgItem);
        }
    }
}