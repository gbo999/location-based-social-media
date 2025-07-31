using com.draconianmarshmallows.geofire;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System;
using System.Collections.Generic;
using UnityEngine;

public class LocationController : MonoBehaviour
{
    [SerializeField] private string firebaseDbUrl;

    private GeoQuery query;
    List<string> s = new List<string>();

    private void Start()
    {

        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(firebaseDbUrl);

        
        var mainDbReference = FirebaseDatabase.DefaultInstance.RootReference;


        var geofireDbReference = mainDbReference.Child("Tags").Child("TagSport").Child(DateTime.Now.ToString("dd-MM-yyyy"));
        var geoFire = new GeoFire(geofireDbReference);

        query = geoFire.queryAtLocation(new GeoLocation(31.8116051562021, 34.7864459913435), 10);

        query.geoQueryReadyListeners += onGeoQueryReady;
        query.geoQueryErrorListeners += onGeoQueryError;
        query.keyEnteredListeners += onKeyEntered;
        query.keyExitedListeners += onKeyExited;
        query.keyMovedListeners += onKeyMoved;
        query.initializeListeners();

        /* // Used for testing in Unity editor::
        

         // Setup DB references and a GeoFire instance::
         var geofireDbReference = mainDbReference.Child("AllPosts").Child("geofire");
         var geoFire = new GeoFire(geofireDbReference);

         // Store test locations::
         Debug.Log("Storing locations...");



     //    geoFire.setLocation("enemy_1", new GeoLocation(37.589680, -122.477000), null);
       // geoFire.setLocation("enemy_2", new GeoLocation(37.589694, -122.477111), null);

         // Setup query to find objects in a radius around a location::
         Debug.Log("Querying objects in an error...");



       */
    }


    //lat =31.8116051562021
    //lng=34.7864459913435

    private void Update()
    {
        if (Input.GetKey(KeyCode.M)) // For testing objects exited radius. 
        {
            var center = query.getCenter();
            center.longitude += .00005D;
            query.setCenter(center);
            //Debug.Log("Moving query location to : " + center.toString());
        }
    }

    private void onGeoQueryReady()
    {
        Debug.Log("Geo-query ready.");
    }

    private void onGeoQueryError(DatabaseError error)
    {
        Debug.LogError("Geo query error: " + error);
    }

    private void onKeyEntered(string key, GeoLocation location)
    {


        Debug.Log("enetered " + key);

     //   s.Add(key);
       // Debug.Log(s.Count);
        
      //  Debug.LogFormat("Geo query ENTER: {0} :: {1}", key, location.toString());
    }

    private void onKeyExited(string key)
    {
        Debug.Log("Geo query EXITED : " + key);
    }

    private void onKeyMoved(string key, GeoLocation location)
    {
        Debug.LogFormat("Geo query moved: {0} :: {1}", key, location);
    }
}
