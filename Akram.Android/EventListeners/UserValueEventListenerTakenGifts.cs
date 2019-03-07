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
using Akram.Views;
using Xamarin.Forms;
using Akram.Models;
using Rg.Plugins.Popup.Extensions;

namespace Akram.Droid
{
    public class UserValueEventListenerTakenGifts : Java.Lang.Object, IValueEventListener
    {
        EventHandler OnChange;

        public UserValueEventListenerTakenGifts(EventHandler OnChange) => this.OnChange = OnChange;

        public void OnCancelled(DatabaseError error)
        {

        }

        public async void OnDataChange(DataSnapshot snapshot)
        {
            try
            {

                MapGiftPage.listItemsTaken = new List<string>();
                if (OnChange != null && snapshot.Value != null && snapshot.Key == "Akram")
                {
                    var getItems = (JavaDictionary)snapshot.Child("Items")?.GetValue(true);
                    var getItemsValues = getItems.Keys;
                    String[] arr = new String[getItemsValues.Count];
                    getItemsValues.CopyTo(arr, 0);
                    for (int i = 0; i < arr.Count(); i++)
                    {
                        MapGiftPage.listItemsTaken.Add(arr[i]);
                    }

                    try
                    {
                        if (!string.IsNullOrEmpty(LoginUserDetails.UserId))
                        {
                            var getCollectionhas = snapshot.Child(LoginUserDetails.UserId).Child("has");
                            if (getCollectionhas.Value != null)
                            {
                                MapGiftPage.hsa = (String)getCollectionhas.Value;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
                MessagingCenter.Send(Xamarin.Forms.Application.Current, "CheckTakenGifts", true);
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