using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Akram.DependencyInterface;
using Akram.Droid.DependencyInterface;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;

[assembly: Dependency(typeof(LaunchGoogleMap))]
namespace Akram.Droid.DependencyInterface
{
    public class LaunchGoogleMap : ILaunchGoogleMap
    {
        public void OpenGoogleMap(string loc)
        {
            //var geoUri = Android.Net.Uri.Parse("https://www.google.com/maps/dir/?api=1&&origin=" + currentLat + "," + currentLong + "&destination=" + destLat + "," + destLong);
            var geoUri = Android.Net.Uri.Parse("http://maps.google.com/maps?daddr=" + loc);
            var mapIntent = new Intent(Intent.ActionView, geoUri);
            mapIntent.SetFlags(ActivityFlags.NewTask);
            Android.App.Application.Context.StartActivity(mapIntent);
        }
    }
}