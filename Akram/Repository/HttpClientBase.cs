using Akram.CustomControls;
using Akram.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Akram.Repository
{
    public class HttpClientBase : HttpClient
    {
        private static readonly HttpClientBase _instance = new HttpClientBase();
        CancellationTokenSource cts;
        static HttpClientBase()
        {

        }
        public HttpClientBase() : base()
        {
            TimeSpan time = new TimeSpan(0, 0, 60);
            Timeout = time;
            cts = new CancellationTokenSource();
            cts.CancelAfter(time);
        }

        #region User Account

        /// <summary>
        /// This is the method that will perform the login operation in the application.
        /// we are sending the post data here which includes username, password.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<wsUser> Login(string url, List<KeyValuePair<string, string>> dict)
        {
            // making the object of the response class which will use for deserialization of the response.
            wsUser objData = new wsUser();
            try
            {
                var result = await PostAsync(new Uri(url), new FormUrlEncodedContent(dict));
                var place = result.Content.ReadAsStringAsync().Result;
                // deserialization of the response returning from the api
                objData = JsonConvert.DeserializeObject<wsUser>(await result.Content.ReadAsStringAsync());

            }

            catch (Exception ex)
            {
            }
            finally { }
            return objData; // returning the object of the response
        }


        /// <summary>
        /// This is the method that will perform the forgot password operation in the application.
        /// we are sending the post data here which includes email.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public async Task<ForgetPasswordResponse> ForgotPassword(string url, List<KeyValuePair<string, string>> dict)
        {
            // making the object of the response class which will use for deserialization of the response.
            ForgetPasswordResponse objData = new ForgetPasswordResponse();
            try
            {
                var result = await PostAsync(new Uri(url), new FormUrlEncodedContent(dict));
                var place = result.Content.ReadAsStringAsync().Result;
                // deserialization of the response returning from the api
                objData = JsonConvert.DeserializeObject<ForgetPasswordResponse>(await result.Content.ReadAsStringAsync());

            }

            catch (Exception ex)
            {
            }
            finally { }
            return objData; // returning the object of the response
        }

        public async Task<ProfileResult> GetProfileData(string url)
        {
            ProfileResult objData = new ProfileResult();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(url);
                    var result = await client.GetAsync(url);
                    var place = result.Content.ReadAsStringAsync().Result;
                    objData = JsonConvert.DeserializeObject<ProfileResult>(await result.Content.ReadAsStringAsync());
                }
            }
            catch (Exception ex)
            {
            }
            return objData;
        }

        #endregion

        #region Gifts And My Collection

        public async Task<wsGiftsResponse> GetHomePageGifts(string url)
        {
            // making the object of the response class which will use for deserialization of the response.
            wsGiftsResponse objData = new wsGiftsResponse();
            try
            {
                var result = await GetAsync(new Uri(url));
                if (result.IsSuccessStatusCode)
                {
                    var stream = result.Content.ReadAsStreamAsync().Result;
                    StreamReader reader = new StreamReader(stream);
                    string content = reader.ReadToEnd();
                    // deserialization of the response returning from the api
                    objData = JsonConvert.DeserializeObject<wsGiftsResponse>(content);
                }
            }
            catch (TaskCanceledException)
            {
            }
            catch (Exception ex)
            {

            }
            finally { }
            return objData; // returning the object of the response
        }

        public async Task<RateGiftModel> RateGift(string url, List<KeyValuePair<string, string>> dict)
        {
            // making the object of the response class which will use for deserialization of the response.
            RateGiftModel objData = new RateGiftModel();
            try
            {
                var result = await PostAsync(new Uri(url), new FormUrlEncodedContent(dict));
                var place = result.Content.ReadAsStringAsync().Result;
                // deserialization of the response returning from the api
                objData = JsonConvert.DeserializeObject<RateGiftModel>(await result.Content.ReadAsStringAsync());

            }

            catch (Exception ex)
            {
            }
            finally { }
            return objData; // returning the object of the response
        }

        #endregion

        #region Interests

        public async Task<List<InstrestResult>> GetIntrest(string url, string postData)
        {
            // making the object of the response class which will use for deserialization of the response.
            List<InstrestResult> objData = new List<InstrestResult>();
            try
            {
                StringContent str = new StringContent(postData, Encoding.UTF8, "application/x-www-form-urlencoded");
                var result = await PostAsync(new Uri(url), str);
                var place = result.Content.ReadAsStringAsync().Result;
                // deserialization of the response returning from the api
                objData = JsonConvert.DeserializeObject<List<InstrestResult>>(await result.Content.ReadAsStringAsync());

            }

            catch (Exception ex)
            {
            }
            finally { }
            return objData; // returning the object of the response
        }

        public async Task<InterestResponse> SendInterest(string url, List<KeyValuePair<string, string>> dict)
        {
            // making the object of the response class which will use for deserialization of the response.
            InterestResponse objData = new InterestResponse();
            try
            {
                var result = await PostAsync(new Uri(url), new FormUrlEncodedContent(dict));
                var place = result.Content.ReadAsStringAsync().Result;
                // deserialization of the response returning from the api
                objData = JsonConvert.DeserializeObject<InterestResponse>(await result.Content.ReadAsStringAsync());

            }

            catch (Exception)
            {
            }
            finally { }
            return objData; // returning the object of the response
        }


        public async Task<List<GetSavedInterest>> GetSelectedIntrest(string url, List<KeyValuePair<string, string>> dict)
        {
            // making the object of the response class which will use for deserialization of the response.
            List<GetSavedInterest> objData = new List<GetSavedInterest>();
            try
            {
                var result = await PostAsync(new Uri(url), new FormUrlEncodedContent(dict));
                var place = result.Content.ReadAsStringAsync().Result;
                // deserialization of the response returning from the api
                objData = JsonConvert.DeserializeObject<List<GetSavedInterest>>(await result.Content.ReadAsStringAsync());

            }

            catch (Exception ex)
            {
            }
            finally { }
            return objData; // returning the object of the response
        }

        #endregion

        #region Get Map Polylines
        public async Task<RootObjectPolyLine> GetPolyLine(double SLat, double SLng, double ELat, double ELng)
        {
            string url = "https://maps.googleapis.com/maps/api/directions/json?origin=" + SLat + "," + SLng + "&destination=" + ELat + "," + ELng + "&key=AIzaSyCAW0qqikPYbOKLu_aobSw04z1Dnfhgpv4";
            //string url = "https://maps.googleapis.com/maps/api/directions/json?origin=30.7082733333333,76.702975&destination=31.959526,76.702975&key=AIzaSyCAW0qqikPYbOKLu_aobSw04z1Dnfhgpv4";
            RootObjectPolyLine objData = new RootObjectPolyLine();
            try
            {
                using (var client = new HttpClient())
                {

                    client.BaseAddress = new Uri(url);

                    TimeSpan time = new TimeSpan(0, 0, 20);
                    client.Timeout = time;
                    var result = await client.GetAsync(url);
                    var place = result.Content.ReadAsStringAsync().Result;
                    objData = JsonConvert.DeserializeObject<RootObjectPolyLine>(await result.Content.ReadAsStringAsync());

                }
            }
            catch (Exception ex)
            {
            }
            return objData;
        }

        #endregion
    }
}
