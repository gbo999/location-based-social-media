using com.draconianmarshmallows.geofire;
using com.draconianmarshmallows.geofire.util;
using System;
using System.Collections.Generic;

[Serializable] public class DbLocation
{
    public string hash;
    public GeoLocation location;
    public BaseLocationContent content;

    public DbLocation(string geoHash, GeoLocation location, BaseLocationContent content)
    {
        this.hash = geoHash;
        this.location = location;
        this.content = content;
    }

    public Dictionary<string, object> toMap()
    {
        var locationDic = new Dictionary<string, object>
        {
            { Constants.KEY_LON, location.longitude },
            { Constants.KEY_LAT, location.latitude },
        };

        var dictionary = new Dictionary<string, object>
        {
            { Constants.KEY_HASH, hash },
            { Constants.KEY_LOCATION, locationDic },
        };
        if (content != null) dictionary.Add(Constants.KEY_CONTENT, content.toMap());
        return dictionary;
    }
}
