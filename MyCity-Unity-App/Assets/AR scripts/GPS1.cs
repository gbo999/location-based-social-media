using ARFoundationRemote;
using UnityEngine;
using ARFoundationRemote.Runtime;
using UnityEngine.Android;
using TMPro;

using Input = ARFoundationRemote.Input; // THIS LINE IS REQUIRED FOR LOCATION SERVICES TO WORK WITH AR FOUNDATION EDITOR REMOTE
using ARLocation;
using DigitalRubyShared;
#if !ENABLE_AR_FOUNDATION_REMOTE_LOCATION_SERVICES
    using LocationServiceStatus = ARFoundationRemote.LocationServiceStatusDummy;
#endif


namespace ARFoundationRemoteExamples
{
    public class GPS1 : MonoBehaviour
    {
        [SerializeField] float desiredAccuracyInMeters = 10f;
        [SerializeField] float updateDistanceInMeters = 10f;
        [SerializeField] int fontSize = 35;

        [SerializeField] TMP_Text loc;

        ARLocationProvider prov;


        [SerializeField] TMP_Text dis;
        
        
        public GameObject prefab;

        [Header("Location data (read-only)")]
        [SerializeField] bool isEnabledByUser;
        [SerializeField] LocationServiceStatus status;
        [SerializeField] LocationInfoSerializable lastData;


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




            prov = FindObjectOfType<ARLocationProvider>();









        }

        public void put()
        {
            GameObject g = new GameObject();
            PlaceAtLocation p;

            var loc = new Location()
            {
                Latitude = lastData.Deserialize().latitude,
                Longitude = lastData.Deserialize().longitude,
                Altitude = lastData.Deserialize().altitude,
                AltitudeMode = AltitudeMode.GroundRelative
            };

            var opts = new PlaceAtLocation.PlaceAtOptions()
            {
                HideObjectUntilItIsPlaced = true,
                MaxNumberOfLocationUpdates = 2,
                MovementSmoothing = 0.1f,
                UseMovingAverage = false
            };

           g =PlaceAtLocation.CreatePlacedInstance(gameObject, loc, opts);


              p= g.GetComponent<PlaceAtLocation>();

            double raw=Location.HorizontalDistance(loc, p.Location);


            dis.text = raw.ToString();
           

        }
        





        void OnGUI()
        {
            var style = new GUIStyle(GUI.skin.button) { fontSize = fontSize };
            if (GUI.Button(new Rect(0, 0, 400, 200), "Start", style))
            {
                Input.location.Start(desiredAccuracyInMeters, updateDistanceInMeters);
            }
            else if (GUI.Button(new Rect(0, 200, 400, 200), "Stop", style))
            {
                Input.location.Stop();
            }
        }


        void Update()
        {
            isEnabledByUser = Input.location.isEnabledByUser;
            status = Input.location.status;
            lastData = Input.location.status == LocationServiceStatus.Running ? LocationInfoSerializable.Create(Input.location.lastData) : default;
       





        }
    }
}
