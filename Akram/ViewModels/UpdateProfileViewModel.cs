using Akram.DependencyInterface;
using Akram.Models;
using Akram.Repository;
using Akram.Views;
using Plugin.Media;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Akram.ViewModels
{
    public class UpdateProfileViewModel : INotifyPropertyChanged
    {
        public string imageBase64String = string.Empty;
        HttpClientBase cbase = new HttpClientBase();
        public byte[] _imagefile;

        public INavigation _navigation;

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
                        if (Device.RuntimePlatform == Device.iOS)
                        {
                            if (_birthDateSelectedItem.Date.ToString("dd/MM/yyyy") != "01/01/1900")
                            {
                                Birthdate = _birthDateSelectedItem.ToShortDateString();
                            }
                        }
                        else{
                            if (_birthDateSelectedItem.Year.ToString() != "1900")
                            {
                                Birthdate = _birthDateSelectedItem.ToShortDateString();
                            }
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
        private string _username;
        public string UserName
        {
            get
            {
                return _username;
            }
            set
            {
                _username = value;
                PropertyChanged(this, new PropertyChangedEventArgs("UserName"));
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
        public event PropertyChangedEventHandler PropertyChanged;

        public UpdateProfileViewModel(INavigation navigation, ProfileResult profileResult)
         {
            _navigation = navigation;
            _email = profileResult.email;
            if(profileResult.phone.Contains("+961"))
            {
                profileResult.phone= profileResult.phone.Replace(@"+961","");
            }
            if (profileResult.phone.Contains("+962"))
            {
                profileResult.phone=profileResult.phone.Replace(@"+962", "");
            }
            if (profileResult.phone.Contains("+971"))
            {
                profileResult.phone=profileResult.phone.Replace(@"+971", "");
            }
            _phoneNumber = profileResult.phone.Trim();
            _birthDate = profileResult.dob;
            _gender = profileResult.gender;
            _facebook = string.IsNullOrEmpty(profileResult.facebook_id) ? "Not Linked" : "Linked";
            _fullname = string.IsNullOrEmpty(profileResult.full_name) ? "Full Name" : CommonLib.FirstCharToUpper(profileResult.full_name);
            _username = string.IsNullOrEmpty(profileResult.username) ? "User Name" : CommonLib.FirstCharToUpper(profileResult.username);
            _profileImgSource = string.IsNullOrEmpty(profileResult.thumb_url) ? "ic_drawer_profile_default.png" : profileResult.thumb_url;
        }


        public Command BackTapped
        {
            get
            {
                return new Command((e) =>
                {
                    _navigation.PopAsync();
                });
            }
        }

        public Command updateBtnCommand
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

                            await Update();
                        }
                    }
                });
            }
        }
        public string CheckValidations()
        {
            string msg = string.Empty;

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



            return msg;
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
                    catch (Exception ex)
                    {

                    }
                });
            }
        }




        public async Task Update()
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


                multipartContent.Add(new StringContent(FullName), "full_name");
                multipartContent.Add(new StringContent(UserName), "user_name");
                multipartContent.Add(new StringContent(Email), "email");
                multipartContent.Add(new StringContent(LoginUserDetails.UserId), "user_id");
                multipartContent.Add(new StringContent(LoginUserDetails.LoginHash), "login_hash");
                multipartContent.Add(new StringContent(Gender), "gender");
                multipartContent.Add(new StringContent(CommonLib.apiKey), "api_key");
                multipartContent.Add(new StringContent(PhoneNumber), "phone");
                multipartContent.Add(new StringContent(Birthdate), "dob");

                HttpClient httpClient = new HttpClient();
                HttpResponseMessage response = await httpClient.PostAsync(ApiUrl.UpdateProfile, multipartContent);

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
                            loginResponse.IsLoggedIn = true;
                            loginResponse.IsLoggedInFacebook = false;
                            loginResponse.Email = result.user_info.email;
                            loginResponse.FullName = result.user_info.full_name;
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

                            await App.Current.MainPage.DisplayAlert("", "Profile Update Successful", "Ok");
                            ViewProfilePage.IsLoad = true;
                            await _navigation.PopAsync();
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
            catch
            {
                LoaderPopup.CloseAllPopup();
            }
        }

    }
}
