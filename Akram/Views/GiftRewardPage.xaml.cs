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
	public partial class GiftRewardPage : ContentPage
	{
		public GiftRewardPage ()
		{
			InitializeComponent ();

            NavigationPage.SetHasNavigationBar(this, false);

            if (Device.RuntimePlatform == Device.iOS)
            {
                firstGrid.Margin = new Thickness(0, 30, 0, 0);
            }
		}

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }
    }
}