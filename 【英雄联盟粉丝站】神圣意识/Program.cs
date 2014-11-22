using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using LeagueSharp;
using LeagueSharp.Common;

namespace SAwareness
{
    internal static class Menu
    {
        public static MenuItemSettings ItemPanel = new MenuItemSettings();
        public static MenuItemSettings AutoLevler = new MenuItemSettings(typeof (AutoLevler)); //Only priority works

        public static MenuItemSettings UiTracker = new MenuItemSettings(typeof (UiTracker));
            //Works but need many improvements

        public static MenuItemSettings UimTracker = new MenuItemSettings(typeof (UimTracker));
            //Works but need many improvements

        public static MenuItemSettings SsCaller = new MenuItemSettings(typeof (SsCaller)); //Works
        public static MenuItemSettings Tracker = new MenuItemSettings();
        public static MenuItemSettings WaypointTracker = new MenuItemSettings(typeof (WaypointTracker)); //Works
        public static MenuItemSettings CloneTracker = new MenuItemSettings(typeof (CloneTracker)); //Works
        public static MenuItemSettings Timers = new MenuItemSettings(typeof (Timers));
        public static MenuItemSettings JungleTimer = new MenuItemSettings(); //Works
        public static MenuItemSettings RelictTimer = new MenuItemSettings(); //Works
        public static MenuItemSettings HealthTimer = new MenuItemSettings(); //Works
        public static MenuItemSettings InhibitorTimer = new MenuItemSettings(); //Works
        public static MenuItemSettings SummonerTimer = new MenuItemSettings(); //Works
        public static MenuItemSettings Health = new MenuItemSettings(typeof (Health));
        public static MenuItemSettings TowerHealth = new MenuItemSettings(); //Missing HPBarPos
        public static MenuItemSettings InhibitorHealth = new MenuItemSettings(); //Works

        public static MenuItemSettings DestinationTracker = new MenuItemSettings(typeof (DestinationTracker));
            //Work & Needs testing

        public static MenuItemSettings Detector = new MenuItemSettings();

        public static MenuItemSettings VisionDetector = new MenuItemSettings(typeof (HiddenObject));
            //Works - OnProcessSpell bugged

        public static MenuItemSettings RecallDetector = new MenuItemSettings(typeof (RecallDetector)); //Works

        public static MenuItemSettings Range = new MenuItemSettings(typeof (Ranges));
            //Many ranges are bugged. Waiting for SpellLib

        public static MenuItemSettings TowerRange = new MenuItemSettings();
        public static MenuItemSettings ShopRange = new MenuItemSettings();
        public static MenuItemSettings VisionRange = new MenuItemSettings();
        public static MenuItemSettings ExperienceRange = new MenuItemSettings();
        public static MenuItemSettings AttackRange = new MenuItemSettings();
        public static MenuItemSettings SpellQRange = new MenuItemSettings();
        public static MenuItemSettings SpellWRange = new MenuItemSettings();
        public static MenuItemSettings SpellERange = new MenuItemSettings();
        public static MenuItemSettings SpellRRange = new MenuItemSettings();
        public static MenuItemSettings ImmuneTimer = new MenuItemSettings(typeof (ImmuneTimer)); //Works
        public static MenuItemSettings Ganks = new MenuItemSettings();
        public static MenuItemSettings GankTracker = new MenuItemSettings(typeof (GankPotentialTracker)); //Works
        public static MenuItemSettings GankDetector = new MenuItemSettings(typeof (GankDetector)); //Works
        public static MenuItemSettings AltarTimer = new MenuItemSettings();
        public static MenuItemSettings Wards = new MenuItemSettings();
        public static MenuItemSettings WardCorrector = new MenuItemSettings(typeof (WardCorrector)); //Works
        public static MenuItemSettings BushRevealer = new MenuItemSettings(typeof (BushRevealer)); //Works        
        public static MenuItemSettings InvisibleRevealer = new MenuItemSettings(typeof (InvisibleRevealer)); //Works   
        public static MenuItemSettings SkinChanger = new MenuItemSettings(typeof (SkinChanger)); //Works
        public static MenuItemSettings AutoSmite = new MenuItemSettings(typeof (AutoSmite)); //Works
        public static MenuItemSettings AutoPot = new MenuItemSettings(typeof (AutoPot));
        public static MenuItemSettings SafeMovement = new MenuItemSettings(typeof (SafeMovement));
        public static MenuItemSettings AutoShield = new MenuItemSettings(typeof (AutoShield));
        public static MenuItemSettings AutoShieldBlockableSpells = new MenuItemSettings();
        public static MenuItemSettings Misc = new MenuItemSettings();
        public static MenuItemSettings MoveToMouse = new MenuItemSettings(typeof (MoveToMouse));
        public static MenuItemSettings SurrenderVote = new MenuItemSettings(typeof (SurrenderVote));
        public static MenuItemSettings AutoLatern = new MenuItemSettings(typeof (AutoLatern));
        public static MenuItemSettings DisconnectDetector = new MenuItemSettings(typeof (DisconnectDetector));
        public static MenuItemSettings AutoJump = new MenuItemSettings(typeof (AutoJump));
        public static MenuItemSettings TurnAround = new MenuItemSettings(typeof (TurnAround));
        public static MenuItemSettings MinionBars = new MenuItemSettings(typeof(MinionBars));
        public static MenuItemSettings MinionLocation = new MenuItemSettings(typeof(MinionLocation));
        public static MenuItemSettings FlashJuke = new MenuItemSettings(typeof(FlashJuke));
        public static MenuItemSettings Activator = new MenuItemSettings(typeof (Activator));
        public static MenuItemSettings ActivatorAutoSummonerSpell = new MenuItemSettings();
        public static MenuItemSettings ActivatorAutoSummonerSpellIgnite = new MenuItemSettings();
        public static MenuItemSettings ActivatorAutoSummonerSpellHeal = new MenuItemSettings();
        public static MenuItemSettings ActivatorAutoSummonerSpellBarrier = new MenuItemSettings();
        public static MenuItemSettings ActivatorAutoSummonerSpellExhaust = new MenuItemSettings();
        public static MenuItemSettings ActivatorAutoSummonerSpellCleanse = new MenuItemSettings();
        public static MenuItemSettings ActivatorOffensive = new MenuItemSettings();
        public static MenuItemSettings ActivatorOffensiveAd = new MenuItemSettings();
        public static MenuItemSettings ActivatorOffensiveAp = new MenuItemSettings();
        public static MenuItemSettings ActivatorDefensive = new MenuItemSettings();
        public static MenuItemSettings ActivatorDefensiveCleanseConfig = new MenuItemSettings();
        public static MenuItemSettings ActivatorDefensiveSelfShield = new MenuItemSettings();
        public static MenuItemSettings ActivatorDefensiveWoogletZhonya = new MenuItemSettings();
        public static MenuItemSettings ActivatorDefensiveDebuffSlow = new MenuItemSettings();
        public static MenuItemSettings ActivatorDefensiveCleanseSelf = new MenuItemSettings();
        public static MenuItemSettings ActivatorDefensiveShieldBoost = new MenuItemSettings();
        public static MenuItemSettings ActivatorDefensiveMikaelCleanse = new MenuItemSettings();
        public static MenuItemSettings ActivatorMisc = new MenuItemSettings();
        public static MenuItemSettings ActivatorAutoHeal = new MenuItemSettings(typeof(AutoHeal));
        public static MenuItemSettings ActivatorAutoUlt = new MenuItemSettings(typeof(AutoUlt));
        public static MenuItemSettings ActivatorAutoQss = new MenuItemSettings(typeof(AutoQSS));
        public static MenuItemSettings ActivatorAutoQssConfig = new MenuItemSettings(typeof(AutoQSS));
        public static MenuItemSettings Killable = new MenuItemSettings(typeof (Killable));
        public static MenuItemSettings EasyRangedJungle = new MenuItemSettings(typeof(EasyRangedJungle));
        public static MenuItemSettings FowWardPlacement = new MenuItemSettings(typeof(FowWardPlacement));
        public static MenuItemSettings GlobalSettings = new MenuItemSettings();
		public static MenuItemSettings TranScreeder = new MenuItemSettings();
                
		
        public class MenuItemSettings
        {
            public bool ForceDisable;
            public dynamic Item;
            public LeagueSharp.Common.Menu Menu;
            public List<MenuItem> MenuItems = new List<MenuItem>();
            public String Name;
            public List<MenuItemSettings> SubMenus = new List<MenuItemSettings>();
            public Type Type;

            public MenuItemSettings(Type type, dynamic item)
            {
                Type = type;
                Item = item;
            }

            public MenuItemSettings(dynamic item)
            {
                Item = item;
            }

            public MenuItemSettings(Type type)
            {
                Type = type;
            }

            public MenuItemSettings(String name)
            {
                Name = name;
            }

            public MenuItemSettings()
            {
            }

            public MenuItemSettings AddMenuItemSettings(String displayName, String name)
            {
                SubMenus.Add(new MenuItemSettings(name));
                MenuItemSettings tempSettings = GetMenuSettings(name);
                if (tempSettings == null)
                {
                    throw new NullReferenceException(name + " not found");
                }
                tempSettings.Menu = Menu.AddSubMenu(new LeagueSharp.Common.Menu(displayName, name));
                return tempSettings;
            }

            public bool GetActive()
            {
                if (Menu == null)
                    return false;
                foreach (MenuItem item in Menu.Items)
                {
                    if (item.DisplayName == "鎵撳紑")
                    {
                        if (item.GetValue<bool>())
                        {
                            return true;
                        }
                        return false;
                    }
                }
                return false;
            }

            public void SetActive(bool active)
            {
                if (Menu == null)
                    return;
                foreach (MenuItem item in Menu.Items)
                {
                    if (item.DisplayName == "鎵撳紑")
                    {
                        item.SetValue(active);
                        return;
                    }
                }
            }

            public MenuItem GetMenuItem(String menuName)
            {
                if (Menu == null)
                    return null;
                foreach (MenuItem item in Menu.Items)
                {
                    if (item.Name == menuName)
                    {
                        return item;
                    }
                }
                return null;
            }

            public LeagueSharp.Common.Menu GetSubMenu(String menuName)
            {
                if (Menu == null)
                    return null;
                return Menu.SubMenu(menuName);
            }

            public MenuItemSettings GetMenuSettings(String name)
            {
                foreach (MenuItemSettings menu in SubMenus)
                {
                    if (menu.Name.Contains(name))
                        return menu;
                }
                return null;
            }
        }

        //public static MenuItemSettings  = new MenuItemSettings();
    }

    internal class Program
    {
        private static float lastDebugTime = 0;

