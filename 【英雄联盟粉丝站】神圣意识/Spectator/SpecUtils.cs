using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LeagueSharp;

namespace SAwareness.Spectator
{
    class SpecUtils
    {
        private const String BaseUrl = "http://www.lolnexus.com/ajax/get-game-info/";
        private const String UrlPartial = ".json?name=";
        private const String SearchString = "lrf://spectator ";

        public static String Key;
        public static String GameId;
        public static String PlatformId;
        public static String RegionTag;
        public static String SpecUrl;

        public static void GetInfo()
        {
            GetRegionInfo();
            GetSpecInfo();
        }

        static void GetRegionInfo()
        {
            Process proc = Process.GetProcesses().First(p => p.ProcessName.Contains("League of Legends"));
            String propFile = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(proc.Modules[0].FileName))))));
            propFile += @"\projects\lol_air_client\releases\";
            DirectoryInfo di = new DirectoryInfo(propFile).GetDirectories().OrderByDescending(d => d.LastWriteTimeUtc).First();
            propFile = di.FullName + @"\deploy\lol.properties";
            propFile = File.ReadAllText(propFile);
            SpecUrl = new Regex("featuredGamesURL=(.+)featured").Match(propFile).Groups[1].Value;
            RegionTag = new Regex("regionTag=(.+)\r").Match(propFile).Groups[1].Value;
            SpectatorDownloader.specHtml = SpecUrl;
        }
        static void GetSpecInfo()
        {
            String GameInfo = new WebClient().DownloadString(BaseUrl + RegionTag + UrlPartial + ObjectManager.Player.Name);
            GameInfo = GameInfo.Substring(GameInfo.IndexOf(SearchString) + SearchString.Length);
            GameInfo = GameInfo.Substring(GameInfo.IndexOf(" ") + 1);
            Key = GameInfo.Substring(0, GameInfo.IndexOf(" "));
            GameInfo = GameInfo.Substring(GameInfo.IndexOf(" ") + 1);
            GameId = GameInfo.Substring(0, GameInfo.IndexOf(" "));
            GameInfo = GameInfo.Substring(GameInfo.IndexOf(" ") + 1);
            PlatformId = GameInfo.Substring(0, GameInfo.IndexOf(" "));
        }
    }
}
