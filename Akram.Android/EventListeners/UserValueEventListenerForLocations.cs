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
    public class UserValueEventListenerForLocations : Java.Lang.Object, IValueEventListener
    {
        EventHandler OnChange;

        public UserValueEventListenerForLocations(EventHandler OnChange) => this.OnChange = OnChange;

        public void OnCancelled(DatabaseError error)
        {

        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            try
            {
                if (OnChange != null && snapshot.Value != null && snapshot.Key == "Akram")
                {
                    App.Database.ClearLocationDetails();

                    var getAllLocations = snapshot.Child("location");
                    //var getItemLocations = getAllLocations.Child("5");
                    var getItemLocations = getAllLocations.Child(GiftPage.ItemId);
                    if (getItemLocations.Value != null)
                    {
                        var getValues = (JavaDictionary)getItemLocations?.GetValue(true);
                        if (getValues != null)
                        {
                            var getLocationValues = getValues.Values;
                            String[] locationValueArray = new String[getLocationValues.Count];
                            getLocationValues.CopyTo(locationValueArray, 0);

                            var getLocationKeys = getValues.Keys;
                            String[] locationKeysArray = new String[getLocationKeys.Count];
                            getLocationKeys.CopyTo(locationKeysArray, 0);

                            GetLocationModel _locationData = new GetLocationModel();
                            for (int l = 0; l < locationKeysArray.Count(); l++)
                            {
                                _locationData.Name = locationKeysArray[l];
                                _locationData.LatLong = locationValueArray[l];
                                App.Database.SaveLocation(_locationData);
                            }
                            
                        }
                    }
                    MessagingCenter.Send(Xamarin.Forms.Application.Current, "LoadLocationPopup", true);
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