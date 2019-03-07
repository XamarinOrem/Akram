using Akram.Models;
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
    public class ChangePasswordViewModel : INotifyPropertyChanged
    {
        public INavigation _navigation;

        HttpClientBase cbase = new HttpClientBase();

        private string _oldPassword;
        public string OldPassword
        {
            get
            {
                return _oldPassword;
            }
            set
            {
                _oldPassword = value;
                PropertyChanged(this, new PropertyChangedEventArgs("OldPassword"));
            }
        }
        private string _newPassword;
        public string NewPassword
        {
            get
            {
                return _newPassword;
            }
            set
            {
                _newPassword = value;
                PropertyChanged(this, new PropertyChangedEventArgs("NewPassword"));
            }
        }
        private string _verifyPassword;
        public string VerifyPassword
        {
            get
            {
                return _verifyPassword;
            }
            set
            {
                _verifyPassword = value;
                PropertyChanged(this, new PropertyChangedEventArgs("VerifyPassword"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ChangePasswordViewModel(INavigation navigation)
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
        public Command changePasswordCommand
        {
            get
            {
                return new Command(async (e) =>
                {
                    var returnMessage = CheckValidations();
                    if (!string.IsNullOrEmpty(returnMessage))
                    {
                        await _navigation.PushPopupAsync(new ShowMessage(returnMessage));
                        return;
                    }
                    else
                    {
                        if (!CommonLib.CheckConnection())
                        {
                            await _navigation.PushPopupAsync(new NoInternetPopup());
                            return;
                        }
                        else
                        {
                            await ChangePassword();
                        }
                    }

                });
            }
        }

        public string CheckValidations()
        {
            string msg = string.Empty;
            if (string.IsNullOrEmpty(OldPassword))
            {
                msg += "Please Enter Old Password" + Environment.NewLine + Environment.NewLine;
            }
            if (string.IsNullOrEmpty(NewPassword))
            {
                msg += "Please Enter New Password" + Environment.NewLine + Environment.NewLine;
            }
            if (string.IsNullOrEmpty(VerifyPassword))
            {
                msg += "Please Enter Verify Password" + Environment.NewLine + Environment.NewLine;
            }
            if (VerifyPassword != NewPassword)
            {
                msg += "New Password and verify password not matched" + Environment.NewLine;
            }
            return msg;
        }

        public async Task ChangePassword()
        {
            try
            {
                await _navigation.PushPopupAsync(new LoaderPopup());

                var nvc = new List<KeyValuePair<string, string>>();
                nvc.Add(new KeyValuePair<string, string>("old_password", OldPassword));
                nvc.Add(new KeyValuePair<string, string>("new_password", NewPassword));
                nvc.Add(new KeyValuePair<string, string>("user_id", LoginUserDetails.UserId));
                nvc.Add(new KeyValuePair<string, string>("login_hash", LoginUserDetails.LoginHash));
                nvc.Add(new KeyValuePair<string, string>("api_key", CommonLib.apiKey));

                var result = await cbase.ForgotPassword(ApiUrl.ResetPasswordUrl, nvc);
                if (result != null)
                {
                    if (result.status.status_code == "-1")
                    {
                        LoaderPopup.CloseAllPopup();
                        await App.Current.MainPage.DisplayAlert("", "Change Password Successfully", "Ok");
                    }
                    else
                    {
                        await _navigation.PopPopupAsync();
                        await App.Current.MainPage.DisplayAlert("", result.status.status_text, "OK");
                    }
                }
                else
                {
                    await _navigation.PopPopupAsync();
                    await App.Current.MainPage.DisplayAlert("", CommonLib.someErrorMsg, "OK");
                }
            }
            catch
            {
                LoaderPopup.CloseAllPopup();
            }
        }

    }
}
