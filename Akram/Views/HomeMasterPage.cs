using Akram.DependencyInterface;
using Akram.Models;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Akram.Views
{
    public class HomeMasterPage : MasterDetailPage
    {
        public static HomeMasterPage _masterPage;

        public static MasterPageItem previousItem;

        MenuList masterPage;
        public static MasterPageItem selectedItem;
        public bool PageDisplay=false;

        public HomeMasterPage()
        {
            _masterPage = this;

            TermsAndConditions.displayBtn = false;

            masterPage = new MenuList();

            Master = masterPage;
            {
                Detail = new NavigationPage(new HomeMapPage());
            }
            Padding = new Thickness(0);

            NavigationPage.SetHasNavigationBar(this, false);


            TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();
            masterPage.BackImage.GestureRecognizers.Add(tapGestureRecognizer);
            tapGestureRecognizer.Tapped += TapGestureRecognizer_Tapped;

            masterPage.ListView.ItemSelected += OnItemSelected;

            masterPage.ListView1.ItemSelected += OnItemSelected1;
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            this.IsPresented = !this.IsPresented;
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var listView = sender as ListView;
            var item = e.SelectedItem as MasterPageItem;
            if (item != null)
            {
                masterPage.ListView.SelectedItem = null;
                IsPresented = false;
                if (item.Title != "Home")
                {
                    await Navigation.PushAsync((Page)Activator.CreateInstance(item.TargetType));
                }
                //Detail = new NavigationPage((Page)Activator.CreateInstance(item.TargetType));
            }
        }

        async void OnItemSelected1(object sender, SelectedItemChangedEventArgs e)
        {
            var listView = sender as ListView;
            var item = e.SelectedItem as MasterPageItem;
            if (item != null)
            {
                if (item.Title == "Interests")
                {
                    masterPage.ListView1.SelectedItem = null;
                    IsPresented = false;
                    await Navigation.PushPopupAsync(new MyIntersetPage());
                }
                else
                {
                    masterPage.ListView1.SelectedItem = null;
                    IsPresented = false;
                    await Navigation.PushAsync((Page)Activator.CreateInstance(item.TargetType));
                    //Detail = new NavigationPage((Page)Activator.CreateInstance(item.TargetType));
                }
            }
        }
    }
}
