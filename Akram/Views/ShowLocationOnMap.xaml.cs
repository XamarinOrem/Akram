using Akram.CustomControls;
using Akram.Models;
using Akram.Repository;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace Akram.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ShowLocationOnMap : ContentPage
	{
        static Plugin.Geolocator.Abstractions.Position position = null;
        public static string PickUpLatitude = "";
        public static string PickUpLongitude = "";
        public static string DestinationLatitude = "";
        public static string DestinationLongitude = "";
        public static List<PloyLineRoutes> poliList = new List<PloyLineRoutes>();
        public MyCollectionModel getData;

        public ShowLocationOnMap (MyCollectionModel _data)
		{
            getData = _data;
            NavigationPage.SetHasNavigationBar(this, false);
        }


        void Back_Tapped(object sende,EventArgs e)
        {
            Navigation.PopAsync();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await Navigation.PushPopupAsync(new LoaderPopup());

            position = await CommonLib.GetLatLong();
            PickUpLatitude = position.Latitude.ToString();
            PickUpLongitude = position.Longitude.ToString();

            InitializeComponent();

            if (Device.RuntimePlatform == Device.iOS)
            {
                firstGrid.Margin = new Thickness(0, 30, 0, 0);
            }

            MyLocationMap.HeightRequest = App.ScreenHeight - 80;

            var getLatLong = getData.imgRadio.Split(',');
            DestinationLatitude = getLatLong[0];
            DestinationLongitude = getLatLong[1];

            if (!string.IsNullOrEmpty(PickUpLatitude) && !string.IsNullOrEmpty(PickUpLongitude))
            {
                MyLocationMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(Convert.ToDouble(PickUpLatitude), Convert.ToDouble(PickUpLongitude)),
                                                    Distance.FromMeters(50)));
            }

            try
            {
                HttpClientBase _base = new HttpClientBase();
                RootObjectPolyLine obj = new RootObjectPolyLine();
                obj = await _base.GetPolyLine(Convert.ToDouble(PickUpLatitude), Convert.ToDouble(PickUpLongitude), Convert.ToDouble(DestinationLatitude), Convert.ToDouble(DestinationLongitude));
                string points = string.Empty;
                foreach (var data in obj.routes)
                {
                    points = data.overview_polyline.points;
                }

                poliList = CommonLib.DecodePoly(points);

                double dic = Calc(Convert.ToDouble(PickUpLatitude), Convert.ToDouble(PickUpLongitude), Convert.ToDouble(DestinationLatitude), Convert.ToDouble(DestinationLongitude));
                if (poliList != null)
                {
                    double destination_Latitude = 0;
                    double destination_Longitude = 0;
                    MyLocationMap.RouteCoordinates = new List<Position>();
                    foreach (var data in poliList)
                    {
                        MyLocationMap.RouteCoordinates.Add(new Position(data.Latitude, data.Longitude));
                        destination_Latitude = data.Latitude;
                        destination_Longitude = data.Longitude;
                    }


                    var start_pin = new CustomPin
                    {
                        Pin = new Pin
                        {
                            Type = PinType.Place,
                            Position = new Position(Convert.ToDouble(PickUpLatitude), Convert.ToDouble(PickUpLongitude)),
                            Label = "Current Location",
                            Address = ""
                        },
                        Id = "Xamarin",
                        startPin = true
                    };
                    var destination_pin = new CustomPin
                    {
                        Pin = new Pin
                        {
                            Type = PinType.Place,
                            Position = new Position(Convert.ToDouble(DestinationLatitude), Convert.ToDouble(DestinationLongitude)),
                            Label = "Destination Address",
                            Address = ""
                        },
                        Id = "1",
                    };

                    MyLocationMap.CustomPins = new List<CustomPin> { start_pin, destination_pin };
                    if (Device.RuntimePlatform == Device.iOS)
                    {
                        MyLocationMap.Pins.Add(start_pin.Pin);
                        MyLocationMap.Pins.Add(destination_pin.Pin);
                    }
                    LoaderPopup.CloseAllPopup();
                }
                else
                {
                    LoaderPopup.CloseAllPopup();
                    await DisplayAlert("","Path Does Not Exist","Ok");
                    await Navigation.PopAsync();
                }
            }
            catch (Exception ex)
            {
                LoaderPopup.CloseAllPopup();
            }
        }


        public static double Calc(double Lat1, double Long1, double Lat2, double Long2)
        {
            try
            {

                double dDistance = Double.MinValue;
                double dLat1InRad = Lat1 * (Math.PI / 180.0);
                double dLong1InRad = Long1 * (Math.PI / 180.0);
                double dLat2InRad = Lat2 * (Math.PI / 180.0);
                double dLong2InRad = Long2 * (Math.PI / 180.0);

                double dLongitude = dLong2InRad - dLong1InRad;
                double dLatitude = dLat2InRad - dLat1InRad;


                double a = Math.Pow(Math.Sin(dLatitude / 2.0), 2.0) +
                           Math.Cos(dLat1InRad) * Math.Cos(dLat2InRad) *
                           Math.Pow(Math.Sin(dLongitude / 2.0), 2.0);


                double c = 2.0 * Math.Asin(Math.Sqrt(a));


                const Double kEarthRadiusKms = 6376.5;
                dDistance = kEarthRadiusKms * c;

                return dDistance;

            }
            catch (Exception ex)
            {
                LoaderPopup.CloseAllPopup();
            }
            return 0;
        }

    }
}