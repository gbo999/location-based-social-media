using com.draconianmarshmallows.geofire.util;
using UnityEngine;

namespace com.draconianmarshmallows.geofire.core
{
    public class GeoHash
    {
        private string geoHash;
        
        private static int DEFAULT_PRECISION = 10; // The default precision of a geohash. 
        
        public static int MAX_PRECISION = 22; // The maximal precision of a geohash. 

        // The maximal number of bits precision for a geohash:
        public static int MAX_PRECISION_BITS = MAX_PRECISION * Base32Utils.BITS_PER_BASE32_CHAR;

        public GeoHash(double latitude, double longitude) {
            initialize(latitude, longitude, DEFAULT_PRECISION);
        }

        public GeoHash(GeoLocation location) {
            initialize(location.latitude, location.longitude, DEFAULT_PRECISION);
        }

        public GeoHash(double latitude, double longitude, int precision) {
            initialize(latitude, longitude, precision);
        }

        private void initialize(double latitude, double longitude, int precision)
        {
            if (precision < 1)
                throw new UnityException("Precision of GeoHash must be larger than zero!");

            if (precision > MAX_PRECISION)
                throw new UnityException("Precision of a GeoHash must be less than " + (MAX_PRECISION + 1) + "!");

            if (!GeoLocation.coordinatesValid(latitude, longitude))
                throw new UnityException(string.Format("Not valid location coordinates: [%f, %f]", latitude, longitude));

            double[] longitudeRange = { -180, 180 };
            double[] latitudeRange = { -90, 90 };

            char[] buffer = new char[precision];

            for (int i = 0; i < precision; i++) {
                int hashValue = 0;
                for (int j = 0; j < Base32Utils.BITS_PER_BASE32_CHAR; j++) {
                    bool even = (((i * Base32Utils.BITS_PER_BASE32_CHAR) + j) % 2) == 0;
                    double val = even ? longitude : latitude;
                    double[] range = even ? longitudeRange : latitudeRange;
                    double mid = (range[0] + range[1]) / 2;
                    if (val > mid) {
                        hashValue = (hashValue << 1) + 1;
                        range[0] = mid;
                    } else {
                        hashValue = (hashValue << 1);
                        range[1] = mid;
                    }
                }
                buffer[i] = Base32Utils.valueToBase32Char(hashValue);
            }
            this.geoHash = new string(buffer);
        }

        public GeoHash(string hash) {
            if (hash.Length == 0 || !Base32Utils.isValidBase32String(hash)) {
                throw new UnityException("Not a valid geoHash: " + hash);
            }
            this.geoHash = hash;
        }

        public string getGeoHashString() {
            return geoHash;
        }

        public bool equals(object o) {
            if (this == o) return true;
            if (o == null || GetType() != o.GetType()) return false;

            GeoHash other = (GeoHash)o;

            return geoHash.Equals(other.geoHash);
        }
        
        public string toString() {
            return "GeoHash{" +
                    "geoHash='" + geoHash + '\'' +
                    '}';
        }
        
        public int hashCode() {
            return geoHash.GetHashCode();
        }
    }
}
