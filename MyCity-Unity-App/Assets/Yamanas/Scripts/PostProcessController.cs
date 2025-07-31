using System;
using System.Collections;
using SocialApp;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Yamanas.Infrastructure.Popups;
using Yamanas.Scripts.MapLoader.AR;
using Yamanas.Scripts.MapLoader.Gamification;

namespace Yamanas.Scripts.MapLoader
{
    public class PostProcessController : MonoBehaviour
    {
        #region Fields

        private EnvState _envState;

        private static PostProcessController _instance;

        [SerializeField] private PopupSystem _popupSystem;

        [SerializeField] private ARPopupSystem _arPopupSystem;

        private double longtitude;

        private double latitude;

        private string _address;

        private string _kind;

        private FeedType _feedType;

        private string _path;

        private string panToSave;

        private string tiltToSave;

        private string panoramaID;

        private double _price;

        private string _eventStartDate;

        private string _dateTosave;

        private VideoPlayer _vPlayer;

        private Image _imageForVideo;

        private string _postText;


        #region Poll

        private string _optionOne;

        private string _optiontwo;

        private string _question;

        #endregion

        #endregion

        #region Methods

        private void Awake()
        {
            _envState = EnvState.Map;
        }

        public void UploadFile(string _url, bool _showPreview = true)
        {
            AppManager.VIEW_CONTROLLER.ShowLoading();
            StartCoroutine(GetFile(_url, _showPreview));
        }


