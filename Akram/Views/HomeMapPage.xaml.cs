using Akram.CustomControls;
using Akram.DependencyInterface;
using Akram.Models;
using Akram.Repository;
using Plugin.Geolocator;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace Akram.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomeMapPage : ContentPage
    {
        public static double _Radius;
        public static Plugin.Geolocator.Abstractions.Position position = null;
        public string minTime = "0";
        public string secTime = "0";
        public static double min = 05;
        public static double sec = 0;
        public bool IsPopUpDisplayed = false;
        public static wsGiftsResponse result = null;
        public static bool IsPageOpened = false;
        public static bool IsAccountPageOpened = false;
        public static bool IsGiftPageOpened = false;
        public static bool IsStarted = false;
        public static string pokemanId = string.Empty;
        public static bool IsRefresh = false;
        public static bool checkGpsEnabled;
        public static PermissionStatus status;
        int i = 0;
        public static List<string> AllItemIds = new List<string>();

        public HomeMapPage()
        {
            position = null;
            IsStarted = false;
            min = 05;
            sec = 0;

            if (Device.RuntimePlatform == Device.Android)
            {
                DependencyService.Get<IStoragePermissions>().GetLocationPermissions();
            }
            else
            {
                DependencyService.Get<IStoragePermissions>().GetContactPermissions();
            }

            InitializeComponent();

            MessagingCenter.Subscribe<Application, bool>(this, "LoadGifts", (sender, message) =>
            {
                ShowGifts(position.Latitude, position.Longitude, 1.0);
            });

            MessagingCenter.Subscribe<Application, bool>(this, "RefreshGifts", (sender, message) =>
            {
                ShowNewGifts(position.Latitude, position.Longitude, 1.0);
            });

            if (Device.RuntimePlatform == Device.iOS)
            {
                firstLayout.Padding = new Thickness(0, 30, 0, 0);
            }

            if (position != null)
            {
                Device.StartTimer(TimeSpan.FromMinutes(1), () =>
                {
                    if (LoginUserDetails.IsLoggedIn)
                    {
                        DependencyService.Get<IFirebaseDatabase>().MakeConnectionForReferesh();
                    }
                    else
                    {
                        ShowNewGifts(position.Latitude, position.Longitude, 1.0);
                    }
                    return true;
                });
            }

            NavigationPage.SetHasNavigationBar(this, false);

            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                checkGpsEnabled = DependencyService.Get<ILocSettings>().CheckGpsEnable();
                Task.Run(async () =>
                {
                    status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        if (!checkGpsEnabled || status != PermissionStatus.Granted)
                        {
                            if (position != null)
                            {
                                if (!IsPopUpDisplayed&&status==PermissionStatus.Granted)
                                {
                                    i = 0;
                                    IsPopUpDisplayed = true;
                                    DisplayAlert("Location Error", "GPS is currently turn off. Please enable it under your mobile Settings > Location", "OK");
                                }
                                i = i + 1;
                                if (i >= 15)
                                {
                                    locationErrorText.Text =!checkGpsEnabled&&status==PermissionStatus.Granted? "Please allow the app to access your location from your mobile settings akram > Location > . Or from the side menu Setting > Location ":checkGpsEnabled&&status!=PermissionStatus.Granted? "You need to give Akram app access to your location to start finding gifts" : "Please allow the app to access your location from your mobile settings akram > Location > . Or from the side menu Setting > Location ";
                                    NoLocationContentView.IsVisible = true;
                                    MainView.IsVisible = false;
                                }
                            }
                            else
                            {
                                locationErrorText.Text = !checkGpsEnabled && status == PermissionStatus.Granted ? "Please allow the app to access your location from your mobile settings akram > Location > . Or from the side menu Setting > Location " : checkGpsEnabled && status != PermissionStatus.Granted ? "You need to give Akram app access to your location to start finding gifts" : "Please allow the app to access your location from your mobile settings akram > Location > . Or from the side menu Setting > Location ";

                                NoLocationContentView.IsVisible = true;
                                MainView.IsVisible = false;
                            }
                        }
                        else
                        {
                            IsPopUpDisplayed = false;
                            if (position == null && !IsStarted)
                            {
                                IsStarted = true;
                                OnStart();
                            }

                            NoLocationContentView.IsVisible = false;
                            MainView.IsVisible = true;
                        }
                    });
                });
                return true;
            });

            MessagingCenter.Subscribe<Application, bool>(this, "NoCollectionPopup", (sender, message) =>
            {
                if (!IsPageOpened)
                {
                    NoCollectionPopup _popUp = new NoCollectionPopup();
                    _popUp.Disappearing += _popUp_Disappearing;
                    Application.Current.MainPage.Navigation.PushPopupAsync(_popUp);
                }
            });

            MessagingCenter.Subscribe<Application, bool>(this, "NoAccountPopup", (sender, message) =>
            {
                if (!IsAccountPageOpened)
                {
                    NoAccountPopUp _noAccountPopup = new NoAccountPopUp();
                    _noAccountPopup.Disappearing += _noAccountPopup_Disappearing;
                    Application.Current.MainPage.Navigation.PushPopupAsync(_noAccountPopup);
                }
            });
        }

        public static double CalculateDistance(double lat, double lng, double lat1, double lng1)
        {
            double pk = (double)(180 / Math.PI);

            double a1 = lat / pk;
            double a2 = lng / pk;
            double b1 = lat1 / pk;
            double b2 = lng1 / pk;

            double t1 = Math.Cos(a1) * Math.Cos(a2) * Math.Cos(b1) * Math.Cos(b2);
            double t2 = Math.Cos(a1) * Math.Sin(a2) * Math.Cos(b1) * Math.Sin(b2);
            double t3 = Math.Sin(a1) * Math.Sin(b1);
            double tt = Math.Acos(t1 + t2 + t3);

            return 6366000 * tt;
        }

        protected override bool OnBackButtonPressed()
        {
            if (Device.RuntimePlatform == Device.Android)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    var result = await DisplayAlert("", "Do you really want to exit the application?", "Yes", "No");
                    if (result)
                    {

                        DependencyService.Get<IAndroidMethods>().CloseApp();

                    }
                });
            }
            return true;
        }

        private void _noAccountPopup_Disappearing(object sender, EventArgs e)
        {
            IsAccountPageOpened = false;
        }

        private void _popUp_Disappearing(object sender, EventArgs e)
        {
            IsPageOpened = false;
        }

        void Handle_Clicked(object sender, System.EventArgs e)
        {
            if (!checkGpsEnabled && status == PermissionStatus.Granted)
            {
                DependencyService.Get<ILocSettings>().OpenSettings();
            }
            else if(checkGpsEnabled&&status!=PermissionStatus.Granted)
            {
                DependencyService.Get<IStoragePermissions>().GetLocationPermissions();
            }
            else if(!checkGpsEnabled&&status!=PermissionStatus.Granted)
            {
                DependencyService.Get<ILocSettings>().OpenSettings();
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<SendPosition>(this, "PositionChanged");
            MessagingCenter.Unsubscribe<Application>(this, "LoadGifts");
            MessagingCenter.Unsubscribe<Application>(this, "RefreshGifts");
            MessagingCenter.Unsubscribe<Application>(this, "DrawCircleFirstTime");
            MessagingCenter.Unsubscribe<Application>(this, "RadiusUpdate");
            MessagingCenter.Unsubscribe<Application>(this, "RadiusUpdateAgain");
        }

        public void OnStart()
        {
            try
            {
                try
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await Navigation.PushPopupAsync(new LoaderPopup());

                        MyMap.HeightRequest = App.ScreenHeight - 80;

                        position = await CommonLib.GetLatLong();

                        if (position != null)
                        {
                            MyMap.Circle = new CustomCircle
                            {
                                Position = position
                            };

                            try
                            {
                                MessagingCenter.Send(Application.Current, "DrawCircleFirstTime", true);
                            }
                            catch (Exception ex)
                            {

                            }

                            if (Device.RuntimePlatform == Device.iOS)
                            {
                                var start_pin_custom = new CustomPin
                                {
                                    Pin = new Pin
                                    {
                                        Type = PinType.Place,
                                        Position = new Position(Convert.ToDouble(position.Latitude), Convert.ToDouble(position.Longitude)),
                                        Label = "",
                                        Address = string.IsNullOrEmpty(LoginUserDetails.FacebookProfilePicture) ? "Current Location" : "User Current Location",

                                    },
                                    Id = string.IsNullOrEmpty(LoginUserDetails.FacebookProfilePicture) ? "Current Location" : "User Current Location",
                                    startPin = true,
                                    Url = ""
                                };

                                MyMap.CustomPins = new List<CustomPin> { start_pin_custom };

                                MyMap.Pins.Add(start_pin_custom.Pin);

                            }
                            else
                            {
                                var start_pin = new Pin
                                {
                                    Type = PinType.Place,
                                    Position = new Position(Convert.ToDouble(position.Latitude), Convert.ToDouble(position.Longitude)),
                                    Label = "",
                                    Address = string.IsNullOrEmpty(LoginUserDetails.FacebookProfilePicture) ? "Current Location" : "User Current Location",
                                };

                                MyMap.Pins.Add(start_pin);
                            }

                            MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(
                         new Position(position.Latitude, position.Longitude), Distance.FromMeters(50)));

                            LoaderPopup.CloseAllPopup();

                            Device.StartTimer(TimeSpan.FromSeconds(0.5), () =>
                            {
                                Task.Factory.StartNew(async () =>
                                {
                                    Plugin.Geolocator.Abstractions.Position getNewPosition = await CommonLib.GetLatLong();
                                    if (position != null && getNewPosition != null)
                                    {
                                        if ((Math.Round((Double)getNewPosition.Latitude, 4) != Math.Round((Double)position.Latitude, 4))
                                        && (Math.Round((Double)getNewPosition.Longitude, 4) != Math.Round((Double)position.Longitude, 4)))
                                        {
                                            try
                                            {
                                                position.Latitude = getNewPosition.Latitude;
                                                position.Longitude = getNewPosition.Longitude;
                                                MessagingCenter.Send(new SendPosition { position = getNewPosition }, "PositionChanged");
                                            }
                                            catch (Exception)
                                            {

                                            }
                                        }
                                    }
                                });
                                return true;
                            });

                            if (LoginUserDetails.IsLoggedIn)
                            {
                                DependencyService.Get<IFirebaseDatabase>().MakeConnection();
                            }
                            else
                            {
                                await ShowGifts(position.Latitude, position.Longitude, 1.0);
                            }
                        }
                        else
                        {
                            return;
                        }
                    });
                }
                catch (Exception ex)
                {

                }


            }
            catch (Exception ex)
            {
                LoaderPopup.CloseAllPopup();
            }

        }

        public void timerTime()
        {
            try
            {
                if (sec == 0)
                {
                    sec = 59;
                    min = min - 1;
                    minTime = (min).ToString();
                    secTime = (sec).ToString();
                }
                else
                {
                    sec = sec - 1;
                    secTime = (sec).ToString();
                }
            }
            catch
            {

            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            notificationImg.IsEnabled = true;
            if (IsRefresh)
            {
                ShowNewGifts(position.Latitude, position.Longitude, 1.0);
                IsRefresh = false;
            }
        }

        public async Task ShowNewGifts(double lat, double lon, double radius)
        {
            try
            {

                if (!CommonLib.CheckConnection())
                {
                    await Navigation.PushPopupAsync(new NoInternetPopup());
                    return;
                }

                HttpClientBase cbase = new HttpClientBase();

                string url = string.Empty;

                if (LoginUserDetails.IsLoggedIn)
                    url = ApiUrl.GetHomePageGiftsUrl + "?api_key=" + CommonLib.apiKey + "&lat=" + lat + "&lon=" + lon + "&radius=" + radius + "&userid=" + LoginUserDetails.UserId;
                else
                    url = ApiUrl.GetHomePageGiftsUrl + "?api_key=" + CommonLib.apiKey + "&lat=" + lat + "&lon=" + lon + "&radius=" + radius;

                result = await cbase.GetHomePageGifts(url);

                if (result != null)
                {
                    if (Device.RuntimePlatform == Device.iOS)
                    {
                        var getExistingPinss = MyMap.CustomPins.Where(a => !string.IsNullOrEmpty(a.Pin.Label)).ToList();
                        if (getExistingPinss.Count > 0)
                        {
                            for (int i = 0; i < getExistingPinss.Count; i++)
                            {
                                MyMap.CustomPins.Remove(getExistingPinss[i]);
                                MyMap.Pins.Remove(getExistingPinss[i].Pin);
                            }
                        }
                    }
                    else
                    {
                        var getExistingPins = MyMap.Pins.Where(a => !string.IsNullOrEmpty(a.Label)).ToList();
                        if (getExistingPins.Count > 0)
                        {
                            for (int i = 0; i < getExistingPins.Count; i++)
                            {
                                MyMap.Pins.Remove(getExistingPins[i]);
                            }
                        }
                    }

                    if (result.sightings != null)
                    {
                        string ColorOfGift = string.Empty;
                        foreach (var item in result.sightings)
                        {
                            string getIdArray = string.Join(",", AllItemIds);
                            string[] splitAllStrings = getIdArray.Split(',');
                            if (splitAllStrings.Count() > 0)
                            {
                                for (int i = 0; i < splitAllStrings.Count(); i++)
                                {
                                    if (splitAllStrings[i] == item.user_id)
                                    {
                                        ColorOfGift = "Gray";
                                        goto there;
                                    }
                                    else
                                    {
                                        ColorOfGift = "Green";
                                    }
                                }
                            }
                            else
                            {
                                ColorOfGift = "Green";
                            }


                        there: var calculateDistance = CalculateDistance(lat, lon, Convert.ToDouble(item.lat), Convert.ToDouble(item.lon));
                            if (calculateDistance <= _Radius)
                            {
                                if (Device.RuntimePlatform == Device.iOS)
                                {
                                    var new_pin_custom = new CustomPin
                                    {
                                        Pin = new Pin
                                        {
                                            Type = PinType.Place,
                                            Position = new Position(Convert.ToDouble(item.lat), Convert.ToDouble(item.lon)),
                                            Label = item.sighting_id,
                                            Address = ColorOfGift
                                        },
                                        Id = ColorOfGift,
                                        Url = item.sighting_id
                                    };

                                    MyMap.CustomPins = new List<CustomPin> { new_pin_custom };

                                    MyMap.Pins.Add(new_pin_custom.Pin);
                                }
                                else
                                {
                                    var new_pin = new Pin
                                    {
                                        Type = PinType.Place,
                                        Position = new Position(Convert.ToDouble(item.lat), Convert.ToDouble(item.lon)),
                                        Label = item.sighting_id,
                                        Address = ColorOfGift
                                    };
                                    MyMap.Pins.Add(new_pin);
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                LoaderPopup.CloseAllPopup();
            }
        }

        public async Task ShowGifts(double lat, double lon, double radius)
        {
            try
            {

                if (!CommonLib.CheckConnection())
                {
                    await Navigation.PushPopupAsync(new NoInternetPopup());
                    return;
                }

                HttpClientBase cbase = new HttpClientBase();

                string url = string.Empty;

                if (LoginUserDetails.IsLoggedIn)
                    url = ApiUrl.GetHomePageGiftsUrl + "?api_key=" + CommonLib.apiKey + "&lat=" + lat + "&lon=" + lon + "&radius=" + radius + "&userid=" + LoginUserDetails.UserId;
                else
                    url = ApiUrl.GetHomePageGiftsUrl + "?api_key=" + CommonLib.apiKey + "&lat=" + lat + "&lon=" + lon + "&radius=" + radius;

                result = await cbase.GetHomePageGifts(url);

                if (result != null)
               {
                    if (result.sightings != null)
                    {
                        string ColorOfGift = string.Empty;
                        foreach (var item in result.sightings)
                        {
                            string getIdArray = string.Join(",", AllItemIds);
                            string[] splitAllStrings = getIdArray.Split(',');
                            if (splitAllStrings.Count() > 0)
                            {
                                for (int i = 0; i < splitAllStrings.Count(); i++)
                                {
                                    if (splitAllStrings[i] == item.user_id)
                                    {
                                        ColorOfGift = "Gray";
                                        goto there;
                                    }
                                    else
                                    {
                                        ColorOfGift = "Green";
                                    }
                                }
                            }
                            else
                            {
                                ColorOfGift = "Green";
                            }
                            _Radius = 20;
                        there: var calculateDistance = CalculateDistance(lat, lon, Convert.ToDouble(item.lat), Convert.ToDouble(item.lon));
                            if (calculateDistance <= _Radius)
                            {
                                if (Device.RuntimePlatform == Device.iOS)
                                {
                                    var new_pin_custom = new CustomPin
                                    {
                                        Pin = new Pin
                                        {
                                            Type = PinType.Place,
                                            Position = new Position(Convert.ToDouble(item.lat), Convert.ToDouble(item.lon)),
                                            Label = item.sighting_id,
                                            Address = ColorOfGift
                                        },
                                        Id = ColorOfGift,
                                        Url = item.sighting_id
                                    };

                                    MyMap.CustomPins = new List<CustomPin> { new_pin_custom };

                                    MyMap.Pins.Add(new_pin_custom.Pin);
                                }
                                else
                                {
                                    var new_pin = new Pin
                                    {
                                        Type = PinType.Place,
                                        Position = new Position(Convert.ToDouble(item.lat), Convert.ToDouble(item.lon)),
                                        Label = item.sighting_id,
                                        Address = ColorOfGift
                                    };
                                    MyMap.Pins.Add(new_pin);
                                }
                            }
                        }
                    }
                }

                //LoaderPopup.CloseAllPopup();

                TimeSpan time = new TimeSpan(0, 4, 59);
                CancellationTokenSource cts = new CancellationTokenSource();
                cts.CancelAfter(time);

                Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                {
                    timerTime();
                    if (cts.IsCancellationRequested)
                    {
                        _Radius = 30;
                        MessagingCenter.Send(Application.Current, "RadiusUpdate", true);
                        advanceLbl.IsVisible = true;
                        SetTimerAgain();
                        return false;
                    }
                    else
                    {
                        timeLabel.Text = minTime + " min, " + secTime + " sec " + "To advance mode";
                        return true;
                    }
                });

            }
            catch (Exception ex)
            {
                //LoaderPopup.CloseAllPopup();
            }
        }

        public void SetTimerAgain()
        {
            minTime = "0";
            secTime = "0";
            min = 05;
            sec = 0;
            TimeSpan time = new TimeSpan(0, 4, 59);
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(time);
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                timerTime();
                if (cts.IsCancellationRequested)
                {
                    _Radius = 40;
                    MessagingCenter.Send(Application.Current, "RadiusUpdateAgain", true);
                    advanceLbl.IsVisible = false;
                    extremeLbl.IsVisible = true;
                    timeLabel.Text = "Max Stage Reached";
                    return false;
                }
                else
                {
                    timeLabel.Text = minTime + " min, " + secTime + " sec " + "To Extreme Mode";
                    return true;
                }
            });
        }


        void Menu_Tapped(object sender, EventArgs e)
        {
            HomeMasterPage._masterPage.IsPresented = !HomeMasterPage._masterPage.IsPresented;
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            var getImage = sender as Image;
            if (getImage != null)
            {
                getImage.IsEnabled = false;
            }
            Navigation.PushAsync(new NotificationPage());
        }
    }
}