using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using NinevaStudios.Places.Internal;
using NinevaStudios.Places.JniToolkit;
using UnityEngine;
#if UNITY_IOS
using System.Runtime.InteropServices;
using MobilePlacesSdk.Scripts.Internal.Dto;
#endif

namespace NinevaStudios.Places
{
	/// <summary>
	/// Entry point to Mobile Places SDK.
	/// </summary>
	[PublicAPI]
	public static class Places
	{
		/// <summary>
		/// Initialises the SDK. Must be called prior to invoking any methods in the plugin.
		/// </summary>
		public static void Init()
		{
			if (PlacesUtils.IsAndroid)
			{
				if (PlacesSettings.AndroidApiKey == PlacesSettings.ANDROID_KEY_PLACEHOLDER)
				{
					Debug.LogError("Android API key is default. Please, set the API key in the Editor (Window/Mobile Places SDK/Edit Settings)");
				}

				C.PlacesBridge.AJCCallStaticOnce("initialize", JniToolkitUtils.Activity, "AIzaSyACsKPzmDSn5GkEgEyl47zK4dAfj_ZA4rE");
			}

#if UNITY_IOS
			if (PlacesSettings.IosApiKey == PlacesSettings.IOS_KEY_PLACEHOLDER)
			{
				Debug.LogError("iOS API key is default. Please, set the API key in the Editor (Window/Mobile Places SDK/Edit Settings)");
			}

			_init(PlacesSettings.IosApiKey);
#endif
		}

		/// <summary>
		/// Fetch the approximate current location of the user's device.
		/// </summary>
		/// <param name="fields"> List of information fields fetched for the likely places. </param>
		/// <param name="onSuccess"> Callback to be invoked with the list of likely places. </param>
		/// <param name="onError"> Callback to be invoked with the error description if one occurs. </param>
		public static void FindCurrentPlace(List<Place.Field> fields, Action<List<PlaceLikelihood>> onSuccess,
			Action<string> onError)
		{
			var fieldsString = PlacesUtils.ParseFields(fields);

			if (PlacesUtils.IsAndroid)
			{
				_findPlaceCallbackProxy = new FindPlaceCallbackProxy(onSuccess, onError);

				C.PlacesBridge.AJCCallStaticOnce("findCurrentPlace",
					JniToolkitUtils.Activity, fieldsString, _findPlaceCallbackProxy);
				return;
			}

#if UNITY_IOS
			var stringAction = new Action<string>(json =>
			{
				var likelihoodsDto = JsonUtility.FromJson<PlaceLikelihoodsDto>(json);
				var likelihoods = new List<PlaceLikelihood>();

				foreach (var dto in likelihoodsDto.likelihoods)
				{
					likelihoods.Add(new PlaceLikelihood(dto));
				}


				onSuccess?.Invoke(likelihoods);
			});

			_findCurrentPlace(fieldsString, PlacesUtils.ActionStringCallback, stringAction.GetPointer(),
				PlacesUtils.ActionStringCallback, onError.GetPointer());
#endif
		}

		/// <summary>
		/// Fetch the details of a place.
		/// </summary>
		/// <param name="placeId"> ID of the place. </param>
		/// <param name="fields"> List of information fields fetched for the likely places. </param>
		/// <param name="onSuccess"> Callback to be invoked with the <see cref="Place"/> data. </param>
		/// <param name="onError"> Callback to be invoked with the error description if one occurs. </param>
		public static void FetchPlace(string placeId, List<Place.Field> fields, Action<Place> onSuccess,
			Action<string> onError)
		{
			var fieldsString = PlacesUtils.ParseFields(fields);

			if (PlacesUtils.IsAndroid)
			{
				_getPlaceCallbackProxy = new GetPlaceCallbackProxy(onSuccess, onError);

				C.PlacesBridge.AJCCallStaticOnce("getPlace", placeId, fieldsString, _getPlaceCallbackProxy);
				return;
			}

#if UNITY_IOS
			var stringAction = new Action<string>(json =>
			{
				var placeDto = JsonUtility.FromJson<PlaceDto>(json);
				onSuccess(new Place(placeDto));
			});

			_fetchPlace(placeId, fieldsString, PlacesUtils.ActionStringCallback, stringAction.GetPointer(),
				PlacesUtils.ActionStringCallback, onError.GetPointer());
#endif
		}

		/// <summary>
		/// Open a native place search autocompletion window.
		/// </summary>
		/// <param name="autocompleteOptions"> Search parameters. </param>
		/// <param name="onSuccess"> Callback to be invoked with the selected <see cref="Place"/>. </param>
		/// <param name="onError"> Callback to be invoked with the error description if one occurs. </param>
		/// <param name="onCancel"> Callback to be invoked if the user closes the window. </param>
		public static void ShowPlaceAutocomplete(AutocompleteOptions autocompleteOptions, Action<Place> onSuccess,
			Action<string> onError,
			Action onCancel)
		{
			if (PlacesUtils.IsAndroid)
			{
				_autocompleteCallbackProxy = new AutocompleteCallbackProxy(onSuccess, onError, onCancel);
				C.PlacesBridge.AJCCallStaticOnce("launchPlacesAutocomplete",
					JniToolkitUtils.Activity, autocompleteOptions.Json, _autocompleteCallbackProxy);
				return;
			}

#if UNITY_IOS
			var stringAction = new Action<string>(json =>
			{
				var placeDto = JsonUtility.FromJson<PlaceDto>(json);
				onSuccess(new Place(placeDto));
			});

			_showPlacesAutocompleteView(PlacesUtils.ActionStringCallback, stringAction.GetPointer(),
				PlacesUtils.ActionStringCallback, onError.GetPointer(), PlacesUtils.ActionVoidCallback,
				onCancel.GetPointer(), autocompleteOptions.Json);
#endif
		}

