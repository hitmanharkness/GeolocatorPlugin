using Android.Locations;
using Plugin.Geolocator.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using Address = Plugin.Geolocator.Abstractions.Address;

namespace Plugin.Geolocator
{
    public static class GeolocationUtils
    {

        #region GEO NAVIGATION FUNCTIONS
        // MY JUNK
        // https://software.intel.com/en-us/blogs/2012/11/30/calculating-a-bearing-between-points-in-location-aware-apps

        static Double degToRad = Math.PI / 180.0;

        static public Double Bearing(Double lat1, Double long1, Double lat2, Double long2)
        {
            return (_bearing(lat1, long1, lat2, long2) + 360.0) % 360;
        }

        static public Double initial(Double lat1, Double long1, Double lat2, Double long2)
        {
            return (_bearing(lat1, long1, lat2, long2) + 360.0) % 360;
        }

        static public Double final(Double lat1, Double long1, Double lat2, Double long2)
        {
            return (_bearing(lat2, long2, lat1, long1) + 180.0) % 360;
        }

        static private Double _bearing(Double lat1, Double long1, Double lat2, Double long2)
        {
            Double phi1 = lat1 * degToRad;
            Double phi2 = lat2 * degToRad;
            Double lam1 = long1 * degToRad;
            Double lam2 = long2 * degToRad;

            return Math.Atan2(Math.Sin(lam2 - lam1) * Math.Cos(phi2),
              Math.Cos(phi1) * Math.Sin(phi2) - Math.Sin(phi1) * Math.Cos(phi2) * Math.Cos(lam2 - lam1)
            ) * 180 / Math.PI;
        }

        // http://www.geodatasource.com/developers/c-sharp
        static public Double Distance(double lat1, double lon1, double lat2, double lon2, char unit)
        {
            double theta = lon1 - lon2;
            double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
            dist = Math.Acos(dist);
            dist = rad2deg(dist);
            dist = dist * 60 * 1.1515;
            if (unit == 'K')
            {
                dist = dist * 1.609344;
            }
            else if (unit == 'N')
            {
                dist = dist * 0.8684;
            }
            return (dist);
        }
        static private double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }
        static private double rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }
        #endregion


        static int TwoMinutes = 120000;

        internal static bool IsBetterLocation(Location location, Location bestLocation)
        {

            if (bestLocation == null)
                return true;

            var timeDelta = location.Time - bestLocation.Time;
            var isSignificantlyNewer = timeDelta > TwoMinutes;
            var isSignificantlyOlder = timeDelta < -TwoMinutes;
            var isNewer = timeDelta > 0;

            if (isSignificantlyNewer)
                return true;

            if (isSignificantlyOlder)
                return false;

            var accuracyDelta = (int)(location.Accuracy - bestLocation.Accuracy);
            var isLessAccurate = accuracyDelta > 0;
            var isMoreAccurate = accuracyDelta < 0;
            var isSignificantlyLessAccurage = accuracyDelta > 200;

            var isFromSameProvider = IsSameProvider(location.Provider, bestLocation.Provider);

            if (isMoreAccurate)
                return true;

            if (isNewer && !isLessAccurate)
                return true;

            if (isNewer && !isSignificantlyLessAccurage && isFromSameProvider)
                return true;

            return false;


        }

        internal static bool IsSameProvider(string provider1, string provider2)
        {
            if (provider1 == null)
                return provider2 == null;

            return provider1.Equals(provider2);
        }

        internal static Position ToPosition(this Location location)
        {
            var p = new Position();
            if (location.HasAccuracy)
                p.Accuracy = location.Accuracy;
            if (location.HasAltitude)
                p.Altitude = location.Altitude;
            if (location.HasBearing)
                p.Heading = location.Bearing;
            if (location.HasSpeed)
                p.Speed = location.Speed;

            p.Longitude = location.Longitude;
            p.Latitude = location.Latitude;
            p.Timestamp = location.GetTimestamp();
            return p;
        }

        internal static IEnumerable<Address> ToAddresses(this IEnumerable<Android.Locations.Address> addresses)
        {
            return addresses.Select(address => new Address
            {
                Longitude = address.Longitude,
                Latitude = address.Latitude,
                FeatureName = address.FeatureName,
                PostalCode = address.PostalCode,
                SubLocality = address.SubLocality,
                CountryCode = address.CountryCode,
                CountryName = address.CountryName,
                Thoroughfare = address.Thoroughfare,
                SubThoroughfare = address.SubThoroughfare,
                Locality = address.Locality
            });
        }

        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        internal static DateTimeOffset GetTimestamp(this Location location)
        {
            try
            {
                return new DateTimeOffset(Epoch.AddMilliseconds(location.Time));
            }
            catch (Exception e)
            {
                return new DateTimeOffset(Epoch);
            }
        }
    }
}