        private static void Main(string[] args)
        {
            try
            {
                //SUpdater.UpdateCheck();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return;
            }

            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void CreateMenu()
        {
            //http://www.cambiaresearch.com/articles/15/javascript-char-codes-key-codes
            try
            {
                Menu.MenuItemSettings tempSettings;
                var menu = new LeagueSharp.Common.Menu("绁炲湥鎰忚瘑", "SAwareness", true);

                //Not crashing
                Menu.Timers.Menu = menu.AddSubMenu(new LeagueSharp.Common.Menu("璁℃椂", "SAwarenessTimers"));
                Menu.Timers.MenuItems.Add(
                    Menu.Timers.Menu.AddItem(
                        new MenuItem("SAwarenessTimersPingTimes", "Ping娆℃暟").SetValue(new Slider(0, 5, 0))));
                Menu.Timers.MenuItems.Add(
                    Menu.Timers.Menu.AddItem(
                        new MenuItem("SAwarenessTimersRemindTime", "鎻愰啋鏃堕棿").SetValue(new Slider(0, 50, 0))));
                Menu.Timers.MenuItems.Add(
                    Menu.Timers.Menu.AddItem(new MenuItem("SAwarenessTimersLocalPing", "鏈湴Ping").SetValue(true)));
                Menu.Timers.MenuItems.Add(
                    Menu.Timers.Menu.AddItem(
                        new MenuItem("SAwarenessTimersChatChoice", "鑱婂ぉ閫夋嫨").SetValue(
                            new StringList(new[] { "涓嶇敤", "鏈湴", "姝ｅ父" }))));
                Menu.JungleTimer.Menu =
                    Menu.Timers.Menu.AddSubMenu(new LeagueSharp.Common.Menu("閲庡尯璁℃椂", "SAwarenessJungleTimer"));
                Menu.JungleTimer.MenuItems.Add(
                    Menu.JungleTimer.Menu.AddItem(new MenuItem("SAwarenessJungleTimersActive", "鎵撳紑").SetValue(false)));
                Menu.RelictTimer.Menu =
                    Menu.Timers.Menu.AddSubMenu(new LeagueSharp.Common.Menu("鍙潃璁℃椂", "SAwarenessRelictTimer"));
                Menu.RelictTimer.MenuItems.Add(
                    Menu.RelictTimer.Menu.AddItem(new MenuItem("SAwarenessRelictTimersActive", "鎵撳紑").SetValue(false)));
                Menu.HealthTimer.Menu =
                    Menu.Timers.Menu.AddSubMenu(new LeagueSharp.Common.Menu("鐢熷懡璁℃椂", "SAwarenessHealthTimer"));
                Menu.HealthTimer.MenuItems.Add(
                    Menu.HealthTimer.Menu.AddItem(new MenuItem("SAwarenessHealthTimersActive", "鎵撳紑").SetValue(false)));
                Menu.InhibitorTimer.Menu =
                    Menu.Timers.Menu.AddSubMenu(new LeagueSharp.Common.Menu("姘存櫠璁℃椂", "SAwarenessInhibitorTimer"));
                Menu.InhibitorTimer.MenuItems.Add(
                    Menu.InhibitorTimer.Menu.AddItem(
                        new MenuItem("SAwarenessInhibitorTimersActive", "鎵撳紑").SetValue(false)));
                Menu.AltarTimer.Menu =
                    Menu.Timers.Menu.AddSubMenu(new LeagueSharp.Common.Menu("鍦ｅ潧璁℃椂", "SAwarenessAltarTimer"));
                Menu.AltarTimer.MenuItems.Add(
                    Menu.AltarTimer.Menu.AddItem(new MenuItem("SAwarenessAltarTimersActive", "鎵撳紑").SetValue(false)));
                Menu.ImmuneTimer.Menu =
                    Menu.Timers.Menu.AddSubMenu(new LeagueSharp.Common.Menu("鍏嶇柅璁℃椂", "SAwarenessImmuneTimer"));
                Menu.ImmuneTimer.MenuItems.Add(
                    Menu.ImmuneTimer.Menu.AddItem(new MenuItem("SAwarenessImmuneTimersActive", "鎵撳紑").SetValue(false)));
                Menu.SummonerTimer.Menu =
                    Menu.Timers.Menu.AddSubMenu(new LeagueSharp.Common.Menu("鍙敜璁℃椂", "SAwarenessSummonerTimer"));
                Menu.SummonerTimer.MenuItems.Add(
                    Menu.SummonerTimer.Menu.AddItem(new MenuItem("SAwarenessSummonerTimersActive", "鎵撳紑").SetValue(false)));
                Menu.Timers.MenuItems.Add(
                    Menu.Timers.Menu.AddItem(new MenuItem("SAwarenessTimersActive", "鎵撳紑").SetValue(false)));

                //Not crashing     
                Menu.Range.Menu = menu.AddSubMenu(new LeagueSharp.Common.Menu("鑼冨洿", "SAwarenessRanges"));
                Menu.ShopRange.Menu =
                    Menu.Range.Menu.AddSubMenu(new LeagueSharp.Common.Menu("鍟嗗簵鑼冨洿",
                        "SAwarenessShopRange"));
                Menu.ShopRange.MenuItems.Add(
                    Menu.ShopRange.Menu.AddItem(
                        new MenuItem("SAwarenessShopRangeMode", "妯″紡").SetValue(
                            new StringList(new[] { "鑷繁", "鏁屾柟", "鍙屾柟" }))));
                Menu.ShopRange.MenuItems.Add(
                    Menu.ShopRange.Menu.AddItem(
                        new MenuItem("SAwarenessShopRangeActive", "鎵撳紑").SetValue(false)));
                Menu.VisionRange.Menu =
                    Menu.Range.Menu.AddSubMenu(new LeagueSharp.Common.Menu("瑙嗛噹鑼冨洿",
                        "SAwarenessVisionRange"));
                Menu.VisionRange.MenuItems.Add(
                    Menu.VisionRange.Menu.AddItem(
                        new MenuItem("SAwarenessVisionRangeMode", "妯″紡").SetValue(
                            new StringList(new[] { "鑷繁", "鏁屾柟", "鍙屾柟" }))));
                Menu.VisionRange.MenuItems.Add(
                    Menu.VisionRange.Menu.AddItem(
                        new MenuItem("SAwarenessVisionRangeActive", "鎵撳紑").SetValue(false)));
                Menu.ExperienceRange.Menu =
                    Menu.Range.Menu.AddSubMenu(new LeagueSharp.Common.Menu("缁忛獙鑼冨洿",
                        "SAwarenessExperienceRange"));
                Menu.ExperienceRange.MenuItems.Add(
                    Menu.ExperienceRange.Menu.AddItem(
                        new MenuItem("SAwarenessExperienceRangeMode", "妯″紡").SetValue(
                            new StringList(new[] { "鑷繁", "鏁屾柟", "鍙屾柟" }))));
                Menu.ExperienceRange.MenuItems.Add(
                    Menu.ExperienceRange.Menu.AddItem(
                        new MenuItem("SAwarenessExperienceRangeActive", "鎵撳紑").SetValue(false)));
                Menu.AttackRange.Menu =
                    Menu.Range.Menu.AddSubMenu(new LeagueSharp.Common.Menu("鏀诲嚮鑼冨洿", "SAwarenessAttackRange"));
                Menu.AttackRange.MenuItems.Add(
                    Menu.AttackRange.Menu.AddItem(
                        new MenuItem("SAwarenessAttackRangeMode", "妯″紡").SetValue(
                            new StringList(new[] { "鑷繁", "鏁屾柟", "鍙屾柟" }))));
                Menu.AttackRange.MenuItems.Add(
                    Menu.AttackRange.Menu.AddItem(new MenuItem("SAwarenessAttackRangeActive", "鎵撳紑").SetValue(false)));
                Menu.TowerRange.Menu =
                    Menu.Range.Menu.AddSubMenu(new LeagueSharp.Common.Menu("鐐鑼冨洿", "SAwarenessTowerRange"));
                Menu.TowerRange.MenuItems.Add(
                    Menu.TowerRange.Menu.AddItem(
                        new MenuItem("SAwarenessTowerRangeMode", "妯″紡").SetValue(
                            new StringList(new[] { "鑷繁", "鏁屾柟", "鍙屾柟" }))));
                Menu.TowerRange.MenuItems.Add(
                    Menu.TowerRange.Menu.AddItem(new MenuItem("SAwarenessTowerRangeRange", "鑼冨洿").SetValue(new Slider(2000, 10000,
                            0))));
                Menu.TowerRange.MenuItems.Add(
                    Menu.TowerRange.Menu.AddItem(new MenuItem("SAwarenessTowerRangeActive", "鎵撳紑").SetValue(false)));
                Menu.SpellQRange.Menu =
                    Menu.Range.Menu.AddSubMenu(new LeagueSharp.Common.Menu("Q鑼冨洿", "SAwarenessSpellQRange"));
                Menu.SpellQRange.MenuItems.Add(
                    Menu.SpellQRange.Menu.AddItem(
                        new MenuItem("SAwarenessSpellQRangeMode", "妯″紡").SetValue(
                            new StringList(new[] { "鑷繁", "鏁屾柟", "鍙屾柟" }))));
                Menu.SpellQRange.MenuItems.Add(
                    Menu.SpellQRange.Menu.AddItem(new MenuItem("SAwarenessSpellQRangeActive", "鎵撳紑").SetValue(false)));
                Menu.SpellWRange.Menu =
                    Menu.Range.Menu.AddSubMenu(new LeagueSharp.Common.Menu("W鑼冨洿", "SAwarenessSpellWRange"));
                Menu.SpellWRange.MenuItems.Add(
                    Menu.SpellWRange.Menu.AddItem(
                        new MenuItem("SAwarenessSpellWRangeMode", "妯″紡").SetValue(
                            new StringList(new[] { "鑷繁", "鏁屾柟", "鍙屾柟" }))));
                Menu.SpellWRange.MenuItems.Add(
                    Menu.SpellWRange.Menu.AddItem(new MenuItem("SAwarenessSpellWRangeActive", "鎵撳紑").SetValue(false)));
                Menu.SpellERange.Menu =
                    Menu.Range.Menu.AddSubMenu(new LeagueSharp.Common.Menu("E鑼冨洿", "SAwarenessSpellERange"));
                Menu.SpellERange.MenuItems.Add(
                    Menu.SpellERange.Menu.AddItem(
                        new MenuItem("SAwarenessSpellERangeMode", "妯″紡").SetValue(
                            new StringList(new[] { "鑷繁", "鏁屾柟", "鍙屾柟" }))));
                Menu.SpellERange.MenuItems.Add(
                    Menu.SpellERange.Menu.AddItem(new MenuItem("SAwarenessSpellERangeActive", "鎵撳紑").SetValue(false)));
                Menu.SpellRRange.Menu =
                    Menu.Range.Menu.AddSubMenu(new LeagueSharp.Common.Menu("R鑼冨洿", "SAwarenessSpellRRange"));
                Menu.SpellRRange.MenuItems.Add(
                    Menu.SpellRRange.Menu.AddItem(
                        new MenuItem("SAwarenessSpellRRangeMode", "妯″紡").SetValue(
                            new StringList(new[] { "鑷繁", "鏁屾柟", "鍙屾柟" }))));
                Menu.SpellRRange.MenuItems.Add(
                    Menu.SpellRRange.Menu.AddItem(new MenuItem("SAwarenessSpellRRangeActive", "鎵撳紑").SetValue(false)));
                Menu.Range.MenuItems.Add(
                    Menu.Range.Menu.AddItem(new MenuItem("SAwarenessRangesActive", "鎵撳紑").SetValue(false)));
               
                //Not crashing
                                         
                 Menu.Tracker.Menu = menu.AddSubMenu(new LeagueSharp.Common.Menu("杩借釜", "SAwarenessTracker"));
                Menu.WaypointTracker.Menu =
                    Menu.Tracker.Menu.AddSubMenu(new LeagueSharp.Common.Menu("璺緞杩借釜",
                        "SAwarenessWaypointTracker"));
                Menu.WaypointTracker.MenuItems.Add(
                    Menu.WaypointTracker.Menu.AddItem(
                        new MenuItem("SAwarenessWaypointTrackerActive", "鎵撳紑").SetValue(false)));
                Menu.DestinationTracker.Menu =
                    Menu.Tracker.Menu.AddSubMenu(new LeagueSharp.Common.Menu("缁堢偣杩借釜",
                        "SAwarenessDestinationTracker"));
                Menu.DestinationTracker.MenuItems.Add(
                    Menu.DestinationTracker.Menu.AddItem(
                        new MenuItem("SAwarenessDestinationTrackerActive", "鎵撳紑").SetValue(false)));
                Menu.CloneTracker.Menu =
                    Menu.Tracker.Menu.AddSubMenu(new LeagueSharp.Common.Menu("鍒嗚韩", "SAwarenessCloneTracker"));
                Menu.CloneTracker.MenuItems.Add(
                    Menu.CloneTracker.Menu.AddItem(new MenuItem("SAwarenessCloneTrackerActive", "鎵撳紑").SetValue(false)));
                Menu.UiTracker.Menu =
                    Menu.Tracker.Menu.AddSubMenu(new LeagueSharp.Common.Menu("鏄剧ずCD", "SAwarenessUITracker"));
                Menu.UiTracker.MenuItems.Add(
                    Menu.UiTracker.Menu.AddItem(new MenuItem("SAwarenessItemPanelActive", "闈㈡澘").SetValue(false)));
                Menu.UiTracker.MenuItems.Add(
                    Menu.UiTracker.Menu.AddItem(
                        new MenuItem("SAwarenessUITrackerScale", "姣斾緥").SetValue(new Slider(100, 100, 0))));
                tempSettings = Menu.UiTracker.AddMenuItemSettings("鏁屾柟CD",
                    "SAwarenessUITrackerEnemyTracker");
                tempSettings.MenuItems.Add(
                    tempSettings.Menu.AddItem(
                        new MenuItem("SAwarenessUITrackerEnemyTrackerXPos", "X鍧愭爣").SetValue(new Slider(-1, 10000,
                            0))));
                tempSettings.MenuItems.Add(
                    tempSettings.Menu.AddItem(
                        new MenuItem("SAwarenessUITrackerEnemyTrackerYPos", "Y鍧愭爣").SetValue(new Slider(-1, 10000,
                            0))));
                tempSettings.MenuItems.Add(
                    tempSettings.Menu.AddItem(
                        new MenuItem("SAwarenessUITrackerEnemyTrackerMode", "妯″紡").SetValue(
                            new StringList(new[] { "闄勮繎", "鍗曚釜", "鍏ㄩ儴" }))));
                tempSettings.MenuItems.Add(
                    tempSettings.Menu.AddItem(
                        new MenuItem("SAwarenessUITrackerEnemyTrackerSideDisplayMode", "鏃佽竟鏄剧ず").SetValue(
                            new StringList(new[] { "榛樿", "绠€鍗曘劎", "鍥綋" }))));
                tempSettings.MenuItems.Add(
                    tempSettings.Menu.AddItem(
                        new MenuItem("SAwarenessUITrackerEnemyTrackerHeadMode", "姣斾緥").SetValue(
                            new StringList(new[] { "杈冨皬", "杈冨ぇ" }))));
                tempSettings.MenuItems.Add(
                    tempSettings.Menu.AddItem(
                        new MenuItem("SAwarenessUITrackerEnemyTrackerHeadDisplayMode", "澶撮《鏄剧ず").SetValue(
                            new StringList(new[] { "榛樿", "绠€鍗曘劎" }))));
                tempSettings.MenuItems.Add(
                    tempSettings.Menu.AddItem(
                        new MenuItem("SAwarenessUITrackerEnemyTrackerActive", "鎵撳紑").SetValue(false)));
                tempSettings = Menu.UiTracker.AddMenuItemSettings("闃熷弸CD",
                    "SAwarenessUITrackerAllyTracker");
                tempSettings.MenuItems.Add(
                    tempSettings.Menu.AddItem(
                        new MenuItem("SAwarenessUITrackerAllyTrackerXPos", "X鍧愭爣").SetValue(new Slider(-1, 10000,
                            0))));
                tempSettings.MenuItems.Add(
                    tempSettings.Menu.AddItem(
                        new MenuItem("SAwarenessUITrackerAllyTrackerYPos", "Y鍧愭爣").SetValue(new Slider(-1, 10000,
                            0))));
                tempSettings.MenuItems.Add(
                    tempSettings.Menu.AddItem(
                        new MenuItem("SAwarenessUITrackerAllyTrackerMode", "妯″紡").SetValue(
                            new StringList(new[] { "闄勮繎", "鍗曚釜", "鍏ㄩ儴" }))));
                tempSettings.MenuItems.Add(
                    tempSettings.Menu.AddItem(
                        new MenuItem("SAwarenessUITrackerAllyTrackerSideDisplayMode", "鏃佽竟鏄剧ず").SetValue(
                            new StringList(new[] { "榛樿", "绠€鍗曘劎", "鍥㈤槦" }))));
                tempSettings.MenuItems.Add(
                    tempSettings.Menu.AddItem(
                        new MenuItem("SAwarenessUITrackerAllyTrackerHeadMode", "姣斾緥").SetValue(
                            new StringList(new[] { "杈冨皬", "杈冨ぇ" }))));
                tempSettings.MenuItems.Add(
                    tempSettings.Menu.AddItem(
                        new MenuItem("SAwarenessUITrackerAllyTrackerHeadDisplayMode", "澶撮《鏄剧ず").SetValue
                            (new StringList(new[] { "榛樿", "绠€鍗曘劎" }))));
                tempSettings.MenuItems.Add(
                    tempSettings.Menu.AddItem(
                        new MenuItem("SAwarenessUITrackerAllyTrackerActive", "鎵撳紑").SetValue(false)));
                //Menu.UiTracker.MenuItems.Add(Menu.UiTracker.Menu.AddItem(new LeagueSharp.Common.MenuItem("SAwarenessUITrackerCameraMoveActive", "Camera move active").SetValue(false)));
                Menu.UiTracker.MenuItems.Add(
                    Menu.UiTracker.Menu.AddItem(
                        new MenuItem("SAwarenessUITrackerPingActive", "Ping鎵撳紑").SetValue(false)));
                Menu.UiTracker.MenuItems.Add(
                    Menu.UiTracker.Menu.AddItem(new MenuItem("SAwarenessUITrackerActive", "鎵撳紑").SetValue(false)));
                Menu.UimTracker.Menu =
                    Menu.Tracker.Menu.AddSubMenu(new LeagueSharp.Common.Menu("娑堝け浣嶇疆", "SAwarenessUIMTracker"));
                Menu.UimTracker.MenuItems.Add(
                    Menu.UimTracker.Menu.AddItem(
                        new MenuItem("SAwarenessUIMTrackerScale", "姣斾緥").SetValue(new Slider(100, 100, 0))));
                Menu.UimTracker.MenuItems.Add(
                    Menu.UimTracker.Menu.AddItem(new MenuItem("SAwarenessUIMTrackerShowSS", "鏁屼汉娑堝け鎻愮ず").SetValue(false)));
                Menu.UimTracker.MenuItems.Add(
                    Menu.UimTracker.Menu.AddItem(new MenuItem("SAwarenessUIMTrackerActive", "鎵撳紑").SetValue(false)));
                Menu.SsCaller.Menu =
                    Menu.Tracker.Menu.AddSubMenu(new LeagueSharp.Common.Menu("鏁屼汉娑堝けPing", "SAwarenessSSCaller"));
                Menu.SsCaller.MenuItems.Add(
                    Menu.SsCaller.Menu.AddItem(
                        new MenuItem("SAwarenessSSCallerPingTimes", "Ping娆℃暟").SetValue(new Slider(0, 5, 0))));
                Menu.SsCaller.MenuItems.Add(
                    Menu.SsCaller.Menu.AddItem(
                        new MenuItem("SAwarenessSSCallerPingType", "Ping绫诲瀷").SetValue(
                            new StringList(new[] { "姝ｅ父", "鍗遍櫓", "Miss", "鍦ㄨ矾涓妡", "鎾ら€€", "鍗忓姪" }))));
                Menu.SsCaller.MenuItems.Add(
                    Menu.SsCaller.Menu.AddItem(new MenuItem("SAwarenessSSCallerLocalPing", "鏈湴Ping").SetValue(false)));
                Menu.SsCaller.MenuItems.Add(
                    Menu.SsCaller.Menu.AddItem(
                        new MenuItem("SAwarenessSSCallerChatChoice", "鑱婂ぉ閫夋嫨").SetValue(
                            new StringList(new[] { "涓嶇敤", "鏈湴", "姝ｅ父" }))));
                Menu.SsCaller.MenuItems.Add(
                    Menu.SsCaller.Menu.AddItem(
                        new MenuItem("SAwarenessSSCallerDisableTime", "绂佺敤鏃堕棿").SetValue(new Slider(20, 180, 1))));
                Menu.SsCaller.MenuItems.Add(
                    Menu.SsCaller.Menu.AddItem(new MenuItem("SAwarenessSSCallerActive", "鎵撳紑").SetValue(false)));
                Menu.Killable.Menu =
                    Menu.Tracker.Menu.AddSubMenu(new LeagueSharp.Common.Menu("鍙潃", "SAwarenessKillable"));
                Menu.Killable.MenuItems.Add(
                    Menu.Killable.Menu.AddItem(new MenuItem("SAwarenessKillableActive", "鎵撳紑").SetValue(false)));
                Menu.Tracker.MenuItems.Add(
                    Menu.Tracker.Menu.AddItem(new MenuItem("SAwarenessTrackerActive", "鎵撳紑").SetValue(false)));
          
                //Not crashing
                Menu.Detector.Menu = menu.AddSubMenu(new LeagueSharp.Common.Menu("鎺㈡祴", "SAwarenessDetector"));
                Menu.VisionDetector.Menu =
                    Menu.Detector.Menu.AddSubMenu(new LeagueSharp.Common.Menu("瑙嗛噹鎺㈡祴",
                        "SAwarenessVisionDetector"));
                Menu.VisionDetector.MenuItems.Add(
                    Menu.VisionDetector.Menu.AddItem(
                        new MenuItem("SAwarenessVisionDetectorDrawRange", "鏄剧ず鑼冨洿").SetValue(false)));
                Menu.VisionDetector.MenuItems.Add(
                    Menu.VisionDetector.Menu.AddItem(
                        new MenuItem("SAwarenessVisionDetectorActive", "鎵撳紑").SetValue(false)));
                Menu.RecallDetector.Menu =
                    Menu.Detector.Menu.AddSubMenu(new LeagueSharp.Common.Menu("鍥炲煄鎺㈡祴",
                        "SAwarenessRecallDetector"));
                Menu.RecallDetector.MenuItems.Add(
                    Menu.RecallDetector.Menu.AddItem(
                        new MenuItem("SAwarenessRecallDetectorPingTimes", "Ping娆℃暟").SetValue(new Slider(0, 5, 0))));
                Menu.RecallDetector.MenuItems.Add(
                    Menu.RecallDetector.Menu.AddItem(
                        new MenuItem("SAwarenessRecallDetectorLocalPing", "鏈湴Ping").SetValue(true)));
                Menu.RecallDetector.MenuItems.Add(
                    Menu.RecallDetector.Menu.AddItem(
                        new MenuItem("SAwarenessRecallDetectorChatChoice", "鑱婂ぉ閫夋嫨").SetValue(
                            new StringList(new[] { "涓嶇敤", "鏈湴", "姝ｅ父" }))));
                Menu.RecallDetector.MenuItems.Add(
                    Menu.RecallDetector.Menu.AddItem(
                        new MenuItem("SAwarenessRecallDetectorMode", "妯″紡").SetValue(
                            new StringList(new[] { "鑱婂ぉ绐楀彛", "CD杩借釜鍣▅", "鍏ㄩ儴" }))));
                Menu.RecallDetector.MenuItems.Add(
                    Menu.RecallDetector.Menu.AddItem(
                        new MenuItem("SAwarenessRecallDetectorActive", "鎵撳紑").SetValue(false)));
                Menu.Detector.MenuItems.Add(
                    Menu.Detector.Menu.AddItem(new MenuItem("SAwarenessDetectorActive", "鎵撳紑").SetValue(false)));
                
                //Not crashing
                Menu.Ganks.Menu = menu.AddSubMenu(new LeagueSharp.Common.Menu("Ganks", "SAwarenessGanks"));
                Menu.GankTracker.Menu =
                    Menu.Ganks.Menu.AddSubMenu(new LeagueSharp.Common.Menu("Gank杩借釜", "SAwarenessGankTracker"));
                Menu.GankTracker.MenuItems.Add(
                    Menu.GankTracker.Menu.AddItem(
                        new MenuItem("SAwarenessGankTrackerTrackRange", "杩借釜鑼冨洿").SetValue(new Slider(1, 20000, 1))));
                Menu.GankTracker.MenuItems.Add(
                   Menu.GankTracker.Menu.AddItem(new MenuItem("SAwarenessGankTrackerKillable", "鍙潃鎻愮ず").SetValue(false)));
                Menu.GankTracker.MenuItems.Add(
                    Menu.GankTracker.Menu.AddItem(new MenuItem("SAwarenessGankTrackerActive", "鎵撳紑").SetValue(false)));
                Menu.GankDetector.Menu =
                    Menu.Ganks.Menu.AddSubMenu(new LeagueSharp.Common.Menu("Gank鎺㈡祴", "SAwarenessGankDetector"));
                Menu.GankDetector.MenuItems.Add(
                    Menu.GankDetector.Menu.AddItem(
                        new MenuItem("SAwarenessGankDetectorPingTimes", "Ping娆℃暟").SetValue(new Slider(0, 5, 0))));
                Menu.GankDetector.MenuItems.Add(
                    Menu.GankDetector.Menu.AddItem(
                        new MenuItem("SAwarenessGankDetectorPingType", "Ping绫诲瀷").SetValue(
                            new StringList(new[] { "姝ｅ父", "鍗遍櫓", "MISS", "鍦ㄨ矾涓妡", "鎾ら€€", "鍗忓姪" }))));
                Menu.GankDetector.MenuItems.Add(
                    Menu.GankDetector.Menu.AddItem(
                        new MenuItem("SAwarenessGankDetectorLocalPing", "鏈湴Ping").SetValue(true)));
                Menu.GankDetector.MenuItems.Add(
                    Menu.GankDetector.Menu.AddItem(
                        new MenuItem("SAwarenessGankDetectorChatChoice", "鑱婂ぉ閫夋嫨").SetValue(
                            new StringList(new[] { "涓嶇敤", "鏈湴", "姝ｅ父" }))));
                Menu.GankDetector.MenuItems.Add(
                    Menu.GankDetector.Menu.AddItem(
                        new MenuItem("SAwarenessGankDetectorTrackRange", "杩借釜鑼冨洿").SetValue(new Slider(1, 10000, 1))));
                Menu.GankDetector.MenuItems.Add(
                    Menu.GankDetector.Menu.AddItem(new MenuItem("SAwarenessGankDetectorShowJungler", "鏄剧ず鎵撻噹").SetValue(false)));
                Menu.GankDetector.MenuItems.Add(
                    Menu.GankDetector.Menu.AddItem(new MenuItem("SAwarenessGankDetectorActive", "鎵撳紑").SetValue(false)));
                Menu.Ganks.MenuItems.Add(
                    Menu.Ganks.Menu.AddItem(new MenuItem("SAwarenessGanksActive", "鎵撳紑").SetValue(false)));
            
                //Not crashing
                Menu.Health.Menu =
                    menu.AddSubMenu(new LeagueSharp.Common.Menu("寤虹瓚", "SAwarenessObjectHealth"));
                Menu.TowerHealth.Menu =
                    Menu.Health.Menu.AddSubMenu(new LeagueSharp.Common.Menu("鐐鐢熷懡", "SAwarenessTowerHealth"));
                Menu.TowerHealth.MenuItems.Add(
                    Menu.TowerHealth.Menu.AddItem(new MenuItem("SAwarenessTowerHealthActive", "鎵撳紑").SetValue(false)));
                Menu.InhibitorHealth.Menu =
                    Menu.Health.Menu.AddSubMenu(new LeagueSharp.Common.Menu("姘存櫠鐢熷懡",
                        "SAwarenessInhibitorHealth"));
                Menu.InhibitorHealth.MenuItems.Add(
                    Menu.InhibitorHealth.Menu.AddItem(
                        new MenuItem("SAwarenessInhibitorHealthActive", "鎵撳紑").SetValue(false)));
                Menu.Health.MenuItems.Add(
                    Menu.Health.Menu.AddItem(
                        new MenuItem("SAwarenessHealthMode", "妯″紡").SetValue(new StringList(new[] { "%", "姝ｅ父" }))));
                Menu.Health.MenuItems.Add(
                    Menu.Health.Menu.AddItem(new MenuItem("SAwarenessHealthActive", "鎵撳紑").SetValue(false)));
                
                //Not crashing
                Menu.Wards.Menu = menu.AddSubMenu(new LeagueSharp.Common.Menu("鐪间綅", "SAwarenessWards"));
                Menu.WardCorrector.Menu =
                    Menu.Wards.Menu.AddSubMenu(new LeagueSharp.Common.Menu("鐪间綅淇", "SAwarenessWardCorrector"));
                Menu.WardCorrector.MenuItems.Add(
                    Menu.WardCorrector.Menu.AddItem(
                        new MenuItem("SAwarenessWardCorrectorKey", "鐑敭").SetValue(new KeyBind(52, KeyBindType.Press))));
                Menu.WardCorrector.MenuItems.Add(
                    Menu.WardCorrector.Menu.AddItem(
                        new MenuItem("SAwarenessWardCorrectorActive", "鎵撳紑").SetValue(false)));
                Menu.BushRevealer.Menu =
                    Menu.Wards.Menu.AddSubMenu(new LeagueSharp.Common.Menu("鑽変笡", "SAwarenessBushRevealer"));
                Menu.BushRevealer.MenuItems.Add(
                    Menu.BushRevealer.Menu.AddItem(
                        new MenuItem("SAwarenessBushRevealerKey", "鐑敭").SetValue(new KeyBind(32, KeyBindType.Press))));
                Menu.BushRevealer.MenuItems.Add(
                    Menu.BushRevealer.Menu.AddItem(new MenuItem("SAwarenessBushRevealerActive", "鎵撳紑").SetValue(false)));
                Menu.BushRevealer.MenuItems.Add(
                    Menu.BushRevealer.Menu.AddItem(new MenuItem("By Beaving & Blm95", "By Beaving & Blm95")));
                Menu.InvisibleRevealer.Menu =
                    Menu.Wards.Menu.AddSubMenu(new LeagueSharp.Common.Menu("闅愬舰鎺㈡祴",
                        "SAwarenessInvisibleRevealer"));
                Menu.InvisibleRevealer.MenuItems.Add(
                    Menu.InvisibleRevealer.Menu.AddItem(
                        new MenuItem("SAwarenessInvisibleRevealerMode", "妯″紡").SetValue(
                            new StringList(new[] { "鎵嬪姩", "鑷姩" }))));
                Menu.InvisibleRevealer.MenuItems.Add(
                    Menu.InvisibleRevealer.Menu.AddItem(
                        new MenuItem("SAwarenessInvisibleRevealerKey", "鐑敭").SetValue(new KeyBind(32, KeyBindType.Press))));
                Menu.InvisibleRevealer.MenuItems.Add(
                    Menu.InvisibleRevealer.Menu.AddItem(
                        new MenuItem("SAwarenessInvisibleRevealerActive", "鎵撳紑").SetValue(false)));
                Menu.Wards.MenuItems.Add(
                    Menu.Wards.Menu.AddItem(new MenuItem("SAwarenessWardsActive", "鎵撳紑").SetValue(false)));
                
                Menu.Activator.Menu = menu.AddSubMenu(new LeagueSharp.Common.Menu("娲诲寲鍓倈", "SAwarenessActivator"));
                Menu.ActivatorAutoSummonerSpell.Menu =
                    Menu.Activator.Menu.AddSubMenu(new LeagueSharp.Common.Menu("鑷姩娉曟湳",
                        "SAwarenessActivatorAutoSummonerSpell"));
                Menu.ActivatorAutoSummonerSpell.MenuItems.Add(
                    Menu.ActivatorAutoSummonerSpell.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorActivatorAutoSummonerSpellActive", "鎵撳紑").SetValue(false)));
                Menu.ActivatorAutoSummonerSpellIgnite.Menu =
                    Menu.ActivatorAutoSummonerSpell.Menu.AddSubMenu(new LeagueSharp.Common.Menu("鑷姩鐐圭噧",
                        "SAwarenessActivatorAutoSummonerSpellIgnite"));
                Menu.ActivatorAutoSummonerSpellIgnite.MenuItems.Add(
                    Menu.ActivatorAutoSummonerSpellIgnite.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorAutoSummonerSpellIgniteActive", "鎵撳紑").SetValue(false)));
                Menu.ActivatorAutoSummonerSpellHeal.Menu =
                    Menu.ActivatorAutoSummonerSpell.Menu.AddSubMenu(new LeagueSharp.Common.Menu("鑷姩娌荤枟",
                        "SAwarenessActivatorAutoSummonerSpellHeal"));
                Menu.ActivatorAutoSummonerSpellHeal.MenuItems.Add(
                    Menu.ActivatorAutoSummonerSpellHeal.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorAutoSummonerSpellHealPercent", "%").SetValue(
                            new Slider(20, 100, 1))));
                Menu.ActivatorAutoSummonerSpellHeal.MenuItems.Add(
                    Menu.ActivatorAutoSummonerSpellHeal.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorAutoSummonerSpellHealAllyActive", "闃熷弸").SetValue(false)));
                Menu.ActivatorAutoSummonerSpellHeal.MenuItems.Add(
                    Menu.ActivatorAutoSummonerSpellHeal.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorAutoSummonerSpellHealActive", "鎵撳紑").SetValue(false)));
                Menu.ActivatorAutoSummonerSpellBarrier.Menu =
                    Menu.ActivatorAutoSummonerSpell.Menu.AddSubMenu(new LeagueSharp.Common.Menu("鑷姩灞忛殰",
                        "SAwarenessActivatorAutoSummonerSpellBarrier"));
                Menu.ActivatorAutoSummonerSpellBarrier.MenuItems.Add(
                    Menu.ActivatorAutoSummonerSpellBarrier.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorAutoSummonerSpellBarrierPercent", "%").SetValue(
                            new Slider(20, 100, 1))));
                Menu.ActivatorAutoSummonerSpellBarrier.MenuItems.Add(
                    Menu.ActivatorAutoSummonerSpellBarrier.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorAutoSummonerSpellBarrierActive", "鎵撳紑").SetValue(false)));
                Menu.ActivatorAutoSummonerSpellExhaust.Menu =
                    Menu.ActivatorAutoSummonerSpell.Menu.AddSubMenu(new LeagueSharp.Common.Menu("鑷姩铏氬急",
                        "SAwarenessActivatorAutoSummonerSpellExhaust"));
                Menu.ActivatorAutoSummonerSpellExhaust.MenuItems.Add(
                    Menu.ActivatorAutoSummonerSpellExhaust.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorAutoSummonerSpellExhaustAutoCast", "鐑敭").SetValue(
                            new KeyBind(32, KeyBindType.Press))));
                Menu.ActivatorAutoSummonerSpellExhaust.MenuItems.Add(
                    Menu.ActivatorAutoSummonerSpellExhaust.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorAutoSummonerSpellExhaustMinEnemies", "鏁屼汉鏁伴噺").SetValue(
                            new Slider(3, 5, 1))));
                Menu.ActivatorAutoSummonerSpellExhaust.MenuItems.Add(
                    Menu.ActivatorAutoSummonerSpellExhaust.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorAutoSummonerSpellExhaustAllyPercent", "闃熷弸").SetValue(
                            new Slider(20, 100, 1))));
                Menu.ActivatorAutoSummonerSpellExhaust.MenuItems.Add(
                    Menu.ActivatorAutoSummonerSpellExhaust.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorAutoSummonerSpellExhaustSelfPercent", "鑷繁").SetValue(
                            new Slider(20, 100, 1))));
                Menu.ActivatorAutoSummonerSpellExhaust.MenuItems.Add(
                    Menu.ActivatorAutoSummonerSpellExhaust.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorAutoSummonerSpellExhaustUseUltSpells", "澶ф嫑").SetValue(
                            false)));
                Menu.ActivatorAutoSummonerSpellExhaust.MenuItems.Add(
                    Menu.ActivatorAutoSummonerSpellExhaust.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorAutoSummonerSpellExhaustActive", "鎵撳紑").SetValue(false)));
                Menu.ActivatorAutoSummonerSpellCleanse.Menu =
                    Menu.ActivatorAutoSummonerSpell.Menu.AddSubMenu(new LeagueSharp.Common.Menu("鍑€鍖栭€夐」",
                        "SAwarenessActivatorAutoSummonerSpellCleanse"));
                Menu.ActivatorAutoSummonerSpellCleanse.MenuItems.Add(
                    Menu.ActivatorAutoSummonerSpellCleanse.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorAutoSummonerSpellCleanseStun", "鐪╂檿").SetValue(false)));
                Menu.ActivatorAutoSummonerSpellCleanse.MenuItems.Add(
                    Menu.ActivatorAutoSummonerSpellCleanse.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorAutoSummonerSpellCleanseSilence", "娌夐粯").SetValue(false)));
                Menu.ActivatorAutoSummonerSpellCleanse.MenuItems.Add(
                    Menu.ActivatorAutoSummonerSpellCleanse.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorAutoSummonerSpellCleanseTaunt", "鍢茶").SetValue(false)));
                Menu.ActivatorAutoSummonerSpellCleanse.MenuItems.Add(
                    Menu.ActivatorAutoSummonerSpellCleanse.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorAutoSummonerSpellCleanseFear", "鎭愭儳").SetValue(false)));
                Menu.ActivatorAutoSummonerSpellCleanse.MenuItems.Add(
                    Menu.ActivatorAutoSummonerSpellCleanse.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorAutoSummonerSpellCleanseCharm", "榄呮儜").SetValue(false)));
                Menu.ActivatorAutoSummonerSpellCleanse.MenuItems.Add(
                    Menu.ActivatorAutoSummonerSpellCleanse.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorAutoSummonerSpellCleanseBlind", "鑷寸洸").SetValue(false)));
                Menu.ActivatorAutoSummonerSpellCleanse.MenuItems.Add(
                    Menu.ActivatorAutoSummonerSpellCleanse.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorAutoSummonerSpellCleanseDisarm", "鐐稿脊").SetValue(false)));
                Menu.ActivatorAutoSummonerSpellCleanse.MenuItems.Add(
                    Menu.ActivatorAutoSummonerSpellCleanse.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorAutoSummonerSpellCleanseSlow", "鍑忛€焲").SetValue(false)));
                Menu.ActivatorAutoSummonerSpellCleanse.MenuItems.Add(
                    Menu.ActivatorAutoSummonerSpellCleanse.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorAutoSummonerSpellCleanseCombatDehancer", "铏氬急")
                            .SetValue(false)));
                Menu.ActivatorAutoSummonerSpellCleanse.MenuItems.Add(
                    Menu.ActivatorAutoSummonerSpellCleanse.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorAutoSummonerSpellCleanseSnare", "闄烽槺").SetValue(false)));
                Menu.ActivatorAutoSummonerSpellCleanse.MenuItems.Add(
                    Menu.ActivatorAutoSummonerSpellCleanse.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorAutoSummonerSpellCleansePoison", "涓瘨").SetValue(false)));
                Menu.ActivatorAutoSummonerSpellCleanse.MenuItems.Add(
                    Menu.ActivatorAutoSummonerSpellCleanse.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorAutoSummonerSpellCleanseMinSpells", "娉曟湳>X").SetValue(
                            new Slider(2, 10, 1))));
                Menu.ActivatorAutoSummonerSpellCleanse.MenuItems.Add(
                    Menu.ActivatorAutoSummonerSpellCleanse.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorAutoSummonerSpellCleanseActive", "鎵撳紑").SetValue(false)));
                Menu.AutoSmite.Menu =
                    Menu.ActivatorAutoSummonerSpell.Menu.AddSubMenu(new LeagueSharp.Common.Menu("鑷姩鎯╂垝",
                        "SAwarenessAutoSmite"));
                Menu.AutoSmite.MenuItems.Add(
                    Menu.AutoSmite.Menu.AddItem(
                        new MenuItem("SAwarenessAutoSmiteSmallCampsActive", "鎯╂垝灏忛噹").SetValue(false)));
                Menu.AutoSmite.MenuItems.Add(
                    Menu.AutoSmite.Menu.AddItem(
                        new MenuItem("SAwarenessAutoSmiteAutoSpell", "浣跨敤鑷姩娉曟湳").SetValue(false)));
                Menu.AutoSmite.MenuItems.Add(
                    Menu.AutoSmite.Menu.AddItem(
                        new MenuItem("SAwarenessAutoSmiteKeyActive", "鐑敭").SetValue(new KeyBind(78, KeyBindType.Toggle))));
                Menu.AutoSmite.MenuItems.Add(
                    Menu.AutoSmite.Menu.AddItem(new MenuItem("SAwarenessAutoSmiteActive", "鎵撳紑").SetValue(false)));

                Menu.ActivatorOffensive.Menu =
                    Menu.Activator.Menu.AddSubMenu(new LeagueSharp.Common.Menu("鏀诲嚮鐗╁搧",
                        "SAwarenessActivatorOffensive"));
                Menu.ActivatorOffensiveAd.Menu =
                    Menu.ActivatorOffensive.Menu.AddSubMenu(new LeagueSharp.Common.Menu("AD",
                        "SAwarenessActivatorOffensiveAd"));
                Menu.ActivatorOffensiveAd.MenuItems.Add(
                    Menu.ActivatorOffensiveAd.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorOffensiveAdBOTRK", "鐮磋触").SetValue(false)));
                Menu.ActivatorOffensiveAd.MenuItems.Add(
                    Menu.ActivatorOffensiveAd.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorOffensiveAdEntropy", "鍐伴敜").SetValue(false)));
                Menu.ActivatorOffensiveAd.MenuItems.Add(
                    Menu.ActivatorOffensiveAd.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorOffensiveAdRavenousHydra", "涔濆ご铔噟").SetValue(false)));
                Menu.ActivatorOffensiveAd.MenuItems.Add(
                    Menu.ActivatorOffensiveAd.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorOffensiveAdSwordOfTheDevine", "绁炲湥涔嬪墤").SetValue(
                            false)));
                Menu.ActivatorOffensiveAd.MenuItems.Add(
                    Menu.ActivatorOffensiveAd.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorOffensiveAdTiamat", "鎻愪簹椹壒").SetValue(false)));
                Menu.ActivatorOffensiveAd.MenuItems.Add(
                    Menu.ActivatorOffensiveAd.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorOffensiveAdYoumuusGhostblade", "骞芥ⅵ").SetValue(
                            false)));
                //Menu.ActivatorOffensiveAd.MenuItems.Add(Menu.ActivatorOffensiveAd.Menu.AddItem(new LeagueSharp.Common.MenuItem("SAwarenessActivatorOffensiveAdMuramana", "Muramana").SetValue(false)));
                Menu.ActivatorOffensiveAd.MenuItems.Add(
                    Menu.ActivatorOffensiveAd.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorOffensiveAdActive", "鎵撳紑").SetValue(false)));

                Menu.ActivatorOffensiveAp.Menu =
                    Menu.ActivatorOffensive.Menu.AddSubMenu(new LeagueSharp.Common.Menu("AP",
                        "SAwarenessActivatorOffensiveAp"));
                Menu.ActivatorOffensiveAp.MenuItems.Add(
                    Menu.ActivatorOffensiveAp.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorOffensiveApBilgewaterCutlass", "灏忓集鍒€").SetValue(
                            false)));
                Menu.ActivatorOffensiveAp.MenuItems.Add(
                    Menu.ActivatorOffensiveAp.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorOffensiveApBlackfireTorch", "榛値鐏偓 ").SetValue(false)));
                Menu.ActivatorOffensiveAp.MenuItems.Add(
                    Menu.ActivatorOffensiveAp.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorOffensiveApDFG", "鍐ョ伀").SetValue(false)));
                Menu.ActivatorOffensiveAp.MenuItems.Add(
                    Menu.ActivatorOffensiveAp.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorOffensiveApHextechGunblade", "绉戞妧鏋垉").SetValue(false)));
                Menu.ActivatorOffensiveAp.MenuItems.Add(
                    Menu.ActivatorOffensiveAp.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorOffensiveApTwinShadows", "鍙岀敓鏆楀奖").SetValue(false)));
                //Menu.ActivatorOffensiveAp.MenuItems.Add(Menu.ActivatorOffensiveAp.Menu.AddItem(new LeagueSharp.Common.MenuItem("SAwarenessActivatorOffensiveApOdynsVeil", "Odyn's Veil").SetValue(false)));
                Menu.ActivatorOffensiveAp.MenuItems.Add(
                    Menu.ActivatorOffensiveAp.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorOffensiveApActive", "鎵撳紑").SetValue(false)));
                Menu.ActivatorOffensive.MenuItems.Add(
                    Menu.ActivatorOffensive.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorOffensiveKey", "鐑敭").SetValue(new KeyBind(32,
                            KeyBindType.Press))));
                Menu.ActivatorOffensive.MenuItems.Add(
                    Menu.ActivatorOffensive.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorOffensiveActive", "鎵撳紑").SetValue(false)));

                Menu.ActivatorDefensive.Menu =
                    Menu.Activator.Menu.AddSubMenu(new LeagueSharp.Common.Menu("闃插尽鐗╁搧",
                        "SAwarenessActivatorDefensive"));
                Menu.ActivatorDefensiveCleanseConfig.Menu =
                    Menu.ActivatorDefensive.Menu.AddSubMenu(new LeagueSharp.Common.Menu("鍑€鍖栭€夐」",
                        "SAwarenessActivatorDefensiveCleanseConfig"));
                Menu.ActivatorDefensiveCleanseConfig.MenuItems.Add(
                    Menu.ActivatorDefensiveCleanseConfig.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorDefensiveCleanseConfigStun", "鏄忚糠").SetValue(false)));
                Menu.ActivatorDefensiveCleanseConfig.MenuItems.Add(
                    Menu.ActivatorDefensiveCleanseConfig.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorDefensiveCleanseConfigSilence", "娌夐粯").SetValue(false)));
                Menu.ActivatorDefensiveCleanseConfig.MenuItems.Add(
                    Menu.ActivatorDefensiveCleanseConfig.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorDefensiveCleanseConfigTaunt", "鍢茶").SetValue(false)));
                Menu.ActivatorDefensiveCleanseConfig.MenuItems.Add(
                    Menu.ActivatorDefensiveCleanseConfig.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorDefensiveCleanseConfigFear", "鎭愭儳").SetValue(false)));
                Menu.ActivatorDefensiveCleanseConfig.MenuItems.Add(
                    Menu.ActivatorDefensiveCleanseConfig.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorDefensiveCleanseConfigCharm", "榄呮儜").SetValue(false)));
                Menu.ActivatorDefensiveCleanseConfig.MenuItems.Add(
                    Menu.ActivatorDefensiveCleanseConfig.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorDefensiveCleanseConfigBlind", "鑷寸洸").SetValue(false)));
                Menu.ActivatorDefensiveCleanseConfig.MenuItems.Add(
                    Menu.ActivatorDefensiveCleanseConfig.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorDefensiveCleanseConfigDisarm", "鐐稿脊").SetValue(false)));
                Menu.ActivatorDefensiveCleanseConfig.MenuItems.Add(
                    Menu.ActivatorDefensiveCleanseConfig.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorDefensiveCleanseConfigSuppress", "鍘嬪埗").SetValue(false)));
                Menu.ActivatorDefensiveCleanseConfig.MenuItems.Add(
                    Menu.ActivatorDefensiveCleanseConfig.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorDefensiveCleanseConfigSlow", "鍑忛€焲").SetValue(false)));
                Menu.ActivatorDefensiveCleanseConfig.MenuItems.Add(
                    Menu.ActivatorDefensiveCleanseConfig.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorDefensiveCleanseConfigCombatDehancer", "铏氬急")
                            .SetValue(false)));
                Menu.ActivatorDefensiveCleanseConfig.MenuItems.Add(
                    Menu.ActivatorDefensiveCleanseConfig.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorDefensiveCleanseConfigSnare", "闄烽槺").SetValue(false)));
                Menu.ActivatorDefensiveCleanseConfig.MenuItems.Add(
                    Menu.ActivatorDefensiveCleanseConfig.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorDefensiveCleanseConfigPoison", "涓瘨").SetValue(false)));
                //Menu.ActivatorDefensiveCleanseConfig.MenuItems.Add(Menu.ActivatorDefensiveCleanseConfig.Menu.AddItem(new LeagueSharp.Common.MenuItem("SAwarenessActivatorDefensiveCleanseConfigActive", "Active").SetValue(false)));

                Menu.ActivatorDefensiveSelfShield.Menu =
                    Menu.ActivatorDefensive.Menu.AddSubMenu(new LeagueSharp.Common.Menu(
                        "鎶ょ浘", "SAwarenessActivatorDefensiveSelfShield"));
                Menu.ActivatorDefensiveSelfShield.MenuItems.Add(
                    Menu.ActivatorDefensiveSelfShield.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorDefensiveSelfShieldSeraphEmbrace", "鐐藉ぉ浣縷").SetValue(
                            false)));
                Menu.ActivatorDefensiveSelfShield.MenuItems.Add(
                    Menu.ActivatorDefensiveSelfShield.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorDefensiveSelfShieldOhmwrecker", "骞叉壈姘存櫠").SetValue(false)));
                Menu.ActivatorDefensiveSelfShield.MenuItems.Add(
                    Menu.ActivatorDefensiveSelfShield.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorDefensiveSelfShieldActive", "鎵撳紑").SetValue(false)));

                Menu.ActivatorDefensiveWoogletZhonya.Menu =
                    Menu.ActivatorDefensive.Menu.AddSubMenu(
                        new LeagueSharp.Common.Menu("涓簹|娌冩牸鍕掔壒",
                            "SAwarenessActivatorDefensiveWoogletZhonya"));
                Menu.ActivatorDefensiveWoogletZhonya.MenuItems.Add(
                    Menu.ActivatorDefensiveWoogletZhonya.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorDefensiveWoogletZhonyaWooglet", "娌冩牸鍕掔壒").SetValue(false)));
                Menu.ActivatorDefensiveWoogletZhonya.MenuItems.Add(
                    Menu.ActivatorDefensiveWoogletZhonya.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorDefensiveWoogletZhonyaZhonya", "涓簹").SetValue(false)));
                Menu.ActivatorDefensiveWoogletZhonya.MenuItems.Add(
                    Menu.ActivatorDefensiveWoogletZhonya.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorDefensiveWoogletZhonyaActive", "鎵撳紑").SetValue(false)));

                Menu.ActivatorDefensiveDebuffSlow.Menu =
                    Menu.ActivatorDefensive.Menu.AddSubMenu(new LeagueSharp.Common.Menu("鍑忛€焲",
                        "SAwarenessActivatorDefensiveDebuffSlow"));
                Menu.ActivatorDefensiveDebuffSlow.MenuItems.Add(
                    Menu.ActivatorDefensiveDebuffSlow.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorDefensiveDebuffSlowRanduins", "鍏伴】").SetValue(false)));
                Menu.ActivatorDefensiveDebuffSlow.MenuItems.Add(
                    Menu.ActivatorDefensiveDebuffSlow.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorDefensiveDebuffSlowConfigRanduins", "鏁屾柟浜烘暟")
                            .SetValue(new Slider(2, 5, 1))));
                Menu.ActivatorDefensiveDebuffSlow.MenuItems.Add(
                    Menu.ActivatorDefensiveDebuffSlow.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorDefensiveDebuffSlowFrostQueensClaim", "鍐伴湝濂崇帇")
                            .SetValue(false)));
                Menu.ActivatorDefensiveDebuffSlow.MenuItems.Add(
                    Menu.ActivatorDefensiveDebuffSlow.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorDefensiveDebuffSlowConfigFrostQueensClaim", "鏁屾柟浜烘暟")
                            .SetValue(new Slider(2, 5, 1))));
                Menu.ActivatorDefensiveDebuffSlow.MenuItems.Add(
                    Menu.ActivatorDefensiveDebuffSlow.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorDefensiveDebuffSlowActive", "鎵撳紑").SetValue(false)));

                Menu.ActivatorDefensiveCleanseSelf.Menu =
                    Menu.ActivatorDefensive.Menu.AddSubMenu(new LeagueSharp.Common.Menu("鍑€鍖栭€夐」",
                        "SAwarenessActivatorDefensiveCleanseSelf"));
                Menu.ActivatorDefensiveCleanseSelf.MenuItems.Add(
                    Menu.ActivatorDefensiveCleanseSelf.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorDefensiveCleanseSelfQSS", "姘撮摱楗板甫").SetValue(false)));
                Menu.ActivatorDefensiveCleanseSelf.MenuItems.Add(
                    Menu.ActivatorDefensiveCleanseSelf.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorDefensiveCleanseSelfMercurialScimitar", "姘撮摱寮垁")
                            .SetValue(false)));
                Menu.ActivatorDefensiveCleanseSelf.MenuItems.Add(
                    Menu.ActivatorDefensiveCleanseSelf.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorDefensiveCleanseSelfDervishBlade", "鑻﹁鍍т箣鍒億").SetValue(
                            false)));
                Menu.ActivatorDefensiveCleanseSelf.MenuItems.Add(
                    Menu.ActivatorDefensiveCleanseSelf.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorDefensiveCleanseSelfConfigMinSpells", "娉曟湳>X").SetValue(
                            new Slider(2, 10, 1))));
                Menu.ActivatorDefensiveCleanseSelf.MenuItems.Add(
                    Menu.ActivatorDefensiveCleanseSelf.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorDefensiveCleanseSelfActive", "鎵撳紑").SetValue(false)));

                Menu.ActivatorDefensiveShieldBoost.Menu =
                    Menu.ActivatorDefensive.Menu.AddSubMenu(new LeagueSharp.Common.Menu(
                        "鎶ょ浘", "SAwarenessActivatorDefensiveShieldBoost"));
                Menu.ActivatorDefensiveShieldBoost.MenuItems.Add(
                    Menu.ActivatorDefensiveShieldBoost.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorDefensiveShieldBoostLocketofIronSolari",
                            "閽㈤搧鐑堥槼涔嬪專").SetValue(false)));
                Menu.ActivatorDefensiveShieldBoost.MenuItems.Add(
                    Menu.ActivatorDefensiveShieldBoost.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorDefensiveShieldBoostTalismanofAscension",
                            "椋炲崌鎶ょ").SetValue(false)));
                Menu.ActivatorDefensiveShieldBoost.MenuItems.Add(
                    Menu.ActivatorDefensiveShieldBoost.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorDefensiveShieldBoostFaceOfTheMountain", "灞卞渤涔嬪")
                            .SetValue(false)));
                Menu.ActivatorDefensiveShieldBoost.MenuItems.Add(
                    Menu.ActivatorDefensiveShieldBoost.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorDefensiveShieldBoostGuardiansHorn", "瀹堟姢澶╀娇").SetValue(
                            false)));
                Menu.ActivatorDefensiveShieldBoost.MenuItems.Add(
                    Menu.ActivatorDefensiveShieldBoost.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorDefensiveShieldBoostConfigHealth", "%").SetValue(
                            new Slider(20, 100, 1))));
                Menu.ActivatorDefensiveShieldBoost.MenuItems.Add(
                    Menu.ActivatorDefensiveShieldBoost.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorDefensiveShieldBoostActive", "鎵撳紑").SetValue(false)));

                Menu.ActivatorDefensiveMikaelCleanse.Menu =
                    Menu.ActivatorDefensive.Menu.AddSubMenu(new LeagueSharp.Common.Menu("鍧╁煔",
                        "SAwarenessActivatorDefensiveMikaelCleanse"));
                Menu.ActivatorDefensiveMikaelCleanse.MenuItems.Add(
                    Menu.ActivatorDefensiveMikaelCleanse.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorDefensiveMikaelCleanseConfigAlly", "闃熷弸浣跨敤").SetValue(false)));
                Menu.ActivatorDefensiveMikaelCleanse.MenuItems.Add(
                    Menu.ActivatorDefensiveMikaelCleanse.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorDefensiveMikaelCleanseConfigAllyHealth", "闃熷弸鐢熷懡")
                            .SetValue(new Slider(20, 100, 0))));
                Menu.ActivatorDefensiveMikaelCleanse.MenuItems.Add(
                    Menu.ActivatorDefensiveMikaelCleanse.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorDefensiveMikaelCleanseConfigSelfHealth", "鑷繁鐢熷懡")
                            .SetValue(new Slider(20, 100, 0))));
                Menu.ActivatorDefensiveMikaelCleanse.MenuItems.Add(
                    Menu.ActivatorDefensiveMikaelCleanse.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorDefensiveMikaelCleanseConfigMinSpells", "娉曟湳>X").SetValue(
                            new Slider(2, 10, 1))));
                Menu.ActivatorDefensiveMikaelCleanse.MenuItems.Add(
                    Menu.ActivatorDefensiveMikaelCleanse.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorDefensiveShieldBoostActive", "鎵撳紑").SetValue(false)));
                Menu.ActivatorDefensive.MenuItems.Add(
                    Menu.ActivatorDefensive.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorDefensiveActive", "鎵撳紑").SetValue(false)));

                //Menu.ActivatorMisc.Menu =
                //    Menu.Activator.Menu.AddSubMenu(new LeagueSharp.Common.Menu("Misc Items",
                //        "SAwarenessActivatorMisc"));
                //Menu.ActivatorMisc.MenuItems.Add(
                //    Menu.ActivatorMisc.Menu.AddItem(
                //        new MenuItem("SAwarenessActivatorMisc", "Banner of Command").SetValue(false)));
                //Menu.ActivatorMisc.MenuItems.Add(
                //    Menu.ActivatorMisc.Menu.AddItem(
                //        new MenuItem("SAwarenessActivatorMisc", "Entropy").SetValue(false)));
                //Menu.ActivatorMisc.MenuItems.Add(
                //    Menu.ActivatorMisc.Menu.AddItem(
                //        new MenuItem("SAwarenessActivatorMisc", "Ravenous Hydra").SetValue(false)));
                //Menu.ActivatorOffensiveAd.MenuItems.Add(
                //    Menu.ActivatorOffensiveAd.Menu.AddItem(
                //        new MenuItem("SAwarenessActivatorOffensiveAdSwordOfTheDevine", "Sword Of The Devine").SetValue(
                //            false)));
                //Menu.ActivatorOffensiveAd.MenuItems.Add(
                //    Menu.ActivatorOffensiveAd.Menu.AddItem(
                //        new MenuItem("SAwarenessActivatorOffensiveAdTiamat", "Tiamat").SetValue(false)));
                //Menu.ActivatorOffensiveAd.MenuItems.Add(
                //    Menu.ActivatorOffensiveAd.Menu.AddItem(
                //        new MenuItem("SAwarenessActivatorOffensiveAdYoumuusGhostblade", "Youmuu's Ghostblade").SetValue(
                //            false)));
                ////Menu.ActivatorOffensiveAd.MenuItems.Add(Menu.ActivatorOffensiveAd.Menu.AddItem(new LeagueSharp.Common.MenuItem("SAwarenessActivatorOffensiveAdMuramana", "Muramana").SetValue(false)));
                //Menu.ActivatorOffensiveAd.MenuItems.Add(
                //    Menu.ActivatorOffensiveAd.Menu.AddItem(
                //        new MenuItem("SAwarenessActivatorOffensiveAdActive", "Active").SetValue(false)));

                //Menu.ActivatorOffensiveAp.Menu =
                //    Menu.ActivatorOffensive.Menu.AddSubMenu(new LeagueSharp.Common.Menu("AP",
                //        "SAwarenessActivatorOffensiveAp"));
                //Menu.ActivatorOffensiveAp.MenuItems.Add(
                //    Menu.ActivatorOffensiveAp.Menu.AddItem(
                //        new MenuItem("SAwarenessActivatorOffensiveApBilgewaterCutlass", "Bilgewater Cutlass").SetValue(
                //            false)));
                //Menu.ActivatorOffensiveAp.MenuItems.Add(
                //    Menu.ActivatorOffensiveAp.Menu.AddItem(
                //        new MenuItem("SAwarenessActivatorOffensiveApBlackfireTorch", "Blackfire Torch").SetValue(false)));
                //Menu.ActivatorOffensiveAp.MenuItems.Add(
                //    Menu.ActivatorOffensiveAp.Menu.AddItem(
                //        new MenuItem("SAwarenessActivatorOffensiveApDFG", "Deathfire Grasp").SetValue(false)));
                //Menu.ActivatorOffensiveAp.MenuItems.Add(
                //    Menu.ActivatorOffensiveAp.Menu.AddItem(
                //        new MenuItem("SAwarenessActivatorOffensiveApHextechGunblade", "Hextech Gunblade").SetValue(false)));
                //Menu.ActivatorOffensiveAp.MenuItems.Add(
                //    Menu.ActivatorOffensiveAp.Menu.AddItem(
                //        new MenuItem("SAwarenessActivatorOffensiveApTwinShadows", "Twin Shadows").SetValue(false)));
                ////Menu.ActivatorOffensiveAp.MenuItems.Add(Menu.ActivatorOffensiveAp.Menu.AddItem(new LeagueSharp.Common.MenuItem("SAwarenessActivatorOffensiveApOdynsVeil", "Odyn's Veil").SetValue(false)));
                //Menu.ActivatorOffensiveAp.MenuItems.Add(
                //    Menu.ActivatorOffensiveAp.Menu.AddItem(
                //        new MenuItem("SAwarenessActivatorOffensiveApActive", "Active").SetValue(false)));
                //Menu.ActivatorOffensive.MenuItems.Add(
                //    Menu.ActivatorOffensive.Menu.AddItem(
                //        new MenuItem("SAwarenessActivatorOffensiveKey", "Key").SetValue(new KeyBind(32,
                //            KeyBindType.Press))));
                //Menu.ActivatorOffensive.MenuItems.Add(
                //    Menu.ActivatorOffensive.Menu.AddItem(
                //        new MenuItem("SAwarenessActivatorOffensiveActive", "Active").SetValue(false)));

                Menu.AutoShield.Menu =
                    Menu.Activator.Menu.AddSubMenu(new LeagueSharp.Common.Menu("鑷姩鎶ょ浘",
                        "SAwarenessAutoShield"));
                Menu.AutoShield.MenuItems.Add(
                    Menu.AutoShield.Menu.AddItem(
                        new MenuItem("SAwarenessAutoShieldBlockAA", "鏍兼尅骞矨").SetValue(false)));
                Menu.AutoShield.MenuItems.Add(
                    Menu.AutoShield.Menu.AddItem(new MenuItem("SAwarenessAutoShieldBlockCC", "鏍兼尅缇ゆ帶").SetValue(false)));
                Menu.AutoShield.MenuItems.Add(
                    Menu.AutoShield.Menu.AddItem(
                        new MenuItem("SAwarenessAutoShieldBlockDamageAmount", "鏍兼尅浼ゅ").SetValue(
                            new StringList(new[] { "涓瓑", "楂榺", "鏋侀珮" }))));
                Menu.AutoShield.MenuItems.Add(
                    Menu.AutoShield.Menu.AddItem(
                        new MenuItem("SAwarenessAutoShieldBlockMinDamageAmount", "鏍兼尅min浼ゅ").SetValue(
                            new Slider(50, 2000, 1))));
                Menu.AutoShield.MenuItems.Add(
                    Menu.AutoShield.Menu.AddItem(new MenuItem("SAwarenessAutoShieldBlockableSpellsActive", "鏍兼尅鎸囧畾娉曟湳").SetValue(false)));
                Menu.AutoShieldBlockableSpells.Menu =
                    Menu.AutoShield.Menu.AddSubMenu(new LeagueSharp.Common.Menu("鏍兼尅",
                        "SAwarenessAutoShieldBlockableSpells"));
                foreach (var spell in AutoShield.GetBlockableSpells())
                {
                    Menu.AutoShieldBlockableSpells.MenuItems.Add(
                        Menu.AutoShieldBlockableSpells.Menu.AddItem(new MenuItem("SAwarenessAutoShieldBlockableSpells" + spell, spell).SetValue(false)));
                }
                Menu.AutoShield.MenuItems.Add(
                    Menu.AutoShield.Menu.AddItem(new MenuItem("SAwarenessAutoShieldAlly", "闃熷弸鎶ょ浘").SetValue(false)));
                Menu.AutoShield.MenuItems.Add(
                    Menu.AutoShield.Menu.AddItem(new MenuItem("SAwarenessAutoShieldActive", "鎵撳紑").SetValue(false)));
                Menu.AutoPot.Menu =
                    Menu.Activator.Menu.AddSubMenu(new LeagueSharp.Common.Menu("鑷姩鑽按", "SAwarenessAutoPot"));
                tempSettings = Menu.AutoPot.AddMenuItemSettings("绾㈣嵂",
                    "SAwarenessAutoPotHealthPot");
                tempSettings.MenuItems.Add(
                    tempSettings.Menu.AddItem(
                        new MenuItem("SAwarenessAutoPotHealthPotPercent", "%").SetValue(new Slider(20, 99,
                            0))));
                tempSettings.MenuItems.Add(
                    tempSettings.Menu.AddItem(new MenuItem("SAwarenessAutoPotHealthPotActive", "鎵撳紑").SetValue(false)));
                tempSettings = Menu.AutoPot.AddMenuItemSettings("钃濊嵂",
                    "SAwarenessAutoPotManaPot");
                tempSettings.MenuItems.Add(
                    tempSettings.Menu.AddItem(
                        new MenuItem("SAwarenessAutoPotManaPotPercent", "%").SetValue(new Slider(20, 99, 0))));
                tempSettings.MenuItems.Add(
                    tempSettings.Menu.AddItem(new MenuItem("SAwarenessAutoPotManaPotActive", "鎵撳紑").SetValue(false)));
                Menu.AutoPot.MenuItems.Add(
                    Menu.AutoPot.Menu.AddItem(new MenuItem("SAwarenessAutoPotActive", "鎵撳紑").SetValue(false)));
                Menu.ActivatorAutoHeal.Menu =
                    Menu.Activator.Menu.AddSubMenu(new LeagueSharp.Common.Menu("鑷姩娌荤枟",
                        "SAwarenessActivatorAutoHeal"));
                Menu.ActivatorAutoHeal.MenuItems.Add(
                    Menu.ActivatorAutoHeal.Menu.AddItem(new MenuItem("SAwarenessActivatorAutoHealPercent", "%").SetValue(new Slider(20, 99, 0))));
                Menu.ActivatorAutoHeal.MenuItems.Add(
                    Menu.ActivatorAutoHeal.Menu.AddItem(new MenuItem("SAwarenessActivatorAutoHealActive", "鎵撳紑").SetValue(false)));
                Menu.ActivatorAutoUlt.Menu =
                    Menu.Activator.Menu.AddSubMenu(new LeagueSharp.Common.Menu("鑷姩澶ф嫑",
                        "SAwarenessActivatorAutoUlt"));
                Menu.ActivatorAutoUlt.MenuItems.Add(
                    Menu.ActivatorAutoUlt.Menu.AddItem(new MenuItem("SAwarenessActivatorAutoUltAlly", "闃熷弸").SetValue(false)));
                Menu.ActivatorAutoUlt.MenuItems.Add(
                    Menu.ActivatorAutoUlt.Menu.AddItem(new MenuItem("SAwarenessActivatorAutoUltActive", "鎵撳紑").SetValue(false)));
                Menu.Activator.MenuItems.Add(
                    Menu.Activator.Menu.AddItem(new MenuItem("SAwarenessActivatorActive", "鎵撳紑").SetValue(false)));

                Menu.ActivatorAutoQss.Menu =
                   Menu.Activator.Menu.AddSubMenu(new LeagueSharp.Common.Menu("姘撮摱寮垁",
                       "SAwarenessActivatorAutoQssConfig"));
                Menu.ActivatorAutoQss.MenuItems.Add(
                    Menu.ActivatorAutoQss.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorAutoQssMinSpells", "娉曟湳>X").SetValue(
                            new Slider(2, 10, 1))));
                Menu.ActivatorAutoQss.MenuItems.Add(
                    Menu.ActivatorAutoQss.Menu.AddItem(new MenuItem("SAwarenessActivatorAutoQssActive", "鎵撳紑").SetValue(false)));

                Menu.ActivatorAutoQssConfig.Menu =
                    Menu.ActivatorAutoQss.Menu.AddSubMenu(new LeagueSharp.Common.Menu("璁剧疆",
                        "SAwarenessActivatorAutoQssConfig"));
                Menu.ActivatorAutoQssConfig.MenuItems.Add(
                    Menu.ActivatorAutoQssConfig.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorAutoQssConfigStun", "鏄忚糠").SetValue(false)));
                Menu.ActivatorAutoQssConfig.MenuItems.Add(
                    Menu.ActivatorAutoQssConfig.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorAutoQssConfigSilence", "娌夐粯").SetValue(false)));
                Menu.ActivatorAutoQssConfig.MenuItems.Add(
                    Menu.ActivatorAutoQssConfig.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorAutoQssConfigTaunt", "鍢茶").SetValue(false)));
                Menu.ActivatorAutoQssConfig.MenuItems.Add(
                    Menu.ActivatorAutoQssConfig.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorAutoQssConfigFear", "鎭愭儳").SetValue(false)));
                Menu.ActivatorAutoQssConfig.MenuItems.Add(
                    Menu.ActivatorAutoQssConfig.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorAutoQssConfigCharm", "榄呮儜").SetValue(false)));
                Menu.ActivatorAutoQssConfig.MenuItems.Add(
                    Menu.ActivatorAutoQssConfig.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorAutoQssConfigBlind", "鑷寸洸").SetValue(false)));
                Menu.ActivatorAutoQssConfig.MenuItems.Add(
                    Menu.ActivatorAutoQssConfig.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorAutoQssConfigDisarm", "鐐稿脊").SetValue(false)));
                Menu.ActivatorAutoQssConfig.MenuItems.Add(
                    Menu.ActivatorAutoQssConfig.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorAutoQssConfigSuppress", "鍘嬪埗").SetValue(false)));
                Menu.ActivatorAutoQssConfig.MenuItems.Add(
                    Menu.ActivatorAutoQssConfig.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorAutoQssConfigSlow", "鍑忛€焲").SetValue(false)));
                Menu.ActivatorAutoQssConfig.MenuItems.Add(
                    Menu.ActivatorAutoQssConfig.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorAutoQssConfigCombatDehancer", "铏氬急")
                            .SetValue(false)));
                Menu.ActivatorAutoQssConfig.MenuItems.Add(
                    Menu.ActivatorAutoQssConfig.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorAutoQssConfigSnare", "闄烽槺").SetValue(false)));
                Menu.ActivatorAutoQssConfig.MenuItems.Add(
                    Menu.ActivatorAutoQssConfig.Menu.AddItem(
                        new MenuItem("SAwarenessActivatorAutoQssConfigPoison", "涓瘨").SetValue(false)));  
                
                ////Not crashing
                Menu.Misc.Menu = menu.AddSubMenu(new LeagueSharp.Common.Menu("鏉傞」", "SAwarenessMisc"));
                Menu.Misc.MenuItems.Add(
                    Menu.Misc.Menu.AddItem(new MenuItem("SAwarenessMiscActive", "鎵撳紑").SetValue(false)));
                Menu.SkinChanger.Menu =
                    Menu.Misc.Menu.AddSubMenu(new LeagueSharp.Common.Menu("鎹㈣偆", "SAwarenessSkinChanger"));
                Menu.SkinChanger.MenuItems.Add(
                    Menu.SkinChanger.Menu.AddItem(
                        new MenuItem("SAwarenessSkinChangerSkinName", "鐨偆").SetValue(
                            new StringList(SkinChanger.GetSkinList(ObjectManager.Player.ChampionName))).DontSave()));
                Menu.SkinChanger.MenuItems.Add(
                    Menu.SkinChanger.Menu.AddItem(new MenuItem("SAwarenessSkinChangerActive", "鎵撳紑").SetValue(false)));
                Menu.SafeMovement.Menu =
                    Menu.Misc.Menu.AddSubMenu(new LeagueSharp.Common.Menu("瀹夊叏绉诲姩", "SAwarenessSafeMovement"));
                Menu.SafeMovement.MenuItems.Add(
                    Menu.SafeMovement.Menu.AddItem(
                        new MenuItem("SAwarenessSafeMovementBlockIntervall", "闅滅闂撮殧").SetValue(new Slider(20,
                            1000, 0))));
                Menu.SafeMovement.MenuItems.Add(
                    Menu.SafeMovement.Menu.AddItem(new MenuItem("SAwarenessSafeMovementActive", "鎵撳紑").SetValue(false)));
                Menu.AutoLevler.Menu =
                    Menu.Misc.Menu.AddSubMenu(new LeagueSharp.Common.Menu("鑷姩鍔犵偣", "SAwarenessAutoLevler"));
                tempSettings = Menu.AutoLevler.AddMenuItemSettings("浼樺厛",
                    "SAwarenessAutoLevlerPriority");
                tempSettings.MenuItems.Add(
                    tempSettings.Menu.AddItem(
                        new MenuItem("SAwarenessAutoLevlerPrioritySliderQ", "Q").SetValue(new Slider(0, 3, 0))));
                tempSettings.MenuItems.Add(
                    tempSettings.Menu.AddItem(
                        new MenuItem("SAwarenessAutoLevlerPrioritySliderW", "W").SetValue(new Slider(0, 3, 0))));
                tempSettings.MenuItems.Add(
                    tempSettings.Menu.AddItem(
                        new MenuItem("SAwarenessAutoLevlerPrioritySliderE", "E").SetValue(new Slider(0, 3, 0))));
                tempSettings.MenuItems.Add(
                    tempSettings.Menu.AddItem(
                        new MenuItem("SAwarenessAutoLevlerPrioritySliderR", "R").SetValue(new Slider(0, 3, 0))));
                tempSettings.MenuItems.Add(
                    tempSettings.Menu.AddItem(
                        new MenuItem("SAwarenessAutoLevlerPriorityFirstSpells", "妯″紡").SetValue(
                            new StringList(new[] { "Q W E", "Q E W", "W Q E", "W E Q", "E Q W", "E W Q" }))));
                tempSettings.MenuItems.Add(
                    tempSettings.Menu.AddItem(
                        new MenuItem("SAwarenessAutoLevlerPriorityFirstSpellsActive", "妯″紡鎵撳紑").SetValue(false)));
                tempSettings.MenuItems.Add(
                    tempSettings.Menu.AddItem(
                        new MenuItem("SAwarenessAutoLevlerPriorityActive", "鎵撳紑").SetValue(false).DontSave()));
                tempSettings = Menu.AutoLevler.AddMenuItemSettings("椤哄簭",
                    "SAwarenessAutoLevlerSequence");
                tempSettings.MenuItems.Add(
                    tempSettings.Menu.AddItem(
                        new MenuItem("SAwarenessAutoLevlerSequenceLoadChampion", "璇诲彇鑻遍泟").SetValue(false)
                            .DontSave()));
                tempSettings.MenuItems.Add(
                    tempSettings.Menu.AddItem(
                        new MenuItem("SAwarenessAutoLevlerSequenceActive", "鎵撳紑").SetValue(false).DontSave()));
                Menu.AutoLevler.MenuItems.Add(
                    Menu.AutoLevler.Menu.AddItem(
                        new MenuItem("SAwarenessAutoLevlerSMode", "妯″紡").SetValue(
                            new StringList(new[] { "椤哄簭", "浼樺厛", "鍙姞 R" }))));
                Menu.AutoLevler.MenuItems.Add(
                    Menu.AutoLevler.Menu.AddItem(new MenuItem("SAwarenessAutoLevlerActive", "鎵撳紑").SetValue(false)));
                Menu.MoveToMouse.Menu =
                    Menu.Misc.Menu.AddSubMenu(new LeagueSharp.Common.Menu("璺熼殢榧犳爣", "SAwarenessMoveToMouse"));
                Menu.MoveToMouse.MenuItems.Add(
                    Menu.MoveToMouse.Menu.AddItem(
                        new MenuItem("SAwarenessMoveToMouseKey", "鐑敭").SetValue(new KeyBind(90, KeyBindType.Press))));
                Menu.MoveToMouse.MenuItems.Add(
                    Menu.MoveToMouse.Menu.AddItem(new MenuItem("SAwarenessMoveToMouseActive", "鎵撳紑").SetValue(false)));
                Menu.SurrenderVote.Menu =
                    Menu.Misc.Menu.AddSubMenu(new LeagueSharp.Common.Menu("鎶曢檷", "SAwarenessSurrenderVote"));
                Menu.SurrenderVote.MenuItems.Add(
                    Menu.SurrenderVote.Menu.AddItem(
                        new MenuItem("SAwarenessSurrenderVoteChatChoice", "鑱婂ぉ閫夋嫨").SetValue(
                            new StringList(new[] { "涓嶇敤", "鏈湴", "姝ｅ父" }))));
                Menu.SurrenderVote.MenuItems.Add(
                    Menu.SurrenderVote.Menu.AddItem(
                        new MenuItem("SAwarenessSurrenderVoteActive", "鎵撳紑").SetValue(false)));
                Menu.AutoLatern.Menu =
                    Menu.Misc.Menu.AddSubMenu(new LeagueSharp.Common.Menu("涓€閿偣鐏", "SAwarenessAutoLatern"));
                Menu.AutoLatern.MenuItems.Add(
                    Menu.AutoLatern.Menu.AddItem(
                        new MenuItem("SAwarenessAutoLaternKey", "鐑敭").SetValue(new KeyBind(84, KeyBindType.Press))));
                Menu.AutoLatern.MenuItems.Add(
                    Menu.AutoLatern.Menu.AddItem(new MenuItem("SAwarenessAutoLaternActive", "鎵撳紑").SetValue(false)));
                Menu.AutoJump.Menu =
                    Menu.Misc.Menu.AddSubMenu(new LeagueSharp.Common.Menu("璺崇溂", "SAwarenessAutoJump"));
                Menu.AutoJump.MenuItems.Add(
                    Menu.AutoJump.Menu.AddItem(
                        new MenuItem("SAwarenessAutoJumpKey", "鐑敭").SetValue(new KeyBind(85, KeyBindType.Press))));
                Menu.AutoJump.MenuItems.Add(
                    Menu.AutoJump.Menu.AddItem(new MenuItem("SAwarenessAutoJumpActive", "鎵撳紑").SetValue(false)));
                Menu.DisconnectDetector.Menu =
                    Menu.Misc.Menu.AddSubMenu(new LeagueSharp.Common.Menu("鏂嚎妫€娴嬪櫒",
                        "SAwarenessDisconnectDetector"));
                Menu.DisconnectDetector.MenuItems.Add(
                    Menu.DisconnectDetector.Menu.AddItem(
                        new MenuItem("SAwarenessDisconnectDetectorChatChoice", "鑱婂ぉ閫夋嫨").SetValue(
                            new StringList(new[] { "涓嶇敤", "鏈湴", "姝ｅ父" }))));
                Menu.DisconnectDetector.MenuItems.Add(
                    Menu.DisconnectDetector.Menu.AddItem(
                        new MenuItem("SAwarenessDisconnectDetectorActive", "鎵撳紑").SetValue(false)));
                Menu.TurnAround.Menu =
                    Menu.Misc.Menu.AddSubMenu(new LeagueSharp.Common.Menu("璋冭浆鏂瑰悜", "SAwarenessTurnAround"));
                Menu.TurnAround.MenuItems.Add(
                    Menu.TurnAround.Menu.AddItem(new MenuItem("SAwarenessTurnAroundActive", "鎵撳紑").SetValue(false)));
                Menu.MinionBars.Menu =
                    Menu.Misc.Menu.AddSubMenu(new LeagueSharp.Common.Menu("琛ュ叺鎻愮ず", "SAwarenessMinionBars"));
                Menu.MinionBars.MenuItems.Add(
                    Menu.MinionBars.Menu.AddItem(new MenuItem("SAwarenessMinionBarsGlowActive", "楂樹寒鎻愮ず").SetValue(false)));
                Menu.MinionBars.MenuItems.Add(
                    Menu.MinionBars.Menu.AddItem(new MenuItem("SAwarenessMinionBarsActive", "鎵撳紑").SetValue(false)));
                //Menu.MinionLocation.Menu =
                //    Menu.Misc.Menu.AddSubMenu(new LeagueSharp.Common.Menu("Minion Location", "SAwarenessMinionLocation"));
                //Menu.MinionLocation.MenuItems.Add(
                //    Menu.MinionLocation.Menu.AddItem(new MenuItem("SAwarenessMinionLocationActive", "Active").SetValue(false)));
                Menu.FlashJuke.Menu =
                    Menu.Misc.Menu.AddSubMenu(new LeagueSharp.Common.Menu("闂幇娆洪獥", "SAwarenessFlashJuke"));
                Menu.FlashJuke.MenuItems.Add(
                    Menu.FlashJuke.Menu.AddItem(new MenuItem("SAwarenessFlashJukeKeyActive", "鐑敭").SetValue(new KeyBind(90, KeyBindType.Press))));
                Menu.FlashJuke.MenuItems.Add(
                    Menu.FlashJuke.Menu.AddItem(new MenuItem("SAwarenessFlashJukeRecall", "鍥炲煄").SetValue(false)));
                Menu.FlashJuke.MenuItems.Add(
                    Menu.FlashJuke.Menu.AddItem(new MenuItem("SAwarenessFlashJukeActive", "鎵撳紑").SetValue(false)));
                Menu.EasyRangedJungle.Menu =
                    Menu.Misc.Menu.AddSubMenu(new LeagueSharp.Common.Menu("鏃犱激鎵撻噹", "SAwarenessEasyRangedJungle"));
                Menu.EasyRangedJungle.MenuItems.Add(
                    Menu.EasyRangedJungle.Menu.AddItem(new MenuItem("SAwarenessEasyRangedJungleActive", "鎵撳紑").SetValue(false)));
                Menu.FowWardPlacement.Menu =
                    Menu.Misc.Menu.AddSubMenu(new LeagueSharp.Common.Menu("鐪间綅鎺ㄨ崘", "SAwarenessFowWardPlacement"));
                Menu.FowWardPlacement.MenuItems.Add(
                    Menu.FowWardPlacement.Menu.AddItem(new MenuItem("SAwarenessFowWardPlacementActive", "鎵撳紑").SetValue(false)));

                Menu.GlobalSettings.Menu =
                    menu.AddSubMenu(new LeagueSharp.Common.Menu("鍏ㄥ眬璁剧疆", "SAwarenessGlobalSettings"));
                Menu.GlobalSettings.MenuItems.Add(
                    Menu.GlobalSettings.Menu.AddItem(
                        new MenuItem("SAwarenessGlobalSettingsServerChatPingActive", "姝ｅ父鑱婂ぉ/姝ｅ父Ping").SetValue(false)));
                
                menu.AddItem(new MenuItem("浣滆€厊", "浣滆€厊 Screeder V0.85"));
				Menu.TranScreeder.Menu =
                    menu.AddSubMenu(new LeagueSharp.Common.Menu("鑻遍泟鑱旂洘绮変笣绔檌", "SAwarenessTranScreeder"));
                Menu.TranScreeder.MenuItems.Add(
                    Menu.TranScreeder.Menu.AddItem(
                        new MenuItem("SAwarenessTranScreederQ", "浜ゆ祦缇わ細21422249")));
                menu.AddToMainMenu();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            try
            {
                CreateMenu();
                PrintMessage("绁炲湥鎰忚瘑 杞藉叆鎴愬姛锛岃嫳闆勮仈鐩熺矇涓濈珯锛圠OL520.cc鍔╂偍娓告垙鎰夊揩锛侊級");
                //Game.OnGameUpdate += GameOnOnGameUpdate;
                new Thread(GameOnOnGameUpdate).Start();
                AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
                AppDomain.CurrentDomain.DomainUnload += delegate { threadActive = false; };
                AppDomain.CurrentDomain.ProcessExit += delegate { threadActive = false; };
            }
            catch (Exception e)
            {
                Console.WriteLine("SAwareness: " + e);
            }
        }
        public static void PrintMessage(string msg) // Credits to ChewyMoon, and his Brain.exe
        {
            Game.PrintChat("<font color=\"#FFFF00\"><b>鑻遍泟鑱旂洘绮変笣绔欙紒-</b></font> <font color=\"#FF0033\">" + msg + "</font>");
			Game.PrintChat("<font color=\"#00FFFF\"><b>浜ゆ祦缇わ細21422249</b></font>");
        }

        private static bool threadActive = true;

        private static void GameOnOnGameUpdate(/*EventArgs args*/)
        {
            try
            {
                while (threadActive)
                {
                    Thread.Sleep(1);
                    Type classType = typeof(Menu);
                    BindingFlags flags = BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly;
                    FieldInfo[] fields = classType.GetFields(flags);
                    foreach (FieldInfo p in fields)
                    {
                        var item = (Menu.MenuItemSettings)p.GetValue(null);
                        if (item.GetActive() == false && item.Item != null)
                        {
                            //item.Item = null;
                        }
                        else if (item.GetActive() && item.Item == null && !item.ForceDisable && item.Type != null)
                        {
                            try
                            {
                                item.Item = System.Activator.CreateInstance(item.Type);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                                throw;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("SAwareness: " + e);
                throw;
            }
            
            //CreateDebugInfos();
        }

        public static PropertyInfo[] GetPublicProperties(Type type)
        {
            if (type.IsInterface)
            {
                var propertyInfos = new List<PropertyInfo>();

                var considered = new List<Type>();
                var queue = new Queue<Type>();
                considered.Add(type);
                queue.Enqueue(type);
                while (queue.Count > 0)
                {
                    Type subType = queue.Dequeue();
                    foreach (Type subInterface in subType.GetInterfaces())
                    {
                        if (considered.Contains(subInterface)) continue;

                        considered.Add(subInterface);
                        queue.Enqueue(subInterface);
                    }

                    PropertyInfo[] typeProperties = subType.GetProperties(
                        BindingFlags.FlattenHierarchy
                        | BindingFlags.Public
                        | BindingFlags.Instance);

                    IEnumerable<PropertyInfo> newPropertyInfos = typeProperties
                        .Where(x => !propertyInfos.Contains(x));

                    propertyInfos.InsertRange(0, newPropertyInfos);
                }

                return propertyInfos.ToArray();
            }

            return type.GetProperties(BindingFlags.Static | BindingFlags.Public);
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return Load();
        }

        public static Assembly Load()
        {
            byte[] ba = null;
            string resource = "SAwareness.Resources.DLL.Evade.dll";
            Assembly curAsm = Assembly.GetExecutingAssembly();
            using (Stream stm = curAsm.GetManifestResourceStream(resource))
            {
                ba = new byte[(int) stm.Length];
                stm.Read(ba, 0, (int) stm.Length);
                return Assembly.Load(ba);
            }
        }

        private static void CreateDebugInfos()
        {
            if (lastDebugTime + 60 > Game.ClockTime)
                return;
            StreamWriter writer = null;
            try
            {
                writer = new StreamWriter("C:\\SAwarenessDebug.log");
                if(writer == null)
                    return;
                writer.WriteLine("Debug Infos of game: " + Game.Id);
                writer.WriteLine("MapId: " + Game.MapId);
                writer.WriteLine("Mode: " + Game.Mode);
                writer.WriteLine("Region: " + Game.Region);
                writer.WriteLine("Type: " + Game.Type);
                writer.WriteLine("Time: " + Game.ClockTime);

                foreach (var hero in ObjectManager.Get<Obj_AI_Hero>())
                {
                    if (hero.IsMe)
                    {
                        writer.WriteLine("Player: ");
                    }
                    else if (hero.IsAlly)
                    {
                        writer.WriteLine("Ally: ");
                    }
                    else if (hero.IsEnemy)
                    {
                        writer.WriteLine("Enemy: ");
                    }
                    writer.WriteLine("Character: " + hero.ChampionName);
                    writer.Write("Summoners: ");
                    foreach (var spell in hero.SummonerSpellbook.Spells)
                    {
                        writer.Write(spell.SData.Name + ", ");
                    }
                    writer.WriteLine("");
                    writer.Write("Items: ");
                    foreach (var item in hero.InventoryItems)
                    {
                        writer.Write(item.Name + ", ");
                    }
                    writer.WriteLine("");
                }
                Type classType = typeof(Menu);
                BindingFlags flags = BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly;
                FieldInfo[] fields = classType.GetFields(flags);
                writer.WriteLine("Activated Options: ");
                foreach (FieldInfo p in fields)
                {
                    var item = (Menu.MenuItemSettings)p.GetValue(null);
                    if (item.GetActive() == false && item.Item != null)
                    {
                        //item.Item = null;
                    }
                    else if (item.GetActive() && !item.ForceDisable)
                    {
                        try
                        {
                            writer.WriteLine("- " + item.Menu.Name);
                            foreach (var menuItem in item.MenuItems)
                            {
                                try{ writer.WriteLine("  - " + menuItem.Name + " | " + menuItem.GetValue<Boolean>()); }
                                catch (Exception e){ if (e is InvalidCastException || e is NullReferenceException) { } }
                                try { writer.WriteLine("  - " + menuItem.Name + " | " + menuItem.GetValue<Slider>().Value); }
                                catch (Exception e) { if (e is InvalidCastException || e is NullReferenceException) { } }
                                try { writer.WriteLine("  - " + menuItem.Name + " | " + menuItem.GetValue<KeyBind>().Active); }
                                catch (Exception e) { if (e is InvalidCastException || e is NullReferenceException) { } }
                                try { writer.WriteLine("  - " + menuItem.Name + " | " + menuItem.GetValue<StringList>().SelectedIndex); }
                                catch (Exception e) { if (e is InvalidCastException || e is NullReferenceException) { } }
                            }
                            //item.Item = System.Activator.CreateInstance(item.Type);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                    }
                }
                lastDebugTime = Game.ClockTime;
            }
            catch (Exception e)
            {
                Console.WriteLine("SAwareness: " + e);
            }
            finally
            {
                if (writer != null)
                {
                    writer.Flush();
                    writer.Close();
                }               
            }            
        }
    }
}