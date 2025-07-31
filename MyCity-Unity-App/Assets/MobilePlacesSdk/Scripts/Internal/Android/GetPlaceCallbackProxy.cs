using System;
using MobilePlacesSdk.Scripts.Internal.Dto;
using NinevaStudios.Places;
using NinevaStudios.Places.Internal;
using UnityEngine;

public class GetPlaceCallbackProxy : AndroidJavaProxy
{
    readonly Action<Place> _onGetPlaceSuccess;
    readonly Action<string> _onGetPlaceError;
    
    internal GetPlaceCallbackProxy(Action<Place> onGetPlaceSuccess, Action<string> onGetPlaceError) : base(C.PlacesGetPlaceCallbackProxy)
    {
        _onGetPlaceSuccess = onGetPlaceSuccess;
        _onGetPlaceError = onGetPlaceError;
    }

    void onGetPlaceSuccess(string placeJson)
    {
        PlacesSceneHelper.Queue(() =>
        {
            var placeDto = JsonUtility.FromJson<PlaceDto>(placeJson);
            _onGetPlaceSuccess?.Invoke(new Place(placeDto));
        });
    }

    void onGetPlaceError(string error)
    {
        PlacesSceneHelper.Queue(() => _onGetPlaceError?.Invoke(error));
    }
}