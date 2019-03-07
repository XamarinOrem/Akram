using Akram.DependencyInterface;
using Akram.Droid.CustomRenderers;
using Android;
using Android.Content.PM;
using Android.OS;
using Plugin.Permissions;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(StoragePermissionsDroid))]
namespace Akram.Droid.CustomRenderers
{
    public class StoragePermissionsDroid : IStoragePermissions
    {
        const string permissionC = Manifest.Permission.Camera;
        const string permissionAL = Manifest.Permission.AccessCoarseLocation;
        const string permissionCL = Manifest.Permission.AccessFineLocation;
        const string permissionRS = Manifest.Permission.ReadExternalStorage;
        const string permissionWS = Manifest.Permission.WriteExternalStorage;
        const string permissionFL = Manifest.Permission.Flashlight;
        const int RequestLocationId = 0;

        readonly string[] permissions =
   {
         Manifest.Permission.Camera,
                  Manifest.Permission.ReadExternalStorage,
           Manifest.Permission.WriteExternalStorage,
           Manifest.Permission.Flashlight
    };

        readonly string[] permissions1 =
          {
         Manifest.Permission.AccessCoarseLocation,
         Manifest.Permission.AccessFineLocation
    };

        public void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        public void GetContactPermissions()
        {
            if ((int)Build.VERSION.SdkInt >= 23 && MainActivity.activity.CheckSelfPermission(permissionC) != (int)Permission.Granted)
            {
                MainActivity.activity.RequestPermissions(permissions, RequestLocationId);
            }
            if ((int)Build.VERSION.SdkInt >= 23 && MainActivity.activity.CheckSelfPermission(permissionRS) != (int)Permission.Granted)
            {
                MainActivity.activity.RequestPermissions(permissions, RequestLocationId);
            }
            if ((int)Build.VERSION.SdkInt >= 23 && MainActivity.activity.CheckSelfPermission(permissionWS) != (int)Permission.Granted)
            {
                MainActivity.activity.RequestPermissions(permissions, RequestLocationId);
            }
            if ((int)Build.VERSION.SdkInt >= 23 && MainActivity.activity.CheckSelfPermission(permissionFL) != (int)Permission.Granted)
            {
                MainActivity.activity.RequestPermissions(permissions, RequestLocationId);
            }

        }

        public void GetScannerPermissions()
        {
            if ((int)Build.VERSION.SdkInt >= 23 && MainActivity.activity.CheckSelfPermission(permissionC) != (int)Permission.Granted)
            {
                MainActivity.activity.RequestPermissions(permissions, RequestLocationId);
            }
        }

        public void GetLocationPermissions()
        {
            try
            {
                if ((int)Build.VERSION.SdkInt >= 23 &&MainActivity.activity.CheckSelfPermission(permissionAL) != (int)Permission.Granted)
                {
                    MainActivity.activity.RequestPermissions(permissions1, RequestLocationId);
                }
                if ((int)Build.VERSION.SdkInt >= 23 && MainActivity.activity.CheckSelfPermission(permissionCL) != (int)Permission.Granted)
                {
                    MainActivity.activity.RequestPermissions(permissions1, RequestLocationId);
                }

            }
            catch (System.Exception ex)
            {

            }


        }
    }
}