using Akram.DependencyInterface;
using Akram.Models;
using Akram.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;

namespace Akram.ViewModels
{
    public class MyCollectionViewModel : INotifyPropertyChanged
    {
        public INavigation _navigation;

        ObservableCollection<MyCollectionModel> obj;

        private ObservableCollection<MyCollectionModel> _myCollectionList;
        public ObservableCollection<MyCollectionModel> MyCollectionList
        {
            get
            {
                return _myCollectionList;

            }
            set
            {
                _myCollectionList = value;
                OnPropertyChanged();
            }
        }


        private MyCollectionModel _selectedItem;
        public MyCollectionModel SelectedItem
        {
            get
            {
                return _selectedItem;

            }
            set
            {
                _selectedItem = value;
                if (_selectedItem != null)
                {
                    OnPropertyChanged();
                    ShowDetailPage();
                }
            }
        }

        private double _height;
        public double Height
        {
            get
            {
                return _height;

            }
            set
            {
                _height = value;
                OnPropertyChanged();
            }
        }

        private bool _vipVisible;
        public bool VipVisible
        {
            get
            {
                return _vipVisible;

            }
            set
            {
                _vipVisible = value;
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


        public event PropertyChangedEventHandler PropertyChanged;

        public MyCollectionViewModel(INavigation navigation)
        {
            _navigation = navigation;

            obj = new ObservableCollection<MyCollectionModel>();

            var getData = App.Database.GetCollections();

            if (getData.Count > 0)
            {
                NoDataVisible = false;
                try
                {
                    for (int i = 0; i < getData.Count; i++)
                    {
                        DateTime value;
                        if (!string.IsNullOrEmpty(getData[i].Date))
                        {

                            if (DateTime.TryParse(getData[i].Date, out value))
                            {
                                DateTime? expiryDate = DateTime.Parse(getData[i].Date.Trim());
                                var compareDate = DateTime.Now;
                                if (compareDate.Date > expiryDate.Value.Date)
                                {
                                    DependencyService.Get<IFirebaseDatabase>().DeleteCollection(getData[i].SightingId.Trim(), getData[i].ItemId.Trim(), LoginUserDetails.UserId);
                                }
                            }
                        }

                        string Date = !string.IsNullOrEmpty(getData[i].Date) ? getData[i].Date.Trim() : string.Empty;

                        MyCollectionModel myModel = new MyCollectionModel();
                        myModel.ItemId = getData[i].ItemId;
                        myModel.Date = Date;
                        myModel.Scan = getData[i].Scan;
                        myModel.Rules = getData[i].Rules;
                        myModel.Item = getData[i].ShopName;
                        myModel.SightingId = getData[i].SightingId;
                        myModel.DateColor = !string.IsNullOrEmpty(Date) ? DateTime.TryParse(getData[i].Date, out value) ? Convert.ToDateTime(Date).Date == DateTime.Now.Date ? Color.Red : Color.FromHex("#379a5f") : Color.FromHex("#379a5f") : Color.FromHex("#379a5f");
                        obj.Add(myModel);
                    }
                }
                catch (Exception ex)
                {

                }

                MyCollectionList = obj;
                Height = obj.Count * 84;
                RaisePropertyChanged(nameof(MyCollectionList));
            }
            else
            {
                NoDataVisible = true;
                Height = 0;
            }

            VipVisible = false;
        }

        void ShowDetailPage()
        {
            if (Device.RuntimePlatform == Device.Android)
            {
                DependencyService.Get<IStoragePermissions>().GetLocationPermissions();
            }
            _navigation.PushAsync(new GiftPage(SelectedItem));


            obj = new ObservableCollection<MyCollectionModel>();

            var getData = App.Database.GetCollections();

            if (getData.Count > 0)
            {
                NoDataVisible = false;
                try
                {
                    DateTime value;

                    for (int i = 0; i < getData.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(getData[i].Date))
                        {
                            if (DateTime.TryParse(getData[i].Date, out value))
                            {
                                DateTime? expiryDate = DateTime.Parse(getData[i].Date.Trim());
                                var compareDate = DateTime.Now;
                                if (compareDate.Date > expiryDate.Value.Date)
                                {
                                    DependencyService.Get<IFirebaseDatabase>().DeleteCollection(getData[i].SightingId.Trim(), getData[i].ItemId.Trim(), LoginUserDetails.UserId);
                                }
                            }
                        }

                        string Date = !string.IsNullOrEmpty(getData[i].Date) ? getData[i].Date.Trim() : string.Empty;
                        obj.Add(new MyCollectionModel
                        {
                            ItemId = getData[i].ItemId,
                            Date = Date,
                            Scan = getData[i].Scan,
                            Rules = getData[i].Rules,
                            Item = getData[i].ShopName,
                            SightingId = getData[i].SightingId,
                            DateColor = !string.IsNullOrEmpty(Date) ? DateTime.TryParse(getData[i].Date, out value) ? Convert.ToDateTime(Date) == DateTime.Now ? Color.Red : Color.FromHex("#379a5f") : Color.FromHex("#379a5f") : Color.FromHex("#379a5f"),
                        });
                    }
                }
                catch (Exception ex)
                {

                }

                MyCollectionList = obj;
                Height = obj.Count * 84;
                RaisePropertyChanged(nameof(MyCollectionList));
            }
            else
            {
                NoDataVisible = true;
                Height = 0;
            }

            VipVisible = false;
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
