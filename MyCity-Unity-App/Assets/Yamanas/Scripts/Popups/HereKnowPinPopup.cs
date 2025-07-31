using System;
using System.Collections.Generic;
using System.Linq;
using Firebase.Database;
using NinevaStudios.Places;
using RSG;
using TMPro;
using UnityEngine;
using Yamanas.Infrastructure.Popups;
using Yamanas.Scripts.Map;
using Yamanas.Scripts.MapLoader.Flows;

namespace Yamanas.Scripts.MapLoader.Popups
{
    public class HereKnowPinPopup : MonoBehaviour,IPromisePopup<PostLocationData> {
        #region Fields

        private OnlineMapsMarker marker;

        [SerializeField] private UIElementsGroup searchGroup;

        [SerializeField] private TMP_InputFieldSearchable SearchInput;

        [SerializeField] private TMP_DropdownSearchable droptwo;

        private Promise<PostLocationData> _promise;
        #endregion

        #region Methods

        
        
        public void SetPromise(Promise<PostLocationData> dataPromise)
        {
            _promise = dataPromise;
        }

        public void OnCloseButtonClick()
        {
            PopupSystem.Instance.ClosePopup(PopupType.HereOrAddressOrPin);
        }
        
        public void OnPinButtonCLick()
        {
            markersMode.markerMode = true;
            PopupSystem.Instance.ShowPopup(PopupType.PressHelper, "");
            
            
        }

        public void OnHereButtonCLick()
        {
            float lng, lat;

            OnlineMapsLocationService.instance.GetLocation(out lng, out lat);

            OnlineMaps.instance.position = new Vector2(lng, lat);


            if (marker == null) marker = OnlineMapsMarkerManager.CreateItem(OnlineMaps.instance.position);
            else
            {
                marker.position = OnlineMaps.instance.position;
            }

            PostProcessController.Instance.Longtitude = lng;
            PostProcessController.Instance.Latitude = lat;

            MapController.Instance.GetAddress(lng,lat,OnAddressGet);
            
            
            
           // PopupSystem.Instance.ShowPopup(PopupType.ApproveLocation,PostProcessController.Instance.Address);
            
           // PostProcessController.Instance.PopupSystem.ShowPopup(PopupType.ChooseActivity, "");
        }

        private void OnAddressGet(string address)
        {
            
            PopupSystem.Instance.ShowPopup(PopupType.ApproveLocation,address);
        }
        
        
        
        
        

        public void OnKnowButtonCLick()
        {
            
            PopupSystem.Instance.ClosePopup(PopupType.HereOrAddressOrPin);
#if UNITY_EDITOR

            searchGroup.ChangeVisibility(true);
            droptwo.OnSelected.AddListener(SearchDropDown);
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
            OnShowPlacesAutocomplete();
#endif
        }

        public void OnShowPlacesAutocomplete()
        {
            var europeBounds = new Place.LatLngBounds(new Place.LatLng(40, -10), new Place.LatLng(70, 40));
            var options =
                new AutocompleteOptions.Builder( /* show a full screen mode activity */ AutocompleteMode.Overlay,
                        new List<Place.Field> {Place.Field.Id})
                    .Build();
            Places.ShowPlaceAutocomplete(options, HandlePlace, ShowText,
                () => { ShowText("Autocomplete was cancelled."); });
        }

        private void ShowText(string obj)
        {
        }

        private void HandlePlace(Place place)
        {
            if (!string.IsNullOrEmpty(place.placeID))
            {
                OnGetPlaceClicked(place.placeID);
            }
        }

        public void OnGetPlaceClicked(string placeId)
        {
            if (string.IsNullOrEmpty(placeId))
            {
                //ShowText("Place ID is empty. Please, fill it before calling GetPlace.");
                return;
            }

            var allFields = Enum.GetValues(typeof(Place.Field)).OfType<Place.Field>().ToList();
            Places.FetchPlace(placeId, allFields, OnPlaceChosen,
                ShowText);
        }

        private void OnPlaceChosen(Place obj)
        {
            PostProcessController.Instance.Address = obj.name;
            PostProcessController.Instance.PopupSystem.ShowPopup(PopupType.ApproveLocation, obj.name);
            OnlineMapsGoogleGeocoding request =
                new OnlineMapsGoogleGeocoding(obj.name, OnlineMapsKeyManager.GoogleMaps());
            request.OnComplete += OnGeocodingComplete;
            request.Send();

         
        }


        public void SearchDropDown()
        {
            if (!OnlineMapsKeyManager.hasGoogleMaps)
            {
                Debug.LogWarning("Please enter Map / Key Manager / Google Maps");
                return;
            }

            /*if (inputField == null) return;
            if (inputField.text.Length < 3) return;*/

            Debug.Log("the dropdown value " + droptwo.options[droptwo.value].text);


            string locationName = droptwo.options[droptwo.value].text;

            PostProcessController.Instance.Address = locationName;

            OnlineMapsGoogleGeocoding request =
                new OnlineMapsGoogleGeocoding(locationName, OnlineMapsKeyManager.GoogleMaps());
            request.OnComplete += OnGeocodingComplete;
            request.Send();

            searchGroup.ChangeVisibility(false);

            SearchInput.text = "";
        }

        private void OnGeocodingComplete(string response)
        {
            OnlineMapsGoogleGeocodingResult[] results = OnlineMapsGoogleGeocoding.GetResults(response);
            if (results == null || results.Length == 0)
            {
                Debug.Log(response);
                return;
            }

            OnlineMapsGoogleGeocodingResult r = results[0];
            OnlineMaps.instance.position = r.geometry_location;

            Vector2 center;
            int zoom;
            OnlineMapsUtils.GetCenterPointAndZoom(new[] {r.geometry_bounds_northeast, r.geometry_bounds_southwest},
                out center, out zoom);
            OnlineMaps.instance.zoom = zoom;

           // if (marker == null) 
                
                marker = OnlineMapsMarkerManager.CreateItem(r.geometry_location, r.formatted_address);
            
                marker.position = r.geometry_location;
                marker.label = r.formatted_address;

                PostProcessController.Instance.Longtitude = r.geometry_location.x;
                PostProcessController.Instance.Latitude = r.geometry_location.y;

                PostProcessController.Instance.PopupSystem.ShowPopup(PopupType.ApproveLocation,
                    PostProcessController.Instance.Address);
            
        }

        #endregion


       
    }
}