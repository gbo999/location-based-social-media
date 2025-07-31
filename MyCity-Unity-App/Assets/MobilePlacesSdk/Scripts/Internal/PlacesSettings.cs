using System.IO;
using UnityEditor;
using UnityEngine;

namespace NinevaStudios.Places.Internal
{
	public class PlacesSettings : ScriptableObject
	{
		public const string IOS_KEY_PLACEHOLDER = "AIzaSyC5nLZEQB4CFyx_0BhEZFsLg67TeA6vpm4";
		public const string ANDROID_KEY_PLACEHOLDER = "AIzaSyCKo5_MdP3liUmFSCWhq--SsbFvRg8VbJ8";

		const string SettingsAssetName = "PlacesSettings";
		const string SettingsAssetPath = "Resources/";

		static PlacesSettings _instance;

		[SerializeField] string _apiKeyIos = IOS_KEY_PLACEHOLDER;
		[SerializeField] string _apiKeyAndroid = ANDROID_KEY_PLACEHOLDER;

		public static string IosApiKey
		{
			get => Instance._apiKeyIos;
			set
			{
				Instance._apiKeyIos = value;
				MarkAssetDirty();
			}
		}

		public static string AndroidApiKey
		{
			get => Instance._apiKeyAndroid;
			set
			{
				Instance._apiKeyAndroid = value;
				MarkAssetDirty();
			}
		}

		public static PlacesSettings Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = Resources.Load(SettingsAssetName) as PlacesSettings;
					if (_instance == null)
					{
						_instance = CreateInstance<PlacesSettings>();

						SaveAsset(Path.Combine(GetPluginPath(), SettingsAssetPath), SettingsAssetName);
					}
				}

				return _instance;
			}
		}

		static string GetPluginPath()
		{
			var absolutePluginPath = GetAbsolutePluginPath();
			Debug.Log(absolutePluginPath);
			return absolutePluginPath.Replace("\\", "/").Replace(Application.dataPath, "Assets");
		}

		static string GetAbsolutePluginPath()
		{
			return Path.GetDirectoryName(Path.GetDirectoryName(FindEditor(Application.dataPath)));
		}

		static string FindEditor(string path)
		{
			foreach (var d in Directory.GetDirectories(path))
			{
				foreach (var f in Directory.GetFiles(d))
				{
					if (f.Contains("PlacesSettingsEditor.cs"))
					{
						return f;
					}
				}

				var rec = FindEditor(d);
				if (rec != null)
				{
					return rec;
				}
			}

			return null;
		}

		static void SaveAsset(string directory, string name)
		{
#if UNITY_EDITOR
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			AssetDatabase.CreateAsset(Instance, directory + name + ".asset");
			AssetDatabase.Refresh();
#endif
		}

		static void MarkAssetDirty()
		{
#if UNITY_EDITOR
			EditorUtility.SetDirty(Instance);
#endif
		}
	}
}