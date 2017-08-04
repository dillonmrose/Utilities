using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Geospatial
{
    class BoundingBox
    {
        LatLong northWest, southEast;
        public BoundingBox(LatLong nw, LatLong se)
        {
            northWest = nw;
            southEast = se;
        }
        public List<LatLong> getRandomPoints(int count)
        {
            Random rng = new Random();
            List<LatLong> randomPoints = new List<LatLong>();
            for (int i = 0; i < count; i++)
            {
                LatLong randomPoint = new LatLong();
                randomPoint.latitude = (northWest.latitude - southEast.latitude) * rng.NextDouble() + southEast.latitude;
                randomPoint.longitude = (southEast.longitude - northWest.longitude) * rng.NextDouble() + northWest.longitude;
                randomPoints.Add(randomPoint);
            }
            return randomPoints;
        }

    }
}
