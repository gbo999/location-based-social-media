using SocialApp;
using System;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using TriLibCore;
using TriLibCore.Extensions;
using TriLibCore.Interfaces;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ModelUploader : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject _loadedGameObject;


    /// <summary>
    /// The load Model Button.
    /// </summary>
    [SerializeField]
    private Button _loadModelButton;

    /// <summary>
    /// The progress indicator Text;
    /// </summary>
    [SerializeField]
    private Text _progressText;

    public Image im;

    public TMP_InputField inputField;


    string modelPath;

    string extension;


    Byte[] thumbnail;

    /// <summary>
    /// Creates the AssetLoaderOptions instance and displays the Model file-picker.
    /// </summary>
    /// <remarks>
    /// You can create the AssetLoaderOptions by right clicking on the Assets Explorer and selecting "TriLib->Create->AssetLoaderOptions->Pre-Built AssetLoaderOptions".
    /// </remarks>
    /// 
    public void save()
    {

        Debug.Log(modelPath);

        StartCoroutine(upload());


    }


    public IEnumerator upload()
    {


        Model3D model = new Model3D();

        string fileName = System.Guid.NewGuid().ToString();



        FileUploadRequset _imageUploadRequest = new FileUploadRequset();
        _imageUploadRequest.FeedType = FeedType.thumb;
        _imageUploadRequest.FileName = fileName+ ".jpg";
        _imageUploadRequest.UploadBytes = thumbnail;

        FileUploadCallback _callBack = new FileUploadCallback();
        AppManager.FIREBASE_CONTROLLER.UploadFile(_imageUploadRequest, callback =>
        {
            _callBack = callback;
        });
        while (!_callBack.IsComplete)
        {
            yield return null;
        }
        if (!_callBack.IsSuccess)
        {
            AppManager.VIEW_CONTROLLER.HideLoading();
            AppManager.VIEW_CONTROLLER.ShowPopupMSG(MessageCode.FailedUploadFeed);
            yield break;
        }
        model.thumbnailUrl = _callBack.DownloadUrl;

        Debug.Log("thumb url "+_callBack.DownloadUrl);



        Debug.Log("here before" +modelPath);


        // string local_file_uri = string.Format("{0}://{1}",Uri.UriSchemeFile, modelPath);

        byte[] fileBytes = System.IO.File.ReadAllBytes(modelPath);

        ModelUploadCallback ModelCallback = new ModelUploadCallback();
        AppManager.FIREBASE_CONTROLLER.UploadModel(fileBytes, callback =>
        {
            ModelCallback = callback;
        });
        while (!ModelCallback.IsComplete)
        {
            yield return null;
        }
        if (!ModelCallback.IsSuccess)
        {
            AppManager.VIEW_CONTROLLER.HideLoading();
            AppManager.VIEW_CONTROLLER.ShowPopupMSG(MessageCode.FailedUploadFeed);
            yield break;
        }

        Debug.Log("model url" + ModelCallback.DownloadUrl);







        model.ModelURl = ModelCallback.DownloadUrl;

        model.FileExtension = extension;

        model.Name = inputField.text;

        uploadModelDataCallback uploadModelDataCallback = null;
        AppManager.FIREBASE_CONTROLLER.UploadModelData(model, callback =>
        {
            uploadModelDataCallback = callback;
        });
        while (uploadModelDataCallback == null)
        {
            yield return null;
        }
        if (!uploadModelDataCallback.IsSuccess)
        {
            AppManager.VIEW_CONTROLLER.HideLoading();
            AppManager.VIEW_CONTROLLER.ShowPopupMSG(MessageCode.FailedUploadFeed);
            yield break;
        }
       
        // upload finish
        AppManager.VIEW_CONTROLLER.ShowPopupMSG(MessageCode.SuccessPost);



    }







    public void LoadModel()
    {
        /*
                modelPath = EditorUtility.OpenFilePanel("Overwrite with 3mf", "", "3mf");


                Debug.Log("Loaded path " + modelPath);
        */

        var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions();
        var assetLoaderFilePicker = AssetLoaderFilePicker.Create();
        assetLoaderFilePicker.LoadModelFromFilePickerAsync("Select a Model file", OnLoad, OnMaterialsLoad, OnProgress, OnBeginLoad, OnError, null, assetLoaderOptions);
    }

    /// <summary>
    /// Called when the the Model begins to load.
    /// </summary>
    /// <param name="filesSelected">Indicates if any file has been selected.</param>
    private void OnBeginLoad(bool filesSelected)
    {
        _loadModelButton.interactable = !filesSelected;
        _progressText.enabled = filesSelected;
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
        _progressText.text = $"Progress: {progress:P}";
    }

    /// <summary>
    /// Called when the Model (including Textures and Materials) has been fully loaded.
    /// </summary>
    /// <remarks>The loaded GameObject is available on the assetLoaderContext.RootGameObject field.</remarks>
    /// <param name="assetLoaderContext">The context used to load the Model.</param>
    private void OnMaterialsLoad(AssetLoaderContext assetLoaderContext)
    {
        if (assetLoaderContext.RootGameObject != null)
        {
            

            Debug.Log("Model fully loaded.");

            Instantiate(assetLoaderContext.RootGameObject, Camera.main.transform);

            //modelPath = assetLoaderContext.BasePath;

            modelPath =  assetLoaderContext.Filename;

            extension = assetLoaderContext.FileExtension;

            RuntimePreviewGenerator.BackgroundColor = Color.white;

            Texture2D tex = RuntimePreviewGenerator.GenerateModelPreview(assetLoaderContext.RootGameObject.transform);

            im.sprite=Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);


            thumbnail = createReadabeTexture2D( tex).EncodeToJPG(AppManager.APP_SETTINGS.UploadImageQuality);




        }
        else
        {
            Debug.Log("Model could not be loaded.");
        }
        _loadModelButton.interactable = true;
        _progressText.enabled = false;
    }

    /// <summary>
    /// Called when the Model Meshes and hierarchy are loaded.
    /// </summary>
    /// <remarks>The loaded GameObject is available on the assetLoaderContext.RootGameObject field.</remarks>
    /// <param name="assetLoaderContext">The context used to load the Model.</param>
    private void OnLoad(AssetLoaderContext assetLoaderContext)
    {
        if (_loadedGameObject != null)
        {
            Destroy(_loadedGameObject);
        }
        _loadedGameObject = assetLoaderContext.RootGameObject;
        if (_loadedGameObject != null)
        {
            Camera.main.FitToBounds(assetLoaderContext.RootGameObject, 2f);

            // _loadedGameObject = assetLoaderContext.RootGameObject;



            // Instantiate(_loadedGameObject, Camera.main.transform);
        }
    }



    Texture2D createReadabeTexture2D(Texture2D texture2d)

    {

        RenderTexture renderTexture = RenderTexture.GetTemporary(

                    texture2d.width,

                    texture2d.height,

                    0,

                    RenderTextureFormat.Default,

                    RenderTextureReadWrite.Linear);

        Graphics.Blit(texture2d, renderTexture);

        RenderTexture previous = RenderTexture.active;

        RenderTexture.active = renderTexture;

        Texture2D readableTextur2D = new Texture2D(texture2d.width, texture2d.height);

        readableTextur2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);

        readableTextur2D.Apply();

        RenderTexture.active = previous;

        RenderTexture.ReleaseTemporary(renderTexture);

        return readableTextur2D;

    }


















}


public class ModelUploadCallback
{
    public bool IsComplete;
    public bool IsSuccess;
    public string DownloadUrl;
}

public class uploadModelDataCallback
{
    public bool IsComplete;
    public bool IsSuccess;


}

[System.Serializable]
public class Model3D
{


    public string ModelId;
    public string Name;
    public string FileExtension;
    public string thumbnailUrl;
    public string ModelURl;
    



}




