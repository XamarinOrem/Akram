using System;
using System.Collections.Generic;
using System.Text;

namespace Akram.Repository
{
    public class ApiUrl
    {
        public const string wsBaseUrl = "https://www.joserv.org/pokmon/rest/";

        #region User Account

        public const string LoginUrl = wsBaseUrl + "login.php";

        public const string SocialLoginUrl = wsBaseUrl + "social_register.php";

        public const string RegisterUrl = wsBaseUrl + "register_user.php";

        public const string ForgotPasswordUrl = wsBaseUrl + "forget_mail.php";

        public const string ViewProfile = wsBaseUrl + "get_user.php?";

        public const string ResetPasswordUrl = wsBaseUrl + "reset_password.php";

        public const string UpdateProfile = wsBaseUrl + "update_user_profile.php?";

        #endregion

        #region Gifts And Collection  

        public const string GetHomePageGiftsUrl = wsBaseUrl + "get_pokemons.php";

        public const string RateGiftUrl = wsBaseUrl + "rate_gift.php";

        public const string DeleteGiftUrl = wsBaseUrl + "delete_one_sighting.php";

        #endregion

        #region Interests

        public const string GetCategory = wsBaseUrl + "get_category.php";

        public const string SendInterest = wsBaseUrl + "user_interest.php";

        #endregion
    }
}
