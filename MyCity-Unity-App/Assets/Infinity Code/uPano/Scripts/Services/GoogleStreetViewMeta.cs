/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using System.Collections.Generic;
using System.Linq;
using InfinityCode.uPano.Json;
using UnityEngine;

namespace InfinityCode.uPano.Services
{
    public class GoogleStreetViewMeta
    {
        private double? _altitude;
        private string _data;
        private string _id;
        private JSONItem _json;
        private double? _latitude;
        private double? _longitude;
        private JSONItem _nextItems;
        private float? _northPan;
        private bool _requestById;
        private GoogleStreetViewDirection[] _directions;
        private GoogleStreetViewDirection[] _nearestDirections;

        public double altitude
        {
            get
            {
                if (!_altitude.HasValue && nextItems != null) _altitude = nextItems[0, 2, 1, 0].V<double>();
                return _altitude.GetValueOrDefault();
            }
        }

        public GoogleStreetViewDirection[] directions
        {
            get
            {
                if (_directions == null && nextItems != null)
                {
                    List<GoogleStreetViewDirection> items = new List<GoogleStreetViewDirection>();

                    int count = (nextItems as JSONArray).count;

                    JSONItem prevPeriods = _requestById? json[1, 0, 5, 0, 8] : json[1, 5, 0, 8];
                    if (prevPeriods != null && !prevPeriods.Equals(null)) count = prevPeriods[0, 0].V<int>();

                    for (int i = 1; i < count; i++)
                    {
                        JSONItem item = nextItems[i];
                        GoogleStreetViewDirection direction = new GoogleStreetViewDirection(item as JSONArray, longitude, latitude);
                        if (!direction.broken) items.Add(direction);
                    }

                    _directions = items.ToArray();
                }

                return _directions;
            }
        }

        public JSONItem json
        {
            get
            {
                if (_json == null)
                {
                    if (string.IsNullOrEmpty(_data)) return null;

                    int start, end;
                    for (start = 0; start < _data.Length; start++)
                    {
                        if (_data[start] == '[') break;
                    }

                    for (end = _data.Length - 1; end >= start; end--)
                    {
                        if (_data[end] == ']') break;
                    }

                    string str = _data.Substring(start, end - start + 1);

                    _json = JSON.Parse(str);
                }

                return _json;
            }
        }

        public string id
        {
            get
            {
                if (string.IsNullOrEmpty(_id) && json != null)
                {
                    if (_requestById) _id = json[1, 0, 1, 1].V<string>();
                    else _id = json[1, 1, 1].V<string>();
                }
                return _id;
            }
        }

        public double latitude
        {
            get
            {
                if (!_latitude.HasValue)
                {
                    if (nextItems != null)
                    {
                        JSONItem i = nextItems[0, 2, 0, 2];
                        if (i != null) _latitude = i.V<double>();
                        else _latitude = 0;
                    }
                    else _latitude = 0;
                }

                return _latitude.GetValueOrDefault();
            }
        }

        public double longitude
        {
            get
            {
                if (!_longitude.HasValue)
                {
                    if (nextItems != null)
                    {
                        JSONItem i = nextItems[0, 2, 0, 3];
                        if (i != null) _longitude = i.V<double>();
                        else _longitude = 0;
                    }
                    else _longitude = 0;
                }

                return _longitude.GetValueOrDefault();
            }
        }

        public GoogleStreetViewDirection[] nearestDirections
        {
            get
            {
                if (_nearestDirections == null)
                {
                    if (directions != null)
                    {
                        List<float> pans = new List<float>();
                        List<GoogleStreetViewDirection> items = new List<GoogleStreetViewDirection>();

                        foreach (GoogleStreetViewDirection item in directions)
                        {
                            if (item.distance > 0.02 || pans.Any(p => Math.Abs(p - item.pan) < 45)) continue;
                            pans.Add(item.pan);
                            items.Add(item);
                        }

                        _nearestDirections = items.ToArray();
                    }
                    else _nearestDirections = new GoogleStreetViewDirection[0];
                }

                return _nearestDirections;
            }
        }

        private JSONItem nextItems
        {
            get
            {
                if (_nextItems == null && json != null)
                {
                    if (_requestById) _nextItems = json[1, 0, 5, 0, 3, 0];
                    else _nextItems = json[1, 5, 0, 3, 0];
                }
                return _nextItems;
            }
        }

        public float northPan
        {
            get
            {
                if (!_northPan.HasValue)
                {
                    if (json != null)
                    {
                        if (_requestById)
                        {
                            JSONItem n = json[1, 0, 5, 0, 1, 2, 0];
                            if (n != null) _northPan = 360 - n.V<float>();
                            else _northPan = 0;
                        }
                        else
                        {
                            JSONItem n = json[1, 5, 0, 1, 2, 0];
                            if (n != null) _northPan = 360 - n.V<float>();
                            else _northPan = 0;
                        }
                    }
                    else _northPan = 0;
                }

                return _northPan.GetValueOrDefault();
            }
        }

        public string rawData
        {
            get { return _data; }
        }

        public bool requestById
        {
            get { return _requestById; }
        }

        public GoogleStreetViewMeta(string data, bool requestById)
        {
            _data = data;
            _requestById = requestById;
        }
    }
}