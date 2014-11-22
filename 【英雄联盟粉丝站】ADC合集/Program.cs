#region
using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
#endregion

namespace Marksman
{
    internal class Program
    {
        public static Menu Config;
        public static Menu QuickSilverMenu;
        public static Champion CClass;
        public static Activator AActivator;
        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            Config = new Menu("Marksman", "Marksman", true);
            CClass = new Champion();
            AActivator = new Activator();
            
            var BaseType = CClass.GetType();

            /* Update this with Activator.CreateInstance or Invoke
               http://stackoverflow.com/questions/801070/dynamically-invoking-any-function-by-passing-function-name-as-string 
               For now stays cancer.
             */
            var championName = ObjectManager.Player.ChampionName.ToLowerInvariant();

            switch (championName)
            {
                case "ashe":
                    CClass = new Ashe();
                    break;
                case "caitlyn":
                    CClass = new Caitlyn();
                    break;
                case "corki":
                    CClass = new Corki();
                    break;
                case "draven":
                    CClass = new Draven();
                    break;
                case "ezreal":
                    CClass = new Ezreal();
                    break;
                case "graves":
                    CClass = new Graves();
                    break;
                case "jinx":
                    CClass = new Jinx();
                    break;
                case "kogmaw":
                    CClass = new Kogmaw();
                    break;
                case "lucian":
                    CClass = new Lucian();
                    break;
                case "missfortune":
                    CClass = new MissFortune();
                    break;   
                case "quinn":
                    CClass = new Quinn();
                    break;
                case "sivir":
                    CClass = new Sivir();
                    break;
                case "teemo":
                    CClass = new Teemo();
                    break;
                case "tristana":
                    CClass = new Tristana();
                    break;
                case "twitch":
                    CClass = new Twitch();
                    break;
                case "vayne":
                    CClass = new Vayne();
                    break;
                case "varus":
                    CClass = new Varus();
                    break;
            }


            CClass.Id = ObjectManager.Player.BaseSkinName;
            CClass.Config = Config;

            var targetSelectorMenu = new Menu("鐩爣閫夋嫨", "Target Selector");
            SimpleTs.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            var orbwalking = Config.AddSubMenu(new Menu("璧扮爫", "Orbwalking"));
            CClass.Orbwalker = new Orbwalking.Orbwalker(orbwalking);

            var items = Config.AddSubMenu(new Menu("鐗╁搧", "Items"));
            items.AddItem(new MenuItem("BOTRK", "鐮磋触").SetValue(true));
            items.AddItem(new MenuItem("GHOSTBLADE", "灏忓集鍒€").SetValue(true));
            QuickSilverMenu = new Menu("姘撮摱鑵板甫", "QuickSilverSash");
            items.AddSubMenu(QuickSilverMenu);
            QuickSilverMenu.AddItem(new MenuItem("AnyStun", "鐪╂檿").SetValue(true));
            QuickSilverMenu.AddItem(new MenuItem("AnySnare", "澶瑰瓙").SetValue(true));
            QuickSilverMenu.AddItem(new MenuItem("AnyTaunt", "鍢茶").SetValue(true));
            foreach (var t in AActivator.BuffList)
            {
                foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(enemy => enemy.IsEnemy))
                {
                    if (t.ChampionName == enemy.ChampionName)
                        QuickSilverMenu.AddItem(new MenuItem(t.BuffName, t.DisplayName).SetValue(t.DefaultValue));
                }
            }
            items.AddItem(
                new MenuItem("UseItemsMode", "浣跨敤鐗╁搧").SetValue(
                    new StringList(new[] {"no", "娣峰悎", "杩炴嫑", "鍏ㄩ儴"}, 2)));

            
            //var Extras = Config.AddSubMenu(new Menu("Extras", "Extras"));
            //new PotionManager(Extras);

            // If Champion is supported draw the extra menus
            if (BaseType != CClass.GetType())
            {
                var combo = new Menu("杩炴嫑", "Combo");
                if (CClass.ComboMenu(combo))
                {
                    Config.AddSubMenu(combo);
                }

                var harass = new Menu("楠氭壈", "Harass");
                if (CClass.HarassMenu(harass))
                {
                    harass.AddItem(new MenuItem("HarassMana", "min钃濋噺%").SetValue(new Slider(50, 100, 0)));
                    Config.AddSubMenu(harass);
                }

                var laneclear = new Menu("娓呯嚎", "LaneClear");
                if (CClass.LaneClearMenu(laneclear))
                {
                    Config.AddSubMenu(laneclear);
                }

                var misc = new Menu("鏉傞」", "Misc");
                if (CClass.MiscMenu(misc))
                {
                    Config.AddSubMenu(misc);
                }

                var extras = new Menu("闄勫姞", "Extras");
                if (CClass.ExtrasMenu(extras))
                {
                    new PotionManager(extras);
                    Config.AddSubMenu(extras);
                }

                var drawing = new Menu("鏄剧ず", "Drawings");
                if (CClass.DrawingMenu(drawing))
                {
                    Config.AddSubMenu(drawing);
                }
				
			Config.AddSubMenu(new Menu("鑻遍泟鑱旂洘绮変笣绔檌", "LOL520.CC"));
				Config.SubMenu("LOL520.CC").AddItem(new MenuItem("qunhao", "浜ゆ祦缇わ細21422249"));
				Config.SubMenu("LOL520.CC").AddItem(new MenuItem("qunhao2", "浜ゆ祦2缇わ細397773763"));
            }


