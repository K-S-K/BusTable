﻿namespace BusTable.Core.Models
{
    public class StopHeader
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public double Lon { get; set; }
        public double Lat { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <returns></returns>
        public double GetDistance(double lat, double lon)
        {
            /*
            The Haversine formula according to Dr. Math.
            http://mathforum.org/library/drmath/view/51879.html
                
            https://www.codeproject.com/Articles/12269/Distance-between-locations-using-latitude-and-long

            dlon = lon2 - lon1
            dlat = lat2 - lat1
            a = (sin(dlat/2))^2 + cos(lat1) * cos(lat2) * (sin(dlon/2))^2
            c = 2 * atan2(sqrt(a), sqrt(1-a)) 
            d = R * c
                
            Where
                * dlon is the change in longitude
                * dlat is the change in latitude
                * c is the great circle distance in Radians.
                * R is the radius of a spherical Earth.
                * The locations of the two points in 
                    spherical coordinates (longitude and 
                    latitude) are lon1,lat1 and lon2, lat2.
        */
            double dDistance = Double.MinValue;
            double dLat1InRad = Lat * (Math.PI / 180.0);
            double dLong1InRad = Lon * (Math.PI / 180.0);
            double dLat2InRad = lat * (Math.PI / 180.0);
            double dLong2InRad = lon * (Math.PI / 180.0);

            double dLongitude = dLong2InRad - dLong1InRad;
            double dLatitude = dLat2InRad - dLat1InRad;

            // Intermediate result a.
            double a = Math.Pow(Math.Sin(dLatitude / 2.0), 2.0) +
                       Math.Cos(dLat1InRad) * Math.Cos(dLat2InRad) *
                       Math.Pow(Math.Sin(dLongitude / 2.0), 2.0);

            // Intermediate result c (great circle distance in Radians).
            double c = 2.0 * Math.Asin(Math.Sqrt(a));

            // Distance.
            // const Double kEarthRadiusMiles = 3956.0;
            const Double kEarthRadiusKms = 6376.5;
            dDistance = kEarthRadiusKms * c;

            return dDistance;
        }

        public override string ToString() => $"[{Id}] \"{Name}\"";
    }
}