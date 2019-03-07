using Akram.Models;
using Akram.Repository;
using Akram.Views;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Akram.ViewModels
{
    public class MyIntrestViewModel : INotifyPropertyChanged
    {
        public INavigation _navigation;
        public List<int> listCategoryId = new List<int>();
        public string categoryId = string.Empty;
        HttpClientBase cbase = new HttpClientBase();
        ObservableCollection<MyCollectionModel> obj;
        private ObservableCollection<MyCollectionModel> _myIntersetList;
        public ObservableCollection<MyCollectionModel> MyInterestList
        {
            get
            {
                return _myIntersetList;

            }
            set
            {
                _myIntersetList = value;
                OnPropertyChanged();
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

        private bool _noDataVisible;
        public bool NoDataVisible
        {
            get
            {
                return _noDataVisible;

            }
            set
            {
                _noDataVisible = value;
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


        public event PropertyChangedEventHandler PropertyChanged;

        public MyIntrestViewModel(INavigation navigation)
        {
            _navigation = navigation;
            _noDataVisible = false;
            _noInternetVisible = false;
            _loadingVisible = false;
            _mainListVisible = false;
            ShowSelectedInterest();
        }

        public async void Load()
        {
            try
            {
                if (!CommonLib.CheckConnection())
                {
                    NoDataVisible = false;
                    NoInternetVisible = true;
                    LoadingVisible = false;
                    MainListVisible = false;
                }
                else
                {
                    NoDataVisible = false;
                    NoInternetVisible = false;
                    LoadingVisible = true;
                    MainListVisible = false;

                    string postData = "api_key=" + CommonLib.apiKey;

                    var result = await cbase.GetIntrest(ApiUrl.GetCategory, postData);
                    if (result.Count > 0)
                    {
                        obj = new ObservableCollection<MyCollectionModel>();
                        foreach (var item in result)
                        {
                            string _image = string.Empty;
                            if (listCategoryId.Count > 0)
                            {
                                _image = listCategoryId.Contains(Convert.ToInt32(item.id)) ? "ic_radio_selected.png" : "ic_radio.png";
                            }
                            else
                            {
                                _image = "ic_radio.png";
                            }
                            obj.Add(new MyCollectionModel
                            {
                                Id = Convert.ToInt32(item.id),
                                Item = CommonLib.FirstCharToUpper(item.name),
                                imgRadio = _image
                            });
                        }

                        MyInterestList = obj;
                        RaisePropertyChanged(nameof(MyInterestList));
                        NoDataVisible = false;
                        NoInternetVisible = false;
                        LoadingVisible = false;
                        MainListVisible = true;
                    }
                    else
                    {
                        NoDataVisible = true;
                        NoInternetVisible = false;
                        LoadingVisible = false;
                        MainListVisible = false;
                    }
                }
            }
            catch
            {

            }
        }

        public async void ShowSelectedInterest()
        {
            try
            {
                if (!CommonLib.CheckConnection())
                {
                    NoInternetVisible = true;
                    LoadingVisible = false;
                }
                else
                {
                    NoInternetVisible = false;
                    LoadingVisible = true;

                    var nvc = new List<KeyValuePair<string, string>>();
                    nvc.Add(new KeyValuePair<string, string>("user_id", LoginUserDetails.UserId));
                    nvc.Add(new KeyValuePair<string, string>("api_key", CommonLib.apiKey));

                    var result = await cbase.GetSelectedIntrest(ApiUrl.GetCategory, nvc);
                    if (result.Count > 0)
                    {
                        foreach (var item in result)
                        {
                            listCategoryId.Add(Convert.ToInt32(item.category_id));
                        }
                        categoryId = string.Join(",", listCategoryId);
                        NoInternetVisible = false;
                        LoadingVisible = false;
                    }
                    Load();
                }
            }
            catch
            {

            }
        }

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

        public Command doneCommand
        {
            get
            {
                return new Command(async (e) =>
                {
                    if (string.IsNullOrEmpty(categoryId))
                    {
                        await _navigation.PushPopupAsync(new ShowMessage("Please select interest first"));
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
                            await SendInterest();
                        }
                    }
                });
            }
        }

        /// <summary>
        /// method that will change the source of the checkbox image and will add or remove the data from the 
        /// list based on selection or unselection.
        /// </summary>
        public Command radio_Tapped
        {
            get
            {
                return new Command((e) =>
                {
                    MyCollectionModel _model = e as MyCollectionModel;
                    var index = MyInterestList.ToList().FindIndex(x => x.Id == _model.Id);

                    if (_model.imgRadio == "ic_radio.png")
                    {
                        // changing the checkbox image
                        MyInterestList[index].imgRadio = "ic_radio_selected.png";
                        if (!listCategoryId.Contains(_model.Id))
                        {
                            listCategoryId.Add(_model.Id);
                        }
                        categoryId = string.Join(",", listCategoryId);
                    }
                    else
                    {
                        // changing the checkbox image
                        MyInterestList[index].imgRadio = "ic_radio.png";
                        listCategoryId.Remove(_model.Id);
                        categoryId = string.Join(",", listCategoryId);
                    }
                    RaisePropertyChanged(nameof(MyInterestList));

                });
            }
        }

        public async Task SendInterest()
        {
            try
            {
                await _navigation.PushPopupAsync(new LoaderPopup());

                var nvc = new List<KeyValuePair<string, string>>();
                nvc.Add(new KeyValuePair<string, string>("user_id", LoginUserDetails.UserId));
                nvc.Add(new KeyValuePair<string, string>("api_key", CommonLib.apiKey));
                nvc.Add(new KeyValuePair<string, string>("login_hash", LoginUserDetails.LoginHash));

                //Loop over String array and add all instances to our bodyPoperties
                foreach (var CatId in listCategoryId)
                {
                    nvc.Add(new KeyValuePair<string, string>("categ_ids[]", CatId.ToString()));
                }

                var result = await cbase.SendInterest(ApiUrl.SendInterest, nvc);
                if (result != null)
                {
                    if (result.status.status_code == "-1")
                    {
                        await _navigation.PopPopupAsync();
                        await App.Current.MainPage.DisplayAlert("", "Saved Successfully", "Ok");
                        await _navigation.PopPopupAsync();
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

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
