using System;
using System.Collections.Generic;
using System.Text;

namespace Akram.Models
{
    public class FacebookResponse
    {
        public enum ResponseCode
        {
            OK, Fail
        }
        public FacebookResponse(ResponseCode code, string result,string profilePicture)
        {
            Code = code;
            Result = result;
            ProfilePic = profilePicture;
        }
        public FacebookResponse(ResponseCode code, string result,string profilePic, Exception error)
            : this(code, result, profilePic)
        {
            this.Error = error;
        }
        public string Result { get; set; }
        public string ProfilePic { get; set; }
        public ResponseCode Code { get; set; }
        public Exception Error { get; set; }
    }

    public class Data
    {
        public int height { get; set; }
        public bool is_silhouette { get; set; }
        public string url { get; set; }
        public int width { get; set; }
    }

    public class Picture
    {
        public Data data { get; set; }
    }

    public class FacebookProfilePicture
    {
        public Picture picture { get; set; }
        public string id { get; set; }
    }
}
