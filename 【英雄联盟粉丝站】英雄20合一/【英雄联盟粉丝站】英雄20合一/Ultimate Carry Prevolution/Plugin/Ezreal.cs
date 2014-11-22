using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using xSLx_Orbwalker;
using Color = System.Drawing.Color;

namespace Ultimate_Carry_Prevolution.Plugin
{
    class Ezreal : Champion
    {
        public Ezreal()
        {
            SetSpells();
            LoadMenu();
        }

        private Spell R2;
        private void SetSpells()
        {
            Q = new Spell(SpellSlot.Q, 1200);
            Q.SetSkillshot(0.25f, 60f, 2000f, true, SkillshotType.SkillshotLine);

            W = new Spell(SpellSlot.W, 1050);
            W.SetSkillshot(0.25f, 80f, 2000f, false, SkillshotType.SkillshotLine);

            E = new Spell(SpellSlot.E, 475);
            E.SetSkillshot(0.25f, 80f, 1600f, false, SkillshotType.SkillshotCircle);

            R = new Spell(SpellSlot.R, 3000);
            R.SetSkillshot(1f, 160f, 2000f, false, SkillshotType.SkillshotLine);

            R2 = new Spell(SpellSlot.R, 3000);
            R2.SetSkillshot(1f, 160f, 2000f, true, SkillshotType.SkillshotLine);
        }

