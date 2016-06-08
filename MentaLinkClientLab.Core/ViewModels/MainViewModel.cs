using MentaLinkClientLab.Core.Models;
using Newtonsoft.Json.Linq;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using static MentaLinkClientLab.Core.Session;

namespace MentaLinkClientLab.Core.ViewModels
{
	public class MainViewModel : BindableBase
	{
		private Game _selectedGame;

		public Game SelectedGame
		{
			get { return _selectedGame; }
			set { _selectedGame = value; OnPropertyChanged(); JoinGameCommand.RaiseCanExecuteChanged(); }
		}

		public DelegateCommand JoinGameCommand { get; set; }
		public DelegateCommand CreateGameCommand { get; set; }
		public DelegateCommand CloseCommand { get; set; }
		public DelegateCommand LoadCommand { get; set; }
		public ObservableCollection<string> Announcements { get; set; } = new ObservableCollection<string>();
		public ObservableCollection<string> Users { get; set; } = new ObservableCollection<string>();
		public ObservableCollection<Game> Games { get; set; } = new ObservableCollection<Game>();
		
		private IMqttClient _client;
		private Action<Game> _joinGameAction;
		private Action _createGameAction;


		public MainViewModel(IMqttClient client, Action<Game> joinGameAction, Action createGameAction)
		{
			_client = client;
			_joinGameAction = joinGameAction;
			_createGameAction = createGameAction;

			JoinGameCommand = new DelegateCommand(() => Join(SelectedGame), () => SelectedGame?.NotJoined == true);
			CreateGameCommand = new DelegateCommand(_createGameAction.Invoke);
			CloseCommand = new DelegateCommand(Close);
			LoadCommand = new DelegateCommand(async () => await Load());
		}

		private void Join(Game game)
		{
			if (game.Name == null)
				return;
			game.NotJoined = false;
			JoinGameCommand.RaiseCanExecuteChanged();
			_joinGameAction?.Invoke(game);
		}


		private async Task Load()
		{
			_client.Subscribe(new string[] { "Announcements" + Host }, new byte[] { 2 });
			await RequestList("userlist");
			await RequestList("gamelist");
			await ProcessMessages();
		}

		private async Task RequestList(string listType)
		{
			var message = new
			{
				type = listType,
				username = User,
				id = Id
			};

			await _client.SendAsync(GameControl, message.ToByteArray());
		}

		public async Task ProcessMessages()
		{
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
						Games.Add(new Game {
									Name = item.Name,
									Players = new ObservableCollection<string>(((JArray)item.Value).ToObject<string[]>()), NotJoined = true
									});
				}
			}

			else if (msg.Topic == "Announcements" + Host ) 
			{
				Announcements.Insert(0, (string)response.payload);
				switch ((string)response.type)
				{
					case "announce_connect":
						Users.Add((string)response.username);
						break;
					case "announce_exit":
						var exitedGames = (response.gamenames as IEnumerable<object>).Select(x=>(string)((JValue)x).Value);
						foreach (var game in Games.Where(x => exitedGames.Contains(x.Name)))
							game.Players.Remove((string)response.username);
						Users.Remove((string)response.username);
						break;
					case "announce_new_game":
						Games.Add(new Game {
							Name = response.gamename,
							Timeout = response.timeout,
							InitialTimeout = response.itimeout,
							AnswersForMatch = response.answers,
							NotJoined = true});
						break;
					case "announce_start_game":
						Games.Remove(Games.SingleOrDefault(x => x.Name == (string)response.gamename));
						break;
					case "announce_end_game":
						Games.Remove(Games.SingleOrDefault(x => x.Name == (string)response.gamename));
						break;
					case "announce_join_game":
						var g = Games.SingleOrDefault(x => x.Name == (string)response.gamename);
						if(g != null)
							g.Players.Add((string)response.username);
						break;
					default:
						break;
				}
			}
		}


	}
}
