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
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms;

namespace Akram.Droid.EventListeners
{
    public class UserValueEventListenerGiftDeatilsTaken : Java.Lang.Object, IValueEventListener
    {

        EventHandler OnChange;

        public UserValueEventListenerGiftDeatilsTaken(EventHandler OnChange) => this.OnChange = OnChange;

        public void OnCancelled(DatabaseError error)
        {

        }

        public async void OnDataChange(DataSnapshot snapshot)
        {
            await Xamarin.Forms.Application.Current.MainPage.Navigation.PushPopupAsync(new LoaderPopup());

            MapGiftPage.AllItemList = new List<string>();
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
                                for (int l = 0; l < collectionKeysArray.Count(); l++)
                                {
                                    if (collectionKeysArray[l] == "item_id")
                                    {
                                        MapGiftPage.AllItemList.Add(collectionValueArray[l]);
                                    }
                                }
                            }
                        }
                    }

                    MessagingCenter.Send(Xamarin.Forms.Application.Current, "GiftsTakenDetails", true);
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