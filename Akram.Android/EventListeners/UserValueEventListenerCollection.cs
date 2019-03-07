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
                            var getCollectionValues = getChild.Values.GetEnumerator();
                            while (getCollectionValues.MoveNext())
                            {
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
                                }
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