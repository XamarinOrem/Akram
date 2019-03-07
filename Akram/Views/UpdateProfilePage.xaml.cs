using Akram.Models;
using Akram.ViewModels;
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
	public partial class UpdateProfilePage : ContentPage
	{
		public UpdateProfilePage (ProfileResult profileResult)
		{
			InitializeComponent ();

            NavigationPage.SetHasNavigationBar(this, false);

            if (Device.RuntimePlatform == Device.iOS)
            {
                firstGrid.Margin = new Thickness(0, 30, 0, 0);
            }

            BindingContext = new UpdateProfileViewModel(Navigation, profileResult);
        }

        void Birth_Date_Tapped(object sender, EventArgs e)
        {
            birthDatePicker.Focus();
        }

        private void Gender_Tapped(object sender, EventArgs e)
        {
            GenderPicker.Focus();
        }

    }
}