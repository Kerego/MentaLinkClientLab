using MentaLinkClientLab.Core.Helpers;
using System.IO;
using Xamarin.Forms;

namespace MentaMobile
{
	public class XamarinBitmapService : IBitmapService
	{
		public object LoadImage(byte[] buffer)
		{
			ImageSource retSource = null;
			if (buffer != null)
				retSource = ImageSource.FromStream(() => new MemoryStream(buffer));
			return retSource;
		}
	}
}
