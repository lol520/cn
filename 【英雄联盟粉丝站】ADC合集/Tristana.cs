#region
using System;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
#endregion

namespace Marksman
{
    internal class Tristana : Champion
    {
        public Spell E;
        public Spell Q;
        public Spell R;

        public static Items.Item Dfg = new Items.Item(3128, 750);

        public Tristana()
        {
            Utils.PrintMessage("Tristana loaded.");
            
            Q = new Spell(SpellSlot.Q, 703);
            E = new Spell(SpellSlot.E, 703);
            R = new Spell(SpellSlot.R, 703);

            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter.OnPossibleToInterrupt += Interrupter_OnPossibleToInterrupt;
        }

        public void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (R.IsReady() && gapcloser.Sender.IsValidTarget(R.Range) && GetValue<bool>("UseRMG"))
                R.CastOnUnit(gapcloser.Sender);
        }

        public void Interrupter_OnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if (R.IsReady() && unit.IsValidTarget(R.Range) && GetValue<bool>("UseRMI"))
                R.CastOnUnit(unit);
        }

        public override void Orbwalking_AfterAttack(Obj_AI_Base unit, Obj_AI_Base target)
        {
            if ((ComboActive || HarassActive) && unit.IsMe && (target is Obj_AI_Hero))
            {
                var useQ = GetValue<bool>("UseQ" + (ComboActive ? "C" : "H"));
                var useE = GetValue<bool>("UseE" + (ComboActive ? "C" : "H"));

                if (useQ && Q.IsReady())
                    Q.CastOnUnit(ObjectManager.Player);

                if (useE && E.IsReady())
                    E.CastOnUnit(target);
            }
        }

        public override void Drawing_OnDraw(EventArgs args)
        {
            Spell[] spellList = { E};
            foreach (var spell in spellList)
            {
                var menuItem = GetValue<Circle>("Draw" + spell.Slot);
                if (menuItem.Active)
                    Utility.DrawCircle(ObjectManager.Player.Position, spell.Range, menuItem.Color);
            }
        }

        public override void Game_OnGameUpdate(EventArgs args)
        {
            if (!Orbwalking.CanMove(100)) return;
            
            //Update Q range depending on level; 600 + 5 Ã— ( Tristana's level - 1)/* dont waste your Q for only 1 or 2 hits. */
            //Update E and R range depending on level; 630 + 9 Ã— ( Tristana's level - 1)
            Q.Range = 600 + 5 * (ObjectManager.Player.Level - 1);
            E.Range = 630 + 9 * (ObjectManager.Player.Level - 1);
            R.Range = 630 + 9 * (ObjectManager.Player.Level - 1);

            if (GetValue<KeyBind>("UseETH").Active)
            {
                 if(ObjectManager.Player.HasBuff("Recall"))
                    return;
                var eTarget = SimpleTs.GetTarget(E.Range, SimpleTs.DamageType.Physical);
                if (E.IsReady() && eTarget.IsValidTarget())
                    E.CastOnUnit(eTarget);
            }

            if (ComboActive || HarassActive)
            {
                var useE = GetValue<bool>("UseE" + (ComboActive ? "C" : "H"));

                if (useE)
                {
                    var eTarget = SimpleTs.GetTarget(E.Range, SimpleTs.DamageType.Physical);
                    if (E.IsReady() && eTarget.IsValidTarget())
                        E.CastOnUnit(eTarget);
                }

                if (Dfg.IsReady())
                {
                    var eTarget = SimpleTs.GetTarget(E.Range, SimpleTs.DamageType.Magical);
                    Dfg.Cast(eTarget);
                }
            }

            //Killsteal
            if (!ComboActive || !GetValue<bool>("UseRM") || !R.IsReady()) return;
            foreach (
                var hero in
                    ObjectManager.Get<Obj_AI_Hero>()
                        .Where(
                            hero =>
                                hero.IsValidTarget(R.Range) &&
                                ObjectManager.Player.GetSpellDamage(hero, SpellSlot.R) - 50 > hero.Health))
                R.CastOnUnit(hero);
        }

        public override bool ComboMenu(Menu config)
        {
            config.AddItem(new MenuItem("UseQC" + Id, "浣跨敤Q").SetValue(true));
            config.AddItem(new MenuItem("UseEC" + Id, "浣跨敤E").SetValue(true));
            return true;
        }

        public override bool HarassMenu(Menu config)
        {
            config.AddItem(new MenuItem("UseQH" + Id, "浣跨敤Q").SetValue(false));
            config.AddItem(new MenuItem("UseEH" + Id, "浣跨敤E").SetValue(true));
            config.AddItem(
                new MenuItem("UseETH" + Id, "浣跨敤E (閿佸畾)").SetValue(new KeyBind("H".ToCharArray()[0],
                    KeyBindType.Toggle)));
            return true;
        }

        public override bool DrawingMenu(Menu config)
        {
            config.AddItem(
                new MenuItem("DrawE" + Id, "E鑼冨洿").SetValue(new Circle(true, Color.CornflowerBlue)));
            return true;
        }

        public override bool MiscMenu(Menu config)
        {
            config.AddItem(new MenuItem("UseRM" + Id, "鍙潃浣跨敤R").SetValue(true));
            config.AddItem(new MenuItem("UseRMG" + Id, "浣跨敤R闃茬獊").SetValue(true));
            config.AddItem(new MenuItem("UseRMI" + Id, "浣跨敤R鎵撴柇").SetValue(true));
            return true;
        }

        public override bool ExtrasMenu(Menu config)
        {

            return true;
        }

    }
}
