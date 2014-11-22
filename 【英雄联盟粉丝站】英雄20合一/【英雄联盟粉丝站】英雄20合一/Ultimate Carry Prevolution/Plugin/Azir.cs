using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using xSLx_Orbwalker;
using Color = System.Drawing.Color;

namespace Ultimate_Carry_Prevolution.Plugin
{
	class Azir : Champion
	{
		public Azir()
		{
			SetSpells();
			LoadMenu();
		}

		//spells
		private Spell _qExtend;
		private static readonly Spellbook SpellBook = ObjectManager.Player.Spellbook;
		private static readonly SpellDataInst QSpell = SpellBook.GetSpell(SpellSlot.Q);
		private static readonly SpellDataInst ESpell = SpellBook.GetSpell(SpellSlot.E);

		//Misc_Insec
		private Vector3 _rVec;

		private void SetSpells()
		{
			Q = new Spell(SpellSlot.Q, 850);
			Q.SetSkillshot(0.1f, 100, 1700, false, SkillshotType.SkillshotLine);

			_qExtend = new Spell(SpellSlot.Q, 1150);
			_qExtend.SetSkillshot(0.1f, 100, 1700, false, SkillshotType.SkillshotLine);

			W = new Spell(SpellSlot.W, 450);
			W.SetSkillshot(.25f, 400, float.MaxValue, false, SkillshotType.SkillshotCircle);

			E = new Spell(SpellSlot.E, 2000);
			E.SetSkillshot(0.25f, 100, 1200, false, SkillshotType.SkillshotLine);

			R = new Spell(SpellSlot.R, 450);
			R.SetSkillshot(0.5f, 700, 1400, false, SkillshotType.SkillshotLine);
		}

