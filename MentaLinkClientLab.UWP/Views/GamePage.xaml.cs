using MentaLinkClientLab.Core;
using MentaLinkClientLab.Core.Helpers;
using MentaLinkClientLab.Core.ViewModels;
using MentaLinkClientLab.Windows;
using Prism.Mvvm;
using Windows.ApplicationModel;
using Windows.System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MentaLinkClientLab.UWP.Views
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class GamePage : Page
	{
		GameViewModel<ImageSource> viewModel;
		IMqttClient client;
		public GamePage()
		{
			this.InitializeComponent();
			Application.Current.Suspending += (s,e) => client.Disconnect();
			InputPane.GetForCurrentView().Showing += KeyboardShow;
			InputPane.GetForCurrentView().Hiding += KeyboardHide;
		}

		private void KeyboardHide(InputPane sender, InputPaneVisibilityEventArgs args)
		{
			ChatContainer.Visibility = Visibility.Visible;
		}

		private void KeyboardShow(InputPane sender, InputPaneVisibilityEventArgs args)
		{
			//ImageContaienr.Height = this.ActualHeight - args.OccludedRect.Height - 50;
			ChatContainer.Visibility = Visibility.Collapsed;
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);
			client = new WindowsMqttClient();
			IBitmapService service = new UWPBitmapService();
			viewModel = new GameViewModel<ImageSource>(client, service, e.Parameter as string);
			this.DataContext = viewModel;
		}
		
		private async void InputBox_KeyDown(object sender, KeyRoutedEventArgs e)
		{
			if (e.Key == VirtualKey.Enter && e.KeyStatus.RepeatCount == 0)
				await viewModel.SendAnswer();
		}


		bool _autoScroll = true;
		private void ScrollViewer_ViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
		{
			var scrollView = sender as ScrollViewer;
			var ExtentHeightChange = e.FinalView.HorizontalOffset - scrollView.HorizontalOffset;
			// User scroll event : set or unset autoscroll mode
			if (ExtentHeightChange == 0)
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
			if (_autoScroll && ExtentHeightChange != 0)
			{   // Content changed and autoscroll mode set
				// Autoscroll
				scrollView.ScrollToVerticalOffset(scrollView.ExtentHeight);
			}
		}

	}
}
