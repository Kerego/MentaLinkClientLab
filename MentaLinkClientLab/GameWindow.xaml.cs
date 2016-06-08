using System.Windows;
using System.Windows.Controls;
using MentaLinkClientLab.Core;
using MentaLinkClientLab.Windows;
using MentaLinkClientLab.Core.ViewModels;
using static MentaLinkClientLab.Core.Session;
using MentaLinkClientLab.Core.Helpers;
using System.Windows.Media;
using MentaLinkClientLab.Core.Models;

namespace MentaLinkClientLab
{
	/// <summary>
	/// Interaction logic for GameWindow.xaml
	/// </summary>
	public partial class GameWindow : Window
	{
		public GameWindow(Game game)
		{
			Title = $"Game: {game} User: {User}";
			InitializeComponent();
			IMqttClient client = new WindowsMqttClient();
			IBitmapService bitmap = new WpfBitmapService();
			DataContext = new GameViewModel<ImageSource>(client, bitmap, game);
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
