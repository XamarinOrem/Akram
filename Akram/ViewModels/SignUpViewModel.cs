using System;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Akram.DependencyInterface;
using Akram.Models;
using Akram.Repository;
using Akram.Views;
using Plugin.Media;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms;

namespace Akram.ViewModels
{
    public class SignUpViewModel : INotifyPropertyChanged
    {
        HttpClientBase cbase = new HttpClientBase();

        public INavigation _navigation;

        public string imageBase64String = string.Empty;

        public byte[] _imagefile;

        /// <summary>
        /// property for the name field
        /// </summary>
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

        /// <summary>
        /// property for the user name field
        /// </summary>
        private string _userName;
        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value;
                PropertyChanged(this, new PropertyChangedEventArgs("UserName"));
            }
        }

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

        /// <summary>
        /// property for the confirm password field
        /// </summary>
        private string _confirmPassword;
        public string ConfirmPassword
        {
            get
            {
                return _confirmPassword;
            }
            set
            {
                _confirmPassword = value;
                PropertyChanged(this, new PropertyChangedEventArgs("ConfirmPassword"));
            }
        }

        /// <summary>
        /// property for the confirm password field
        /// </summary>
        private string _phoneNumber;
        public string PhoneNumber
        {
            get
            {
                return _phoneNumber;
            }
            set
            {
                _phoneNumber = value;
                PropertyChanged(this, new PropertyChangedEventArgs("PhoneNumber"));
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

        private string _phoneCode;
        public string PhoneCode
        {
            get
            {
                return _phoneCode;
            }
            set
            {
                _phoneCode = value;
                PropertyChanged(this, new PropertyChangedEventArgs("PhoneCode"));
            }
        }

        private string _birthDate;
        public string Birthdate
        {
            get
            {
                return _birthDate;
            }
            set
            {
                _birthDate = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Birthdate"));
            }
        }

        private ImageSource _profileImgSource;
        public ImageSource ProfileImgSource
        {
            get
            {
                return _profileImgSource;
            }
            set
            {
                _profileImgSource = value;
                PropertyChanged(this, new PropertyChangedEventArgs("ProfileImgSource"));
            }
        }

        private Color _genderColor;
        public Color GenderColor
        {
            get
            {
                return _genderColor;
            }
            set
            {
                _genderColor = value;
                PropertyChanged(this, new PropertyChangedEventArgs("GenderColor"));
            }
        }

        private Color _phoneCodeColor;
        public Color PhoneCodeColor
        {
            get
            {
                return _phoneCodeColor;
            }
            set
            {
                _phoneCodeColor = value;
                PropertyChanged(this, new PropertyChangedEventArgs("PhoneCodeColor"));
            }
        }

        private Color _dateColor;
        public Color DateColor
        {
            get
            {
                return _dateColor;
            }
            set
            {
                _dateColor = value;
                PropertyChanged(this, new PropertyChangedEventArgs("DateColor"));
            }
        }

        private string _genderSelectedItem;

        public string GenderPicker_SelectedItem
        {
            get => _genderSelectedItem;
            set
            {
                if (value != null)
                {
                    SetProperty(ref _genderSelectedItem, value);
                    if (_genderSelectedItem != null)
                    {
                        Gender = _genderSelectedItem;
                        GenderColor = Color.Black;
                    }
                }
            }
        }

        private string _phoneCodeSelectedItem;

        public string PhoneCodePicker_SelectedItem
        {
            get => _phoneCodeSelectedItem;
            set
            {
                if (value != null)
                {
                    SetProperty(ref _phoneCodeSelectedItem, value);
                    if (_phoneCodeSelectedItem != null)
                    {
                        PhoneCode = _phoneCodeSelectedItem;
                        PhoneCodeColor = Color.Black;
                    }
                }
            }
        }

        private DateTime _birthDateSelectedItem;

        public DateTime BirthDatePicker_SelectedItem
        {
            get => _birthDateSelectedItem;
            set
            {
                if (value != null)
                {
                    SetProperty(ref _birthDateSelectedItem, value);
                    if (_birthDateSelectedItem != null)
                    {
                        if (_birthDateSelectedItem.ToShortDateString() != "01/01/1900")
                        {
                            Birthdate = _birthDateSelectedItem.ToShortDateString();
                            DateColor = Color.Black;
                        }
                        else
                        {
                            Birthdate = "Birthdate";
                            DateColor = Color.Gray;
                        }
                    }
                }
            }
        }

        public bool SetProperty<T>(ref T storage, T value,
            [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public SignUpViewModel(INavigation navigation)
        {
            _navigation = navigation;
            _birthDate = "Birthdate";
            _gender = "Choose Gender";
            _phoneCode = "+961";
            _profileImgSource = "add_image.png";
            _dateColor = Color.Gray;
            _genderColor = Color.Gray;
            _phoneCodeColor = Color.Gray;
        }

        /// <summary>
        /// This tapped event will be used for opening the login page.
        /// </summary>
        public Command loginCommand
        {
            get
            {
                return new Command((e) =>
                {
                    _navigation.PopAsync(); ;

                });
            }
        }

        /// <summary>
        /// This event will be used for sign up functionality.
        /// </summary>
        public Command signUpCommand
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
                            TermsAndConditions.displayBtn = true;
                            await Register();
                        }
                    }
                });
            }
        }


        public Command chooseImgCommand
        {
            get
            {
                return new Command(async (e) =>
                {
                    try
                    {
                        if (Device.RuntimePlatform == Device.Android)
                        {
                            DependencyService.Get<IStoragePermissions>().GetContactPermissions();
                        }

                        var action = await App.Current.MainPage.DisplayActionSheet("Choose Image", "Cancel", null,
                           "Take Picture", "Choose From Gallery");
                        string _selectedColor = string.Empty;
                        if (action == "Take Picture")
                        {
                            await CrossMedia.Current.Initialize();
                            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsPickPhotoSupported)
                            {
                                return;
                            }
                            if (Device.RuntimePlatform == "iOS")
                            {
                                await Task.Delay(1000);
                            }
                            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                            {
                                Directory = "Sample",
                                Name = "test.jpg"
                            });
                            if (file == null)
                                return;
                            using (var memoryStream = new MemoryStream())
                            {
                                file.GetStream().CopyTo(memoryStream);
                                var myfile = memoryStream.ToArray();
                                _imagefile = myfile;
                                file.Dispose();
                            }
                            Stream stream = new MemoryStream(_imagefile);
                            imageBase64String = CommonLib.ConvertToBase64(stream);
                            ProfileImgSource = ImageSource.FromStream(() => new MemoryStream(_imagefile));
                        }
                        else if (action == "Choose From Gallery")
                        {
                            await CrossMedia.Current.Initialize();
                            if (!CrossMedia.Current.IsPickPhotoSupported)
                            {
                                return;
                            }
                            var file = await CrossMedia.Current.PickPhotoAsync();
                            if (file != null)
                            {
                                using (var memoryStream = new MemoryStream())
                                {
                                    file.GetStream().CopyTo(memoryStream);
                                    var myfile = memoryStream.ToArray();
                                    _imagefile = myfile;
                                    file.Dispose();
                                }
                                Stream stream = new MemoryStream(_imagefile);
                                imageBase64String = CommonLib.ConvertToBase64(stream);
                                ProfileImgSource = ImageSource.FromStream(() => new MemoryStream(_imagefile));
                            }
                        }
                        else
                        {
                            return;
                        }

                    }
                    catch (Exception exx)
                    {

                    }
                });
            }
        }

        public string CheckValidations()
        {
            string msg = string.Empty;
            if (string.IsNullOrEmpty(Name))
            {
                msg += "Please Enter Full Name" + Environment.NewLine + Environment.NewLine;
            }
            if (string.IsNullOrEmpty(UserName))
            {
                msg += "Please Enter User Name" + Environment.NewLine + Environment.NewLine;
            }
            if (!string.IsNullOrEmpty(UserName))
            {
                if (!CommonLib.CheckValidUserName(UserName))
                {
                    msg += "Please Enter User Name without white spaces and special characters" + Environment.NewLine + Environment.NewLine;
                }
            }
            if (string.IsNullOrEmpty(Email))
            {
                msg += "Please Enter Email" + Environment.NewLine + Environment.NewLine;
            }
            if (!string.IsNullOrEmpty(Email))
            {
                if (!CommonLib.CheckValidEmail(Email))
                {
                    msg += "Please Enter Valid Email" + Environment.NewLine + Environment.NewLine;
                }
            }
            if (string.IsNullOrEmpty(PhoneNumber))
            {
                msg += "Please Enter Phone Number" + Environment.NewLine + Environment.NewLine;
            }

            if (!string.IsNullOrEmpty(PhoneNumber))
            {
                if (PhoneNumber.Length <= 8 || PhoneNumber.Length > 12)
                {
                    msg += "Please Enter Valid Phone Number" + Environment.NewLine + Environment.NewLine;
                }
            }

            if (string.IsNullOrEmpty(Password))
            {
                msg += "Please Enter Password" + Environment.NewLine + Environment.NewLine;
            }
            if (!string.IsNullOrEmpty(Password))
            {
                if (Password.Length < 6)
                {
                    msg += "Password Length should be 6 or more" + Environment.NewLine + Environment.NewLine;
                }
            }
            if (!string.IsNullOrEmpty(Password))
            {
                if (Password != ConfirmPassword)
                {
                    msg += "Password and Confirm Password Not Matched";
                }
            }
            return msg;
        }

        public async Task Register()
        {
            try
            {
                await _navigation.PushPopupAsync(new LoaderPopup());

                string boundary = "---8d0f01e6b3b5dafaaadaad";
                MultipartFormDataContent multipartContent = new MultipartFormDataContent(boundary);

                byte[] bitmapData;
                try
                {
                    if (_imagefile != null)
                    {
                        StreamImageSource streamImageSource = (StreamImageSource)ProfileImgSource;
                        System.Threading.CancellationToken cancellationToken = System.Threading.CancellationToken.None;
                        System.Threading.Tasks.Task<System.IO.Stream> task =
                            streamImageSource.Stream(cancellationToken);
                        System.IO.Stream stream = task.Result;

                        bitmapData = CommonLib.ReadFully(stream);
                        var fileContent = new ByteArrayContent(bitmapData);

                        fileContent.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/octet-stream");
                        fileContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
                        {
                            Name = "thumb_url",
                            FileName = "user" + ".jpeg",
                        };

                        multipartContent.Add(fileContent);
                    }
                }
                catch { }


                multipartContent.Add(new StringContent(Name.Trim()), "full_name");
                multipartContent.Add(new StringContent(UserName.Trim()), "user_name");
                multipartContent.Add(new StringContent(Email.Trim()), "email");
                multipartContent.Add(new StringContent(Password.Trim()), "password");
                multipartContent.Add(new StringContent(Gender.Trim()), "gender");
                multipartContent.Add(new StringContent(CommonLib.apiKey), "api_key");
                multipartContent.Add(new StringContent(PhoneCode.Trim() +" "+ PhoneNumber.Trim()), "phone");
                multipartContent.Add(new StringContent(Birthdate.Trim()), "dob");

                HttpClient httpClient = new HttpClient();
                HttpResponseMessage response = await httpClient.PostAsync(ApiUrl.RegisterUrl, multipartContent);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var result = Newtonsoft.Json.JsonConvert.DeserializeObject<wsUser>(content);
                    if (result != null)
                    {
                        if (result.status.status_code == "-1")
                        {
                            SaveLoginResponse loginResponse = new SaveLoginResponse();
                            loginResponse.UserId = result.user_info.user_id;
                            loginResponse.LoginHash = result.user_info.login_hash;
                            loginResponse.UserName = result.user_info.username;
                            loginResponse.Password = result.user_info.password;
                            loginResponse.IsLoggedInFacebook = false;
                            loginResponse.IsLoggedIn = true;
                            loginResponse.FullName = result.user_info.full_name;
                            loginResponse.Email = result.user_info.email;
                            loginResponse.FacebookProfilePicture = result.user_info.thumb_url;
                            App.Database.SaveLoggedUser(loginResponse);

                            LoginUserDetails.UserId = result.user_info.user_id;
                            LoginUserDetails.LoginHash = result.user_info.login_hash;
                            LoginUserDetails.UserName = result.user_info.username;
                            LoginUserDetails.Password = result.user_info.password;
                            LoginUserDetails.IsLoggedIn = true;
                            LoginUserDetails.IsLoggedInFacebook = false;
                            LoginUserDetails.FacebookProfilePicture = result.user_info.thumb_url;
                            LoginUserDetails.Email = result.user_info.email;
                            LoginUserDetails.FullName = result.user_info.full_name;
                            LoaderPopup.CloseAllPopup();

                            await App.Current.MainPage.DisplayAlert("", "Registration Successful", "Ok");
                            await _navigation.PushAsync(new TermsAndConditions());
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
            }
            catch (Exception ex)
            {
                LoaderPopup.CloseAllPopup();
            }
        }
    }
}
