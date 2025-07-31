/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using DigitalRubyShared;
using InfinityCode.OnlineMapsExamples;
using InfinityCode.uPano.HotSpots;
using InfinityCode.uPano.InteractiveElements;
using InfinityCode.uPano.Renderers.Base;
using System;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;
using Yamanas.Infrastructure.Popups;
using Yamanas.Scripts.Map;
using Yamanas.Scripts.MapLoader;
using static OnlineMapsGoogleGeocoding;

namespace InfinityCode.uPano.Examples
{
    /// <summary>
    /// Example of how to get pan and tilt under the cursor, convert it to Unity World Position, and position GameObjects
    /// </summary>
    [AddComponentMenu("uPano/Examples/InstantiateGameObjectsUnderCursorExample")]
    public class InstantiateGameObjectsUnderCursorExample : MonoBehaviour
    {
        #region Events

        public event Action OnButtonUp;

        #endregion
        
        
        #region Fields

        #region Touch

        const float pinchTurnRatio = Mathf.PI / 2;

        const float minTurnAngle = 0;

        const float pinchRatio = 1;

        private float turnAngleDelta;

        private float turnAngle;

        private RotateGestureRecognizer rotateGesture;

        private Vector3 mousePosition;

        #endregion

        #region Geo

        float panToSave = 0;

        float tiltToSave = 0;

        string panoramaID = "";

        private PanoRenderer panoRenderer;

        OnlineMapsPanoConnector panoConnector;

        #endregion

        private bool _creationAvaialbl ;

        // private PutSpecialMarkers put;

        [SerializeField] private StreetViewButtons _streetViewButtons;

        [SerializeField] private GameObject prefab;

     //   public Text text;

        private HotSpotManager hotSpotManager;

        bool isButtonUp = false;

        private HotSpot hotSpot1;


        private float initialDistance;

        private Vector3 initialScale;

        #endregion

        private void Start()
        {
            Pano.OnPanoStarted += alow;

            // Store an instance of PanoRenderer
            hotSpotManager = GetComponent<HotSpotManager>();
            //put = FindObjectOfType<PutSpecialMarkers>();
            panoConnector = FindObjectOfType<OnlineMapsPanoConnector>();
            panoConnector.OnLoaded += GetAddress;
            
            rotateGesture = new RotateGestureRecognizer();
            rotateGesture.StateUpdated += RotateGestureCallback;
            rotateGesture.AllowSimultaneousExecution(null);

            FingersScript.Instance.AddGesture(rotateGesture);
            //   FingersScript.Instance.PassThroughObjects.Add(put.insbutton);
            FingersScript.Instance.PassThroughObjects.Add(GetComponent<PanoRenderer>().gameObject);
            FingersScript.Instance.PassThroughObjects.Add(GetComponent<Pano>().gameObject);
        }

        private void GetAddress(Pano obj)
        {
            PostProcessController.Instance.Longtitude = panoConnector.meta.longitude;
            PostProcessController.Instance.Latitude = panoConnector.meta.latitude;
            MapController.Instance.GetAddress();
        }

        private void alow(Pano obj)
        {
            panoRenderer = FindObjectOfType<PanoRenderer>();
            hotSpotManager = FindObjectOfType<HotSpotManager>();
        }

        public void AllowCreation()
        {
            _creationAvaialbl = true;
        }

        private void RotateGestureCallback(GestureRecognizer gesture)
        {
            if (gesture.State == GestureRecognizerState.Executing)
            {
                hotSpot1.rotation *= Quaternion.Euler(rotateGesture.RotationRadiansDelta * Mathf.Rad2Deg, 0, 0);


                Debug.Log(hotSpot1.rotation.ToString());
            }
        }

        #region Methods

        public void CreateStreetView()
        {
            _creationAvaialbl = false;
            if (prefab != null)
            {
                float pan, tilt;
                panoRenderer.GetPanTiltUnderCursor(out pan, out tilt);

                HotSpot hotSpot = hotSpotManager.Create(pan, tilt, prefab);

                hotSpot.scale = new Vector3(5, 5, 5);

                hotSpot.distanceMultiplier -= 0.09f;
                hotSpot.OnClick.AddListener(SomeMethod);
                hotSpot.OnPointerDown.AddListener(buttonDown);
                hotSpot.OnPointerUp.AddListener(buttonUp);
            }
        }

        private void Update()
        {
            if (hotSpot1 != null && Input.mousePosition != mousePosition && !isButtonUp)
            {
                float pana, tilta;
                panoRenderer.GetPanTiltUnderCursor(out pana, out tilta);
                // Debug.Log("under cursor pan and tilt:" + pana + " tilt:" + tilta);
                hotSpot1.SetPanTilt(pana, tilta);
                mousePosition = Input.mousePosition;

                panToSave = pana;
                tiltToSave = tilta;
                panoramaID = panoConnector.meta.id;
            }

            /*
            float pan, tilt;
            panoRenderer.GetPanTiltUnderCursor(out pan, out tilt);
            */


            if (Input.GetMouseButtonDown(0) &&_creationAvaialbl )

            {
                CreateStreetView();
            }
        }

        public void Save()
        {
            
            PostProcessController.Instance.PanoramaID = panoramaID;
            PostProcessController.Instance.PanToSave = panToSave.ToString();
            PostProcessController.Instance.TiltToSave = tiltToSave.ToString();
            //MapController.Instance.GetAddress();
            PostProcessController.Instance.PopupSystem.ShowPopup(PopupType.ApproveLocation,PostProcessController.Instance.Address);

        }


        private void buttonUp(InteractiveElement arg0)
        {
            Debug.Log("Release");

            isButtonUp = true;
            hotSpot1 = null;
            OnButtonUp?.Invoke();
            

        }

        private void buttonDown(InteractiveElement arg0)
        {
            Debug.Log("Long press");
            hotSpot1 = arg0 as HotSpot;
            mousePosition = Input.mousePosition;
        }

        private void SomeMethod(InteractiveElement arg0)
        {
            isButtonUp = false;
            //hotSpot1 = arg0 as HotSpot;
            Debug.Log("pressed");
        }

        #endregion

        #region Properties



        #endregion
    }
}