		private void LoadMenu()
		{
			var champMenu = new Menu("Azir Plugin", "Azir");
			{
				var spellMenu = new Menu("娉曟湳", "SpellMenu");
				{
					//Q Menu
					spellMenu.AddSubMenu(new Menu("Q", "QSpell"));
					spellMenu.SubMenu("QSpell").AddItem(new MenuItem("Q_Out_Of_Range", "瓒呭嚭鑼冨洿浣跨敤").SetValue(true));
					spellMenu.SubMenu("QSpell").AddItem(new MenuItem("Use_Q_Extend", "浣跨敤max Q").SetValue(true));
					spellMenu.SubMenu("QSpell").AddItem(new MenuItem("Q_Behind_Target", "鐩爣韬悗").SetValue(true));
					spellMenu.SubMenu("QSpell").AddItem(new MenuItem("Q_If_Multi_Slave", "澶氫釜澹叺").SetValue(true));
					spellMenu.SubMenu("QSpell").AddItem(new MenuItem("Q_Hitchance", "Q鍑讳腑").SetValue(new Slider(3, 1, 3)));
					//W Menu
					spellMenu.AddSubMenu(new Menu("W", "WSpell"));
					spellMenu.SubMenu("WSpell").AddItem(new MenuItem("AutoAA_W_Range", "鑷姩鏀诲嚮").SetValue(true));
					spellMenu.SubMenu("WSpell").AddItem(new MenuItem("Use_W_Q_Poke", "浣跨敤WQ").SetValue(true));
					spellMenu.SubMenu("WSpell").AddItem(new MenuItem("Use_W_Tower", "鑷姩W濉攟").SetValue(true));
					//E Menu
					spellMenu.AddSubMenu(new Menu("E", "ESpell"));
					spellMenu.SubMenu("ESpell").AddItem(new MenuItem("Use_E_As_Gapcloser", "瓒呭嚭Q鑼冨洿").SetValue(false));
					spellMenu.SubMenu("ESpell").AddItem(new MenuItem("Use_E_If_Killable", "鍙潃").SetValue(true));
					spellMenu.SubMenu("ESpell").AddItem(new MenuItem("Always_E_To_Knockup", "E鍑婚").SetValue(false));
					spellMenu.SubMenu("ESpell").AddItem(new MenuItem("E_If_HP_Above", "HP >").SetValue(new Slider(70)));
					//R Menu
					spellMenu.AddSubMenu(new Menu("R", "RSpell"));
					spellMenu.SubMenu("RSpell").AddItem(new MenuItem("R_If_Hp_Below", "HP <").SetValue(new Slider(20)));
					spellMenu.SubMenu("RSpell").AddItem(new MenuItem("R_If_Hit", "鍑讳腑>=").SetValue(new Slider(3, 0, 5)));
					spellMenu.SubMenu("RSpell").AddItem(new MenuItem("R_Enemy_Into_Wall", "R鏁屼汉鍒板").SetValue(true));

					champMenu.AddSubMenu(spellMenu);
				}

				var comboMenu = new Menu("杩炴嫑", "Combo");
				{
					AddSpelltoMenu(comboMenu, "Q", true);
					AddSpelltoMenu(comboMenu, "W", true);
					AddSpelltoMenu(comboMenu, "E", true);
					AddSpelltoMenu(comboMenu, "R", true);
					champMenu.AddSubMenu(comboMenu);
				}
				var harassMenu = new Menu("楠氭壈", "Harass");
				{
					AddSpelltoMenu(harassMenu, "Q", true);
					AddSpelltoMenu(harassMenu, "W", true);
					AddSpelltoMenu(harassMenu, "E", true);
					AddManaManagertoMenu(harassMenu, 30);
					champMenu.AddSubMenu(harassMenu);
				}
				var laneClearMenu = new Menu("娓呯嚎", "LaneClear");
				{
					AddSpelltoMenu(laneClearMenu, "Q", true);
					AddSpelltoMenu(laneClearMenu, "W", true);
					AddManaManagertoMenu(laneClearMenu, 20);
					champMenu.AddSubMenu(laneClearMenu);
				}

				var miscMenu = new Menu("鏉傞」", "Misc");
				{
					miscMenu.AddItem(new MenuItem("Misc_useER_Interrupt", "浣跨敤 E/R 鎵撴柇").SetValue(true));
					miscMenu.AddItem(new MenuItem("Misc_Escape", "閫冭窇").SetValue(new KeyBind("Z".ToCharArray()[0], KeyBindType.Press)));
					miscMenu.AddItem(new MenuItem("Misc_Insec", "鐩爣閫夋嫨").SetValue(new KeyBind("J".ToCharArray()[0], KeyBindType.Press)));
					miscMenu.AddItem(new MenuItem("Misc_QE_Combo", "Q->E").SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));
					champMenu.AddSubMenu(miscMenu);
				}
				var drawMenu = new Menu("鑼冨洿", "Drawing");
				{
					drawMenu.AddItem(new MenuItem("Draw_Disabled", "绂佺敤").SetValue(false));
					drawMenu.AddItem(new MenuItem("Draw_Q", "Q鑼冨洿").SetValue(true));
					drawMenu.AddItem(new MenuItem("Draw_QExtend", "Q Max鑼冨洿").SetValue(true));
					drawMenu.AddItem(new MenuItem("Draw_W", "W鑼冨洿").SetValue(true));
					drawMenu.AddItem(new MenuItem("Draw_E", "E鑼冨洿").SetValue(true));
					drawMenu.AddItem(new MenuItem("Draw_R", "R鑼冨洿").SetValue(true));

					var drawComboDamageMenu = new MenuItem("Draw_ComboDamage", "鏄剧ず浼ゅ").SetValue(true);
					drawMenu.AddItem(drawComboDamageMenu);
					Utility.HpBarDamageIndicator.DamageToUnit = GetComboDamage;
					Utility.HpBarDamageIndicator.Enabled = drawComboDamageMenu.GetValue<bool>();
					drawComboDamageMenu.ValueChanged += delegate(object sender, OnValueChangeEventArgs eventArgs)
					{
						Utility.HpBarDamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
					};

					champMenu.AddSubMenu(drawMenu);
				}
			}

			Menu.AddSubMenu(champMenu);
			Menu.AddToMainMenu();

		}
		IEnumerable<SpellSlot> GetSpellCombo()
		{
			var spellCombo = new List<SpellSlot>();
			if(Q.IsReady())
				spellCombo.Add(SpellSlot.Q);
			if(E.IsReady())
				spellCombo.Add(SpellSlot.E);
			if(R.IsReady())
				spellCombo.Add(SpellSlot.R);
			return spellCombo;
		}

