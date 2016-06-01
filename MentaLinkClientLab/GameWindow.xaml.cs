using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MentaLinkClientLab
{
	/// <summary>
	/// Interaction logic for GameWindow.xaml
	/// </summary>
	public partial class GameWindow : Window
	{
		private MqttClient _client;
		private string _game;
		private string _host;
		private string _id;
		private string _user;

		ObservableCollection<string> Announcements = new ObservableCollection<string>();

		public GameWindow(string id, string host, string user, string game)
		{
			_client = new MqttClient("test.mosquitto.org");
			_id = id;
			_host = host;
			_user = user;
			_game = game;
			Title = game + " " +_user;
			InitializeComponent();
			_client.Connect(Guid.NewGuid().ToString());
			_client.MqttMsgPublishReceived += MessageReceived;
			_client.Subscribe(new string[] { "GameControl" + host + game + "_announcements", "GameControl" + host + game + "_images" },  new byte[] { 2, 2 });
			//ImageView.Source = LoadImage(File.ReadAllBytes(@"C:\Users\Kerego\Pictures\anvyl.png"));
			ChatList.ItemsSource = Announcements;
			this.Closed += WindowClosed;
		}

		private void WindowClosed(object sender, EventArgs e)
		{
			_client.Disconnect();
		}

		private void MessageReceived(object sender, MqttMsgPublishEventArgs e)
		{
			if(e.Topic == "GameControl" + _host + _game + "_announcements")
			{
				var response = e.Message.ToDynamic();
				switch ((string)response.type)
				{
					case "announce_end_game":
						Dispatcher.InvokeAsync(() => Content = new TextBlock() {
							HorizontalAlignment = HorizontalAlignment.Center,
							VerticalAlignment = VerticalAlignment.Center,
							Text = "Game Ended or aborted\r\nPoints: " + (string)response.points });
						break;
					case "announce_next_round":
						Dispatcher.InvokeAsync(() =>
						{
							Announcements.Add($"Match! Points: " + (string)response.points);
						});
						break;
					case "announce_start_game":
						Dispatcher.InvokeAsync(() => InputBox.IsEnabled = true);
						//InputBox.IsEnabled = true;
						break;
					default:
						break;
				}
			}
			else if(e.Topic == "GameControl" + _host + _game + "_images")
			{
				Dispatcher.InvokeAsync(() => {
					try
					{
						ImageView.Source = LoadImage(e.Message);
					}
					catch(Exception ex)
					{
						MessageBox.Show($"Cannot convert binary to image\r\n{ex.Message}");
					}
				});
			}
		}

		private void TextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Enter)
				return;

			var message = new
			{
				type = "answer",
				gamename = _game,
				payload = (sender as TextBox).Text,
				id = _id
			};
			
			(sender as TextBox).Text = string.Empty;

			_client.Publish("GameControl" + _host + _game + "_answers", message.ToByteArray(), 2, false);

		}

		private BitmapImage LoadImage(byte[] imageData)
		{
			if (imageData == null || imageData.Length == 0) return null;
			var image = new BitmapImage();
			using (var mem = new MemoryStream(imageData))
			{
				mem.Position = 0;
				image.BeginInit();
				image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
				image.CacheOption = BitmapCacheOption.OnLoad;
				image.UriSource = null;
				image.StreamSource = mem;
				image.EndInit();
			}
			image.Freeze();
			return image;
		}

		private bool _autoScroll = true;
		private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			var scrollView = sender as ScrollViewer;
			// User scroll event : set or unset autoscroll mode
			if (e.ExtentHeightChange == 0)
			{   // Content unchanged : user scroll event
				if (scrollView.VerticalOffset == scrollView.ScrollableHeight)
				{   // Scroll bar is in bottom
					// Set autoscroll mode
					_autoScroll = true;
				}
				else
				{   // Scroll bar isn't in bottom
					// Unset autoscroll mode
					_autoScroll = false;
				}
			}

			// Content scroll event : autoscroll eventually
			if (_autoScroll && e.ExtentHeightChange != 0)
			{   // Content changed and autoscroll mode set
				// Autoscroll
				scrollView.ScrollToVerticalOffset(scrollView.ExtentHeight);
			}

		}
	}
}
