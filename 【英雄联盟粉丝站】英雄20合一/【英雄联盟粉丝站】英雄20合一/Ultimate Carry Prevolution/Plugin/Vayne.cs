using System.Collections.Generic;
using System.Xml.Xsl;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using xSLx_Orbwalker;
using Color = System.Drawing.Color;

namespace Ultimate_Carry_Prevolution.Plugin
{
    class Vayne : Champion
    {
        public Vayne()
        {
            SetSpells();
            LoadMenu();
        }

        public Obj_AI_Base SelectedTarget = null;

        private void SetSpells()
        {
            Q = new Spell(SpellSlot.Q);
            Q.SetSkillshot(300, 250, 1250, false, SkillshotType.SkillshotCircle);
            
            W = new Spell(SpellSlot.W);

            E = new Spell(SpellSlot.E, 550);
            E.SetTargetted(250, 1600);

            R = new Spell(SpellSlot.R);
        }

        private void LoadMenu()
        {
            var champMenu = new Menu("Vayne Plugin", "Vayne");
            {
                var comboMenu = new Menu("杩炴嫑", "Combo");
                {
                    comboMenu.AddItem(new MenuItem("Focus_Target", "閿佸畾鐩爣").SetValue(true));
                    AddSpelltoMenu(comboMenu, "Q", true);
                    AddSpelltoMenu(comboMenu, "E", true);
                    AddSpelltoMenu(comboMenu, "R", true);
                    AddSpelltoMenu(comboMenu, "Botrk", true, "浣跨敤鐮磋触/灏忓集鍒€");
                    champMenu.AddSubMenu(comboMenu);
                }

                var harassMenu = new Menu("楠氭壈", "Harass");
                {
                    AddSpelltoMenu(harassMenu, "Q", true);
                    AddSpelltoMenu(harassMenu, "E", true);
                    AddManaManagertoMenu(harassMenu, 30);
                    champMenu.AddSubMenu(harassMenu);
                }

                var laneClearMenu = new Menu("娓呯嚎", "LaneClear");
                {
                    AddSpelltoMenu(laneClearMenu, "Q", true);
                    AddManaManagertoMenu(laneClearMenu, 0);
                    champMenu.AddSubMenu(laneClearMenu);
                }

                var miscMenu = new Menu("鏉傞」", "Misc");
                {
                    miscMenu.AddItem(new MenuItem("Misc_Q_Always", "Q+骞矨").SetValue(new KeyBind("N".ToCharArray()[0], KeyBindType.Toggle)).SetValue(false));
                    miscMenu.AddItem(new MenuItem("Misc_useE_Gap_Closer", "E闃茬獊").SetValue(true));
                    miscMenu.AddItem(new MenuItem("Misc_useE_Interrupt", "E鎵撴柇").SetValue(true));
                    miscMenu.AddItem(new MenuItem("Misc_E_Next", "鑷姩E").SetValue(new KeyBind("E".ToCharArray()[0], KeyBindType.Toggle)));
                    miscMenu.AddItem(new MenuItem("Misc_Push_Distance", "E璺濈").SetValue(new Slider(300, 350, 400)));
                    champMenu.AddSubMenu(miscMenu);
                }

                var drawMenu = new Menu("鏄剧ず", "Drawing");
                {
                    drawMenu.AddItem(new MenuItem("Draw_Disabled", "绂佺敤").SetValue(false));
                    drawMenu.AddItem(new MenuItem("Draw_E", "鏄剧ずE").SetValue(true));

                    MenuItem drawComboDamageMenu = new MenuItem("Draw_ComboDamage", "鏄剧ず浼ゅ").SetValue(true);
                    drawMenu.AddItem(drawComboDamageMenu);
                    Utility.HpBarDamageIndicator.DamageToUnit = GetComboDamage;
                    Utility.HpBarDamageIndicator.Enabled = drawComboDamageMenu.GetValue<bool>();
                    drawComboDamageMenu.ValueChanged +=
                        delegate(object sender, OnValueChangeEventArgs eventArgs)
                        {
                            Utility.HpBarDamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
                        };

                    champMenu.AddSubMenu(drawMenu);
                }
            }

            Menu.AddSubMenu(champMenu);
            Menu.AddToMainMenu();
        }

        public override void OnDraw()
        {
            if (Menu.Item("Draw_Disabled").GetValue<bool>())
            {
                xSLxOrbwalker.DisableDrawing();
                return;
            }
            xSLxOrbwalker.EnableDrawing();

            if (Menu.Item("Draw_E").GetValue<bool>())
                if (E.Level > 0)
                    Utility.DrawCircle(MyHero.Position, E.Range, E.IsReady() ? Color.Green : Color.Red);
        }

        private IEnumerable<SpellSlot> GetSpellCombo()
        {
            var spellCombo = new List<SpellSlot>();
            if (Q.IsReady())
                spellCombo.Add(SpellSlot.Q);
            if (W.IsReady())
                spellCombo.Add(SpellSlot.W);
            if (E.IsReady())
                spellCombo.Add(SpellSlot.E);
            return spellCombo;
        }

