using System;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

namespace ChatTranslator
{
    class Program
    {
        public static Menu Config;
        public static String[] fromArray = new String[] { "auto", "en", "de", "es", "fr", "pl", "hu", "sq", "sv", "cs", "ro", "da", "bg", "pt", "sr", "fi", "lv", "sk", "sl", "ph", "tr", "el", "ms", "zh-CN", "zh-TW", "mk", "bg", "ru", "ko", "it", "be", "vi", "uk" };
        public static String[] toArray = new String[]           { "en", "de", "es", "fr", "pl", "hu", "sq", "sv", "cs", "ro", "da", "bg", "pt", "sr", "fi", "lv", "sk", "sl", "ph", "tr", "el", "ms", "zh-CN", "zh-TW", "mk", "bg", "ru", "ko", "it", "be", "vi", "uk" };
        public static String[] sendText =  new String[]         { "en", "de", "es", "fr", "pl", "hu", "sq", "sv", "cs", "ro", "da", "bg", "pt", "sr", "fi", "lv", "sk", "sl", "ph", "tr", "el", "ms", "zh-CN", "zh-TW", "mk", "bg", "ru", "ko", "it", "be", "vi", "uk" };
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;


        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            Config = new Menu("ChatTranslator", "ChatTranslator", true);
            Config.AddSubMenu(new Menu("IncomingText", "IncomingText"));
            Config.AddSubMenu(new Menu("OutgoingText", "OutgoingText"));
            Config.AddItem(new MenuItem("Phonetical", "Use special characters").SetValue(false));
            Config.SubMenu("IncomingText").AddItem(new MenuItem("From", "From: ").SetValue(new StringList(fromArray)));
            Config.SubMenu("IncomingText").AddItem(new MenuItem("To", "To: ").SetValue(new StringList(toArray)));
            Config.SubMenu("IncomingText").AddItem(new MenuItem("Enabled", "Enabled").SetValue(true));
            Config.SubMenu("OutgoingText").AddItem(new MenuItem("OutFrom", "From: ").SetValue(new StringList(sendText)));
            Config.SubMenu("OutgoingText").AddItem(new MenuItem("OutTo", "To: ").SetValue(new StringList(toArray)));
            Config.SubMenu("OutgoingText").AddItem(new MenuItem("EnabledOut", "Enabled").SetValue(false));
            Config.AddToMainMenu();
			Echo("aaaa");
            Game.PrintChat("<font color='#9933FF'>Soresu </font><font color='#FFFFFF'>- ChatTranslator</font>");
            Game.OnGameInput += Game_GameInput;
            Game.OnGameProcessPacket += Game_OnGameProcessPacket;
        }

        private static void Game_GameInput(GameInputEventArgs args)
        {
            if (Config.SubMenu("OutgoingText").Item("EnabledOut").GetValue<bool>() && sendText[Config.Item("OutFrom").GetValue<StringList>().SelectedIndex] != toArray[Config.Item("OutTo").GetValue<StringList>().SelectedIndex])
            {
			var message="";
			message+=args.Input;
                sendTranslated(message);
				args.Process = false;
                
            }

        }
        static void Game_OnGameProcessPacket(GamePacketEventArgs args)
        {
            if (args.PacketData[0] == 0x68 && Config.SubMenu("IncomingText").Item("Enabled").GetValue<bool>())
            {
			
                var p = new GamePacket(args);
                string dump = p.Dump();
                string removethejunk = dump.Substring(162);
                string removethenull = removethejunk.Replace("00", "");
                byte[] bytethedata = FromHex(removethenull);
                string textFromChat = Encoding.UTF8.GetString(bytethedata);
				Echo(textFromChat);
                //System.IO.File.AppendAllText(@"C:\Users\Public\TestFolder\WriteText.txt", textFromChat + "\n" + p.Dump() + "\n");
                
                
                //Game.PrintChat(p.Dump());
            
            }
        } 


        private static async void Echo(string text)
        {

            if (text.Length > 1)
            {
                string from = fromArray[Config.Item("From").GetValue<StringList>().SelectedIndex];
                string to = toArray[Config.Item("To").GetValue<StringList>().SelectedIndex];
                string x = "";
				byte[] bytes = Encoding.UTF8.GetBytes(text);
				text=Encoding.Default.GetString(bytes);
                x += await TranslateGoogle(text, from, to, true);
                if(!CheckJunk(x)){
                    Game.PrintChat(x);
                }
                
            }

        }
        private static async void sendTranslated(string text)
        {
		
            if (text.Length > 1)
            {
                bool all = false;
                if (text.Contains("/all") || text.Contains("/ All"))
                {
                    text=text.Replace("/all", "");
                    all = true;
                }
                string from = sendText[Config.Item("OutFrom").GetValue<StringList>().SelectedIndex];
                string to = toArray[Config.Item("OutTo").GetValue<StringList>().SelectedIndex];
                string x = "";
                x += await TranslateGoogle(text, from, to, false);
                if(all==true){
					

                    Game.Say("/all " + x);
                }
                else
                {

                    Game.Say(x);
                } 
            }

        }

