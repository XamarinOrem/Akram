using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.Maps;

namespace Akram.CustomControls
{
    public class CustomMap : Map
    {
        public List<CustomPin> CustomPins { get; set; }
        public List<Position> RouteCoordinates { get; set; }

        public CustomCircle Circle { get; set; }

        public CustomMap()
        {
            RouteCoordinates = new List<Position>();
        }
    }

    public class CustomCircle
    {
        public Plugin.Geolocator.Abstractions.Position Position { get; set; }
        public double Radius { get; set; }
    }
}
