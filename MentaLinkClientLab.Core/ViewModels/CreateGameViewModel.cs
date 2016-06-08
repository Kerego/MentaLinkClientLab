using MentaLinkClientLab.Core.Models;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Threading.Tasks;

namespace MentaLinkClientLab.Core.ViewModels
{
	public class CreateGameViewModel : BindableBase
	{
		private string _name;

		public string Name
		{
			get { return _name; }
			set { _name = value; OnPropertyChanged(); CreateGameCommand.RaiseCanExecuteChanged(); }
		}

		private int _roundTimeout = 5;

		public int RoundTimeout
		{
			get { return _roundTimeout; }
			set { _roundTimeout = value; OnPropertyChanged(); }
		}

		private int _initialTimeout = 5;

		public int InitialTimeout
		{
			get { return _initialTimeout; }
			set { _initialTimeout = value; OnPropertyChanged(); }
		}

		private int _answersForMatch = 2;

		public int AnswersForMatch
		{
			get { return _answersForMatch; }
			set { _answersForMatch = value; OnPropertyChanged(); }
		}

		public DelegateCommand CreateGameCommand { get; set; }

		IMqttClient _client;
		Action _gameCreatedCallback;

		public CreateGameViewModel(IMqttClient client, Action gameCreatedCallback)
		{
			_client = client;
			_gameCreatedCallback = gameCreatedCallback;
			CreateGameCommand = new DelegateCommand(
				async () => await CreateGame(), 
				() => !String.IsNullOrWhiteSpace(_name));
		}

		private async Task CreateGame()
		{
			var message = new
			{
				type = "newgame",
				gamename = _name,
				timeout = _roundTimeout,
				itimeout = _initialTimeout,
				answers = _answersForMatch
			};
			await _client.SendAsync(Session.GameControl, message.ToByteArray());
			_gameCreatedCallback?.Invoke();
		}
	}
}
