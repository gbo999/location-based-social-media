using System.Collections.Generic;
using JetBrains.Annotations;
using NinevaStudios.Places;
using NinevaStudios.Places.Internal.Dto;
using UnityEngine;

/// <summary>
/// Options for the autocomplete screen
/// </summary>
[PublicAPI]
public class AutocompleteOptions
{
	public string Json { get; }

	AutocompleteOptions(string json)
	{
		Json = json;
	}

	/// <summary>
	/// Builder fore options for the autocomplete screen
	/// </summary>
	public class Builder
	{
		readonly AutocompleteMode _autocompleteMode;
		readonly List<Place.Field> _placeFields;
		string _hint;
		string _initialQuery;
		Place.LatLngBounds _locationBias;
		Place.LatLngBounds _locationRestriction;
		TypeFilter? _typeFilter;
		List<string> _countries;

		/// <summary>
		/// Create an instance of the AutocompleteOptions Builder.
		/// </summary>
		/// <param name="autocompleteMode">Autocomplete mode</param>
		/// <param name="placeFields">The fields of the place to be requested. See Place.Field for the specifiable fields. Read more: https://developers.google.com/places/android-sdk/place-data-fields</param>
		public Builder(AutocompleteMode autocompleteMode, List<Place.Field> placeFields)
		{
			_autocompleteMode = autocompleteMode;
			_placeFields = placeFields;
		}

		/// <summary>
		/// Sets countries to restrict results to.
		/// </summary>
		/// <param name="countries">This must be a list of ISO 3166-1 Alpha-2 country codes (case insensitive). If no countries are set, no country filtering will take place.</param>
		/// <returns>Builder instance</returns>
		public Builder SetCountries(List<string> countries)
		{
			_countries = countries;
			return this;
		}

		/// <summary>
		/// Sets country to restrict results to.
		/// </summary>
		/// <param name="country">This must be a ISO 3166-1 Alpha-2 country code (case insensitive). If no countries are set, no country filtering will take place.</param>
		/// <returns>Builder instance</returns>
		public Builder SetCountry(string country)
		{
			_countries = new List<string> {country};
			return this;
		}

		/// <summary>
		/// Sets the hint text to display in the search input field when there is no text entered.
		/// </summary>
		/// <param name="hint">text to display in the search input field when there is no text entered.</param>
		/// <returns>Builder instance</returns>
		public Builder SetHint(string hint)
		{
			_hint = hint;
			return this;
		}

		/// <summary>
		/// Sets the initial query in the search input.
		/// </summary>
		/// <param name="initialQuery">The initial query in the search input.</param>
		/// <returns>Builder instance</returns>
		public Builder SetInitialQuery(string initialQuery)
		{
			_initialQuery = initialQuery;
			return this;
		}
		
		/// <summary>
		/// Sets the location bias applied for autocomplete predictions.
		/// Note: the autocomplete predictions will not necessarily be within the location specified.
		/// To enforce a restriction, use <see cref="SetLocationRestriction"/>.
		/// You may not set both LocationBias and LocationRestriction simultaneously.
		/// </summary>
		/// <returns>Builder instance</returns>
		public Builder SetLocationBias(Place.LatLngBounds locationBias)
		{
			_locationBias = locationBias;
			return this;
		}

		/// <summary>
		/// Sets the location restriction applied for autocomplete predictions.
		/// To impose a location bias instead, use <see cref="SetLocationBias"/>.
		/// You may not set both LocationBias and LocationRestriction simultaneously.
		/// </summary>
		/// <returns>Builder instance</returns>
		public Builder SetLocationRestriction(Place.LatLngBounds locationRestriction)
		{
			_locationRestriction = locationRestriction;
			return this;
		}

		/// <summary>
		/// Sets the filter that restricts the type of the results included in the response.
		/// </summary>
		/// <param name="typeFilter"> One of the <see cref="TypeFilter"/> constants. </param>
		/// <returns>Builder instance</returns>
		public Builder SetTypeFilter(TypeFilter typeFilter)
		{
			_typeFilter = typeFilter;
			return this;
		}

		/// <summary>
		/// Builds all fields to an instance of <see cref="AutocompleteOptions"/>.
		/// </summary>
		/// <returns></returns>
		public AutocompleteOptions Build() => new AutocompleteOptions(JsonUtility.ToJson(new PlacesAutocompleteOptionsDto
			{
				AutocompleteMode = _autocompleteMode,
				PlaceFields = _placeFields,
				Countries = _countries,
				Hint = _hint,
				InitialQuery = _initialQuery,
				LocationBias = _locationBias,
				LocationRestriction = _locationRestriction,
				TypeFilter = _typeFilter
			}));
	}
}