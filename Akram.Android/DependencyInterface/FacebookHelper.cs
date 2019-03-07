using System;
using Akram.DependencyInterface;
using Akram.Droid.DependencyInterface;
using Akram.Models;
using Android.Content;
using Newtonsoft.Json.Linq;
using Xamarin.Auth;
using Xamarin.Forms;
using static Akram.Models.FacebookResponse;

[assembly: Dependency(typeof(FacebookHelper))]
namespace Akram.Droid.DependencyInterface
{
    public class FacebookHelper : IFacebookHelper
    {
        private string data;
        private string profilePicture;

        public void Start(Action<FacebookResponse> callback)
        {
            var auth = new OAuth2Authenticator(
               clientId: "1050522261786692",
               scope: "public_profile,email",
               authorizeUrl: new Uri("https://m.facebook.com/dialog/oauth/"),
               redirectUrl: new Uri("https://support.google.com/a/answer/60216?hl=en"));
            auth.Completed += (sender, eventArgs) =>
            {

                // We presented the UI, so it's up to us to dimiss it on iOS.
                //DismissViewController(true, null);
                try
                {
                    if (eventArgs.IsAuthenticated)
                    {
                        var request = new OAuth2Request("GET", new Uri("https://graph.facebook.com/me?fields=email,name,first_name,last_name"), null, eventArgs.Account);
                        request.GetResponseAsync().ContinueWith(t =>
                        {
                            if (t.IsFaulted)
                            {
                                callback?.Invoke(new FacebookResponse(ResponseCode.Fail, null,null));
                            }
                            else
                            {
                                data = t.Result.GetResponseText();
                                var obj = JObject.Parse(data);
                                var facebookId = obj["id"].ToString();
                                var request1 = new OAuth2Request("GET", new Uri("https://graph.facebook.com/"+facebookId+"/?fields=picture&type=large"), null, eventArgs.Account);

                                request1.GetResponseAsync().ContinueWith(k =>
                                {
                                    if(k.IsFaulted)
                                    {
                                        callback?.Invoke(new FacebookResponse(ResponseCode.Fail, null,null));
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
                        callback?.Invoke(new FacebookResponse(ResponseCode.Fail, null,null));
                    }
                }
                catch (Exception)
                {
                    
                }
            };
            var context = Android.App.Application.Context;
            Intent myIntent = auth.GetUI(context);
            myIntent.SetFlags(ActivityFlags.NewTask);
            context.StartActivity(myIntent);
        }
    }
}