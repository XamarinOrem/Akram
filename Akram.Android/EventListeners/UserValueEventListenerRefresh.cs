using Akram.Models;
using Akram.Views;
using Android.Runtime;
using Firebase.Database;
using Java.Util;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Xamarin.Forms;

namespace Akram.Droid
{
    public class UserValueEventListenerRefresh : Java.Lang.Object, IValueEventListener
    {
        EventHandler OnChange;

        public UserValueEventListenerRefresh(EventHandler OnChange) => this.OnChange = OnChange;

        public void OnCancelled(DatabaseError error)
        {

        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            //HomeMapPage.AllItemIds = new List<string>();
            try
            {
                if (OnChange != null && snapshot.Value != null && snapshot.Key == "Akram")
                {
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
                                    if (collectionKeysArray[l] == "item_id")
                                    {
                                        HomeMapPage.AllItemIds.Add(collectionValueArray[l]);
                                    }
                                }
                            }
                        }
                    }

                    MessagingCenter.Send(Application.Current, "RefreshGifts", true);
                }
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