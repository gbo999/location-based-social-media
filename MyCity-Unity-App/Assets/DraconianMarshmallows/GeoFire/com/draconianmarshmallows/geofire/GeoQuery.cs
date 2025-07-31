namespace com.draconianmarshmallows.geofire
{
    using com.draconianmarshmallows.geofire.core;
    using com.draconianmarshmallows.geofire.util;
    using Firebase.Database;
    using System.Collections.Generic;
    using System;
    using UnityEngine;

    /**
     * A GeoQuery object can be used for geo queries in a given circle. 
     */
    public class GeoQuery
    {
        public class LocationInfo
        {
            public GeoLocation location;
            public bool inGeoQuery;
            public GeoHash geoHash;

            public LocationInfo(GeoLocation location, bool inGeoQuery) {
                this.location = location;
                this.inGeoQuery = inGeoQuery;
                geoHash = new GeoHash(location);
            }
        }

        public Action geoQueryReadyListeners
        {
            get { return mGeoQueryReadyListeners; }
            set { mGeoQueryReadyListeners = value; }
        }

        public Action<DatabaseError> geoQueryErrorListeners
        {
            get { return mGeoQueryErrorListeners; }
            set { mGeoQueryErrorListeners = value; }
        }

        public Action<string, GeoLocation> keyEnteredListeners
        {
            get { return mKeyEnteredListeners; }
            set { mKeyEnteredListeners = value; }
        }

        public Action<string> keyExitedListeners
        {
            get { return mKeyExitedListeners; }
            set { mKeyExitedListeners = value; }
        }

        public Action<string, GeoLocation> keyMovedListeners
        {
            get { return mKeyMovedListeners; }
            set { mKeyMovedListeners = value; }
        }

        private void onChildAdded(object sender, ChildChangedEventArgs e)
        {
            childAdded(e.Snapshot);
        }

        private void onChildChanged(object sender, ChildChangedEventArgs e)
        {
            childChanged(e.Snapshot);
        }

        private void onChildRemoved(object sender, ChildChangedEventArgs e)
        {
            childRemoved(e.Snapshot);
        }

        public void onChildMoved(DataSnapshot dataSnapshot, string s) {
            // ignore, this should be handled by onChildChanged
        }
        
        public void onCancelled(DatabaseError databaseError) {
            // ignore, our API does not support onCancelled
        }

        private Action mGeoQueryReadyListeners;
        private Action<DatabaseError> mGeoQueryErrorListeners;
        private Action<string, GeoLocation> mKeyEnteredListeners;
        private Action<string> mKeyExitedListeners;
        private Action<string, GeoLocation> mKeyMovedListeners;

        private GeoFire geoFire;
        private Dictionary<GeoHashQuery, Query> firebaseQueries = new Dictionary<GeoHashQuery, Query>();
        private HashSet<GeoHashQuery> outstandingQueries = new HashSet<GeoHashQuery>();
        private Dictionary<string, LocationInfo> locationInfos = new Dictionary<string, LocationInfo>();
        private GeoLocation center;
        private double radius;
        private HashSet<GeoHashQuery> queries;
        private bool readyFired;

        /**
         * Creates a new GeoQuery object centered at the given location and with the given radius.
         * @param geoFire The GeoFire object this GeoQuery uses.
         * @param center The center of this query.
         * @param radius The radius of this query, in kilometers.
         */
        public GeoQuery(GeoFire geoFire, GeoLocation center, double radius) {
            this.geoFire = geoFire;
            this.center = center;
            this.radius = radius * 1000; // Convert from kilometers to meters. 
        }

        private bool locationIsInQuery(GeoLocation location) {
            return GeoUtils.distance(location, center) <= radius;
        }

        private void updateLocationInfo(string key, GeoLocation location)
        {
            LocationInfo oldInfo = null;
            locationInfos.TryGetValue(key, out oldInfo);

            bool isNew = (oldInfo == null);
            bool changedLocation = (oldInfo != null && ! oldInfo.location.equals(location));
            bool wasInQuery = (oldInfo != null && oldInfo.inGeoQuery);
            bool isInQuery = locationIsInQuery(location);

            if ((isNew || ! wasInQuery) && isInQuery && keyEnteredListeners != null)
            {
                keyEnteredListeners(key, location);
            }
            else if (!isNew && changedLocation && isInQuery && keyMovedListeners != null)
            {
                keyMovedListeners(key, location);
            }
            else if (wasInQuery && ! isInQuery && keyExitedListeners != null)
            {
                keyExitedListeners(key);
            }

            LocationInfo newInfo = new LocationInfo(location, locationIsInQuery(location));
            locationInfos[key] = newInfo;
        }

        private bool geoHashQueriesContainGeoHash(GeoHash geoHash) {
            if (queries == null) {
                return false;
            }
            foreach (GeoHashQuery query in queries)
            {
                if (query.containsGeoHash(geoHash)) {
                    return true;
                }
            }
            return false;
        }

        public void reset()
        {
            foreach (KeyValuePair<GeoHashQuery, Query> entry in firebaseQueries)
            {
                removeListenerCallbacks(entry.Value);
            }
            outstandingQueries.Clear();
            firebaseQueries.Clear();
            queries = null;
            locationInfos.Clear();
        }

        private void addListenerCallbacks(Query query)
        {
            query.ChildAdded += onChildAdded;
            query.ChildChanged += onChildChanged;
            query.ChildRemoved += onChildRemoved;
        }

        private void removeListenerCallbacks(Query query)
        {
            query.ChildAdded -= onChildAdded;
            query.ChildChanged -= onChildChanged;
            query.ChildRemoved -= onChildRemoved;
        }

        private bool hasListeners() {
            return geoQueryReadyListeners != null
                || geoQueryErrorListeners != null
                || keyEnteredListeners != null
                || keyExitedListeners != null
                || keyMovedListeners != null;
        }

        private bool canFireReady()
        {
            return ! readyFired && outstandingQueries.Count == 0;
        }

        private void checkAndFireReady() {
            if (canFireReady() && geoQueryReadyListeners != null)
            {
                readyFired = true;
                geoQueryReadyListeners();
            }
        }

        private void addValueToReadyListener(Query firebase, GeoHashQuery query)
        {
            firebase.ValueChanged += new EventHandler<ValueChangedEventArgs>(
                delegate (object sender, ValueChangedEventArgs e)
                {
                    outstandingQueries.Remove(query);
                    checkAndFireReady();
                });
        }

        private void setupQueries()
        {
            HashSet<GeoHashQuery> oldQueries = (queries == null) ? new HashSet<GeoHashQuery>() : queries;
            HashSet<GeoHashQuery> newQueries = GeoHashQuery.queriesAtLocation(center, radius);
            queries = newQueries;

            

            foreach (GeoHashQuery query in oldQueries)
            {
                if ( ! newQueries.Contains(query))
                {
                    removeListenerCallbacks(firebaseQueries[query]);
                    firebaseQueries.Remove(query);
                    outstandingQueries.Remove(query);
                }
            }

            foreach (GeoHashQuery query in newQueries)
            {
                if ( ! oldQueries.Contains(query))
                {
                    outstandingQueries.Add(query);
                    DatabaseReference databaseReference = geoFire.getDatabaseReference();

                    var startValue = query.getStartValue();
                    var endValue = query.getEndValue();

                    Query firebaseQuery = databaseReference.OrderByChild(Constants.KEY_HASH)
                        .StartAt(startValue).EndAt(endValue);
                    
                    addListenerCallbacks(firebaseQuery);
                    addValueToReadyListener(firebaseQuery, query);
                    firebaseQueries.Add(query, firebaseQuery);
                }
            }

            var keys = new List<string>(locationInfos.Keys);
            foreach (string key in keys)
            {
                LocationInfo oldLocationInfo = locationInfos[key];
                updateLocationInfo(key, oldLocationInfo.location);
            }

         
            
            // Remove locations that are not part of the geo query anymore. 
            foreach (string key in keys)
            {
                var value = locationInfos[key];

                if ( ! geoHashQueriesContainGeoHash(value.geoHash))
                    locationInfos.Remove(key);
            }
            checkAndFireReady();
        }

        private void childAdded(DataSnapshot dataSnapshot)
        {
            GeoLocation location = GeoFire.getLocationValue(dataSnapshot);

            if (location != null) {
                updateLocationInfo(dataSnapshot.Key, location);
            } else {
                // throw an error in future?
            }
        }

        private void childChanged(DataSnapshot dataSnapshot) {
            GeoLocation location = GeoFire.getLocationValue(dataSnapshot);
            if (location != null) {
                updateLocationInfo(dataSnapshot.Key, location);
            } else {
                // throw an error in future?
            }
        }

        private void childRemoved(DataSnapshot dataSnapshot) {
            String key = dataSnapshot.Key;
            LocationInfo info = locationInfos[key];
            if (info != null) {
                geoFire.getDatabaseRefForKey(key).ValueChanged += new EventHandler<ValueChangedEventArgs>(
                    delegate (object sender, ValueChangedEventArgs e)
                    {
                        GeoLocation location = GeoFire.getLocationValue(dataSnapshot);
                        GeoHash hash = (location != null) ? new GeoHash(location) : null;

                        if (hash == null || geoHashQueriesContainGeoHash(hash))
                        {
                            var locationInfo = locationInfos[key];
                            locationInfos.Remove(key);

                            if (locationInfo != null && locationInfo.inGeoQuery
                                && keyExitedListeners != null)
                            {
                                keyExitedListeners(key);
                            }
                        }
                    });
            }
        }

        public void initializeListeners()
        {
            if (queries == null)
            {
                setupQueries();
            }
            else
            {
                
                
                foreach (KeyValuePair<string, LocationInfo> entry in locationInfos)
                {
                    string key = entry.Key;
                    LocationInfo info = entry.Value;

                    if (info.inGeoQuery && keyEnteredListeners != null)
                    {
                        keyEnteredListeners(key, info.location);
                    }
                }



                if (canFireReady() && geoQueryReadyListeners != null)
                {
                    geoQueryReadyListeners();
                }
            }
        }

        /**
         * Returns the current center of this query.
         * @return The current center.
         */
        public GeoLocation getCenter() {
            return center;
        }

        /**
         * Sets the new center of this query and triggers new events if necessary.
         * @param center The new center.
         */
        public void setCenter(GeoLocation center) {
            this.center = center;
            if (hasListeners()) {
                setupQueries();
            }
        }

        /**
         * Returns the radius of the query, in kilometers.
         * @return The radius of this query, in kilometers.
         */
        public double getRadius() {
            return radius / 1000; // convert from meters. 
        }

        /**
         * Sets the radius of this query, in kilometers, and triggers new events if necessary.
         * @param radius The new radius value of this query in kilometers.
         */
        public void setRadius(double radius)
        {
            this.radius = radius * 1000; // Convert to meters. 
            if (hasListeners()) {
                setupQueries();
            }
        }

        /**
         * Sets the center and radius (in kilometers) of this query, and triggers new events if necessary.
         * @param center The new center.
         * @param radius The new radius value of this query in kilometers.
         */
        public void setLocation(GeoLocation center, double radius) {
            this.center = center;
            
            this.radius = radius * 1000; // convert radius to meters. 
            if (hasListeners()) {
                setupQueries();
            }
        }
    }
}
