using Akram.CustomControls;
using Akram.iOS.CustomRenderes;
using Akram.Models;
using Akram.Repository;
using Akram.Views;
using CoreGraphics;
using Foundation;
using MapKit;
using ObjCRuntime;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.iOS;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer_iOS))]
namespace Akram.iOS.CustomRenderes
{
    public class CustomMapRenderer_iOS : MapRenderer
    {
        public static UIColor circleColor;
        public static double circleRadius;
        UIView customPinView;
        List<CustomPin> customPins;
        MKCircleRenderer circleRenderer;

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            var NativeMap = Control as MKMapView;

        }

        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            try
            {
                base.OnElementChanged(e);

                if (e.OldElement != null)
                {
                    var nativeMap = Control as MKMapView;
                    if (nativeMap != null)
                    {
                        nativeMap.RemoveAnnotations(nativeMap.Annotations);
                        nativeMap.GetViewForAnnotation = null;
                        nativeMap.RemoveOverlays(nativeMap.Overlays);
                        nativeMap.OverlayRenderer = null;
                        circleRenderer = null;
                        nativeMap.DidSelectAnnotationView -= OnDidSelectAnnotationView;
                    }
                }

                if (e.NewElement != null)
                {
                    var formsMap = (CustomMap)e.NewElement;
                    var nativeMap = Control as MKMapView;
                    var circle = formsMap.Circle;

                    nativeMap.ShowsUserLocation = false;
                    nativeMap.ScrollEnabled = false;
                    nativeMap.GetViewForAnnotation = GetViewForAnnotation;
                    nativeMap.OverlayRenderer = GetOverlayRenderer;
                    nativeMap.DidSelectAnnotationView += OnDidSelectAnnotationView;

                    if (circle != null)
                    {
                        var circleOverlay = MKCircle.Circle(
                            new CoreLocation.CLLocationCoordinate2D
                            (circle.Position.Latitude, circle.Position.Longitude),
                            circle.Radius);
                        nativeMap.AddOverlay(circleOverlay);
                    }
                }

                MessagingCenter.Subscribe<Xamarin.Forms.Application, bool>(this, "DrawCircleFirstTime", (sender, message) =>
                {
                    var getMap = ((CustomMap)Element);
                    var circle = getMap.Circle;
                    if (getMap != null)
                    {
                        if (circle != null)
                        {
                            DrawCircle(getMap, 20, UIColor.FromRGB(169,169,169));
                        }
                    }
                });

                MessagingCenter.Subscribe<Xamarin.Forms.Application, bool>(this, "RadiusUpdate", (sender, message) =>
                    {
                        var getMap = ((CustomMap)Element);
                        var circle = getMap.Circle;
                        if (getMap != null)
                        {
                            if (circle != null)
                            {
                            DrawCircle(getMap, 30, UIColor.FromRGB(216,191,216));
                            }
                        }
                    });

                MessagingCenter.Subscribe<Xamarin.Forms.Application, bool>(this, "RadiusUpdateAgain", (sender, message) =>
                    {
                    var getMap = ((CustomMap)Element);
                        var circle = getMap.Circle;
                        if (getMap != null)
                        {
                            if (circle != null)
                            {
                            DrawCircle(getMap, 40, UIColor.FromRGB(255,0,0));
                            }
                        }
                    });

                    MessagingCenter.Subscribe<SendPosition>(this, "PositionChanged", (sender) =>
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            var getMap = ((CustomMap)Element);
                            getMap.Circle.Position = sender.position;
                            
                        var getPin = getMap.CustomPins.Where(a => a.Pin.Label == string.Empty).FirstOrDefault();
                            if (getPin != null)
                            {
                            var getIndex = getMap.CustomPins.IndexOf(getPin);
                            getMap.CustomPins.RemoveAt(getIndex);
                            getMap.Pins.Remove(getPin.Pin);
                            }
                       
                        var start_pin_custom = new CustomPin
                        {
                            Pin = new Pin
                            {
                                Type = PinType.Place,
                                Position = new Position(Convert.ToDouble(sender.position.Latitude), Convert.ToDouble(sender.position.Longitude)),
                                Label = "",
                                Address = string.IsNullOrEmpty(LoginUserDetails.FacebookProfilePicture) ? "Current Location" : "User Current Location",

                            },
                            Id = string.IsNullOrEmpty(LoginUserDetails.FacebookProfilePicture) ? "Current Location" : "User Current Location",
                            startPin = true,
                            Url = ""
                        };

                        getMap.CustomPins = new List<CustomPin> { start_pin_custom };

                        getMap.Pins.Add(start_pin_custom.Pin);

                        getMap.MoveToRegion(MapSpan.FromCenterAndRadius(
                new Position(sender.position.Latitude, sender.position.Longitude), Xamarin.Forms.Maps.Distance.FromMeters(50)));

                        DrawCircle(getMap, circleRadius, circleColor);
                    });
                });
            }
            catch (Exception ex)
            {

            }
        }

        MKOverlayRenderer GetOverlayRenderer(MKMapView mapView, IMKOverlay overlayWrapper)
        {
            if (circleRenderer == null && !Equals(overlayWrapper, null))
            {
                var overlay = Runtime.GetNSObject(overlayWrapper.Handle) as IMKOverlay;
                circleRenderer = new MKCircleRenderer(overlay as MKCircle)
                {
                    FillColor = circleColor,
                    Alpha = 0.4f
                };
            }
            return circleRenderer;
        }


        protected override MKAnnotationView GetViewForAnnotation(MKMapView mapView, IMKAnnotation annotation)
        {
            MKAnnotationView annotationView = null;

            if (annotation is MKUserLocation)
                return null;

            var customPin = GetCustomPin(annotation as MKPointAnnotation);
            if (customPin == null)
            {
                return null;
            }

            annotationView = mapView.DequeueReusableAnnotation(customPin.Id.ToString());
            if (annotationView == null)
            {
                annotationView = new CustomMKAnnotationView(annotation, customPin.Id.ToString());
                if (customPin.Pin.Address == "Current Location")
                {
                    var assembly = typeof(App).GetTypeInfo().Assembly;
                    var img = UIImage.FromResource(assembly, "Akram.ic_signup_default_green.png");
                    UIImageView _image = new UIImageView(new CGRect(-30, -30, 60, 60));
                    _image.Layer.CornerRadius = _image.Layer.Bounds.Size.Width / 2;
                    _image.ClipsToBounds = true;
                    _image.Layer.BorderWidth = 2;
                    _image.Layer.BorderColor = UIColor.FromRGB(16, 162, 74).CGColor;
                    _image.Image = img;
                    annotationView.AddSubview(_image);
                }
                else if (customPin.Pin.Address == "User Current Location")
                {
                    var assembly = typeof(App).GetTypeInfo().Assembly;
                    var stream = GetStreamFromUrl(LoginUserDetails.FacebookProfilePicture);
                    var imageData = NSData.FromStream(stream);
                    var img = UIImage.LoadFromData(imageData);
                    UIImageView _image = new UIImageView(new CGRect(-30, -30, 60, 60));
                    _image.Layer.CornerRadius = _image.Layer.Bounds.Size.Width / 2;
                    _image.ClipsToBounds = true;
                    _image.Layer.BorderWidth = 2;
                    _image.Layer.BorderColor = UIColor.FromRGB(16, 162, 74).CGColor;
                    _image.Image = img;
                    //var returnImg = OverlayImage(_image.Image, img1);
                    annotationView.AddSubview(_image);
                }
                else if (customPin.Pin.Address == "Green")
                {
                    annotationView.Image = UIImage.FromFile("map_green_gift.png");
                }
                else
                {
                    annotationView.Image = UIImage.FromFile("gift_gray.png");
                }
                annotationView.CalloutOffset = new CGPoint(0, 0);
                //annotationView.LeftCalloutAccessoryView = new UIImageView(UIImage.FromFile("monkey.png"));
                //annotationView.RightCalloutAccessoryView = UIButton.FromType(UIButtonType.DetailDisclosure);
                ((CustomMKAnnotationView)annotationView).Id = customPin.Id.ToString();
                ((CustomMKAnnotationView)annotationView).Url = customPin.Url;
            }
            annotationView.CanShowCallout = false;

            return annotationView;
        }

        public UIImage OverlayImage(UIImage originalImage, UIImage newImage)
        {
            var size = new CGSize(originalImage.Size.Width, originalImage.Size.Height + newImage.Size.Height);

            UIGraphics.BeginImageContextWithOptions(size, true, 1.0f);
            originalImage.DrawAsPatternInRect(new CGRect(0, 0, size.Width, originalImage.Size.Height));
            newImage.DrawAsPatternInRect(new CGRect(0, originalImage.Size.Height, size.Width, newImage.Size.Height));

            var processedImage = UIGraphics.GetImageFromCurrentImageContext();
            return processedImage;
        }

        private static Stream GetStreamFromUrl(string url)
        {
            byte[] imageData = null;

            using (var wc = new System.Net.WebClient())
                imageData = wc.DownloadData(url);

            return new MemoryStream(imageData);
        }

        void OnDidSelectAnnotationView(object sender, MKAnnotationViewEventArgs e)
        {
            var customView = e.View as CustomMKAnnotationView;

            if (customView != null)
            {
                if (LoginUserDetails.IsLoggedIn)
                {
                    if (customView.Id == "Green")
                    {
                        HomeMapPage.pokemanId = customView.Url;
                        Xamarin.Forms.Application.Current.MainPage.Navigation.PushAsync(new MapGiftPage(HomeMapPage.result, HomeMapPage.pokemanId));

                    }
                    else if (customView.Id == "Gray")
                    {
                        MessagingCenter.Send(Xamarin.Forms.Application.Current, "NoCollectionPopup", true);
                    }
                }
                else
                {
                    MessagingCenter.Send(Xamarin.Forms.Application.Current, "NoAccountPopup", true);
                }
            }
        }

        CustomPin GetCustomPin(MKPointAnnotation annotation)
        {
            if (annotation != null)
            {
                var position = new Position(annotation.Coordinate.Latitude, annotation.Coordinate.Longitude);
                foreach (var pin in ((CustomMap)Element).CustomPins)
                {
                        return pin;
                }
            }
            return null;
        }

        public void DrawCircle(CustomMap getMap, double radius, UIColor color)
        {
            var nativeMap = Control as MKMapView;

            if (nativeMap?.Overlays!= null)
            {
                if (nativeMap.Overlays.Any())
                {
                    nativeMap.RemoveOverlays(nativeMap.Overlays);

                    circleRenderer = null;

                }
            }
            circleColor = color;
            circleRadius = radius;

            nativeMap.OverlayRenderer = GetOverlayRenderer;

            if (getMap.Circle != null)
            {
                if (getMap.Circle.Position != null)
                {
                    var circleOverlay = MKCircle.Circle(
                            new CoreLocation.CLLocationCoordinate2D
                        (getMap.Circle.Position.Latitude, getMap.Circle.Position.Longitude),
                        radius);
                    nativeMap.AddOverlay(circleOverlay);
                }
            }
        }

    }
}