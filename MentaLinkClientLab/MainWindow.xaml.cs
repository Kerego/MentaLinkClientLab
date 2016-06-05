using MentaLinkClientLab.Core;
using MentaLinkClientLab.Core.ViewModels;
using System.Windows;

namespace MentaLinkClientLab
{
	public partial class MainWindow : Window
	{
		MainViewModel viewModel;
		public MainWindow(IMqttClient client)
		{
			viewModel = new MainViewModel(client,
			game => 
			{
				GameWindow window = new GameWindow(game);
				window.Show();
			});
			this.Title = Session.User;
			this.DataContext = viewModel;
			InitializeComponent();
		}
	}
}
