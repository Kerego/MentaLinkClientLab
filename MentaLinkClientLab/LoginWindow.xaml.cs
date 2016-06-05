using System.Windows;
using MentaLinkClientLab.Windows;
using MentaLinkClientLab.Core.ViewModels;
using MentaLinkClientLab.Core;

namespace MentaLinkClientLab
{
	public partial class LoginWindow : Window
	{
		public LoginWindow()
		{
			InitializeComponent();
			IMqttClient client = new WindowsMqttClient();
			LoginViewModel vm = new LoginViewModel(client, () => {
				var mainWindow = new MainWindow(client);
				mainWindow.Show();
				this.Close();
			});
			DataContext = vm;
		}


	}
}
