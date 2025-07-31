using JetBrains.Annotations;

namespace NinevaStudios.Places
{
	/// <summary>
	/// Filter to restrict the result set of autocomplete predictions to certain types.
	/// </summary>
	[PublicAPI]
	public enum TypeFilter
	{
		/// <summary>
		/// Only return geocoding results with a precise address.
		/// </summary>
		None = 0,

		/// <summary>
		/// Only return geocoding results with a precise address.
		/// </summary>
		Address = 2,

		/// <summary>
		/// Return any result matching the following place types: Locality or Administrative Area Level 3
		/// </summary>
		Cities = 5,

		/// <summary>
		/// Only return results that are classified as businesses.
		/// </summary>
		Establishment = 34,

		/// <summary>
		/// Only return geocoding results, rather than business results. For example, parks, cities and street addresses.
		/// </summary>
		Geocode = 3,

		/// <summary>
		/// Return any result matching the following place types: Locality, Sub-locality, Postal Code, Country, Administrative Area Level 1 and 2
		/// </summary>
		Regions = 4
	}
}
