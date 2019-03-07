using Akram.ViewModels;
using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Akram.Views
{
    public partial class ForgotPasswordPage : PopupPage
    {
        public ForgotPasswordPage()
        {
            InitializeComponent();

            BindingContext = new ForgetPasswordViewModel(Navigation);
        }
    }
}
