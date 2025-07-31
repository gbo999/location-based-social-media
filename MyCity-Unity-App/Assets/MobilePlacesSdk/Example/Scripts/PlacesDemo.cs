using System;
using System.Collections.Generic;
using System.Linq;
using NinevaStudios.Places.Internal;
using UnityEngine;
using UnityEngine.UI;

namespace NinevaStudios.Places.Demo
{
	public class PlacesDemo : MonoBehaviour
	{
		[SerializeField] InputField placeIdInput;
		[SerializeField] Image photoImage;
		[SerializeField] Button getPhotoButton;
		[SerializeField] Text debugText;

		PlacePhotoMetadata _selectedMetadata;

		void Start() => Places.Init();

		public void OnShowPlacesAutocomplete()
		{
			var europeBounds = new Place.LatLngBounds(new Place.LatLng(40, -10), new Place.LatLng(70, 40));
			var options = new AutocompleteOptions.Builder( /* show a full screen mode activity */ AutocompleteMode.Fullscreen, new List<Place.Field> {Place.Field.Id})
				.SetLocationBias(europeBounds)
				.Build();
			Places.ShowPlaceAutocomplete(options, HandlePlace, ShowText, () => { ShowText("Autocomplete was cancelled."); });
		}

		public void OnShowPlacesAutocompleteAdvanced()
		{
			var placeFields = new List<Place.Field> {Place.Field.Id, Place.Field.Name}; // fetch only place Id and name
			var ukraineBounds = new Place.LatLngBounds(new Place.LatLng(46, 22), new Place.LatLng(53, 40));
			var options = new AutocompleteOptions.Builder( /* show a full screen mode activity */ AutocompleteMode.Fullscreen, placeFields)
				.SetHint("This is a hint")
				.SetInitialQuery("Lviv")
				.SetTypeFilter(TypeFilter.Establishment)
				.SetLocationRestriction(ukraineBounds)
				.SetCountries(new List<string> {"UA"})
				.Build();
			Places.ShowPlaceAutocomplete(options, HandlePlace, ShowText, () => {ShowText("Autocomplete was cancelled.");});
		}

		public void OnGetPlaceClicked()
		{
			if (string.IsNullOrEmpty(placeIdInput.text))
			{
				ShowText("Place ID is empty. Please, fill it before calling GetPlace.");
				return;
			}

			var allFields = Enum.GetValues(typeof(Place.Field)).OfType<Place.Field>().ToList();
			Places.FetchPlace(placeIdInput.text, allFields, HandlePlace,
				ShowText);
		}

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
		}

		void HandlePlace(Place place)
		{
			ShowText(place.ToString());
			foreach (var addressComponent in place.addressComponents)
			{
				print($"Name: {addressComponent.name}, types: {string.Join(",", addressComponent.types)}");
			}

			if (!string.IsNullOrEmpty(place.placeID))
			{
				placeIdInput.text = place.placeID;
			}
		}

		void HandlePlaceLikelihoods(List<PlaceLikelihood> likelihoods)
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
		}
	}
}