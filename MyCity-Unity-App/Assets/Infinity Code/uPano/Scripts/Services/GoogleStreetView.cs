/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using InfinityCode.uPano.Attributes;
using InfinityCode.uPano.Directions;
using InfinityCode.uPano.InteractiveElements;
using InfinityCode.uPano.Plugins;
using InfinityCode.uPano.Renderers;
using InfinityCode.uPano.Requests;
using UnityEngine;

namespace InfinityCode.uPano.Services
{
    /// <summary>
    /// Component for downloading panoramas from Google Street View
    /// </summary>
    [RequirePanoRenderer(typeof(SphericalPanoRenderer))]
    [WizardEnabled(false)]
    [AddComponentMenu("uPano/Services/Google Street View")]
    public class GoogleStreetView: Plugin
    {
        /// <summary>
        /// Types of downloading panoramas
        /// </summary>
        public enum LoadType
        {
            /// <summary>
            /// Download a panorama by id
            /// </summary>
            id,

            /// <summary>
            /// Download a panorama by location
            /// </summary>
            location
        }

        /// <summary>
        /// How to register a request
        /// </summary>
        public enum RegisterAs
        {
            /// <summary>
            /// Register as load of map
            /// </summary>
            loadMap = 0,

            /// <summary>
            /// Register as Street View Image request
            /// </summary>
            downloadStreetViewImage = 1
        }

        /// <summary>
        /// Action that occurs when the request is failed
        /// </summary>
        public Action<GoogleStreetViewRequest> OnError;

        /// <summary>
        /// Action that occurs when a panorama or zoom level is loaded.
        /// </summary>
        public Action OnLoaded;

        /// <summary>
        /// Google API key
        /// </summary>
        public string apiKey;

        /// <summary>
        /// Show directions to neighboring panoramas
        /// </summary>
        public bool directions = true;

        /// <summary>
        /// Prefab of a direction arrow
        /// </summary>
        public GameObject directionPrefab;

        /// <summary>
        /// Type of downloading a panorama
        /// </summary>
        public LoadType loadType = LoadType.id;

        /// <summary>
        /// Latitude of a panorama
        /// </summary>
        public double locationLat;

        /// <summary>
        /// Longitude of a panorama
        /// </summary>
        public double locationLng;

        /// <summary>
        /// Metadata for the current panorama.
        /// </summary>
        public GoogleStreetViewMeta meta;

        /// <summary>
        /// ID of a panorama 
        /// </summary>
        public string panoID;

        /// <summary>
        /// Use progressive loading panoramas (zoom - 0, 1, 2, 3, 4)
        /// </summary>
        public bool progressive = true;

        /// <summary>
        /// Zoom of panorama (0-4). Size of side = 512 * 2 ^ zoom
        /// </summary>
        [Range(0, 4)]
        public int zoom = 3;

        /// <summary>
        /// How to register a request
        /// </summary>
        public RegisterAs registerAs = RegisterAs.loadMap;

        private int currentZoom = 0;
        private GoogleStreetViewRequest currentRequest;
        

        private void CreateDirections(GoogleStreetViewRequest request, SphericalPanoRenderer panoRenderer)
        {
            if (!directions) return;

            DirectionManager directionManager = panoRenderer.GetComponent<DirectionManager>();
            if (directionManager == null) directionManager = panoRenderer.gameObject.AddComponent<DirectionManager>();

            directionManager.Clear();

            try
            {
                foreach (GoogleStreetViewDirection item in request.meta.nearestDirections)
                {
                    Direction direction = directionManager.Create(item.pan, directionPrefab);
                    if (!string.IsNullOrEmpty(item.title)) direction.title = item.title;
                    direction["id"] = item.id;
                    direction.OnClick.AddListener(LoadNextPanorama);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        /// <summary>
        /// Start downloading the panorama
        /// </summary>
        /// <returns>Instance of the request</returns>
        public GoogleStreetViewRequest Download()
        {
            currentZoom = zoom;
            if (progressive) currentZoom = 0;

            if (loadType == LoadType.location) currentRequest = DownloadByLocation(apiKey, locationLng, locationLat, currentZoom, "en", registerAs);
            else currentRequest = DownloadByID(apiKey, panoID, currentZoom, "en", registerAs);
            return currentRequest;
        }

        /// <summary>
        /// Download panorama by id
        /// </summary>
        /// <param name="key">Google API key</param>
        /// <param name="id">ID of the panorama</param>
        /// <param name="zoom">Zoom of panorama (0-4). Size of side = 512 * 2 ^ zoom</param>
        /// <param name="language">Language code of the meta</param>
        /// <param name="registerAs">How to register a request</param>
        /// <returns>Instance of the request</returns>
        public static GoogleStreetViewRequest DownloadByID(string key, string id, int zoom = 3, string language = "en", RegisterAs registerAs = RegisterAs.loadMap)
        {
            GoogleStreetViewRequest request = new GoogleStreetViewRequest(key, id, zoom, true, language, registerAs);
            return request;
        }

        /// <summary>
        /// Download panorama by location
        /// </summary>
        /// <param name="key">Google API key</param>
        /// <param name="lng">Longitude</param>
        /// <param name="lat">Latitude</param>
        /// <param name="zoom">Zoom of panorama (0-4). Size of side = 512 * 2 ^ zoom</param>
        /// <param name="language">Language code of the meta</param>
        /// <param name="registerAs">How to register a request</param>
        /// <returns>Instance of the request</returns>
        public static GoogleStreetViewRequest DownloadByLocation(string key, double lng, double lat, int zoom = 3, string language = "en", RegisterAs registerAs = RegisterAs.loadMap)
        {
            GoogleStreetViewRequest request = new GoogleStreetViewRequest(key, lng, lat, zoom, language, registerAs);
            return request;
        }

        private void LoadNextPanorama(InteractiveElement element)
        {
            if (currentRequest != null) currentRequest.Dispose();

            currentZoom = progressive ? 0 : zoom;
            GoogleStreetViewRequest request = new GoogleStreetViewRequest(apiKey, element["id"] as string, zoom, true);
            request.OnSuccess += OnRequestComplete;
        }

        private void OnRequestComplete(GoogleStreetViewRequest request)
        {
            if (request.hasErrors)
            {
                if (OnError != null) OnError(request);
                Debug.LogWarning(request.error);
                return;
            }

            panoID = request.panoID;

            SphericalPanoRenderer panoRenderer = GetComponent<SphericalPanoRenderer>();
            if (panoRenderer == null) return;

            panoRenderer.texture = request.texture;

            if (request.meta != null)
            {
                meta = request.meta;
                float pan = panoRenderer.pano.pan;
                panoRenderer.pano.northPan = request.northPan;
                panoRenderer.pano.pan = pan;

                CreateDirections(request, panoRenderer);
            }

            if (OnLoaded != null) OnLoaded();

            if (progressive && currentZoom < zoom)
            {
                currentZoom++;
                currentRequest = new GoogleStreetViewRequest(apiKey, panoID, currentZoom);
                currentRequest.OnComplete += OnRequestComplete;
            }
        }

        protected override void Start()
        {
            base.Start();

            if (_pano == null) return;

            Download().OnComplete += OnRequestComplete;
        }
    }
}
