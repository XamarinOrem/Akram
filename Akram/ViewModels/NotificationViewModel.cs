using Akram.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace Akram.ViewModels
{
    public class NotificationViewModel : INotifyPropertyChanged
    {
        public INavigation _navigation;

        ObservableCollection<NotificationModel> obj;

        private ObservableCollection<NotificationModel> _notificationList;
        public ObservableCollection<NotificationModel> NotificationList
        {
            get
            {
                return _notificationList;

            }
            set
            {
                _notificationList = value;
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

        private bool _mainDataVisible;
        public bool MainDataVisible
        {
            get
            {
                return _mainDataVisible;

            }
            set
            {
                _mainDataVisible = value;
                OnPropertyChanged();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public NotificationViewModel(INavigation navigation)
        {
            _navigation = navigation;

            obj = new ObservableCollection<NotificationModel>();

            var getData = App.Database.GetNotifications();

            if (getData.Count > 0)
            {
                NoDataVisible = false;
                for (int i = 0; i < getData.Count; i++)
                {
                    obj.Add(new NotificationModel
                    {
                        Title = getData[i].Title,
                        Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Suspendisse molestie, tortor quis vestibulum dictum, odio nunc cursus leo, at efficitur erat nulla vel sem. In volutpat sodales mi"
                    });
                }
                MainDataVisible = true;
                
                NotificationList = obj;

                RaisePropertyChanged(nameof(NotificationList));
            }
            else
            {
                NoDataVisible = true;
                MainDataVisible = false;
            }
        }

        public Command back
        {
            get
            {
                return new Command((e) =>
                {
                    _navigation.PopAsync();

                });
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
