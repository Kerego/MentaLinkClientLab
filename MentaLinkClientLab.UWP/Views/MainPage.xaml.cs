using MentaLinkClientLab.Core;
using MentaLinkClientLab.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MentaLinkClientLab.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
		MainViewModel viewModel;
		IMqttClient client;
		public MainPage()
        {
			this.InitializeComponent();
			Application.Current.Suspending += AppSuspending;
        }

		private void AppSuspending(object sender, SuspendingEventArgs e) => viewModel.Close();

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);
			ApplicationView.GetForCurrentView().Title = Session.User;
			client = e.Parameter as IMqttClient;
			viewModel = new MainViewModel(client,
				async game =>
				{
					//Frame.Navigate(typeof(GamePage), game);
					var currentAV = ApplicationView.GetForCurrentView();
					var newAV = CoreApplication.CreateNewView();
					await newAV.Dispatcher.RunAsync(
									CoreDispatcherPriority.Normal,
									async () =>
									{
										var newWindow = Window.Current;
										var newAppView = ApplicationView.GetForCurrentView();
										newAppView.Title = $"Game: {game} User: {Session.User}"; ;

										var frame = new Frame();
										frame.Navigate(typeof(GamePage), game);
										newWindow.Content = frame;
										newWindow.Activate();

										await ApplicationViewSwitcher.TryShowAsStandaloneAsync(
											newAppView.Id,
											ViewSizePreference.UseMinimum,
											currentAV.Id,
											ViewSizePreference.UseMinimum);
									});


				});
			this.DataContext = viewModel;
		}
	}
}
