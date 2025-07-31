using com.draconianmarshmallows.geofire.util;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.draconianmarshmallows.geofire
{
    /**
     * A wrapper class for location coordinates.
     */
    [Serializable] public class GeoLocation
    {
        /** The latitude of this location in the range of [-90, 90] */
        public double latitude;

        /** The longitude of this location in the range of [-180, 180] */
        public double longitude;

        public double altitude;
        public BaseLocationContent content;

        /**
         * Creates a new GeoLocation with the given latitude and longitude.
         *
         * @throws java.lang.IllegalArgumentException If the coordinates are not valid geo coordinates
         * @param latitude The latitude in the range of [-90, 90]
         * @param longitude The longitude in the range of [-180, 180]
         */
        public GeoLocation(double latitude, double longitude) {
            initialize(latitude, longitude, null);
        }

        public GeoLocation(double latitude, double longitude, BaseLocationContent content)
        {
            initialize(latitude, longitude, content);
        }

        private void initialize(double latitude, double longitude, 
            BaseLocationContent content)
        {
            if (!coordinatesValid(latitude, longitude))
            {
                throw new UnityException("Not a valid geo location: " + latitude + ", "
                    + longitude);
            }
            this.latitude = latitude;
            this.longitude = longitude;
            this.content = content;
        }

        public virtual Dictionary<string, object> toMap()
        {
            var dictionary = new Dictionary<string, object>();

            var locationDic = new Dictionary<string, object>();

            locationDic.Add(Constants.KEY_LAT, latitude);
            locationDic.Add(Constants.KEY_LON, longitude);
            dictionary.Add(Constants.KEY_LOCATION, locationDic);
            dictionary.Add(Constants.KEY_CONTENT, content.toMap());
            return dictionary;
        }

        /**
         * Checks if these coordinates are valid geo coordinates.
         * @param latitude The latitude must be in the range [-90, 90]
         * @param longitude The longitude must be in the range [-180, 180]
         * @return True if these are valid geo coordinates
         */
        public static bool coordinatesValid(double latitude, double longitude)
        {
            return (latitude >= -90 && latitude <= 90 && longitude >= -180 && longitude <= 180);
        }

        public bool equals(object o)
        {
            if (this == o) return true;
            if (o == null || GetType() != o.GetType()) return false;

            GeoLocation that = (GeoLocation) o;

            if (compare(that.latitude, latitude) != 0) return false;
            if (compare(that.longitude, longitude) != 0) return false;

            return true;
        }
        
        public int hashCode()
        {
            int result;
            long temp;
            temp = BitConverter.DoubleToInt64Bits(latitude);
            result = (int) (temp ^ (temp >> 32));
            temp = BitConverter.DoubleToInt64Bits(longitude);
            result = 31 * result + (int) (temp ^ (temp >> 32));
            return result;
        }
        
        public String toString() {
            return "GeoLocation(" + latitude + ", " + longitude + ")";
        }

        public static int compare(double d1, double d2)
        {
            if (d1 < d2) return -1;           // Neither val is NaN, thisVal is smaller
            if (d1 > d2) return 1;            // Neither val is NaN, thisVal is larger
                    
           long thisBits = BitConverter.DoubleToInt64Bits(d1);
           long anotherBits = BitConverter.DoubleToInt64Bits(d2);
           
            return (thisBits == anotherBits ? 0 :   // Values are equal
                (thisBits < anotherBits ? -1 :      // (-0.0, 0.0) or (!NaN, NaN)
                    1));                            // (0.0, -0.0) or (NaN, !NaN)
        }
    }
}
