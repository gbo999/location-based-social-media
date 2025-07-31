using NinevaStudios.Places.Internal;
using UnityEditor;
using UnityEngine;

namespace MobilePlacesSdk.Scripts.Editor
{
	[CustomEditor(typeof(PlacesSettings))]
	public class PlacesSettingsEditor : UnityEditor.Editor
	{
		const string ApiKeyTooltip = "You must obtain an API key from Google in order to use Google Places API";
		
		[MenuItem("Window/Mobile Places SDK/Edit Settings", false, 1000)]
		public static void Edit()
		{
			Selection.activeObject = PlacesSettings.Instance;
		}

		public override void OnInspectorGUI()
		{
			using (new EditorGUILayout.VerticalScope("box"))
			{
				GUILayout.Label("Android Settings", EditorStyles.boldLabel);
				var androidApiKey = EditorGUILayout.TextField(new GUIContent("Android API Key [?]", ApiKeyTooltip), PlacesSettings.AndroidApiKey);
				CheckApiKey(androidApiKey, PlacesSettings.ANDROID_KEY_PLACEHOLDER);

				EditorGUILayout.Space();
				if (GUILayout.Button("Read how to get and setup Android API key"))
				{
					Application.OpenURL("https://docs.ninevastudios.com/#/unity-plugins/places-sdk?id=setup-android");
				}
			}

			EditorGUILayout.Space();

			using (new EditorGUILayout.VerticalScope("box"))
			{
				GUILayout.Label("iOS Settings", EditorStyles.boldLabel);
				var iosApiKey = EditorGUILayout.TextField(new GUIContent("iOS API Key [?]", ApiKeyTooltip), PlacesSettings.IosApiKey);
				CheckApiKey(iosApiKey, PlacesSettings.IOS_KEY_PLACEHOLDER);
				PlacesSettings.IosApiKey = iosApiKey;

				EditorGUILayout.Space();
				if (GUILayout.Button("Read how to get and setup iOS API key"))
				{
					Application.OpenURL("https://docs.ninevastudios.com/#/unity-plugins/places-sdk?id=setup-ios");
				}
			}

			EditorGUILayout.Space();

			EditorGUILayout.HelpBox(
				"If the places search closes right away when running on the device it means that the API key is misconfigured. Please go through the setup instructions in the documentation and double-check all the steps.",
				MessageType.Warning);

			using (new EditorGUILayout.HorizontalScope())
			{
				if (GUILayout.Button("Android Setup"))
				{
					Application.OpenURL("https://docs.ninevastudios.com/#/unity-plugins/places-sdk?id=setup-android");
				}

				if (GUILayout.Button("iOS Setup"))
				{
					Application.OpenURL("https://docs.ninevastudios.com/#/unity-plugins/places-sdk?id=setup-ios");
				}
			}

			EditorGUILayout.Space();

			using (new EditorGUILayout.HorizontalScope("box"))
			{
				if (GUILayout.Button("Read Documentation"))
				{
					Application.OpenURL("https://docs.ninevastudios.com/#/unity-plugins/places-sdk");
				}

				if (GUILayout.Button("Ask us anything on Discord"))
				{
					Application.OpenURL("https://bit.ly/nineva_support_discord");
				}
			}
		}

		static void CheckApiKey(string key, string placeholder)
		{
			if (key == placeholder)
			{
				EditorGUILayout.HelpBox(
					"This is a placeholder API key! Please go through the setup section in the docs to setup your own.",
					MessageType.Error);
			}
		}
	}
}