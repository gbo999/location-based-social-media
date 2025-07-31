using System;
using MobilePlacesSdk.Scripts.Internal.Dto;
using NinevaStudios.Places;
using NinevaStudios.Places.Internal;
using UnityEngine;

public class AutocompleteCallbackProxy : AndroidJavaProxy
{
    readonly Action<Place> _onAutocompleteSuccess;
    readonly Action<string> _onAutocompleteError;
    readonly Action _onAutocompleteCancel;
    
    public AutocompleteCallbackProxy(Action<Place> onAutocompleteSuccess, Action<string> onAutocompleteError, Action onAutocompleteCancel) : base(
        C.PlacesAutocompleteCallbackProxy)
    {
        _onAutocompleteSuccess = onAutocompleteSuccess;
        _onAutocompleteError = onAutocompleteError;
        _onAutocompleteCancel = onAutocompleteCancel;
    }
    
    void onAutocompleteSuccess(string placeJson)
    {
        PlacesSceneHelper.Queue(() =>
        {
            var placeDto = JsonUtility.FromJson<PlaceDto>(placeJson);
            _onAutocompleteSuccess?.Invoke(new Place(placeDto));
        });
    }
    
    void onAutocompleteError(string error)
    {
        PlacesSceneHelper.Queue(() => _onAutocompleteError?.Invoke(error));
    }
    
    void onAutocompleteCancel()
    {
        PlacesSceneHelper.Queue(() => _onAutocompleteCancel?.Invoke());
    }
}
