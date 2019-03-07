using Akram.DependencyInterface;
using Akram.ViewModels;
using Rg.Plugins.Popup.Extensions;
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
    public partial class MyCollectionPage : ContentPage
    {
        public MyCollectionPage()
        {
            try
            {
                InitializeComponent();

                if (Device.RuntimePlatform == Device.iOS)
                {
                    firstGrid.Margin = new Thickness(0, 30, 0, 0);
                    secondGrid.Margin = new Thickness(0, 30, 0, 0);
                    thirdGrid.Margin = new Thickness(0, 30, 0, 0);
                }

                NavigationPage.SetHasNavigationBar(this, false);

                NoData.IsVisible = false;

                mainLayout.IsVisible = false;

                loaderLayout.IsVisible = true;

                DependencyService.Get<IFirebaseDatabase>().GetGiftsMyCollection();

                MessagingCenter.Subscribe<Application, bool>(this, "LoadCollections", (sender, message) =>
                {
                    loaderLayout.IsVisible = false;

                    BindingContext = new MyCollectionViewModel(Navigation);

                    var bindingContext = (MyCollectionViewModel)BindingContext;

                    if (bindingContext != null)
                    {
                        if (bindingContext.NoDataVisible)
                        {
                            NoData.IsVisible = true;
                            mainLayout.IsVisible = false;
                        }
                        else if (bindingContext.MyCollectionList.Count > 0)
                        {
                            NoData.IsVisible = false;
                            mainLayout.IsVisible = true;
                        }
                    }
                });
            }

            catch (Exception ex)
            {
                
            }
        }

        void Back_Tapped(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }
    }
}