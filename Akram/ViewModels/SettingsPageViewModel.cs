using Akram.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;

namespace Akram.ViewModels
{
    public class SettingsPageViewModel : INotifyPropertyChanged
    {
        public INavigation _navigation;


        public event PropertyChangedEventHandler PropertyChanged;

        public SettingsPageViewModel(INavigation navigation)
        {
            _navigation = navigation;

        }


        public Command ProfileCommand
        {
            get
            {
                return new Command((e) =>
                {
                    _navigation.PushAsync(new ViewProfilePage());
                });
            }
        }
        public Command LocationCommand
        {
            get
            {
                return new Command( (e) =>
                {
                    _navigation.PushAsync(new LocationPage());
                });
            }
        }
    }
}