		private float GetComboDamage(Obj_AI_Base target)
		{
			double comboDamage = (float)MyHero.GetComboDamage(target, GetSpellCombo());

			//add in soilder atk
			if(soilderCount() >= 0 || W.IsReady())
				comboDamage += xSLxOrbwalker.GetAzirAASandwarriorDamage(target) * 2;

			return (float)(comboDamage + MyHero.GetAutoAttackDamage(target));
		}

		public override void OnDraw()
		{
			if(Menu.Item("Draw_Disabled").GetValue<bool>())
			{
				xSLxOrbwalker.DisableDrawing();
				return;
			}
			xSLxOrbwalker.EnableDrawing();
			if(Menu.Item("Draw_Q").GetValue<bool>())
				if(Q.Level > 0)
					Utility.DrawCircle(MyHero.Position, Q.Range - 2, Q.IsReady() ? Color.Green : Color.Red);
			if(Menu.Item("Draw_QExtend").GetValue<bool>())
				if(Q.Level > 0)
					Utility.DrawCircle(MyHero.Position, _qExtend.Range - 2, Q.IsReady() ? Color.Green : Color.Red);
			if(Menu.Item("Draw_W").GetValue<bool>())
				if(W.Level > 0)
					Utility.DrawCircle(MyHero.Position, W.Range - 2, W.IsReady() ? Color.Green : Color.Red);

			if(Menu.Item("Draw_E").GetValue<bool>())
				if(E.Level > 0)
					Utility.DrawCircle(MyHero.Position, E.Range, E.IsReady() ? Color.Green : Color.Red);

			if(Menu.Item("Draw_R").GetValue<bool>())
				if(R.Level > 0)
					Utility.DrawCircle(MyHero.Position, R.Range, R.IsReady() ? Color.Green : Color.Red);
		}

		public override void OnPassive()
		{
			if(Menu.Item("Misc_Escape").GetValue<KeyBind>().Active)
			{
				Misc_Escape();
				return;
			}

			if(Menu.Item("Misc_Insec").GetValue<KeyBind>().Active)
			{
				xSLxOrbwalker.Orbwalk(Game.CursorPos, null);
				Misc_Insec();

				return;
			}

			if(Menu.Item("Misc_QE_Combo").GetValue<KeyBind>().Active)
			{
				var soilderTarget = SimpleTs.GetTarget(900, SimpleTs.DamageType.Magical);

				xSLxOrbwalker.Orbwalk(Game.CursorPos, null);
				Cast_QE(soilderTarget);

				return;
			}

			if(Menu.Item("AutoAA_W_Range").GetValue<bool>())
				Auto_Attack_Enemy();

			if (!Menu.Item("Use_W_Tower").GetValue<bool>()) return;
			IEnumerable<Obj_AI_Turret> Tower;
			Tower = ObjectManager.Get<Obj_AI_Turret>().Where(tower => tower.IsEnemy && tower.Health > 0 && tower.Position.Distance(MyHero.ServerPosition) < W.Range);

			var objAiTurrets = Tower as Obj_AI_Turret[] ?? Tower.ToArray();
			if(objAiTurrets.FirstOrDefault() != null && MyHero.Spellbook.GetSpell(SpellSlot.W).Ammo > 1)
				W.Cast(objAiTurrets.FirstOrDefault(), UsePackets());
		}

		public override void OnCombo()
		{
			var target = SimpleTs.GetTarget(_qExtend.Range, SimpleTs.DamageType.Magical);
			var soilderTarget = SimpleTs.GetTarget(1000, SimpleTs.DamageType.Magical);

			if(IsSpellActive("R") && Should_R(target) && MyHero.Distance(target) < R.Range)
				Cast_BasicSkillshot_Enemy(R);

			//WQ
			if(IsSpellActive("W") && IsSpellActive("Q") && soilderCount() == 0 && Menu.Item("Use_W_Q_Poke").GetValue<bool>())
			{
				Cast_WQ(target);
			}

			//W
			if(IsSpellActive("W") && W.IsReady())
			{
				Cast_W(target);
			}

			//Q
			if(IsSpellActive("Q") && Q.IsReady())
			{
				Cast_Q(target);
				return;
			}

			//E
			if(IsSpellActive("E"))
			{
				Cast_E(soilderTarget);
			}
		}

