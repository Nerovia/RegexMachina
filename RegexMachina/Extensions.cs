using SadConsole;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegexMachina
{
	static class Extensions
	{
		public static Rectangle Rectangle(this SadConsole.UI.Controls.ControlBase control)
		{
			return new Rectangle(control.Position.X, control.Position.Y, control.Width, control.Height);
		}

		public static void Print(this ICellSurface cell, Point point, string text)
		{
			cell.Print(point.X, point.Y, text);
		}

		public static void Print(this ICellSurface cell, Point point, string text, Color foreground)
		{
			cell.Print(point.X, point.Y, text, foreground);
		}
	}
}
