using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Akram.Models
{
    public class SaveLoginResponse
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string UserId { get; set; }

        public string LoginHash { get; set; }

        public string FullName { get; set; }

        public bool IsLoggedIn { get; set; }

        public bool IsLoggedInFacebook { get; set; }

        public string FacebookProfilePicture { get; set; }
    }

    public class LoginUserDetails
    {
        public static string UserName { get; set; }

        public static string Email { get; set; }

        public static string Password { get; set; }

        public static string UserId { get; set; }

        public static bool IsLoggedInFacebook { get; set; }

        public static string LoginHash { get; set; }

        public static bool IsLoggedIn { get; set; }

        public static string FullName { get; set; }

        public static string FacebookProfilePicture { get; set; }
    }

    public class ForgetPasswordResponse
    {
        public Status status { get; set; }
    }
}
