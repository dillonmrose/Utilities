using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Geospatial
{
    class LatLong
    {
        public double latitude, longitude;
 
        public LatLong()
        {
            latitude = 0;
            longitude = 0;
        }
        public LatLong(double x, double y)
        {
            latitude = x;
            longitude = y;
        }
        public LatLong(string x, string y)
        {
            latitude = double.Parse(x);
            longitude = double.Parse(y);
        }
    }
}
