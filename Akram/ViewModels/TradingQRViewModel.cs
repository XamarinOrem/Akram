using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;

namespace Akram.ViewModels
{
    public class TradingQRViewModel : INotifyPropertyChanged
    {
        public INavigation _navigation;


        public event PropertyChangedEventHandler PropertyChanged;

        public TradingQRViewModel(INavigation navigation)
        {
            _navigation = navigation;

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
        public Command scan
        {
            get
            {
                return new Command((e) =>
                {

                });
            }
        }
    }
}
