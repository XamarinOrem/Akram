using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Akram.DependencyInterface;
using Akram.iOS.DependencyInterface;
using CoreLocation;
using Foundation;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(Location_iOS))]
namespace Akram.iOS.DependencyInterface
{
    public class Location_iOS : ILocSettings
    {
        public bool CheckGpsEnable()
        {
            bool status = false;
            if (CLLocationManager.Status == CLAuthorizationStatus.Denied)
            {
                status = false;
            }
            else
            {
                status = true;
            }
            return status;
        }

        public void OpenSettings()
        {
            var url = new NSUrl("App-Prefs:root=LOCATION_SERVICES");

            if (UIApplication.SharedApplication.CanOpenUrl(url))
            {
                UIApplication.SharedApplication.OpenUrl(url);
            }
        }
    }
}