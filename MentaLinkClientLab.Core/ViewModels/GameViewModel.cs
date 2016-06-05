using MentaLinkClientLab.Core.Helpers;
using MentaLinkClientLab.Core.Models;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using static MentaLinkClientLab.Core.Session;

namespace MentaLinkClientLab.Core.ViewModels
{
	public class GameViewModel<T> : BindableBase where T : class
	{
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

		private IMqttClient _client;
		private IBitmapService _bitmap;
		private string _game;
		private bool _gameStarted;

		public ObservableCollection<string> Announcements { get; set; } = new ObservableCollection<string>();
		public ICommand CloseCommand { get; }
		public ICommand LoadCommand { get; }
		public ICommand AnswerCommand { get; }

		public GameViewModel(IMqttClient client, IBitmapService bitmap, string game)
		{
			_client = client;
			_bitmap = bitmap;
			_game = game;
			
			_client.Connect(Guid.NewGuid().ToString());
			_client.Subscribe(new string[] { GameControl + game + "_announcements", GameControl + game + "_images" }, new byte[] { 2, 2 });

			CloseCommand = new DelegateCommand(_client.Disconnect);
			LoadCommand = new DelegateCommand(async () => await Load());
			AnswerCommand = new DelegateCommand(async () => await SendAnswer());
		}


		private async Task Load()
		{
			while (true)
			{
				var msg = await _client.ReceiveAsync();
				HandleMessage(msg);
			}
		}

		private void HandleMessage(Message e)
		{
			if (e.Topic == GameControl + _game + "_announcements")
			{
				var response = e.Payload.ToDynamic();
				switch ((string)response.type)
				{
					case "announce_end_game":
						if (_gameStarted)
							Announcements.Add("Game Over, Total Points: " + (string)response.points);
						else
							Announcements.Add("Game aborted, not enough players");
						//Content = new TextBlock
						//{
						//	HorizontalAlignment = HorizontalAlignment.Center,
						//	VerticalAlignment = VerticalAlignment.Center,
						//	Text = "Game Ended or aborted\r\nPoints: " + (string)response.points
						//};
						break;
					case "announce_next_round":
						Announcements.Add($"Match! Points: " + (string)response.points);
						break;
					case "announce_start_game":
						Announcements.Add((string)response.payload);
						_gameStarted = true;
						break;
					default:
						break;
				}
			}
			else if (e.Topic == GameControl + _game + "_images")
			{
				ImageSource = _bitmap.LoadImage(e.Payload) as T;
			}
		}


		public async Task SendAnswer()
		{
			if(!_gameStarted)
			{
				Announcements.Add("Wait for start of the game!");
				return;
			}

			var message = new
			{
				type = "answer",
				gamename = _game,
				payload = Answer,
				id = Id
			};
			
			Announcements.Add(Answer);

			Answer = string.Empty;

			await _client.SendAsync(GameControl + _game + "_answers", message.ToByteArray());
		}
	}
}
