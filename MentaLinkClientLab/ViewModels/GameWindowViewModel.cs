using System;
using System.Windows.Media;
using uPLibrary.Networking.M2Mqtt;

namespace MentaLinkClientLab.ViewModels
{
	public class GameWindowViewModel : BindableBase
	{
		private MqttClient _client;
		private string _game;
		private string _host;
		private string _id;
		private string _user;

		private ImageSource _imageSource;

		public ImageSource ImageSource
		{
			get { return _imageSource; }
			set { _imageSource = value; }
		}



		public GameWindowViewModel(string id, string host, string user, string game)
		{
			//ImageView.Source = LoadImage(File.ReadAllBytes(@"C:\Users\Kerego\Pictures\anvyl.png"));

			//_client = new MqttClient("test.mosquitto.org");
			//_id = id;
			//_host = host;
			//_user = user;
			//_game = game;
			//_client.Connect(Guid.NewGuid().ToString());
			//_client.MqttMsgPublishReceived += MessageReceived;
			//_client.Subscribe(new string[] { "GameControl" + host + game + "_announcements", "GameControl" + host + game + "_images" }, new byte[] { 2, 2 });

		}
	}
}
