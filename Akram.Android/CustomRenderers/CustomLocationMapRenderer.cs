using System;
using System.ComponentModel;
using Akram.CustomControls;
using Akram.Droid.CustomRenderers;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Xamarin.Forms.Maps.Android;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

[assembly: ExportRenderer(typeof(LocationMap), typeof(CustomLocationMapRenderer))]
namespace Akram.Droid.CustomRenderers
{
    public class CustomLocationMapRenderer : MapRenderer, GoogleMap.IInfoWindowAdapter
    {
        public static Marker MoveAble_Marker;
        bool isDrawn;

        public CustomLocationMapRenderer(Context context) : base(context)
        {

        }

        protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Map> e)
        {
            try
            {
                base.OnElementChanged(e);

                if (e.OldElement != null)
                {
                    NativeMap.InfoWindowClick -= OnInfoWindowClick;
                }


            }
            catch { }
        }


        void OnInfoWindowClick(object sender, GoogleMap.InfoWindowClickEventArgs e)
        {
            try
            {
            }
            catch { }
        }


        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                base.OnElementPropertyChanged(sender, e);

                if (e.PropertyName.Equals("VisibleRegion") && !isDrawn)
                {
                    NativeMap.Clear();
                    NativeMap.InfoWindowClick += OnInfoWindowClick;

                    var polylineOptions = new PolylineOptions();
                    polylineOptions.InvokeColor(0x66FF0000);
                    var getPins = ((LocationMap)Element).CustomPins;
                    if (getPins != null)
                    {
                        foreach (var pin in getPins)
                        {
                           BitmapDescriptor icon = BitmapDescriptorFactory.FromResource(Resource.Drawable.map_pin);

                            var marker = new MarkerOptions();
                            marker.SetPosition(new LatLng(pin.Pin.Position.Latitude, pin.Pin.Position.Longitude));
                            marker.SetTitle(pin.Pin.Label);
                            marker.SetSnippet(pin.Pin.Address);
                            marker.SetIcon(icon);
                            if (pin.Id != "1")
                            {
                                MoveAble_Marker = NativeMap.AddMarker(marker);
                            }
                            else
                            {
                                NativeMap.AddMarker(marker);
                            }

                        }
                    }
                    var getRouteCoordinates = ((LocationMap)Element).RouteCoordinates;
                    if (getRouteCoordinates != null)
                    {
                        foreach (var position in getRouteCoordinates)
                        {
                            polylineOptions.Add(new LatLng(position.Latitude, position.Longitude));
                        }
                        NativeMap.AddPolyline(polylineOptions);
                    }

                    isDrawn = true;

                }
            }
            catch {

            }

        }

        CustomPin GetCustomPin(Marker annotation)
        {
            var position = new Position(annotation.Position.Latitude, annotation.Position.Longitude);
            foreach (var pin in ((LocationMap)Element).CustomPins)
            {
                if (pin.Pin.Position == position)
                {
                    return pin;
                }
            }
            return null;

        }

        public Android.Views.View GetInfoWindow(Marker marker)
        {
            return null;
        }

        public Android.Views.View GetInfoContents(Marker marker)
        {
            throw new NotImplementedException();
        }
    }



}
