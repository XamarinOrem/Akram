using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Akram.CustomControls;
using Akram.DependencyInterface;
using Akram.Droid.CustomRenderers;
using Akram.Models;
using Akram.Repository;
using Akram.Views;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Hardware;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Net;
using Plugin.Media.Abstractions;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.Android;
using static Android.Graphics.PorterDuff;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace Akram.Droid.CustomRenderers
{
    public class CustomMapRenderer : MapRenderer, GoogleMap.IInfoWindowAdapter
    {
        private LatLngBounds.Builder _builder;
        //private SensorManager sm = null;
        //private Sensor grv = null;
        private CustomMap getMap = null;
        //private float mDeclination = 0;
        //private float[] mRotationMatrix = new float[16];
        Circle mapCircle;
        //private LocationRequest mLocationRequest = null;
        //public event EventHandler OrientationChanged;

        public CustomMapRenderer(Context context) : base(context)
        {
            _builder = new LatLngBounds.Builder();
            //sm = (SensorManager)Android.App.Application.Context.GetSystemService(Context.SensorService);

            //grv = sm.GetDefaultSensor(SensorType.Accelerometer);

            ////var listener = new MyOrientationListner();
            ////listener.OrientationChanged += OnOrientationChanged;

            //sm.RegisterListener(this, grv, SensorDelay.Normal);

            //if (grv == null)
            //{
            //    Toast.MakeText(Android.App.Application.Context, "SensorType.GeomagneticRotationVector not present!", ToastLength.Long).Show();
            //    return;
            //}
            //var windowManager = Android.App.Application.Context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
            //var orientation = windowManager.DefaultDisplay.Rotation;
        }

        public static Marker MoveAble_Marker;
        bool isDrawn;

        protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Map> e)
        {
            try
            {
                base.OnElementChanged(e);

                MessagingCenter.Subscribe<Application, bool>(this, "DrawCircle", (sender, message) =>
                {
                    var getMap = ((CustomMap)Element);
                    NativeMap.UiSettings.MyLocationButtonEnabled = false;
                    NativeMap.UiSettings.ScrollGesturesEnabled = false;
                    NativeMap.MyLocationEnabled = false;
                    NativeMap.SetMinZoomPreference(19);
                    NativeMap.MarkerClick += Map_MarkerClick;
                    DrawCircle(getMap, 20, 0X66A9A9A9);
                });

                MessagingCenter.Subscribe<Application, bool>(this, "RadiusUpdate", (sender, message) =>
                {
                    var getMap = ((CustomMap)Element);
                    NativeMap.UiSettings.MyLocationButtonEnabled = false;
                    NativeMap.MyLocationEnabled = false;
                    NativeMap.UiSettings.ScrollGesturesEnabled = false;
                    NativeMap.SetMinZoomPreference(19);
                    NativeMap.MarkerClick += Map_MarkerClick;
                    DrawCircle(getMap, 30, 0X66d8bfd8);
                });

                MessagingCenter.Subscribe<Application, bool>(this, "RadiusUpdateAgain", (sender, message) =>
                {
                    var getMap = ((CustomMap)Element);
                    NativeMap.UiSettings.MyLocationButtonEnabled = false;
                    NativeMap.MyLocationEnabled = false;
                    NativeMap.UiSettings.ScrollGesturesEnabled = false;
                    NativeMap.SetMinZoomPreference(19);
                    NativeMap.MarkerClick += Map_MarkerClick;
                    DrawCircle(getMap, 40, 0X66FF0000);
                });

                MessagingCenter.Subscribe<Application, bool>(this, "DrawCircleFirstTime", (sender, message) =>
                 {
                     var getMap = ((CustomMap)Element);
                     if (NativeMap != null)
                     {
                         NativeMap.UiSettings.MyLocationButtonEnabled = false;
                         NativeMap.MyLocationEnabled = false;
                         NativeMap.UiSettings.ScrollGesturesEnabled = false;
                         NativeMap.SetMinZoomPreference(19);
                         NativeMap.MarkerClick += Map_MarkerClick;
                     }
                     if (getMap != null)
                     {
                         DrawCircle(getMap, 20, 0X66A9A9A9);
                     }
                 });

                MessagingCenter.Subscribe<SendPosition>(this, "PositionChanged", (sender) =>
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        var getMap = ((CustomMap)Element);
                        getMap.Circle.Position = sender.position;
                        double getRadius = 0;
                        int getColor = 0;
                        if (mapCircle != null)
                        {
                            getRadius = mapCircle.Radius;
                            getColor = mapCircle.FillColor;
                            mapCircle.Remove();
                        }

                        var getPin = getMap.Pins.Where(a => a.Label == string.Empty).FirstOrDefault();
                        if (getPin != null)
                        {
                            getMap.Pins.Remove(getPin);
                        }

                        NativeMap.UiSettings.MyLocationButtonEnabled = false;
                        NativeMap.MyLocationEnabled = false;
                        NativeMap.SetMinZoomPreference(19);

                        var start_pin = new Pin
                        {
                            Type = PinType.Place,
                            Position = new Position(Convert.ToDouble(sender.position.Latitude), Convert.ToDouble(sender.position.Longitude)),
                            Address = string.IsNullOrEmpty(LoginUserDetails.FacebookProfilePicture) ? "Current Location" : "User Current Location",
                            Label = "",
                        };

                        getMap.Pins.Add(start_pin);
                        //getMap.MoveToRegion(MapSpan.FromCenterAndRadius(
                        //new Position(sender.position.Latitude, sender.position.Longitude), Xamarin.Forms.Maps.Distance.FromMeters(10)));


                        CameraPosition oldPos = new CameraPosition(new LatLng(sender.position.Latitude, sender.position.Longitude), 19, 60, 0);
                        if (getMap != null)
                        {
                            CameraPosition pos = CameraPosition.InvokeBuilder(oldPos).Build();

                            CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(pos);

                            NativeMap.MoveCamera(cameraUpdate);
                            NativeMap.AnimateCamera(cameraUpdate);
                        }

                        DrawCircle(getMap, getRadius, getColor);
                    });
                });

                if (e.OldElement != null)
                {
                    NativeMap.InfoWindowClick -= OnInfoWindowClick;
                }
            }
            catch { }
        }

        protected override MarkerOptions CreateMarker(Pin pin)
        {
            var marker = new MarkerOptions();
            marker.SetPosition(new LatLng(pin.Position.Latitude, pin.Position.Longitude));
            marker.SetTitle(pin.Label);
            marker.SetSnippet(pin.Address);
            marker.Flat(false);
            if (pin.Address == "Current Location")
            {
                var getStream = CommonLib.GetStreamFromLocalImage("Akram.ic_signup_default_green.png");
                var getBitmap = bitmapDescriptorFromVector(getStream);
                marker.SetIcon(BitmapDescriptorFactory.FromBitmap(getBitmap));
            }
            else if (pin.Address == "User Current Location")
            {
                Bitmap getBitmap = bitmapDescriptorFromVectorFromUrl(LoginUserDetails.FacebookProfilePicture);
                marker.SetIcon(BitmapDescriptorFactory.FromBitmap(getBitmap));
            }
            else if (pin.Address == "Green")
            {
                marker.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.map_green_gift));
            }
            else
            {
                marker.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.gift_gray));
            }
            return marker;
        }


        protected override void OnMapReady(Android.Gms.Maps.GoogleMap map)
        {
            try
            {
                base.OnMapReady(map);

                getMap = ((CustomMap)Element);

                if (getMap.Circle == null)
                {
                    return;
                }

                if (getMap.Circle.Position == null)
                {
                    return;
                }

                map.UiSettings.TiltGesturesEnabled = true;
                map.UiSettings.CompassEnabled = true;
                map.UiSettings.RotateGesturesEnabled = true;
                map.BuildingsEnabled = true;
                map.SetMinZoomPreference(19);
                map.UiSettings.ScrollGesturesEnabled = false;
                map.UiSettings.MyLocationButtonEnabled = false;
                map.MyLocationEnabled = false;

                map.MarkerClick += Map_MarkerClick;

                map.Clear();

                DrawCircle(getMap, 20, 0X66A9A9A9);
            }
            catch (Exception)
            {
                if (Device.RuntimePlatform == Device.Android)
                {
                    DependencyService.Get<IStoragePermissions>().GetLocationPermissions();
                }
            }


        }

        private async void Map_MarkerClick(object sender, GoogleMap.MarkerClickEventArgs e)
        {
            var getMarker = e.Marker as Marker;
            if (getMarker != null)
            {
                if (LoginUserDetails.IsLoggedIn)
                {
                    if (getMarker.Snippet == "Green")
                    {
                        await Application.Current.MainPage.Navigation.PushPopupAsync(new LoaderPopup());
                        HomeMapPage.pokemanId = getMarker.Title;
                        await Xamarin.Forms.Application.Current.MainPage.Navigation.PushAsync(new MapGiftPage(HomeMapPage.result, HomeMapPage.pokemanId));
                        LoaderPopup.CloseAllPopup();
                    }
                    else if (getMarker.Snippet == "Gray")
                    {
                        MessagingCenter.Send(Application.Current, "NoCollectionPopup", true); 
                    }
                }
                else
                {
                    MessagingCenter.Send(Application.Current, "NoAccountPopup", true); 
                }
            }
        }

        public void DrawCircle(CustomMap getMap, double radius, int color)
        {
            if (mapCircle != null)
                mapCircle.Remove();
            if (getMap.Circle != null)
            {
                if (getMap.Circle.Position != null)
                {
                    var circleOptions = new CircleOptions();
                    circleOptions.InvokeCenter(new LatLng(getMap.Circle.Position.Latitude, getMap.Circle.Position.Longitude));
                    circleOptions.InvokeRadius(radius);
                    circleOptions.InvokeFillColor(color);
                    circleOptions.InvokeStrokeColor(color);
                    circleOptions.InvokeStrokeWidth(0);

                    mapCircle = NativeMap.AddCircle(circleOptions);
                }
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

        }

        private Bitmap MakeCircle(Bitmap bitmap, Bitmap pinStreamImg)
        {
            int w = bitmap.Width;
            int h = bitmap.Height;

            int radius = Math.Min(h / 2, w / 2);
            Bitmap output = Bitmap.CreateBitmap(w + 8, h + 8, Bitmap.Config.Argb8888);

            Paint p = new Paint();
            p.AntiAlias = true;

            Canvas c = new Canvas(output);
            c.DrawARGB(0, 0, 0, 0);
            p.SetStyle(Paint.Style.Fill);

            c.DrawCircle((w / 2) + 4, (h / 2) + 4, radius, p);

            p.SetXfermode(new PorterDuffXfermode(Mode.SrcIn));

            c.DrawBitmap(bitmap, 4, 4, p);
            p.SetXfermode(null);
            p.SetStyle(Paint.Style.Stroke);
            p.Color = Android.Graphics.Color.Rgb(16, 162, 74);
            p.StrokeWidth = 5;
            c.DrawCircle((w / 2) + 4, (h / 2) + 4, radius, p);


            var returnImg = OverlayImage(output, pinStreamImg);

            return returnImg;
        }

        public Bitmap OverlayImage(Bitmap bmp1, Bitmap bmp2)
        {
            Bitmap bmOverlay = Bitmap.CreateBitmap(bmp2.Width, bmp2.Height, bmp2.GetConfig());
            Canvas canvas = new Canvas(bmOverlay);
            canvas.DrawBitmap(bmp2, new Matrix(), null);
            canvas.DrawBitmap(bmp1, 7, -1, null);
            return bmOverlay;
        }

        private static Stream GetStreamFromUrl(string url)
        {
            byte[] imageData = null;

            using (var wc = new System.Net.WebClient())
                imageData = wc.DownloadData(url);

            return new MemoryStream(imageData);
        }

        private Bitmap bitmapDescriptorFromVectorFromUrl(string imageUrl)
        {
            Bitmap image = null;
            try
            {
                var getStream = GetStreamFromUrl(imageUrl);
                var bitmapimage = BitmapFactory.DecodeStream(getStream);
                var resizeImage = Bitmap.CreateScaledBitmap(bitmapimage, 120,120, false);
                var getPinStream = CommonLib.GetStreamFromLocalImage("Akram.pin.png");
                var bitmapimage1 = BitmapFactory.DecodeStream(getPinStream);
                var resizeImage1 = Bitmap.CreateScaledBitmap(bitmapimage1, 150, 150, false);
                image = MakeCircle(resizeImage, resizeImage1);
            }
            catch (Exception e)
            {

            }
            return image;

        }

        private Bitmap bitmapDescriptorFromVector(Stream imageUrl)
        {
            Bitmap image = null;
            try
            {
                var bitmapimage = BitmapFactory.DecodeStream(imageUrl);
                var resizeImage = Bitmap.CreateScaledBitmap(bitmapimage, 120, 120, false);
                var getPinStream = CommonLib.GetStreamFromLocalImage("Akram.pin.png");
                var bitmapimage1 = BitmapFactory.DecodeStream(getPinStream);
                var resizeImage1 = Bitmap.CreateScaledBitmap(bitmapimage1, 150, 150, false);
                image = MakeCircle(resizeImage, resizeImage1);
            }
            catch (Exception e)
            {

            }
            return image;

        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            try
            {
                base.OnLayout(changed, l, t, r, b);

                if (changed)
                {
                    isDrawn = false;
                }
            }
            catch { }
        }

        void OnInfoWindowClick(object sender, GoogleMap.InfoWindowClickEventArgs e)
        {
            try
            {
                var customPin = GetCustomPin(e.Marker);
                if (customPin == null)
                {
                    throw new Exception("Custom pin not found");
                }

                if (!string.IsNullOrWhiteSpace(customPin.Url))
                {
                    var url = Android.Net.Uri.Parse(customPin.Url);
                    var intent = new Intent(Intent.ActionView, url);
                    intent.AddFlags(ActivityFlags.NewTask);
                    Android.App.Application.Context.StartActivity(intent);
                }
            }
            catch { }
        }

        public Android.Views.View GetInfoWindow(Marker marker)
        {
            return null;
        }

        CustomPin GetCustomPin(Marker annotation)
        {
            var position = new Position(annotation.Position.Latitude, annotation.Position.Longitude);
            foreach (var pin in ((CustomMap)Element).CustomPins)
            {
                if (pin.Pin.Position == position)
                {
                    return pin;
                }
            }
            return null;

        }

        public Android.Views.View GetInfoContents(Marker marker)
        {
            throw new NotImplementedException();
        }

        //private void updateCamera(float bearing)
        //{
        //    CameraPosition oldPos = NativeMap.CameraPosition;
        //    if (getMap != null)
        //    {
        //        CameraPosition pos = CameraPosition.InvokeBuilder(oldPos).Bearing(bearing).Build();

        //        CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(pos);

        //        NativeMap.MoveCamera(cameraUpdate);
        //    }
        //}

        public void onLocationChanged(Plugin.Geolocator.Abstractions.Position location)
        {
            LatLng pos = new LatLng(location.Latitude, location.Longitude);
            if (getMap != null)
            {
                CameraPosition cameraPosition = new CameraPosition.Builder().Target(pos).Zoom(17).Bearing(90).Tilt(30).Build();
                NativeMap.MoveCamera(CameraUpdateFactory.NewCameraPosition(cameraPosition));
            }

            //GeomagneticField field = new GeomagneticField(
            //        (float)location.Latitude,
            //        (float)location.Longitude,
            //        (float)location.Altitude,
            //        1000
            //    );

            //// getDeclination returns degrees
            //mDeclination = field.Declination;
        }

        //public void OnSensorChanged([GeneratedEnum] SensorType sensor, float[] values)
        //{
        //    if (getMap != null)
        //    {
        //        SensorManager.GetRotationMatrixFromVector(mRotationMatrix, values);
        //        float[] orientation = new float[3];
        //        SensorManager.GetOrientation(mRotationMatrix, orientation);
        //        float bearing = Math.toDegrees(orientation[0]) + mDeclination;
        //        updateCamera(bearing);
        //    }
        //}

        public void OnAccuracyChanged([GeneratedEnum] SensorType sensor, [GeneratedEnum] SensorStatus accuracy)
        {

        }

        //private void OnOrientationChanged(object sender, EventArgs e)
        //{
        //    OrientationChanged?.Invoke(this, EventArgs.Empty);



        //}
    }
}