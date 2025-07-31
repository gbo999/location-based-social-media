using System;
using System.Collections.Generic;
using NinevaStudios.Places.Internal;
using NinevaStudios.Places.JniToolkit;
using UnityEngine;

public class GetPlacePhotosCallbackProxy : AndroidJavaProxy
{
	readonly Action<List<PlacePhotoMetadata>> _onGetPlacePhotosSuccess;
	readonly Action<string> _onGetPlacePhotosError;

	internal GetPlacePhotosCallbackProxy(Action<List<PlacePhotoMetadata>> onGetPlacePhotosSuccess, Action<string> onGetPlacePhotosError) : base(
		C.PlacesGetPlacePhotosCallbackProxy)
	{
		_onGetPlacePhotosSuccess = onGetPlacePhotosSuccess;
		_onGetPlacePhotosError = onGetPlacePhotosError;
	}

	void onGetPhotosSuccess(AndroidJavaObject photosList)
	{
		PlacesSceneHelper.Queue(() =>
		{
			var javaList = photosList.FromJavaList<AndroidJavaObject>();
			var metadatas = new List<PlacePhotoMetadata>();
			javaList.ForEach(javaMetadata => metadatas.Add(new PlacePhotoMetadata(javaMetadata)));
			_onGetPlacePhotosSuccess?.Invoke(metadatas);
		});
	}

	void onGetPhotosError(string error)
	{
		PlacesSceneHelper.Queue(() => _onGetPlacePhotosError?.Invoke(error));
	}
}