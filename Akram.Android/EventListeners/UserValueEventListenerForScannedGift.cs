using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Akram.Models;
using Akram.Views;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Database;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Akram.Droid
{
    public class UserValueEventListenerForScannedGift : Java.Lang.Object, IValueEventListener
    {
        EventHandler OnChange;

        public UserValueEventListenerForScannedGift(EventHandler OnChange) => this.OnChange = OnChange;

        public void OnCancelled(DatabaseError error)
        {

        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            string getScanningResult = string.Empty;
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
                                        if (GiftPage.ItemId == collectionValueArray[l])
                                        {
                                            var getScanItem = collectionKeysArray.Where(a => a.ToLower() == "scan").FirstOrDefault();
                                            var getIndex = collectionKeysArray.IndexOf(getScanItem);
                                            getScanningResult = collectionValueArray[getIndex];
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (getScanningResult == "1")
                {
                    MessagingCenter.Send(new SendPosition { pokeman_id = getScanningResult }, "ScanChanged");
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
