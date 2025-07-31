/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System.IO;
using InfinityCode.uPano.Renderers;
using InfinityCode.uPano.Renderers.Base;
using InfinityCode.uPano.Requests;
using InfinityCode.uPano.Services;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.uPano.Editors.Services
{
    [CustomEditor(typeof(GoogleStreetView))]
    public class GoogleStreetViewEditor : SerializedEditor
    {
        private SerializedProperty apiKey;
        private SerializedProperty directions;
        private SerializedProperty directionPrefab;
        private SerializedProperty loadType;
        private SerializedProperty panoID;
        private SerializedProperty locationLng;
        private SerializedProperty locationLat;
        private SerializedProperty zoom;
        private SerializedProperty progressive;
        private SerializedProperty registerAs;

        private GoogleStreetView streetView;
        private bool wrongPanoRenderer;


        protected override void CacheSerializedFields()
        {
            apiKey = serializedObject.FindProperty("apiKey");
            loadType = serializedObject.FindProperty("loadType");
            panoID = serializedObject.FindProperty("panoID");
            locationLng = serializedObject.FindProperty("locationLng");
            locationLat = serializedObject.FindProperty("locationLat");
            zoom = serializedObject.FindProperty("zoom");
            progressive = serializedObject.FindProperty("progressive");
            directions = serializedObject.FindProperty("directions");
            directionPrefab = serializedObject.FindProperty("directionPrefab");
            registerAs = serializedObject.FindProperty("registerAs");
        }

        private void FixPanoRenderer()
        {
            DestroyImmediate(streetView.GetComponent<PanoRenderer>());
            streetView.gameObject.AddComponent<SphericalPanoRenderer>();
            EditorApplication.update -= FixPanoRenderer;
        }

        protected override void OnEnable()
        {
            streetView = target as GoogleStreetView;

            PanoRenderer panoRenderer = streetView.GetComponent<PanoRenderer>();
            wrongPanoRenderer = !(panoRenderer is SphericalPanoRenderer);

            base.OnEnable();
        }

        protected override void OnGUI()
        {
            if (wrongPanoRenderer)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                EditorGUILayout.HelpBox("Wrong Pano Renderer.\nThis should be a Spherical Pano Renderer.", MessageType.Error);
                if (GUILayout.Button("Fix"))
                {
                    EditorApplication.update += FixPanoRenderer;
                }
                EditorGUILayout.EndVertical();
            }

            PropertyField(apiKey);
            PropertyField(registerAs);
            PropertyField(loadType);
            if (loadType.enumValueIndex == (int)GoogleStreetView.LoadType.id)
            {
                PropertyField(panoID);
            }
            else
            {
                PropertyField(locationLng);
                PropertyField(locationLat);
            }

            PropertyField(zoom);
            PropertyField(progressive);
            PropertyField(directions, "Directions (Experimental)");
            if (directions.boolValue)
            {
                EditorGUI.indentLevel++;
                PropertyField(directionPrefab, "Prefab");
                EditorGUI.indentLevel--;
            }

            if (GUILayout.Button("Download"))
            {
                GoogleStreetViewRequest request;
                if (loadType.enumValueIndex == (int)GoogleStreetView.LoadType.id)
                {
                    request = GoogleStreetView.DownloadByID(apiKey.stringValue, panoID.stringValue, zoom.intValue);
                }
                else
                {
                    request = GoogleStreetView.DownloadByLocation(apiKey.stringValue, locationLng.doubleValue, locationLat.doubleValue, zoom.intValue);
                }
                request.OnComplete += OnRequestComplete;
            }
        }

        private void OnRequestComplete(GoogleStreetViewRequest request)
        {
            if (request.hasErrors) Debug.LogError(request.error);
            else
            {
                string filename = EditorUtility.SaveFilePanelInProject("Google Street View", "GoogleStreetView.png", "png", "Please enter a file name to save the texture to");
                if (string.IsNullOrEmpty(filename)) return;

                File.WriteAllBytes(filename, request.texture.EncodeToPNG());
                AssetDatabase.Refresh();

                if (!wrongPanoRenderer && EditorUtility.DisplayDialog("Set the panorama?", "Set the panorama to Spherical Pano Renderer?", "Set", "Cancel"))
                {
                    SphericalPanoRenderer panoRenderer = streetView.GetComponent<SphericalPanoRenderer>();
                    if (panoRenderer != null)
                    {
                        panoRenderer.texture = AssetDatabase.LoadAssetAtPath<Texture2D>(filename);
                    }
                }
            }
        }
    }
}