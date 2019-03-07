using System;
using System.ComponentModel;
using Akram.iOS.CustomRenderes;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Picker), typeof(BorderlessPickerRenderer))]
namespace Akram.iOS.CustomRenderes
{
    public class BorderlessPickerRenderer : PickerRenderer
    {
        public static void Init() { }
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            Control.Layer.BorderWidth = 0;
            Control.BorderStyle = UITextBorderStyle.None;
        }
    }
}
