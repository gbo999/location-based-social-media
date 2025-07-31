using System;
using ARLocation;
using ARLocation.MapboxRoutes;
using SocialApp;
using TMPro;
using UnityEngine;

namespace Yamanas.Scripts.AR
{
    public class RouteManager : MonoBehaviour
    {
        #region Fields

        private double _latitude;

        private double _logntitude;

        [SerializeField] private TMP_Text _textTest;
        
        #endregion

        private static RouteManager _instance;

        
        
        private void Start()
        {
            AppManager.NAVIGATION.OnNavigateAR += OnShowAR;
        }

        private void OnShowAR()
        {
            ARLocationProvider.Instance.OnEnabled.AddListener(onLocationEnabled);
        }


        private void onLocationEnabled(Location location)
        {
            Debug.Log($"lat is {_latitude}");
            
            Location loc = new Location(_latitude, _logntitude, 0);

            RouteWaypoint start = new RouteWaypoint {Type = RouteWaypointType.UserLocation};

            RouteWaypoint end = new RouteWaypoint {Type = RouteWaypointType.Location, Location = loc};

            StartCoroutine(FindObjectOfType<MapboxRoute>().LoadRoute(start, end));
        }

        #region Properties

        public static RouteManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<RouteManager>();
                }

                return _instance;
            }
        }

        public double Latitude
        {
            get => _latitude;
            set => _latitude = value;
        }

        public double Logntitude
        {
            get => _logntitude;
            set => _logntitude = value;
        }

        #endregion
    }
}