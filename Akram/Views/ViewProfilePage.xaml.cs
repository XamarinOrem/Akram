using Akram.Models;
using Akram.ViewModels;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Akram.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ViewProfilePage : ContentPage
	{
        public static bool IsLoad = false;
		public ViewProfilePage ()
		{
            try
            {
                InitializeComponent();

                NavigationPage.SetHasNavigationBar(this, false);

                if (Device.RuntimePlatform == Device.iOS)
                {
                    firstGrid.Margin = new Thickness(0, 30, 0, 0);
                }

                if(LoginUserDetails.IsLoggedInFacebook)
                {
                    changePasswordBtn.IsVisible = false;
                    changePasswordLine.IsVisible = false;
                }
                else
                {
                    changePasswordBtn.IsVisible = true;
                    changePasswordLine.IsVisible = true;
                }
                BindingContext = new ViewProfileViewModel(Navigation);
            }
            catch (Exception)
            {

            }

		}

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (IsLoad)
            {
                BindingContext = new ViewProfileViewModel(Navigation);
                IsLoad = false;
            }
        }

        protected override bool OnBackButtonPressed()
        {
            return true;       
        }

        void Try_Again_Button_Clicked(object sender, EventArgs e)
        {
            BindingContext = new ViewProfileViewModel(Navigation);
        }
    }
}