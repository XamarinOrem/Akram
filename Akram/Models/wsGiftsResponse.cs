using System;
using System.Collections.Generic;
using System.Text;

namespace Akram.Models
{
    public class wsGiftsResponse
    {
        public int result_count { get; set; }
        public List<Sighting> sightings { get; set; }
        public string max_distance { get; set; }
        public string default_distance { get; set; }
        public Status status { get; set; }
    }

    public class Sighting
    {
        public double rating { get; set; }
        public List<string> rules { get; set; }
        public string pokemon_id { get; set; }
        public User user { get; set; }
        public string name { get; set; }
        public string image { get; set; }
        public string type { get; set; }
        public string category_id { get; set; }
        public string repetition { get; set; }
        public string sighting_id { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string lat { get; set; }
        public string lon { get; set; }
        public string user_id { get; set; }
        public string date_found { get; set; }
        public string visible { get; set; }
        public string distance { get; set; }
    }

    public class User
    {
        public string user_id { get; set; }
        public string full_name { get; set; }
        public string gender { get; set; }
        public string score { get; set; }
        public string type { get; set; }
        public string role { get; set; }
        public string facebook_profile { get; set; }
        public string insta_profile { get; set; }
        public string phone { get; set; }
        public string thumb_url { get; set; }
        public string facebook_id { get; set; }
        public string twitter_id { get; set; }
        public string google_id { get; set; }
        public string login_hash { get; set; }
        public string team { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string deny_access { get; set; }
    }

    public class Status
    {
        public string status_code { get; set; }
        public string status_text { get; set; }
    }
}
