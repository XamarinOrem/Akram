using Akram.CustomControls;
using Akram.Droid.CustomRenderers;
using Android.Content;
using Android.Graphics;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomEntry), typeof(CustomEntryRenderer_Droid))]
namespace Akram.Droid.CustomRenderers
{
    public class CustomEntryRenderer_Droid : EntryRenderer
    {
        public CustomEntryRenderer_Droid(Context context) : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement == null)
            {
                var font = Typeface.CreateFromAsset(Android.App.Application.Context.ApplicationContext.Assets, e.NewElement.FontFamily + ".OTF");
                Control.Typeface = font;

                //GradientDrawable gd = new GradientDrawable();

                ////this line sets the bordercolor
                //gd.SetColor(global::Android.Graphics.Color.Rgb(92,92,92));


                //Control.SetBackgroundDrawable(gd);
                //Control?.SetBackgroundColor(Android.Graphics.Color.Transparent);
                //Control.Gravity = GravityFlags.CenterVertical;
                

                Control?.SetPadding(10, 10, 10, 10);
            }
        }
    }
}