		public override void OnHarass()
		{
			if(ManaManagerAllowCast())
			{
				var target = SimpleTs.GetTarget(_qExtend.Range, SimpleTs.DamageType.Magical);
				var soilderTarget = SimpleTs.GetTarget(1000, SimpleTs.DamageType.Magical);

				//WQ
				if(IsSpellActive("W") && IsSpellActive("Q") && soilderCount() == 0 &&
					Menu.Item("Use_W_Q_Poke").GetValue<bool>())
				{
					Cast_WQ(target);
				}

				//W
				if(IsSpellActive("W") && W.IsReady())
				{
					Cast_W(target);
				}

				//Q
				if(IsSpellActive("Q") && Q.IsReady())
				{
					Cast_Q(target);
					return;
				}

				//E
				if(IsSpellActive("E"))
				{
					Cast_E(soilderTarget);
				}
			}
		}

		public override void OnLaneClear()
		{
			if(!ManaManagerAllowCast())
				return;

			var allMinionsQ = MinionManager.GetMinions(MyHero.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.NotAlly);
			var allMinionsW = MinionManager.GetMinions(MyHero.ServerPosition, W.Range, MinionTypes.All, MinionTeam.NotAlly);

			if(soilderCount() > 0)
			{
				var slaves = (from obj in ObjectManager.Get<Obj_AI_Base>()
							  where obj.Name == "AzirSoldier" && obj.IsAlly && MyHero.Distance(obj.ServerPosition) < 2000
							  select obj).ToList();
				foreach(var slave in slaves)
				{
					Q.UpdateSourcePosition(slave.ServerPosition, slave.ServerPosition);
					var qpred = Q.GetCircularFarmLocation(allMinionsQ);

					if(qpred.MinionsHit > 2 && MyHero.Distance(qpred.Position) < Q.Range && IsSpellActive("Q"))
						Q.Cast(qpred.Position, UsePackets());
				}
			}
			else if(W.IsReady())
			{
				var wpred = W.GetCircularFarmLocation(allMinionsW);
				if(wpred.MinionsHit > 0 && MyHero.Distance(wpred.Position) < W.Range && IsSpellActive("W"))
					W.Cast(wpred.Position);
			}
		}

		public override void OnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
		{
			if(spell.DangerLevel < InterruptableDangerLevel.Medium || unit.IsAlly || !Menu.Item("Misc_useER_Interrupt").GetValue<bool>())
				return;

			if(unit.IsValidTarget(E.Range))
			{
				Cast_E((Obj_AI_Hero)unit);
				return;
			}

			if(unit.IsValidTarget(R.Range))
				R.Cast(unit);
		}

