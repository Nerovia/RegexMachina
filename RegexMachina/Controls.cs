using SadConsole;
using SadConsole.UI;
using SadConsole.UI.Controls;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegexMachina
{
	class StoragePicker : Window
	{
		public string Path { get => pathBox.Text; set => pathBox.Text = value; }
		TextBox pathBox;
		Label label;

		public StoragePicker(int width, int height) : base(width + 2, height + 2)
		{
			var x = (Game.Instance.ScreenCellsX - width - 2) / 2;
			var y = (Game.Instance.ScreenCellsY - height - 2) / 2;
			Position = new Point(x, y);
			CloseOnEscKey = true;
			DefaultForeground = Color.HotPink;

			label = new Label(width - 2)
			{
				Position = new Point(2, 1),
				TextColor = Color.HotPink,
				DisplayText = "Path"
			};

			pathBox = new TextBox(width - 2)
			{
				Position = new Point(2, 2)
			};

			var cancelButton = new Button(8)
			{
				Position = new Point(2, height),
				Text = "Cancel"
			};

			var acceptButton = new Button(8)
			{
				Position = new Point(width - 8, height),
				Text = "Accept"
			};

			Controls.Add(label);
			Controls.Add(pathBox);
			Controls.Add(cancelButton);
			Controls.Add(acceptButton);

			cancelButton.Click += CancelButton_Click;
			acceptButton.Click += AcceptButton_Click;
		}

		private void AcceptButton_Click(object sender, EventArgs e)
		{
			OnPicked(Path);
		}

		private void CancelButton_Click(object sender, EventArgs e)
		{
			Hide();
			Dispose();
		}

		public event EventHandler<string> Picked;
		protected virtual void OnPicked(string path)
		{
			Picked?.Invoke(this, path);
			Hide();
			Dispose();
		}

	}
}
