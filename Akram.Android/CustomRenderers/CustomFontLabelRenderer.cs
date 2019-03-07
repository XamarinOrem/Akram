using System;
using System.ComponentModel;
using System.Reflection;
using Android.Content;
using Android.Graphics;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Java.Lang;
using Android.Text;
using Akram.Droid.CustomRenderers;

[assembly: ExportRenderer(typeof(Label), typeof(CustomFontLabelRenderer))]
namespace Akram.Droid.CustomRenderers
{
    public class CustomFontLabelRenderer : LabelRenderer
    {

        public CustomFontLabelRenderer(Context context) : base(context)
        {

        }
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);



            if (!string.IsNullOrEmpty(e.NewElement?.FontFamily))
            {
                try
                {
                    var font = Typeface.CreateFromAsset(Forms.Context.ApplicationContext.Assets, e.NewElement.FontFamily + ".OTF");
                    Control.Typeface = font;
                    UpdateFormattedText();
                }
                catch (System.Exception)
                {
                    // An exception means that the custom font wasn't found.
                    // Typeface.CreateFromAsset throws an exception when it didn't find a matching font.
                    // When it isn't found we simply do nothing, meaning it reverts back to default.
                }
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == Label.FormattedTextProperty.PropertyName ||
                e.PropertyName == Label.TextProperty.PropertyName ||
                e.PropertyName == Label.FontAttributesProperty.PropertyName ||
                e.PropertyName == Label.FontProperty.PropertyName ||
                e.PropertyName == Label.FontSizeProperty.PropertyName ||
                e.PropertyName == Label.FontFamilyProperty.PropertyName ||
                e.PropertyName == Label.TextColorProperty.PropertyName)
            {
                UpdateFormattedText();
            }
        }


        private void UpdateFormattedText()
        {
            if (Element?.FormattedText == null)
                return;

            var extensionType = typeof(FormattedStringExtensions);
            var type = extensionType.GetNestedType("FontSpan", BindingFlags.NonPublic);
            var ss = new SpannableString(Control.TextFormatted);
            var spans = ss.GetSpans(0, ss.ToString().Length, Class.FromType(type));
            foreach (var span in spans)
            {
                var start = ss.GetSpanStart(span);
                var end = ss.GetSpanEnd(span);
                var flags = ss.GetSpanFlags(span);
                var font = (Font)type.GetProperty("Font").GetValue(span, null);
                ss.RemoveSpan(span);
                var newSpan = new CustomTypefaceSpan(Control, Element, font);
                ss.SetSpan(newSpan, start, end, flags);
            }
            Control.TextFormatted = ss;
        }
    }
}
