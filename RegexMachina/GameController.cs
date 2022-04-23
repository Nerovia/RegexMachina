using SadConsole;
using SadConsole.UI;
using SadConsole.UI.Controls;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace RegexMachina
{
	class GameController
	{
		public static int Width { get => Game.Instance.ScreenCellsX; }

		public static int Height { get => Game.Instance.ScreenCellsY; }

		public static GameController Instance { get; } = new GameController();

		ControlsConsole Console { get => Game.Instance.Screen as ControlsConsole; }

		public const int ControlsWidth = 10;
		const int ControlsHeight = 10;
		const int ControlsMargin = 8;

		public static int TermWidth { get => Width - ControlsWidth - ControlsMargin - 2; }

		TextBox tapeBox;
		TextBox patternBox;
		TextBox substituteBox;
		ListBox termList;
		Button addTermButton;
		Button removeTermButton;
		Button stepButton;
		Button runButton;
		Button saveButton;
		Button loadButton;
		
		CancellationTokenSource runToken;

		public int CurrentTermIndex
		{
			get => termList.SelectedIndex;
			set
			{
				termList.SelectedIndex = value;
				termList.ScrollToSelectedItem();
			}
		}

		Term CurrentTerm { get => termList.SelectedItem as Term; }

		IEnumerable<object> Terms { get => termList.Items; }

		string Tape { get => tapeBox.Text; set => tapeBox.Text = value; }
		public string LastPath = @"C:\Users\Alain Rohrbach\Downloads\test.regexe";

		public void Init()
		{
			var console = new ControlsConsole(Width, Height);
			Game.Instance.Screen = console;
			Game.Instance.DestroyDefaultStartingConsole();
			
			Layout();
			Setup();
			Begin();
		}

		private void Layout()
		{
			addTermButton = new Button(ControlsWidth)
			{
				Position = new Point(ControlsMargin / 2, 2),
				Text = "Add"
			};

			removeTermButton = new Button(ControlsWidth)
			{
				Text = "Remove"
			};
			
			stepButton = new Button(ControlsWidth)
			{
				Text = "Step"
			};

			runButton = new Button(ControlsWidth)
			{
				Text = "Run"
			};

			saveButton = new Button(ControlsWidth)
			{
				Text = "Save"
			};

			loadButton = new Button(ControlsWidth)
			{
				Text = "Load"
			};

			termList = new ListBox(TermWidth - 2, ControlsHeight - 4)
			{
				Position = new Point(ControlsWidth + ControlsMargin, 2),
				
			};


			var colors = new Colors();
			colors.ControlBackgroundNormal.SetColor(new Color(19, 19, 19));
			colors.RebuildAppearances();
			termList.SetThemeColors(colors);

			patternBox = new TextBox(TermWidth / 2 - 2);

			substituteBox = new TextBox(TermWidth / 2 - 2);

			tapeBox = new TextBox(TermWidth - 2);

			//Console.DrawBox(termList.Rectangle().Expand(1, 1), ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThinExtended, new ColoredGlyph(Color.Gray)));
			Console.Print(termList.Position.Translate(0, -1), "Terms", Color.HotPink);

			removeTermButton.PlaceRelativeTo(addTermButton, Direction.Types.Down, 0);

			stepButton.PlaceRelativeTo(removeTermButton, Direction.Types.Down, 1);
			runButton.PlaceRelativeTo(stepButton, Direction.Types.Down, 0);

			saveButton.PlaceRelativeTo(runButton, Direction.Types.Down, 1);
			loadButton.PlaceRelativeTo(saveButton, Direction.Types.Down, 0);

			patternBox.PlaceRelativeTo(termList, Direction.Types.Down, 2);
			Console.Print(patternBox.Position.Translate(0, -1), "Pattern", Color.HotPink);

			substituteBox.PlaceRelativeTo(patternBox, Direction.Types.Right, 2);
			Console.Print(substituteBox.Position.Translate(0, -1), "Substitute", Color.HotPink);

			tapeBox.PlaceRelativeTo(patternBox, Direction.Types.Down, 2);
			Console.Print(tapeBox.Position.Translate(0, -1), "Tape", Color.HotPink);

			Console.Controls.Add(addTermButton);
			Console.Controls.Add(removeTermButton);
			Console.Controls.Add(termList);
			Console.Controls.Add(patternBox);
			Console.Controls.Add(substituteBox);
			Console.Controls.Add(stepButton);
			Console.Controls.Add(runButton);
			Console.Controls.Add(tapeBox);
			Console.Controls.Add(saveButton);
			Console.Controls.Add(loadButton);
		}

		private void Setup()
		{
			patternBox.TextChanged += Pattern_TextChanged;
			substituteBox.TextChanged += Substitute_TextChanged;
			termList.SelectedItemChanged += Terms_SelectedItemChanged;
			termList.SelectedItemExecuted += (s, e) => patternBox.IsFocused = true;
			addTermButton.Click += AddTermButton_Click;
			removeTermButton.Click += RemoveTermButton_Click;
			stepButton.Click += StepButton_Click;
			runButton.Click += RunButton_Click;
			saveButton.Click += SaveButton_Click;
			loadButton.Click += LoadButton_Click;
		}

		private void LoadButton_Click(object sender, EventArgs e)
		{
			var w = new StoragePicker(30, 4)
			{
				Title = "Load Regexe",
				Path = LastPath
			};
			w.Picked += (s, e) =>
			{
				TryLoadFrom(e);
			};
			w.Show(true);
		}

		private void SaveButton_Click(object sender, EventArgs e)
		{
			var w = new StoragePicker(30, 4)
			{
				Title = "Save Regexe",
				Path = LastPath
			};
			w.Picked += (s, e) =>
			{
				TrySaveTo(e);
			};
			w.Show(true);
		}


		private bool TrySaveTo(string path)
		{
			if (!Regex.IsMatch(path, @"\.regexe$"))
				path = path += ".regexe";
			try
			{
				System.IO.File.WriteAllText(path, Serialize());
			}
			catch
			{
				return false;
			}

			return true;
		}

		private string Serialize()
		{
			var sb = new StringBuilder();
			sb.AppendLine(Tape);
			foreach (Term term in Terms)
				sb.AppendLine($"{term.Pattern}\t{term.Substitute}");
			return sb.ToString();
		}

		private bool TryLoadFrom(string path)
		{
			if (!Regex.IsMatch(path, @"\.regexe$"))
				return false;
			var content = System.IO.File.ReadAllText(path).Split('\n', StringSplitOptions.TrimEntries);

			Tape = "";
			termList.Items.Clear();

			if (content.Length >= 1)
				Tape = content[0];
			for (int i = 1; i < content.Length; i++)
			{
				var term = new Term();
				var c = content[i].Split('\t');
				if (c.Length >= 1)
					term.Pattern = c[0];
				if (c.Length >= 2)
					term.Substitute = c[1];
				if (!term.IsEmpty)
					termList.Items.Add(term);
			}
			
			return true;
		}

		private void Begin()
		{

		}

		private void StepOver()
		{
			int i = CurrentTermIndex;
			int max = Terms.Count();
			while (max-- > 0)
			{
				if (++i >= Terms.Count())
					i = 0;
				CurrentTermIndex = i;

				if (CurrentTerm.CanExecute(Tape))
				{
					Tape = CurrentTerm.Execute(Tape);
					break;
				}
			}
		}

		private void Step()
		{
			if (Terms.Count() <= 0)
				return;
			if (CurrentTerm == null)
				CurrentTermIndex = 0;

			Tape = CurrentTerm.Execute(Tape);

			int i = CurrentTermIndex;
			if (++i >= Terms.Count())
				i = 0;
			CurrentTermIndex = i;

		}


		public async Task Run()
		{
			runToken = new CancellationTokenSource();
			while (!runToken.IsCancellationRequested)
			{
				StepOver();
				try
				{
					await Task.Delay(TimeSpan.FromMilliseconds(500), runToken.Token);
				}
				catch { }
			}
			runToken.Dispose();
			runToken = null;
		}


		private async void RunButton_Click(object sender, EventArgs e)
		{
			if (runToken != null)
			{
				runToken.Cancel();
				runButton.Text = "Run";
			}
			else
			{
				runButton.Text = "Stop";
				await Run();
			}
		}

		private void StepButton_Click(object sender, EventArgs e)
		{
			Step();
		}

		private void RemoveTermButton_Click(object sender, EventArgs e)
		{
			if (CurrentTerm != null)
			{
				var index = termList.SelectedIndex;
				termList.Items.Remove(CurrentTerm);
				if (termList.Items.Count() > 0)
				{
					index = Math.Min(index, termList.Items.Count() - 1);
					termList.SelectedIndex = index;
				}
			}
		}

		private void AddTermButton_Click(object sender, EventArgs e)
		{
			var newTerm = new Term();
			termList.Items.Add(newTerm);
			termList.SelectedItem = newTerm;
			termList.ScrollToSelectedItem();
		}

		private void Terms_SelectedItemChanged(object sender, ListBox.SelectedItemEventArgs e)
		{
			removeTermButton.IsEnabled = CurrentTerm != null;
			patternBox.Text = CurrentTerm?.Pattern;
			substituteBox.Text = CurrentTerm?.Substitute;
		}

		private void Substitute_TextChanged(object sender, EventArgs e)
		{
			if (CurrentTerm != null)
			{
				CurrentTerm.Substitute = substituteBox.Text;
				termList.IsDirty = true;
			}
		}

		private void Pattern_TextChanged(object sender, EventArgs e)
		{
			if (CurrentTerm != null)
			{
				CurrentTerm.Pattern = patternBox.Text;
				termList.IsDirty = true;
			}
		}

	}
}
