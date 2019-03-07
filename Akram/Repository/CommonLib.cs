using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Akram.CustomControls;
using Akram.Models;
using Akram.Views;
using Newtonsoft.Json;
using Plugin.Connectivity;
using Plugin.Geolocator;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Xamarin.Forms.Maps;

namespace Akram.Repository
{
    public class CommonLib
    {
        public static string someErrorMsg = "Something went wrong, Please try again.";

        public static string apiKey = "450908816KGdcae2aYMK";

        public static bool CheckConnection()
        {
            var con = CrossConnectivity.Current.IsConnected;
            return con == true ? true : false;
        }

        /// <summary>
        /// Checks the valid email.
        /// </summary>
        /// <returns><c>true</c>, if valid email was checked, <c>false</c> otherwise.</returns>
        /// <param name="Email">Email.</param>
        public static bool CheckValidEmail(string Email)
        {
            bool isEmail = Regex.IsMatch(Email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
            return isEmail;
        }

        /// <summary>
        /// Method for checking that the username does not contains spaces and special characters
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        public static bool CheckValidUserName(string UserName)
        {
            bool isCorrect = Regex.IsMatch(UserName, @"^(\d|\w)+$", RegexOptions.IgnoreCase);
            return isCorrect;
        }

        /// <summary>
        /// Method for cheking returning data is url or not.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool CheckUrl(string url)
        {
            bool isUri = Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute);
            return isUri;
        }


        /// <summary>
        /// Method that will returns the latitude and longitude for the given address.
        /// </summary>
        /// <param name="Address"></param>
        /// <returns></returns>
        public static async Task<Plugin.Geolocator.Abstractions.Position> GetLatLong()      
        {
            Plugin.Geolocator.Abstractions.Position position;
            var locator = CrossGeolocator.Current;

            locator.DesiredAccuracy = 120;

            if (!CrossGeolocator.Current.IsGeolocationEnabled)
                return null;
            if (!CrossGeolocator.Current.IsGeolocationAvailable)
                return null;

            try
            {
                position = await locator.GetPositionAsync(TimeSpan.FromSeconds(10));
            }
            catch (Exception ex)
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
                if (status != PermissionStatus.Granted)
                {
                    LoaderPopup.CloseAllPopup();
                }
                return null;
            }

            return position;
        }

        /// <summary>
        /// Method for converting Stream Image into Base64 String
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string ConvertToBase64(Stream stream)
        {
            byte[] bytes;
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                bytes = memoryStream.ToArray();
            }

            string base64 = Convert.ToBase64String(bytes);
            return base64;
        }

        /// <summary>
        /// Method for getting Stream from the local image.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static Stream GetStreamFromLocalImage(string fileName)
        {
            var assembly = typeof(App).GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream(fileName);
            return stream;
        }

        /// <summary>
        /// Converting Stream into byte array
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Make first char of input to upper case
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string FirstCharToUpper(string input)
        {
            switch (input)
            {
                case null: throw new ArgumentNullException(nameof(input));
                case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
                default: return input.First().ToString().ToUpper() + input.Substring(1);
            }
        }

        /// <summary>
        /// Method for decoding the polylines in the map section
        /// </summary>
        /// <param name="encodedPoints"></param>
        /// <returns></returns>

        public static List<PloyLineRoutes> DecodePoly(string encodedPoints)
        {
            if (encodedPoints == null || encodedPoints == "") return null;
            List<PloyLineRoutes> poly = new List<PloyLineRoutes>();
            char[] polylinechars = encodedPoints.ToCharArray();
            int index = 0;

            int currentLat = 0;
            int currentLng = 0;
            int next5bits;
            int sum;
            int shifter;

            try
            {
                while (index < polylinechars.Length)
                {
                    // calculate next latitude
                    sum = 0;
                    shifter = 0;
                    do
                    {
                        next5bits = (int)polylinechars[index++] - 63;
                        sum |= (next5bits & 31) << shifter;
                        shifter += 5;
                    } while (next5bits >= 32 && index < polylinechars.Length);

                    if (index >= polylinechars.Length)
                        break;

                    currentLat += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);

                    //calculate next longitude
                    sum = 0;
                    shifter = 0;
                    do
                    {
                        next5bits = (int)polylinechars[index++] - 63;
                        sum |= (next5bits & 31) << shifter;
                        shifter += 5;
                    } while (next5bits >= 32 && index < polylinechars.Length);

                    if (index >= polylinechars.Length && next5bits >= 32)
                        break;

                    currentLng += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);
                    PloyLineRoutes p = new PloyLineRoutes();
                    p.Latitude = Convert.ToDouble(currentLat) / 100000.0;
                    p.Longitude = Convert.ToDouble(currentLng) / 100000.0;
                    poly.Add(p);
                }
            }
            catch (Exception)
            {
                // logo it
            }
            return poly;
        }
    }
}
