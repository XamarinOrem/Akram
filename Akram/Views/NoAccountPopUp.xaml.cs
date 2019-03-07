using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
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
	public partial class NoAccountPopUp : PopupPage
	{
		public NoAccountPopUp ()
		{
			InitializeComponent ();

            HomeMapPage.IsAccountPageOpened = true;
        }

        private void Login_Tapped(object sender, EventArgs e)
        {
            Navigation.PopPopupAsync();
            Application.Current.MainPage = new NavigationPage(new LoginPage());
        }

        private void Cancel_Tapped(object sender, EventArgs e)
        {
            Navigation.PopPopupAsync();
        }
    }
}