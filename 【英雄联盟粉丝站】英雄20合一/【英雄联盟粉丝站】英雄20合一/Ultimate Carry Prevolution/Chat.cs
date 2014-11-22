using System;
using System.Drawing;
using LeagueSharp;
using System.Windows.Forms;

namespace Ultimate_Carry_Prevolution
{
	internal class Chat
	{
		public static void WellCome()
		{
			if (FlamerProtection())
			{
				Game.PrintChat("We not like your Behavier in LoL, Change it or your name get Permanent Banned, UCP-Disabled");
				return;
			}
			Game.PrintChat(
				"<font color ='{0}'>xSLx</font> <font color ='{1}'>鑻遍泟20鍚堜竴</font> <font color ='{1}'>lol520.cc</font>",
				HtmlColor.Cyan, HtmlColor.Cyan);
			Game.PrintChat("<font color ='{0}'>鑻遍泟鑱旂洘绮変笣绔檌</font>", HtmlColor.Gold);
			Game.PrintChat(
				"<font color ='{0}'>鐗堟湰: </font> <font color ='{1}'>1.0." + Loader.VersionNumber +
				"</font> <font color ='{0}'>Loading</font>",
			
				HtmlColor.Cyan, HtmlColor.Gold);
			if (IsChampionSupported())
			{
				Game.PrintChat(
					"<font color ='{0}'>Loaded Plugin for </font> <font color ='{1}'>" + ObjectManager.Player.ChampionName + "</font>",
					HtmlColor.Cyan, HtmlColor.Gold);
			Game.PrintChat("<font color=\"#FFFF00\">====== 鎴愬姛杞藉叆:|xSLx鑻遍泟20鍚堜竴|=======</font>");
			Game.PrintChat("<font color=\"#FFFF00\">====== 鐗堟湰: v 1.0.1.0====== </font>");
			Game.PrintChat("<font color=\"#FFFF00\">|鍓戦瓟|鐙愮嫺|闃垮崱涓絴鐗涘ご|鏈ㄦ湪|鍑ゅ嚢|瀵掑啺|鐨囧笣|鏈哄櫒|濂宠|椋炴満|EZ|鍓戝К|绾冲皵|鍒€濡箌閲戝厠涓潀鏃ュコ|鍗㈣タ瀹墊鐙瓙鐙梶鐟炲吂|杈涘痉鎷墊钖囨仼|</font>");
			Game.PrintChat("<font color=\"#FFFF00\">杞藉叆鎴愬姛锛岃嫳闆勮仈鐩熺矇涓濈珯锛圠OL520.cc鍔╂偍娓告垙鎰夊揩锛侊級</font>");
				//// ContestMessage
				//Game.PrintChat(
				//	"<font color ='{0}'>Support us by Voting, a Vote is like a Donation for Free <font color ='{1}'> >>> </font> press F1 <font color ='{1}'> <<< </font></font>",
				//	HtmlColor.Gold ,HtmlColor.Red );
				//// end
			}
			else
				Game.PrintChat(
					"<font color ='{0}'>" + ObjectManager.Player.ChampionName +
					"</font> <font color ='{1}'>褰撳墠鑻遍泟鏆傛椂涓嶅彲鐢紒</font>", HtmlColor.Gold,
					HtmlColor.Cyan);
		}

		private static bool FlamerProtection()
		{
			// A list of IngameNames ( i dont know if they use L# or not ) but i think so and if they do, they not have to use UCP
			// till they changed the behavier.
			if (ObjectManager.Player.Name == "Zourimi") // 13.11 ( 7 day banned )
				return true;
			return false;
		}

		private static bool IsChampionSupported()
		{
			try 
			{//sync, sync where?
				// ReSharper disable once AssignNullToNotNullAttribute
				var handle = Activator.CreateInstance(null , "Ultimate_Carry_Prevolution.Plugin." + ObjectManager.Player.ChampionName);
				var champion = (Champion) handle.Unwrap();
				return true;
			}
				// ReSharper disable once EmptyGeneralCatchClause
			//catch (Exception ex)
			catch(Exception)
			{
				//Game.PrintChat(ex.Message);
				return false;
			}
		}

