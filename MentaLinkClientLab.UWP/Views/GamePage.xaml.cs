using MentaLinkClientLab.Core;
using MentaLinkClientLab.Core.Helpers;
using MentaLinkClientLab.Core.Models;
using MentaLinkClientLab.Core.ViewModels;
using MentaLinkClientLab.Windows;
using Windows.System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MentaLinkClientLab.UWP.Views
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class GamePage : NavigationPage
	{
		GameViewModel<ImageSource> viewModel;
		IMqttClient client;
		public GamePage()
		{
			this.InitializeComponent();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);
			client = new WindowsMqttClient();
			IBitmapService service = new UWPBitmapService();
			viewModel = new GameViewModel<ImageSource>(client, service, e.Parameter as Game);
			ApplicationView.GetForCurrentView().Title = $"Game: {viewModel.Game} Player: {Session.User}";
			this.DataContext = viewModel;
		}
		
		private async void InputBox_KeyDown(object sender, KeyRoutedEventArgs e)
		{
			if (e.Key == VirtualKey.Enter && e.KeyStatus.RepeatCount == 0)
				await viewModel.SendAnswer();
		}

	}
}
