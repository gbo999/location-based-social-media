/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using System.Collections.Generic;
using InfinityCode.uPano.Services;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InfinityCode.uPano.Requests
{
    /// <summary>
    /// Request for Google Street View, which works in edit and play mode
    /// </summary>
    public class GoogleStreetViewRequest : StatusRequest<GoogleStreetViewRequest>
    {
        private string _error;
        private bool _isUserContent;
        private GoogleStreetViewMeta _meta;
        private float _northPan = 0;
        private string _panoID;
        private Texture2D _texture;
        private int countSuccess;
        private bool isDone;
        private int maxX;
        private int maxY;
        private List<WWWRequest> requests;
        private Texture2D tempTexture;
        private int zoom;

        public override string error
        {
            get { return _error; }
        }

        public override bool hasErrors
        {
            get { return !string.IsNullOrEmpty(_error); }
        }

        public bool isUserContent
        {
            get { return _isUserContent; }
        }

        /// <summary>
        /// Indicates if coroutine should be kept suspended
        /// </summary>
        public override bool keepWaiting
        {
            get { return isDone; }
        }

        public GoogleStreetViewMeta meta
        {
            get { return _meta; }
        }

        public float northPan
        {
            get { return _northPan; }
        }

        public string panoID
        {
            get { return _panoID; }
        }

        /// <summary>
        /// Gets a result texture
        /// </summary>
        public Texture2D texture
        {
            get { return _texture; }
        }

        /// <summary>
        /// Makes a new request to Google Street View by id
        /// </summary>
        /// <param name="key">Google API key</param>
        /// <param name="id">ID of panorama</param>
        /// <param name="zoom">Zoom of panorama (0-4). Size of side = 512 * 2 ^ zoom</param>
        /// <param name="downloadMeta">Download meta data of a panorama</param>
        /// <param name="language">Language code of the meta</param>
        /// <param name="registerAs">How to register a request.</param>
        public GoogleStreetViewRequest(string key, string id, int zoom, bool downloadMeta = false, string language = "en", GoogleStreetView.RegisterAs registerAs = GoogleStreetView.RegisterAs.loadMap)
        {
            this.zoom = zoom;
            _panoID = id;

            maxX = 1 << zoom;
            maxY = zoom > 0 ? maxX / 2 : 1;

            if (downloadMeta)
            {
                GoogleStreetViewMetaRequest request = new GoogleStreetViewMetaRequest(key, id, language, registerAs);
                request.OnComplete += OnDownloadMetaComplete;
            }
            else DownloadByID(id);
        }

        /// <summary>
        /// Makes a new request to Google Street View by location
        /// </summary>
        /// <param name="key">Google API key</param>
        /// <param name="longitude">Longitude</param>
        /// <param name="latitude">Latitude</param>
        /// <param name="zoom">Zoom of panorama (0-4). Size of side = 512 * 2 ^ zoom</param>
        /// <param name="language">Language code of the meta</param>
        /// <param name="registerAs">How to register a request.</param>
        public GoogleStreetViewRequest(string key, double longitude, double latitude, int zoom, string language = "en", GoogleStreetView.RegisterAs registerAs = GoogleStreetView.RegisterAs.loadMap)
        {
            this.zoom = zoom;

            maxX = 1 << zoom;
            maxY = zoom > 0 ? maxX / 2 : 1;

            GoogleStreetViewMetaRequest request = new GoogleStreetViewMetaRequest(key, longitude, latitude, language, registerAs);
            request.OnComplete += OnDownloadMetaComplete;
        }

        public override void Dispose()
        {
            if (requests != null)
            {
                foreach (WWWRequest request in requests)
                {
                    if (request != null) request.Dispose();
                }
                requests = null;
            }

            if (tempTexture != null)
            {
                Object.Destroy(tempTexture);
                tempTexture = null;
            }

            _texture = null;
        }

        private void DownloadByID(string panoID)
        {
            requests = new List<WWWRequest>();

            tempTexture = new Texture2D(maxX * 512, maxY * 512, TextureFormat.RGB24, false);
            _isUserContent = panoID.StartsWith("AF1");

            for (int y = 0; y < maxY; y++)
            {
                for (int x = 0; x < maxX; x++)
                {
                    string url;
                    if (!_isUserContent) url = "https://geo0.ggpht.com/cbk?cb_client=apiv3&authuser=0&hl=en&panoid=" + panoID + "&output=tile&x=" + x + "&y=" + y + "&zoom=" + zoom + "&nbt&fover=2";
                    else url = "https://lh3.ggpht.com/p/" + panoID + "=x" + x + "-y" + y + "-z" + zoom;
                    WWWRequest request = new WWWRequest(url);
                    requests.Add(request);
                    int px = x;
                    int py = y;
                    request.OnComplete += delegate { OnPanoPartDownloaded(request, px, py); };
                }
            }
        }

        private void FinalizeTexture()
        {
            if (tempTexture == null) return;

            tempTexture.Apply();

            int startHeight = maxY * 512;
            int realHeight = startHeight;
            int step = realHeight / 4;
            int c = realHeight / 2;

            while (step != 1)
            {
                Color32 c1 = tempTexture.GetPixel(0, c);
                Color32 c2 = tempTexture.GetPixel(100, c);
                Color32 c3 = tempTexture.GetPixel(500, c);

                if (IsEmptyColor(c1) && IsEmptyColor(c2) && IsEmptyColor(c3))
                {
                    c += step;
                }
                else
                {
                    realHeight = startHeight - c;
                    c -= step;
                }

                step /= 2;
            }

            if (realHeight == -1)
            {
                return;
            }

            int realWidth = realHeight * 2;

            _texture = new Texture2D(realWidth, realHeight, TextureFormat.RGB24, false);
            Graphics.CopyTexture(tempTexture, 0, 0, 0, startHeight - realHeight, realWidth, realHeight, _texture, 0, 0, 0, 0);
            _texture.Apply();

            Object.DestroyImmediate(tempTexture);
        }

        /// <summary>
        /// Check color for the color of blank areas
        /// </summary>
        /// <param name="c">Color</param>
        /// <returns>True - is color of blank area, false - otherwise</returns>
        public static bool IsEmptyColor(Color32 c)
        {
            if (c.r == 0 && c.g == 0 && c.b == 0) return true;
            if (c.r == 205 && c.g == 205 && c.b == 205) return true;
            return false;
        }

        private void OnDownloadMetaComplete(GoogleStreetViewMetaRequest request)
        {
            if (request.hasErrors)
            {
                _error = request.error;
                BroadcastActions();
                return;
            }

            try
            {
                _meta = request.meta;
                _panoID = meta.id;
                _northPan = meta.northPan;

                DownloadByID(_panoID);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private void OnPanoPartDownloaded(WWWRequest www, int x, int y)
        {
            requests[y * maxX + x] = null;

            if (www.hasErrors)
            {
                TryFinalize();
                return;
            }

            countSuccess++;

            Texture2D partTexture = www.texture;
            Graphics.CopyTexture(partTexture, 0, 0, 0, 0, partTexture.width, partTexture.height, tempTexture, 0, 0, x * 512, (maxY - y - 1) * 512);
            Object.DestroyImmediate(partTexture);

            TryFinalize();
        }

        private void TryFinalize()
        {
            foreach (WWWRequest request in requests)
            {
                if (request != null) return;
            }

            if (countSuccess == 0) _error = "Can not download any part of the texture.";
            FinalizeTexture();

            isDone = true;
            BroadcastActions();
            Dispose();
        }
    }
}