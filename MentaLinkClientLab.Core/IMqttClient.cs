using MentaLinkClientLab.Core.Models;
using System.Threading.Tasks;

namespace MentaLinkClientLab.Core
{
	public interface IMqttClient
	{
		void Connect(string clientId);
		Task ConnectAsync(string clientId);
		void Disconnect();
		void Send(string topic, byte[] payload);
		Task SendAsync(string topic, byte[] payload);
		Task<Message> ReceiveAsync();
		void Subscribe(string[] topics, byte[] qualityService);
		void UnSubscribe(string[] topics);
		Task SubscribeAsync(string[] topics, byte[] qualityService);
		Task UnSubscribeAsync(string[] topics);
	}
}
