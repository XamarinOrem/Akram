using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Akram.CustomControls;
using Akram.Droid.CustomRenderers;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(LineLabel), typeof(CustomLabelRenderer_Droid))]
namespace Akram.Droid.CustomRenderers
{
    public class CustomLabelRenderer_Droid : LabelRenderer
    {
        public CustomLabelRenderer_Droid(Context context) : base(context)
        {

        }

        protected LineLabel LineSpacingLabel { get; private set; }

        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null)
            {
                this.LineSpacingLabel = (LineLabel)this.Element;
            }

            if (!string.IsNullOrEmpty(e.NewElement?.StyleId))
            {
                var font = Typeface.CreateFromAsset(Android.App.Application.Context.ApplicationContext.Assets, e.NewElement.StyleId + ".OTF");

                Control.Typeface = font;

                var lineSpacing = this.LineSpacingLabel.LineSpacing;

                this.Control.SetLineSpacing(1f, (float)lineSpacing);

                this.UpdateLayout();
            }
        }
    }
}