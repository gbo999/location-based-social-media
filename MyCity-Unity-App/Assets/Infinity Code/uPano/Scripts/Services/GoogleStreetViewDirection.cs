/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using InfinityCode.uPano.Json;
using UnityEngine;

namespace InfinityCode.uPano.Services
{
    public class GoogleStreetViewDirection
    {
        public double altitude;
        public string id;
        public double longitude;
        public double latitude;
        public string title;
        public double distance;
        public float pan;
        public JSONArray json;

        public bool broken;

        public GoogleStreetViewDirection(JSONArray json, double currentLongitude, double currentLatitude)
        {
            this.json = json;
            JSONItem longitudeNode = json[2, 0, 3];
            if (longitudeNode != null) longitude = longitudeNode.V<double>();
            else
            {
                broken = true;
                return;
            }
            JSONItem latitudeNode = json[2, 0, 2];
            if (latitudeNode != null) latitude = latitudeNode.V<double>();
            else
            {
                broken = true;
                return;
            }
            JSONItem altitudeNode = json[2, 1, 0];
            if (altitudeNode != null) altitude = altitudeNode.V<double>();

            double tx1, tx2, ty1, ty2;
            GeoHelper.CoordinatesToTile(currentLongitude, currentLatitude, 20, out tx1, out ty1);
            GeoHelper.CoordinatesToTile(longitude, latitude, 20, out tx2, out ty2);

            pan = 90 + (float)MathHelper.Angle2D(tx1, ty1, tx2, ty2);

            if (json.count > 3) title = json[3, 2, 0, 0].V<string>();

            id = json[0, 1].V<string>();

            distance = GeoHelper.Distance(currentLongitude, currentLatitude, longitude, latitude);
        }
    }
}