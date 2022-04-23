using SadConsole;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RegexMachina
{
	class Term
	{
		public string Pattern { get; set; }

		public string Substitute { get; set; }

	
		public bool IsEmpty { get => string.IsNullOrEmpty(Pattern) && string.IsNullOrEmpty(Substitute); }

		public string Execute(string tape)
		{
			return Regex.Replace(tape, Pattern, Substitute);
		}

		public bool CanExecute(string tape)
		{
			return Regex.IsMatch(tape, Pattern);
		}

		public override string ToString()
		{
			if (IsEmpty)
				return "Empty Term";
			else
				return $"\"{Pattern}\" {(char)26} \"{Substitute}\"";
			
		}
	}
}
