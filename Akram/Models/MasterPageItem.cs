using Plugin.Geolocator.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Akram.Models
{
    public class MasterPageItem : INotifyPropertyChanged
    {
        // event handler for updating the list views
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
        }
        public string Title { get; set; }

        public string Icon { get; set; }

        public Type TargetType { get; set; }
    }


    public class SendPosition
    {
        public Position position { get; set; }

        public string pokeman_id { get; set; }
    }
}
