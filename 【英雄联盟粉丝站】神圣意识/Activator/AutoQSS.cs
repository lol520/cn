using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace SAwareness
{
    class AutoQSS
    {
        private static Qss _qss;
        private readonly List<BuffType> _buffs = new List<BuffType>();

        public AutoQSS()
        {
            if (_qss == null)
            {
                Init();
            }
            Game.OnGameUpdate += Game_OnGameUpdate;
        }

        ~AutoQSS()
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
                    _qss = new Qss(SpellSlot.R);
                    break;

                case "Gankplank":
                    _qss = new Qss(SpellSlot.W);
                    break;

                case "Olaf":
                    _qss = new Qss(SpellSlot.R);
                    break;

                default:
                    return;
            }
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (!IsActive() || _qss == null || ObjectManager.Player.Spellbook.GetSpell(_qss.SpellSlot).State != SpellState.Ready)
                return;

            CreateBuffList();

            List<BuffInstance> buffList = Activator.GetActiveCcBuffs(_buffs);

            if (buffList.Count() >=
                Menu.ActivatorAutoQss.GetMenuItem("SAwarenessActivatorAutoQssMinSpells")
                    .GetValue<Slider>()
                    .Value)
            {
                Packet.C2S.Cast.Encoded(new Packet.C2S.Cast.Struct(ObjectManager.Player.NetworkId, _qss.SpellSlot)).Send();
            }

            if (ObjectManager.Player.HasBuffOfType(BuffType.Slow))
            {
                Packet.C2S.Cast.Encoded(new Packet.C2S.Cast.Struct(ObjectManager.Player.NetworkId, _qss.SpellSlot)).Send();              
            }
        }

        private void CreateBuffList()
        {
            _buffs.Clear();
            if (
                Menu.ActivatorAutoQssConfig.GetMenuItem("SAwarenessActivatorAutoQssConfigStun")
                    .GetValue<bool>())
                _buffs.Add(BuffType.Stun);
            if (
                Menu.ActivatorAutoQssConfig.GetMenuItem("SAwarenessActivatorAutoQssConfigSilence")
                    .GetValue<bool>())
                _buffs.Add(BuffType.Silence);
            if (
                Menu.ActivatorAutoQssConfig.GetMenuItem("SAwarenessActivatorAutoQssConfigTaunt")
                    .GetValue<bool>())
                _buffs.Add(BuffType.Taunt);
            if (
                Menu.ActivatorAutoQssConfig.GetMenuItem("SAwarenessActivatorAutoQssConfigFear")
                    .GetValue<bool>())
                _buffs.Add(BuffType.Fear);
            if (
                Menu.ActivatorAutoQssConfig.GetMenuItem("SAwarenessActivatorAutoQssConfigCharm")
                    .GetValue<bool>())
                _buffs.Add(BuffType.Charm);
            if (
                Menu.ActivatorAutoQssConfig.GetMenuItem("SAwarenessActivatorAutoQssConfigBlind")
                    .GetValue<bool>())
                _buffs.Add(BuffType.Blind);
            if (
                Menu.ActivatorAutoQssConfig.GetMenuItem("SAwarenessActivatorAutoQssConfigDisarm")
                    .GetValue<bool>())
                _buffs.Add(BuffType.Disarm);
            if (
                Menu.ActivatorAutoQssConfig.GetMenuItem("SAwarenessActivatorAutoQssConfigSuppress")
                    .GetValue<bool>())
                _buffs.Add(BuffType.Suppression);
            if (
                Menu.ActivatorAutoQssConfig.GetMenuItem("SAwarenessActivatorAutoQssConfigSlow")
                    .GetValue<bool>())
                _buffs.Add(BuffType.Slow);
            if (
                Menu.ActivatorAutoQssConfig.GetMenuItem(
                    "SAwarenessActivatorAutoQssConfigCombatDehancer").GetValue<bool>())
                _buffs.Add(BuffType.CombatDehancer);
            if (
                Menu.ActivatorAutoQssConfig.GetMenuItem("SAwarenessActivatorAutoQssConfigSnare")
                    .GetValue<bool>())
                _buffs.Add(BuffType.Snare);
            if (
                Menu.ActivatorAutoQssConfig.GetMenuItem("SAwarenessActivatorAutoQssConfigPoison")
                    .GetValue<bool>())
                _buffs.Add(BuffType.Poison);
        }

        private class Qss
        {
            public SpellSlot SpellSlot;

            public Qss(SpellSlot spellSlot)
            {
                SpellSlot = spellSlot;
            }
        }
    }
}
