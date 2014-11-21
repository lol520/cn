using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using LX_Orbwalker;

namespace LxOrbwalker_Standalone
{
    class Program
    {
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }


        public static void Game_OnGameLoad(EventArgs args)
        {
            Game.PrintChat("<font color='#FF0000'>濂剁摱姹夊寲-LX璧扮爫</font> 杞藉叆鎴愬姛锛岃嫳闆勮仈鐩熺矇涓濈珯锛圠OL520.cc鍔╂偍娓告垙鎰夊揩锛侊級 - <font color='#5882FA'>濂剁摱姹夊寲</font>");
            var menu = new Menu("濂剁摱姹夊寲-LX璧扮爫", "my_mainmenu", true);
            var orbwalkerMenu = new Menu("濂剁摱姹夊寲-LX璧扮爫", "my_Orbwalker");
            LXOrbwalker.AddToMenu(orbwalkerMenu);
            menu.AddSubMenu(orbwalkerMenu);
            menu.AddToMainMenu();
        }

    }
}