		private void Misc_Escape()
		{
			Vector3 wVec = MyHero.ServerPosition + Vector3.Normalize(Game.CursorPos - MyHero.ServerPosition) * 450;

			if(W.IsReady() || soilderCount() > 0)
			{
				if((E.IsReady() || ESpell.State == SpellState.Surpressed))
				{
					W.Cast(wVec);
					W.LastCastAttemptT = Environment.TickCount;
				}

				if((_qExtend.IsReady() || QSpell.State == SpellState.Surpressed) &&
					((Environment.TickCount - E.LastCastAttemptT < Game.Ping + 500 &&
						Environment.TickCount - E.LastCastAttemptT > Game.Ping + 50) || E.IsReady()))
				{
					if(Environment.TickCount - W.LastCastAttemptT > Game.Ping + 300 || ESpell.State == SpellState.Cooldown || !W.IsReady())
					{
						Vector3 qVec = MyHero.ServerPosition +
										Vector3.Normalize(Game.CursorPos - MyHero.ServerPosition) * 800;

						var lastAttempt = (int)qVec.Distance(Get_Nearest_Soilder_To_Mouse().ServerPosition) / 1000;

						Q.Cast(qVec, UsePackets());
						Q.LastCastAttemptT = Environment.TickCount + lastAttempt;
						return;
					}
				}

				if((E.IsReady() || ESpell.State == SpellState.Surpressed))
				{
					if(MyHero.Distance(Game.CursorPos) > Get_Nearest_Soilder_To_Mouse().Distance(Game.CursorPos) && Environment.TickCount - Q.LastCastAttemptT > Game.Ping)
					{
						E.Cast(Get_Nearest_Soilder_To_Mouse().ServerPosition, UsePackets());
						E.LastCastAttemptT = Environment.TickCount - 250;
						return;
					}
					if(Environment.TickCount - W.LastCastAttemptT < Game.Ping + 300 && (Q.IsReady() || QSpell.State == SpellState.Surpressed))
					{
						E.Cast(wVec, UsePackets());
						E.LastCastAttemptT = Environment.TickCount - 250;
					}
				}
			}

		}
		private void Cast_QE(Obj_AI_Hero target)
		{
			if(soilderCount() > 0)
			{
				if((Q.IsReady() || QSpell.State == SpellState.Surpressed) && E.IsReady())
				{
					var slaves = (from obj in ObjectManager.Get<Obj_AI_Base>()
								  where obj.Name == "AzirSoldier" && obj.IsAlly && target.Distance(obj.ServerPosition) < 2000
								  select obj).ToList();

					foreach(var slave in slaves)
					{
						if(target != null && MyHero.Distance(target) < 800)
						{
							Q.UpdateSourcePosition(slave.ServerPosition, slave.ServerPosition);
							var qPred = Q.GetPrediction(target);

							if(Q.IsReady() && MyHero.Distance(target) < 800 && qPred.Hitchance >= Get_Q_Hitchance())
							{
								var vec = target.ServerPosition - MyHero.ServerPosition;
								var castBehind = qPred.CastPosition + Vector3.Normalize(vec) * 75;

								Q.Cast(castBehind, UsePackets());
								E.Cast(slave.ServerPosition, UsePackets());
								return;

							}
						}
					}
				}
			}
			else if(W.IsReady())
			{
				var wVec = MyHero.ServerPosition + Vector3.Normalize(target.ServerPosition - MyHero.ServerPosition) * 450;

				Q.UpdateSourcePosition(wVec, wVec);
				var qPred = Q.GetPrediction(target);

				if((Q.IsReady() || QSpell.State == SpellState.Surpressed) && (E.IsReady() || ESpell.State == SpellState.Surpressed) && MyHero.Distance(target) < 800 && qPred.Hitchance >= HitChance.High)
				{
					var vec = target.ServerPosition - MyHero.ServerPosition;
					var castBehind = qPred.CastPosition + Vector3.Normalize(vec) * 75;

					W.Cast(wVec);
					_qExtend.Cast(castBehind, UsePackets());
					Utility.DelayAction.Add(1, () => E.Cast(getNearestSoilderToEnemy(target).ServerPosition, UsePackets()));
				}
			}
		}

