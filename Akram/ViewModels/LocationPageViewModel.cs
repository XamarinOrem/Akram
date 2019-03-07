using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;

namespace Akram.ViewModels
{
    public class LocationPageViewModel : INotifyPropertyChanged
    {
        public INavigation _navigation;


        public event PropertyChangedEventHandler PropertyChanged;

        public LocationPageViewModel(INavigation navigation)
        {
            _navigation = navigation;

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
    }
}
