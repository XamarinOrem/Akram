using System;
using System.Collections.Generic;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;

namespace Akram.Views
{
    public partial class ShowMessage : PopupPage
    {
        public ShowMessage(string message)
        {
            InitializeComponent();

            msgTxt.Text = message;

            CloseWhenBackgroundIsClicked = false;
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            Navigation.PopPopupAsync();

        }
    }
}
