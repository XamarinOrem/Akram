using Akram.CustomControls;
using Akram.Repository;
using Akram.Views;
using ImageCircle.Forms.Plugin.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms;

namespace Akram.Models
{
    public class MenuList : ContentPage
    {
        public ListView ListView { get { return listView; } }
        public static ListView listView;
        public ListView ListView1 { get { return listView1; } }
        public static ListView listView1;
        Image backImage;
        public Image BackImage { get { return backImage; } }

        public MenuList()
        {
            var masterPageItems = new List<MasterPageItem>();

            var masterPageItems1 = new List<MasterPageItem>();

            BackgroundColor = Color.Transparent;

            BackgroundImage = "menu_drawer_bg_half.png";

            if(Device.RuntimePlatform==Device.iOS)
            {
                this.Padding = new Thickness(0, 30, 0, 0);
            }

            masterPageItems.Add(new MasterPageItem
            {
                Title = "Home",
                Icon = "ic_drawer_home.png",
                //TargetType = typeof(HomeMapPage)

            });

            masterPageItems.Add(new MasterPageItem
            {
                Title = "About Us",
                Icon = "ic_drawer_about_us.png",
                TargetType = typeof(AboutUsPage)

            });

            masterPageItems.Add(new MasterPageItem
            {
                Title = "Terms And Conditions",
                Icon = "ic_drawer_terms_and_conditions.png",
                TargetType = typeof(TermsAndConditions)

            });

            masterPageItems.Add(new MasterPageItem
            {
                Title = "How To Play?",
                Icon = "ic_drawer_how_to_play.png",
                TargetType = typeof(AboutUsPage)

            });

            if (LoginUserDetails.IsLoggedIn)
            {
                masterPageItems1.Add(new MasterPageItem
                {
                    Title = "My Collection",
                    Icon = "ic_drawer_my_collection.png",
                    TargetType = typeof(MyCollectionPage)

                });

                masterPageItems1.Add(new MasterPageItem
                {
                    Title = "Trading",
                    Icon = "ic_drawer_trading.png",
                    TargetType = typeof(TradingPage)

                });

                masterPageItems1.Add(new MasterPageItem
                {
                    Title = "Interests",
                    Icon = "ic_drawer_interests.png",
                    TargetType = typeof(MyIntersetPage)

                });

                masterPageItems1.Add(new MasterPageItem
                {
                    Title = "Settings",
                    Icon = "ic_drawer_settings.png",
                    TargetType = typeof(SettingsPage)

                });
            }
            else
            {
                masterPageItems1.Add(new MasterPageItem
                {
                    Title = "Log In",
                    Icon = "ic_darwer_logout.png",
                    TargetType = typeof(LoginPage)

                });
            }


            listView = new CustomListView
            {
                ItemsSource = masterPageItems,
                HeaderTemplate = new DataTemplate(typeof(HeaderCell)),
                Header = "Test",
                Margin = new Thickness(0),
                HeightRequest= masterPageItems.Count*65,
                HasUnevenRows =true,
                ItemTemplate = new DataTemplate(typeof(CustomCell)),
                VerticalOptions = LayoutOptions.StartAndExpand,
                SeparatorVisibility = SeparatorVisibility.Default,
                SeparatorColor=Color.FromHex("#A7A9AA"),
                BackgroundColor = Color.Transparent,

            };

            listView1 = new CustomListView
            {
                ItemsSource = masterPageItems1,
                HeaderTemplate = new DataTemplate(typeof(FooterCell)),
                Header = "Test",
                HasUnevenRows = true,
                HeightRequest = masterPageItems1.Count * 65,
                ItemTemplate = new DataTemplate(typeof(CustomCell)),
                VerticalOptions = LayoutOptions.StartAndExpand,
                SeparatorVisibility = SeparatorVisibility.Default,
                SeparatorColor = Color.FromHex("#A7A9AA"),
                BackgroundColor = Color.Transparent,

            };

            var scrollView = new ScrollView();

            StackLayout scrollLayout = new StackLayout();
            scrollLayout.Children.Add(listView);
            scrollLayout.Children.Add(listView1);

            scrollView.Content = scrollLayout;

            Title = "Akram Customer";
            //Icon = "menu1.png";
            BackgroundColor = Color.Transparent;

            Label profileLbl = new Label
            {
                FontSize = 26,
                HorizontalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                VerticalTextAlignment = TextAlignment.Center,
                FontFamily = "MYRIADPROREGULAR",
                StyleId = "MYRIADPROREGULAR",
                TextColor = Color.White,
                Text = "Profile",
                Margin = new Thickness(0, 8, 0, 8)
        };

            backImage = new Image();
            backImage.HorizontalOptions = LayoutOptions.EndAndExpand;
            backImage.HeightRequest = 40;
            backImage.WidthRequest = 40;
            backImage.Source = "ic_back.png";
            backImage.Margin = new Thickness(0, 8, 0, 8);

            Image backImage1 = new Image();
            backImage1.HorizontalOptions = LayoutOptions.StartAndExpand;
            backImage1.HeightRequest = 40;
            backImage1.WidthRequest = 40;

            StackLayout upperStackLayout = new StackLayout();
            upperStackLayout.Orientation = StackOrientation.Horizontal;
            upperStackLayout.BackgroundColor = Color.FromHex("#107E3E");
            upperStackLayout.HorizontalOptions = LayoutOptions.FillAndExpand;

            upperStackLayout.Children.Add(backImage1);
            upperStackLayout.Children.Add(profileLbl);
            upperStackLayout.Children.Add(backImage);

            StackLayout stackLayout = new StackLayout();
            stackLayout.Orientation = StackOrientation.Horizontal;
            stackLayout.Margin = new Thickness(10,7,10,0);

            CircleImage userImg = new CircleImage();
            userImg.HeightRequest = 80;
            userImg.WidthRequest = 80;
            userImg.Margin = new Thickness(20, 0, 0, 0);
            userImg.Aspect = Aspect.AspectFill;
            userImg.HorizontalOptions = LayoutOptions.Start;
            userImg.Source = string.IsNullOrEmpty(LoginUserDetails.FacebookProfilePicture)?"ic_drawer_profile_default_menu.png":LoginUserDetails.FacebookProfilePicture;


            StackLayout nameLayout = new StackLayout();
            nameLayout.Spacing = 5;
            nameLayout.VerticalOptions = LayoutOptions.Center;

            Label lbl = new Label
            {
                FontSize = 22,
                HorizontalTextAlignment = TextAlignment.Start,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.StartAndExpand,
                VerticalTextAlignment = TextAlignment.Start,
                FontFamily = "MYRIADPROREGULAR",
                StyleId = "MYRIADPROREGULAR",
                TextColor = Color.White,
                Text = !string.IsNullOrEmpty(LoginUserDetails.FullName) ? CommonLib.FirstCharToUpper(LoginUserDetails.FullName) : "FULL NAME"
            };

            Label UserNamelbl = new Label
            {
                FontSize = 16,
                HorizontalTextAlignment = TextAlignment.Start,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.StartAndExpand,
                VerticalTextAlignment = TextAlignment.Start,
                FontFamily = "MYRIADPROREGULAR",
                StyleId = "MYRIADPROREGULAR",
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.Black,
                Text = !string.IsNullOrEmpty(LoginUserDetails.UserName) ?CommonLib.FirstCharToUpper(LoginUserDetails.UserName) : "User Name" 
            };

            nameLayout.Children.Add(lbl);
            nameLayout.Children.Add(UserNamelbl);

            stackLayout.Children.Add(userImg);
            stackLayout.Children.Add(nameLayout);

            stackLayout.Margin = new Thickness(0, 10, 0, 10);

            StackLayout _layout = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
                VerticalOptions = LayoutOptions.StartAndExpand,
                Spacing = 0,
                BackgroundColor = Color.Transparent,
                Padding = new Thickness(0),
                Children = {
                    upperStackLayout,stackLayout, scrollView
                }
            };

