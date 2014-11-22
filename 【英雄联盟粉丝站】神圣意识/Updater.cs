using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Reflection;
using LeagueSharp;

namespace SAwareness
{
    public static class SUpdater
    {
        private const int Localmajorversion = 0;
        private const int Localversion = 8;

        public static void UpdateCheck()
        {
            var bgw = new BackgroundWorker();
            bgw.DoWork += bgw_DoWork;
            bgw.RunWorkerAsync();
        }

        private static void bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            var myUpdater =
                new Updater("https://raw.githubusercontent.com/Screeder/SAwareness/master/Properties/Version",
                    "https://github.com/Screeder/SAwareness/releases/download/", "SAwareness.exe", Localmajorversion,
                    Localversion);
            if (myUpdater.NeedUpdate)
            {
                Game.PrintChat("SAwareness Updating ...");
                if (myUpdater.Update())
                {
                    Game.PrintChat("SAwareness updated, reload please");
                }
            }
        }
    }

    internal class Updater
    {
        private readonly string _updatelink;

        private readonly WebClient _wc = new WebClient {Proxy = null};
        public bool NeedUpdate = false;

        public Updater(string versionlink, string updatelink, String assemblyName, int localmajorversion,
            int localversion)
        {
            _updatelink = updatelink;

            String str = _wc.DownloadString(versionlink);
            str = str.Trim();
            _updatelink = updatelink + "v" + str + "/" + assemblyName;
            if (Convert.ToInt32(str.Remove(str.IndexOf("."))) > localmajorversion)
                NeedUpdate = true;
            if (Convert.ToInt32(str.Remove(0, str.IndexOf(".") + 1)) > localversion)
                NeedUpdate = true;
        }

        public bool Update()
        {
            try
            {
                if (
                    File.Exists(
                        Path.Combine(Assembly.GetExecutingAssembly().Location) + ".bak"))
                {
                    File.Delete(
                        Path.Combine(Assembly.GetExecutingAssembly().Location) + ".bak");
                }
                File.Move(Assembly.GetExecutingAssembly().Location,
                    Path.Combine(Assembly.GetExecutingAssembly().Location) + ".bak");
                _wc.DownloadFile(_updatelink,
                    Path.Combine(Assembly.GetExecutingAssembly().Location));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}