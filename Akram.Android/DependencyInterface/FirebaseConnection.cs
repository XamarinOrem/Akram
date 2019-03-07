using System;
using Akram.DependencyInterface;
using Akram.Droid.DependencyInterface;
using Akram.Droid.EventListeners;
using Akram.Models;
using Akram.Views;
using Firebase.Database;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms;

[assembly: Dependency(typeof(FirebaseConnection))]
namespace Akram.Droid.DependencyInterface
{

    public class FirebaseConnection : IFirebaseDatabase
    {
        private DatabaseReference mDatabase;

        public void MakeConnection()
        {
            try
            {
                mDatabase = FirebaseDatabase.GetInstance("https://akramtheone-restore.firebaseio.com/").GetReference("Akram");

                mDatabase.AddListenerForSingleValueEvent(new UserValueEventListener((sender, e) =>
                {
                    var userFromFirebase = (e as UserEventArgs).value;
                }));
            }
            catch (Exception ex)
            {

            }
        }

        public void MakeConnectionForReferesh()
        {
            try
            {
                mDatabase = FirebaseDatabase.GetInstance("https://akramtheone-restore.firebaseio.com/").GetReference("Akram");

                mDatabase.AddListenerForSingleValueEvent(new UserValueEventListenerRefresh((sender, e) =>
                {
                    var userFromFirebase = (e as UserEventArgs).value;
                }));
            }
            catch (Exception ex)
            {

            }
        }

        public void Collect(CollectModel collectModel)
        {
            try
            {
                mDatabase = FirebaseDatabase.GetInstance("https://akramtheone-restore.firebaseio.com/").GetReference("Akram");

                var resp = mDatabase.Child(collectModel.userId).Child("Collection");

                if (!string.IsNullOrEmpty(MapGiftPage.hsa))
                {
                    var has = mDatabase.Child(collectModel.userId).Child("has");
                    has.SetValue(MapGiftPage.hsa + "," + collectModel.item_id);
                }
                else
                {
                    var has = mDatabase.Child(collectModel.userId).Child("has");
                    has.SetValue(collectModel.item_id);
                }
                var Sighting = resp.Child(collectModel.Sighting_id);
                Sighting.Child("Distance").SetValue(collectModel.Distance);
                Sighting.Child("item_id").SetValue(collectModel.item_id);
                Sighting.Child("expiry").SetValue(collectModel.expiry);
                Sighting.Child("scan").SetValue(collectModel.scan);
                Sighting.Child("Name").SetValue(collectModel.Name);
                Sighting.Child("Loc").SetValue(collectModel.Loc);
                Sighting.Child("Sighting_id").SetValue(collectModel.Sighting_id);
                Sighting.Child("Type").SetValue(collectModel.Type);
                Sighting.Child("rules").SetValue(collectModel.rules);

                var getItems = mDatabase.Child("Items");
                if (getItems != null)
                {
                    var sightingId = getItems.Child(collectModel.Sighting_id);
                    sightingId.SetValue("Taken");
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
                mDatabase = FirebaseDatabase.GetInstance("https://akramtheone-restore.firebaseio.com/").GetReference("Akram");
                var resp = mDatabase.Child(userId).Child("Collection");

                var has = mDatabase.Child(userId).Child("has");
                string hasVal = "";
                string[] words = MapGiftPage.hsa.Split(',');
                foreach (string word in words)
                {
                    if (!string.IsNullOrEmpty(word.Trim()) && word.Trim() != itemId)
                    {
                        hasVal += word + ",";
                    }
                }
                has.SetValue(hasVal);
                MapGiftPage.hsa = hasVal;
                var Sighting = resp.Child(id);
                Sighting.RemoveValue();

            }
            catch (Exception ex)
            {

            }
        }

        public async void CheckGiftTaken()
        {
            try
            {
                mDatabase = FirebaseDatabase.GetInstance("https://akramtheone-restore.firebaseio.com/").GetReference("Akram");

                mDatabase.AddListenerForSingleValueEvent(new UserValueEventListenerTakenGifts((sender, e) =>
                {
                    var userFromFirebase = (e as UserEventArgs).value;
                }));
            }
            catch (Exception ex)
            {

            }
        }

        public void GetGiftsMyCollection()
        {
            try
            {
                mDatabase = FirebaseDatabase.GetInstance("https://akramtheone-restore.firebaseio.com/").GetReference("Akram");

                mDatabase.AddListenerForSingleValueEvent(new UserValueEventListenerCollection((sender, e) =>
                {
                    var userFromFirebase = (e as UserEventArgs).value;
                }));
            }
            catch (Exception ex)
            {

            }
        }

        public void GetLocations()
        {
            try
            {
                mDatabase = FirebaseDatabase.GetInstance("https://akramtheone-restore.firebaseio.com/").GetReference("Akram");

                mDatabase.AddListenerForSingleValueEvent(new UserValueEventListenerForLocations((sender, e) =>
                {
                    var userFromFirebase = (e as UserEventArgs).value;
                }));
            }
            catch (Exception ex)
            {

            }
        }

        public void CheckGiftForScan()
        {
            mDatabase = FirebaseDatabase.GetInstance("https://akramtheone-restore.firebaseio.com/").GetReference("Akram");

            mDatabase.AddListenerForSingleValueEvent(new UserValueEventListenerForScannedGift((sender, e) =>
            {
                var userFromFirebase = (e as UserEventArgs).value;
            }));
        }

        public void CheckGiftExists()
        {
            try
            {
                mDatabase = FirebaseDatabase.GetInstance("https://akramtheone-restore.firebaseio.com/").GetReference("Akram");

                mDatabase.AddListenerForSingleValueEvent(new UserValueEventListenerForCheckGiftExists((sender, e) =>
                {
                    var userFromFirebase = (e as UserEventArgs).value;
                }));
            }
            catch (Exception ex)
            {

            }

        }

        public void GetScennedCollectionData()
        {
            mDatabase = FirebaseDatabase.GetInstance("https://akramtheone-restore.firebaseio.com/").GetReference("Akram");

            mDatabase.AddListenerForSingleValueEvent(new UserValueEventListenerForScannedCollection((sender, e) =>
            {
                var userFromFirebase = (e as UserEventArgs).value;
            }));


        }

        public void GiftDetailsCheckTaken()
        {
            try
            {
                mDatabase = FirebaseDatabase.GetInstance("https://akramtheone-restore.firebaseio.com/").GetReference("Akram");

                mDatabase.AddListenerForSingleValueEvent(new UserValueEventListenerGiftDeatilsTaken((sender, e) =>
                {
                    var userFromFirebase = (e as UserEventArgs).value;
                }));
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