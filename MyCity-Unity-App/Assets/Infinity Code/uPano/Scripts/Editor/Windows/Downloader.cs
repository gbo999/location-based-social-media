/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using InfinityCode.uPano.Editors.Utils;
using InfinityCode.uPano.Json;
using InfinityCode.uPano.Requests;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace InfinityCode.uPano.Editors.Windows
{
    public class Downloader : EditorWindow
    {
        private string url;

        private enum Providers
        {
            Unknown,
            GoogleStreetView,
            Yandex
        }

        private Providers provider = Providers.Unknown;
        private bool adjustHeight = true;
        private Color emptyColor = Color.gray;
        private int maxTextureSize = 4096;
        private bool generateMipMaps = false;
        private int zoom = 4;
        private bool pleaseWaitMessage;
        private string yandexApiKey;

        private void DetectPanoProvider()
        {
            if (DetectGSV()) provider = Providers.GoogleStreetView;
            else if (DetectYandex()) provider = Providers.Yandex;
            else provider = Providers.Unknown;
        }

        private bool DetectYandex()
        {
            if (url.ToLower().Contains("maps.yandex")) return true;
            if (url.ToLower().Contains("yandex.ru/maps")) return true;
            if (url.ToLower().Contains("yandex.com/maps")) return true;
            return false;
        }

        private bool DetectGSV()
        {
            if (!url.ToLower().Contains("google.com/maps")) return false;

            string pid = GetGSVPanoID();
            if (pid == null) return false;
            string key = GetGSVKey();
            return key != null;
        }

        private string GetGSVKey()
        {
            Match match = Regex.Match(url, @"@([\d.]+),([-\d.]+)");
            if (match.Success) return match.Groups[1].Value + "x" + match.Groups[2].Value;

            match = Regex.Match(url, @"cbll=([\d.]+),([-\d.]+)");
            if (match.Success) return match.Groups[1].Value + "x" + match.Groups[2].Value;

            return null;
        }

        private string GetGSVPanoID()
        {
            Match match = Regex.Match(url, @"data=.*!1s([\w_-]*?)!2e");
            if (match.Success) return match.Groups[1].Value;

            match = Regex.Match(url, @"panoid=(\w+)");
            if (match.Success) return match.Groups[1].Value;

            return null;
        }

        private void OnGoogleStreetViewComplete(GoogleStreetViewRequest request, string key)
        {
            pleaseWaitMessage = false;

            if (request.hasErrors)
            {
                Debug.LogWarning(request.error);
                return;
            }

            SaveTexture(key, request.texture);
        }

        private void OnGUI()
        {
            EditorGUIUtility.labelWidth = 210;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField("URL");
            url = EditorGUILayout.TextArea(url);
            if (EditorGUI.EndChangeCheck()) DetectPanoProvider();

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField("Panorama Provider: ", provider.ToString());
            EditorGUI.EndDisabledGroup();

            if (provider == Providers.Yandex)
            {
                yandexApiKey = EditorGUILayout.TextField("Yandex API key", yandexApiKey);
                adjustHeight = EditorGUILayout.Toggle("Adjust Height (height = width / 2):", adjustHeight);
            }

            if (provider == Providers.GoogleStreetView)
            {
                zoom = EditorGUILayout.IntSlider("Zoom", zoom, 0, 4);
                EditorGUILayout.HelpBox("Zoom of panorama (0-4). Size of side = 512 * 2 ^ zoom.", MessageType.Info);
            }

            emptyColor = EditorGUILayout.ColorField("Empty Color: ", emptyColor);

            maxTextureSize = EditorGUILayout.IntPopup("Max Size:", maxTextureSize,
                new[] { "512", "1024", "2048", "4096", "8192" },
                new[] { 512, 1024, 2048, 4096, 8192 });

            generateMipMaps = EditorGUILayout.Toggle("Generate Mip Maps: ", generateMipMaps);

            EditorGUI.BeginDisabledGroup(provider == Providers.Unknown || pleaseWaitMessage);
            if (GUILayout.Button("Download")) StartDownload();
            EditorGUI.EndDisabledGroup();

            if (pleaseWaitMessage)
            {
                EditorGUILayout.LabelField("Please wait, the panorama is being downloaded ...");
                EditorGUILayout.HelpBox("If the panorama does not download for a long time, help Unity Editor send out coroutines by changing the size of this window.", MessageType.Info);


                Repaint();
            }
        }

        [MenuItem(EditorUtils.MENU_PATH + "Pano Downloader", false, 2)]
        private static void OpenWindow()
        {
            GetWindow<Downloader>(true, "Pano Downloader", true);
        }

        private void StartDownload()
        {
            try
            {
                if (provider == Providers.GoogleStreetView) StartDownloadGoogle();
                else if (provider == Providers.Yandex) StartDownloadYandex();
            }
            catch (Exception)
            {
                throw;
            }

        }

        private void StartDownloadGoogle()
        {
            string pid = GetGSVPanoID();

            if (pid == null)
            {
                Debug.Log("Wrong URL");
                return;
            }

            string key = GetGSVKey();
            if (key == null)
            {
                Debug.Log("Wrong URL");
                return;
            }

            GoogleStreetViewRequest request = new GoogleStreetViewRequest(key, pid, zoom);
            request.OnComplete += r => { OnGoogleStreetViewComplete(r, key); };

            pleaseWaitMessage = true;
        }

        private void StartDownloadYandex()
        {
            string s = UnityWebRequest.UnEscapeURL(url);

            Match match = Regex.Match(s, @"panorama\[point\]=([\d\.,]+)");
            if (!match.Success) return;

            string ll = match.Groups[1].Value;

            WebClient client = new WebClient();
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            byte[] data = client.DownloadData("https://api-maps.yandex.ru/services/panoramas/1.x/?l=stv&lang=ru_RU&origin=locate&provider=streetview&format=json&ll=" + ll + "&apikey=" + yandexApiKey);
            string jsonString = Encoding.UTF8.GetString(data);

            JSONItem json = JSON.Parse(jsonString);

            JSONItem dataNode = json["data"];
            if (dataNode == null)
            {
                Debug.Log("No data");
                return;
            }

            JSONItem imagesNode = dataNode["Data/Images"];
            if (imagesNode == null) return;

            string imageId = imagesNode.V<string>("imageId");
            JSONItem zoomsNode = imagesNode["Zooms"];

            int panoWidth = 0;
            int panoHeight = 0;
            int zoom = 0;

            foreach (JSONItem zoomNode in zoomsNode)
            {
                int w = zoomNode.V<int>("width");
                int h = zoomNode.V<int>("height");

                if (w > 8192 || h > 8192) continue;

                panoWidth = w;
                panoHeight = h;

                zoom = zoomNode.V<int>("level");
                break;
            }

            if (panoWidth == 0 || panoHeight == 0) return;

            int textureWidth = panoWidth;
            int textureHeight = panoHeight;

            if (adjustHeight) textureHeight = textureWidth / 2;

            int countX = panoWidth / 256;
            int countY = panoHeight / 256;

            if (countX * 256 != panoWidth) countX++;
            if (countY * 256 != panoHeight) countY++;

            Color[] colors = new Color[textureWidth * textureHeight];
            for (int i = 0; i < textureWidth * textureHeight; i++) colors[i] = emptyColor;

            if (EditorUtility.DisplayCancelableProgressBar("Download panorama tiles", "", 0))
            {
                EditorUtility.ClearProgressBar();
                return;
            }

            for (int cy = 0; cy < countY; cy++)
            {
                for (int cx = 0; cx < countX; cx++)
                {
                    try
                    {
                        client = new WebClient();
                        string imageURL = string.Format("http://pano.maps.yandex.net/{0}/{1}.{2}.{3}", imageId, zoom, cx, cy);
                        data = client.DownloadData(imageURL);
                        Texture2D t = new Texture2D(1, 1, TextureFormat.RGB24, false);
                        t.LoadImage(data);

                        int tw = t.width;
                        int th = t.height;

                        Color[] pixels = t.GetPixels();

                        int cx2 = cx * 256;
                        int cy2 = cy * 256 + th;

                        for (int py = 0; py < th; py++)
                        {
                            int tRow = (textureHeight - cy2 + py) * textureWidth;
                            int pRow = py * tw;
                            for (int px = 0; px < tw; px++)
                            {
                                colors[tRow + cx2 + px] = pixels[pRow + px];
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        Debug.Log(exception.Message + "\n" + exception.StackTrace);
                        return;
                    }

                    if (EditorUtility.DisplayCancelableProgressBar("Download panorama tiles", "", (cy * countX + cx) / (float)(countX * countY)))
                    {
                        EditorUtility.ClearProgressBar();
                        return;
                    }
                }
            }

            if (EditorUtility.DisplayCancelableProgressBar("Import panorama texture", "", 1))
            {
                EditorUtility.ClearProgressBar();
                return;
            }

            Texture2D texture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGB24, false);
            texture.SetPixels(colors);
            texture.Apply();

            SaveTexture(ll, texture);

            DestroyImmediate(texture);

            EditorUtility.ClearProgressBar();
        }

        private void SaveTexture(string filename, Texture2D texture)
        {
            string directory = Path.Combine("Assets", "Panoramas");
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

            string filePath = Path.Combine(directory, filename + ".png");

            File.WriteAllBytes(filePath, texture.EncodeToPNG());

            AssetDatabase.Refresh();

            Debug.Log("Panorama save to " + filePath);

            TextureImporter importer = AssetImporter.GetAtPath(filePath) as TextureImporter;
            importer.mipmapEnabled = generateMipMaps;
            importer.maxTextureSize = maxTextureSize;

            AssetDatabase.ImportAsset(filePath, ImportAssetOptions.ForceUpdate);

            Selection.activeObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(filePath);
            EditorGUIUtility.PingObject(Selection.activeObject);
        }
    }
}