using System;
using System.Xml;
using UnityEngine;
using Yamanas.Infrastructure.Popups;
using Yamanas.Scripts.Infrastructure.Server;
using Yamanas.Scripts.MapLoader;

namespace Yamanas.Scripts.Map
{
    public class MapController : MonoBehaviour
    {
        #region Fields

        private OnlineMapsMarker3D onlineMapsMarkertoput;

        [SerializeField] private Vector2 _lastPosition;

        [SerializeField] private GameObject prefab;

        [SerializeField] private Texture2D textureTofirst;

        private bool markerDrag;

        private static MapController _instance;

        private Action<string> _addressCallback;

       
        #endregion


        #region Methods

        private void Start()
        {
            _lastPosition = OnlineMaps.instance.position;
            OnlineMaps.instance.OnChangePosition += PositionQuery;
            OnlineMapsControlBase.instance.OnMapClick += OnMapClick;
            GeoPostLoader.Instance.makeUsualQuery();
        }

        private void OnMapClick()
        {
            if (Input.GetKey(KeyCode.LeftControl) || markersMode.markerMode)
            {
                // Get the coordinates under the cursor.
                double lng, lat;
                OnlineMapsControlBase.instance.GetCoords(out lng, out lat);

                PinFactory.Instance.CreatePinPrePost(lng, lat);


                // longToSave = lng;
                // latToSave = lat;
                // PostProcessController.Instance.Longtitude = lng;
                // PostProcessController.Instance.Latitude = lat;
                // prefab.GetComponent<SpriteRenderer>().sprite = Sprite.Create(textureTofirst,
                //     new Rect(0.0f, 0.0f, textureTofirst.width, textureTofirst.height), new Vector2(0.5f, 0.5f), 100.0f);
                // onlineMapsMarkertoput = OnlineMapsMarker3DManager.CreateItem(lng, lat, prefab);
                // onlineMapsMarkertoput.sizeType = OnlineMapsMarker3D.SizeType.scene;
                // onlineMapsMarkertoput.scale = 17;
                // onlineMapsMarkertoput.rotation = Quaternion.Euler(85.803f, 0, -180.957f);
                //
                // onlineMapsMarkertoput.isDraggable = true;
                //
                // onlineMapsMarkertoput.OnRelease += StopDragging;
                //
                //
                // markersMode.markerMode = false;
                //
                // PostProcessController.Instance.PopupSystem.ClosePopup(PopupType.PressHelper);
                //
                // PostProcessController.Instance.PopupSystem.ShowPopup(PopupType.DrageHelper, "");


               // GetAddress();
                /*helperText.ChangeVisibility(false);
                DragHelper.ChangeVisibility(true);*/
            }
        }

        private void PositionQuery()
        {
            if (OnlineMapsUtils.DistanceBetweenPointsD(
                new Vector2(OnlineMaps.instance.position.x, OnlineMaps.instance.position.y),
                new Vector2(_lastPosition.x, _lastPosition.y)) > 20)
            {
                GeoPostLoader.Instance.makeUsualQuery();
                _lastPosition = OnlineMaps.instance.position;
            }
        }

        /*
        private void StopDragging(OnlineMapsMarkerBase obj)
        {
            markersMode.markerMode = false;

            if (markerDrag == false)
            {
                obj.isDraggable = false;
            }

            double lang, lat;
            obj.GetPosition(out lang, out lat);
            // longToSave = lang;
            // latToSave = lang;
            //
            PostProcessController.Instance.Longtitude = lang;
            PostProcessController.Instance.Latitude = lat;


            GetAddress();
        }
        */
        
        public void GetAddress(double longitude, double latitude, Action<string> addresCallback)
        {
            //publishPanel.SetActive(false);

            _addressCallback = addresCallback;
            
            OnlineMapsGoogleGeocoding.ReverseGeocodingParams requestParams =
                new OnlineMapsGoogleGeocoding.ReverseGeocodingParams(longitude,
                    latitude);

            requestParams.key = MyCity.Config.APIKeys.GoogleMapsAPIKey;
            requestParams.language = "en";


            OnlineMapsGoogleGeocoding request = OnlineMapsGoogleGeocoding.Find(requestParams);


            request.Send();
            request.OnComplete += OnRequestCompleteWIthCallback;
        }

        private void OnRequestCompleteWIthCallback(string s)
        {
            XmlDocument xDoc = new XmlDocument();

            xDoc.LoadXml(s);

            Debug.Log("success");

            XmlNodeList name = xDoc.GetElementsByTagName("formatted_address");

            Debug.Log(name[1].InnerText);


            string addressToSave = "at " + name[1].InnerText;

            PostProcessController.Instance.Address = addressToSave;
            
            _addressCallback?.Invoke(addressToSave);
        }
        

        public void GetAddress()
        {
            //publishPanel.SetActive(false);

            OnlineMapsGoogleGeocoding.ReverseGeocodingParams requestParams =
                new OnlineMapsGoogleGeocoding.ReverseGeocodingParams(PostProcessController.Instance.Longtitude,
                    PostProcessController.Instance.Latitude);

            requestParams.key = MyCity.Config.APIKeys.GoogleMapsAPIKey;
            requestParams.language = "en";


            OnlineMapsGoogleGeocoding request = OnlineMapsGoogleGeocoding.Find(requestParams);


            request.Send();
            request.OnComplete += OnRequestComplete;
        }

        private void OnRequestComplete(string s)
        {
            XmlDocument xDoc = new XmlDocument();

            xDoc.LoadXml(s);

            Debug.Log("success");

            XmlNodeList name = xDoc.GetElementsByTagName("formatted_address");

            Debug.Log(name[1].InnerText);


            string addressToSave = "at " + name[1].InnerText;

            PostProcessController.Instance.Address = addressToSave;
        }

        #endregion

        #region Properties

       

        public static MapController Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<MapController>();
                }

                return _instance;
            }
        }

        #endregion
    }
}