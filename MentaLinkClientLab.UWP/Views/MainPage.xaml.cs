using MentaLinkClientLab.Core;
using MentaLinkClientLab.Core.ViewModels;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MentaLinkClientLab.UWP.Views
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : NavigationPage
	{
		MainViewModel viewModel;
		IMqttClient client;
		public MainPage()
		{
			this.InitializeComponent();
			Application.Current.Suspending += (s,e) => viewModel.Close();
			NavigationCacheMode = NavigationCacheMode.Enabled;
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);
			if (e.NavigationMode == NavigationMode.Back)
				return;
			Frame.BackStack.RemoveAt(Frame.BackStack.Count - 1);
			ApplicationView.GetForCurrentView().Title = Session.User;
			client = e.Parameter as IMqttClient;
			viewModel = new MainViewModel
			(
				client, 
				game => Frame.Navigate(typeof(GamePage), game),
				() => Frame.Navigate(typeof(CreateGamePage), client)
			);
			this.DataContext = viewModel;
		}
	}
}
