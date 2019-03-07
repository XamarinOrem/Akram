using System;
using Akram.CustomControls;
using Akram.iOS.CustomRenderes;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomListView), typeof(CustomListViewRenderer_iOS))]
namespace Akram.iOS.CustomRenderes
{
    public class CustomListViewRenderer_iOS:ListViewRenderer
    {
        public CustomListViewRenderer_iOS()
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            base.OnElementChanged(e);

            if(this.Control!=null)
            {
                Control.ScrollEnabled = false;
            }
        }
    }
}
