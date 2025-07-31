
using ARFoundationRemote;
using ARFoundationRemote.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;
using Yamanas.Scripts.MapLoader;
using Input = ARFoundationRemote.Input;
#if !ENABLE_AR_FOUNDATION_REMOTE_LOCATION_SERVICES
       using LocationServiceStatus = ARFoundationRemote.LocationServiceStatusDummy;
#endif

public class GPS : MonoBehaviour
{
   //public GameObject longText;
    //  public GameObject latText;
    public TextMeshProUGUI LocationText;
    private TextMeshProUGUI perm;
    //public GameObject ARGReen;

    //  Text location;


    IEnumerator coroutine;

    public static float longi;
    public static float lati;
    void Awake()
    {
        if (Defines.isAndroid)
        {
            LocationServicesSender.RequestFineLocationPermission();

        }

        if (!Defines.enableLocationServices)
        {


            Debug.LogError(LocationServiceRemote.missingDefineError);
        }
    }
    //
    IEnumerator Start()
    {
        coroutine = UpdateGPS();

        // location = LocationText.GetComponent<Text>();

        // Text longitudeText = longText.GetComponent<Text>();
        // Text latitudeText = latText.GetComponent<Text>();

        try
        {
            Input.location.Start();

        }

     catch (Exception e)
        {
            perm.text = e.ToString();

        }





        if (!Input.location.isEnabledByUser)
        {

            LocationText.text = Input.location.status.ToString();
            yield break;

        }




        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (maxWait < 1)
        {
            print("Timed out");
            LocationText.text = "Timed out";
            yield break;
        }


        if (Input.location.status == LocationServiceStatus.Failed)
        {
            LocationText.text = "Unable to determine device location";
            print("Unable to determine device location");
            yield break;
        }
        else
        {
            print("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
            // longitudeText.text = "Longitude: " + Input.location.lastData.longitude;
            // latitudeText.text = "Latitude: " + Input.location.lastData.latitude;
            LocationText.text= "Longitude: " + Input.location.lastData.longitude + " Latitude: " + Input.location.lastData.latitude; ;
            StartCoroutine(coroutine);
        }
    }

    IEnumerator UpdateGPS()
    {
        float UPDATE_TIME = 3f; //Every  3 seconds
        WaitForSeconds updateTime = new WaitForSeconds(UPDATE_TIME);

        while (true)
        {

            //   LocationText.text = Input.location.status == LocationServiceStatus.Running ? LocationInfoSerializable.Create(Input.location.lastData).ToString() : default;

         //   print("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);

            LocationText.text = "Longitude: " + Input.location.lastData.longitude + " Latitude: " + Input.location.lastData.latitude;





            longi = Input.location.lastData.longitude;
            lati = Input.location.lastData.latitude;


            PostProcessController.Instance.Latitude = lati;
            PostProcessController.Instance.Longtitude = longi;

            //longitudeText.text = "Longitude: " + Input.location.lastData.longitude;
            //latitudeText.text = "Latitude: " + Input.location.lastData.latitude;
            yield return updateTime;
        }
    }

    void StopGPS()
    {
        Input.location.Stop();
        StopCoroutine(coroutine);
    }

    void OnDisable()
    {
        StopGPS();
    }
}

