using System;
using System.Collections.Generic;

namespace MobilePlacesSdk.Scripts.Internal.Dto
{
    [Serializable]
    public struct PlaceLikelihoodDto
    {
        public float likelihood;
        public PlaceDto place;
    }

    [Serializable]
    public struct PlaceLikelihoodsDto
    {
        public List<PlaceLikelihoodDto> likelihoods;
    }
}