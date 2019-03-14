using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Akram.DependencyInterface;
using Akram.iOS.DependencyInterface;
using Akram.Models;
using Akram.ViewModels;
using Akram.Views;
using Firebase.Database;
using Foundation;
using System.Linq;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Rg.Plugins.Popup.Extensions;

[assembly: Dependency(typeof(FirebaseConnection))]
namespace Akram.iOS.DependencyInterface
{
    public class FirebaseConnection : IFirebaseDatabase
    {
        private DatabaseReference mDatabase;

        public void CheckGiftExists()
        {
            try
            {
                var database = Firebase.Database.Database.DefaultInstance;
                mDatabase = database.GetReferenceFromUrl("https://akramtheone-restore.firebaseio.com/Akram");

                mDatabase.ObserveSingleEvent(DataEventType.Value, (snapshot) =>
                {
                    if (snapshot.GetValue() != NSNull.Null)
                    {
                        var getUserCollection = snapshot.GetChildSnapshot(TradingViewModel.ItemId);
                        if (getUserCollection.GetValue() != null)
                        {
                            var getChild = getUserCollection.GetChildSnapshot("Collection")?.GetValue();
                            if (getChild != null)
                            {
                                MessagingCenter.Send(Xamarin.Forms.Application.Current, "GiftExists", true);
                            }
                            else
                            {
                                MessagingCenter.Send(Xamarin.Forms.Application.Current, "GiftNotExists", true);
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {

            }
        }

        public void CheckGiftForScan()
        {
            string getScanningResult = string.Empty;

            var database = Firebase.Database.Database.DefaultInstance;
            mDatabase = database.GetReferenceFromUrl("https://akramtheone-restore.firebaseio.com/Akram");

            mDatabase.ObserveSingleEvent(DataEventType.Value, (snapshot) =>
            {
                var getUserCollection = snapshot.GetChildSnapshot(LoginUserDetails.UserId);
                if (getUserCollection.HasChild("Collection"))
                {
                    var getChild = (NSDictionary)getUserCollection.GetChildSnapshot("Collection")?.GetValue();
                    if (getChild != null)
                    {
                        var getCollectionValues = getChild.Values.GetEnumerator();
                        while (getCollectionValues.MoveNext())
                        {
                            var item = (NSDictionary)getCollectionValues.Current;
                            var collectionValueArray = item.Values;
                            var collectionKeysArray = item.Keys;
                            for (int l = 0; l < collectionKeysArray.Count(); l++)
                            {
                                if (collectionKeysArray[l].ToString() == "item_id")
                                {
                                    if (GiftPage.ItemId == collectionValueArray[l].ToString())
                                    {
                                        var getScanItem = collectionKeysArray.Where(a => a.ToString().ToLower() == "scan").FirstOrDefault();
                                        var getIndex = collectionKeysArray.IndexOf(getScanItem);
                                        getScanningResult = collectionValueArray[getIndex].ToString();
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
            });
        }

        public void CheckGiftTaken()
        {
            try
            {
                var database = Firebase.Database.Database.DefaultInstance;
                mDatabase = database.GetReferenceFromUrl("https://akramtheone-restore.firebaseio.com/Akram");

                try
                {
                    MapGiftPage.listItemsTaken = new List<string>();
                    mDatabase.ObserveSingleEvent(DataEventType.Value, (snapshot) =>
                    {
                        var getItems = (NSDictionary)snapshot.GetChildSnapshot("Items")?.GetValue();
                        var getItemsValues = getItems.Keys;
                        for (int i = 0; i < getItemsValues.Count(); i++)
                        {
                            MapGiftPage.listItemsTaken.Add(getItemsValues[i].ToString());
                        }

                        try
                        {
                            if (!string.IsNullOrEmpty(LoginUserDetails.UserId))
                            {
                                var getCollectionhas = snapshot.GetChildSnapshot(LoginUserDetails.UserId).GetChildSnapshot("has");
                                if (getCollectionhas.GetValue() != null)
                                {
                                    MapGiftPage.hsa = (NSString)getCollectionhas.GetValue();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    });
                    MessagingCenter.Send(Xamarin.Forms.Application.Current, "CheckTakenGifts", true);
                }
                catch (Exception ex)
                {

                }
            }
            catch (Exception ex)
            {

            }
        }

        public void Collect(CollectModel collectModel)
        {
            try
            {
                var database = Firebase.Database.Database.DefaultInstance;
                mDatabase = database.GetReferenceFromUrl("https://akramtheone-restore.firebaseio.com/Akram");

                var resp = mDatabase.GetChild(collectModel.userId).GetChild("Collection");

                if (!string.IsNullOrEmpty(MapGiftPage.hsa))
                {
                    var has = mDatabase.GetChild(collectModel.userId).GetChild("has");
                    var setData = MapGiftPage.hsa + "," + collectModel.item_id;
                    has.SetValue((NSString)setData);
                }
                else
                {
                    var has = mDatabase.GetChild(collectModel.userId).GetChild("has");
                    has.SetValue((NSString)collectModel.item_id);
                }
                var Sighting = resp.GetChild(collectModel.Sighting_id);
                Sighting.GetChild("Distance").SetValue((NSString)collectModel.Distance);
                Sighting.GetChild("item_id").SetValue((NSString)collectModel.item_id);
                Sighting.GetChild("expiry").SetValue((NSString)collectModel.expiry);
                Sighting.GetChild("scan").SetValue((NSString)collectModel.scan);
                Sighting.GetChild("Name").SetValue((NSString)collectModel.Name);
                Sighting.GetChild("Loc").SetValue((NSString)collectModel.Loc);
                Sighting.GetChild("Sighting_id").SetValue((NSString)collectModel.Sighting_id);
                Sighting.GetChild("Type").SetValue((NSString)collectModel.Type);
                Sighting.GetChild("rules").SetValue((NSString)collectModel.rules);

                var getItems = mDatabase.GetChild("Items");
                if (getItems != null)
                {
                    var sightingId = getItems.GetChild(collectModel.Sighting_id);
                    sightingId.SetValue((NSString)"Taken");
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void DeleteCollection(string id, string itemId, string userId)
        {
            try
            {
                var database = Firebase.Database.Database.DefaultInstance;
                mDatabase = database.GetReferenceFromUrl("https://akramtheone-restore.firebaseio.com/Akram");
                var resp = mDatabase.GetChild(userId).GetChild("Collection");

                var has = mDatabase.GetChild(userId).GetChild("has");
                string hasVal = "";
                string[] words = MapGiftPage.hsa.Split(',');
                foreach (string word in words)
                {
                    if (!string.IsNullOrEmpty(word.Trim()) && word.Trim() != itemId)
                    {
                        hasVal += word + ",";
                    }
                }
                has.SetValue((NSString)hasVal);
                MapGiftPage.hsa = hasVal;
                var Sighting = resp.GetChild(id);
                Sighting.RemoveValue();

            }
            catch (Exception ex)
            {

            }

        }

        public void GetGiftsMyCollection()
        {
            try
            {
                var database = Firebase.Database.Database.DefaultInstance;
                mDatabase = database.GetReferenceFromUrl("https://akramtheone-restore.firebaseio.com/Akram");
                try
                {
                    mDatabase.ObserveSingleEvent(DataEventType.Value, (snapshot) =>
                    {
                        App.Database.ClearCollectionDetails();

                        var getNotifications = App.Database.GetNotifications();

                        var getUserCollection = snapshot.GetChildSnapshot(LoginUserDetails.UserId);
                        if (getUserCollection.HasChild("Collection"))
                        {
                            var getChild = (NSDictionary)getUserCollection.GetChildSnapshot("Collection")?.GetValue();
                            if (getChild != null)
                            {
                                string getKey = string.Empty;
                                foreach (var item in getChild.Keys)
                                {
                                    getKey = item.ToString();
                                }

                                var getCollectionValues = getChild.Values.GetEnumerator();
                                while (getCollectionValues.MoveNext())
                                {
                                    var item = (NSDictionary)getCollectionValues.Current;
                                    var itemValues = item.Values;
                                    var itemKeys = item.Keys;
                                    SaveCollectionData _collectionData = new SaveCollectionData();
                                    for (int l = 0; l < itemKeys.Count(); l++)
                                    {
                                        if (itemKeys[l].ToString() == "Name")
                                        {
                                            _collectionData.ShopName = itemValues[l].ToString();
                                        }
                                        if (itemKeys[l].ToString() == "expiry")
                                        {
                                            _collectionData.Date = itemValues[l].ToString();
                                        }
                                        if (itemKeys[l].ToString() == "Sighting_id")
                                        {
                                            _collectionData.SightingId = itemValues[l].ToString();
                                        }
                                        if (itemKeys[l].ToString() == "item_id")
                                        {
                                            _collectionData.ItemId = itemValues[l].ToString();
                                        }
                                        if (itemKeys[l].ToString() == "rules")
                                        {
                                            _collectionData.Rules = itemValues[l].ToString();
                                        }
                                        if (itemKeys[l].ToString() == "scan")
                                        {
                                            _collectionData.Scan = itemValues[l].ToString();
                                        }
                                        if (itemKeys[l].ToString() == "Distance")
                                        {
                                            _collectionData.Distance = itemValues[l].ToString();
                                        }
                                        if (itemKeys[l].ToString() == "Loc")
                                        {
                                            _collectionData.Location = itemValues[l].ToString();
                                        }
                                        if (itemKeys[l].ToString() == "Type")
                                        {
                                            _collectionData.Type = itemValues[l].ToString();
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
                        MessagingCenter.Send(Xamarin.Forms.Application.Current, "LoadCollections", true);
                    });
                }
                catch (Exception ex)
                {

                }
            }
            catch (Exception ex)
            {

            }
        }

        public void GetLocations()
        {
            try
            {
                var database = Firebase.Database.Database.DefaultInstance;
                mDatabase = database.GetReferenceFromUrl("https://akramtheone-restore.firebaseio.com/Akram");

                mDatabase.ObserveSingleEvent(DataEventType.Value, (snapshot) =>
               {
                App.Database.ClearLocationDetails();

                var getAllLocations = snapshot.GetChildSnapshot("location");
                   
                   if (getAllLocations.HasChildren)
                   {
                        //var getItemLocations = getAllLocations.GetChildSnapshot("5");
                       var getItemLocations = getAllLocations.GetChildSnapshot(GiftPage.ItemId);
                        if (getItemLocations.HasChildren)
                       {
                           var getValues = (NSDictionary)getItemLocations?.GetValue();
                           if (getValues != null)
                           {
                               var getLocationValues = getValues.Values;

                               var getLocationKeys = getValues.Keys;

                               GetLocationModel _locationData = new GetLocationModel();
                               for (int l = 0; l < getLocationKeys.Count(); l++)
                               {
                                   _locationData.Name = getLocationKeys[l].ToString();
                                   _locationData.LatLong = getLocationValues[l].ToString();
                                   App.Database.SaveLocation(_locationData);
                               }

                           }
                       }
                   }
                MessagingCenter.Send(Xamarin.Forms.Application.Current, "LoadLocationPopup", true);
            });

            }
            catch (Exception ex)
            {

            }
        }

        public void GetScennedCollectionData()
        {
            var database = Firebase.Database.Database.DefaultInstance;
            mDatabase = database.GetReferenceFromUrl("https://akramtheone-restore.firebaseio.com/Akram");

            mDatabase.ObserveSingleEvent(DataEventType.Value, (snapshot) =>
            {
                var getUserCollection = snapshot.GetChildSnapshot(TradingViewModel.ItemId);
                if (getUserCollection.GetValue() != null)
                {
                    var getChild = (NSDictionary)getUserCollection.GetChildSnapshot("Collection")?.GetValue();
                    if (getChild != null)
                    {
                        var getCollectionValues = getChild.Values.GetEnumerator();

                        while (getCollectionValues.MoveNext())
                        {
                            var item = (NSDictionary)getCollectionValues.Current;
                            var collectionValueArray = item.Values;
                            var collectionKeysArray = item.Keys;

                            for (int l = 0; l < collectionKeysArray.Count(); l++)
                            {
                                if (TradingViewModel.collectModel.Sighting_id == collectionValueArray[l].ToString())
                                {
                                    var getDistanceItem = collectionKeysArray.Where(a => a.ToString().ToLower() == "distance").FirstOrDefault();
                                    if (getDistanceItem != null)
                                    {
                                        var getIndex = collectionKeysArray.IndexOf(getDistanceItem);
                                        if (getIndex != -1)
                                        {
                                            TradingViewModel.collectModel.Distance = collectionValueArray[getIndex].ToString();
                                        }
                                    }


                                    var getExpiryItem = collectionKeysArray.Where(a => a.ToString().ToLower() == "expiry").FirstOrDefault();
                                    if (getExpiryItem != null)
                                    {
                                        var getIndex = collectionKeysArray.IndexOf(getExpiryItem);
                                        if (getIndex != -1)
                                        {
                                            TradingViewModel.collectModel.expiry = collectionValueArray[getIndex].ToString();
                                        }
                                    }


                                    var getLocItem = collectionKeysArray.Where(a => a.ToString().ToLower() == "loc").FirstOrDefault();
                                    if (getLocItem != null)
                                    {
                                        var getIndex = collectionKeysArray.IndexOf(getLocItem);
                                        if (getIndex != -1)
                                        {
                                            TradingViewModel.collectModel.Loc = collectionValueArray[getIndex].ToString();
                                        }
                                    }


                                    var getScanItem = collectionKeysArray.Where(a => a.ToString().ToLower() == "scan").FirstOrDefault();
                                    if (getScanItem != null)
                                    {
                                        var getIndex = collectionKeysArray.IndexOf(getScanItem);
                                        if (getIndex != -1)
                                        {
                                            TradingViewModel.collectModel.scan = collectionValueArray[getIndex].ToString();
                                        }
                                    }


                                    var getTypeItem = collectionKeysArray.Where(a => a.ToString().ToLower() == "type").FirstOrDefault();

                                    if (getTypeItem != null)
                                    {
                                        var getIndex = collectionKeysArray.IndexOf(getTypeItem);
                                        if (getIndex != -1)
                                        {
                                            TradingViewModel.collectModel.Type = collectionValueArray[getIndex].ToString();
                                        }
                                    }


                                    var getRulesItem = collectionKeysArray.Where(a => a.ToString().ToLower() == "rules").FirstOrDefault();
                                    if (getRulesItem != null)
                                    {
                                        var getIndex = collectionKeysArray.IndexOf(getRulesItem);
                                        if (getIndex != -1)
                                        {
                                            TradingViewModel.collectModel.rules = collectionValueArray[getIndex].ToString();
                                        }
                                    }

                                    var getItemID = collectionKeysArray.Where(a => a.ToString().ToLower() == "item_id").FirstOrDefault();
                                    if (getItemID != null)
                                    {
                                        var getIndex = collectionKeysArray.IndexOf(getItemID);
                                        if (getIndex != -1)
                                        {
                                            TradingViewModel.collectModel.item_id = collectionValueArray[getIndex].ToString();
                                        }
                                    }

                                    var getSightingID = collectionKeysArray.Where(a => a.ToString().ToLower() == "Sighting_id").FirstOrDefault();
                                    if (getSightingID != null)
                                    {
                                        var getIndex = collectionKeysArray.IndexOf(getSightingID);
                                        if (getIndex != -1)
                                        {
                                            TradingViewModel.collectModel.Sighting_id = collectionValueArray[getIndex].ToString();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                MessagingCenter.Send(Xamarin.Forms.Application.Current, "CollectionScanDataFetched", true);

            });
        }

        public async void GiftDetailsCheckTaken()
        {
            await Xamarin.Forms.Application.Current.MainPage.Navigation.PushPopupAsync(new LoaderPopup());
            MapGiftPage.AllItemList = new List<string>();
            try
            {
                var database = Firebase.Database.Database.DefaultInstance;
                mDatabase = database.GetReferenceFromUrl("https://akramtheone-restore.firebaseio.com/Akram");

                mDatabase.ObserveSingleEvent(DataEventType.Value, (snapshot) =>
                {
                    var getUserCollection = snapshot.GetChildSnapshot(LoginUserDetails.UserId);
                    if (getUserCollection.HasChild("Collection"))
                    {
                        var getChild = (NSDictionary)getUserCollection.GetChildSnapshot("Collection")?.GetValue();
                        if (getChild != null)
                        {
                            var getCollectionValues = getChild.Values.GetEnumerator();
                            while (getCollectionValues.MoveNext())
                            {
                                var item = (NSDictionary)getCollectionValues.Current;
                                var itemValues = item.Values;
                                var itemKeys = item.Keys;
                                for (int l = 0; l < itemKeys.Count(); l++)
                                {
                                    if (itemKeys[l].ToString() == "item_id")
                                    {
                                        MapGiftPage.AllItemList.Add(itemValues[l].ToString());
                                    }
                                }
                            }
                        }
                    }

                    MessagingCenter.Send(Xamarin.Forms.Application.Current, "GiftsTakenDetails", true);
                });
            }
            catch (Exception ex)
            {

            }
        }

        public void MakeConnection()
        {
            //HomeMapPage.AllItemIds = new List<string>();
            try
            {
                var database = Firebase.Database.Database.DefaultInstance;
                mDatabase = database.GetReferenceFromUrl("https://akramtheone-restore.firebaseio.com/Akram");

                mDatabase.ObserveSingleEvent(DataEventType.Value, (snapshot) =>
                {
                    var getUserCollection = snapshot.GetChildSnapshot(LoginUserDetails.UserId);
                    if (getUserCollection.HasChild("Collection"))
                    {
                        var getChild = (NSDictionary)getUserCollection.GetChildSnapshot("Collection")?.GetValue();
                        if (getChild != null)
                        {
                            var getCollectionValues = getChild.Values.GetEnumerator();
                            while (getCollectionValues.MoveNext())
                            {
                                var item = (NSDictionary)getCollectionValues.Current;
                                var itemValues = item.Values;
                                var itemKeys = item.Keys;
                                for (int l = 0; l < itemKeys.Count(); l++)
                                {
                                    if (itemKeys[l].ToString() == "item_id")
                                    {
                                        HomeMapPage.AllItemIds.Add(itemValues[l].ToString());
                                    }
                                }
                            }
                        }
                    }

                    MessagingCenter.Send(Xamarin.Forms.Application.Current, "LoadGifts", true);
                });


            }
            catch (Exception ex)
            {

            }
        }

        public void MakeConnectionForReferesh()
        {
            //HomeMapPage.AllItemIds = new List<string>();
            try
            {
                var database = Firebase.Database.Database.DefaultInstance;
                mDatabase = database.GetReferenceFromUrl("https://akramtheone-restore.firebaseio.com/Akram");

                mDatabase.ObserveSingleEvent(DataEventType.Value, (snapshot) =>
                {
                    var getUserCollection = snapshot.GetChildSnapshot(LoginUserDetails.UserId);
                    if (getUserCollection.HasChild("Collection"))
                    {
                        var getChild = (NSDictionary)getUserCollection.GetChildSnapshot("Collection")?.GetValue();
                        if (getChild != null)
                        {
                            var getCollectionValues = getChild.Values.GetEnumerator();
                            while (getCollectionValues.MoveNext())
                            {
                                var item = (NSDictionary)getCollectionValues.Current;
                                var itemValues = item.Values;
                                var itemKeys = item.Keys;
                                for (int l = 0; l < itemKeys.Count(); l++)
                                {
                                    if (itemKeys[l].ToString() == "item_id")
                                    {
                                        HomeMapPage.AllItemIds.Add(itemValues[l].ToString());
                                    }
                                }
                            }
                        }
                    }

                    MessagingCenter.Send(Xamarin.Forms.Application.Current, "RefreshGifts", true);
                });


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