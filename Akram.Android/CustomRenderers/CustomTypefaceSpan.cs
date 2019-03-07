using System;
using Android.Graphics;
using Android.Text;
using Android.Text.Style;
using Android.Util;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace Akram.Droid.CustomRenderers
{
    public class CustomTypefaceSpan : MetricAffectingSpan
    {
        private readonly Typeface _typeFace;
        private readonly TextView _textView;
        private Font _font;

        public CustomTypefaceSpan(TextView textView, Label label, Font font)
        {
            _textView = textView;
            _font = font;
            _typeFace = Typeface.CreateFromAsset(Android.App.Application.Context.Assets, GetFontName(_font.FontFamily ?? label.FontFamily, _font.FontAttributes));
        }

        private static string GetFontName(string fontFamily, FontAttributes fontAttributes)
        {
            return "MYRIADPROREGULAR.OTF";
        }

        public override void UpdateDrawState(TextPaint paint)
        {
            ApplyCustomTypeFace(paint);
        }

        public override void UpdateMeasureState(TextPaint paint)
        {
            ApplyCustomTypeFace(paint);
        }

        private void ApplyCustomTypeFace(Paint paint)
        {
            paint.SetTypeface(_typeFace);
            paint.TextSize = TypedValue.ApplyDimension(ComplexUnitType.Sp, _font.ToScaledPixel(), _textView.Resources.DisplayMetrics);
        }
    }
}
