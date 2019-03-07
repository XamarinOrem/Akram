using Akram.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Akram.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class NotificationPage : ContentPage
	{
		public NotificationPage ()
		{
			InitializeComponent ();

            if (Device.RuntimePlatform == Device.iOS)
            {
                firstGrid.Margin = new Thickness(0, 30, 0, 0);

                secondGrid.Margin = new Thickness(0, 30, 0, 0);
            }

            NavigationPage.SetHasNavigationBar(this, false);
            BindingContext = new NotificationViewModel(Navigation);
        }
	}
}