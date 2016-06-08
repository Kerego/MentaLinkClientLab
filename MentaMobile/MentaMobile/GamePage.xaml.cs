using MentaLinkClientLab.Core;
using MentaLinkClientLab.Core.Helpers;
using MentaLinkClientLab.Core.Models;
using MentaLinkClientLab.Core.ViewModels;
using System;

using Xamarin.Forms;

namespace MentaMobile
{
	public partial class GamePage : ContentPage
	{
		GameViewModel<ImageSource> viewModel;
		public GamePage(Game game)
		{
			InitializeComponent();
			IMqttClient client = DependencyService.Get<IMqttClient>(DependencyFetchTarget.NewInstance);
			IBitmapService service = new XamarinBitmapService();
			viewModel = new GameViewModel<ImageSource>(client, service, game);
			BindingContext = viewModel;
			InputBox.Completed += InputBox_Completed;
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			viewModel.LoadCommand.Execute(null);
		}


		private async void InputBox_Completed(object sender, EventArgs e)
		{
			await viewModel.SendAnswer(InputBox.Text);
		}
	}
}
