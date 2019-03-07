using System;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Plugin.CurrentActivity;
using ImageCircle.Forms.Plugin.Droid;
using Plugin.Permissions;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Android.Widget;
using Android.Views;
using Android.Graphics;
using Xamarin.Forms;
using Android.Content;
using Plugin.FirebasePushNotification;
using Android;

namespace Akram.Droid
{
    [Activity(Label = "Akram", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = false,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public static MainActivity activity;
        [assembly: UsesFeature("android.hardware.camera", Required = false)]
        [assembly: UsesFeature("android.hardware.camera.autofocus", Required = false)]
        [assembly: UsesFeature("android.hardware.location", Required = false)]
        [assembly: UsesFeature("android.hardware.location.gps", Required = false)]
        [assembly: UsesFeature("android.hardware.location.network", Required = false)]

        const string permissionWL = Manifest.Permission.WakeLock;
        readonly string[] permissions ={Manifest.Permission.WakeLock};
        const int RequestLocationId = 0;

        protected override void OnCreate(Bundle bundle)
        {
            try
            {
                TabLayoutResource = Resource.Layout.Tabbar;
                ToolbarResource = Resource.Layout.Toolbar;

                activity = this;

                base.OnCreate(bundle);

                // fetch screen height and width
                var width = Resources.DisplayMetrics.WidthPixels;
                var height = Resources.DisplayMetrics.HeightPixels;
                var density = Resources.DisplayMetrics.Density;

                App.ScreenWidth = (width - 0.5f) / density;
                App.ScreenHeight = (height - 0.5f) / density;

                Xamarin.FormsMaps.Init(this, bundle);

                CrossCurrentActivity.Current.Init(this, bundle);
                Rg.Plugins.Popup.Popup.Init(this, bundle);

                ZXing.Net.Mobile.Forms.Android.Platform.Init();

                this.Window.SetFlags(WindowManagerFlags.KeepScreenOn, WindowManagerFlags.KeepScreenOn);

                global::Xamarin.Forms.Forms.Init(this, bundle);
                ImageCircleRenderer.Init();

                if ((int)Build.VERSION.SdkInt >= 23 && MainActivity.activity.CheckSelfPermission(permissionWL) != (int)Permission.Granted)
                {
                    MainActivity.activity.RequestPermissions(permissions, RequestLocationId);
                }

                //Set the default notification channel for your app when running Android Oreo
                if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
                {
                    //Change for your default notification channel id here
                    FirebasePushNotificationManager.DefaultNotificationChannelId = "DefaultChannel";

                    //Change for your default notification channel name here
                    FirebasePushNotificationManager.DefaultNotificationChannelName = "General";
                }

                LoadApplication(new App());

                FirebasePushNotificationManager.Initialize(this, true);

                FirebasePushNotificationManager.ProcessIntent(this, Intent);
            }
            catch (Exception)
            {

            }
        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            FirebasePushNotificationManager.ProcessIntent(this, intent);
        }
    }
}

