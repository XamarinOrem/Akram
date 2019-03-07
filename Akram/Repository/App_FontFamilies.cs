using System;
using Xamarin.Forms;

namespace Akram.Repository
{
    public class App_FontFamilies
    {

        /// <summary>
        /// for page titles
        /// </summary>
        /// <value>The page titles.</value>
        public static string PageTitles
        {
            get
            {
                if (Device.RuntimePlatform == "iOS")
                {
                    return "MYRIADPROBOLD";
                }
                else
                {
                    return "MYRIADPROBOLD";
                }


            }

        }

        /// <summary>
        /// PageDiscription 
        /// </summary>
        /// <value>The page titles.</value>


        public static string PageTextRegular
        {
            get
            {
                if (Device.RuntimePlatform == "iOS")
                {
                    return "MYRIADPROREGULAR";
                }
                else
                {
                    return "MYRIADPROREGULAR";
                }


            }

        }

    }
}
