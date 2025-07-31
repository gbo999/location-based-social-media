using System;
using System.Collections.Generic;
using MobilePlacesSdk.Scripts.Internal.Dto;
using NinevaStudios.Places;
using NinevaStudios.Places.Internal;
using UnityEngine;

public class FindPlaceCallbackProxy : AndroidJavaProxy
{
    readonly Action<List<PlaceLikelihood>> _onFindPlaceSuccess;
    readonly Action<string> _onFindPlaceError;
    
    internal FindPlaceCallbackProxy(Action<List<PlaceLikelihood>> onFindPlaceSuccess, Action<string> onFindPlaceError) : base(C.PlacesFindPlaceCallbackProxy)
    {
        _onFindPlaceSuccess = onFindPlaceSuccess;
        _onFindPlaceError = onFindPlaceError;
    }

    void onFindPlaceSuccess(string placeLikelihoodsJson)
    {
        PlacesSceneHelper.Queue(() =>
        {
            var likelihoodsDto = JsonUtility.FromJson<PlaceLikelihoodsDto>(placeLikelihoodsJson);
            var likelihoods = new List<PlaceLikelihood>();

            foreach (var dto in likelihoodsDto.likelihoods)
            {
                likelihoods.Add(new PlaceLikelihood(dto));
            }
            
            
            _onFindPlaceSuccess?.Invoke(likelihoods);
        });
    }

    void onFindPlaceError(string error)
    {
        PlacesSceneHelper.Queue(() => _onFindPlaceError?.Invoke(error));
    }
}
