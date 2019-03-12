using Akram.DependencyInterface;
using Akram.Models;
using Akram.Repository;
using Akram.ViewModels;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing;
using ZXing.Net.Mobile.Forms;
using ZXing.QrCode;

namespace Akram.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GiftPage : ContentPage
    {
        HttpClientBase cBase = new HttpClientBase();
        ZXingBarcodeImageView barcode;
        public bool IsPopOpened = false;
        string phoneNumber = string.Empty;
        string instaLink = string.Empty;
        string facebookLink = string.Empty;
        public static bool IsScanned = false;
        public static string ItemId = string.Empty;

        public GiftPage(MyCollectionModel SelectedItem)
        {
            try
            {
                InitializeComponent();

                ItemId = SelectedItem.ItemId;

                NavigationPage.SetHasNavigationBar(this, false);

                if (Device.RuntimePlatform == Device.iOS)
                {
                    firstGrid.Margin = new Thickness(0, 30, 0, 0);
                }
                if (!string.IsNullOrEmpty(SelectedItem.Rules))
                {
                    string[] splitString = SelectedItem.Rules.Split(',');
                    for (int i = 0; i < splitString.Count(); i++)
                    {
                        Label _ruleLabel = new Label();
                        _ruleLabel.Text = i + 1 + ". " + CommonLib.FirstCharToUpper(splitString[i]);
                        _ruleLabel.HorizontalOptions = LayoutOptions.Start;
                        _ruleLabel.FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label));
                        _ruleLabel.TextColor = Color.FromHex("#5A6E66");
                        _ruleLabel.HorizontalTextAlignment = TextAlignment.Start;
                        _ruleLabel.Margin = new Thickness(20, 0, 0, 0);
                        _ruleLabel.FontFamily = "MYRIADPROBOLD";
                        _ruleLabel.StyleId = "MYRIADPROBOLD";

                        rulesLayout.Children.Add(_ruleLabel);
                    }
                }
                else
                {
                    Label _ruleLabel = new Label();
                    _ruleLabel.Text = "No Rules Found";
                    _ruleLabel.HorizontalOptions = LayoutOptions.Center;
                    _ruleLabel.FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label));
                    _ruleLabel.TextColor = Color.FromHex("#5A6E66");
                    _ruleLabel.HorizontalTextAlignment = TextAlignment.Center;
                    _ruleLabel.Margin = new Thickness(20, 0, 0, 0);
                    _ruleLabel.FontFamily = "MYRIADPROBOLD";
                    _ruleLabel.StyleId = "MYRIADPROBOLD";

                    rulesLayout.Children.Add(_ruleLabel);
                }

                barcode = new ZXingBarcodeImageView
                {
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    VerticalOptions = LayoutOptions.StartAndExpand,
                    HeightRequest = 270,
                    WidthRequest = 270,
                    BarcodeOptions = new QrCodeEncodingOptions
                    {
                        Height = 270,
                        Width = 270
                    },
                    Aspect = Aspect.AspectFill
                };
                barcode.BarcodeFormat = ZXing.BarcodeFormat.QR_CODE;
                barcode.BarcodeValue = "id(" + SelectedItem.SightingId + ")," + "name(" + SelectedItem.Item + ")," + "type,dis,loc,user_id(" + LoginUserDetails.UserId + "),2";
                mainLayout.Children.Insert(0, barcode);

                if (Convert.ToInt32(SelectedItem.ItemId) > 0)
                {
                    GetData(SelectedItem.ItemId);
                }
                else
                {
                    iconsLayout.IsVisible = false;
                }
                if (SelectedItem.Scan == "1")
                {
                    scanCodeTxt.Text = "Scanned, Waiting For Redeem";
                }
                else
                {
                    scanCodeTxt.Text = "Waiting To Read";
                }
            }
            catch (Exception ex)
            {

            }

            Device.StartTimer(TimeSpan.FromSeconds(2), () =>
            {
                if (!IsScanned)
                {
                    DependencyService.Get<IFirebaseDatabase>().CheckGiftForScan();
                    return true;
                }
                else
                {
                    return false;
                }

            });

            MessagingCenter.Subscribe<SendPosition>(this, "ScanChanged", (sender) =>
            {
                if (sender.pokeman_id == "1")
                {
                    IsScanned = true;

                    DependencyService.Get<IFirebaseDatabase>().DeleteCollection(SelectedItem.SightingId, ItemId, ItemId);

                    UpdateAfterRedeemed();
                }
                MessagingCenter.Unsubscribe<SendPosition>(this, "ScanChanged");
            });
        }

        /// <summary>
        /// Show popup and close popup after redeemed.
        /// </summary>
        public async void UpdateAfterRedeemed()
        {
            await Application.Current.MainPage.Navigation.PushPopupAsync(new RedeemGiftPopup("Reedemed"));

            await Task.Delay(1000);

            await Application.Current.MainPage.Navigation.PopAllPopupAsync();

            await Application.Current.MainPage.Navigation.PopAsync();
        }

        public async void GetData(string itemId)
        {
            try
            {
                if (!CommonLib.CheckConnection())
                {

                }
                else
                {
                    await Navigation.PushPopupAsync(new LoaderPopup());
                    string postData = "api_key=" + CommonLib.apiKey + "&userid=" + itemId;
                    var result = await cBase.GetProfileData(ApiUrl.ViewProfile + postData);

                    if (result != null)
                    {
                        phoneNumber = result.phone;
                        instaLink = result.insta_profile;
                        facebookLink = result.facebook_profile;
                        phoneImg.IsVisible = !string.IsNullOrEmpty(result.phone) ? true : false;
                        instaImg.IsVisible = !string.IsNullOrEmpty(result.insta_profile) ? true : false;
                        fbImg.IsVisible = !string.IsNullOrEmpty(result.facebook_profile) ? true : false;

                    }
                    else
                    {
                        iconsLayout.IsVisible = false;
                    }
                    LoaderPopup.CloseAllPopup();
                }
            }
            catch
            {
                LoaderPopup.CloseAllPopup();
            }
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        /// <summary>
        /// phone tapped
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Phone_Tapped(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri(String.Format("tel:{0}", phoneNumber)));
        }

        /// <summary>
        /// Insta tapped
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Insta_Tapped(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(instaLink))
                {
                    Device.OpenUri(new Uri(instaLink));
                }
            }
            catch (Exception)
            {
                DisplayAlert("", "Link not valid", "Ok");
            }
        }

        /// <summary>
        /// Facebook tapped
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Facebook_Tapped(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(facebookLink))
                {
                    Device.OpenUri(new Uri(facebookLink));
                }
            }
            catch (Exception)
            {
                DisplayAlert("", "Link not valid", "Ok");
            }
        }

        protected override bool OnBackButtonPressed()
        {
            if (IsPopOpened)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private async void Redeem_Button_Click(object s, EventArgs e)
        {
            await Navigation.PushPopupAsync(new LoaderPopup());

            DependencyService.Get<IFirebaseDatabase>().GetLocations();

            MessagingCenter.Subscribe<Application, bool>(this, "LoadLocationPopup", (sender, message) =>
            {
                LoaderPopup.CloseAllPopup();
                var getLoactions = App.Database.GetLocations();
                if (getLoactions.Count > 0)
                {
                    ShowLocationPopup showLocationPopup = new ShowLocationPopup();
                    IsPopOpened = true;
                    showLocationPopup.Disappearing += OnDisappearing;
                    Navigation.PushPopupAsync(showLocationPopup);
                }
                else
                {
                    DisplayAlert("", "No Locations Found", "Ok");
                }
            });
        }

        void OnDisappearing(object sender, EventArgs e)
        {
            IsPopOpened = false;
        }
    }
}