using JetBrains.Annotations;
using MobilePlacesSdk.Scripts.Internal.Dto;

namespace NinevaStudios.Places
{
	/// <summary>
	/// A <see cref="Place"/> and the relative likelihood of the place being the best match
	/// within the list of returned places for a single request.
	/// </summary>
	[PublicAPI]
	public class PlaceLikelihood
	{
		/// <summary>
		/// A value indicating the degree of confidence that the device is at the corresponding <see cref="Place"/>
		/// between 0 and 1.
		/// </summary>
		public float likelihood;
		/// <summary>
		/// The <see cref="Place"/> associated with this PlaceLikelihood.
		/// </summary>
		public Place place;

		internal PlaceLikelihood(PlaceLikelihoodDto dto)
		{
			likelihood = dto.likelihood;
			place = new Place(dto.place);
		}
	}
}