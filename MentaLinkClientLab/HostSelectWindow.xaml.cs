using MentaLinkClientLab.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MentaLinkClientLab
{

	public static class Extensions
	{
		public static byte[] ToByteArray(this object obj)
		{
			var str = JsonConvert.SerializeObject(obj);
			return Encoding.UTF8.GetBytes(str);
		}

		public static dynamic ToDynamic(this byte[] array)
		{
			var str = Encoding.UTF8.GetString(array);
			return JsonConvert.DeserializeObject(str);
		}

		public static string AsString(this byte[] array) => Encoding.UTF8.GetString(array);

	}

	/// <summary>
	/// Interaction logic for HostSelectWindow.xaml
	/// </summary>
	public partial class HostSelectWindow : Window
	{
		MqttClient client;
		Guid user = Guid.NewGuid();
		public HostSelectWindow()
		{
			InitializeComponent();
			client = new MqttClient("test.mosquitto.org");
			client.Connect(user.ToString());

			HostBox.Text = "_AA";
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			ConnectLabel.Text = "Connecting...";

			string host = "GameControl" + HostBox.Text;
			var myTopic = host + UserBox.Text + user.ToString();

			var message = new
			{
				type = "connect",
				username = UserBox.Text,
				id = user.ToString()
			};

			client.Publish(host, message.ToByteArray(), 2, false);

			client.Subscribe(new string[] { myTopic },
				new byte[] { 2 });

			client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
		}

		private void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
		{
			var response = e.Message.ToDynamic();
			if(response.type == "connect" && response.status == "ok")
			{
				Dispatcher.InvokeAsync(() =>
				{
					var client = sender as MqttClient;
					var mainWindow = new MainWindow(client, user.ToString(), UserBox.Text, HostBox.Text);
					mainWindow.Show();
					this.Close();
					client.MqttMsgPublishReceived -= Client_MqttMsgPublishReceived;
				});
			}




		}
	}
}
