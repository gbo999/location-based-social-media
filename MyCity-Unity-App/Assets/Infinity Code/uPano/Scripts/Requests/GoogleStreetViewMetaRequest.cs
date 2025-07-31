/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using InfinityCode.uPano.Services;
using UnityEngine;

namespace InfinityCode.uPano.Requests
{
    public class GoogleStreetViewMetaRequest: StatusRequest<GoogleStreetViewMetaRequest>
    {
        private string _error;
        private GoogleStreetViewMeta _meta;
        private bool isDone;
        private bool requestByID = false;
        private bool isUserContent;

        public override string error
        {
            get { return _error; }
        }

        public override bool hasErrors
        {
            get { return !string.IsNullOrEmpty(_error); }
        }

        public override bool keepWaiting
        {
            get { return isDone; }
        }

        public GoogleStreetViewMeta meta
        {
            get { return _meta; }
        }

        public GoogleStreetViewMetaRequest(string key, string id, string language, GoogleStreetView.RegisterAs registerAs = GoogleStreetView.RegisterAs.loadMap)
        {
            if (registerAs == GoogleStreetView.RegisterAs.loadMap) new WWWRequest("https://maps.googleapis.com/maps/api/js?key=" + key);
            else if (registerAs == GoogleStreetView.RegisterAs.downloadStreetViewImage) new WWWRequest("https://maps.googleapis.com/maps/api/streetview?size=16x16&pano=" + id + "&fov=80&heading=70&pitch=0&key=" + key);

            requestByID = true;
            isUserContent = id.StartsWith("AF1");
            string url;
            if (isUserContent) url = "https://www.google.com/maps/photometa/v1?authuser=0&hl={1}&pb=!1m4!1smaps_sv.tactile!11m2!2m1!1b1!2m2!1s{1}!2s{1}!3m5!1m2!1e10!2s{0}!2m1!5s0x132f61b6532013ad%3A0x28f1c82e908503c4!4m57!1e1!1e2!1e3!1e4!1e5!1e6!1e8!1e12!2m1!1e1!4m1!1i48!5m1!1e1!5m1!1e2!6m1!1e1!6m1!1e2!9m36!1m3!1e2!2b1!3e2!1m3!1e2!2b0!3e3!1m3!1e3!2b1!3e2!1m3!1e3!2b0!3e3!1m3!1e8!2b0!3e3!1m3!1e1!2b0!3e3!1m3!1e4!2b0!3e3!1m3!1e10!2b1!3e2!1m3!1e10!2b0!3e3";
            else url = "https://maps.googleapis.com/maps/api/js/GeoPhotoService.GetMetadata?pb=!1m5!1sapiv3!5sUS!11m2!1m1!1b0!2m1!1s{1}!3m3!1m2!1e2!2s{0}!4m6!1e1!1e2!1e3!1e4!1e8!1e6&callback=_xdc_._s391fj";
            WWWRequest request = new WWWRequest(string.Format(url, id, language));
            request.OnComplete += OnRequestComplete;
        }

        public GoogleStreetViewMetaRequest(string key, double longitude, double latitude, string language, GoogleStreetView.RegisterAs registerAs = GoogleStreetView.RegisterAs.loadMap)
        {
            if (registerAs == GoogleStreetView.RegisterAs.loadMap) new WWWRequest("https://maps.googleapis.com/maps/api/js?key=" + key);
            else if (registerAs == GoogleStreetView.RegisterAs.downloadStreetViewImage) new WWWRequest(string.Format(CultureHelper.numberFormat, "https://maps.googleapis.com/maps/api/streetview?size=16x16&location={0},{1}&fov=80&heading=70&pitch=0&key={2}", latitude, longitude, key));

            string url = "https://maps.googleapis.com/maps/api/js/GeoPhotoService.SingleImageSearch?pb=!1m5!1sapiv3!5sUS!11m2!1m1!1b0!2m4!1m2!3d{1}!4d{0}!2d750!3m17!2m1!1s{2}!9m1!1e2!11m12!1m3!1e2!2b1!3e2!1m3!1e3!2b1!3e2!1m3!1e10!2b1!3e2!4m6!1e1!1e2!1e3!1e4!1e8!1e6&callback=_xdc_._dqzs51";
            url = string.Format(CultureHelper.cultureInfo, url, longitude, latitude, language);
            WWWRequest request = new WWWRequest(url);
            request.OnComplete += OnRequestComplete;
        }

        private void OnRequestComplete(WWWRequest request)
        {
            _error = request.error;
            isDone = true;

            if (!request.hasErrors)
            {
                if (request.text.Contains("Search returned no images.")) _error = "Search returned no images.";
                else
                {
                    try
                    {
                        _meta = new GoogleStreetViewMeta(request.text, requestByID);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                    
                }
            }

            BroadcastActions();
            Dispose();
        }
    }
}