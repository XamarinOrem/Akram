using System;
using System.Collections.Generic;
using System.Text;

namespace Akram.Models
{
    public class wsUser
    {
        public UserInfo user_info { get; set; }
        public Status status { get; set; }
    }

    public class UserInfo
    {
        public string user_id { get; set; }
        public string login_hash { get; set; }
        public string facebook_id { get; set; }
        public string twitter_id { get; set; }
        public string google_id { get; set; }
        public string full_name { get; set; }
        public string thumb_url { get; set; }
        public string email { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string team { get; set; }
    }
}
