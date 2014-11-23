﻿using System.Linq;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace RivenSharp
{
    class SummonerItems
    {
        private Obj_AI_Hero player;
        private Spellbook sumBook;
        private SpellSlot ignite;
        private SpellSlot smite;


        public enum ItemIds
        {
            //MuramanaDe = 3043,
            Muramana = 3042,
            Tiamat = 3077,
            Hydra = 3074,
            MercScim = 3139,
            Hextech = 3146,
            SwordOD = 3131,
            Ghostblade = 3142,
            BotRK = 3153,
            Cutlass = 3144
        }

        public SummonerItems(Obj_AI_Hero myHero)
        {
            player = myHero;
            sumBook = player.SummonerSpellbook;
            ignite = player.GetSpellSlot("summonerdot");
            smite = player.GetSpellSlot("SummonerSmite");
        }

        public void castIgnite(Obj_AI_Hero target)
        {
            if (ignite != SpellSlot.Unknown && sumBook.CanUseSpell(ignite) == SpellState.Ready)
                sumBook.CastSpell(ignite, target);
        }

        public void castSmite(Obj_AI_Hero target)
        {
            if (smite != SpellSlot.Unknown && sumBook.CanUseSpell(smite) == SpellState.Ready)
                sumBook.CastSpell(smite, target);
        }

        public void cast(ItemIds item)
        {
            var itemId = (int)item;
            if (Items.CanUseItem(itemId))
                Items.UseItem(itemId);
        }

        public void cast(ItemIds item, Vector3 target)
        {
            var itemId = (int)item;
            if (Items.CanUseItem(itemId))
                getInvSlot(itemId).UseItem(target);

        }

        public void cast(ItemIds item, Obj_AI_Base target)
        {
            var itemId = (int)item;
            if (Items.CanUseItem(itemId))
                Items.UseItem(itemId, target);
        }

        private InventorySlot getInvSlot(int id)
        {
            return player.InventoryItems.FirstOrDefault(iSlot => (int)iSlot.Id == id);
        }
    }
}
