using System;
using System.Collections.Generic;
using System.Text;

namespace Akram.Models
{
    public class InstrestResult
    {
        public string id { get; set; }

        public string name { get; set; }
    }

    public class InterestResponse
    {
        public Status status { get; set; }
        public List<string> category { get; set; }
    }

    public class GetSavedInterest
    {
        public string __invalid_name__0 { get; set; }
        public string category_id { get; set; }
    }
}
