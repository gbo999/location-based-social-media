/*         INFINITY CODE         */
/*   https://infinity-code.com   */


using Firebase;
using Firebase.Database;
//using Firebase.Database;
using Firebase.Unity.Editor;
using InfinityCode.uPano;
using InfinityCode.uPano.Examples;
using InfinityCode.uPano.HotSpots;
using SocialApp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static OnlineMapsGoogleGeocoding;
using UnityEngine.Video;
using DanielLochner.Assets.SimpleSideMenu;
using InfinityCode.OnlineMapsDemos;
using Yamanas.Scripts.MapLoader;

namespace InfinityCode.OnlineMapsExamples
{
    /// <summary>
    /// Example of how to create a marker on click.
    /// </summary>
    [AddComponentMenu("Infinity Code/Online Maps/Examples (API Usage)/PutSpecialMarkers")]
    public class PutSpecialMarkers : MonoBehaviour
    {
        [SerializeField] private Texture2D textureTofirst;

        public ParticleSystem confetti;

        public UIElementsGroup successPopup;


        public UIElementsGroup helperText;

        public UIElementsGroup DragHelper;

        public TMP_Text SureText;


       // private UIBubblePopup bubblePopup;

        public GameObject pref;
        InstantiateGameObjectsUnderCursorExample ins;

        OnlineMapsPanoConnector on;

        public Texture2D MusicTagTexture;

        public UIElementsGroup SelectActivityMenu;

        public UIElementsGroup EventMenu;

        public Image eventPicutre;

        public Image eventCamera;

        public GameObject insbutton;

        public UIElementsGroup SaleMenu;

        public Image salePicutre;

        public Image saleCamera;


        public UIElementsGroup ShareMenu;

        public Image sharePicutre;

        public Image shareCamera;


        private string KindTochoose;


        //public ZUIManager zUIManager;

        public ToggleGroup tog;

        public UIElementsGroup PollMenu;


        public Image PictureOne;
        public Image PictureTwo;
        public Image cameraOne;
        public Image cameratwo;


        public VideoPlayer VPlayer;


        string path;

        string pathOne;

        string pathTwo;


        public TMP_InputField OptionOne;


        public TMP_InputField OptionTwo;

        public TMP_InputField Question;

        //****************************************************************////


        public GameObject publishPanel;

        //  public InputField inputTextShare;
        public TMP_InputField inputTextShare;

        public TMP_InputField inputTextEvent;
        public TMP_InputField inputTextSale;


        // private InputField inputTextChosen;
        public TMP_InputField inputTextPrice;

        public Texture2D selected;
        public static string selectedString = "cleaning";

        [SerializeField] GameObject prefab;

        //   public OnlineMapsPanoConnector panoConnector;
        public double longToSave;
        public double latToSave;
        public string addressToSave;
        public string panToSave;
        public string tiltToSave;
        public string panoramaID;


        private int FeedIndex = 0;


        public Texture2D music;

        public Texture2D sports;

        public Texture2D religion;

        public Texture2D enrichment;

        /*      public Texture2D Foodtag;
              public Texture2D arttag;
              public Texture2D religiontag;
              public Texture2D sporttag;
              public Texture2D musictag;
              public Texture2D objtag;
              public Texture2D partytag;
              public Texture2D gametag;
              public Texture2D networkingtag;
              public Texture2D othertag;

              public Texture2D hometag;
              public Texture2D carSales;
              public Texture2D elecsales;
              public Texture2D kitchSales;
              public Texture2D gardSales;
              public Texture2D hobSales;
              public Texture2D kidsSales;
              public Texture2D compSales;
              public Texture2D realSales;
      */
        public Texture2D clothingSales;

        public GameObject selectedEventPanel;

        public DatePickerControl dateTimeControl;
        public GameObject dateTimePicker;

        public GameObject setPricePanel;

        private float price = 500;

        public bool isInsta = false;


        public SimpleSideMenu sideMenu;


        public void CloseSuccessPopup()
        {
            successPopup.ChangeVisibility(false);
        }


        public void ChooseMusic()
        {
            AppManager.myCityController.currentTag = AppSettings.MusicTag;


            sideMenu.Close();
        }

        public void ChooseReligion()
        {
            AppManager.myCityController.currentTag = AppSettings.religiontag;
            sideMenu.Close();
        }

        public void ChooseSports()
        {
            AppManager.myCityController.currentTag = AppSettings.sporttag;
            sideMenu.Close();
        }


        public void ChooseEnrichment()
        {
            AppManager.myCityController.currentTag = AppSettings.enrichmentTag;
            sideMenu.Close();
        }


        /*public void insta()
        {
            ins.prefab = pref;

            isInsta = !isInsta;
        }*/

        public void check()
        {
            Debug.Log(tog.ActiveToggles().First().name);
        }


        public void SelectPhotoFromGalleryOne()
        {
#if !UNITY_EDITOR && UNITY_ANDROID
            NativeGallery.GetImageFromGallery(OnImagePickedOptionOne);


#else

            OnImagePickedOptionOne("");


#endif
        }


        public void SelectPhotoFromGalleryTwo()
        {
#if !UNITY_EDITOR && UNITY_ANDROID
            NativeGallery.GetImageFromGallery(OnImagePickedOptionTwo);


#else

            OnImagePickedOptionTwo("");


#endif
        }

        private void OnImagePickedOptionTwo(string pathFromAndroid)
        {
#if UNITY_EDITOR
            pathTwo = EditorUtility.OpenFilePanel("Overwrite with jpg", "", "jpg");

#elif !UNITY_EDITOR && UNITY_ANDROID
            pathTwo = pathFromAndroid;
#endif


            if (string.IsNullOrEmpty(pathTwo))
                return;


            FeedIndex = 3;

            byte[] fileBytes = System.IO.File.ReadAllBytes(pathTwo);

            Texture2D _texture = new Texture2D(2, 2);
            // _texture = NativeCamera.LoadImageAtPath(_url, -1, false);
            _texture.LoadImage(fileBytes);


            ResizeTexture(_texture);


            Debug.Log("before switch " + KindTochoose);


            PictureTwo.sprite = Sprite.Create(_texture, new Rect(0.0f, 0.0f, _texture.width, _texture.height),
                new Vector2(0.5f, 0.5f), 100.0f);
            PictureTwo.preserveAspect = true;
            cameratwo.gameObject.SetActive(false);
        }

