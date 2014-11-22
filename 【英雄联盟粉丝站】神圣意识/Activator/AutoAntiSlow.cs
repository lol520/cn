using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace SAwareness
{
    class AutoAntiSlow
    {
        private static AntiSlow _antiSlow;

        public AutoAntiSlow()
        {
            if (_antiSlow == null)
            {
                Init();
            }
            Game.OnGameUpdate += Game_OnGameUpdate;
        }

        ~AutoAntiSlow()
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
                case "Evelynn":
                    _antiSlow = new AntiSlow(SpellSlot.W);
                    break;

                case "Garen":
                    _antiSlow = new AntiSlow(SpellSlot.Q);
                    break;

                case "MasterYi":
                    _antiSlow = new AntiSlow(SpellSlot.R);
                    break;

                default:
                    return;
            }
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (!IsActive() || _antiSlow == null || ObjectManager.Player.Spellbook.GetSpell(_antiSlow.SpellSlot).State != SpellState.Ready)
                return;

            if (ObjectManager.Player.HasBuffOfType(BuffType.Slow))
            {
                Packet.C2S.Cast.Encoded(new Packet.C2S.Cast.Struct(ObjectManager.Player.NetworkId, _antiSlow.SpellSlot)).Send();              
            }
        }

        private class AntiSlow
        {
            public SpellSlot SpellSlot;

            public AntiSlow(SpellSlot spellSlot)
            {
                SpellSlot = spellSlot;
            }
        }
    }
}
