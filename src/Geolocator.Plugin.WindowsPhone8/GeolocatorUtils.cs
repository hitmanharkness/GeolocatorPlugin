using Microsoft.Phone.Maps.Services;
using Plugin.Geolocator.Abstractions;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;

namespace Plugin.Geolocator
{
    static class GeolocatorUtils
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

        internal static GeoPositionAccuracy ToAccuracy(this double desiredAccuracy)
        {
            if (desiredAccuracy < 100)
                return GeoPositionAccuracy.High;

            return GeoPositionAccuracy.Default;
        }

        internal static Position ToPosition(this GeoPosition<GeoCoordinate> position)
        {
            if (position.Location.IsUnknown)
                return null;

            var p = new Position();
            p.Accuracy = position.Location.HorizontalAccuracy;
            p.Longitude = position.Location.Longitude;
            p.Latitude = position.Location.Latitude;

            if (!double.IsNaN(position.Location.VerticalAccuracy) && !double.IsNaN(position.Location.Altitude))
            {
                p.AltitudeAccuracy = position.Location.VerticalAccuracy;
                p.Altitude = position.Location.Altitude;
            }

            if (!double.IsNaN(position.Location.Course))
                p.Heading = position.Location.Course;

            if (!double.IsNaN(position.Location.Speed))
                p.Speed = position.Location.Speed;

            p.Timestamp = position.Timestamp.ToUniversalTime();

            return p;
        }

        internal static IEnumerable<Address> ToAddresses(this IEnumerable<MapLocation> addresses)
        {
            return addresses.Select(address => new Address
            {
                Longitude = address.GeoCoordinate.Longitude,
                Latitude = address.GeoCoordinate.Latitude,
                FeatureName = address.Information.Name,
                PostalCode = address.Information.Address.PostalCode,
                CountryCode = address.Information.Address.CountryCode,
                CountryName = address.Information.Address.Country,
                Thoroughfare = address.Information.Address.Street,
                SubThoroughfare = address.Information.Address.Township,
                Locality = address.Information.Address.City
            });
        }
    }
}
