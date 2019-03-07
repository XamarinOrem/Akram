using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Akram.DependencyInterface;
using Akram.iOS.DependencyInterface;
using Akram.Models;
using Foundation;
using Newtonsoft.Json.Linq;
using UIKit;
using Xamarin.Auth;
using static Akram.Models.FacebookResponse;

[assembly: Xamarin.Forms.Dependency(typeof(FacebookHelper))]
namespace Akram.iOS.DependencyInterface
{
    public class FacebookHelper : IFacebookHelper
    {
        private string data;
        private string profilePicture;

        public void Start(Action<FacebookResponse> callback)
        {
            try
            {
                var auth = new OAuth2Authenticator(
                clientId: "1050522261786692",
                scope: "public_profile,email",
                authorizeUrl: new Uri("https://m.facebook.com/dialog/oauth/"),
                redirectUrl: new Uri("https://support.google.com/a/answer/60216?hl=en"));

                var firstController = UIApplication.SharedApplication.KeyWindow.RootViewController.ChildViewControllers.First().ChildViewControllers.Last().ChildViewControllers.First();

                //var firstController = UIApplication.SharedApplication.KeyWindow.RootViewController;
                //firstController.PresentViewController((UIViewController)auth.GetUI(), true, null);


                auth.Completed += (sender, eventArgs) =>
                {
                    // DismissViewController(true, null);

                    if (eventArgs.IsAuthenticated)
                    {
                        var request = new OAuth2Request("GET", new Uri("https://graph.facebook.com/me?fields=email,name,first_name,last_name"), null, eventArgs.Account);
                        request.GetResponseAsync().ContinueWith(t =>
                        {
                            if (t.IsFaulted)
                            {
                                callback?.Invoke(new FacebookResponse(ResponseCode.Fail, null, string.Empty));
                            }
                            else
                            {
                                data = t.Result.GetResponseText();
                                var obj = JObject.Parse(data);
                                var facebookId = obj["id"].ToString();
                                var request1 = new OAuth2Request("GET", new Uri("https://graph.facebook.com/" + facebookId + "/?fields=picture&type=large"), null, eventArgs.Account);

                                request1.GetResponseAsync().ContinueWith(k =>
                                {
                                    if (k.IsFaulted)
                                    {
                                        callback?.Invoke(new FacebookResponse(ResponseCode.Fail, null, null));
                                    }
                                    else
                                    {
                                        profilePicture = k.Result.GetResponseText();
                                        callback?.Invoke(new FacebookResponse(ResponseCode.OK, data, profilePicture));
                                    }
                                });
                            }
                        });
                    }
                    else
                    {
                        callback?.Invoke(new FacebookResponse(ResponseCode.Fail, null, string.Empty));
                    }
                };

                firstController.PresentViewController(auth.GetUI(), true, null);

            }
            catch (Exception ex)
            {

            }
        }
    }
}