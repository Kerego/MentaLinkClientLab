using MentaLinkClientLab.Core;
using MentaLinkClientLab.Core.ViewModels;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MentaLinkClientLab.UWP.Views
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class CreateGamePage : NavigationPage
	{
		CreateGameViewModel viewModel;
		public CreateGamePage()
		{
			InitializeComponent();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);
			var client = e.Parameter as IMqttClient;
			viewModel = new CreateGameViewModel(client, () => Frame.GoBack());
			DataContext = viewModel;
		}
	}
}
