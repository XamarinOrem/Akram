using System;
using System.Collections.Generic;
using Akram.ViewModels;
using Xamarin.Forms;

namespace Akram.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this,false);

            BindingContext = new LoginViewModel(Navigation);
        }
    }
}
