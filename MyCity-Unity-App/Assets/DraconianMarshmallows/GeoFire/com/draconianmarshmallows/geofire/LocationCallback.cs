using UnityEngine;

namespace com.draconianmarshmallows.geofire
{
    /**
     * Classes implementing this interface can be used to receive the locations stored in GeoFire.
     */
    public interface ILocationCallback
    {
        /**
         * This method is called with the current location of the key. location will be null if there is no location
         * stored in GeoFire for the key.
         * @param key The key whose location we are getting.
         * @param location The location of the key.
         */
        void onLocationResult(string key, GeoLocation location);

        /**
         * Called if the callback could not be added due to failure on the server or security rules.
         * @param databaseError The error that occurred.
         */
        void onCancelled(UnityException databaseError);
    }
}
