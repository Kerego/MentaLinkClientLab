using MentaLinkClientLab.Models;
using Newtonsoft.Json.Linq;
using Prism.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MentaLinkClientLab.ViewModels
{
	public class MainWindowViewModel : BindableBase
	{
		Queue<Message> _messages = new Queue<Message>();
		private string _id;
		private string _user;
		private string _host;
		private string _userTopic;
		MqttClient _client;


		private string _gameInputText;
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


		public MainWindowViewModel(MqttClient client, string id, string user, string host)
		{
			this._id = id;
			this._user = user;
			this._host = host;
			_userTopic = "GameControl" + host + user + id;
			_client = client;

			GameSelectedCommand = new DelegateCommand<Game>(Join, x => x.NotJoined == true);
			GameInputCommand = new DelegateCommand(GameInput);
			CloseCommand = new DelegateCommand(Close);
			LoadCommand = new DelegateCommand(Load);
		}


		private void Join(Game game)
		{
			game.NotJoined = false;
			GameSelected(game);
		}


		private async void Load()
		{
			_client.MqttMsgPublishReceived += MessageReceived;
			_client.Subscribe(new string[] { "Announcements" + _host }, new byte[] { 2 });

			var message = new
			{
				type = "userlist",
				username = _user,
				id = _id
			};

			var gameListMessage = new
			{
				type = "gamelist",
				username = _user,
				id = _id
			};

			_client.Publish("GameControl" + _host, message.ToByteArray(), 2, false);
			_client.Publish("GameControl" + _host, gameListMessage.ToByteArray(), 2, false);

			while (true)
			{
				var msg = await GetMessage();
				HandleMessage(msg);
			}

		}

		private async Task<Message> GetMessage()
		{
			while (!_messages.Any())
				await Task.Delay(50);
			return _messages.Dequeue();
		}

		private void HandleMessage(Message msg)
		{
			var response = msg.Payload.ToDynamic();

			if (msg.Topic == _userTopic)
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

			else if (msg.Topic == "Announcements" + _host)
			{
				switch ((string)response.type)
				{
					case "announce_connect":
						Users.Add((string)response.username);
						Announcements.Add((string)response.payload);
						break;
					case "announce_exit":
						var exitedGames = (response.gamenames as IEnumerable<object>).Select(x=>(string)((JValue)x).Value);
						foreach (var game in Games.Where(x => exitedGames.Contains(x.Name)))
							game.Players--;
						Users.Remove((string)response.username);
						Announcements.Add((string)response.payload);
						break;
					case "announce_new_game":
						Games.Add(new Game { Name = response.gamename, NotJoined = true});
						Announcements.Add((string)response.payload);
						break;
					case "announce_start_game":
						Announcements.Add((string)response.payload);
						Games.Remove(Games.Single(x => x.Name == (string)response.gamename));
						break;
					case "announce_end_game":
						Announcements.Add((string)response.payload);
						Games.Remove(Games.SingleOrDefault(x => x.Name == (string)response.gamename));
						break;
					case "announce_join_game":
						Games.Single(x => x.Name == (string)response.gamename).Players++;
						Announcements.Add((string)response.payload);
						break;
					default:
						break;
				}
			}
		}

		private void MessageReceived(object sender, MqttMsgPublishEventArgs e)
		{
			var msg = new Message()
			{
				Topic = e.Topic,
				Payload = e.Message,
			};
			var str = e.Message.ToDynamic();
			_messages.Enqueue(msg);
		}

		private void GameInput()
		{
			if (String.IsNullOrWhiteSpace(_gameInputText))
				return;

			var message = new
			{
				type = "newgame",
				gamename = _gameInputText,
				id = _id,
			};

			_gameInputText = string.Empty;

			_client.Publish("GameControl" + _host, message.ToByteArray(), 2, false);
		}

		private void GameSelected(Game game)
		{
			string gn = game.Name;
			if (gn == null)
				return;

			var message = new
			{
				type = "join",
				gamename = gn,
				id = _id
			};
			_client.Publish("GameControl" + _host, message.ToByteArray(), 2, false);
			
			//not nice
			var window = new GameWindow(_id, _host, _user, gn);
			window.Show();
		}


		private void Close()
		{
			var message = new
			{
				type = "exit",
				username = _user,
				//gamenames = Games.Where(x=>x.NotJoined == false).Select(x=>x.Name),
				id = _id
			};

			_client.Publish("GameControl" + _host, message.ToByteArray(), 2, false);

			Task.Delay(200).Wait();
			_client.Disconnect();
		}


	}
}
