using MentaLinkClientLab.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace MentaMobile
{
	public class App : Application
	{
		public App()
		{
			// The root page of your application
			IMqttClient client = DependencyService.Get<IMqttClient>(DependencyFetchTarget.NewInstance);
			MainPage = new NavigationPage(new LoginPage(client));
		}

		protected override void OnStart()
		{
			// Handle when your app starts
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
	}
}
