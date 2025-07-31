using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using MobilePlacesSdk.Scripts.Internal.Dto;

namespace NinevaStudios.Places
{
	/// <summary>
	/// Use the values of Place.Field to specify which place data types to return.
	/// </summary>
	[PublicAPI]
	public class Place
	{
		/// <summary>
		/// <see cref="Place.BusinessStatus"/> for this Place.
		/// </summary>
		public BusinessStatus businessStatus;
		/// <summary>
		/// The name of this Place.
		/// </summary>
		public string name;
		/// <summary>
		/// The unique ID of this Place.
		/// </summary>
		public string placeID;
		/// <summary>
		/// The place's phone number in international format.
		/// </summary>
		public string phoneNumber;
		/// <summary>
		/// A human-readable address for this Place.
		/// </summary>
		public string formattedAddress;
		/// <summary>
		/// The place's rating, from 1 to 5, based on aggregated user reviews.
		/// </summary>
		public float rating;
		/// <summary>
		/// Returns the price level for this place on a scale from <see cref="PriceLevel.Cheap"/> to <see cref="PriceLevel.Expensive"/>.
		/// </summary>
		public PriceLevel priceLevel;
		/// <summary>
		/// A list of place types for this Place.
		/// </summary>
		public List<Type> types;
		/// <summary>
		/// The location of this Place.
		/// </summary>
		public LatLng coordinate;
		/// <summary>
		/// The URL of the website of this Place.
		/// </summary>
		public string website;
		/// <summary>
		/// The attributions that must be shown to the user, if data from the Place is used.
		/// </summary>
		public string attributions;
		/// <summary>
		/// A viewport for displaying this Place.
		/// </summary>
		public LatLngBounds viewport;
		/// <summary>
		/// The address components for this Place's location.
		/// </summary>
		public List<AddressComponent> addressComponents;
		/// <summary>
		/// The <see cref="PlusCode"/> location of this Place.
		/// </summary>
		public PlusCode plusCode;
		/// <summary>
		/// The <see cref="OpeningHours"/> of this Place.
		/// </summary>
		public OpeningHours openingHours;
		/// <summary>
		/// The total number of user ratings of this Place.
		/// </summary>
		public int userRatingsTotal;
		/// <summary>
		/// The number of minutes this place’s current timezone is offset from UTC.
		/// </summary>
		public int UTCOffsetMinutes;

		/// <summary>
		/// <see cref="Place"/> data fields to be requested in the <see cref="Places.FindCurrentPlace"/> and <see cref="Places.FetchPlace"/> calls.
		/// </summary>
		[PublicAPI]
		public enum Field
		{
			Address = 0,
			AddressComponents = 1,
			BusinessStatus = 2,
			Id = 3,
			LatLng = 4,
			Name = 5,
			OpeningHours = 6,
			PhoneNumber = 7,
			PlusCode = 9,
			PriceLevel = 10,
			Rating = 11,
			Types = 12,
			UserRatingsTotal = 13,
			UtcOffset = 14,
			Viewport = 15,
			WebsiteUri = 16,
		}

		/// <summary>
		/// Type of a <see cref="Place"/>.
		/// </summary>
		[PublicAPI]
		public enum Type
		{
			Other = 0,
			Accounting,
			AdministrativeAreaLevel1,
			AdministrativeAreaLevel2,
			AdministrativeAreaLevel3,
			AdministrativeAreaLevel4,
			AdministrativeAreaLevel5,
			Airport,
			AmusementPark,
			Aquarium,
			Archipelago,
			ArtGallery,
			Atm,
			Bakery,
			Bank,
			Bar,
			BeautySalon,
			BicycleStore,
			BookStore,
			BowlingAlley,
			BusStation,
			Cafe,
			Campground,
			CarDealer,
			CarRental,
			CarRepair,
			CarWash,
			Casino,
			Cemetery,
			Church,
			CityHall,
			ClothingStore,
			ColloquialArea,
			Continent,
			ConvenienceStore,
			Country,
			Courthouse,
			Dentist,
			DepartmentStore,
			Doctor,
			Drugstore,
			Electrician,
			ElectronicsStore,
			Embassy,
			Establishment,
			Finance,
			FireStation,
			Floor,
			Florist,
			Food,
			FuneralHome,
			FurnitureStore,
			GasStation,
			GeneralContractor,
			Geocode,
			GroceryOrSupermarket,
			Gym,
			HairCare,
			HardwareStore,
			Health,
			HinduTemple,
			HomeGoodsStore,
			Hospital,
			InsuranceAgency,
			Intersection,
			JewelryStore,
			Laundry,
			Lawyer,
			Library,
			LightRailStation,
			LiquorStore,
			Locality,
			LocalGovernmentOffice,
			Locksmith,
			Lodging,
			MealDelivery,
			MealTakeaway,
			Mosque,
			MovieRental,
			MovieTheater,
			MovingCompany,
			Museum,
			NaturalFeature,
			Neighborhood,
			NightClub,
			Painter,
			Park,
			Parking,
			PetStore,
			Pharmacy,
			Physiotherapist,
			PlaceOfWorship,
			Plumber,
			PlusCode,
			PointOfInterest,
			Police,
			Political,
			PostalCode,
			PostalCodePrefix,
			PostalCodeSuffix,
			PostalTown,
			PostBox,
			PostOffice,
			Premise,
			PrimarySchool,
			RealEstateAgency,
			Restaurant,
			RoofingContractor,
			Room,
			Route,
			RvPark,
			School,
			SecondarySchool,
			ShoeStore,
			ShoppingMall,
			Spa,
			Stadium,
			Storage,
			Store,
			StreetAddress,
			StreetNumber,
			Sublocality,
			SublocalityLevel1,
			SublocalityLevel2,
			SublocalityLevel3,
			SublocalityLevel4,
			SublocalityLevel5,
			Subpremise,
			SubwayStation,
			Supermarket,
			Synagogue,
			TaxiStand,
			TouristAttraction,
			TownSquare,
			TrainStation,
			TransitStation,
			TravelAgency,
			University,
			VeterinaryCare,
			Zoo
		}

