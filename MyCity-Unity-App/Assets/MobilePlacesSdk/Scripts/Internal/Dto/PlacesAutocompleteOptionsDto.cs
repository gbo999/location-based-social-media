using System;
using System.Collections.Generic;

namespace NinevaStudios.Places.Internal.Dto
{
	[Serializable]
	public class PlacesAutocompleteOptionsDto
	{
		public AutocompleteMode AutocompleteMode;
		public List<Place.Field> PlaceFields;
		public List<string> Countries;
		public string Hint;
		public string InitialQuery;
		public TypeFilter? TypeFilter;
		public Place.LatLngBounds LocationBias;
		public Place.LatLngBounds LocationRestriction;
	}
}