using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Akram.Models
{
    public class NotificationModel : INotifyPropertyChanged
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }


    public class SaveNotificationDetails
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }
    }
}