        private float GetComboDamage(Obj_AI_Base target)
        {
            double comboDamage = (float)ObjectManager.Player.GetComboDamage(target, GetSpellCombo());
            if (Bilge.IsReady())
                comboDamage += MyHero.GetItemDamage(target, Damage.DamageItems.Bilgewater);

            if (Botrk.IsReady())
                comboDamage += MyHero.GetItemDamage(target, Damage.DamageItems.Botrk);

            return (float)(comboDamage + ObjectManager.Player.GetAutoAttackDamage(target));
        }

        public override void OnPassive()
        {
			if(xSLxOrbwalker.CurrentMode != xSLxOrbwalker.Mode.Combo)
				return;
			if(Menu.Item("Focus_Target").GetValue<bool>())
			{
			    if (SimpleTs.GetSelectedTarget() != null)
			        xSLxOrbwalker.ForcedTarget = SimpleTs.GetSelectedTarget();
			    else
			        xSLxOrbwalker.ForcedTarget = null;
			}

			xSLxOrbwalker.ForcedTarget = null;
        }

        public override void OnCombo()
        {
            if (IsSpellActive("Q") && Menu.Item("Misc_Q_Always").GetValue<bool>() && Q.IsReady())
                Q.Cast();
            if (IsSpellActive("E"))
                Cast_E();

            var Q_Target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Physical);
            if (Q_Target != null)
            {
                if (IsSpellActive("Botrk"))
                {
                    if (Bilge.IsReady() && (GetComboDamage(Q_Target) + MyHero.GetAutoAttackDamage(Q_Target) * 6 < Q_Target.Health || GetHealthPercent() < 35))
                        Use_Bilge(Q_Target);

                    if (Botrk.IsReady() && (GetComboDamage(Q_Target) + MyHero.GetAutoAttackDamage(Q_Target) * 6 < Q_Target.Health || GetHealthPercent() < 35))
                        Use_Botrk(Q_Target);
                }
            }
            if (IsSpellActive("R"))
                Cast_R();
        }

        public override void OnHarass()
        {
            if (IsSpellActive("E"))
                Cast_E();
        }

        public override void OnLaneClear()
        {
            xSLxOrbwalker.ForcedTarget = null;

            int minion = MinionManager.GetMinions(MyHero.ServerPosition, xSLxOrbwalker.GetAutoAttackRange()).Count;

            if (IsSpellActive("Q") && ManaManagerAllowCast() && minion > 0)
                Q.Cast(Game.CursorPos);
        }

        public override void OnAfterAttack(Obj_AI_Base unit, Obj_AI_Base target)
        {
            if (!unit.IsMe)
                return;

            E_Next_AA((Obj_AI_Hero)target);
            
            if(xSLxOrbwalker.CurrentMode == xSLxOrbwalker.Mode.Combo || (xSLxOrbwalker.CurrentMode == xSLxOrbwalker.Mode.Harass && ManaManagerAllowCast()))
                if(IsSpellActive("Q") && Q.IsReady())
                    Q.Cast(Game.CursorPos);
        }

        public override void OnGapClose(ActiveGapcloser gapcloser)
        {
            if (!Menu.Item("Misc_useE_Gap_Closer").GetValue<bool>()) return;

            if (E.IsReady() && gapcloser.Sender.IsValidTarget(E.Range))
                E.Cast(gapcloser.Sender, UsePackets());
        }

        public override void OnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if (spell.DangerLevel < InterruptableDangerLevel.Medium || unit.IsAlly)
                return;

            if (Menu.Item("Misc_useE_Interrupt").GetValue<bool>() && unit.IsValidTarget(E.Range))
                E.Cast(unit, UsePackets());
        }

        private void E_Next_AA(Obj_AI_Hero target)
        {
            if (!E.IsReady() || !target.IsValidTarget(E.Range) || !Menu.Item("Misc_E_Next").GetValue<KeyBind>().Active)
                return;

            E.Cast(target, UsePackets());
            Menu.Item("Misc_E_Next").SetValue(new KeyBind("E".ToCharArray()[0], KeyBindType.Toggle));
        }

        private void Cast_E()
        {
            var pushDistance = Menu.Item("Misc_Push_Distance").GetValue<Slider>().Value;
            var target = SimpleTs.GetTarget(E.Range, SimpleTs.DamageType.Physical);

            if (MyHero.Distance(target) < 50)
                E.Cast(target, UsePackets());

            var targetPred = E.GetPrediction(target);
            var targetPredPos = targetPred.UnitPosition + Vector3.Normalize(targetPred.UnitPosition - MyHero.ServerPosition)*(pushDistance+target.BoundingRadius);

            if (IsPassWall(targetPred.UnitPosition, targetPredPos))
                E.Cast(target, UsePackets());
        }

        private void Cast_R()
        {
	        var target = xSLxOrbwalker.GetPossibleTarget();
			var dmg = GetComboDamage(target) + MyHero.GetAutoAttackDamage(target) * 6;

			if(dmg > target.Health)
                R.Cast();
        }

    }
}
