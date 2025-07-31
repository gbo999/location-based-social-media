using System;
using System.Collections.Generic;
using NinevaStudios.Places;

namespace MobilePlacesSdk.Scripts.Internal.Dto
{
	[Serializable]
	public struct PlaceDto
	{
		public int businessStatus;
		public string name;
		public string placeID;
		public string phoneNumber;
		public string formattedAddress;
		public float rating;
		public int priceLevel;
		public List<int> types;
		public Place.LatLng coordinate;
		public string website;
		public string attributions;
		public Place.LatLngBounds viewport;
		public List<Place.AddressComponentDto> addressComponents;
		public Place.PlusCode plusCode;
		public Place.OpeningHours openingHours;
		public int userRatingsTotal;
		public int UTCOffsetMinutes;
	}
}