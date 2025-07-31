using System;
using NinevaStudios.Places.Internal;
using UnityEngine;

public class GetPhotoCallbackProxy : AndroidJavaProxy
{
	readonly Action<Texture2D> _onGetPhotoSuccess;
	readonly Action<string> _onGetPhotoError;

	internal GetPhotoCallbackProxy(Action<Texture2D> onGetPhotoSuccess, Action<string> onGetPhotoError) : base(C.GetPhotoCallbackProxy)
	{
		_onGetPhotoSuccess = onGetPhotoSuccess;
		_onGetPhotoError = onGetPhotoError;
	}

	void onGetPhotoSuccess(AndroidJavaObject bitmap)
	{
		PlacesSceneHelper.Queue(() =>
		{
			var texture = PlacesUtils.Texture2DFromBitmap(bitmap);
			_onGetPhotoSuccess?.Invoke(texture);
		});
	}

	void onGetPhotoError(string error)
	{
		PlacesSceneHelper.Queue(() => _onGetPhotoError?.Invoke(error));
	}
}