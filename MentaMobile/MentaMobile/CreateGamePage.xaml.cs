using MentaLinkClientLab.Core;
using MentaLinkClientLab.Core.ViewModels;

using Xamarin.Forms;

namespace MentaMobile
{
	public partial class CreateGamePage : ContentPage
	{
		CreateGameViewModel viewModel;
		public CreateGamePage(IMqttClient client)
		{
			viewModel = new CreateGameViewModel(client, () => Navigation.PopAsync());
			InitializeComponent();
			BindingContext = viewModel;
		}
	}
}
