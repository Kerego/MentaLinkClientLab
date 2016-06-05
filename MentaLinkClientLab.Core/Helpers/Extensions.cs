using Newtonsoft.Json;
using System.Text;

namespace MentaLinkClientLab.Core
{
	public static class Extensions
	{
		public static byte[] ToByteArray(this object obj) => Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));

		public static dynamic ToDynamic(this byte[] array) => JsonConvert.DeserializeObject(Encoding.UTF8.GetString(array, 0, array.Length));

		public static string AsString(this byte[] array) => Encoding.UTF8.GetString(array, 0, array.Length);

	}
}