		private void Misc_Insec()
		{
			Obj_AI_Hero target = null;

			if(SimpleTs.GetSelectedTarget() != null)
                target = SimpleTs.GetSelectedTarget();

			if(target == null || !target.IsEnemy || target.Type != GameObjectType.obj_AI_Hero)
				return;

			if(soilderCount() > 0)
			{
				if((Q.IsReady() || QSpell.State == SpellState.Surpressed) && E.IsReady())
				{
					var slaves = (from obj in ObjectManager.Get<Obj_AI_Base>()
								  where obj.Name == "AzirSoldier" && obj.IsAlly && target.Distance(obj.ServerPosition) < 2000
								  select obj).ToList();

					foreach(var slave in slaves.Where(slave => MyHero.Distance(target) < 800))
					{
						Q.UpdateSourcePosition(slave.ServerPosition, slave.ServerPosition);
						var qPred = Q.GetPrediction(target);
						var vec = target.ServerPosition - MyHero.ServerPosition;
						var castBehind = qPred.CastPosition + Vector3.Normalize(vec) * 75;
						_rVec = qPred.CastPosition - Vector3.Normalize(vec) * 300;

						if(!Q.IsReady() || (!E.IsReady() && ESpell.State != SpellState.Surpressed) || !R.IsReady() ||
							qPred.Hitchance < Get_Q_Hitchance())
							continue;
						Q.Cast(castBehind, UsePackets());
						E.Cast(slave.ServerPosition, UsePackets());
						E.LastCastAttemptT = Environment.TickCount;
					}
				}
				if(!R.IsReady())
					return;
				if(MyHero.Distance(target) < 200 && Environment.TickCount - E.LastCastAttemptT > Game.Ping + 150)
					R.Cast(_rVec);
			}
			else if(W.IsReady())
			{
				var wVec = MyHero.ServerPosition + Vector3.Normalize(target.ServerPosition - MyHero.ServerPosition) * 450;

				Q.UpdateSourcePosition(wVec, wVec);
				var qPred = Q.GetPrediction(target);

				if((Q.IsReady() || QSpell.State == SpellState.Surpressed) && (E.IsReady() || ESpell.State == SpellState.Surpressed)
					&& R.IsReady() && MyHero.Distance(target) < 800 && qPred.Hitchance >= HitChance.High)
				{
					var vec = target.ServerPosition - MyHero.ServerPosition;
					var castBehind = qPred.CastPosition + Vector3.Normalize(vec) * 75;
					_rVec = MyHero.Position;

					W.Cast(wVec);
					_qExtend.Cast(castBehind, UsePackets());
					E.Cast(getNearestSoilderToEnemy(target).ServerPosition, UsePackets());
				}
				if(!R.IsReady())
					return;
				if(MyHero.Distance(target) < 200 && Environment.TickCount - E.LastCastAttemptT > Game.Ping + 150)
				{
					R.Cast(_rVec);
				}
			}
		}

		private void Cast_WQ(Obj_AI_Base target)
		{
			if(!(MyHero.Distance(target) < 1150) || !(MyHero.Distance(target) > 450))
				return;
			if(!W.IsReady() || (!Q.IsReady() && QSpell.State != SpellState.Surpressed))
				return;
			var wVec = MyHero.ServerPosition + Vector3.Normalize(target.ServerPosition - MyHero.ServerPosition) * 450;

			Q.UpdateSourcePosition(wVec, wVec);
			var qPred = Q.GetPrediction(target);

			if(qPred.Hitchance < HitChance.High)
				return;
			W.Cast(wVec);
			_qExtend.Cast(qPred.CastPosition, UsePackets());
		}

		private void Cast_W(Obj_AI_Base target)
		{
			if(MyHero.Distance(target) < 1200)
			{
				if(MyHero.Distance(target) < 450)
				{
					//Game.PrintChat("W Cast1");
					W.Cast(target);
					if(CanAttack())
						MyHero.IssueOrder(GameObjectOrder.AttackUnit, target);
				}
				else if(MyHero.Distance(target) < 600)
				{
					Vector3 wVec = MyHero.ServerPosition + Vector3.Normalize(target.ServerPosition - MyHero.ServerPosition) * 450;

					//Game.PrintChat("W Cast2");
					if(W.IsReady())
					{
						W.Cast(wVec);
						if(CanAttack())
							MyHero.IssueOrder(GameObjectOrder.AttackUnit, target);
					}
				}
				else if(MyHero.Distance(target) < 950)
				{
					Vector3 wVec = MyHero.ServerPosition + Vector3.Normalize(target.ServerPosition - MyHero.ServerPosition) * 450;

					//Game.PrintChat("W Cast2");
					if(W.IsReady() && (Q.IsReady() || QSpell.State == SpellState.Surpressed))
					{
						Q.UpdateSourcePosition(wVec, wVec);
						var qPred = Q.GetPrediction(target);

						if(qPred.Hitchance >= HitChance.High)
						{
							W.Cast(wVec);
						}
					}
				}
			}
		}

