using MentaLinkClientLab.Core;
using MentaLinkClientLab.Core.ViewModels;
using MentaLinkClientLab.Windows;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MentaLinkClientLab.UWP.Views
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class LoginPage : Page
	{
		LoginViewModel viewModel;
		public LoginPage()
		{
			InitializeComponent();
			IMqttClient client = new WindowsMqttClient();
			viewModel = new LoginViewModel(client, () => Frame.Navigate(typeof(MainPage), client));
			DataContext = viewModel;
		}
	}
}
