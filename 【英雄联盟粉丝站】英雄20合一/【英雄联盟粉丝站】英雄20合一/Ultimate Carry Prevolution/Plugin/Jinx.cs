using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using xSLx_Orbwalker;
using Color = System.Drawing.Color;

namespace Ultimate_Carry_Prevolution.Plugin
{
    class Jinx : Champion
    {
        public Jinx()
        {
            SetSpells();
            LoadMenu();
        }

        private void SetSpells()
        {
            Q = new Spell(SpellSlot.Q);

            W = new Spell(SpellSlot.W, 1500);
            W.SetSkillshot(0.6f, 60f, 1450, true, SkillshotType.SkillshotLine);

            E = new Spell(SpellSlot.E, 900f);
            E.SetSkillshot(0.7f, 120f, 1750f, false, SkillshotType.SkillshotCircle);

            R = new Spell(SpellSlot.R, 25000f);
            R.SetSkillshot(0.6f, 140f, 1700f, false, SkillshotType.SkillshotLine);
        }

        private void LoadMenu()
        {
			var champMenu = new Menu("Jinx Plugin", "Jinx");
            {
				var SpellMenu = new Menu("娉曟湳", "SpellMenu");
				{
					var qMenu = new Menu("Q", "QMenu");
					{
						qMenu.AddItem(new MenuItem("Auto_Switch_Q", "瓒呭嚭鑼冨洿鍒囨崲Q").SetValue(true));
						SpellMenu.AddSubMenu(qMenu);
					}

					var wMenu = new Menu("W", "WMenu");
					{
                        wMenu.AddItem(new MenuItem("W_Min_Range", "W Min鑼冨洿").SetValue(new Slider(300, 0, 1500)));
						wMenu.AddItem(new MenuItem("Auto_W_Slow", "鑷姩W鍑忛€熺殑|").SetValue(true));
						wMenu.AddItem(new MenuItem("Auto_W_Immobile", "鑷姩W闈欐").SetValue(true));
						SpellMenu.AddSubMenu(wMenu);
					}

					var eMenu = new Menu("E", "EMenu");
					{
						eMenu.AddItem(new MenuItem("Auto_E_Slow", "鑷姩E鍑忛€熺殑|").SetValue(true));
						eMenu.AddItem(new MenuItem("Auto_E_Immobile", "鑷姩E闈欐").SetValue(true));
						eMenu.AddItem(new MenuItem("E_Behind_Target", "E闄勮繎").SetValue(true));
						eMenu.AddItem(new MenuItem("E_Behind_Distance", "E璺濈").SetValue(new Slider(200, 100, 300)));
						SpellMenu.AddSubMenu(eMenu);
					}

					var rMenu = new Menu("R", "RMenu");
					{
						rMenu.AddItem(new MenuItem("R_Min_Range", "R Min鑼冨洿").SetValue(new Slider(300, 0, 1000)));
                        rMenu.AddItem(new MenuItem("R_Max_Range", "R Max鑼冨洿").SetValue(new Slider(2000, 0, 4000)));
						rMenu.AddItem(new MenuItem("R_Overkill_Check", "鍑绘潃鎻愮ず").SetValue(true));

						rMenu.AddSubMenu(new Menu("Don't use R on", "涓峈"));
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
                    comboMenu.AddItem(new MenuItem("R_Nearest_Killable", "R闄勮繎鍙潃").SetValue(new KeyBind("R".ToCharArray()[0], KeyBindType.Press)));
                    champMenu.AddSubMenu(comboMenu);
                }

                var harassMenu = new Menu("楠氭壈", "Harass");
                {
                    AddSpelltoMenu(harassMenu, "Q", true);
                    AddSpelltoMenu(harassMenu, "W", true);
                    AddManaManagertoMenu(harassMenu, 30);
                    champMenu.AddSubMenu(harassMenu);
                }

                var fleeMenu = new Menu("閫冭窇", "Flee");
                {
                    AddSpelltoMenu(fleeMenu, "E", true, "Use E In Front of Enemy");
                    champMenu.AddSubMenu(fleeMenu);
                }

                var laneClearMenu = new Menu("娓呯嚎", "LaneClear");
                {
                    AddSpelltoMenu(laneClearMenu, "Q", true, "Switch back to Mini");

                    champMenu.AddSubMenu(laneClearMenu);
                }

                var miscMenu = new Menu("鏉傞」", "Misc");
                {
                    miscMenu.AddItem(new MenuItem("E_Gap_Closer", "E绐佽繘").SetValue(true));
                    champMenu.AddSubMenu(miscMenu);
                }

				var drawMenu = new Menu("鏄剧ず", "Drawing");
				{
					drawMenu.AddItem(new MenuItem("Draw_Disabled", "绂佺敤").SetValue(false));
					drawMenu.AddItem(new MenuItem("Draw_W", "鏄剧ずW").SetValue(true));
					drawMenu.AddItem(new MenuItem("Draw_E", "鏄剧ずE").SetValue(true));
					drawMenu.AddItem(new MenuItem("Draw_R", "鏄剧ずR").SetValue(true));
					drawMenu.AddItem(new MenuItem("Draw_R_Killable", "鏄剧ずR鍙潃").SetValue(true));

					MenuItem drawComboDamageMenu = new MenuItem("Draw_ComboDamage", "鏄剧ず浼ゅ").SetValue(true);
					drawMenu.AddItem(drawComboDamageMenu);
					Utility.HpBarDamageIndicator.DamageToUnit = GetComboDamage;
					Utility.HpBarDamageIndicator.Enabled = drawComboDamageMenu.GetValue<bool>();
					drawComboDamageMenu.ValueChanged +=
						delegate(object sender, OnValueChangeEventArgs eventArgs)
						{
							Utility.HpBarDamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
						}; // i have there a sync option you not, mine auto syncs to yours

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
				    var health = unit.Health + unit.HPRegenRate + 25;
					if (ObjectManager.Player.GetSpellDamage(unit, SpellSlot.R) > health)
					{
						Vector2 wts = Drawing.WorldToScreen(unit.Position);
						Drawing.DrawText(wts[0] - 20, wts[1], Color.White, "KILL!!!");
				    }
				}
			}
		}

		public override void OnPassive()
		{
		    R.Range = Menu.Item("R_Max_Range").GetValue<Slider>().Value;

			if (Menu.Item("R_Nearest_Killable").GetValue<KeyBind>().Active)
                Cast_R_Killable();

			Cast_E(Menu.Item("Auto_E_Immobile").GetValue<bool>(), Menu.Item("Auto_E_Slow").GetValue<bool>(), false);

			Cast_W(Menu.Item("Auto_W_Immobile").GetValue<bool>(), Menu.Item("Auto_W_Slow").GetValue<bool>());
		}

		public override void OnCombo()
		{
            var W_Target = SimpleTs.GetTarget(1500, SimpleTs.DamageType.Physical);
            var minRange = Menu.Item("W_Min_Range").GetValue<Slider>().Value;

		    if (W_Target != null)
		    {
		        if (MyHero.Distance(W_Target) > minRange)
		        {
		            var W_Pred = W.GetPrediction(W_Target);
		            if (IsSpellActive("W") && W.IsReady() && W_Pred.Hitchance >= HitChance.High)
		                W.Cast(W_Pred.CastPosition, UsePackets());
		        }
		    }

		    if (IsSpellActive("Q"))
				Q_Check();

		    if (IsSpellActive("E"))
				Cast_E(Menu.Item("Auto_W_Immobile").GetValue<bool>(), Menu.Item("Auto_W_Slow").GetValue<bool>(), Menu.Item("E_Behind_Target").GetValue<bool>());
			if (IsSpellActive("R"))
                Cast_R();
		}

		public override void OnHarass()
		{
			if (ManaManagerAllowCast())
			{
                var W_Target = SimpleTs.GetTarget(1500, SimpleTs.DamageType.Physical);
                var minRange = Menu.Item("W_Min_Range").GetValue<Slider>().Value;

			    if (W_Target != null)
			    {
			        if (MyHero.Distance(W_Target) > minRange)
			        {
			            var W_Pred = W.GetPrediction(W_Target);
			            if (IsSpellActive("W") && W.IsReady() && W_Pred.Hitchance >= HitChance.High)
			                W.Cast(W_Pred.CastPosition, UsePackets());
			        }
			    }

			    if (IsSpellActive("Q"))
					Q_Check();
			}
		}

		public override void OnFlee()
		{
			if (IsSpellActive("E"))
				Cast_E_Escape();
		}

        public override void OnLaneClear()
        {
            if (!IsFishBoneActive() && Q.IsReady() && IsSpellActive("Q"))
                Q.Cast();
        }

		public override void OnGapClose(ActiveGapcloser gapcloser)
		{
			if (!Menu.Item("E_Gap_Closer").GetValue<bool>()) return;

			if (E.IsReady() && gapcloser.Sender.IsValidTarget(E.Range))
				E.Cast(gapcloser.Sender, UsePackets());
		}

		private void Cast_E_Escape()
		{
			foreach (
				var unit in
					ObjectManager.Get<Obj_AI_Hero>()
						.Where(x => x.IsValidTarget(E.Range) && !x.IsDead && x.IsEnemy)
						.OrderBy(x => MyHero.Distance(x)))
			{
				var E_Pred = E.GetPrediction(unit);
				var E_Behind_Vec = E_Pred.CastPosition - Vector3.Normalize(unit.ServerPosition - MyHero.ServerPosition) * 150;

				if (E.IsReady())
				    E.Cast(E_Behind_Vec, UsePackets());
			}
		}

        private void Q_Check()
        {
            if (!Q.IsReady())
                return;

            var Q_Range = 525 + (50+25*Q.Level);
            var target = SimpleTs.GetTarget(Q_Range, SimpleTs.DamageType.Physical);

            if (target != null)
            {
                if (!IsFishBoneActive() && MyHero.Distance(target.ServerPosition) < 525 + MyHero.BoundingRadius + target.BoundingRadius)
                {
                    Q.Cast();
                    return;
                }
                if (IsFishBoneActive() && MyHero.Distance(target.ServerPosition) > 25 + MyHero.BoundingRadius + target.BoundingRadius)
                    Q.Cast();
            }
        }

        private void Cast_R()
		{
			var target = SimpleTs.GetTarget(R.Range, SimpleTs.DamageType.Physical);
            if (target != null)
            {
                if (Menu.Item("Dont_R" + target.BaseSkinName) != null)
                {
                    if (!Menu.Item("Dont_R" + target.BaseSkinName).GetValue<bool>())
                    {
                        if (target.IsValidTarget(Q.Range))
                        {
                            if (MyHero.GetSpellDamage(target, SpellSlot.R) > target.Health + 25)
                            {
                                var minRange = Menu.Item("R_Min_Range").GetValue<Slider>().Value;
                                var maxRange = Menu.Item("R_Max_Range").GetValue<Slider>().Value;

                                if (MyHero.Distance(target) > minRange && MyHero.Distance(target) < maxRange)
                                {
                                    if (Menu.Item("R_Overkill_Check").GetValue<bool>())
                                    {
                                        if (MyHero.GetAutoAttackDamage(target)*3 >= target.Health)
                                            return;

                                        R.Cast(target, UsePackets());
                                    }
                                    else
                                    {
                                        R.Cast(target, UsePackets());
                                    }
                                }
                            }
                        }
                    }
                }
            }
		}

		private void Cast_R_Killable()
		{
		    R.Range = 4000;
			foreach (var unit in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsValidTarget(R.Range) && !x.IsDead && x.IsEnemy).OrderBy(x => x.Health))
			{
                if (Menu.Item("Dont_R" + unit.BaseSkinName) != null)
			    {
                    if (!Menu.Item("Dont_R" + unit.BaseSkinName).GetValue<bool>())
			        {
			            var health = unit.Health + unit.HPRegenRate*2 + 25;
			            if (ObjectManager.Player.GetSpellDamage(unit, SpellSlot.R) > health)
			            {
			                R.Cast(unit, UsePackets());
			                return;
			            }
			        }
			    }
			}
		}

