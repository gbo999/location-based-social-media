using Newtonsoft.Json.Linq;
using Piglet;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace TriLibCore.Samples
{
    /// <summary>
    /// Represents a sample that loads a compressed (Zipped) Model.
    /// </summary>
    public class URL3Dexp : MonoBehaviour
    {
        /// <summary>
        /// Creates the AssetLoaderOptions instance, configures the Web Request, and downloads the Model.
        /// </summary>
        /// <remarks>
        /// You can create the AssetLoaderOptions by right clicking on the Assets Explorer and selecting "TriLib->Create->AssetLoaderOptions->Pre-Built AssetLoaderOptions".
        /// </remarks>

        private GltfImportTask _task;

        private GameObject _model;




        void Start()
        {
            StartCoroutine(GetRequestWithBody("https://api.sketchfab.com/v3/models/c578c8c12a984e79a6d958a90f86cdc8/download"));
          //  StartCoroutine(GetRequestWithBody("https://api.sketchfab.com/v3/models/c578c8c12a984e79a6d958a90f86cdc8"));

        }



        public IEnumerator GetRequestWithBody(string url)
        {
            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {

                

                //_json = "[\"" + _string + "\"]";
                www.SetRequestHeader("Authorization", "Token eea7682161e64c70a39c605117d9eedf");
              //  www.SetRequestHeader("Authorization", "Bearer eea7682161e64c70a39c605117d9eedf");
                //  www.SetRequestHeader("accept", "text/plain");
                //  www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(_json));
                yield return www.SendWebRequest();
                if (www.isNetworkError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    if (www.isDone)
                    {
                        string jsonResult = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);

                        Debug.Log(jsonResult);
                        
                        
                        //Root r = JsonUtility.FromJson<Root>(jsonResult);


                       // Debug.Log(r.gltf.url.Replace("\\", ""));


/*
                        _task = RuntimeGltfImporter.GetImportTask(r.gltf.url.Replace("\\", ""));
                        _task.OnProgress = OnProgress;
                        _task.OnCompleted = OnComplete;*/




                        //  execute(r.gltf.url.Replace("\\", "").Trim());

                        /*
                        dynamic stuff = JObject.Parse(jsonResult);
                           Debug.Log(stuff.gltf.url);
*/
                        //    Debug.Log(jsonResult);
                        //  jsonToget = JsonUtility.FromJson<object>(jsonResult);


                    }
                }
            }


        }



        private void OnProgress(GltfImportStep step, int completed, int total)
        {
            Debug.LogFormat("{0}: {1}/{2}", step, completed, total);
        }


        private void OnComplete(GameObject importedModel)
        {
            _model = importedModel;
            Debug.Log("Success!");



        }

   /*     private void Update()
        {
            _task.MoveNext();

            // spin model about y-axis
            if (_model != null)
                _model.transform.Rotate(0, 1, 0);
        }

*/




        /*




                private void execute(string url)
                {
                    var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions();
                    var webRequest = AssetDownloader.CreateWebRequest(url);
                    AssetDownloader.LoadModelFromUri(webRequest, OnLoad, OnMaterialsLoad, OnProgress, OnError, null, assetLoaderOptions,isZipFile:true);
                }

                /// <summary>
                /// Called when any error occurs.
                /// </summary>
                /// <param name="obj">The contextualized error, containing the original exception and the context passed to the method where the error was thrown.</param>
                private void OnError(IContextualizedError obj)
                {
                    Debug.LogError($"An error occurred while loading your Model: {obj.GetInnerException()}");
                }

                /// <summary>
                /// Called when the Model loading progress changes.
                /// </summary>
                /// <param name="assetLoaderContext">The context used to load the Model.</param>
                /// <param name="progress">The loading progress.</param>
                private void OnProgress(AssetLoaderContext assetLoaderContext, float progress)
                {
                    Debug.Log($"Loading Model. Progress: {progress:P}");
                }

                /// <summary>
                /// Called when the Model (including Textures and Materials) has been fully loaded.
                /// </summary>
                /// <remarks>The loaded GameObject is available on the assetLoaderContext.RootGameObject field.</remarks>
                /// <param name="assetLoaderContext">The context used to load the Model.</param>
                private void OnMaterialsLoad(AssetLoaderContext assetLoaderContext)
                {
                    Debug.Log("Materials loaded. Model fully loaded.");

                    Instantiate(assetLoaderContext.RootGameObject);


                }

                /// <summary>
                /// Called when the Model Meshes and hierarchy are loaded.
                /// </summary>
                /// <remarks>The loaded GameObject is available on the assetLoaderContext.RootGameObject field.</remarks>
                /// <param name="assetLoaderContext">The context used to load the Model.</param>
                private void OnLoad(AssetLoaderContext assetLoaderContext)
                {
                    Debug.Log("Model loaded. Loading materials.");
                }*/
    }






    [Serializable]
    public class Gltf
    {
        public string url;
        public int size;
        public int expires;
    }
    [Serializable]
    public class Usdz
    {
        public string url;
        public int size;
        public int expires;
    }
    [Serializable]
    public class Root
    {
        public Gltf gltf;
        public Usdz usdz;
    }
































}
