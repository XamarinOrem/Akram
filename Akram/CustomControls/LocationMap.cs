using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.Maps;

namespace Akram.CustomControls
{
    public class LocationMap:Map
    {
        public List<CustomPin> CustomPins { get; set; }

        public List<Position> RouteCoordinates { get; set; }
    }
}