        public void SelectPhotoFromGallery()
        {
#if !UNITY_EDITOR && UNITY_ANDROID
            NativeGallery.GetImageFromGallery(OnImagePicked);


#else

            OnImagePicked("");


#endif
        }


        public void SelectVideoFromGallery()
        {
#if !UNITY_EDITOR && UNITY_ANDROID
            NativeGallery.GetVideoFromGallery(OnVideoPicked);


#else
            OnVideoPicked("");


#endif
        }


        private void OnVideoPicked(string path)
        {
            StartCoroutine(OnVideoPickedEnu(path));
        }


        public void Eventchosen()
        {
            string kind = tog.ActiveToggles().First().name;

            SelectActivityMenu.ChangeVisibility(false);


            if (kind == "Event")
            {
                EventMenu.ChangeVisibility(true);
                KindTochoose = "Event";
                AppManager.myCityController.currentTag = AppSettings.EventTag;

                Debug.Log("inside toggle " + KindTochoose);

                return;
            }

            else if (kind == "Sale")
            {
                SaleMenu.ChangeVisibility(true);
                KindTochoose = "Sale";
                AppManager.myCityController.currentTag = AppSettings.SaleTag;


                Debug.Log("inside toggle " + KindTochoose);
                return;
            }


            else if (kind == "Share")
            {
                ShareMenu.ChangeVisibility(true);
                KindTochoose = "Share";
                Debug.Log("inside toggle " + KindTochoose);
                AppManager.myCityController.currentTag = AppSettings.ShareTag;


                return;
            }


            else
            {
                PollMenu.ChangeVisibility(true);
                KindTochoose = "Poll";
                Debug.Log("inside toggle " + KindTochoose);
                AppManager.myCityController.currentTag = AppSettings.PollTag;

                return;
            }
        }


        public void PostSimpleText()
        {
            switch (KindTochoose)
            {
                case "Event":
                    if (string.IsNullOrWhiteSpace(inputTextEvent.text))
                    {
                        return;
                    }

                    break;

                case "Sale":
                    if (string.IsNullOrWhiteSpace(inputTextSale.text))
                    {
                        return;
                    }

                    break;

                case "Share":
                    if (string.IsNullOrWhiteSpace(inputTextShare.text))
                    {
                        return;
                    }

                    break;
            }


            FeedIndex = 0;
            UploadFile(string.Empty);
            // publishPanel.SetActive(false);
        }


        public void checkImage()
        {
            eventPicutre.sprite = Sprite.Create(null, new Rect(0.0f, 0.0f, 100, 100), new Vector2(0.5f, 0.5f));
        }


        public void OnImagePicked(string pathFromAndroid)
        {
#if UNITY_EDITOR
            path = EditorUtility.OpenFilePanel("Overwrite with jpg", "", "jpg");

#elif !UNITY_EDITOR && UNITY_ANDROID
            path = pathFromAndroid;
#endif


            if (string.IsNullOrEmpty(path))
                return;


            FeedIndex = 1;

            byte[] fileBytes = System.IO.File.ReadAllBytes(path);

            Texture2D _texture = new Texture2D(2, 2);
            // _texture = NativeCamera.LoadImageAtPath(_url, -1, false);
            _texture.LoadImage(fileBytes);


            ResizeTexture(_texture);


            Debug.Log("before switch " + KindTochoose);


            switch (KindTochoose)
            {
                case "Event":


                    Debug.Log("in event");

                    eventPicutre.sprite = Sprite.Create(_texture, new Rect(0.0f, 0.0f, _texture.width, _texture.height),
                        new Vector2(0.5f, 0.5f), 100.0f);
                    eventPicutre.preserveAspect = true;
                    eventCamera.gameObject.SetActive(false);


                    break;


                case "Sale":
                    salePicutre.sprite = Sprite.Create(_texture, new Rect(0.0f, 0.0f, _texture.width, _texture.height),
                        new Vector2(0.5f, 0.5f), 100.0f);
                    salePicutre.preserveAspect = true;
                    saleCamera.gameObject.SetActive(false);
                    Debug.Log("in sale");

                    break;


                case "Share":
                    sharePicutre.sprite = Sprite.Create(_texture, new Rect(0.0f, 0.0f, _texture.width, _texture.height),
                        new Vector2(0.5f, 0.5f), 100.0f);
                    sharePicutre.preserveAspect = true;
                    shareCamera.gameObject.SetActive(false);
                    Debug.Log("in share");

                    break;
            }


            ;


            //  UploadFile(path);


            // publishPanel.SetActive(false);
        }


        public void OnImagePickedOptionOne(string pathFromAndroid)
        {
#if UNITY_EDITOR
            pathOne = EditorUtility.OpenFilePanel("Overwrite with jpg", "", "jpg");

#elif !UNITY_EDITOR && UNITY_ANDROID
            pathOne = pathFromAndroid;
#endif


            if (string.IsNullOrEmpty(pathOne))
                return;


            FeedIndex = 3;

            byte[] fileBytes = System.IO.File.ReadAllBytes(pathOne);

            Texture2D _texture = new Texture2D(2, 2);
            // _texture = NativeCamera.LoadImageAtPath(_url, -1, false);
            _texture.LoadImage(fileBytes);


            ResizeTexture(_texture);


            Debug.Log("before switch " + KindTochoose);


            PictureOne.sprite = Sprite.Create(_texture, new Rect(0.0f, 0.0f, _texture.width, _texture.height),
                new Vector2(0.5f, 0.5f), 100.0f);
            PictureOne.preserveAspect = true;
            cameraOne.gameObject.SetActive(false);
        }


