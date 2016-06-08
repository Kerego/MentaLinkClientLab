using MentaLinkClientLab.Core;
using MentaLinkClientLab.Core.ViewModels;
using Xamarin.Forms;

namespace MentaMobile
{
	public partial class LoginPage : ContentPage
	{
		LoginViewModel viewModel;
		public LoginPage(IMqttClient client)
		{
			InitializeComponent();
			viewModel = new LoginViewModel(client, () => { Navigation.PushAsync(new MainPage(client), true); Navigation.RemovePage(this); } );
			BindingContext = viewModel;
		}
	}
}
