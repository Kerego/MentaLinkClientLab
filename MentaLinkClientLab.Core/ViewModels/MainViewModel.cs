using MentaLinkClientLab.Core.Models;
using Newtonsoft.Json.Linq;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using static MentaLinkClientLab.Core.Session;

namespace MentaLinkClientLab.Core.ViewModels
{
	public class MainViewModel : BindableBase
	{
		private IMqttClient _client;
		private string _gameInputText;
		private Action<string> _joinGameAction;

		public string GameInputText
		{
			get { return _gameInputText; }
			set { _gameInputText = value; OnPropertyChanged(); }
		}
		public ICommand GameSelectedCommand { get; set; }
		public ICommand GameInputCommand { get; set; }
		public ICommand CloseCommand { get; set; }
		public ICommand LoadCommand { get; set; }
		public ObservableCollection<string> Announcements { get; set; } = new ObservableCollection<string>();
		public ObservableCollection<string> Users { get; set; } = new ObservableCollection<string>();
		public ObservableCollection<Game> Games { get; set; } = new ObservableCollection<Game>();


		public MainViewModel(IMqttClient client, Action<string> joinGameAction)
		{
			_client = client;
			_joinGameAction = joinGameAction;

			GameSelectedCommand = new DelegateCommand<Game>(async x => await Join(x), x => x.NotJoined == true);
			GameInputCommand = new DelegateCommand(async () => await GameInput());
			CloseCommand = new DelegateCommand(Close);
			LoadCommand = new DelegateCommand(async () => await Load());
		}

		private async Task Join(Game game)
		{
			if (game.Name == null)
				return;
			game.NotJoined = false;

			var message = new
			{
				type = "join",
				gamename = game.Name,
				id = Id
			};
			await _client.SendAsync(GameControl, message.ToByteArray());

			_joinGameAction?.Invoke(game.Name);
		}


		private async Task Load()
		{
			_client.Subscribe(new string[] { "Announcements" + Host }, new byte[] { 2 });

			var message = new
			{
				type = "userlist",
				username = User,
				id = Id
			};

			var gameListMessage = new
			{
				type = "gamelist",
				username = User,
				id = Id
			};

			await _client.SendAsync(GameControl, message.ToByteArray());
			await _client.SendAsync(GameControl, gameListMessage.ToByteArray());

			while (true)
			{
				var msg = await _client.ReceiveAsync();
				HandleMessage(msg);
			}

		}

		public void Close()
		{
			var message = new
			{
				type = "exit",
				username = User,
				id = Id
			};

			_client.Send(GameControl, message.ToByteArray());
			
			Task.Delay(200).Wait();
			_client.Disconnect();
		}

		private void HandleMessage(Message msg)
		{
			var response = msg.Payload.ToDynamic();

			if (msg.Topic == UserTopic)
			{
				if (response.type == "userlist")
				{
					Users.Clear();
					foreach (var item in response.payload)
					Users.Add((string)item.Value);
				}
				else if (response.type == "gamelist")
				{
					Games.Clear();
					foreach (var item in response.payload)
						Games.Add(new Game { Name = item.Name, Players = item.Value.Count, NotJoined = true });
				}
			}

			else if (msg.Topic == "Announcements" + Host ) 
			{
				Announcements.Add((string)response.payload);
				switch ((string)response.type)
				{
					case "announce_connect":
						Users.Add((string)response.username);
						break;
					case "announce_exit":
						var exitedGames = (response.gamenames as IEnumerable<object>).Select(x=>(string)((JValue)x).Value);
						foreach (var game in Games.Where(x => exitedGames.Contains(x.Name)))
							game.Players--;
						Users.Remove((string)response.username);
						break;
					case "announce_new_game":
						Games.Add(new Game { Name = response.gamename, NotJoined = true});
						break;
					case "announce_start_game":
						Games.Remove(Games.SingleOrDefault(x => x.Name == (string)response.gamename));
						break;
					case "announce_end_game":
						Games.Remove(Games.SingleOrDefault(x => x.Name == (string)response.gamename));
						break;
					case "announce_join_game":
						Games.SingleOrDefault(x => x.Name == (string)response.gamename).Players++;
						break;
					default:
						break;
				}
			}
		}

		private async Task GameInput()
		{
			if (String.IsNullOrWhiteSpace(_gameInputText))
				return;

			var message = new
			{
				type = "newgame",
				gamename = _gameInputText,
				id = Id,
			};

			_gameInputText = string.Empty;

			await _client.SendAsync(GameControl, message.ToByteArray());
		}
		


	}
}
