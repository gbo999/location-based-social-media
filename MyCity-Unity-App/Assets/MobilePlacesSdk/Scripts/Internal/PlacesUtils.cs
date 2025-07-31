using System.Collections.Generic;
using AOT;
using NinevaStudios.Places.JniToolkit;

namespace NinevaStudios.Places.Internal
{
	using System;
	using System.IO;
	using System.Runtime.InteropServices;
	using UnityEngine;

	public static class PlacesUtils
	{
		public static bool IsAndroid => Application.platform == RuntimePlatform.Android;

		public static bool IsIos => Application.platform == RuntimePlatform.IPhonePlayer;

		public static bool IsNotAndroid => !IsAndroid;

		public static bool IsNotIosRuntime => !IsIos;

		public static bool IsPlatformSupported => IsAndroid || IsIos;

		public static bool IsPlatformNotSupported => !IsPlatformSupported;

		public static bool IsZero(this IntPtr intPtr)
		{
			return intPtr == IntPtr.Zero;
		}

		public static bool IsNonZero(this IntPtr intPtr)
		{
			return intPtr != IntPtr.Zero;
		}
		
		public static T Cast<T>(this IntPtr instancePtr)
		{
			var instanceHandle = GCHandle.FromIntPtr(instancePtr);
			if (!(instanceHandle.Target is T))
			{
				throw new InvalidCastException("Failed to cast IntPtr");
			}

			var castedTarget = (T) instanceHandle.Target;
			return castedTarget;
		}
		
		public static IntPtr GetPointer(this object obj)
		{
			return obj == null ? IntPtr.Zero : GCHandle.ToIntPtr(GCHandle.Alloc(obj));
		}

		public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
		{
			// Unix timestamp is seconds past epoch
			DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
			return dtDateTime;
		}

		public static string ToFullStreamingAssetsPath(this string fileName)
		{
			if (String.IsNullOrEmpty(fileName))
			{
				return null;
			}

			return Path.Combine(Application.streamingAssetsPath, fileName);
		}
#if UNITY_IOS
		internal delegate void ActionStringCallbackDelegate(IntPtr actionPtr, string data);
		
		[MonoPInvokeCallback(typeof(ActionStringCallbackDelegate))]
		public static void ActionStringCallback(IntPtr actionPtr, string data)
		{
			if (Debug.isDebugBuild)
			{
				Debug.Log("ActionStringCallback: " + data);
			}
			
			if (actionPtr != IntPtr.Zero)
			{
				var action = actionPtr.Cast<Action<string>>();
				action(data);
			}
		}
		
		internal delegate void ActionIntCallbackDelegate(IntPtr actionPtr, int data);
		
		[MonoPInvokeCallback(typeof(ActionIntCallbackDelegate))]
		public static void ActionIntCallback(IntPtr actionPtr, int data)
		{
			if (Debug.isDebugBuild)
			{
				Debug.Log("ActionIntCallbackDelegate: " + data);
			}
			
			if (actionPtr != IntPtr.Zero)
			{
				var action = actionPtr.Cast<Action<int>>();
				action(data);
			}
		}
		
		internal delegate void ActionImageCallbackDelegate(IntPtr actionPtr, IntPtr byteArrPtr, int arrayLength);
		
		[MonoPInvokeCallback(typeof(ActionImageCallbackDelegate))]
		public static void ActionImageCallback(IntPtr actionPtr, IntPtr byteArrPtr, int arrayLength)
		{
			if (Debug.isDebugBuild)
			{
				Debug.Log("Picked img ptr: " + byteArrPtr.ToInt32() + ", array length: " + arrayLength);
			}

			var buffer = new byte[arrayLength];

			Marshal.Copy(byteArrPtr, buffer, 0, arrayLength);
			var tex = new Texture2D(2, 2);
			tex.LoadImage(buffer);

			if (actionPtr != IntPtr.Zero)
			{
				var action = actionPtr.Cast<Action<Texture2D>>();
				action(tex);
			}
		}
		
		internal delegate void ActionVoidCallbackDelegate(IntPtr actionPtr);
		
		[MonoPInvokeCallback(typeof(ActionVoidCallbackDelegate))]
		public static void ActionVoidCallback(IntPtr actionPtr)
		{
			if (Debug.isDebugBuild)
			{
				Debug.Log("ActionVoidCallback");
			}
			
			if (actionPtr != IntPtr.Zero)
			{
				var action = actionPtr.Cast<Action>();
				action();
			}
		}
#endif
		
		public const string LocationPermission = "android.permission.ACCESS_FINE_LOCATION";

		public static void RequestPermission(string permission, Action<string, bool> onPermissionRequested)
		{
			if (!IsAndroid)
			{
				onPermissionRequested.Invoke(permission, true);
				return;
			}
			
			var proxy = new PermissionRequestCallbackProxy(onPermissionRequested);
			C.PlacesBridge.AJCCallStaticOnce("requestPermission", 
				JniToolkitUtils.Activity, permission, proxy);
		}

		public static string ParseFields(IEnumerable<Place.Field> fields)
		{
			var fieldsString = "";
			foreach (var field in fields)
			{
				fieldsString += string.IsNullOrEmpty(fieldsString) ? ((int) field).ToString() : "," + (int) field;
			}

			return fieldsString;
		}
		
		public static Texture2D Texture2DFromBitmap(AndroidJavaObject bitmapAjo)
		{
			var compressFormatPng = new AndroidJavaClass(C.AndroidGraphicsBitmapCompressFormat).GetStatic<AndroidJavaObject>("PNG");
			var outputStream = new AndroidJavaObject(C.JavaIoBytearrayOutputStream);
			bitmapAjo.CallBool("compress", compressFormatPng, 100, outputStream);
			var buffer = outputStream.Call<byte[]>("toByteArray");

			var tex = new Texture2D(2, 2);
			tex.LoadImage(buffer);
			return tex;
		}
	}
}