        private static async Task<string> TranslateGoogle(string text, string fromCulture, string toCulture, bool langs)
        {
			
            string url = string.Format(@"http://translate.google.com/translate_a/t?client=j&text={0}&hl=en&sl={1}&tl={2}",
                               text.Replace(' ', '+'), fromCulture, toCulture);
            
			byte[] bytessss = Encoding.Default.GetBytes(url);
            url = Encoding.UTF8.GetString(bytessss);
			//System.IO.File.AppendAllText(@"C:\Users\Public\TestFolder\WriteText.txt", Encoding.UTF8.GetString(bytessss)  + "\n");
            string html;
			
            try
            {
                System.Uri uri = new System.Uri(url);
                html = await DownloadStringAsync(uri);
				//System.IO.File.AppendAllText(@"C:\Users\Public\TestFolder\WriteText.txt", html  + "\n");
            }
            catch (Exception ex)
            {

                return "";
            }
            JavaScriptSerializer ser = new JavaScriptSerializer();
            var source = ser.Deserialize(Regex.Match(html, "src\":(\".*?\"),\"", RegexOptions.IgnoreCase).Groups[1].Value, typeof(string)) as string;
            
			if (toCulture.Equals(source))
            {
                return "";
            }

            string result = "";
            if(langs==true){
            result += "(" + source + " => " + toCulture + ")";
            }
            string trans = "";
            if (Config.Item("Phonetical").GetValue<bool>()==true)
            {
			trans = ser.Deserialize(Regex.Match(html, "trans\":(\".*?\"),\"", RegexOptions.IgnoreCase).Groups[1].Value, typeof(string)) as string;
            }else{

                trans = ser.Deserialize(Regex.Match(html, "translit\":(\".*?\"),\"", RegexOptions.IgnoreCase).Groups[1].Value, typeof(string)) as string;
                //Game.PrintChat(trans);
                if (string.IsNullOrEmpty(trans))
                {
                    trans = ser.Deserialize(Regex.Match(html, "trans\":(\".*?\"),\"", RegexOptions.IgnoreCase).Groups[1].Value, typeof(string)) as string;
                }
            }
            byte[] bytes = Encoding.UTF8.GetBytes(trans);
			trans = Encoding.Default.GetString(bytes);

            if (trans == text || trans.ToLower().Equals(text.ToLower()) || StringCompare(text.Replace(" ", ""), trans.Replace(" ", ""))>85)
            {
                return "";
            }
            result += trans;

            if (string.IsNullOrEmpty(trans))
            {
                return "";
            }
			if (trans=="aaaa" || trans=="AAAA")
            {
                return "";
            }
            //System.IO.File.AppendAllText(@"C:\Users\Public\TestFolder\WriteText.txt", trans + "\n");
            if (CheckJunk(trans))
            {
                return "";
            }
            return result;
        }
        public static byte[] FromHex(string hex)
        {
            hex = hex.Replace(" ", "");
            byte[] raw = new byte[hex.Length / 2];
            for (int i = 0; i < raw.Length; i++)
            {
                raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return raw;
        }
        public static bool CheckJunk(string text)
        {
            bool result = false;


            int num = text.Length - text.Replace("@", "").Length;
            if (num > 0 || text.Contains("½") || text.Contains("ĂŻÂ"))
            {
                result = true;
            }


            return result;
        }
        public static Task<string> DownloadStringAsync(Uri url)
        {
			
            var tcs = new TaskCompletionSource<string>();
            var wc = new WebClient();
            wc.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0");
            wc.Headers.Add(HttpRequestHeader.AcceptCharset, "UTF-8");
            wc.Encoding = Encoding.UTF8;
            wc.DownloadStringCompleted += (s, e) =>
            {
                if (e.Error != null) tcs.TrySetException(e.Error);
                else if (e.Cancelled) tcs.TrySetCanceled();
                else tcs.TrySetResult(e.Result);
            };
            wc.DownloadStringAsync(url);
            return tcs.Task;
        }
        static double StringCompare(string a, string b)
        {
            if (a == b)
                return 100;
            if ((a.Length == 0) || (b.Length == 0)) 
            {
                return 0;
            }
            double maxLen = a.Length > b.Length ? a.Length : b.Length;
            int minLen = a.Length < b.Length ? a.Length : b.Length;
            int sameCharAtIndex = 0;
            for (int i = 0; i < minLen; i++) 
            {
                if (a[i] == b[i])
                {
                    sameCharAtIndex++;
                }
            }
            return sameCharAtIndex / maxLen * 100;
        }
    }
}