		public static class HtmlColor
		{
			public const string AliceBlue = "#F0F8FF";
			public const string AntiqueWhite = "#FAEBD7";
			public const string Aqua = "#00FFFF";
			public const string Aquamarine = "#7FFFD4";
			public const string Azure = "#F0FFFF";
			public const string Beige = "#F5F5DC";
			public const string Bisque = "#FFE4C4";
			public const string Black = "#000000";
			public const string BlanchedAlmond = "#FFEBCD";
			public const string Blue = "#0000FF";
			public const string BlueViolet = "#8A2BE2";
			public const string Brown = "#A52A2A";
			public const string BurlyWood = "#DEB887";
			public const string CadetBlue = "#5F9EA0";
			public const string Chartreuse = "#7FFF00";
			public const string Chocolate = "#D2691E";
			public const string Coral = "#FF7F50";
			public const string CornflowerBlue = "#6495ED";
			public const string Cornsilk = "#FFF8DC";
			public const string Crimson = "#DC143C";
			public const string Cyan = "#00FFFF";
			public const string DarkBlue = "#00008B";
			public const string DarkCyan = "#008B8B";
			public const string DarkGoldenRod = "#B8860B";
			public const string DarkGray = "#A9A9A9";
			public const string DarkGreen = "#006400";
			public const string DarkKhaki = "#BDB76B";
			public const string DarkMagenta = "#8B008B";
			public const string DarkOliveGreen = "#556B2F";
			public const string DarkOrange = "#FF8C00";
			public const string DarkOrchid = "#9932CC";
			public const string DarkRed = "#8B0000";
			public const string DarkSalmon = "#E9967A";
			public const string DarkSeaGreen = "#8FBC8F";
			public const string DarkSlateBlue = "#483D8B";
			public const string DarkSlateGray = "#2F4F4F";
			public const string DarkTurquoise = "#00CED1";
			public const string DarkViolet = "#9400D3";
			public const string DeepPink = "#FF1493";
			public const string DeepSkyBlue = "#00BFFF";
			public const string DimGray = "#696969";
			public const string DodgerBlue = "#1E90FF";
			public const string FireBrick = "#B22222";
			public const string FloralWhite = "#FFFAF0";
			public const string ForestGreen = "#228B22";
			public const string Fuchsia = "#FF00FF";
			public const string Gainsboro = "#DCDCDC";
			public const string GhostWhite = "#F8F8FF";
			public const string Gold = "#FFD700";
			public const string GoldenRod = "#DAA520";
			public const string Gray = "#808080";
			public const string Green = "#008000";
			public const string GreenYellow = "#ADFF2F";
			public const string HoneyDew = "#F0FFF0";
			public const string HotPink = "#FF69B4";
			public const string IndianRed = "#CD5C5C";
			public const string Indigo = "#4B0082";
			public const string Ivory = "#FFFFF0";
			public const string Khaki = "#F0E68C";
			public const string Lavender = "#E6E6FA";
			public const string LavenderBlush = "#FFF0F5";
			public const string LawnGreen = "#7CFC00";
			public const string LemonChiffon = "#FFFACD";
			public const string LightBlue = "#ADD8E6";
			public const string LightCoral = "#F08080";
			public const string LightCyan = "#E0FFFF";
			public const string LightGoldenRodYellow = "#FAFAD2";
			public const string LightGray = "#D3D3D3";
			public const string LightGreen = "#90EE90";
			public const string LightPink = "#FFB6C1";
			public const string LightSalmon = "#FFA07A";
			public const string LightSeaGreen = "#20B2AA";
			public const string LightSkyBlue = "#87CEFA";
			public const string LightSlateGray = "#778899";
			public const string LightSteelBlue = "#B0C4DE";
			public const string LightYellow = "#FFFFE0";
			public const string Lime = "#00FF00";
			public const string LimeGreen = "#32CD32";
			public const string Linen = "#FAF0E6";
			public const string Magenta = "#FF00FF";
			public const string Maroon = "#800000";
			public const string MediumAquaMarine = "#66CDAA";
			public const string MediumBlue = "#0000CD";
			public const string MediumOrchid = "#BA55D3";
			public const string MediumPurple = "#9370DB";
			public const string MediumSeaGreen = "#3CB371";
			public const string MediumSlateBlue = "#7B68EE";
			public const string MediumSpringGreen = "#00FA9A";
			public const string MediumTurquoise = "#48D1CC";
			public const string MediumVioletRed = "#C71585";
			public const string MidnightBlue = "#191970";
			public const string MintCream = "#F5FFFA";
			public const string MistyRose = "#FFE4E1";
			public const string Moccasin = "#FFE4B5";
			public const string NavajoWhite = "#FFDEAD";
			public const string Navy = "#000080";
			public const string OldLace = "#FDF5E6";
			public const string Olive = "#808000";
			public const string OliveDrab = "#6B8E23";
			public const string Orange = "#FFA500";
			public const string OrangeRed = "#FF4500";
			public const string Orchid = "#DA70D6";
			public const string PaleGoldenRod = "#EEE8AA";
			public const string PaleGreen = "#98FB98";
			public const string PaleTurquoise = "#AFEEEE";
			public const string PaleVioletRed = "#DB7093";
			public const string PapayaWhip = "#FFEFD5";
			public const string PeachPuff = "#FFDAB9";
			public const string Peru = "#CD853F";
			public const string Pink = "#FFC0CB";
			public const string Plum = "#DDA0DD";
			public const string PowderBlue = "#B0E0E6";
			public const string Purple = "#800080";
			public const string Red = "#FF0000";
			public const string RosyBrown = "#BC8F8F";
			public const string RoyalBlue = "#4169E1";
			public const string SaddleBrown = "#8B4513";
			public const string Salmon = "#FA8072";
			public const string SandyBrown = "#F4A460";
			public const string SeaGreen = "#2E8B57";
			public const string SeaShell = "#FFF5EE";
			public const string Sienna = "#A0522D";
			public const string Silver = "#C0C0C0";
			public const string SkyBlue = "#87CEEB";
			public const string SlateBlue = "#6A5ACD";
			public const string SlateGray = "#708090";
			public const string Snow = "#FFFAFA";
			public const string SpringGreen = "#00FF7F";
			public const string SteelBlue = "#4682B4";
			public const string Tan = "#D2B48C";
			public const string Teal = "#008080";
			public const string Thistle = "#D8BFD8";
			public const string Tomato = "#FF6347";
			public const string Turquoise = "#40E0D0";
			public const string Violet = "#EE82EE";
			public const string Wheat = "#F5DEB3";
			public const string White = "#FFFFFF";
			public const string WhiteSmoke = "#F5F5F5";
			public const string Yellow = "#FFFF00";
			public const string YellowGreen = "#9ACD32";
		}
	}
}