        /*   public void LoadVideo()
           {
               StartCoroutine(OnVideoPicked());
           }
   */


        public IEnumerator OnVideoPickedEnu(string PathFromAndroid)
        {
#if UNITY_EDITOR
            path = EditorUtility.OpenFilePanel("Overwrite with mp4", "", "mp4");

#elif !UNITY_EDITOR && UNITY_ANDROID
        path = PathFromAndroid;

#endif


            if (string.IsNullOrEmpty(path))
                yield break;

            FeedIndex = 2;

            /*byte[] fileBytes = System.IO.File.ReadAllBytes(path);

            Texture2D _texture = new Texture2D(2, 2);
            // _texture = NativeCamera.LoadImageAtPath(_url, -1, false);
            _texture.LoadImage(fileBytes);


            ResizeTexture(_texture);


            Debug.Log("before switch " + KindTochoose);*/

            byte[] fileBytes = System.IO.File.ReadAllBytes(path);

            byte[] imageBytes = null;
            byte[] videoBytes = null;
            if (!CheckVideoSize(fileBytes))
            {
                AppManager.VIEW_CONTROLLER.ShowPopupMSG(MessageCode.MaxVideoSize);
                yield break;
            }
#if !UNITY_EDITOR && UNITY_ANDROID
         VPlayer.url = "file://" + path;

#else
            VPlayer.url = path;


#endif

            // VPlayer.url = path;


            VPlayer.Prepare();
            while (!VPlayer.isPrepared)
            {
                yield return null;
            }

            VPlayer.Play();
            while (!VPlayer.isPlaying)
            {
                yield return null;
            }

            yield return null;

            NativeCamera.VideoProperties _videoProp = NativeCamera.GetVideoProperties(path);

            float videoWidth = NativeGallery.GetVideoProperties(path).width;
            float videoHeight = NativeGallery.GetVideoProperties(path).height;

            videoBytes = fileBytes;
            while (VPlayer.frame < 2)
            {
                yield return null;
            }

            Texture2D _texture = ReadExternalTexture(VPlayer.texture);

            // rotate image 90 degrees
            // if is vertical
            if (videoHeight < videoWidth)
            {
                _texture = rotateTexture(_texture);
            }

            if (_videoProp.rotation == 270)
            {
                for (int i = 0; i < 2; i++)
                {
                    _texture = rotateTexture(_texture);
                }
            }


            ResizeTexture(_texture);
            imageBytes = _texture.EncodeToJPG(AppManager.APP_SETTINGS.UploadImageQuality);
            /*  
              sharePicutre.sprite = Sprite.Create(_texture, new Rect(0.0f, 0.0f, _texture.width, _texture.height), new Vector2(0.5f, 0.5f), 100.0f);
              sharePicutre.preserveAspect = true;*/
            VPlayer.Stop();


            switch (KindTochoose)
            {
                case "Event":


                    Debug.Log("in event");

                    eventPicutre.sprite = Sprite.Create(_texture, new Rect(0.0f, 0.0f, _texture.width, _texture.height),
                        new Vector2(0.5f, 0.5f), 100.0f);
                    eventPicutre.preserveAspect = true;
                    //eventCamera.gameObject.SetActive(false);


                    break;


                case "Sale":
                    salePicutre.sprite = Sprite.Create(_texture, new Rect(0.0f, 0.0f, _texture.width, _texture.height),
                        new Vector2(0.5f, 0.5f), 100.0f);
                    salePicutre.preserveAspect = true;
                    // saleCamera.gameObject.SetActive(false);


                    break;


                case "Share":
                    sharePicutre.sprite = Sprite.Create(_texture, new Rect(0.0f, 0.0f, _texture.width, _texture.height),
                        new Vector2(0.5f, 0.5f), 100.0f);
                    sharePicutre.preserveAspect = true;
                    //shareCamera.gameObject.SetActive(false);
                    Debug.Log("in share");

                    break;
            }


            //  UploadFile(path);


            yield break;

            // publishPanel.SetActive(false);
        }

        private Texture2D tagToPrefab(string tag)
        {
            switch (tag)
            {
                case "Music": return music;
                case "Enrichment": return enrichment;
                case "Religion": return religion;
                case "Sports": return sports;

                default:
                    return sports;
            }
        }


        private Texture2D ReadExternalTexture(Texture externalTexture)
        {
            Texture2D myTexture2D = new Texture2D(externalTexture.width, externalTexture.height);
            if (myTexture2D == null)
            {
                myTexture2D = new Texture2D(externalTexture.width, externalTexture.height);
            }

            //Make RenderTexture type variable
            RenderTexture tmp = RenderTexture.GetTemporary(
                externalTexture.width,
                externalTexture.height,
                0,
                RenderTextureFormat.ARGB32,
                RenderTextureReadWrite.sRGB);

            Graphics.Blit(externalTexture, tmp);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = tmp;

            myTexture2D.ReadPixels(new UnityEngine.Rect(0, 0, tmp.width, tmp.height), 0, 0);
            myTexture2D.Apply();

            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(tmp);

            return myTexture2D;
        }

        public static Texture2D rotateTexture(Texture2D t)
        {
            Texture2D newTexture = new Texture2D(t.height, t.width, t.format, false);

            for (int i = 0; i < t.width; i++)
            {
                for (int j = 0; j < t.height; j++)
                {
                    newTexture.SetPixel(j, i, t.GetPixel(t.width - i, j));
                }
            }

            newTexture.Apply();
            return newTexture;
        }


        private FeedType GetFeedType()
        {
            FeedType _type = FeedType.Text;
            if (FeedIndex == 0)
            {
                _type = FeedType.Text;
            }

            if (FeedIndex == 1)
            {
                _type = FeedType.Image;
            }

            if (FeedIndex == 2)
            {
                _type = FeedType.Video;
            }

            if (FeedIndex == 3)
            {
                _type = FeedType.Poll;
            }

            return _type;
        }


