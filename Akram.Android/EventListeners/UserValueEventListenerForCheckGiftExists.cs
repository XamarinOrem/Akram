using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Akram.Models;
using Akram.ViewModels;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Database;
using Xamarin.Forms;

namespace Akram.Droid
{
    public class UserValueEventListenerForCheckGiftExists : Java.Lang.Object, IValueEventListener
    {
        EventHandler OnChange;

        public UserValueEventListenerForCheckGiftExists()
        {
        }

        public UserValueEventListenerForCheckGiftExists(EventHandler OnChange) => this.OnChange = OnChange;

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
                            MessagingCenter.Send(Xamarin.Forms.Application.Current, "GiftExists", true);
                        }
                        else
                        {
                            MessagingCenter.Send(Xamarin.Forms.Application.Current, "GiftNotExists", true);
                        }
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