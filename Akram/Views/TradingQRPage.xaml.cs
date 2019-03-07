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
	public partial class TradingQRPage : ContentPage
	{
		public TradingQRPage ()
		{
			InitializeComponent ();

            NavigationPage.SetHasNavigationBar(this, false);

            if (Device.RuntimePlatform == Device.iOS)
            {
                firstGrid.Margin = new Thickness(0, 30, 0, 0);
            }

            BindingContext = new TradingQRViewModel(Navigation);
        }
	}
}