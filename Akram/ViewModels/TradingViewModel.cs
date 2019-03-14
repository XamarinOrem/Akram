using Akram.DependencyInterface;
using Akram.Models;
using Akram.Views;
using Plugin.Media;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;
using ZXing;
using ZXing.Net.Mobile.Forms;

namespace Akram.ViewModels
{
    public class TradingViewModel : INotifyPropertyChanged
    {
        public static INavigation _navigation;

        public static CollectModel collectModel = new CollectModel();

        public static string ItemId = string.Empty;

        ZXingScannerPage scanPage = new ZXingScannerPage();

        public static ZXing.Result _Result = null;

        public event PropertyChangedEventHandler PropertyChanged;

        public TradingViewModel(INavigation navigation)
        {
            _navigation = navigation;

            scanPage.Disappearing += ScanPage_Disappearing;

            scanPage.OnScanResult += (result) =>
            {
                // Stop scanning

                if (Device.RuntimePlatform == Device.Android)
                {
                    NavigationPage.SetHasNavigationBar(scanPage, false);
                }

                scanPage.IsScanning = false;

                _Result = result;

                TradingPage.CodeScanned = true;

                // Pop the page and show the result
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await _navigation.PopAsync();
                });
            };

            MessagingCenter.Subscribe<Application, bool>(this, "GiftExists", (sender, message) =>
            {
                MessagingCenter.Unsubscribe<Application>(this, "GiftExists");
                addData();
            });

            MessagingCenter.Subscribe<Application, bool>(this, "GiftNotExists", (sender, message) =>
            {
                MessagingCenter.Unsubscribe<Application>(this, "GiftNotExists");
                LoaderPopup.CloseAllPopup();
                Application.Current.MainPage.DisplayAlert("", "The other user don't have the gift", "OK");
            });

            MessagingCenter.Subscribe<Application, bool>(this, "CollectionScanDataFetched", (sender, message) =>
            {
                DependencyService.Get<IFirebaseDatabase>().Collect(collectModel);

                DependencyService.Get<IFirebaseDatabase>().DeleteCollection(collectModel.Key, ItemId, ItemId);

                Application.Current.MainPage.DisplayAlert("","Gift Traded Successfully","OK");

                LoaderPopup.CloseAllPopup();

                _navigation.PopAsync();
            });
        }

        private void ScanPage_Disappearing(object sender, EventArgs e)
        {
            scanPage.IsScanning = false;
        }

        public async static void ScanResult()
        {
            try
            {
                var getResult = _Result.Text.Split(',');
                if (getResult != null)
                {
                    //collectModel.Sighting_id = Regex.Match(getResult[0], @"\d+").Value;
                    //collectModel.item_id = Regex.Match(getResult[5], @"\d+").Value;
                    ItemId = getResult[6];
                    collectModel.Name = getResult[1];
                    collectModel.userId = LoginUserDetails.UserId;
                }

                await _navigation.PushPopupAsync(new LoaderPopup());
                DependencyService.Get<IFirebaseDatabase>().CheckGiftExists();
            }
            catch (Exception ex)
            {
                LoaderPopup.CloseAllPopup();
                await _navigation.PopAsync();

                await Application.Current.MainPage.DisplayAlert("", "QR code is not supported", "OK");
            }
        }

        public void addData()
        {
            DependencyService.Get<IFirebaseDatabase>().GetScennedCollectionData();
        }



        public Command StartBtnCommand
        {
            get
            {
                return new Command(async (e) =>
                {
                    try
                    {
                        await Task.Delay(1000);
                        // Navigate to our scanner page

                        if (Device.RuntimePlatform == Device.Android)
                        {
                            NavigationPage.SetHasNavigationBar(scanPage, false);
                        }
                        await _navigation.PushAsync(scanPage);

                    }
                    catch (Exception ex)
                    {

                    }
                });
            }
        }


    }
}