            Content = new StackLayout
            {
                BackgroundColor = Color.Transparent,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.StartAndExpand,
                Spacing = 0,
                Padding = new Thickness(0, 0, 0, 0),
                Children = {
                   _layout
                }
            };
        }

        public class HeaderCell:ContentView
        {
            public HeaderCell()
            {
                Label lbl = new Label
                {
                    FontSize = 25,
                    HorizontalTextAlignment = TextAlignment.Start,
                    HorizontalOptions = LayoutOptions.StartAndExpand,
                    VerticalOptions = LayoutOptions.StartAndExpand,
                    VerticalTextAlignment = TextAlignment.Center,
                    FontFamily = "MYRIADPROREGULAR",
                    StyleId = "MYRIADPROREGULAR",
                    TextColor = Color.White,
                    Text="Categories",
                    Margin=new Thickness(10,0,0,0)
                };

                BoxView box = new BoxView
                {
                    BackgroundColor = Color.FromHex("#A7A9AA"),
                    HeightRequest=0.5,
                    HorizontalOptions=LayoutOptions.FillAndExpand
                };

                BoxView box1 = new BoxView
                {
                    BackgroundColor = Color.FromHex("#A7A9AA"),
                    HeightRequest = 0.5,
                    HorizontalOptions = LayoutOptions.FillAndExpand
                };

                if (Device.RuntimePlatform == Device.iOS)
                {
                    this.Padding = new Thickness(0, 25, 0, 0);
                }

                var sl_projectinfo = new StackLayout
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    BackgroundColor=Color.Transparent,
                    Margin=0,
                    Spacing = 6,
                    Children =
                    {
                        box,lbl,box1
                    }
                };

