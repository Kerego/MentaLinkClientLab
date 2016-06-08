using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace MentaLinkClientLab.UWP
{
	public class NavigationPage : Page
	{
		public bool AllowNavigationBack { get; set; } = true;

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);
			SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;

			SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
				Frame.CanGoBack && AllowNavigationBack ?
				AppViewBackButtonVisibility.Visible :
				AppViewBackButtonVisibility.Collapsed;
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			SystemNavigationManager.GetForCurrentView().BackRequested -= OnBackRequested;
			base.OnNavigatedFrom(e);
		}

		private void OnBackRequested(object sender, BackRequestedEventArgs e)
		{
			if (!Frame.CanGoBack)
				return;
			e.Handled = true;
			Frame.GoBack();
		}

	}
}
