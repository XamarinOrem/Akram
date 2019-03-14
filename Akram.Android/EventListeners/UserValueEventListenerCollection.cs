using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Firebase.Database;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Akram.Models;
using Xamarin.Forms;
using Java.Util;
using Akram.DependencyInterface;

namespace Akram.Droid
{
    public class UserValueEventListenerCollection : Java.Lang.Object, IValueEventListener
    {
        EventHandler OnChange;

        public UserValueEventListenerCollection(EventHandler OnChange) => this.OnChange = OnChange;

        public void OnCancelled(DatabaseError error)
        {

        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            try
            {
                bool _isDeleted = false;

                if (OnChange != null && snapshot.Value != null && snapshot.Key == "Akram")
                {
                    App.Database.ClearCollectionDetails();

                    var getNotifications = App.Database.GetNotifications();

                    var getUserCollection = snapshot.Child(LoginUserDetails.UserId);
                    if (getUserCollection.Value != null)
                    {
                        var getChild = (JavaDictionary)getUserCollection.Child("Collection")?.GetValue(true);
                        if (getChild != null)
                        {
                            string getKey = string.Empty;
                            foreach (var item in getChild.Keys)
                            {
                                getKey = item.ToString();
                            }
                            var getCollectionKeys = getChild.Keys.Count;
                            var getCollectionValues = getChild.Values.GetEnumerator();
                            while (getCollectionValues.MoveNext())
                            {
                                _isDeleted = false;
                                var item = (JavaDictionary)getCollectionValues.Current;
                                var itemValues = item.Values;
                                String[] collectionValueArray = new String[itemValues.Count];
                                itemValues.CopyTo(collectionValueArray, 0);
                                var itemKeys = item.Keys;
                                String[] collectionKeysArray = new String[itemKeys.Count];
                                itemKeys.CopyTo(collectionKeysArray, 0);
                            
                                SaveCollectionData _collectionData = new SaveCollectionData();
                                for (int l = 0; l < collectionKeysArray.Count(); l++)
                                {
                                    if (collectionKeysArray[l] == "Name")
                                    {
                                        _collectionData.ShopName = collectionValueArray[l];
                                    }
                                    if (collectionKeysArray[l] == "expiry")
                                    {
                                        _collectionData.Date = collectionValueArray[l];
                                    }
                                    if (collectionKeysArray[l] == "Sighting_id")
                                    {
                                        _collectionData.SightingId = collectionValueArray[l];
                                    }
                                    if (collectionKeysArray[l] == "item_id")
                                    {
                                        _collectionData.ItemId = collectionValueArray[l];
                                    }
                                    if (collectionKeysArray[l] == "rules")
                                    {
                                        _collectionData.Rules = collectionValueArray[l];
                                    }
                                    if (collectionKeysArray[l] == "scan")
                                    {
                                        _collectionData.Scan = collectionValueArray[l];
                                    }
                                    if (collectionKeysArray[l] == "Distance")
                                    {
                                        _collectionData.Distance = collectionValueArray[l];
                                    }
                                    if (collectionKeysArray[l] == "Loc")
                                    {
                                        _collectionData.Location = collectionValueArray[l];
                                    }
                                    if (collectionKeysArray[l] == "Type")
                                    {
                                        _collectionData.Type = collectionValueArray[l];
                                    }
                                    _collectionData.Key = getKey;
                                }

                                DateTime _value;
                                if (!string.IsNullOrEmpty(_collectionData.Date))
                                {
                                    if (DateTime.TryParse(_collectionData.Date, out _value))
                                    {
                                        DateTime? expiryDate = DateTime.Parse(_collectionData.Date.Trim());
                                        var compareDate = DateTime.Now;
                                        if (compareDate.Date > expiryDate.Value.Date)
                                        {
                                            _isDeleted = true;
                                            DependencyService.Get<IFirebaseDatabase>().DeleteCollection(_collectionData.Key.Trim(), _collectionData.ItemId.Trim(), LoginUserDetails.UserId);
                                        }
                                    }
                                }

                                if (!_isDeleted)
                                {
                                    App.Database.SaveCollection(_collectionData);
                                    if (_collectionData.Date != null)
                                    {
                                        DateTime value;
                                        if (DateTime.TryParse(_collectionData.Date, out value))
                                        {
                                            var expiryDate = DateTime.Parse(_collectionData.Date);
                                            var compareDate = DateTime.Now.AddDays(-2);
                                            var checkNoOfDays = (expiryDate.Date - compareDate.Date).TotalDays;
                                            if (checkNoOfDays <= 2)
                                            {
                                                foreach (var notifications in getNotifications)
                                                {
                                                    if (!notifications.Title.Contains("Last Day to redeem the gift " + _collectionData.ShopName.Trim()))
                                                    {
                                                        SaveNotificationDetails _notificationDetails = new SaveNotificationDetails();
                                                        _notificationDetails.Title = "Last Day to redeem the gift " + _collectionData.ShopName.Trim();
                                                        App.Database.SaveNotifications(_notificationDetails);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                             
                            }
                        }
                    }
                }
                MessagingCenter.Send(Xamarin.Forms.Application.Current, "LoadCollections", true);
            }
            catch (Exception ex)
            {

            }
        }




        public class UserEventArgs : EventArgs
        {
            public UserEventArgs(bool value)
            {
                this.value = value;
            }

            public bool value { get; set; }
        }


    }
}