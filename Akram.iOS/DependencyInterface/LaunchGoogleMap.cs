using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Akram.DependencyInterface;
using Akram.iOS.DependencyInterface;
using Foundation;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(LaunchGoogleMap))]
namespace Akram.iOS.DependencyInterface
{
    public class LaunchGoogleMap : ILaunchGoogleMap
    {
        public void OpenGoogleMap(string location)
        {
            NSUrl url = new NSUrl("http://maps.google.com/maps?daddr=" + location);
            UIApplication.SharedApplication.OpenUrl(url);
        }
    }
}