		[PublicAPI]
		public enum BusinessStatus
		{
			/** The business status is not known. */
			Unknown,

			/** The business is operational. */
			Operational,

			/** The business is closed temporarily. */
			ClosedTemporarily,

			/** The business is closed permanently. */
			ClosedPermanently
		}

		[PublicAPI]
		public enum PriceLevel
		{
			Unknown = -1,
			Free = 0,
			Cheap = 1,
			Medium = 2,
			High = 3,
			Expensive = 4,
		}

		[PublicAPI]
		[Serializable]
		public struct LatLng
		{
			public float latitude;
			public float longitude;

			public LatLng(float lat, float lng)
			{
				latitude = lat;
				longitude = lng;
			}
		}

		[PublicAPI]
		[Serializable]
		public struct LatLngBounds
		{
			public LatLng southWest;
			public LatLng northEast;

			public LatLngBounds(LatLng sw, LatLng ne)
			{
				southWest = sw;
				northEast = ne;
			}
		}

		[PublicAPI]
		[Serializable]
		public struct AddressComponent
		{
			public string name;
			public List<Type> types;

			public AddressComponent(AddressComponentDto dto)
			{
				name = dto.name;
				types = new List<Type>();
				foreach (var type in dto.types)
				{
					types.Add((Type) type);
				}
			}
		}
		
		[Serializable]
		public struct AddressComponentDto
		{
			public string name;
			public List<int> types;
		}

		[PublicAPI]
		[Serializable]
		public struct PlusCode
		{
			public string globalCode;
			public string compoundCode;
		}

		[PublicAPI]
		[Serializable]
		public struct OpeningHours
		{
			public List<string> weekdayText;
			public List<OpeningHoursPeriod> periods;
		}

		[PublicAPI]
		[Serializable]
		public struct OpeningHoursPeriod
		{
			public OpeningHoursPeriodEvent openEvent;
			public OpeningHoursPeriodEvent closeEvent;
		}

		[PublicAPI]
		[Serializable]
		public struct OpeningHoursPeriodEvent
		{
			public int day;
			public int hour;
			public int minute;
		}

		internal Place(PlaceDto dto)
		{
			businessStatus = (BusinessStatus) dto.businessStatus;
			name = dto.name;
			placeID = dto.placeID;
			phoneNumber = dto.phoneNumber;
			formattedAddress = dto.formattedAddress;
			rating = dto.rating;
			priceLevel = (PriceLevel) dto.priceLevel;
			types = dto.types.Select(typeInt => (Type) typeInt).ToList();
			coordinate = dto.coordinate;
			website = dto.website;
			attributions = dto.attributions;
			viewport = dto.viewport;
			
			var components = new List<AddressComponent>();
			foreach (var component in dto.addressComponents)
			{
				components.Add(new AddressComponent(component));
			}
			addressComponents = components;
			
			plusCode = dto.plusCode;
			openingHours = dto.openingHours;
			userRatingsTotal = dto.userRatingsTotal;
			UTCOffsetMinutes = dto.UTCOffsetMinutes;
		}

		public override string ToString() => $"Business status: {businessStatus}, name: {name}, place ID: {placeID}, phone number: {phoneNumber}, formatted address: {formattedAddress}, rating: {rating}, price level: {priceLevel}, types: {string.Join(",", types)}, coordinate: {$"{coordinate.latitude}|{coordinate.longitude}"}, website: {website}, attributions: {attributions}, plus code: {plusCode.globalCode}, user ratings total: {userRatingsTotal}";
	}
}