            CClass.MainMenu(Config);

            Config.AddToMainMenu();

            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnGameUpdate += Game_OnGameUpdate;
            Orbwalking.AfterAttack += Orbwalking_AfterAttack;
            Orbwalking.BeforeAttack += Orbwalking_BeforeAttack;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            CClass.Drawing_OnDraw(args);
            return;

            var y = 10;

            foreach (
                var t in
                    ObjectManager.Player.Buffs.Select(
                        b => b.DisplayName + " - " + b.IsActive + " - " + (b.EndTime > Game.Time) + " - " + b.IsPositive)
                )
            {
                Drawing.DrawText(0, y, System.Drawing.Color.Wheat, t);
                y = y + 16;
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            CheckChampionBuff();
            //Update the combo and harass values.
            CClass.ComboActive = CClass.Config.Item("Orbwalk").GetValue<KeyBind>().Active;
            
            var existsMana = ObjectManager.Player.MaxMana/100*Config.Item("HarassMana").GetValue<Slider>().Value;
            CClass.HarassActive = CClass.Config.Item("Farm").GetValue<KeyBind>().Active &&
                                  ObjectManager.Player.Mana >= existsMana;
                                  
            CClass.LaneClearActive = CClass.Config.Item("LaneClear").GetValue<KeyBind>().Active;
            CClass.Game_OnGameUpdate(args);

            var useItemModes = Config.Item("UseItemsMode").GetValue<StringList>().SelectedIndex;

            //Items
            if (
                !((CClass.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo &&
                   (useItemModes == 2 || useItemModes == 3))
                  ||
                  (CClass.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed &&
                   (useItemModes == 1 || useItemModes == 3))))
                return;

            var botrk = Config.Item("BOTRK").GetValue<bool>();
            var ghostblade = Config.Item("GHOSTBLADE").GetValue<bool>();
            var target = CClass.Orbwalker.GetTarget();

            if (botrk)
            {
                if (target != null && target.Type == ObjectManager.Player.Type &&
                    target.ServerPosition.Distance(ObjectManager.Player.ServerPosition) < 450)
                {
                    var hasCutGlass = Items.HasItem(3144);
                    var hasBotrk = Items.HasItem(3153);

                    if (hasBotrk || hasCutGlass)
                    {
                        var itemId = hasCutGlass ? 3144 : 3153;
                        var damage = ObjectManager.Player.GetItemDamage(target, Damage.DamageItems.Botrk);
                        if (hasCutGlass || ObjectManager.Player.Health + damage < ObjectManager.Player.MaxHealth)
                            Items.UseItem(itemId, target);
                    }
                }
            }

            if (ghostblade && target != null && target.Type == ObjectManager.Player.Type &&
                Orbwalking.InAutoAttackRange(target))
                Items.UseItem(3142);
        }
        
        private static void CheckChampionBuff()
        {
            foreach (var t1 in ObjectManager.Player.Buffs)
            {
                foreach (var t in QuickSilverMenu.Items)
                {
                    if (QuickSilverMenu.Item(t.Name).GetValue<bool>())
                    {
                        {
                            if (t1.Name.ToLower().Contains(t.Name.ToLower()))
                            {
                                if (Items.HasItem(3139)) Items.UseItem(3139); 
                                if (Items.HasItem(3140)) Items.UseItem(3140);
                            }
                        }
                    }

                    if (QuickSilverMenu.Item("AnySnare").GetValue<bool>() &&
                        ObjectManager.Player.HasBuffOfType(BuffType.Snare))
                    {
                        if (Items.HasItem(3139)) Items.UseItem(3139);
                        if (Items.HasItem(3140)) Items.UseItem(3140);
                    }
                    if (QuickSilverMenu.Item("AnyStun").GetValue<bool>() &&
                        ObjectManager.Player.HasBuffOfType(BuffType.Stun))
                    {
                        if (Items.HasItem(3139)) Items.UseItem(3139);
                        if (Items.HasItem(3140)) Items.UseItem(3140);
                    }
                    if (QuickSilverMenu.Item("AnyTaunt").GetValue<bool>() &&
                        ObjectManager.Player.HasBuffOfType(BuffType.Taunt))
                    {
                        if (Items.HasItem(3139)) Items.UseItem(3139);
                        if (Items.HasItem(3140)) Items.UseItem(3140);
                    }

                }
            }
        }

        private static void Orbwalking_AfterAttack(Obj_AI_Base unit, Obj_AI_Base target)
        {
            CClass.Orbwalking_AfterAttack(unit, target);
        }

        private static void Orbwalking_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            CClass.Orbwalking_BeforeAttack(args);
        }
    }
}