		private void Cast_Q(Obj_AI_Hero target)
		{
			if(soilderCount() < 1)
				return;

			var slaves = (from obj in ObjectManager.Get<Obj_AI_Base>()
						  where obj.Name == "AzirSoldier" && obj.IsAlly && target.Distance(obj.ServerPosition) < 2000
						  select obj).ToList();

			foreach(var slave in slaves)
			{
				if(target != null && MyHero.Distance(target) < _qExtend.Range && Should_Q(target, slave))
				{

					Q.UpdateSourcePosition(slave.ServerPosition, slave.ServerPosition);
					var qPred = Q.GetPrediction(target);

					if(Q.IsReady() && MyHero.Distance(target) < 800 && qPred.Hitchance >= Get_Q_Hitchance())
					{
						if(Menu.Item("Q_Behind_Target").GetValue<bool>())
						{
							var vec = target.ServerPosition - MyHero.ServerPosition;
							var castBehind = qPred.CastPosition + Vector3.Normalize(vec) * 75;

							Q.Cast(castBehind, UsePackets());
							return;
						}
						Q.Cast(qPred.CastPosition, UsePackets());
						return;
					}
					if (!Q.IsReady() || !(MyHero.Distance(target) > 800) || qPred.Hitchance < HitChance.High ||
					    !Menu.Item("Use_Q_Extend").GetValue<bool>()) continue;
					var qVector = MyHero.ServerPosition + Vector3.Normalize(qPred.CastPosition - MyHero.ServerPosition) * 800;

					//Game.PrintChat("QHarass");
					_qExtend.Cast(qVector, UsePackets());
					return;
				}
			}
		}

		private void Cast_E(Obj_AI_Hero target)
		{
			if(soilderCount() < 1)
				return;

			var slaves = (from obj in ObjectManager.Get<Obj_AI_Base>()
						  where obj.Name == "AzirSoldier" && obj.IsAlly && target.Distance(obj.ServerPosition) < 2000
						  select obj).ToList();

			if(MyHero.Distance(target) > 1200 && Menu.Item("Use_E_As_Gapcloser").GetValue<bool>())
			{
				var slavetar = getNearestSoilderToEnemy(target);
				if(slavetar != null && slavetar.Distance(target) < MyHero.Distance(target))
				{
					E.Cast(slavetar, UsePackets());
				}
			}

			foreach(var slave in slaves)
			{
				if(target != null && MyHero.Distance(slave) < E.Range)
				{
					Q.UpdateSourcePosition(slave.ServerPosition, slave.ServerPosition);
					var ePred = E.GetPrediction(target);
					var obj = VectorPointProjectionOnLineSegment(MyHero.ServerPosition.To2D(), slave.ServerPosition.To2D(), ePred.UnitPosition.To2D());
					var isOnseg = (bool)obj[2];
					var pointLine = (Vector2)obj[1];

					if (!E.IsReady() || !isOnseg || !(pointLine.Distance(ePred.UnitPosition.To2D()) < E.Width) ||
					    !Should_E(target)) continue;
					E.Cast(slave.ServerPosition, UsePackets());
					return;
				}
			}
		}

		private bool Should_Q(Obj_AI_Base target, Obj_AI_Base slave)
		{
			if(!Menu.Item("Q_Out_Of_Range").GetValue<bool>())
				return true;

			if(slave.Distance(target.ServerPosition) > 390)
				return true;

			if(soilderCount() > 1 && Menu.Item("Q_If_Multi_Slave").GetValue<bool>())
				return true;

			if(MyHero.GetSpellDamage(target, SpellSlot.Q) > target.Health + 10)
				return true;


			return false;
		}
		private bool Should_E(Obj_AI_Base target)
		{
			if(Menu.Item("Always_E_To_Knockup").GetValue<bool>())
				return true;

			if(Menu.Item("Use_E_If_Killable").GetValue<bool>() && GetComboDamage(target) > target.Health + 15)
				return true;

			if(Menu.Item("eKS").GetValue<bool>() && MyHero.GetSpellDamage(target, SpellSlot.E) > target.Health + 10)
				return true;

			//hp 
			var hp = Menu.Item("E_If_HP_Above").GetValue<Slider>().Value;
			var hpPercent = MyHero.Health / MyHero.MaxHealth * 100;

			if(hpPercent > hp)
				return true;

			return false;
		}

