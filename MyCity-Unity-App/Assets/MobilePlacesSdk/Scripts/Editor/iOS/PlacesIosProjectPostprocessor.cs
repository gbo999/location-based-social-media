using System;

#if UNITY_IOS
#pragma warning disable 0162

namespace NinevaStudios.Places.Internal
{
	using Internal;
	using System.IO;
	using System.Linq;
	using UnityEditor;
	using UnityEditor.Callbacks;
	using UnityEditor.iOS.Xcode;
	using UnityEngine;

	/// <summary>
	/// See https://developers.google.com/places/ios-sdk/start#install-manually
	/// </summary>
	public static class GoogleMapsIosProjectPostprocessor
	{
		[PostProcessBuild(2000)]
		public static void OnPostProcessBuild(BuildTarget target, string path)
		{
			Debug.Log("Places Mobile SDK is now postprocessing iOS Project");
			var projectPath = PBXProject.GetPBXProjectPath(path);

			var project = new PBXProject();
			project.ReadFromFile(projectPath);

#if UNITY_2019_3_OR_NEWER
			var targetGuid = project.GetUnityFrameworkTargetGuid();
#else
			var targetName = PBXProject.GetUnityTargetName();
			var targetGuid = project.TargetGuidByName(targetName);
#endif

			AddFrameworks(project, targetGuid);

			var linkedResourcePath = GetLinkedResourcePath("GooglePlaces.bundle", path);
			AddPlacesBundleToProjectResources(project, targetGuid, linkedResourcePath, "GooglePlaces.bundle");

			File.WriteAllText(projectPath, project.WriteToString());
			
			ModifyPlist(path, AddLocationPrivacyEntry);

			Debug.Log("Places Mobile SDK has finished postprocessing iOS Project");
		}

		static string GetLinkedResourcePath(string bundle, string path)
		{
			const string frameworksFolderPath = "Frameworks";
			var files = Directory.GetDirectories(Path.Combine(path, frameworksFolderPath), bundle, SearchOption.AllDirectories);

			if (files.Any())
			{
				var linkedResourcePathAbsolute = files.First();
				var relativePath = linkedResourcePathAbsolute.Substring(linkedResourcePathAbsolute.IndexOf(frameworksFolderPath));
				return relativePath;
			}

			Debug.Log("Failed to find " + bundle);
			return string.Empty;
		}

		static void AddPlacesBundleToProjectResources(PBXProject project, string targetGUID, string sourcePath, string destPath)
		{
			var addFolderReference = project.AddFolderReference(sourcePath, destPath, PBXSourceTree.Source);
			project.AddFileToBuild(targetGUID, addFolderReference);
		}

		static void AddFrameworks(PBXProject project, string targetGuid)
		{
			project.AddFrameworkToProject(targetGuid, "libc++.tbd", false);
			project.AddFrameworkToProject(targetGuid, "libz.tbd", false);
			project.AddFrameworkToProject(targetGuid, "Accelerate.framework", false);
			project.AddFrameworkToProject(targetGuid, "CoreData.framework", false);
			project.AddFrameworkToProject(targetGuid, "CoreImage.framework", false);
			project.AddFrameworkToProject(targetGuid, "CoreGraphics.framework", false);
			project.AddFrameworkToProject(targetGuid, "CoreLocation.framework", false);
			project.AddFrameworkToProject(targetGuid, "CoreTelephony.framework", false);
			project.AddFrameworkToProject(targetGuid, "CoreText.framework", false);
			project.AddFrameworkToProject(targetGuid, "GLKit.framework", false);
			project.AddFrameworkToProject(targetGuid, "ImageIO.framework", false);
			project.AddFrameworkToProject(targetGuid, "OpenGLES.framework", false);
			project.AddFrameworkToProject(targetGuid, "QuartzCore.framework", false);
			project.AddFrameworkToProject(targetGuid, "SystemConfiguration.framework", false);
			project.AddFrameworkToProject(targetGuid, "UIKit.framework", false);

			// Add `-ObjC` to "Other Linker Flags".
			project.AddBuildProperty(targetGuid, "OTHER_LDFLAGS", "-ObjC");
		}
		
		static bool HasRootElement(this PlistDocument plist, string key)
		{
			return plist.root.values.ContainsKey(key);
		}
		
		static void AddLocationPrivacyEntry(PlistDocument plist)
		{
			const string NSLocationWhenInUseUsageDescription = "NSLocationWhenInUseUsageDescription";

			if (!plist.HasRootElement(NSLocationWhenInUseUsageDescription))
			{
				plist.root.AsDict().SetString(NSLocationWhenInUseUsageDescription, "Places SDK requires location permission to function properly.");
			}
		}
		
		static void ModifyPlist(string projectPath, Action<PlistDocument> modifier)
		{
			try
			{
				var plistInfoFile = new PlistDocument();

				var infoPlistPath = Path.Combine(projectPath, "Info.plist");
				plistInfoFile.ReadFromString(File.ReadAllText(infoPlistPath));

				modifier(plistInfoFile);

				File.WriteAllText(infoPlistPath, plistInfoFile.WriteToString());
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}
	}
}

#pragma warning restore 0162
#endif