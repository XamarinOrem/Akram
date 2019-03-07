using System;
using Akram.Droid.CustomRenderers;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Entry), typeof(CustomFontEntryRenderer))]
namespace Akram.Droid.CustomRenderers
{
    public class CustomFontEntryRenderer : EntryRenderer
    {
        public CustomFontEntryRenderer(Context context) : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (!string.IsNullOrEmpty(e.NewElement?.FontFamily))
            {
                try
                {
                    var font = Typeface.CreateFromAsset(Android.App.Application.Context.ApplicationContext.Assets, e.NewElement.FontFamily + ".OTF");
                    Control.Typeface = font;

                    GradientDrawable gd = new GradientDrawable();

                    //this line sets the bordercolor
                    gd.SetColor(global::Android.Graphics.Color.Transparent);

                    Control.SetBackgroundDrawable(gd);
                    Control?.SetBackgroundColor(Android.Graphics.Color.White );
                    Control.Gravity=GravityFlags.CenterVertical;

                    Control?.SetPadding(20, 10, 10, 10);



                }
                catch (Exception)
                {
                    // An exception means that the custom font wasn't found.
                    // Typeface.CreateFromAsset throws an exception when it didn't find a matching font.
                    // When it isn't found we simply do nothing, meaning it reverts back to default.
                }
            }
        }

    }
}