		private void Cast_W(bool stun, bool slow)
		{
			if (!W.IsReady())
				return;

			foreach (var unit in AllHerosEnemy.Where(x => x.IsValidTarget(W.Range) && !x.IsDead && x.IsEnemy))
			{
				if (stun && W.GetPrediction(unit).Hitchance == HitChance.Immobile)
				{
					W.Cast(unit.ServerPosition, UsePackets());
					return;
				}

				if (slow && unit.HasBuffOfType(BuffType.Slow))
				{
					W.Cast(unit, UsePackets());
					return;
				}
			}
		}

        private void Cast_E(bool stun, bool slow, bool behind)
        {
            if (!E.IsReady())
                return;

            foreach (var unit in AllHerosEnemy.Where(x => x.IsValidTarget(E.Range) && !x.IsDead && x.IsEnemy))
            {
                if (stun && E.GetPrediction(unit).Hitchance == HitChance.Immobile)
                {
                    E.Cast(unit.ServerPosition, UsePackets());
                    return;
                }

                if (slow && unit.HasBuffOfType(BuffType.Slow))
                {
                    var E_Pred_Slow = E.GetPrediction(unit);
                    var E_Behind_Vec_Slow = E_Pred_Slow.CastPosition +
                                            Vector3.Normalize(unit.ServerPosition - MyHero.ServerPosition)*100;
                    if (MyHero.Distance(E_Behind_Vec_Slow) < E.Range)
                    {
                        E.Cast(E_Behind_Vec_Slow, UsePackets());
                        return;
                    }
                }

                var E_Behind_Distance = Menu.Item("E_Behind_Distance").GetValue<Slider>().Value;
                var E_Pred = E.GetPrediction(unit);
                var E_Behind_Vec = E_Pred.CastPosition +
                                   Vector3.Normalize(E_Pred.CastPosition - MyHero.ServerPosition) * E_Behind_Distance;

                if (E.IsReady() && behind && MyHero.Distance(E_Behind_Vec) < E.Range)
                    E.Cast(E_Behind_Vec);
            }

        }

        private int CountAlliesNearTarget(Obj_AI_Base target, float range)
		{
			return
				ObjectManager.Get<Obj_AI_Hero>()
					.Count(
						hero =>
							hero.Team == ObjectManager.Player.Team &&
							hero.ServerPosition.Distance(target.ServerPosition) <= range);
		}

		private bool IsFishBoneActive()
		{
			return MyHero.AttackRange < 525 + MyHero.BoundingRadius;
		}
    }

}