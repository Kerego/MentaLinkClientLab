using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace MentaLinkClientLab.Core.Models
{
	public class Game : BindableBase
	{
		public string Name { get; set; }
		public int Timeout { get; set; }
		public int InitialTimeout { get; set; }
		public int AnswersForMatch { get; set; }
		public ObservableCollection<string> Players { get; set; } = new ObservableCollection<string>();

		private bool _joined;
		public bool NotJoined
		{
			get { return _joined; }
			set { _joined = value; OnPropertyChanged(); }
		}

		public Game(string name, int timeout, int itimeout, int asnwersForMatch)
		{
			Name = name;
			Timeout = timeout;
			InitialTimeout = itimeout;
			AnswersForMatch = asnwersForMatch;
		}
		public Game()
		{

		}

	}


}
