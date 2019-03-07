using Akram.Models;
using Akram.Repository;
using Akram.Views;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;

namespace Akram.ViewModels
{
    public class ViewProfileViewModel : INotifyPropertyChanged
    {
        ProfileResult result;
        public INavigation _navigation;
        HttpClientBase cbase = new HttpClientBase();

        public event PropertyChangedEventHandler PropertyChanged;


        private string _image;
        public string Image
        {
            get
            {
                return _image;
            }
            set
            {
                _image = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Image"));
            }
        }
        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Name"));
            }
        }
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
        private string _phone;
        public string Phone
        {
            get
            {
                return _phone;
            }
            set
            {
                _phone = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Phone"));
            }
        }
        private string _gender;
        public string Gender
        {
            get
            {
                return _gender;
            }
            set
            {
                _gender = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Gender"));
            }
        }
        private string _gDate;
        public string GDate
        {
            get
            {
                return _gDate;
            }
            set
            {
                _gDate = value;
                PropertyChanged(this, new PropertyChangedEventArgs("GDate"));
            }
        }
        private string _facebook;
        public string Facebook
        {
            get
            {
                return _facebook;
            }
            set
            {
                _facebook = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Facebook"));
            }
        }
        private string _fullname;
        public string FullName
        {
            get
            {
                return _fullname;
            }
            set
            {
                _fullname = value;
                PropertyChanged(this, new PropertyChangedEventArgs("FullName"));
            }
        }

        private bool _loadingVisible;
        public bool LoadingVisible
        {
            get
            {
                return _loadingVisible;

            }
            set
            {
                _loadingVisible = value;
                OnPropertyChanged();
            }
        }


        private bool _noInternetVisible;
        public bool NoInternetVisible
        {
            get
            {
                return _noInternetVisible;

            }
            set
            {
                _noInternetVisible = value;
                OnPropertyChanged();
            }
        }

        private bool _mainListVisible;
        public bool MainListVisible
        {
            get
            {
                return _mainListVisible;

            }
            set
            {
                _mainListVisible = value;
                OnPropertyChanged();
            }
        }

        public ViewProfileViewModel(INavigation navigation)
        {
            _navigation = navigation;
            _noInternetVisible = false;
            _loadingVisible = false;
            _mainListVisible = false;
            load();
        }

        public async void load()
        {
            try
            {
                if (!CommonLib.CheckConnection())
                {
                    NoInternetVisible = true;
                    LoadingVisible = false;
                    MainListVisible = false;
                }
                else
                {
                    NoInternetVisible = false;
                    LoadingVisible = true;
                    MainListVisible = false;

                    string postData = "api_key=" + CommonLib.apiKey + "&userid=" + LoginUserDetails.UserId;
                    result = await cbase.GetProfileData(ApiUrl.ViewProfile + postData);

                    if (result != null)
                    {
                        if (!string.IsNullOrEmpty(result.phone))
                        {
                            if (result.phone.Contains("+961"))
                            {
                                result.phone = result.phone.Replace(@"+961", "");
                            }
                            if (result.phone.Contains("+962"))
                            {
                                result.phone = result.phone.Replace(@"+962", "");
                            }
                            if (result.phone.Contains("+971"))
                            {
                                result.phone = result.phone.Replace(@"+971", "");
                            }
                        }

                        Image = string.IsNullOrEmpty(result.thumb_url) ? "ic_drawer_profile_default.png" : result.thumb_url;
                        Email = result.email;
                        GDate = result.dob;
                        Gender = result.gender;
                        Name = string.IsNullOrEmpty(result.username) ? "User Name" :CommonLib.FirstCharToUpper(result.username);
                        Phone = result.phone;
                        Facebook = string.IsNullOrEmpty(result.facebook_id)? "Not Linked" : "Linked";
                        FullName = string.IsNullOrEmpty(result.full_name) ? "Full Name" : CommonLib.FirstCharToUpper(result.full_name);

                        NoInternetVisible = false;
                        LoadingVisible = false;
                        MainListVisible = true;
                    }

                }
            }
            catch
            {

            }
        }
        public Command BackTapped
        {
            get
            {
                return new Command((e) =>
                {
                    bool doesPageExists = _navigation.NavigationStack.Any(p => p is ChangePasswordPage);
                    if(doesPageExists)
                    {
                        _navigation.PopPopupAsync();
                    }
                    _navigation.PopAsync();
                });
            }
        }

        public Command changePasswordCommand
        {
            get
            {
                return new Command((e) =>
                {
                    _navigation.PushPopupAsync(new ChangePasswordPage());
                });
            }
        }

        public Command editCommand
        {
            get
            {
                return new Command((e) =>
                {
                    _navigation.PushAsync(new UpdateProfilePage(result));
                });
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