        public void PublishPost()
        {
            ShareMenu.ChangeVisibility(false);
            EventMenu.ChangeVisibility(false);
            SaleMenu.ChangeVisibility(false);
            PollMenu.ChangeVisibility(false);


            UploadFile(path);
        }

        public void Save()
        {
            ins.Save();
        }


        public void PublishPoll()
        {
            UploadPoll(pathOne, pathTwo);
        }

        public void UploadPoll(string pathOne, string pathTwo)
        {
            AppManager.VIEW_CONTROLLER.ShowLoading();
            StartCoroutine(GetPoll(pathOne, pathTwo));
        }


        private IEnumerator GetPoll(string pathOne, string pathTwo)
        {
            FeedType _type = GetFeedType();
            Feed _feed = new Feed();
            _feed.Type = _type;
            _feed.panStreetview = panToSave;
            _feed.tiltStreetview = tiltToSave;
            _feed.panoramaID = panoramaID;
            _feed.groupPostID = AppManager.myCityController.groupPostID;

            Debug.Log("groupostID" + _feed.groupPostID);

            _feed.tag = AppManager.myCityController.currentTag;

            _feed.OwnerID = AppManager.Instance.auth.CurrentUser.UserId;

            byte[] imageOneBytes = null;
            byte[] imageTwoBytes = null;


            string fileNameOne = System.Guid.NewGuid().ToString();
            string fileNameTwo = System.Guid.NewGuid().ToString();


            _feed.kind = KindTochoose;

            byte[] fileBytesOne = System.IO.File.ReadAllBytes(pathOne);

            byte[] fileBytesTwo = System.IO.File.ReadAllBytes(pathTwo);


            Texture2D _textureOne = new Texture2D(2, 2);
            //_texture = NativeCamera.LoadImageAtPath(_url, -1, false);
            _textureOne.LoadImage(fileBytesOne);
            yield return new WaitForEndOfFrame();

            ResizeTexture(_textureOne);

            imageOneBytes = _textureOne.EncodeToJPG(AppManager.APP_SETTINGS.UploadImageQuality);

            _feed.MediaWidthOne = _textureOne.width;
            _feed.MeidaHeightOne = _textureOne.height;


            Texture2D _textureTwo = new Texture2D(2, 2);
            //_texture = NativeCamera.LoadImageAtPath(_url, -1, false);
            _textureTwo.LoadImage(fileBytesTwo);
            yield return new WaitForEndOfFrame();

            ResizeTexture(_textureTwo);


            imageTwoBytes = _textureTwo.EncodeToJPG(AppManager.APP_SETTINGS.UploadImageQuality);


            _feed.MediaWidthTwo = _textureTwo.width;
            _feed.MeidaHeightTwo = _textureTwo.height;


            _feed.TXTOne = OptionOne.text;


            _feed.TXTTwo = OptionTwo.text;

            FileUploadRequset _imageUploadRequestOne = new FileUploadRequset();
            _imageUploadRequestOne.FeedType = _type;
            _imageUploadRequestOne.FileName = fileNameOne + "." + Utils.GetFileExtension(pathOne);
            _imageUploadRequestOne.UploadBytes = imageOneBytes;

            FileUploadCallback _callBack = new FileUploadCallback();
            AppManager.FIREBASE_CONTROLLER.UploadFile(_imageUploadRequestOne, callback => { _callBack = callback; });
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

            _feed.ImageURLOne = _callBack.DownloadUrl;

            _feed.BodyTXT = Question.text;


            FileUploadRequset _imageUploadRequestTwo = new FileUploadRequset();
            _imageUploadRequestTwo.FeedType = _type;
            _imageUploadRequestTwo.FileName = fileNameTwo + "." + Utils.GetFileExtension(pathTwo);
            _imageUploadRequestTwo.UploadBytes = imageTwoBytes;

            FileUploadCallback _callBackTwo = new FileUploadCallback();
            AppManager.FIREBASE_CONTROLLER.UploadFile(_imageUploadRequestTwo, callback => { _callBackTwo = callback; });
            while (!_callBackTwo.IsComplete)
            {
                yield return null;
            }

            if (!_callBackTwo.IsSuccess)
            {
                AppManager.VIEW_CONTROLLER.HideLoading();
                AppManager.VIEW_CONTROLLER.ShowPopupMSG(MessageCode.FailedUploadFeed);
                yield break;
            }

            _feed.ImageURLTwo = _callBackTwo.DownloadUrl;
            _feed.dateToSave = DateTime.Now.ToString("MM-yyyy");

            _feed.Feedlng = longToSave;
            _feed.Feedlat = latToSave;
            //  _feed.groupId = AppManager.myCityController.groupID;
            _feed.address = addressToSave;

            //     texture.LoadImage(fileContent);

            _feed.DateCreated = GetDate();
            FeedUploadCallback _feedCallback = null;
            AppManager.FIREBASE_CONTROLLER.AddNewPost(_feed, callback => { _feedCallback = callback; });
            while (_feedCallback == null)
            {
                yield return null;
            }

            if (!_feedCallback.IsSuccess)
            {
                AppManager.VIEW_CONTROLLER.HideLoading();
                AppManager.VIEW_CONTROLLER.ShowPopupMSG(MessageCode.FailedUploadFeed);
                yield break;
            }

            //BodyTextInput.text = string.Empty;
            // upload finish


            // AppManager.VIEW_CONTROLLER.ShowPopupMSG(MessageCode.SuccessPost);

            successPopup.ChangeVisibility(true);

            markerDrag = true;

            AppManager.VIEW_CONTROLLER.HideLoading();


            OnlineMapsMarker3DManager.RemoveItem(onlineMapsMarkertoput);


            //bubblePopup.CreateMarker(_feedCallback.postID);
        }


        public void UploadFile(string _url, bool _showPreview = true)
        {
            AppManager.VIEW_CONTROLLER.ShowLoading();
            StartCoroutine(GetFile(_url, _showPreview));
        }


