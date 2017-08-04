using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Geospatial;

namespace Tools.Geospatial
{
    class Geospatial
    {
        private static double GetRadius(double d)
        {
            return d * Math.PI / 180.0;
        }
        public static double CalcDistance(LatLong from, LatLong to)
        {
            double EARTH_RADIUS = 6371; //3,959 miles (6,371 km)
            double distance;
            double radLat1 = GetRadius(from.latitude);
            double radLat2 = GetRadius(to.latitude);
            double a = radLat1 - radLat2;
            double b = GetRadius(from.longitude) - GetRadius(from.longitude);

            distance = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) + Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2)));
            distance = distance * EARTH_RADIUS;
            distance = Math.Round(distance * 10000) / 10000;

            return distance;
        }
    }
}
