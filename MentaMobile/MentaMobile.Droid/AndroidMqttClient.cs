using System;
using System.Collections.Generic;
using System.Linq;
using MentaLinkClientLab.Core;
using uPLibrary.Networking.M2Mqtt;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt.Messages;
using MentaLinkClientLab.Core.Models;
using MentaMobile.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(AndroidMqttClient))]
namespace MentaMobile.Droid
{
	public class AndroidMqttClient : IMqttClient, IDisposable
	{
		MqttClient _client = new MqttClient(Constants.BrokerUrl);
		Queue<Message> _messages = new Queue<Message>();
		public AndroidMqttClient()
		{
			_client.MqttMsgPublishReceived += MessageReceived;
		}
		private void MessageReceived(object sender, MqttMsgPublishEventArgs e) => _messages.Enqueue(new Message(e.Topic, e.Message));
		public void Connect(string clientId) => _client.Connect(clientId);
		public async Task ConnectAsync(string clientId) => await Task.Run(() => Connect(clientId));
		public void Send(string topic, byte[] payload) => _client.Publish(topic, payload, 2, false);
		public async Task SendAsync(string topic, byte[] payload) => await Task.Run(() => Send(topic, payload));
		public void Subscribe(string[] topics, byte[] qosLevels) => _client.Subscribe(topics, qosLevels);
		public void UnSubscribe(string[] topics) => _client.Unsubscribe(topics);
		public async Task SubscribeAsync(string[] topics, byte[] qosLevels) => await Task.Run(() => Subscribe(topics, qosLevels));
		public async Task UnSubscribeAsync(string[] topics) => await Task.Run(() => UnSubscribe(topics));
		public void Disconnect() => _client.Disconnect();
		public void Dispose() => Disconnect();
		public async Task<Message> ReceiveAsync()
		{
			while (!_messages.Any())
				await Task.Delay(50);
			return _messages.Dequeue();
		}

	}
}