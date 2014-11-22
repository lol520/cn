using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using xSLx_Orbwalker;
using Color = System.Drawing.Color;

namespace Ultimate_Carry_Prevolution.Plugin
{
	class Anivia : Champion
	{
		private GameObject _qShot;
		private GameObject _rlocation;
		public Anivia()
		{
			SetSpells();
			LoadMenu();
		}

		private void SetSpells()
		{
			Q = new Spell(SpellSlot.Q, 1000);
			Q.SetSkillshot(500, 110, 750, false, SkillshotType.SkillshotLine);

			W = new Spell(SpellSlot.W, 950);
			W.SetSkillshot(250, 1, float.MaxValue, false, SkillshotType.SkillshotLine);

			E = new Spell(SpellSlot.E, 650);

			R = new Spell(SpellSlot.R, 625);
			R.SetSkillshot(250, 200, float.MaxValue, false, SkillshotType.SkillshotCircle);
		}

		private void LoadMenu()
		{
			var champMenu = new Menu("Anivia Plugin", "Anivia");
			{
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
					AddSpelltoMenu(harassMenu, "R", true);
					AddManaManagertoMenu(harassMenu, 30);
					champMenu.AddSubMenu(harassMenu);
				}
				var laneClearMenu = new Menu("娓呯嚎", "LaneClear");
				{
					AddSpelltoMenu(laneClearMenu, "Q", true);
					AddSpelltoMenu(laneClearMenu, "E", true);
					AddSpelltoMenu(laneClearMenu, "R", true);
					AddManaManagertoMenu(laneClearMenu, 20);
					champMenu.AddSubMenu(laneClearMenu);
				}

				var miscMenu = new Menu("鏉傞」", "Misc");
				{
					miscMenu.AddItem(new MenuItem("Q_AutoDetonate", "鑷姩Q鐖嗙偢").SetValue(true));
					miscMenu.AddItem(new MenuItem("Q_Interrupt", "浣跨敤Q鎵撴柇").SetValue(true));
					miscMenu.AddItem(new MenuItem("W_AntiGapClose", "浣跨敤W闃茬獊").SetValue(true));
					miscMenu.AddItem(new MenuItem("W_Interrupt", "浣跨敤W鎵撴柇").SetValue(true));
					miscMenu.AddItem(new MenuItem("W_killable", "鍙潃浣跨敤W").SetValue(true));
					miscMenu.AddItem(new MenuItem("R_ActiveCheck", "鍏抽棴鑷姩R").SetValue(true));
					champMenu.AddSubMenu(miscMenu);
				}
				var drawMenu = new Menu("鑼冨洿", "Drawing");
				{
					drawMenu.AddItem(new MenuItem("Draw_Disabled", "绂佺敤").SetValue(false));
					drawMenu.AddItem(new MenuItem("Draw_Q", "Q鑼冨洿").SetValue(true));
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

		private float GetComboDamage(Obj_AI_Base enemy)
		{
			var damage = 0d;

			if(Q.IsReady())
				damage += MyHero.GetSpellDamage(enemy, SpellSlot.Q);

			if(E.IsReady() & (Q.IsReady() || R.IsReady()))
				damage += MyHero.GetSpellDamage(enemy, SpellSlot.E) * 2;
			else if(E.IsReady())
				damage += MyHero.GetSpellDamage(enemy, SpellSlot.E);

			if(R.IsReady())
				damage += MyHero.GetSpellDamage(enemy, SpellSlot.R) * 3;

			return (float)damage;
		}
		public override void OnGapClose(ActiveGapcloser gapcloser)
		{
			if(!Menu.Item("W_AntiGapClose").GetValue<bool>())
				return;

			if(!W.IsReady() || !gapcloser.Sender.IsValidTarget(W.Range))
				return;
			var vec = MyHero.ServerPosition - Vector3.Normalize(MyHero.ServerPosition - gapcloser.Sender.ServerPosition) * 1;
			W.Cast(vec, UsePackets());
		}

		public override void OnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
		{
			if(Menu.Item("Q_Interrupt").GetValue<bool>())
				if(MyHero.Distance(unit) < Q.Range && unit != null)
					if(Q.GetPrediction(unit).Hitchance >= HitChance.Medium && Q.IsReady())
						Q.Cast(unit, UsePackets());
			if(!Menu.Item("W_Interrupt").GetValue<bool>())
				return;
			if(MyHero.Distance(unit) < W.Range && unit != null && W.IsReady())
				W.Cast(unit, UsePackets());
		}

		public override void ObjSpellMissileOnOnCreate(GameObject sender, EventArgs args)
		{
			if(sender.Name.Contains("cryo_storm_"))
				_rlocation = sender;
			if(sender.Name != "cryo_FlashFrost_Player_mis.troy")
				return;
			_qShot = sender;
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
					Utility.DrawCircle(MyHero.Position, Q.Range, Q.IsReady() ? Color.Green : Color.Red);

			if(Menu.Item("Draw_W").GetValue<bool>())
				if(W.Level > 0)
					Utility.DrawCircle(MyHero.Position, W.Range - 2, W.IsReady() ? Color.Green : Color.Red);

			if(Menu.Item("Draw_E").GetValue<bool>())
				if(E.Level > 0)
					Utility.DrawCircle(MyHero.Position, E.Range, E.IsReady() ? Color.Green : Color.Red);

			if(Menu.Item("Draw_R").GetValue<bool>())
				if(R.Level > 0)
					Utility.DrawCircle(MyHero.Position, R.Range, R.IsReady() ? Color.Green : Color.Red);

			if(_qShot != null && _qShot.IsValid)
				Utility.DrawCircle(_qShot.Position, 190, Color.Red);

			if(_rlocation != null && _rlocation.IsValid)
				Utility.DrawCircle(_rlocation.Position, 400, Color.Red);

		}
		public override void OnPassive()
		{
			CheckQDetonation();
			CheckRHitSomeone();
			SetupWWidth();
		}

		public override void OnCombo()
		{
			if(IsSpellActive("Q"))
				Cast_Q(true);
			if(IsSpellActive("R"))
				Cast_R(true);
			if(IsSpellActive("E"))
				Cast_E(true);
			if(IsSpellActive("W"))
				Cast_W();
		}

		public override void OnHarass()
		{
			if(IsSpellActive("Q") && ManaManagerAllowCast())
				Cast_Q(true);
			if(IsSpellActive("R") && ManaManagerAllowCast())
				Cast_R(true);
			if(IsSpellActive("E") && ManaManagerAllowCast())
				Cast_E(true);
			if(IsSpellActive("W") && ManaManagerAllowCast())
				Cast_W();
		}

		public override void OnLaneClear()
		{
			if(IsSpellActive("Q") && ManaManagerAllowCast())
				Cast_Q(false);
			if(IsSpellActive("R") && ManaManagerAllowCast())
				Cast_R(false);
			if(IsSpellActive("E") && ManaManagerAllowCast())
				Cast_E(false);
		}
		private void Cast_Q(bool mode)
		{
			if(!Q.IsReady())
				return;
			if(mode)
			{
				var target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Magical);
				if(target == null)
					return;
				if(MyHero.Spellbook.GetSpell(SpellSlot.Q).ToggleState == 1)
					Q.Cast(target, UsePackets());
			}
			else
			{
				if(MyHero.Spellbook.GetSpell(SpellSlot.Q).ToggleState == 1)
					Cast_BasicSkillshot_AOE_Farm(Q);
			}
		}

		private void Cast_W()
		{
			if(!W.IsReady())
				return;
			if(!Menu.Item("W_killable").GetValue<bool>())
				return;
			var target = SimpleTs.GetTarget(W.Range, SimpleTs.DamageType.Magical);
			if(!(GetComboDamage(target) >= target.Health))
				return;
			var pred = W.GetPrediction(target);
			var vec = new Vector3(pred.CastPosition.X - MyHero.ServerPosition.X, 0,
				pred.CastPosition.Z - MyHero.ServerPosition.Z);
			var castBehind = pred.CastPosition + Vector3.Normalize(vec) * 125;

			if(W.IsReady())
				W.Cast(castBehind, UsePackets());
		}

		private void Cast_E(bool mode)
		{
			if(!E.IsReady())
				return;
			if(mode)
			{
				var target = SimpleTs.GetTarget(E.Range, SimpleTs.DamageType.Magical);
				if(target.HasBuff("Chilled"))
				{
					E.CastOnUnit(target, UsePackets());
					return;
				}
				if(MyHero.GetSpellDamage(target, SpellSlot.E) > target.Health)
				{
					E.CastOnUnit(target, UsePackets());
					return;
				}
				if(R.IsReady() && MyHero.Distance(target) <= R.Range - 25)
					E.CastOnUnit(target, UsePackets());
			}
			else
			{
				var minions = MinionManager.GetMinions(MyHero.Position, E.Range, MinionTypes.All, MinionTeam.Neutral);
				var miniontarget = minions.FirstOrDefault(minion => minion.HasBuff("Chilled") && minion.MaxHealth > 800);
				if(miniontarget != null)
					E.CastOnUnit(miniontarget, UsePackets());
			}

		}

		private void Cast_R(bool mode)
		{
			if(!R.IsReady())
				return;
			if(mode)
			{
				var target = SimpleTs.GetTarget(R.Range, SimpleTs.DamageType.Magical);
				if(target == null)
					return;
				if(MyHero.Spellbook.GetSpell(SpellSlot.R).ToggleState == 1)
					R.Cast(target, UsePackets());
			}
			else
			{
				if(MyHero.Spellbook.GetSpell(SpellSlot.R).ToggleState == 1)
					Cast_BasicSkillshot_AOE_Farm(R);
			}

		}

		private void SetupWWidth()
		{
			var width = 300 + (100 * W.Level);
			W.Width = width;
		}

		private void CheckRHitSomeone()
		{
			try
			{
				if(!Menu.Item("R_ActiveCheck").GetValue<bool>() || _rlocation.Position == default(Vector3) || _rlocation == null)
					return;
				if(xSLxOrbwalker.CurrentMode == xSLxOrbwalker.Mode.Combo &&
					!(_rlocation.Position.CountEnemysInRange(400) >= 1))
					R.Cast();

				var minions = MinionManager.GetMinions(_rlocation.Position, 400, MinionTypes.All, MinionTeam.NotAlly).Any();
				var targets = _rlocation.Position.CountEnemysInRange(400) > 0;
				if(!(minions || targets))
					R.Cast();
			}
			// ReSharper disable once EmptyGeneralCatchClause
			catch
			{
			}
		}

		private void CheckQDetonation()
		{
			try
			{
				if(!Menu.Item("Q_AutoDetonate").GetValue<bool>() || _qShot.Position == default(Vector3) || _qShot == null)
					return;
				if(_qShot.Position.CountEnemysInRange(190) >= 1)
				{
					Q.Cast();
				}
				if(xSLxOrbwalker.CurrentMode == xSLxOrbwalker.Mode.LaneClear &&
					(MinionManager.GetMinions(_qShot.Position, 190, MinionTypes.All, MinionTeam.NotAlly).Count >= 3 ||
					 MinionManager.GetMinions(_qShot.Position, 190, MinionTypes.All, MinionTeam.Neutral).Any()))
					Q.Cast();
			}
			// ReSharper disable once EmptyGeneralCatchClause
			catch
			{
			}
		}

	}
}