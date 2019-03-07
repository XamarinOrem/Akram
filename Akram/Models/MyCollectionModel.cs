using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;

namespace Akram.Models
{
    public class MyCollectionModel: INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string Rules { get; set; }
        public string Item { get; set; }
        public string Date { get; set; }
        public string Scan { get; set; }
        public string SightingId { get; set; }
        public Color DateColor { get; set; }
        public string ItemId { get; set; }
        private string _imgRadio;

        public string imgRadio
        {
            get { return _imgRadio; }
            set
            {
                _imgRadio = value;

                OnPropertyChanged(); // Notify, that Image has been changed
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class SaveCollectionData
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public string ShopName { get; set; }

        public string ItemId { get; set; }

        public string Date { get; set; }

        public string SightingId { get; set; }

        public string Rules { get; set; }

        public string Scan { get; set; }
    }
}
