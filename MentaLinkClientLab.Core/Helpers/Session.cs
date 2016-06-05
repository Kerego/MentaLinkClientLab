using System;

namespace MentaLinkClientLab.Core
{
	public sealed class Session
	{
		public static string User { get; set; }
		public static string Host { get; set; }
		public static string GameControl { get; set; }
		public static Guid Id { get; set; }
		public static string UserTopic { get { return GameControl + User + Id; } }
	}
}
