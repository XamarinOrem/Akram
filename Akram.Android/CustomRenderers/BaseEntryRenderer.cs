using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Akram.CustomControls;
using Akram.Droid.CustomRenderers;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(BaseEntry), typeof(BaseEntryRenderer))]
namespace Akram.Droid.CustomRenderers
{
    public class BaseEntryRenderer : EntryRenderer
    {
        public BaseEntryRenderer(Context context) : base(context)
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
                    Control?.SetBackgroundColor(Android.Graphics.Color.White);
                    Control.Gravity = GravityFlags.CenterVertical;

                    Control?.SetPadding(20, 10, 10, 10);



                }
                catch (Exception)
                {
                    // An exception means that the custom font wasn't found.
                    // Typeface.CreateFromAsset throws an exception when it didn't find a matching font.
                    // When it isn't found we simply do nothing, meaning it reverts back to default.
                }
            }

            BaseEntry entry = (BaseEntry)this.Element;

            if (this.Control != null)
            {
                if (entry != null)
                {
                    SetReturnType(entry);

                    // Editor Action is called when the return button is pressed
                    Control.EditorAction += (object sender, TextView.EditorActionEventArgs args) =>
                    {
                        if (entry.ReturnType != CustomControls.ReturnType.Next)
                            entry.Unfocus();

                        // Call all the methods attached to base_entry event handler Completed
                        entry.InvokeCompleted();
                    };
                }
            }
        }

        private void SetReturnType(BaseEntry entry)
        {
            CustomControls.ReturnType type = entry.ReturnType;

            switch (type)
            {
                case CustomControls.ReturnType.Go:
                    Control.ImeOptions = ImeAction.Go;
                    Control.SetImeActionLabel("Go", ImeAction.Go);
                    break;
                case CustomControls.ReturnType.Next:
                    Control.ImeOptions = ImeAction.Next;
                    Control.SetImeActionLabel("Next", ImeAction.Next);
                    break;
                case CustomControls.ReturnType.Send:
                    Control.ImeOptions = ImeAction.Send;
                    Control.SetImeActionLabel("Send", ImeAction.Send);
                    break;
                case CustomControls.ReturnType.Search:
                    Control.ImeOptions = ImeAction.Search;
                    Control.SetImeActionLabel("Search", ImeAction.Search);
                    break;
                default:
                    Control.ImeOptions = ImeAction.Done;
                    Control.SetImeActionLabel("Done", ImeAction.Done);
                    break;
            }
        }
    }
}