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
    public partial class LoaderPopup : PopupPage
    {
        public LoaderPopup()
        {
            InitializeComponent();
        }

        public static async void CloseAllPopup()
        {
            await App.Current.MainPage.Navigation.PopAllPopupAsync(false);
        }
    }
}