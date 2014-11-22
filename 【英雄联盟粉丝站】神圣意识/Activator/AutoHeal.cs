using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LeagueSharp;
using LeagueSharp.Common;

namespace SAwareness
{
    class AutoHeal
    {

        private static Heal _heal;

        public AutoHeal()
        {
            if (_heal == null)
            {
                Init();
            }
            Game.OnGameUpdate += Game_OnGameUpdate;
        }

        ~AutoHeal()
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
                case "Alistar":
                    _heal = new Heal(SpellSlot.E, 575, false);
                    break;

                case "Gangplank":
                    _heal = new Heal(SpellSlot.W, 0, false, true);
                    break;

                case "Kayle":
                    _heal = new Heal(SpellSlot.W, 900);
                    break;

                case "Nami":
                    _heal = new Heal(SpellSlot.W, 725);
                    break;

                case "Nidalee":
                    _heal = new Heal(SpellSlot.E, 600);
                    break;

                case "Sona":
                    _heal = new Heal(SpellSlot.W, 1000, false);
                    break;

                case "Soraka":
                    _heal = new Heal(SpellSlot.W, 450, true, false, false);
                    break;

                case "Taric":
                    _heal = new Heal(SpellSlot.Q, 750);
                    break;

                default:
                    return;
            }
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (!IsActive() || _heal == null || ObjectManager.Player.Spellbook.GetSpell(_heal.SpellSlot).State != SpellState.Ready)
                return;

            foreach (var hero in ObjectManager.Get<Obj_AI_Hero>())
            {
                if (hero.IsMe || hero.IsAlly)
                {
                    if ((hero.Health/hero.MaxHealth)*100 < Menu.ActivatorAutoHeal.GetMenuItem(
                        "SAwarenessActivatorAutoHealPercent").GetValue<Slider>().Value)
                    {
                        if (hero.IsMe && !_heal.SelfCast)
                        {
                            continue;
                        }
                        CalcHeal(hero);
                        if (_heal.HealValue > hero.MaxHealth - hero.Health)
                        {
                            continue;
                        }
                        if (!_heal.Target && _heal.OnlySelf && hero.IsMe)
                        {
                            Packet.C2S.Cast.Encoded(new Packet.C2S.Cast.Struct(hero.NetworkId, _heal.SpellSlot)).Send();
                            return;
                        }
                        if (ObjectManager.Player.ServerPosition.Distance(hero.ServerPosition) < _heal.Range)
                        {
                            if (!_heal.Target)
                            {
                                Packet.C2S.Cast.Encoded(new Packet.C2S.Cast.Struct(hero.NetworkId, _heal.SpellSlot)).Send();
                                return;
                            }
                            Packet.C2S.Cast.Encoded(new Packet.C2S.Cast.Struct(hero.NetworkId, _heal.SpellSlot)).Send();
                            return;
                        }                        
                    }
                }
            }
        }

        private static void CalcHeal(Obj_AI_Hero hero)
        {
            bool selfHeal = ObjectManager.Player.NetworkId == hero.NetworkId;
            switch (ObjectManager.Player.ChampionName)
            {
                case "Alistar":
                    _heal.HealValue = (!selfHeal ?
                        30 + (15 * ObjectManager.Player.Spellbook.GetSpell(_heal.SpellSlot).Level) + 0.10 * ObjectManager.Player.FlatMagicDamageMod :
                        60 + (30 * ObjectManager.Player.Spellbook.GetSpell(_heal.SpellSlot).Level) + 0.20 * ObjectManager.Player.FlatMagicDamageMod);
                    break;

                case "Gangplank":
                    _heal.HealValue =  80 + (70 * ObjectManager.Player.Spellbook.GetSpell(_heal.SpellSlot).Level) + 1.00 * ObjectManager.Player.FlatMagicDamageMod;
                    break;

                case "Kayle":
                    _heal.HealValue =  60 + (45 * ObjectManager.Player.Spellbook.GetSpell(_heal.SpellSlot).Level) + 0.45 * ObjectManager.Player.FlatMagicDamageMod;
                    break;

                case "Nami":
                    _heal.HealValue =  65 + (30 * ObjectManager.Player.Spellbook.GetSpell(_heal.SpellSlot).Level) + 0.30 * ObjectManager.Player.FlatMagicDamageMod;
                    break;

                case "Nidalee":
                    _heal.HealValue = 45 + (40 * ObjectManager.Player.Spellbook.GetSpell(_heal.SpellSlot).Level) + 0.50 * ObjectManager.Player.FlatMagicDamageMod;
                    break;

                case "Sona":
                    _heal.HealValue = 25 + (20 * ObjectManager.Player.Spellbook.GetSpell(_heal.SpellSlot).Level) + 0.20 * ObjectManager.Player.FlatMagicDamageMod; //TODO: Fix for max Health
                    break;

                case "Soraka":
                    _heal.HealValue = 120 + (30 * ObjectManager.Player.Spellbook.GetSpell(_heal.SpellSlot).Level) + 0.60 * ObjectManager.Player.FlatMagicDamageMod;
                    break;

                case "Taric":
                    _heal.HealValue = (!selfHeal ? 
                        60 + (40 * ObjectManager.Player.Spellbook.GetSpell(_heal.SpellSlot).Level) + 0.30 * ObjectManager.Player.FlatMagicDamageMod + 0.05 * ObjectManager.Player.FlatHPPoolMod : 
                        84 + (56 * ObjectManager.Player.Spellbook.GetSpell(_heal.SpellSlot).Level) + 0.42 * ObjectManager.Player.FlatMagicDamageMod + 0.07 * ObjectManager.Player.FlatHPPoolMod );
                    break;

                default:
                    return;
            }
        }

        private class Heal
        {
            public SpellSlot SpellSlot;
            public bool OnlySelf;
            public double Range;
            public bool SelfCast;
            public bool Target;

            public double HealValue;

            public Heal(SpellSlot spellSlot, double range, bool target = true, bool onlySelf = false, bool selfCast = true)
            {
                SpellSlot = spellSlot;
                OnlySelf = onlySelf;
                Range = range;
                SelfCast = selfCast;
                Target = target;
            }
        }
    }
}
