using System;
using Akram.DependencyInterface;
using Akram.iOS.CustomRenderes;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using Plugin.FirebasePushNotification;
using Syncfusion.SfRating.XForms.iOS;
using UIKit;
using UserNotifications;

[assembly: Dependency(typeof(StoragePermissions_iOS))]
namespace Akram.iOS.CustomRenderes
{
    public class MyNotificationCenterDelegate : UNUserNotificationCenterDelegate
    {
        public override void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler)
        {
            completionHandler();
        }

        public override void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
        {
            completionHandler(UNNotificationPresentationOptions.Sound | UNNotificationPresentationOptions.Alert);

        }
    }

    public class StoragePermissions_iOS : IStoragePermissions
    {
        public void GetContactPermissions()
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                var authOptions = UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound;
                UNUserNotificationCenter.Current.RequestAuthorization(authOptions, (granted, error) => {
                    Console.WriteLine(granted);
                });
                UNUserNotificationCenter.Current.Delegate = new MyNotificationCenterDelegate();
            }
        }

        public void GetLocationPermissions()
        {
            
        }

        public void GetScannerPermissions()
        {
            
        }
    }
}
