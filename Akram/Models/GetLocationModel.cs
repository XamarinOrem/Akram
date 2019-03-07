using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Akram.Models
{
    public class GetLocationModel
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public string Name { get; set; }

        public string LatLong { get; set; }
    }
}
