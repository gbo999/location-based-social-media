using System;
using System.Collections.Generic;
using System.Linq;
using NinevaStudios.Places.Internal;
using UnityEngine;
using UnityEngine.UI;

namespace NinevaStudios.Places.Demo
{
    public class searchYamanas : MonoBehaviour
    {
        /*[SerializeField] InputField placeIdInput;
        [SerializeField] Image photoImage;
        [SerializeField] Button getPhotoButton;
        [SerializeField] Text debugText;

        PlacePhotoMetadata _selectedMetadata;*/

        #region Editor

        private OnlineMapsMarker marker;

        [SerializeField] private Text _debugText;

        #endregion


        void Start() => Places.Init();

        public void OnShowPlacesAutocomplete()
        {
#if !UNITY_EDITOR && UNITY_ANDROID
			var europeBounds = new Place.LatLngBounds(new Place.LatLng(40, -10), new Place.LatLng(70, 40));
			var options =
 new AutocompleteOptions.Builder( /* show a full screen mode activity */ AutocompleteMode.Overlay, new List<Place.Field> {Place.Field.Id})
								.Build();
			Places.ShowPlaceAutocomplete(options, HandlePlace, ShowText, () => { ShowText("Autocomplete was cancelled."); });

#endif
        }

        private void ShowText(string obj)
        {
            _debugText.text = obj;
        }

        public void OnShowPlacesAutocompleteAdvanced()
        {
            var placeFields = new List<Place.Field> {Place.Field.Id, Place.Field.Name}; // fetch only place Id and name
            var ukraineBounds = new Place.LatLngBounds(new Place.LatLng(46, 22), new Place.LatLng(53, 40));
            var options = new AutocompleteOptions.Builder( /* show a full screen mode activity */
                    AutocompleteMode.Fullscreen, placeFields)
                .SetHint("This is a hint")
                .SetInitialQuery("Lviv")
                .SetTypeFilter(TypeFilter.Establishment)
                .SetLocationRestriction(ukraineBounds)
                .SetCountries(new List<string> {"UA"})
                .Build();
            Places.ShowPlaceAutocomplete(options, HandlePlace, ShowText,
                () => { ShowText("Autocomplete was cancelled."); });
        }

        public void OnGetPlaceClicked(string placeId)
        {
            if (string.IsNullOrEmpty(placeId))
            {
                //ShowText("Place ID is empty. Please, fill it before calling GetPlace.");
                return;
            }

            var allFields = Enum.GetValues(typeof(Place.Field)).OfType<Place.Field>().ToList();
            Places.FetchPlace(placeId, allFields, search,
                ShowText);
        }

        private void search(Place obj)
        {
            if (!OnlineMapsKeyManager.hasGoogleMaps)
            {
                Debug.LogWarning("Please enter Map / Key Manager / Google Maps");
                return;
            }

            if (obj.formattedAddress == null) return;
            if (obj.formattedAddress.Length < 3) return;

            string locationName = obj.formattedAddress;

            OnlineMapsGoogleGeocoding request =
                new OnlineMapsGoogleGeocoding(locationName, OnlineMapsKeyManager.GoogleMaps());
            request.OnComplete += OnGeocodingComplete;
            request.Send();
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

            if (marker == null) marker = OnlineMapsMarkerManager.CreateItem(r.geometry_location, r.formatted_address);
            else
            {
                marker.position = r.geometry_location;
                marker.label = r.formatted_address;
            }
        }


        void HandlePlace(Place place)
        {
            //ShowText(place.ToString());
            foreach (var addressComponent in place.addressComponents)
            {
                print($"Name: {addressComponent.name}, types: {string.Join(",", addressComponent.types)}");
            }

            if (!string.IsNullOrEmpty(place.placeID))
            {
                OnGetPlaceClicked(place.placeID);
            }
        }
    }
/*
        public void OnFindPlaceClicked()
		{
			PlacesUtils.RequestPermission(PlacesUtils.LocationPermission, (permission, granted) =>
			{
				if (permission != PlacesUtils.LocationPermission)
				{
					ShowText("Wrong permission.");
					return;
				}

				if (granted)
				{
					Places.FindCurrentPlace(new List<Place.Field> {Place.Field.Id, Place.Field.Name, Place.Field.LatLng}, HandlePlaceLikelihoods, print);
				}
				else
				{
					ShowText("Permission was not granted by the user.");
				}
			});
		}

		public void OnGetPlacePhotosClicked()
		{
			if (string.IsNullOrEmpty(placeIdInput.text))
			{
				ShowText("Place ID is empty. Please, fill it before calling GetPlacePhotos.");
				return;
			}

			Places.GetPlacePhotos(placeIdInput.text, OnGetPlacePhotosSuccess, OnGetPlacePhotosError);
		}

		public void OnFetchPhotoClicked()
		{
			Places.FetchPhoto(_selectedMetadata, OnPhotoReceived, OnPhotoFetchError);
		}*/


    /*	void HandlePlaceLikelihoods(List<PlaceLikelihood> likelihoods)
        {
            likelihoods = likelihoods.OrderByDescending(likelihood => likelihood.likelihood).ToList();
            var mostLikelyPlace = likelihoods[0];
            ShowText($"Most likely place: {mostLikelyPlace.place.name} ({mostLikelyPlace.likelihood * 100f}%). Place data: {mostLikelyPlace.place}");
        }

        void OnPhotoReceived(Texture2D photo)
        {
            photoImage.color = Color.white;
            photoImage.sprite = Sprite.Create(photo, new Rect(0, 0, photo.width, photo.height), new Vector2(0.5f, 0.5f));
        }

        void OnPhotoFetchError(string error)
        {
            photoImage.color = Color.clear;
            ShowText(error);
        }

        void OnGetPlacePhotosSuccess(List<PlacePhotoMetadata> metadatas)
        {
            if (metadatas.Count < 1)
            {
                OnGetPlacePhotosError("Photo metadata list is empty.");
                return;
            }

            _selectedMetadata = metadatas.First();
            getPhotoButton.interactable = true;
            ShowText($"Selected photo metadata. Width: {_selectedMetadata.Width}, height: {_selectedMetadata.Height}, attributions: {_selectedMetadata.Attributions}.");
        }

        void OnGetPlacePhotosError(string error)
        {
            getPhotoButton.interactable = false;
            ShowText(error);
        }

        void ShowText(string message)
        {
            debugText.text = message;
        }*/
}