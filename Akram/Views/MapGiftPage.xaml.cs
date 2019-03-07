using Akram.DependencyInterface;
using Akram.Models;
using Akram.Repository;
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
    public partial class MapGiftPage : ContentPage
    {
        private string pokemonId = string.Empty;
        public static string hsa = string.Empty;
        HttpClientBase cbase = new HttpClientBase();
        wsGiftsResponse obj = null;
        public string sightingId = string.Empty;
        public bool isStopTimer = false;
        public static List<string> AllItemList = new List<string>();
        public static List<string> listItemsTaken = new List<string>();

        public MapGiftPage(wsGiftsResponse giftsResponse, string id)
        {
            isStopTimer = false;
            InitializeComponent();

            HomeMapPage.IsGiftPageOpened = true;

            if (Device.RuntimePlatform == Device.iOS)
            {
                firstGrid.Margin = new Thickness(0, 30, 0, 0);
                doneBtn.WidthRequest = 100;
            }

            sightingId = id;

            DependencyService.Get<IFirebaseDatabase>().GiftDetailsCheckTaken();

            MessagingCenter.Subscribe<Application, bool>(this, "GiftsTakenDetails", (sender, message) =>
            {
                string getTakenIdArray = string.Join(",", AllItemList);
                string[] splitStrings = getTakenIdArray.Split(',');

                if (splitStrings.Count() > 0)
                {
                    foreach (var item in giftsResponse.sightings)
                    {
                        var getSightingIdMatch = splitStrings.Where(A => A == item.user_id).FirstOrDefault();
                        collectBtn.IsVisible = !string.IsNullOrEmpty(getSightingIdMatch) ? false : true;
                        if (!collectBtn.IsVisible)
                        {
                            LoaderPopup.CloseAllPopup();
                            DisplayAlert("", "You have gift from this merchant", "OK");
                            Navigation.PopAsync();
                        }
                        else
                        {
                            Device.StartTimer(TimeSpan.FromSeconds(30), () =>
                            {
                                LoaderPopup.CloseAllPopup();
                                if (!isStopTimer)
                                {
                                    DependencyService.Get<IFirebaseDatabase>().CheckGiftTaken();
                                    return true;
                                }
                                else
                                {

                                    return false;
                                }
                            });
                        }
                    }
                }
                else
                {
                    LoaderPopup.CloseAllPopup();
                    collectBtn.IsVisible = true;                    
                }
            });

            

            MessagingCenter.Subscribe<Application, bool>(this, "CheckTakenGifts", (sender, message) =>
             {
                 string getIdArray = string.Join(",", listItemsTaken);
                 string[] splitAllStrings = getIdArray.Split(',');
                 if (splitAllStrings.Count() > 0)
                 {
                     var getSightingIdMatch = splitAllStrings.Where(A => A == sightingId).FirstOrDefault();
                     collectBtn.IsVisible = !string.IsNullOrEmpty(getSightingIdMatch) ? false : true;
                     if (!collectBtn.IsVisible)
                     {
                         DisplayAlert("", "The item is already taken", "OK");
                         isStopTimer = true;
                         Navigation.PopAsync();
                     }
                 }
                 else
                 {
                     collectBtn.IsVisible = true;
                 }
                 MessagingCenter.Unsubscribe<Application>(this, "CheckTakenGifts");
             });


            if (Device.RuntimePlatform == Device.iOS)
            {
                firstGrid.Margin = new Thickness(0, 10, 0, 0);
            }
            obj = giftsResponse;
            if (giftsResponse.sightings.Count > 0)
            {
                foreach (var item in giftsResponse.sightings)
                {
                    if (item.sighting_id == id)
                    {
                        shopNameLbl.Text = item.name;
                        pokemonId = item.pokemon_id;

                        for (int i = 0; i < item.rules.Count; i++)
                        {
                            Label _ruleLabel = new Label();
                            _ruleLabel.Text = i + 1 + ". " + item.rules[i];
                            _ruleLabel.HorizontalOptions = LayoutOptions.Start;
                            _ruleLabel.FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label));
                            _ruleLabel.TextColor = Color.FromHex("##5A6E66");
                            _ruleLabel.HorizontalTextAlignment = TextAlignment.Start;
                            _ruleLabel.Margin = new Thickness(20, 0, 0, 0);
                            _ruleLabel.FontFamily = "MYRIADPROBOLD";
                            _ruleLabel.StyleId = "MYRIADPROBOLD";

                            rulesLayout.Children.Add(_ruleLabel);
                        }

                    }
                }

            }
            NavigationPage.SetHasNavigationBar(this, false);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await Xamarin.Forms.Application.Current.MainPage.Navigation.PushPopupAsync(new LoaderPopup());
            MessagingCenter.Unsubscribe<SendPosition>(this, "GiftPage");
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            isStopTimer = true;
            HomeMapPage.IsGiftPageOpened = false;
        }

        void Handle_Tapped(object sender, System.EventArgs e)
        {
            Navigation.PopAsync();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushPopupAsync(new LoaderPopup());
                CollectModel collectModel = new CollectModel();
                foreach (var item in obj.sightings)
                {
                    if (item.sighting_id == sightingId)
                    {
                        var rules = string.Join(",", item.rules);
                        collectModel.Distance = item.distance;
                        collectModel.item_id = item.user_id;
                        collectModel.scan = "0";
                        collectModel.expiry = DateTime.Now.AddDays(7).ToString("yyyy-MM-dd");
                        collectModel.Loc = item.lon + "/" + item.lat;
                        collectModel.Name = item.name;
                        collectModel.Sighting_id = item.sighting_id;
                        collectModel.Type = item.type;
                        collectModel.userId = LoginUserDetails.UserId;
                        collectModel.rules = rules;

                        SaveNotificationDetails _notificationDetails = new SaveNotificationDetails();
                        _notificationDetails.Title = "Gift Collected " + item.name.Trim();
                        App.Database.SaveNotifications(_notificationDetails);
                    }
                }

                DependencyService.Get<IFirebaseDatabase>().Collect(collectModel);

                LoaderPopup.CloseAllPopup();

                await Xamarin.Forms.Application.Current.MainPage.Navigation.PushPopupAsync(new RedeemGiftPopup("Collected"));

                await Task.Delay(1000);

                await Xamarin.Forms.Application.Current.MainPage.Navigation.PopAllPopupAsync();

                await DeleteGift();
            }
            catch (Exception)
            {
                LoaderPopup.CloseAllPopup();
            }
        }

        private void rating_ValueChanged(object sender, Syncfusion.SfRating.XForms.ValueEventArgs e)
        {
            var getRating = e.Rating;
            if (e.Value >= 1)
                doneBtn.IsVisible = true;
            else
                doneBtn.IsVisible = false;
        }

        private async void doneBtn_Clicked(object sender, EventArgs e)
        {
            if (rating.Value >= 1)
                await RateGift();
        }


        public async Task DeleteGift()
        {
            try
            {
                await Navigation.PushPopupAsync(new LoaderPopup());
                var nvc = new List<KeyValuePair<string, string>>();
                nvc.Add(new KeyValuePair<string, string>("sighting_id", sightingId));
                nvc.Add(new KeyValuePair<string, string>("user_id", LoginUserDetails.UserId));
                nvc.Add(new KeyValuePair<string, string>("api_key", CommonLib.apiKey));
                nvc.Add(new KeyValuePair<string, string>("login_hash", LoginUserDetails.LoginHash));
                //nvc.Add(new KeyValuePair<string, string>("lon", collectModel.Loc));
                nvc.Add(new KeyValuePair<string, string>("is_deleted", "1"));

                var result = await cbase.ForgotPassword(ApiUrl.DeleteGiftUrl, nvc);

                if (result.status.status_code == "-1")
                {
                    LoaderPopup.CloseAllPopup();
                    HomeMapPage.IsRefresh = true;
                    await Xamarin.Forms.Application.Current.MainPage.Navigation.PopAsync();
                }
            }
            catch (Exception)
            {
                LoaderPopup.CloseAllPopup();
            }
        }

        public async Task RateGift()
        {
            try
            {
                await Navigation.PushPopupAsync(new LoaderPopup());

                var nvc = new List<KeyValuePair<string, string>>();
                nvc.Add(new KeyValuePair<string, string>("pokmon_id", pokemonId));
                nvc.Add(new KeyValuePair<string, string>("rating", rating.Value.ToString()));
                nvc.Add(new KeyValuePair<string, string>("api_key", CommonLib.apiKey));
                var result = await cbase.RateGift(ApiUrl.RateGiftUrl, nvc);

                if (result != null)
                {
                    if (result.status.status_code == "-1")
                    {
                        await App.Current.MainPage.DisplayAlert("", "Gift Rated Successfully", "OK");
                        doneBtn.IsVisible = false;
                        rating.Value = 0;
                        ratingGiftFrame.IsVisible = false;
                        LoaderPopup.CloseAllPopup();
                    }
                    else
                    {
                        LoaderPopup.CloseAllPopup();
                        await App.Current.MainPage.DisplayAlert("", result.status.status_text, "OK");
                    }
                }
                else
                {
                    LoaderPopup.CloseAllPopup();
                    await App.Current.MainPage.DisplayAlert("", CommonLib.someErrorMsg, "OK");
                }
            }
            catch
            {
                LoaderPopup.CloseAllPopup();
            }
        }
    }
}