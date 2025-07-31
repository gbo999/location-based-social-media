using com.draconianmarshmallows.geofire.util;
using System.Collections.Generic;
using UnityEngine;

public class BaseLocationContent
{
    public int type = Constants.OBJECT_TYPE_UNKNOWN;

    public int contentType
    {
        get { return type; }
        set { type = value; }
    }

    public virtual Dictionary<string, object> toMap()
    {
        var map = new Dictionary<string, object>();
        map.Add(Constants.KEY_TYPE, type);
        return map;
    }

    public override string ToString()
    {
        return JsonUtility.ToJson(this);
    }
}
