using MentaLinkClientLab.Core.Helpers;
using MentaLinkClientLab.Core.Models;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using static MentaLinkClientLab.Core.Session;

namespace MentaLinkClientLab.Core.ViewModels
{
	public class GameViewModel<T> : BindableBase where T : class
	{
		#region properties
		private T _imageSource;

		public T ImageSource
		{
			get { return _imageSource; }
			set { _imageSource = value; OnPropertyChanged(); }
		}

		private string _answer;

		public string Answer
		{
			get { return _answer; }
			set { _answer = value; OnPropertyChanged(); }
		}

		private string _announcement;

		public string Announcement
		{
			get { return _announcement; }
			set { _announcement = value; OnPropertyChanged(); }
		}

		private int _points;

		public int Points
		{
			get { return _points; }
			set { _points = value; OnPropertyChanged(); }
		}

		private int _round;

		public int Round
		{
			get { return _round; }
			set { _round = value; OnPropertyChanged(); }
		}

		private bool _isLoading;

		public bool IsLoading
		{
			get { return _isLoading; }
			set { _isLoading = value; OnPropertyChanged(); }
		}

		private int _timeLeft;

		public int TimeLeft
		{
			get { return _timeLeft; }
			set { _timeLeft = value; OnPropertyChanged(); }
		}

		private Game _game;
		public Game Game
		{
			get { return _game; }
			set { _game = value; OnPropertyChanged(); }
		}
		#endregion

		private IMqttClient _client;
		private IBitmapService _bitmap;
		private bool _gameStarted;

		public ICommand CloseCommand { get; }
		public ICommand LoadCommand { get; }
		public ICommand AnswerCommand { get; }

		public GameViewModel(IMqttClient client, IBitmapService bitmap, Game game)
		{
			_client = client;
			_bitmap = bitmap;
			_game = game;
			
			_client.Connect(Guid.NewGuid().ToString());
			_client.Subscribe(new string[] { GameControl + game.Name + "_announcements", GameControl + game.Name + "_images" }, new byte[] { 2, 2 });

			CloseCommand = new DelegateCommand(_client.Disconnect);
			LoadCommand = new DelegateCommand(async () => await Load());
			AnswerCommand = new DelegateCommand(async () => await SendAnswer());
		}

		private async Task Load()
		{
			var message = new
			{
				type = "join",
				gamename = _game.Name,
				id = Id
			};
			NotifyUser("Wait for the game to start!");
			DecrementTime();
			await _client.SendAsync(GameControl, message.ToByteArray());
			while (true)
			{
				var msg = await _client.ReceiveAsync();
				HandleMessage(msg);
			}
		}

		private void NotifyUser(string text)
		{
			TimeLeft = 0;
			Announcement = text;
			ImageSource = null;
			IsLoading = true;
		}
		private async Task DecrementTime()
		{
			while (true)
			{
				while (0 < TimeLeft)
				{
					TimeLeft--;
					await Task.Delay(1000);
				}
				await Task.Delay(200);
			}
		}

		private void HandleMessage(Message e)
		{
			if (e.Topic == GameControl + _game.Name + "_announcements")
			{
				var response = e.Payload.ToDynamic();
				switch ((string)response.type)
				{
					case "announce_end_game":
						if (_gameStarted)
							NotifyUser("Game Over");
						else
							NotifyUser("Game aborted, not enough players");
						ImageSource = null;
						break;
					case "announce_next_round":
						NotifyUser($"+{(string)response.points} Points");
						Points += (int)response.points;
						Round++;
						break;
					case "announce_start_game":
						NotifyUser((string)response.payload);
						_gameStarted = true;
						break;
					default:
						break;
				}
			}
			else if (e.Topic == GameControl + _game.Name + "_images")
			{
				ImageSource = _bitmap.LoadImage(e.Payload) as T;
				Announcement = "";
				IsLoading = false;
				TimeLeft = Game.Timeout;
			}
		}


		public async Task SendAnswer()
		{
			if(!_gameStarted)
			{
				NotifyUser("Wait for the game to start!");
				return;
			}

			var message = new
			{
				type = "answer",
				gamename = _game.Name,
				payload = Answer,
				id = Id
			};

			Answer = string.Empty;
			await _client.SendAsync(GameControl + _game.Name + "_answers", message.ToByteArray());
		}
	}
}
