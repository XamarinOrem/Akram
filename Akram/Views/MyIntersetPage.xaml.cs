using Akram.ViewModels;
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
	public partial class MyIntersetPage : PopupPage
	{
		public MyIntersetPage ()
		{
			InitializeComponent ();

            NavigationPage.SetHasNavigationBar(this, false);
            BindingContext = new MyIntrestViewModel(Navigation);
        }

        void Try_Again_Button_Clicked(object sender,EventArgs e)
        {
            BindingContext = new MyIntrestViewModel(Navigation);
        }

    }
}