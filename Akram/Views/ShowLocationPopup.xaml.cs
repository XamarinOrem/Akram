using Akram.DependencyInterface;
using Akram.Models;
using Akram.Repository;
using Plugin.Geolocator;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Akram.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ShowLocationPopup : PopupPage
    {
        string PickUpLatitude = string.Empty;
        string PickUpLongitude = string.Empty;

        ObservableCollection<MyCollectionModel> obj;

        public ShowLocationPopup()
        {
            InitializeComponent();

            CloseWhenBackgroundIsClicked = false;

            var result = App.Database.GetLocations();

            if (result.Count > 0)
            {
                obj = new ObservableCollection<MyCollectionModel>();
                foreach (var item in result)
                {
                    obj.Add(new MyCollectionModel
                    {
                        Item = CommonLib.FirstCharToUpper(item.Name),
                        imgRadio = item.LatLong
                    });
                }

                locationList.ItemsSource = obj;
                locationList.HeightRequest = obj.Count * 55;
            }
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            Navigation.PopPopupAsync();

        }


        private void locationList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var item = e.Item as MyCollectionModel;          
            DependencyService.Get<ILaunchGoogleMap>().OpenGoogleMap(item.imgRadio);
            //Navigation.PushAsync(new ShowLocationOnMap(item));
        }
    }
}