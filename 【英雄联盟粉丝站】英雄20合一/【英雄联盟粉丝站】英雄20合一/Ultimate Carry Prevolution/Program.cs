using System;
using LeagueSharp;

namespace Ultimate_Carry_Prevolution
{
	class Program
	{
		// ReSharper disable once UnusedParameter.Local
		static void Main(string[] args)
		{
			Events.Game.OnGameStart += OnGameStart;
		}

		private static void OnGameStart(EventArgs args)
		{
			//Game.PrintChat("Its Released when its Released, not when you find our repo.");
			LoadUC();
		}

		private static void LoadUC()
		{
			// ReSharper disable once ObjectCreationAsStatement
			new Loader();
		}
	}
}
