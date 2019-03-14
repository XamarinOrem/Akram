using System;
using System.Collections.Generic;
using Akram.Database;
using Akram.DependencyInterface;
using Akram.Models;
using Akram.Views;
using Plugin.FirebasePushNotification;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Akram
{
    public partial class App : Application
    {
        public static string DeviceToken = string.Empty;

        public static double ScreenWidth;
        public static double ScreenHeight;
        SaveLoginResponse getLoginUser = App.Database.GetLoginUser();
        public App()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NjQxN0AzMTM2MmUzMjJlMzBPKzU1STRkNDA1Y29rWlhzSnQ4bHl2N0FJLzNFWFFnaFcxamswdHQ1cHFFPQ==");

            InitializeComponent();

            var navigationStyle = new Style(typeof(NavigationPage));

            var barTextColorSetter = new Setter { Property = NavigationPage.BarTextColorProperty, Value = Color.White };
            var barBackgroundColorSetter = new Setter { Property = NavigationPage.BarBackgroundColorProperty, Value = Color.FromHex("#10A24A") };

            navigationStyle.Setters.Add(barTextColorSetter);
            navigationStyle.Setters.Add(barBackgroundColorSetter);

            Current.Resources.Add(navigationStyle);

            GetMainPage();
        }


        public void GetMainPage()
        {
            if (!string.IsNullOrEmpty(getLoginUser.UserId))
            {
                LoginUserDetails.UserId = getLoginUser.UserId;
                LoginUserDetails.LoginHash = getLoginUser.LoginHash;
                LoginUserDetails.UserName = getLoginUser.UserName;
                LoginUserDetails.IsLoggedInFacebook = getLoginUser.IsLoggedInFacebook;
                LoginUserDetails.Password = getLoginUser.Password;
                LoginUserDetails.IsLoggedIn = getLoginUser.IsLoggedIn;
                LoginUserDetails.Email = getLoginUser.Email;
                LoginUserDetails.FullName = getLoginUser.FullName;
                LoginUserDetails.FacebookProfilePicture = getLoginUser.FacebookProfilePicture;
                MainPage = new NavigationPage(new HomeMasterPage());
            }
            else
            {
                MainPage = new NavigationPage(new LoginPage());
            }
        }

        private static AkramLocalDatabase _database;

        /// <summary>
        /// Getting the database path to be used statically in the entire application.
        /// </summary>
        public static AkramLocalDatabase Database
        {
            get
            {
                if (_database == null)
                {
                    try
                    {
                        // initialization of the database.
                        _database = new AkramLocalDatabase(DependencyService.Get<IFileHelper>().GetLocalFilePath("DBAkram.db3"));
                    }
                    catch (Exception) { }
                    finally { }
                }
                return _database;
            }
        }

        protected override void OnStart()
        {
            try
            {
                CrossFirebasePushNotification.Current.Subscribe("general");

                CrossFirebasePushNotification.Current.OnTokenRefresh += (s, p) =>
                {
                    DeviceToken = p.Token;
                };

                var getDeviceToken = CrossFirebasePushNotification.Current.Token;

                DeviceToken = getDeviceToken;


                CrossFirebasePushNotification.Current.OnNotificationReceived += (s, p) =>
                {
                    try
                    {
                       
                        if (p.Data != null)
                        {
                            SaveNotificationDetails _notificationDetails = new SaveNotificationDetails();
                            foreach (var item in p.Data)
                            {
                                if (item.Key=="title")
                                {
                                    _notificationDetails.Title = item.Value.ToString();
                                    App.Database.SaveNotifications(_notificationDetails);
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {

                    }
                };

                CrossFirebasePushNotification.Current.OnNotificationOpened += (s, p) =>
                {
                    foreach (var data in p.Data)
                    {
                        if ($"{data.Key}" == "message")
                        {
                            App.Current.MainPage = new NavigationPage(new HomeMasterPage());
                        }

                    }
                };
            }
            catch (Exception)
            {

            }
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
