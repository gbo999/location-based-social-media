using System;
using NinevaStudios.Places.Internal;
using NinevaStudios.Places.JniToolkit;
using UnityEngine;

#if UNITY_IOS
using System.Runtime.InteropServices;
#endif

public class PlacePhotoMetadata
{
	public AndroidJavaObject JavaMetadata { get; }
	public IntPtr IosMetadata { get; }

	public PlacePhotoMetadata(AndroidJavaObject ajo)
	{
		JavaMetadata = ajo;
	}

	public PlacePhotoMetadata(IntPtr ptr)
	{
		IosMetadata = ptr;
	}

	/// <summary>
	/// Returns the attributions that must be shown to the user if this photo is displayed.
	/// </summary>
	public string Attributions
	{
		get
		{
			var result = "";

			if (PlacesUtils.IsAndroid && JavaMetadata != null)
			{
				result = JavaMetadata.CallStr("getAttributions");
			}

#if UNITY_IOS
			result = _getPhotoMetadataAttributions(IosMetadata);
#endif

			return result;
		}
	}

	/// <summary>
	/// Returns the maximum height in which this photo is available.
	/// </summary>
	public int Height
	{
		get
		{
			var result = 0;
			
			if (PlacesUtils.IsAndroid && JavaMetadata != null)
			{
				result = JavaMetadata.CallInt("getHeight");
			}

#if UNITY_IOS
			result = _getPhotoMetadataHeight(IosMetadata);
#endif
			return result;
		}
	}

	/// <summary>
	/// Returns the maximum width in which this photo is available.
	/// </summary>
	public int Width
	{
		get
		{
			var result = 0;
			
			if (PlacesUtils.IsAndroid && JavaMetadata != null)
			{
				result = JavaMetadata.CallInt("getWidth");
			}

#if UNITY_IOS
			result = _getPhotoMetadataWidth(IosMetadata);
#endif
			return result;
		}
	}

	~PlacePhotoMetadata()
	{
#if UNITY_IOS
		_releasePhotoMetadata(IosMetadata);
#endif
	}

#if UNITY_IOS
	[DllImport("__Internal")]
	static extern void _releasePhotoMetadata(IntPtr metadataPointer);
	
	
	[DllImport("__Internal")]
	static extern int _getPhotoMetadataWidth(IntPtr metadataPointer);

	[DllImport("__Internal")]
	static extern int _getPhotoMetadataHeight(IntPtr metadataPointer);

	[DllImport("__Internal")]
	static extern string _getPhotoMetadataAttributions(IntPtr metadataPointer);
#endif
}