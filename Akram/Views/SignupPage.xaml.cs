using System;
using System.Collections.Generic;
using System.Linq;
using Akram.Repository;
using Akram.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace Akram.Views
{
    public partial class SignupPage : ContentPage
    {
        int _limit = 30;

        public SignupPage()
        {
            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);

            if (Device.RuntimePlatform == Device.iOS)
            {
                firstGrid.Margin = new Thickness(0, 30, 0, 0);
            }

            if (Device.RuntimePlatform == Device.Android)
            {
                App.Current.On<Xamarin.Forms.PlatformConfiguration.Android>().
               UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
            }

            //getCountryCodes();

            nameTxt.Completed += (object sender, EventArgs e) => userNameTxt.Focus();

            userNameTxt.Completed += (object sender, EventArgs e) => emailTxt.Focus();

            emailTxt.Completed += (object sender, EventArgs e) => passwordTxt.Focus();

            passwordTxt.Completed += (object sender, EventArgs e) => confirmPasswordTxt.Focus();

            confirmPasswordTxt.Completed += (object sender, EventArgs e) => phoneNumberTxt.Focus();

            phoneNumberTxt.Completed += (object sender, EventArgs e) => genderText.Focus();

            BindingContext = new SignUpViewModel(Navigation);
        }

        //public void getCountryCodes()
        //{
        //    try
        //    {
        //        //var getList = CommonLib.LoadJson();
        //        if (getList != null)
        //        {
        //            phoneCodePicker.SelectedIndex = 0;
        //            phoneCodePicker.Title = getList[0].dial_code;
        //            foreach (var item in getList)
        //            {
        //                if (!string.IsNullOrEmpty(item.dial_code))
        //                {
        //                    phoneCodePicker.Items.Add(item.dial_code);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //    }
        //}


        void Phone_Code_Tapped(object sender, EventArgs e)
        {
            phoneCodePicker.Focus();
        }

        void Birth_Date_Tapped(object sender, EventArgs e)
        {
            birthDatePicker.Focus();
        }

        private void Gender_Tapped(object sender, EventArgs e)
        {
            GenderPicker.Focus();
        }

        private void nameTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            var _entry = sender as Xamarin.Forms.Entry;
            string _text = _entry.Text;      //Get Current Text
            if (_text.Length > _limit)       //If it is more than your character restriction
            {
                _text = _text.Remove(_text.Length - 1);  // Remove Last character
                _entry.Text = _text;        //Set the Old value
            }
        }
    }
}
