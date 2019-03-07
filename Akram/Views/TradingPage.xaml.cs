using Akram.DependencyInterface;
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
	public partial class TradingPage : ContentPage
	{
        public static bool CodeScanned = false;
		public TradingPage ()
		{
			InitializeComponent ();

            NavigationPage.SetHasNavigationBar(this, false);

            if (Device.RuntimePlatform == Device.Android)
            {
                DependencyService.Get<IStoragePermissions>().GetScannerPermissions();
            }

            if (Device.RuntimePlatform == Device.iOS)
            {
                firstGrid.Margin = new Thickness(0, 30, 0, 0);
            }

            BindingContext = new TradingViewModel(Navigation);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (CodeScanned)
            {
                TradingViewModel.ScanResult();
                CodeScanned = false;
            }
        }

        void Back_Tapped(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }
    }
}