        private void LoadMenu()
        {
            var champMenu = new Menu("Ezreal Plugin", "Ezreal");
            {
				var SpellMenu = new Menu("娉曟湳", "SpellMenu");
				{
					var qMenu = new Menu("Q", "QMenu");
					{
                        qMenu.AddItem(new MenuItem("Q_Max_Range", "Q Max 鑼冨洿").SetValue(new Slider(1200, 500, 1200)));
                        qMenu.AddItem(new MenuItem("Auto_Q_Slow", "鑷姩Q鍑忛€熺殑").SetValue(true));
                        qMenu.AddItem(new MenuItem("Auto_Q_Immobile", "鑷姩Q闈欐").SetValue(true));
						SpellMenu.AddSubMenu(qMenu);
					}

					var wMenu = new Menu("W", "WMenu");
					{
                        wMenu.AddItem(new MenuItem("W_Max_Range", "W Max 鑼冨洿").SetValue(new Slider(1050, 500, 1050)));
						SpellMenu.AddSubMenu(wMenu);
					}

					var eMenu = new Menu("E", "EMenu");
					{
						eMenu.AddItem(new MenuItem("E_On_Killable", "鍙潃鐢‥").SetValue(true));
                        eMenu.AddItem(new MenuItem("E_On_Safe", "妫€鏌瀹夊叏").SetValue(true));
						SpellMenu.AddSubMenu(eMenu);
					}

					var rMenu = new Menu("R", "RMenu");
					{
						rMenu.AddItem(new MenuItem("R_Min_Range", "R Min 鑼冨洿").SetValue(new Slider(300, 0, 1000)));
                        rMenu.AddItem(new MenuItem("R_Max_Range", "R Max 鑼冨洿").SetValue(new Slider(2000, 0, 4000)));
                        rMenu.AddItem(new MenuItem("R_Mec", "R 鍑讳腑 >=").SetValue(new Slider(3, 1, 5)));
						rMenu.AddItem(new MenuItem("R_Overkill_Check", "鍑绘潃鎻愮ず").SetValue(true));

						rMenu.AddSubMenu(new Menu("涓嶄娇鐢≧", "Dont_R"));
						foreach(var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(enemy => enemy.Team != MyHero.Team)
						)
                            rMenu.SubMenu("Dont_R")
								.AddItem(new MenuItem("Dont_R" + enemy.BaseSkinName, enemy.BaseSkinName).SetValue(false));

						SpellMenu.AddSubMenu(rMenu);
					}

				    champMenu.AddSubMenu(SpellMenu);
				}

                var comboMenu = new Menu("杩炴嫑", "Combo");
                {
                    AddSpelltoMenu(comboMenu, "Q", true);
                    AddSpelltoMenu(comboMenu, "W", true);
                    AddSpelltoMenu(comboMenu, "E", true);
                    AddSpelltoMenu(comboMenu, "R", true);
                    comboMenu.AddItem(new MenuItem("R_Nearest_Killable", "R鏈€杩戝彲鏉€").SetValue(new KeyBind("R".ToCharArray()[0], KeyBindType.Press)));
                    champMenu.AddSubMenu(comboMenu);
                }

                var harassMenu = new Menu("楠氭壈", "Harass");
                {
                    AddSpelltoMenu(harassMenu, "Q", true);
                    harassMenu.AddItem(new MenuItem("Q_Auto_Harass", "Q锛堥攣瀹氾級").SetValue(new KeyBind("N".ToCharArray()[0], KeyBindType.Press)));
                    AddSpelltoMenu(harassMenu, "W", true);
                    AddManaManagertoMenu(harassMenu, 30);
                    champMenu.AddSubMenu(harassMenu);
                }

                var laneClearMenu = new Menu("娓呯嚎", "LaneClear");
                {
                    AddSpelltoMenu(laneClearMenu, "Q", true);
                    champMenu.AddSubMenu(laneClearMenu);
                }

                var miscMenu = new Menu("鏉傞」", "Misc");
                {
                    harassMenu.AddItem(new MenuItem("Misc_Use_WE", "WE鎸囬拡").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
                    champMenu.AddSubMenu(miscMenu);
                }

				var drawMenu = new Menu("鏄剧ず", "Drawing");
				{
					drawMenu.AddItem(new MenuItem("Draw_Disabled", "绂佺敤").SetValue(false));
                    drawMenu.AddItem(new MenuItem("Draw_Q", "Q鑼冨洿").SetValue(true));
					drawMenu.AddItem(new MenuItem("Draw_W", "W鑼冨洿").SetValue(true));
					drawMenu.AddItem(new MenuItem("Draw_E", "E鑼冨洿").SetValue(true));
					drawMenu.AddItem(new MenuItem("Draw_R", "R鑼冨洿").SetValue(true));
					drawMenu.AddItem(new MenuItem("Draw_R_Killable", "R鏍囪").SetValue(true));

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

		private IEnumerable<SpellSlot> GetSpellCombo()
		{
			var spellCombo = new List<SpellSlot>();
			if (Q.IsReady())
				spellCombo.Add(SpellSlot.Q);
            if (W.IsReady())
                spellCombo.Add(SpellSlot.W);
			if (E.IsReady())
				spellCombo.Add(SpellSlot.E);
			if (R.IsReady())
				spellCombo.Add(SpellSlot.R);
			return spellCombo;
		}

        private float GetComboDamage(Obj_AI_Base target)
        {
            double comboDamage = (float) ObjectManager.Player.GetComboDamage(target, GetSpellCombo());
            return (float) (comboDamage + ObjectManager.Player.GetAutoAttackDamage(target));
        }


        public override void OnDraw()
		{
			if (Menu.Item("Draw_Disabled").GetValue<bool>())
			{
				xSLxOrbwalker.DisableDrawing();
				return;
			}
			xSLxOrbwalker.EnableDrawing();
            if (Menu.Item("Draw_Q").GetValue<bool>())
                if (Q.Level > 0)
                    Utility.DrawCircle(MyHero.Position, Q.Range, Q.IsReady() ? Color.Green : Color.Red);
			if (Menu.Item("Draw_W").GetValue<bool>())
				if (W.Level > 0)
					Utility.DrawCircle(MyHero.Position, W.Range, W.IsReady() ? Color.Green : Color.Red);

			if (Menu.Item("Draw_E").GetValue<bool>())
				if (E.Level > 0)
					Utility.DrawCircle(MyHero.Position, E.Range, E.IsReady() ? Color.Green : Color.Red);

			if (Menu.Item("Draw_R").GetValue<bool>())
				if (R.Level > 0)
					Utility.DrawCircle(MyHero.Position, R.Range, R.IsReady() ? Color.Green : Color.Red);

			if (Menu.Item("Draw_R_Killable").GetValue<bool>() && R.IsReady())
			{
				foreach (var unit in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsValidTarget(4000) && !x.IsDead && x.IsEnemy).OrderBy(x => x.Health))
				{
				    var health = unit.Health + unit.HPRegenRate * 3 + 25;
                    if (Get_R_Dmg(unit) > health)
					{
						Vector2 wts = Drawing.WorldToScreen(unit.Position);
						Drawing.DrawText(wts[0] - 20, wts[1], Color.White, "KILL!!!");
				    }
				}
			}
		}

        public override void OnPassive()
        {
            Q.Range = Menu.Item("Q_Max_Range").GetValue<Slider>().Value;
            W.Range = Menu.Item("W_Max_Range").GetValue<Slider>().Value;
            R.Range = Menu.Item("R_Max_Range").GetValue<Slider>().Value;

            if (Menu.Item("R_Nearest_Killable").GetValue<KeyBind>().Active)
                Cast_R_Killable();

            if (Menu.Item("Q_Auto_Harass").GetValue<KeyBind>().Active)
                Cast_BasicSkillshot_Enemy(Q);

            if (Menu.Item("Misc_Use_WE").GetValue<KeyBind>().Active && W.IsReady() && E.IsReady())
            {
                var vec = MyHero.ServerPosition + Vector3.Normalize(Game.CursorPos - MyHero.ServerPosition)*E.Range;

                W.Cast(vec, UsePackets());
                E.Cast(vec, UsePackets());
            }

            var target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Physical);

            if (target != null)
            {
                if (Q.GetPrediction(target).Hitchance >= HitChance.Immobile && Menu.Item("Auto_Q_Slow").GetValue<bool>())
                    Q.Cast(target, UsePackets());
                if (target.HasBuffOfType(BuffType.Slow) && Menu.Item("Auto_Q_Immobile").GetValue<bool>())
                    Q.Cast(target, UsePackets());
            }
        }

        public override void OnCombo()
        {
            if (IsSpellActive("Q"))
                Cast_BasicSkillshot_Enemy(Q);
            if (IsSpellActive("W"))
                Cast_BasicSkillshot_Enemy(W);
            if (IsSpellActive("E"))
                Cast_E();
            if (IsSpellActive("R"))
                Cast_R();
        }

        public override void OnHarass()
        {
            if (IsSpellActive("Q") && ManaManagerAllowCast())
                Cast_BasicSkillshot_Enemy(Q);
            if (IsSpellActive("W") && ManaManagerAllowCast())
                Cast_BasicSkillshot_Enemy(W);
        }

        public override void OnLaneClear()
        {
            if (IsSpellActive("Q") && ManaManagerAllowCast())
                Cast_BasicSkillshot_AOE_Farm(Q);
        }

        private void Cast_E()
        {
            var target = SimpleTs.GetTarget(E.Range + 500, SimpleTs.DamageType.Magical);

            if (E.IsReady() && target != null && Menu.Item("E_On_Killable").GetValue<bool>())
            {
                if (MyHero.GetSpellDamage(target, SpellSlot.E) > target.Health + 25)
                {
                    if (Menu.Item("E_On_Safe").GetValue<bool>())
                    {
                        var E_pos = E.GetPrediction(target);
                        if(Utility.CountEnemysInRange(500, E_pos.UnitPosition) < 2)
                            E.Cast(E_pos.UnitPosition, UsePackets());
                    }
                    else
                    {
                        E.Cast(target, UsePackets());
                    }
                }
            }
        }

        private void Cast_R()
        {
            var target = SimpleTs.GetTarget(R.Range, SimpleTs.DamageType.Magical);

            if (R.IsReady() && target != null)
            {
                if (Menu.Item("Dont_R" + target.BaseSkinName) != null)
                {
                    if (!Menu.Item("Dont_R" + target.BaseSkinName).GetValue<bool>())
                    {
                        var minRange = Menu.Item("R_Min_Range").GetValue<Slider>().Value;
                        var minHit = Menu.Item("R_Mec").GetValue<Slider>().Value;

                        if (Get_R_Dmg(target) > target.Health && MyHero.Distance(target) > minRange)
                        {
                            R.Cast(target, UsePackets());
                            return;
                        }

                        if (MyHero.Distance(target) > minRange)
                        {
                            R.CastIfWillHit(target, minHit, UsePackets());
                        }

                    }
                }
            }
        }

        private void Cast_R_Killable()
        {
            foreach (var unit in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsValidTarget(2000) && !x.IsDead && x.IsEnemy).OrderBy(x => x.Health))
            {
                if (Menu.Item("Dont_R" + unit.BaseSkinName) != null)
                {
                    if (!Menu.Item("Dont_R" + unit.BaseSkinName).GetValue<bool>())
                    {
                        var health = unit.Health + unit.HPRegenRate * 3 + 25;
                        if (Get_R_Dmg(unit) > health)
                        {
                            R.Cast(unit, UsePackets());
                            return; 
                        }
                    }
                }
            }
        }

        private float Get_R_Dmg(Obj_AI_Hero target)
        {
            double dmg = 0;

            dmg += MyHero.GetSpellDamage(target, SpellSlot.R);

            var R_Pred = R2.GetPrediction(target);
            var collisionCount = R_Pred.CollisionObjects.Count;

            if (collisionCount >= 7)
                dmg = dmg * .3;
            else if (collisionCount != 0)
                dmg = dmg * (collisionCount / 10);

            return (float)dmg;
        }
    }
}
