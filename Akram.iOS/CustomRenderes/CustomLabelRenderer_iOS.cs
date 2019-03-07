using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Akram.CustomControls;
using Akram.iOS.CustomRenderes;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(LineLabel), typeof(CustomLabelRenderer_iOS))]
namespace Akram.iOS.CustomRenderes
{
    public class CustomLabelRenderer_iOS : LabelRenderer
    {
        public CustomLabelRenderer_iOS()
        {

        }



        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                base.OnElementPropertyChanged(sender, e);

                var lineSpacingLabel = (LineLabel)this.Element;
                var paragraphStyle = new NSMutableParagraphStyle()
                {
                    LineSpacing = (nfloat)lineSpacingLabel.LineSpacing
                };
                var _string = new NSMutableAttributedString(lineSpacingLabel.Text);
                var style = UIStringAttributeKey.ParagraphStyle;
                var range = new NSRange(0, _string.Length);

                _string.AddAttribute(style, paragraphStyle, range);

                this.Control.AttributedText = _string;
            }
            catch (Exception)
            {

            }
        }
    }
}