using Prism.Mvvm;

namespace MentaLinkClientLab.Core.Models
{
	public class Game : BindableBase
	{
		public string Name { get; set; }
		private int _players;

		public int Players
		{
			get { return _players; }
			set { _players = value; OnPropertyChanged(); }
		}

		private bool _joined;
		public bool NotJoined
		{
			get { return _joined; }
			set { _joined = value; OnPropertyChanged(); }
		}

	}


}
