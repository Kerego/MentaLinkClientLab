namespace MentaLinkClientLab.Core.Models
{
	public class Message
	{
		public string Topic { get; set; }
		public byte[] Payload { get; set; }

		public Message(string topic, byte[] message)
		{
			Topic = topic;
			Payload = message;
		}
	}
}