		private bool Should_R(Obj_AI_Hero target)
		{
			if(MyHero.GetSpellDamage(target, SpellSlot.R) > target.Health + 10)
				return true;

			var hp = Menu.Item("R_If_Hp_Below").GetValue<Slider>().Value;
			var hpPercent = MyHero.Health / MyHero.MaxHealth * 100;
			if(hpPercent < hp)
				return true;

			var rIfHit = Menu.Item("R_If_Hit").GetValue<Slider>().Value;
			var pred = R.GetPrediction(target);
			if(pred.AoeTargetsHitCount >= rIfHit)
				return true;

			if(WallStun(target) && GetComboDamage(target) > target.Health / 2 && Menu.Item("R_Enemy_Into_Wall").GetValue<bool>())
			{
				//Game.PrintChat("Walled");
				return true;
			}

			return false;
		}

		public static HitChance Get_Q_Hitchance()
		{
			var hitC = HitChance.High;
			var qHitchance = Menu.Item("Q_Hitchance").GetValue<Slider>().Value;

			// HitChance.Low = 3, Medium , High .... etc..
			switch(qHitchance)
			{
				case 1:
					hitC = HitChance.Low;
					break;
				case 2:
					hitC = HitChance.Medium;
					break;
				case 3:
					hitC = HitChance.High;
					break;
				case 4:
					hitC = HitChance.VeryHigh;
					break;
			}

			return hitC;
		}

		private void Auto_Attack_Enemy()
		{

			if(soilderCount() < 1)
				return;

			var soilderTarget = SimpleTs.GetTarget(800, SimpleTs.DamageType.Magical);

			AttackTarget(soilderTarget);
		}

		private void AttackTarget(Obj_AI_Base target)
		{
			if(soilderCount() < 1)
				return;

			var tar = getNearestSoilderToEnemy(target);
			if(tar != null && MyHero.Distance(tar) < 800)
			{
				if(target != null && tar.Distance(target) <= 390 && CanAttack())
				{
					xSLxOrbwalker.Orbwalk(Game.CursorPos, target);
				}
			}

		}
		private Obj_AI_Base Get_Nearest_Soilder_To_Mouse()
		{

			// xSalice this need a remaake couse your funktions in it are "not used"
			var soilder = (from obj in ObjectManager.Get<Obj_AI_Base>()
						   where obj.Name == "AzirSoldier" && obj.IsAlly && MyHero.Distance(obj.ServerPosition) < 2000
						   select obj).ToList();

			return soilder.FirstOrDefault();
		}

		private Obj_AI_Base getNearestSoilderToEnemy(Obj_AI_Base target)
		{
			//here the same
			var soilder = (from obj in ObjectManager.Get<Obj_AI_Base>()
						   where obj.Name == "AzirSoldier" && obj.IsAlly && target.Distance(obj.ServerPosition) < 2000
						   select obj).ToList();

			return soilder.FirstOrDefault();
		}

		private int soilderCount()
		{
			return ObjectManager.Get<Obj_AI_Base>().Count(obj => obj.Name == "AzirSoldier" && obj.IsAlly);
		}

		private bool CanAttack()
		{
			return xSLxOrbwalker.CanAttack();
		}

		private bool WallStun(Obj_AI_Hero target)
		{
			var pred = R.GetPrediction(target);

			var pushedPos = pred.CastPosition + Vector3.Normalize(pred.CastPosition - MyHero.ServerPosition) * 200;

			return IsWall(pushedPos.To2D());
		}

		private Object[] VectorPointProjectionOnLineSegment(Vector2 v1, Vector2 v2, Vector2 v3)
		{
			var cx = v3.X;
			var cy = v3.Y;
			var ax = v1.X;
			var ay = v1.Y;
			var bx = v2.X;
			var by = v2.Y;
			var rL = ((cx - ax) * (bx - ax) + (cy - ay) * (by - ay)) /
					   ((float)Math.Pow(bx - ax, 2) + (float)Math.Pow(by - ay, 2));
			var pointLine = new Vector2(ax + rL * (bx - ax), ay + rL * (by - ay));
			float rS;
			if(rL < 0)
			{
				rS = 0;
			}
			else if(rL > 1)
			{
				rS = 1;
			}
			else
			{
				rS = rL;
			}
			var isOnSegment = rS.CompareTo(rL) == 0;
			var pointSegment = isOnSegment ? pointLine : new Vector2(ax + rS * (bx - ax), ay + rS * (@by - ay));
			return new object[] { pointSegment, pointLine, isOnSegment };
		}
	}
}
