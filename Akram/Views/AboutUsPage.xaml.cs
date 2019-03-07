using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Akram.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AboutUsPage : ContentPage
	{
		public AboutUsPage ()
		{
			InitializeComponent ();

            if (Device.RuntimePlatform == Device.iOS)
            {
                firstGrid.Margin = new Thickness(0, 30, 0, 0);
            }


            NavigationPage.SetHasNavigationBar(this, false);
		}

        void Back_Tapped(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private void Insta_Tapped(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri("https://instagram.com/akram_app?utm_source=ig_profile_share&igshid=1sfqgi7n7qjt6"));
        }
        private void Facebook_Tapped(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri("https://m.facebook.com/AkramAppOffical/?locale2=ar_AR"));
        }
        private void Email_Tapped(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri("mailto:Info@AkramApp.com"));
        }
        private void Link_Tapped(object sender, EventArgs e)
        {
            var getLabel = sender as Label;
            Device.OpenUri(new Uri(getLabel.Text));
        }
    }
}