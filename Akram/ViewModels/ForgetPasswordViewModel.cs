using Akram.Repository;
using Akram.Views;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Akram.ViewModels
{
    public class ForgetPasswordViewModel : INotifyPropertyChanged
    {
        HttpClientBase cbase = new HttpClientBase();

        public INavigation _navigation;
        /// <summary>
        /// property for the email field
        /// </summary>
        private string _email;
        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                _email = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Email"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ForgetPasswordViewModel(INavigation navigation)
        {
            _navigation = navigation;
        }

        /// <summary>
        /// This tapped event will be used for opening the signup page.
        /// </summary>
        public Command cancelCommand
        {
            get
            {
                return new Command((e) =>
                {
                    _navigation.PopPopupAsync();

                });
            }
        }
        /// <summary>
        /// This event will be used for forgotPasswordCommand button functionality.
        /// </summary>
        public Command forgotPasswordCommand
        {
            get
            {
                return new Command(async (e) =>
                {
                    var returnMessage = CheckLoginValidations();
                    if (!string.IsNullOrEmpty(returnMessage))
                    {
                        await _navigation.PushPopupAsync(new ShowMessage(returnMessage));
                        return;
                    }
                    if (!CommonLib.CheckConnection())
                    {
                        await _navigation.PushPopupAsync(new NoInternetPopup());
                        return;
                    }
                    else
                    {
                        await ForgotPassword();
                    }            
                });
            }
        }

        public string CheckLoginValidations()
        {
            string msg = string.Empty;
            if (string.IsNullOrEmpty(Email))
            {
                msg += "Please Enter Email"+Environment.NewLine;
            }
            if (!string.IsNullOrEmpty(Email))
            {
                if (!CommonLib.CheckValidEmail(Email))
                {
                    msg += "Please Enter Valid Email" + Environment.NewLine;
                }
            }
            return msg;
        }

        public async Task ForgotPassword()
        {
            try
            {
                await _navigation.PushPopupAsync(new LoaderPopup());

                var nvc = new List<KeyValuePair<string, string>>();
                nvc.Add(new KeyValuePair<string, string>("email", Email));
                nvc.Add(new KeyValuePair<string, string>("api_key", CommonLib.apiKey));

                var result = await cbase.ForgotPassword(ApiUrl.ForgotPasswordUrl,nvc);
                if (result != null)
                {
                    if (result.status.status_text.ToLower()=="success")
                    {
                        LoaderPopup.CloseAllPopup();
                        await _navigation.PushPopupAsync(new ForgotPasswordSuccessPage());
                    }
                    else
                    {
                        await _navigation.PopPopupAsync();
                        await App.Current.MainPage.DisplayAlert("", "Wrong email address, Please try again", "OK");
                    }
                }
                else
                {
                    await _navigation.PopPopupAsync();
                    await App.Current.MainPage.DisplayAlert("", "Wrong email address, Please try again", "OK");
                }
            }
            catch(Exception ex)
            {
                LoaderPopup.CloseAllPopup();
            }
        }

    }
}