		/// <summary>
		/// Get metadata for the photos of the place.
		/// </summary>
		/// <param name="placeId"> ID of the place. </param>
		/// <param name="onSuccess"> Callback to be invoked with the photo metadatas. </param>
		/// <param name="onError"> Callback to be invoked with the error description if one occurs. </param>
		public static void GetPlacePhotos(string placeId, Action<List<PlacePhotoMetadata>> onSuccess,
			Action<string> onError)
		{
			if (PlacesUtils.IsAndroid)
			{
				_getPlacePhotosCallbackProxy = new GetPlacePhotosCallbackProxy(onSuccess, onError);
				C.PlacesBridge.AJCCallStaticOnce("getPlacePhotos", placeId, _getPlacePhotosCallbackProxy);
				return;
			}

#if UNITY_IOS
			var intAction = new Action<int>(count =>
			{
				var metadatas = new List<PlacePhotoMetadata>();

				for (var i = 0; i < count; i++)
				{
					metadatas.Add(new PlacePhotoMetadata(_getPhotoPointer(i)));
				}

				onSuccess(metadatas);
			});

			_getPlacePhotos(placeId, PlacesUtils.ActionIntCallback, intAction.GetPointer(),
				PlacesUtils.ActionStringCallback, onError.GetPointer());
#endif
		}

		/// <summary>
		/// Download the photo as Texture2D.
		/// </summary>
		/// <param name="metadata"> Metadata of the photo to be downloaded. </param>
		/// <param name="onSuccess"> Callback to be invoked with the photo texture. </param>
		/// <param name="onError"> Callback to be invoked with the error description if one occurs. </param>
		public static void FetchPhoto(PlacePhotoMetadata metadata, Action<Texture2D> onSuccess, Action<string> onError)
		{
			if (metadata == null)
			{
				onError("Metadata is not valid.");
				return;
			}

			if (PlacesUtils.IsAndroid)
			{
				_getPhotoCallbackProxy = new GetPhotoCallbackProxy(onSuccess, onError);
				C.PlacesBridge.AJCCallStaticOnce("fetchPhoto", metadata.JavaMetadata, _getPhotoCallbackProxy);
				return;
			}

#if UNITY_IOS
			_getPhoto(metadata.IosMetadata, PlacesUtils.ActionImageCallback, onSuccess.GetPointer(),
				PlacesUtils.ActionStringCallback, onError.GetPointer());
#endif
		}

		static AutocompleteCallbackProxy _autocompleteCallbackProxy;
		static GetPlaceCallbackProxy _getPlaceCallbackProxy;
		static FindPlaceCallbackProxy _findPlaceCallbackProxy;
		static GetPhotoCallbackProxy _getPhotoCallbackProxy;
		static GetPlacePhotosCallbackProxy _getPlacePhotosCallbackProxy;

#if UNITY_IOS
		[DllImport("__Internal")]
		static extern void _init(string apiKey);

		[DllImport("__Internal")]
		static extern void _showPlacesAutocompleteView(PlacesUtils.ActionStringCallbackDelegate onSuccess,
			IntPtr onSuccessPointer, PlacesUtils.ActionStringCallbackDelegate onError, IntPtr onErrorPointer,
			PlacesUtils.ActionVoidCallbackDelegate onCancelled, IntPtr onCancelledPointer, string serializedFilter);

		[DllImport("__Internal")]
		static extern void _fetchPlace(string placeId, string typesString,
			PlacesUtils.ActionStringCallbackDelegate onSuccess,
			IntPtr onSuccessPointer, PlacesUtils.ActionStringCallbackDelegate onError, IntPtr onErrorPointer);

		[DllImport("__Internal")]
		static extern void _findCurrentPlace(string typesString, PlacesUtils.ActionStringCallbackDelegate onSuccess,
			IntPtr onSuccessPointer, PlacesUtils.ActionStringCallbackDelegate onError, IntPtr onErrorPointer);

		[DllImport("__Internal")]
		static extern void _getPlacePhotos(string placeId, PlacesUtils.ActionIntCallbackDelegate onSuccess,
			IntPtr onSuccessPointer,
			PlacesUtils.ActionStringCallbackDelegate onError, IntPtr onErrorPointer);

		[DllImport("__Internal")]
		static extern void _getPhoto(IntPtr metadataPointer, PlacesUtils.ActionImageCallbackDelegate onSuccess,
			IntPtr onSuccessPointer,
			PlacesUtils.ActionStringCallbackDelegate onError, IntPtr onErrorPointer);

		[DllImport("__Internal")]
		static extern IntPtr _getPhotoPointer(int index);
#endif
	}
}