        private IEnumerator GetFile(string _url, bool _showPreview)
        {
            Feed _feed = new Feed();
            _feed.Type = _feedType;
            _feed.panStreetview = panToSave;
            _feed.tiltStreetview = tiltToSave;
            _feed.panoramaID = panoramaID;

            _feed.BodyTXT = _postText;

            _feed.groupPostID = AppManager.myCityController.groupPostID;

            Debug.Log("groupostID" + _feed.groupPostID);

            _feed.tag = AppManager.myCityController.currentTag;

            _feed.OwnerID = AppManager.Instance.auth.CurrentUser.UserId;
            // _feed.ToUserID = feedDadaLoader.GetUserID();
            byte[] imageBytes = null;
            byte[] videoBytes = null;
            string fileName = System.Guid.NewGuid().ToString();
            //  Texture2D previewTexure = new Texture2D(2, 2);

            _feed.kind = _kind;

            // double val;
            // if (!double.TryParse(inputTextPrice.text, out val))
            //     val = 0.0;


            _feed.price = _price;

            _feed.dateToSave = DateTime.Now.ToString("MM-yyyy");

            if (_feed.kind == "Event")
            {
                _feed.startDate = "scheduled for " + _eventStartDate;


                _feed.dateToSave = _dateTosave;
            }


            if (_feedType == FeedType.Image || _feedType == FeedType.Video)
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(_url);

                if (_feedType == FeedType.Video)
                {
                    if (!CheckVideoSize(fileBytes))
                    {
                        AppManager.VIEW_CONTROLLER.ShowPopupMSG(MessageCode.MaxVideoSize);
                        yield break;
                    }

                    //  VPlayer.url = "file://" + _url;

                    _vPlayer.url = _url;


                    _vPlayer.Prepare();
                    while (!_vPlayer.isPrepared)
                    {
                        yield return null;
                    }

                    _vPlayer.Play();
                    while (!_vPlayer.isPlaying)
                    {
                        yield return null;
                    }

                    yield return null;

                    NativeCamera.VideoProperties _videoProp = NativeCamera.GetVideoProperties(_url);

                    float videoWidth = NativeGallery.GetVideoProperties(_url).width;
                    float videoHeight = NativeGallery.GetVideoProperties(_url).height;

                    videoBytes = fileBytes;
                    while (_vPlayer.frame < 2)
                    {
                        yield return null;
                    }

                    Texture2D _texture = ReadExternalTexture(_vPlayer.texture);

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
                    _imageForVideo.sprite = Sprite.Create(_texture,
                        new Rect(0.0f, 0.0f, _texture.width, _texture.height),
                        new Vector2(0.5f, 0.5f), 100.0f);
                    _imageForVideo.preserveAspect = true;
                    _vPlayer.Stop();
                }


                if (_feedType == FeedType.Image)
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


            AppManager.VIEW_CONTROLLER.ShowLoading();

            // wait for preview callback
            if (_feedType == FeedType.Image)
            {
                // upload image
                FileUploadRequset _imageUploadRequest = new FileUploadRequset();
                _imageUploadRequest.FeedType = _feedType;
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


            if (_feedType == FeedType.Video)
            {
                // upload video
                FileUploadRequset _videoUploadRequest = new FileUploadRequset();
                _videoUploadRequest.FeedType = _feedType;
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


            try
            {
                _feed.Feedlng = longtitude;
                _feed.Feedlat = latitude;
                //  _feed.groupId = AppManager.myCityController.groupID;
                _feed.address = _address;

                Debug.Log("lng is address");
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
                throw;
            }


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

            if (_feedType == FeedType.Video)
            {
                string databasePath = AppSettings.AllPostsKey + "/" + _feed.Key + "/VideoURL";
                AppManager.Instance.Firebase.UploadAndCompressVideo(
                    AppSettings.FeedUploadVideoPath + fileName + "." + Utils.GetFileExtension(_url), databasePath);
            }
            //BodyTextInput.text = string.Empty;
            // upload finish


            // AppManager.VIEW_CONTROLLER.ShowPopupMSG(MessageCode.SuccessPost);


            if (_envState == EnvState.Map || _envState == EnvState.StreetView)
            {
                _popupSystem.ShowPopup(PopupType.Success, "");
            }
            else if (_envState == EnvState.AR)
            {
                _arPopupSystem.ShowPopup(ARPopupType.Success);
            }

            // markerDrag = true;

            AppManager.VIEW_CONTROLLER.HideLoading();

            try
            {
                OnlineMapsMarker3DManager.RemoveItem(PinFactory.Instance.OnlineMapsMarkertoput);
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
                throw;
            }

            Debug.Log($"is this is null or not {PinFactory.Instance.OnlineMapsMarkertoput == null}");


            PinFactory.Instance.CreatePin(_feed);
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

        public bool CheckVideoSize(byte[] _bytes)
        {
            int _mb = _bytes.Length / 1024 / 1024;
            return _mb <= AppManager.APP_SETTINGS.MaxUploadVideoSizeMB;
        }

        #region PollMethods

        public void UploadPoll(string pathOne, string pathTwo)
        {
            AppManager.VIEW_CONTROLLER.ShowLoading();
            StartCoroutine(GetPoll(pathOne, pathTwo));
        }


        private IEnumerator GetPoll(string pathOne, string pathTwo)
        {
            FeedType _type = FeedType.Poll;
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


            //  _feed.kind = KindTochoose;

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


            _feed.TXTOne = _optionOne;


            _feed.TXTTwo = _optiontwo;

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

            _feed.BodyTXT = _question;


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

            _feed.Feedlng = longtitude;
            _feed.Feedlat = latitude;
            //  _feed.groupId = AppManager.myCityController.groupID;
            _feed.address = _address;

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

            _popupSystem.ShowPopup(PopupType.Success, "");


            //successPopup.ChangeVisibility(true);

            //  markerDrag = true;

            AppManager.VIEW_CONTROLLER.HideLoading();

            OnlineMapsMarker3DManager.RemoveItem(PinFactory.Instance.OnlineMapsMarkertoput);

            yield return new WaitForSeconds(1);
            try
            {
                AppManager.FIREBASE_CONTROLLER.AddCurrencyAndPoints(10);
               
                FeatherFactory.Instance.CreateFive();
            }
            catch (Exception e)
            {
                Debug.Log("feathers or not " +e.ToString());
            }

            // OnlineMapsMarker3DManager.RemoveItem(onlineMapsMarkertoput);

            PinFactory.Instance.CreatePin(_feed);


            // bubblePopup.CreateMarker(_feedCallback.postID);
        }

        #endregion

        #endregion

        #region Properties

        public string PostText
        {
            get => _postText;
            set => _postText = value;
        }

        public Image ImageVideo
        {
            get => _imageForVideo;
            set => _imageForVideo = value;
        }

        public VideoPlayer VideoPlayer
        {
            get => _vPlayer;
            set => _vPlayer = value;
        }

        public string DateTosave
        {
            get => _dateTosave;
            set => _dateTosave = value;
        }

        public string EventStartDate
        {
            get => _eventStartDate;
            set => _eventStartDate = value;
        }

        public double Price
        {
            get => _price;
            set => _price = value;
        }


        public string Path
        {
            get => _path;
            set => _path = value;
        }

        public string Kind
        {
            get => _kind;
            set => _kind = value;
        }

        public FeedType FeedType
        {
            get => _feedType;
            set => _feedType = value;
        }

        public string Address
        {
            get => _address;
            set => _address = value;
        }

        public double Longtitude
        {
            get => longtitude;
            set => longtitude = value;
        }

        public double Latitude
        {
            get => latitude;
            set => latitude = value;
        }

        public string PanToSave
        {
            get => panToSave;
            set => panToSave = value;
        }

        public string TiltToSave
        {
            get => tiltToSave;
            set => tiltToSave = value;
        }

        public string PanoramaID
        {
            get => panoramaID;
            set => panoramaID = value;
        }

        public EnvState EnvState
        {
            get => _envState;
            set => _envState = value;
        }

        public string OptionOne
        {
            get => _optionOne;
            set => _optionOne = value;
        }

        public string Optiontwo
        {
            get => _optiontwo;
            set => _optiontwo = value;
        }

        public string Question
        {
            get => _question;
            set => _question = value;
        }

        public PopupSystem PopupSystem => _popupSystem;

        public ARPopupSystem ARPopupSystem => _arPopupSystem;

        public static PostProcessController Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<PostProcessController>();
                }

                return _instance;
            }
        }

        #endregion
    }
}