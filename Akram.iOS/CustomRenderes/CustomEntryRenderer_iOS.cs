using System;
using Akram.CustomControls;
using Akram.iOS.CustomRenderes;
using CoreGraphics;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomEntry), typeof(CustomEntryRenderer_iOS))]
namespace Akram.iOS.CustomRenderes
{
    public class CustomEntryRenderer_iOS : EntryRenderer
    {
        public CustomEntryRenderer_iOS()
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            if (this.Control == null)
                return;
            this.Control.BorderStyle = UIKit.UITextBorderStyle.Line;
            Control.VerticalAlignment = UIControlContentVerticalAlignment.Center;
            Control.LeftView = new UIView(new CGRect(0, 0, 10, 0));
            Control.LeftViewMode = UITextFieldViewMode.Always;
            Control.RightView = new UIView(new CGRect(0, 0, 10, 0));
            Control.RightViewMode = UITextFieldViewMode.Always;
        }
    }
}
