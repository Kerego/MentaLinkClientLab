using MentaLinkClientLab.Core;
using MentaLinkClientLab.Core.ViewModels;

using Xamarin.Forms;

namespace MentaMobile
{
	public partial class MainPage : ContentPage
	{
		MainViewModel viewModel;
		IMqttClient _client;
		public MainPage(IMqttClient client)
		{
			_client = client;
			viewModel = new MainViewModel(client,
				game =>
				{
					Navigation.PushAsync(new GamePage(game), true);
				},
				() =>
				{
					Navigation.PushAsync(new CreateGamePage(client), true);
				});
			BindingContext = viewModel;
			InitializeComponent();
			
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();
			await viewModel.LoadCommand?.Execute();
		}
	}
}
