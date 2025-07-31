using System;
using SocialApp;
using UnityEngine;
using Yamanas.Infrastructure.Popups;
using Yamanas.Scripts.Map;
using Yamanas.Scripts.Map.InfoWindow;
using Random = UnityEngine.Random;

namespace Yamanas.Scripts.MapLoader
{
    public class PinFactory : MonoBehaviour
    {

        #region Consts

        private const string MARKER_DATA = "FeedData";

        private static PinFactory _instance;

        #endregion


        #region Fields

        [SerializeField] private GameObject SharePrefab;

        [SerializeField] private GameObject SalePrefab;

        [SerializeField] private GameObject EventPrefab;

        [SerializeField] private GameObject PollPrefab;

        [SerializeField] private GameObject _pinToCreate;

        private OnlineMapsMarker3D onlineMapsMarkertoput;
        
        [SerializeField] private GameObject prefab;

        [SerializeField] private Texture2D textureTofirst;

        [SerializeField] private int _extraHeight = 3;

        [SerializeField] private AudioSource _audioSource;

        [SerializeField] private AudioClip[] _audioClips;

        #endregion

        #region Methods

        public void CreatePinPrePost(double lng, double lat)
        {
            PostProcessController.Instance.Longtitude = lng;
            PostProcessController.Instance.Latitude = lat;
            
            Debug.Log($"lng is {PostProcessController.Instance.Longtitude} and lat is {PostProcessController.Instance.Latitude}");
            
            
            prefab.GetComponentInChildren<SpriteRenderer>().sprite = Sprite.Create(textureTofirst,
                new Rect(0.0f, 0.0f, textureTofirst.width, textureTofirst.height), new Vector2(0.5f, 0.5f), 100.0f);
            onlineMapsMarkertoput = OnlineMapsMarker3DManager.CreateItem(lng, lat, prefab);
            onlineMapsMarkertoput.sizeType = OnlineMapsMarker3D.SizeType.scene;
            onlineMapsMarkertoput.scale = 17;
            onlineMapsMarkertoput.rotation = Quaternion.Euler(85.803f, 0, -180.957f);

            onlineMapsMarkertoput.isDraggable = true;

            onlineMapsMarkertoput.OnRelease += StopDragging;


            markersMode.markerMode = false;

            PostProcessController.Instance.PopupSystem.ClosePopup(PopupType.PressHelper);

            PostProcessController.Instance.PopupSystem.ShowPopup(PopupType.DrageHelper, "");

            MapController.Instance.GetAddress();
        }

        public void CreatePin(Feed feed)
        {
            if (feed.tag == AppSettings.ShareTag)
            {
                _pinToCreate = SharePrefab;
            }
            else if (feed.tag == AppSettings.SaleTag)
            {
                _pinToCreate = SalePrefab;
            }
            else if (feed.tag == AppSettings.EventTag)
            {
                _pinToCreate = EventPrefab;
            }
            else if (feed.tag == AppSettings.PollTag)
            {
                _pinToCreate = PollPrefab;
            }
            else
            {
                _pinToCreate = SharePrefab;
            }

            GetProfileImageRequest _request = new GetProfileImageRequest();
            _request.Id = feed.OwnerID;
            _request.Size = ImageSize.Size_512;

            AppManager.FIREBASE_CONTROLLER.GetProfileImageForMarker(_request, feed, _pinToCreate,
                OnProfileImageMarkerGetted);
        }

        public void OnProfileImageMarkerGetted(GetProfileImageCallback _callback)
        {
            try
            {
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(_callback.ImageBytes);
                Debug.Log("length is" + _callback.ImageBytes.Length);
                SpriteRenderer sp = _callback.pref.GetComponentsInChildren<SpriteRenderer>()[1];
                sp.sprite = Sprite.Create(texture,
                    new Rect(0.0f, 0.0f, texture.width, texture.height+_extraHeight), new Vector2(0.5f, 0.5f),
                    100.0f);
                _callback.pref.GetComponentsInChildren<SpriteRenderer>()[1].transform.localScale =
                    new Vector3(0.114355631f, 0.114355631f, 0.114355631f);
                Feed _dataFeed = _callback.feed;
                OnlineMapsMarker3D onlineMapsMarkertoput =
                    OnlineMapsMarker3DManager.CreateItem(_dataFeed.Feedlng, _dataFeed.Feedlat, _callback.pref);
                int rand = Random.Range(0, _audioClips.Length -1);
                _audioSource.clip = _audioClips[rand];
                _audioSource.Play();
                onlineMapsMarkertoput.sizeType = OnlineMapsMarker3D.SizeType.scene;
                onlineMapsMarkertoput.scale = 2f;
                onlineMapsMarkertoput.rotation = Quaternion.identity;
                onlineMapsMarkertoput[MARKER_DATA] = _dataFeed;
                onlineMapsMarkertoput.OnClick += OnMarkerClick;
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }

        private void OnMarkerClick(OnlineMapsMarkerBase obj)
        {
           
            InfowWindowController.Instance.UpdateData(obj);
            int rand = Random.Range(0, _audioClips.Length -1);
            _audioSource.clip = _audioClips[rand];
            _audioSource.Play();
        }

        private void StopDragging(OnlineMapsMarkerBase obj)
        {
            markersMode.markerMode = false;

            obj.isDraggable = false;


            double lang, lat;
            obj.GetPosition(out lang, out lat);
            // longToSave = lang;
            // latToSave = lang;
            //
            PostProcessController.Instance.Longtitude = lang;
            PostProcessController.Instance.Latitude = lat;


            MapController.Instance.GetAddress();
        }

        #endregion

        #region Proerties

        public OnlineMapsMarker3D OnlineMapsMarkertoput => onlineMapsMarkertoput;

        public static PinFactory Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<PinFactory>();
                }

                return _instance;
            }
        }

        #endregion
    }
}