        private IEnumerator GetFile(string _url, bool _showPreview)
        {
            FeedType _type = GetFeedType();
            Feed _feed = new Feed();
            _feed.Type = _type;
            _feed.panStreetview = panToSave;
            _feed.tiltStreetview = tiltToSave;
            _feed.panoramaID = panoramaID;


            _feed.groupPostID = AppManager.myCityController.groupPostID;

            Debug.Log("groupostID" + _feed.groupPostID);

            _feed.tag = AppManager.myCityController.currentTag;

            _feed.OwnerID = AppManager.Instance.auth.CurrentUser.UserId;
            // _feed.ToUserID = feedDadaLoader.GetUserID();
            byte[] imageBytes = null;
            byte[] videoBytes = null;
            string fileName = System.Guid.NewGuid().ToString();
            //  Texture2D previewTexure = new Texture2D(2, 2);

            _feed.kind = KindTochoose;

            double val;
            if (!double.TryParse(inputTextPrice.text, out val))
                val = 0.0;


            _feed.price = val;

            _feed.dateToSave = DateTime.Now.ToString("MM-yyyy");

            if (_feed.kind == "Event")
            {
                _feed.startDate = "scheduled for " + dateTimeControl.fecha.ToString("dd-MM-yyyy HH:mm");


                _feed.dateToSave = dateTimeControl.fecha.ToString("MM-yyyy");
            }


            if (_type == FeedType.Image || _type == FeedType.Video)
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(_url);

                if (_type == FeedType.Video)
                {
                    if (!CheckVideoSize(fileBytes))
                    {
                        AppManager.VIEW_CONTROLLER.ShowPopupMSG(MessageCode.MaxVideoSize);
                        yield break;
                    }

                    //  VPlayer.url = "file://" + _url;

                    VPlayer.url = _url;


                    VPlayer.Prepare();
                    while (!VPlayer.isPrepared)
                    {
                        yield return null;
                    }

                    VPlayer.Play();
                    while (!VPlayer.isPlaying)
                    {
                        yield return null;
                    }

                    yield return null;

                    NativeCamera.VideoProperties _videoProp = NativeCamera.GetVideoProperties(_url);

                    float videoWidth = NativeGallery.GetVideoProperties(_url).width;
                    float videoHeight = NativeGallery.GetVideoProperties(_url).height;

                    videoBytes = fileBytes;
                    while (VPlayer.frame < 2)
                    {
                        yield return null;
                    }

                    Texture2D _texture = ReadExternalTexture(VPlayer.texture);

                    // rotate image 90 degrees
                    // if is vertical
                    if (videoHeight < videoWidth)
                    {
                        _texture = rotateTexture(_texture);
                    }

                    if (_videoProp.rotation == 270)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            _texture = rotateTexture(_texture);
                        }
                    }


                    ResizeTexture(_texture);
                    imageBytes = _texture.EncodeToJPG(AppManager.APP_SETTINGS.UploadImageQuality);
                    _feed.MediaWidth = _texture.width;
                    _feed.MeidaHeight = _texture.height;
                    _feed.VideoFileName = fileName;
                    sharePicutre.sprite = Sprite.Create(_texture, new Rect(0.0f, 0.0f, _texture.width, _texture.height),
                        new Vector2(0.5f, 0.5f), 100.0f);
                    sharePicutre.preserveAspect = true;
                    VPlayer.Stop();
                }


