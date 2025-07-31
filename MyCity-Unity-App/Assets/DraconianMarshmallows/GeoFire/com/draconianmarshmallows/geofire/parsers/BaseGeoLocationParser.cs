using System;
using com.draconianmarshmallows.geofire.util;
using Firebase.Database;

namespace com.draconianmarshmallows.geofire.parsers
{
    public class BaseGeoLocationParser
    {
        public virtual GeoLocation parse(DataSnapshot dataSnapshot)
        {
            var location = dataSnapshot.Child(Constants.KEY_LOCATION);
            var latValue = location.Child(Constants.KEY_LAT).Value;
            var lonValue = location.Child(Constants.KEY_LON).Value;
            double latitude = double.Parse(latValue.ToString());
            double longitude = double.Parse(lonValue.ToString());
            return new GeoLocation(latitude, longitude, parseContent(dataSnapshot));
        }

        protected virtual BaseLocationContent parseContent(DataSnapshot dataSnapshot)
        {
            return null;
        }
    }
}
