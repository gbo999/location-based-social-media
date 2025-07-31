using System.Collections.Generic;
using Firebase.Database;
using UnityEngine;
using com.draconianmarshmallows.geofire.core;
using com.draconianmarshmallows.geofire.util;
using com.draconianmarshmallows.geofire.parsers;
using System;

namespace com.draconianmarshmallows.geofire
{
    /**
     * A GeoFire instance is used to store geo location data in Firebase.
     */
    public class GeoFire
    {
        public static BaseGeoLocationParser locationParser;

        /**
         * A listener that can be used to be notified about a successful write or an error on writing.
         */
        public interface CompletionListener
        {
            /**
             * Called once a location was successfully saved on the server or an error occurred. On success, the parameter
             * error will be null; in case of an error, the error will be passed to this method.
             *
             * @param key   The key whose location was saved.
             * @param error The error or null if no error occurred.
             */
            void onComplete(string key, Exception error);
        }

        /**
         * A small wrapper class to forward any events to the LocationEventListener.
         */
        public class LocationValueEventListener {

            private ILocationCallback callback;

            public LocationValueEventListener(ILocationCallback callback) {
                this.callback = callback;
            }
            
            public void onDataChange(object sender, ValueChangedEventArgs e)
            {
                var dataSnapshot = e.Snapshot;

                if (dataSnapshot.GetValue(true) == null)
                {
                    callback.onLocationResult(dataSnapshot.Key.ToString(), null);
                } else {
                    GeoLocation location = getLocationValue(dataSnapshot);
                    if (location != null) {
                        callback.onLocationResult(dataSnapshot.Key, location);
                    } else {
                        string message = "GeoFire data has invalid format: " 
                            + dataSnapshot.GetValue(true);
                        callback.onCancelled(new UnityException(message));
                    }
                }
            }
            
            void onCancelled(DatabaseError databaseError) {
                callback.onCancelled(new UnityException(databaseError.Message));
            }
        }

        public static GeoLocation getLocationValue(DataSnapshot dataSnapshot)
        {
            var location = dataSnapshot.Child(Constants.KEY_LOCATION);
            var latValue = location.Child(Constants.KEY_LAT).Value;
            var lonValue = location.Child(Constants.KEY_LON).Value;
            double latitude  = double.Parse(latValue.ToString());
            double longitude = double.Parse(lonValue.ToString());

            if (locationParser != null
                && GeoLocation.coordinatesValid(latitude, longitude))
            {
                return locationParser.parse(dataSnapshot);
            } else {
                return null;
            }
        }

        private DatabaseReference databaseReference;

        /**
         * Creates a new GeoFire instance at the given Firebase reference. 
         * Also adds the a default location location parser.
         *
         * @param databaseReference The Firebase reference this GeoFire instance uses. 
         */
        public GeoFire(DatabaseReference databaseReference)
        {
            initialize(databaseReference, new BaseGeoLocationParser());
        }

        /**
         * Creates a new GeoFire instance at the given Firebase reference. 
         * Allows custom location parser. 
         * 
         * @param databaseReference The Firebase reference this GeoFire instance uses. 
         * @param locationParser A custom location-parser. 
         */
        public GeoFire(DatabaseReference databaseReference, 
            BaseGeoLocationParser locationParser)
        {
            initialize(databaseReference, locationParser);
        }

        private void initialize(DatabaseReference databaseReference, 
            BaseGeoLocationParser locationParser)
        {
            this.databaseReference = databaseReference;
            GeoFire.locationParser = locationParser;
        }

        /**
         * @return The Firebase reference this GeoFire instance uses.
         */
        public DatabaseReference getDatabaseReference() {
            return databaseReference;
        }

        public DatabaseReference getDatabaseRefForKey(string key) {
            return databaseReference.Child(key);
        }

        /**
         * Sets the location for a given key.
         *
         * @param key      The key to save the location for.
         * @param location The location of this key.
         */
        public void setLocation(string key, GeoLocation location, BaseLocationContent content) {
            setLocation(key, location, null, content);
        }

        /**
         * Sets the location for a given key.
         *
         * @param key                The key to save the location for.
         * @param location           The location of this key.
         * @param completionListener A listener that is called once the location was successfully 
         *                           saved on the server or an error occurred.
         */
        public void setLocation(string key, GeoLocation location, 
            CompletionListener completionListener, BaseLocationContent content)
        {
            if (key == null) {
                throw new UnityException("Null pointer.");
            }
            DatabaseReference keyRef = getDatabaseRefForKey(key);
            GeoHash geoHash = new GeoHash(location);

            var updates = new Dictionary<String, object>();
            updates.Add(Constants.KEY_HASH, geoHash.getGeoHashString());
            updates.Add(Constants.KEY_LOCATION, location);
            var dbLocation = new DbLocation(geoHash.getGeoHashString(), location, content);
            var geohashString = geoHash.getGeoHashString();
            keyRef.UpdateChildrenAsync(dbLocation.toMap());
        }

        /**
         * Removes the location for a key from this GeoFire.
         *
         * @param key The key to remove from this GeoFire.
         */
        public void removeLocation(string key) {
            removeLocation(key, null);
        }

        /**
         * Removes the location for a key from this GeoFire.
         *
         * @param key                The key to remove from this GeoFire.
         * @param completionListener A completion listener that is called once the location is successfully 
         *                           removed from the server or an error occurred. 
         */
        public void removeLocation(string key, CompletionListener completionListener) {
            if (key == null) {
                throw new UnityException("No Key");
            }
            DatabaseReference keyRef = getDatabaseRefForKey(key);

            if (completionListener != null)
            {
                var result = keyRef.RemoveValueAsync();
                completionListener.onComplete(key, result.Exception);
            } else {
                keyRef.RemoveValueAsync();
            }
        }

        /**
         * Gets the current location for a key and calls the callback with the current value.
         *
         * @param key      The key whose location to get.
         * @param callback The callback that is called once the location is retrieved.
         */
        public void getLocation(String key, ILocationCallback callback) {
            DatabaseReference keyRef = this.getDatabaseRefForKey(key);
            LocationValueEventListener valueListener = new LocationValueEventListener(callback);
            keyRef.ValueChanged += valueListener.onDataChange;
        }

        /**
         * Returns a new Query object centered at the given location and with the given radius.
         *
         * @param center The center of the query.
         * @param radius The radius of the query, in kilometers.
         * @return The new GeoQuery object.
         */
        public GeoQuery queryAtLocation(GeoLocation center, double radius) {
            return new GeoQuery(this, center, radius);
        }
    }
}