                Content = sl_projectinfo;
            }
        }

        public class FooterCell : ContentView
        {
            public FooterCell()
            {
                Label lbl = new Label
                {
                    FontSize = 25,
                    HorizontalTextAlignment = TextAlignment.Start,
                    HorizontalOptions = LayoutOptions.StartAndExpand,
                    VerticalOptions = LayoutOptions.StartAndExpand,
                    VerticalTextAlignment = TextAlignment.Center,
                    FontFamily = "MYRIADPROREGULAR",
                    StyleId = "MYRIADPROREGULAR",
                    TextColor = Color.White,
                    Text = "User",
                    Margin = new Thickness(10, 0, 0, 0)
                };

                BoxView box1 = new BoxView
                {
                    BackgroundColor = Color.FromHex("#A7A9AA"),
                    HeightRequest = 0.5,
                    HorizontalOptions = LayoutOptions.FillAndExpand
                };

                var sl_projectinfo = new StackLayout
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    BackgroundColor = Color.Transparent,
                    Margin = 0,
                    Spacing =6,
                    Children =
                    {
                        lbl,box1
                    }
                };

                Content = sl_projectinfo;
            }
        }

        public class CustomCell : ViewCell
        {
            public CustomCell()
            {
                Image image = new Image
                {
                    HeightRequest = 35,
                    WidthRequest = 35
                };
                image.SetBinding(Image.SourceProperty, new Binding("Icon"));

                Label lbl = new Label
                {
                    FontSize = 18,
                    HorizontalTextAlignment = TextAlignment.Start,
                    HorizontalOptions = LayoutOptions.StartAndExpand,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    VerticalTextAlignment = TextAlignment.Center,
                    FontFamily = "MYRIADPROREGULAR",
                    StyleId = "MYRIADPROREGULAR",
                    TextColor=Color.White
                };
                lbl.SetBinding(Label.TextProperty, new Binding("Title"));

                var sl_projectinfo = new StackLayout
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    Padding = new Thickness(10, 5, 0, 5),
                    Orientation = StackOrientation.Horizontal,
                    Spacing = 10,
                    Children =
                    {
                        image,lbl
                    }
                };

                View = sl_projectinfo;
            }

        }
    }
}
