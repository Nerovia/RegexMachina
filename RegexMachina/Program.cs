using SadConsole;
using SadRogue.Primitives;
using System;
using Console = SadConsole.Console;

namespace RegexMachina
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var SCREEN_WIDTH = 60;
			var SCREEN_HEIGHT = 25;

			SadConsole.Settings.WindowTitle = "RegexMachina";
			SadConsole.Settings.UseDefaultExtendedFont = true;
			SadConsole.Game.Create(SCREEN_WIDTH, SCREEN_HEIGHT);
			SadConsole.Game.Instance.DefaultFontSize = IFont.Sizes.Two;
			SadConsole.Game.Instance.OnStart = GameController.Instance.Init;
			SadConsole.Game.Instance.Run();
			SadConsole.Game.Instance.Dispose();
		}
	}
}