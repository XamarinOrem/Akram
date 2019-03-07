using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Akram.ViewModels;
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
    public class UserValueEventListenerForScannedCollection : Java.Lang.Object, IValueEventListener
    {
        EventHandler OnChange;

        public UserValueEventListenerForScannedCollection()
        {
        }

        public UserValueEventListenerForScannedCollection(EventHandler OnChange) => this.OnChange = OnChange;

        public void OnCancelled(DatabaseError error)
        {

        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            try
            {
                if (OnChange != null && snapshot.Value != null && snapshot.Key == "Akram")
                {
                    var getUserCollection = snapshot.Child(TradingViewModel.ItemId);
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
                                for (int l = 0; l < collectionKeysArray.Count(); l++)
                                {
                                    if (TradingViewModel.collectModel.Sighting_id == collectionValueArray[l])
                                    {
                                        var getDistanceItem = collectionKeysArray.Where(a => a.ToLower() == "distance").FirstOrDefault();
                                        if (getDistanceItem != null)
                                        {
                                            var getIndex = collectionKeysArray.IndexOf(getDistanceItem);
                                            if (getIndex != -1)
                                            {
                                                TradingViewModel.collectModel.Distance = collectionValueArray[getIndex];
                                            }
                                        }


                                        var getExpiryItem = collectionKeysArray.Where(a => a.ToLower() == "expiry").FirstOrDefault();
                                        if (getExpiryItem != null)
                                        {
                                            var getIndex = collectionKeysArray.IndexOf(getExpiryItem);
                                            if (getIndex != -1)
                                            {
                                                TradingViewModel.collectModel.expiry = collectionValueArray[getIndex];
                                            }
                                        }


                                        var getLocItem = collectionKeysArray.Where(a => a.ToLower() == "loc").FirstOrDefault();
                                        if (getLocItem != null)
                                        {
                                            var getIndex = collectionKeysArray.IndexOf(getLocItem);
                                            if (getIndex != -1)
                                            {
                                                TradingViewModel.collectModel.Loc = collectionValueArray[getIndex];
                                            }
                                        }


                                        var getScanItem = collectionKeysArray.Where(a => a.ToLower() == "scan").FirstOrDefault();
                                        if (getScanItem != null)
                                        {
                                            var getIndex = collectionKeysArray.IndexOf(getScanItem);
                                            if (getIndex != -1)
                                            {
                                                TradingViewModel.collectModel.scan = collectionValueArray[getIndex];
                                            }
                                        }


                                        var getTypeItem = collectionKeysArray.Where(a => a.ToLower() == "type").FirstOrDefault();

                                        if (getTypeItem != null)
                                        {
                                            var getIndex = collectionKeysArray.IndexOf(getTypeItem);
                                            if (getIndex != -1)
                                            {
                                                TradingViewModel.collectModel.Type = collectionValueArray[getIndex];
                                            }
                                        }


                                        var getRulesItem = collectionKeysArray.Where(a => a.ToLower() == "rules").FirstOrDefault();
                                        if (getRulesItem != null)
                                        {
                                            var getIndex = collectionKeysArray.IndexOf(getRulesItem);
                                            if (getIndex != -1)
                                            {
                                                TradingViewModel.collectModel.rules = collectionValueArray[getIndex];
                                            }
                                        }

                                        var getItemID = collectionKeysArray.Where(a => a.ToLower() == "item_id").FirstOrDefault();
                                        if (getItemID != null)
                                        {
                                            var getIndex = collectionKeysArray.IndexOf(getItemID);
                                            if (getIndex != -1)
                                            {
                                                TradingViewModel.collectModel.item_id = collectionValueArray[getIndex];
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        MessagingCenter.Send(Xamarin.Forms.Application.Current, "CollectionScanDataFetched", true);
                    }
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