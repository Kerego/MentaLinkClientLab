using MentaLinkClientLab.ViewModels;
using System.Windows;
using uPLibrary.Networking.M2Mqtt;

namespace MentaLinkClientLab
{

	public partial class MainWindow : Window
	{
		MainWindowViewModel viewModel;
		public MainWindow(MqttClient client, string id, string user, string host)
		{
			viewModel = new MainWindowViewModel(client, id, user, host);
			this.Title = user;
			this.DataContext = viewModel;
			InitializeComponent();
		}
	}
}