                if (_type == FeedType.Image)
                {
                    Texture2D _texture = new Texture2D(2, 2);
                    //_texture = NativeCamera.LoadImageAtPath(_url, -1, false);
                    _texture.LoadImage(fileBytes);
                    yield return new WaitForEndOfFrame();

                    ResizeTexture(_texture);


                    imageBytes = _texture.EncodeToJPG(AppManager.APP_SETTINGS.UploadImageQuality);


                    _feed.MediaWidth = _texture.width;
                    _feed.MeidaHeight = _texture.height;

                    // previewTexure = _texture;
                }
            }


            switch (_feed.kind)
            {
                case "Event":

                    _feed.BodyTXT = inputTextEvent.text;

                    break;

                case "Sale":

                    _feed.BodyTXT = inputTextSale.text;

                    break;

                case "Share":

                    _feed.BodyTXT = inputTextShare.text;
                    ;

                    break;
            }


            AppManager.VIEW_CONTROLLER.ShowLoading();

            // wait for preview callback
            if (_type == FeedType.Image)
            {
                // upload image
                FileUploadRequset _imageUploadRequest = new FileUploadRequset();
                _imageUploadRequest.FeedType = _type;
                _imageUploadRequest.FileName = fileName + "." + Utils.GetFileExtension(_url);
                _imageUploadRequest.UploadBytes = imageBytes;

                FileUploadCallback _callBack = new FileUploadCallback();
                AppManager.FIREBASE_CONTROLLER.UploadFile(_imageUploadRequest, callback => { _callBack = callback; });
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

                _feed.ImageURL = _callBack.DownloadUrl;
            }


            if (_type == FeedType.Video)
            {
                // upload video
                FileUploadRequset _videoUploadRequest = new FileUploadRequset();
                _videoUploadRequest.FeedType = _type;
                _videoUploadRequest.FileName = fileName + "." + Utils.GetFileExtension(_url);
                _videoUploadRequest.UploadBytes = videoBytes;

                FileUploadCallback _callBack = new FileUploadCallback();
                AppManager.FIREBASE_CONTROLLER.UploadFile(_videoUploadRequest, callback => { _callBack = callback; }
                );
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

                // upload video preview
                FileUploadRequset _imageUploadRequest = new FileUploadRequset();
                _imageUploadRequest.FeedType = FeedType.Image;
                _imageUploadRequest.FileName = System.Guid.NewGuid() + ".jpg";
                _imageUploadRequest.UploadBytes = imageBytes;

                FileUploadCallback _callBack2 = new FileUploadCallback();
                AppManager.FIREBASE_CONTROLLER.UploadFile(_imageUploadRequest, callback => { _callBack2 = callback; }
                );
                while (!_callBack2.IsComplete)
                {
                    yield return null;
                }

                if (!_callBack2.IsSuccess)
                {
                    AppManager.VIEW_CONTROLLER.HideLoading();
                    AppManager.VIEW_CONTROLLER.ShowPopupMSG(MessageCode.FailedUploadFeed);
                    yield break;
                }

                //_feed.VideoURL = _callBack.DownloadUrl;
                _feed.ImageURL = _callBack2.DownloadUrl;
            }


            _feed.Feedlng = longToSave;
            _feed.Feedlat = latToSave;
            //  _feed.groupId = AppManager.myCityController.groupID;
            _feed.address = addressToSave;

            //     texture.LoadImage(fileContent);

            _feed.DateCreated = GetDate();
            FeedUploadCallback _feedCallback = null;
            AppManager.FIREBASE_CONTROLLER.AddNewPost(_feed, callback => { _feedCallback = callback; });
            while (_feedCallback == null)
            {
                yield return null;
            }

            if (!_feedCallback.IsSuccess)
            {
                AppManager.VIEW_CONTROLLER.HideLoading();
                AppManager.VIEW_CONTROLLER.ShowPopupMSG(MessageCode.FailedUploadFeed);
                yield break;
            }

            if (_type == FeedType.Video)
            {
                string databasePath = AppSettings.AllPostsKey + "/" + _feed.Key + "/VideoURL";
                AppManager.Instance.Firebase.UploadAndCompressVideo(
                    AppSettings.FeedUploadVideoPath + fileName + "." + Utils.GetFileExtension(_url), databasePath);
            }
            //BodyTextInput.text = string.Empty;
            // upload finish


            // AppManager.VIEW_CONTROLLER.ShowPopupMSG(MessageCode.SuccessPost);

            successPopup.ChangeVisibility(true);


            markerDrag = true;

            AppManager.VIEW_CONTROLLER.HideLoading();


            OnlineMapsMarker3DManager.RemoveItem(onlineMapsMarkertoput);


            //bubblePopup.CreateMarker(_feedCallback.postID);
        }

        public bool CheckVideoSize(byte[] _bytes)
        {
            int _mb = _bytes.Length / 1024 / 1024;
            return _mb <= AppManager.APP_SETTINGS.MaxUploadVideoSizeMB;
        }

        private string GetDate()
        {
            return System.DateTime.Now.ToString(AppManager.APP_SETTINGS.SystemDateFormat);
        }

        private void ResizeTexture(Texture2D _texture)
        {
            int maxAllowResoulution = (int) AppManager.APP_SETTINGS.MaxAllowFeedImageQuality;
            int _width = _texture.width;
            int _height = _texture.height;
            if (_width > _height)
            {
                if (_width > maxAllowResoulution)
                {
                    float _delta = (float) _width / (float) maxAllowResoulution;
                    _height = Mathf.FloorToInt((float) _height / _delta);
                    _width = maxAllowResoulution;
                }
            }
            else
            {
                if (_height > maxAllowResoulution)
                {
                    float _delta = (float) _height / (float) maxAllowResoulution;
                    _width = Mathf.FloorToInt((float) _width / _delta);
                    _height = maxAllowResoulution;
                }
            }

            TextureScale.Bilinear(_texture, _width, _height);
        }


        //public GameObject DebugText;
        Text debug;
        public bool markerDrag = true;

        private OnlineMapsMarker3D onlineMapsMarkertoput;


        /*

                private string cleaningString = "cleaning";
                private string fixString = "fix";
                private string PesticideString = "pesticide";
                private string dumpsterString = "dumpster";
                private string benchString = "bench";
                private string treeString = "tree";
                private string signString = "sign";
                private string cuttingString = "cutting";
                private string lightingString = "lighting";
                private string roundaboutString = "roundabout";
                private string crossingString = "crosswalk";
                private string parkString = "Playground";


                public Texture2D cleaning;
                public Texture2D fix;
                public Texture2D Pesticide;
                public Texture2D dumpster;
                public Texture2D bench;
                public Texture2D tree;
                public Texture2D sign;
                public Texture2D cutting;
                public Texture2D lighting;
                public Texture2D roundabout;
                public Texture2D crossing;
                public Texture2D park;
                public Texture2D football;

                public Texture2D rest;



                public void cleaningChosen()
                {

                    selected = cleaning;
                    selectedString = cleaningString;
                   */ /* InstantiateGameObjectsUnderCursorExample.selectedStreetviewString = selectedString;

                    if (LoadHotSpotsForStreetView.isStarted)
                    {
                        InstantiateGameObjectsUnderCursorExample.selectedPrefab = InstantiateGameObjectsUnderCursorExample.cleaningStreetView;

                    }
        */ /*


                }

                public void fixChosen()
                {

                    selected = fix;
                    selectedString = fixString;
                   */ /* InstantiateGameObjectsUnderCursorExample.selectedStreetviewString = selectedString;

                    if (LoadHotSpotsForStreetView.isStarted)
                    {
                        InstantiateGameObjectsUnderCursorExample.selectedPrefab = InstantiateGameObjectsUnderCursorExample.fixStreetView;

                    }*/ /*

                }


                public void peststicideChosen()
                {

                    selected = Pesticide;
                    selectedString = PesticideString;
                  */ /*  InstantiateGameObjectsUnderCursorExample.selectedStreetviewString = selectedString;

                    if (LoadHotSpotsForStreetView.isStarted)
                    {
                        InstantiateGameObjectsUnderCursorExample.selectedPrefab = InstantiateGameObjectsUnderCursorExample.PesticideStreetView;

                    }
                  */ /*
                }


                public void dumpsterChosen()
                {

                    selected = dumpster;
                    selectedString = dumpsterString;
                   */ /* InstantiateGameObjectsUnderCursorExample.selectedStreetviewString = selectedString;

                    if (LoadHotSpotsForStreetView.isStarted)
                    {
                        InstantiateGameObjectsUnderCursorExample.selectedPrefab = InstantiateGameObjectsUnderCursorExample.dumpsterStreetView;

                    }*/ /*

                }

                public void benchChosen()
                {

                    selected = bench;
                    selectedString = benchString;
                    */ /*InstantiateGameObjectsUnderCursorExample.selectedStreetviewString = selectedString;

                    if (LoadHotSpotsForStreetView.isStarted)
                    {
                        InstantiateGameObjectsUnderCursorExample.selectedPrefab = InstantiateGameObjectsUnderCursorExample.benchStreetView;

                    }
        */ /*
                }




                public void treeChosen()
                {

                    selected = tree;
                    selectedString = treeString;
                   */ /* InstantiateGameObjectsUnderCursorExample.selectedStreetviewString = selectedString;

                    if (LoadHotSpotsForStreetView.isStarted)
                    {
                        InstantiateGameObjectsUnderCursorExample.selectedPrefab = InstantiateGameObjectsUnderCursorExample.treeStreetView;

                    }*/ /*

                }



                public void signChosen()
                {

                    selected = sign;
                    selectedString = signString;
        */ /*
                    InstantiateGameObjectsUnderCursorExample.selectedStreetviewString = selectedString;

                    if (LoadHotSpotsForStreetView.isStarted)
                    {
                        InstantiateGameObjectsUnderCursorExample.selectedPrefab = InstantiateGameObjectsUnderCursorExample.signStreetView;

                    }*/ /*
                }






                public void pruningChosen()
                {

                    selected = cutting;
                    selectedString = cuttingString;
                    */ /*InstantiateGameObjectsUnderCursorExample.selectedStreetviewString = selectedString;

                    if (LoadHotSpotsForStreetView.isStarted)
                    {
                        InstantiateGameObjectsUnderCursorExample.selectedPrefab = InstantiateGameObjectsUnderCursorExample.cuttingStreetView;

                    }*/ /*

                }




                public void lightingChosen()
                {

                    selected = lighting;
                    selectedString = lightingString;
                   */ /* InstantiateGameObjectsUnderCursorExample.selectedStreetviewString = selectedString;

                    if (LoadHotSpotsForStreetView.isStarted)
                    {
                        InstantiateGameObjectsUnderCursorExample.selectedPrefab = InstantiateGameObjectsUnderCursorExample.lightingStreetView;

                    }*/ /*

                }


                public void roundaboutChosen()
                {

                    selected = roundabout;
                    selectedString = roundaboutString;
                   */ /* InstantiateGameObjectsUnderCursorExample.selectedStreetviewString = selectedString;

                    if (LoadHotSpotsForStreetView.isStarted)
                    {
                        InstantiateGameObjectsUnderCursorExample.selectedPrefab = InstantiateGameObjectsUnderCursorExample.roundaboutStreetView;

                    }*/ /*

                }


                public void crossingChosen()
                {

                    selected = crossing;
                    selectedString = crossingString;

                   */ /* InstantiateGameObjectsUnderCursorExample.selectedStreetviewString = selectedString;

                    if (LoadHotSpotsForStreetView.isStarted)
                    {
                        InstantiateGameObjectsUnderCursorExample.selectedPrefab = InstantiateGameObjectsUnderCursorExample.crossingStreetView;

                    }*/ /*
                }






                public void parkChosen()
                {

                    selected = park;
                    selectedString = parkString;
                  */ /*  InstantiateGameObjectsUnderCursorExample.selectedStreetviewString = selectedString;

                    if (LoadHotSpotsForStreetView.isStarted)
                    {
                        InstantiateGameObjectsUnderCursorExample.selectedPrefab = InstantiateGameObjectsUnderCursorExample.parkStreetView;

                    }*/ /*

                }



                public void footballChosen()
                {

                    selected = football;
                    selectedString = "football";
                    */ /*  InstantiateGameObjectsUnderCursorExample.selectedStreetviewString = selectedString;

                      if (LoadHotSpotsForStreetView.isStarted)
                      {
                          InstantiateGameObjectsUnderCursorExample.selectedPrefab = InstantiateGameObjectsUnderCursorExample.parkStreetView;

                      }*/ /*

                }


        */


        private void GetIcon(ImageSize size_256)
        {
            GetProfileImageRequest _request = new GetProfileImageRequest();
            _request.groupID = AppManager.myCityController.groupPostID;
            _request.Size = size_256;

            AppManager.FIREBASE_CONTROLLER.GetIcon(_request, OnProfileImageGetted);
        }

        private void OnProfileImageGetted(GetProfileImageCallback _callback)
        {
            Debug.Log("is callbak " + _callback.IsSuccess);


            if (_callback.IsSuccess)
            {
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(_callback.ImageBytes);
                prefab.GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture,
                    new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
                //ResizeAvarar(AvatarSize);
            }
            else
            {
                Debug.Log("not success");
            }
        }

        private void Start()
        {
            on = GetComponent<OnlineMapsPanoConnector>();

            on.OnLoaded += addcomp;

            //GetIcon(ImageSize.Size_128);


            //bubblePopup = gameObject.GetComponent<UIBubblePopup>();


            /*      FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://mycity3-8.firebaseio.com/");

                  DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;



                  FirebaseDatabase.DefaultInstance
                .GetReference(MyCitySettings.locations)
                .GetValueAsync().ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        Debug.Log("error in get");
                    }
                    else if (task.IsCompleted)
                    {
                        DataSnapshot MainSnapshot = task.Result;
                        foreach (DataSnapshot snapshot in MainSnapshot.Children)
                        {



                            locations.Add(JsonUtility.FromJson<LocationMyCity>(snapshot.GetRawJsonValue()));



                        }



                    }
                });*/


            /*
                        foreach(LocationMyCity location in locations)
                        {

                            OnlineMapsMarker onlineMapsMarkertoput = OnlineMapsMarkerManager.CreateItem(Convert.ToDouble(location.longtitude), Convert.ToDouble(location.latitude), marker, "created by " + MyCityMananger.FullName);



                        }*/

            /*
             * 
             * 
                              for (int i = 0; i < LoadHotSpotsForStreetView.ListLocations.Count; i++)
                              {

                                  OnlineMapsMarkerManager.CreateItem(LoadHotSpotsForStreetView.ListLocations.ElementAt(i).longtitude, LoadHotSpotsForStreetView.ListLocations.ElementAt(i).latitude, marker, "");


                              }*/


            /* foreach (Record record in ReadHotSpotsFromDB(meta.id))
             {
                 int prefabIndex = record.prefabIndex;
                 if (prefabIndex < 0 || prefabIndex >= hotSpotPrefabs.Length) continue;
                 GameObject prefab = hotSpotPrefabs[prefabIndex];
                 if (prefab == null) continue;

                 HotSpot hotSpot = manager.Create(0, 0, prefab);
                 UpdatePosition(hotSpot, record.longitude, record.latitude, record.altitude);
             }

*/

            // Subscribe to the click event.


            OnlineMapsControlBase.instance.OnMapClick += OnMapClick;
            //   Pano.OnPanoDestroy += OnPanoDestroy;
        }

        private void addcomp(Pano obj)
        {
            ins = obj.gameObject.AddComponent<InstantiateGameObjectsUnderCursorExample>();
        }

        private void OnPanoDestroy(Pano obj)
        {
            LoadHotSpotsForStreetView.isStarted = false;


            /* if (LoadHotSpotsForStreetView.ListLocations.Count>0) { 

                 for (int i = 0; i < LoadHotSpotsForStreetView.ListLocations.Count; i++)
                 {

                     if (LoadHotSpotsForStreetView.ListLocations.ElementAt(i) != null)


                     OnlineMapsMarkerManager.CreateItem(LoadHotSpotsForStreetView.ListLocations.ElementAt(i).longtitude, LoadHotSpotsForStreetView.ListLocations.ElementAt(i).latitude, marker, "");


                 }

         }*/
        }

        private void OnMapClick()
        {
            if (Input.GetKey(KeyCode.LeftControl) || markersMode.markerMode)
            {
                // Get the coordinates under the cursor.
                double lng, lat;
                OnlineMapsControlBase.instance.GetCoords(out lng, out lat);
                longToSave = lng;
                latToSave = lat;

                PostProcessController.Instance.Longtitude = lng;
                PostProcessController.Instance.Longtitude = lat;

                prefab.GetComponent<SpriteRenderer>().sprite = Sprite.Create(textureTofirst,
                    new Rect(0.0f, 0.0f, textureTofirst.width, textureTofirst.height), new Vector2(0.5f, 0.5f), 100.0f);
                onlineMapsMarkertoput = OnlineMapsMarker3DManager.CreateItem(lng, lat, prefab);
                onlineMapsMarkertoput.sizeType = OnlineMapsMarker3D.SizeType.scene;
                onlineMapsMarkertoput.scale = 17;
                onlineMapsMarkertoput.rotation = Quaternion.Euler(85.803f, 0, -180.957f);

                onlineMapsMarkertoput.isDraggable = true;

                onlineMapsMarkertoput.OnRelease += StopDragging;


                markersMode.markerMode = false;

                helperText.ChangeVisibility(false);
                DragHelper.ChangeVisibility(true);
            }
        }


        public void saveMaps()
        {
            if (longToSave != 0.0)
            {
                SelectActivityMenu.ChangeVisibility(true);
            }

            OnlineMapsMarker3DManager.RemoveItemAt(OnlineMapsMarker3DManager.CountItems - 1);


            publish();
        }

        public void StopDrag()
        {
            markerDrag = false;
        }


        private void StopDragging(OnlineMapsMarkerBase obj)
        {
            markersMode.markerMode = false;

            if (markerDrag == false)
            {
                obj.isDraggable = false;
            }

            double lang, lat;
            obj.GetPosition(out lang, out lat);

            longToSave = lang;
            latToSave = lat;


            PostProcessController.Instance.Longtitude = lang;
            PostProcessController.Instance.Latitude = lat;


            publish();
        }


        public void publish()
        {
            //publishPanel.SetActive(false);

            ReverseGeocodingParams requestParams = new ReverseGeocodingParams(longToSave, latToSave);

            requestParams.key = MyCity.Config.APIKeys.GoogleMapsAPIKey;
            requestParams.language = "en";


            OnlineMapsGoogleGeocoding request = OnlineMapsGoogleGeocoding.Find(requestParams);


            request.Send();
            request.OnComplete += OnRequestComplete;
        }


        public void areSure()
        {
            SureText.text = "are you sure you want save the pin " + addressToSave;
        }

        private void OnRequestComplete(string s)
        {
            XmlDocument xDoc = new XmlDocument();

            xDoc.LoadXml(s);

            Debug.Log("success");

            XmlNodeList name = xDoc.GetElementsByTagName("formatted_address");

            Debug.Log(name[1].InnerText);


            addressToSave = "at " + name[1].InnerText;

            PostProcessController.Instance.Address = addressToSave;
        }


        /*private   IEnumerator addPostFromMaps(Feed feed,FirebaseController controller)
                {

                FeedUploadCallback _feedCallback = null;
                controller.AddNewPost(feed, (callback) => {

                    

                    _feedCallback = callback;

                });

  */ /*          FeedsDataLoader feedsDataLoader = FindObjectOfType<FeedsDataLoader>();
            
            feedsDataLoader.ResetLoader();
            */ /*



                while (_feedCallback == null)
                {
                    yield return null;
                }

            if (!_feedCallback.IsSuccess)
            {
                Debug.Log("problem uploading to feed");
                yield break;
            }


            else
            {
                Debug.Log("success uploading to feed from maps");

            }



            markersMode.markerMode = false;



        }
*/


        //Debug.Log(name[0].InnerText);


        //Debug.Log(s);
    }
}