﻿#region

using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

#endregion

namespace Marksman
{
    internal class Graves : Champion
    {
        public Spell Q;
        public Spell R;
        public Spell W;

        public Graves()
        {
            Utils.PrintMessage("Graves loaded.");

            Q = new Spell(SpellSlot.Q, 950f); // Q likes to shoot a bit too far away, so moving the range inward.
            Q.SetSkillshot(0.25f, 15f * 2 * (float)Math.PI / 180, 2000f, false, SkillshotType.SkillshotCone);

            W = new Spell(SpellSlot.W, 1100f);
            W.SetSkillshot(0.25f, 250f, 1650f, false, SkillshotType.SkillshotCircle);

            R = new Spell(SpellSlot.R, 1100f);
            R.SetSkillshot(0.25f, 100f, 2100f, true, SkillshotType.SkillshotLine);
        }

        public override void Game_OnGameUpdate(EventArgs args)
        {
            if (Q.IsReady() && GetValue<KeyBind>("UseQTH").Active)
            {
                if(ObjectManager.Player.HasBuff("Recall"))
                    return;
                var t = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Physical);
                if (t != null)
                    Q.Cast(t, false, true);
            }
            
            if ((!ComboActive && !HarassActive) || !Orbwalking.CanMove(100)) return;
            var useQ = GetValue<bool>("UseQ" + (ComboActive ? "C" : "H"));
            var useW = GetValue<bool>("UseW" + (ComboActive ? "C" : "H"));
            var useR = GetValue<bool>("UseR" + (ComboActive ? "C" : "H"));

            if (Q.IsReady() && useQ)
            {
                var t = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Physical);
                if (t != null)
                    Q.Cast(t, false, true);
            }

            if (W.IsReady() && useW)
            {
                var t = SimpleTs.GetTarget(W.Range, SimpleTs.DamageType.Physical);
                if (t != null)
                    W.Cast(t, false, true);
            }

            if (R.IsReady() && useR)
            {
                foreach (
                    var hero in
                        ObjectManager.Get<Obj_AI_Hero>()
                            .Where(
                                hero =>
                                    hero.IsValidTarget(R.Range) &&
                                    ObjectManager.Player.GetSpellDamage(hero, SpellSlot.R, 1) - 20 > hero.Health))
                    R.Cast(hero, false, true);
            }
        }

        public override void Orbwalking_AfterAttack(Obj_AI_Base unit, Obj_AI_Base target)
        {
            if ((ComboActive || HarassActive) && unit.IsMe && (target is Obj_AI_Hero))
            {
                var useQ = GetValue<bool>("UseQ" + (ComboActive ? "C" : "H"));
                var useW = GetValue<bool>("UseW" + (ComboActive ? "C" : "H"));

                if (Q.IsReady() && useQ)
                    Q.Cast(target);

                if (W.IsReady() && useW)
                    W.Cast(target);
            }
        }

        public override void Drawing_OnDraw(EventArgs args)
        {
            Spell[] spellList = { Q };
            foreach (var spell in spellList)
            {
                var menuItem = GetValue<Circle>("Draw" + spell.Slot);
                if (menuItem.Active)
                    Utility.DrawCircle(ObjectManager.Player.Position, spell.Range, menuItem.Color);
            }
        }

        public override bool ComboMenu(Menu config)
        {
            config.AddItem(new MenuItem("UseQC" + Id, "浣跨敤Q").SetValue(true));
            config.AddItem(new MenuItem("UseRC" + Id, "浣跨敤R").SetValue(true));
            config.AddItem(new MenuItem("UseWC" + Id, "浣跨敤W").SetValue(true));
            return true;
        }

        public override bool HarassMenu(Menu config)
        {
            config.AddItem(new MenuItem("UseQH" + Id, "浣跨敤Q").SetValue(true));
            config.AddItem(new MenuItem("UseWH" + Id, "浣跨敤W").SetValue(false));
            config.AddItem(
                new MenuItem("UseQTH" + Id, "浣跨敤Q (閿佸畾)").SetValue(new KeyBind("H".ToCharArray()[0],
                    KeyBindType.Toggle)));           
            
            return true;
        }

        public override bool DrawingMenu(Menu config)
        {
            config.AddItem(
                new MenuItem("DrawQ" + Id, "Q鑼冨洿").SetValue(new Circle(true,
                    System.Drawing.Color.FromArgb(100, 255, 0, 255))));
            return true;
        }

        public override bool ExtrasMenu(Menu config)
        {

            return true;
        }


    }
}
