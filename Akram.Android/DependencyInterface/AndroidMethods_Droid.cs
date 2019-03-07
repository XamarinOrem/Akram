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

[assembly: Xamarin.Forms.Dependency(typeof(AndroidMethods_Droid))]
namespace Akram.Droid.DependencyInterface
{
    public class AndroidMethods_Droid : IAndroidMethods
    {
        public void CloseApp()
        {
            Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
        }
    }
}