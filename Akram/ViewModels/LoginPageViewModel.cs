using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Akram.DependencyInterface;
using Akram.Models;
using Akram.Repository;
using Akram.Views;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms;

namespace Akram.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
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

        /// <summary>
        /// property for the password field
        /// </summary>
        private string _password;

        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Password"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public LoginViewModel(INavigation navigation)
        {
            _navigation = navigation;
        }

        /// <summary>
        /// This tapped event will be used for opening the signup page.
        /// </summary>
        public Command signUpCommand
        {
            get
            {
                return new Command((e) =>
                {
                    _navigation.PushAsync(new SignupPage());

                });
            }
        }

        /// <summary>
        /// This tapped event will be used for opening the forgot password page.
        /// </summary>
        public Command forgotPasswordCommand
        {
            get
            {
                return new Command((e) =>
                {
                    _navigation.PushPopupAsync(new ForgotPasswordPage());

                });
            }
        }


        public Command skipLoginCommand
        {
            get
            {
                return new Command((e) =>
                {
                    if (Device.RuntimePlatform == Device.Android)
                    {
                        DependencyService.Get<IStoragePermissions>().GetLocationPermissions();
                    }

                    SaveLoginResponse loginResponse = new SaveLoginResponse();
                    loginResponse.UserName = Email;
                    loginResponse.Password = Password;
                    loginResponse.IsLoggedIn = false;
                    loginResponse.IsLoggedInFacebook = false;
                    loginResponse.FacebookProfilePicture = string.Empty;
                    App.Database.SaveLoggedUser(loginResponse);
                    App.Current.MainPage = new NavigationPage(new HomeMasterPage());

                });
            }
        }

        public Command facebookLoginCommand
        {
            get
            {
                return new Command((e) =>
                {
                    try
                    {
                        if (Device.RuntimePlatform == Device.Android)
                        {
                            _navigation.PushPopupAsync(new LoaderPopup());
                        }

                        DependencyService.Get<IFacebookHelper>().Start(async (resp) =>
                        {
                            if (resp.Code == FacebookResponse.ResponseCode.OK)
                            {
                                try
                                {
                                    var obj = JObject.Parse(resp.Result);
                                    var email = obj["email"].ToString();
                                    var id = obj["id"].ToString();
                                    var first_name = obj["first_name"].ToString();
                                    var last_name = obj["last_name"].ToString();
                                    FacebookProfilePicture profileObj = null;
                                    if (resp.ProfilePic != null)
                                    {
                                        profileObj = JsonConvert.DeserializeObject<FacebookProfilePicture>(resp.ProfilePic);
                                    }
                                    var nvc = new List<KeyValuePair<string, string>>();
                                    nvc.Add(new KeyValuePair<string, string>("full_name", first_name +" " +last_name));
                                    nvc.Add(new KeyValuePair<string, string>("email", email));
                                    nvc.Add(new KeyValuePair<string, string>("facebook_id", id));
                                    nvc.Add(new KeyValuePair<string, string>("thumb_url", profileObj.picture.data.url));
                                    nvc.Add(new KeyValuePair<string, string>("api_key", CommonLib.apiKey));
                                    var result = await cbase.Login(ApiUrl.SocialLoginUrl, nvc);

                                    if (result != null)
                                    {
                                        Device.BeginInvokeOnMainThread(async () =>
                                        {
                                            if (result.status.status_code == "-1")
                                            {
                                                SaveLoginResponse loginResponse = new SaveLoginResponse();
                                                loginResponse.UserId = result.user_info.user_id;
                                                loginResponse.LoginHash = result.user_info.login_hash;
                                                loginResponse.UserName = result.user_info.username;
                                                loginResponse.Password = result.user_info.password;
                                                loginResponse.IsLoggedIn = true;
                                                loginResponse.Email = result.user_info.email;
                                                loginResponse.IsLoggedInFacebook = true;
                                                loginResponse.FullName = result.user_info.full_name;

                                                loginResponse.FacebookProfilePicture =result.user_info.thumb_url;
                                                App.Database.SaveLoggedUser(loginResponse);

                                                LoginUserDetails.UserId = result.user_info.user_id;
                                                LoginUserDetails.LoginHash = result.user_info.login_hash;
                                                LoginUserDetails.UserName = result.user_info.username;
                                                LoginUserDetails.Password = result.user_info.password;
                                                LoginUserDetails.IsLoggedIn = true;
                                                LoginUserDetails.IsLoggedInFacebook = true;
                                                LoginUserDetails.Email = result.user_info.email;
                                                LoginUserDetails.FacebookProfilePicture = result.user_info.thumb_url;
                                                LoginUserDetails.FullName = result.user_info.full_name;
                                                LoaderPopup.CloseAllPopup();

                                                App.Current.MainPage = new NavigationPage(new HomeMasterPage());
                                            }
                                            else
                                            {
                                                if (Device.RuntimePlatform == Device.Android)
                                                {
                                                    LoaderPopup.CloseAllPopup();
                                                }
                                                await App.Current.MainPage.DisplayAlert("", result.status.status_text, "OK");
                                            }
                                        });
                                    }
                                    else
                                    {
                                        if (Device.RuntimePlatform == Device.Android)
                                        {
                                            LoaderPopup.CloseAllPopup();
                                        }
                                        await App.Current.MainPage.DisplayAlert("", CommonLib.someErrorMsg, "OK");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    if (Device.RuntimePlatform == Device.Android)
                                    {
                                        LoaderPopup.CloseAllPopup();
                                    }
                                    await App.Current.MainPage.DisplayAlert("", ex.Message, "Ok");
                                }
                            }
                            else
                            {
                                if (Device.RuntimePlatform == Device.Android)
                                {
                                    LoaderPopup.CloseAllPopup();
                                }
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        if (Device.RuntimePlatform == Device.Android)
                        {
                            LoaderPopup.CloseAllPopup();
                        }
                    }
                });
            }
        }

        /// <summary>
        /// This event will be used for login button functionality.
        /// </summary>
        public Command loginBtnCommand
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
                    else
                    {
                        if (!CommonLib.CheckConnection())
                        {
                            await _navigation.PushPopupAsync(new NoInternetPopup());
                            return;
                        }
                        else
                        {
                            await Login();
                        }
                    }

                });
            }
        }

        public string CheckLoginValidations()
        {
            string msg = string.Empty;
            if (string.IsNullOrEmpty(Email))
            {
                msg += "Please Enter User Name" + Environment.NewLine + Environment.NewLine;
            }
            if (string.IsNullOrEmpty(Password))
            {
                msg += "Please Enter Password" + Environment.NewLine;
            }
            return msg;
        }


        public async Task Login()
        {
            try
            {
                await _navigation.PushPopupAsync(new LoaderPopup());
                var nvc = new List<KeyValuePair<string, string>>();
                nvc.Add(new KeyValuePair<string, string>("username", Email));
                nvc.Add(new KeyValuePair<string, string>("password", Password));
                nvc.Add(new KeyValuePair<string, string>("api_key", CommonLib.apiKey));

                var result = await cbase.Login(ApiUrl.LoginUrl, nvc);
                if (result != null)
                {
                    if (result.status.status_code == "-1")
                    {
                        SaveLoginResponse loginResponse = new SaveLoginResponse();
                        loginResponse.UserId = result.user_info.user_id;
                        loginResponse.LoginHash = result.user_info.login_hash;
                        loginResponse.UserName = result.user_info.username;
                        loginResponse.FullName = result.user_info.full_name;
                        loginResponse.Password = result.user_info.password;
                        loginResponse.IsLoggedIn = true;
                        loginResponse.Email = result.user_info.email;
                        loginResponse.IsLoggedInFacebook = false;
                        loginResponse.FacebookProfilePicture = result.user_info.thumb_url;

                        App.Database.SaveLoggedUser(loginResponse);

                        LoginUserDetails.UserId = result.user_info.user_id;
                        LoginUserDetails.LoginHash = result.user_info.login_hash;
                        LoginUserDetails.UserName = result.user_info.username;
                        LoginUserDetails.Password = result.user_info.password;
                        LoginUserDetails.FullName = result.user_info.full_name;
                        LoginUserDetails.IsLoggedIn = true;
                        LoginUserDetails.Email = result.user_info.email;
                        LoginUserDetails.IsLoggedInFacebook = false;
                        LoginUserDetails.FacebookProfilePicture = result.user_info.thumb_url;
                        LoaderPopup.CloseAllPopup();

                        App.Current.MainPage = new NavigationPage(new HomeMasterPage());
                    }
                    else
                    {
                        LoaderPopup.CloseAllPopup();
                        await App.Current.MainPage.DisplayAlert("", result.status.status_text, "OK");
                    }
                }
                else
                {
                    LoaderPopup.CloseAllPopup();
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
