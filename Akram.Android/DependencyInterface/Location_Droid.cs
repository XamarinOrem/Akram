using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Locations;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Android;
using Xamarin.Forms;
using Akram.Droid.DependencyInterface;
using Akram.DependencyInterface;

[assembly: Xamarin.Forms.Dependency(typeof(Location_Droid))]
namespace Akram.Droid.DependencyInterface
{
    public class Location_Droid : ILocSettings
    {
        public bool CheckGpsEnable()
        {
            bool status = false;
            LocationManager LM = (LocationManager)Android.App.Application.Context.GetSystemService(Context.LocationService);
            if (LM.IsProviderEnabled(LocationManager.GpsProvider))
            {
                status = true;
            }
            else
            {
                status = false;
            }
            return status;
        }

        public void OpenSettings()
        {
            Context ctx = Android.App.Application.Context;
            Intent myIntent = new Intent(Android.Provider.Settings.ActionLocationSourceSettings);
            myIntent.SetFlags(ActivityFlags.NewTask);
            ctx.StartActivity(myIntent);
        }
    }
}