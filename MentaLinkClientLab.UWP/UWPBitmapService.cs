using MentaLinkClientLab.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace MentaLinkClientLab.UWP
{
	public class UWPBitmapService : IBitmapService
	{
		public object LoadImage(byte[] buffer)
		{
			using (InMemoryRandomAccessStream ms = new InMemoryRandomAccessStream())
			{
				using (DataWriter writer = new DataWriter(ms.GetOutputStreamAt(0)))
				{
					writer.WriteBytes((byte[])buffer);
					writer.StoreAsync().GetResults();
				}
				var image = new BitmapImage();
				image.SetSource(ms);
				return image;
			}
		}
	}
}
