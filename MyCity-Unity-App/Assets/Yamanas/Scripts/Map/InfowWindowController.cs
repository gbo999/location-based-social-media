using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Firebase.Database;
using SocialApp;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;
using Yamanas.Infrastructure.Popups;
using Yamanas.Scripts.AR;

namespace Yamanas.Scripts.Map.InfoWindow
{
    public class InfowWindowController : MonoBehaviour
    {
        #region Consts

        private const string MARKER_DATA = "FeedData";

        #endregion


        #region Fields-View

        [Header("Bubble Position")] [SerializeField]
        private float howMuchToLess = 0f;

        [SerializeField] private float howMuchMore = -7;

        [SerializeField] private float howmuchz = -200f;

        [SerializeField] private float distanceFromCamera = 1f;

        [SerializeField] private float theX = 130f;

        [SerializeField] private float theY = 130f;

        [SerializeField] private Camera cam;

        [Header("Bubble Poll")] [SerializeField]
        private ToggleGroup RadioGrop;

        [SerializeField] private Button vote;

        [SerializeField] private TMP_Text optionOneText;

        [SerializeField] private TMP_Text optionTwoText;


        [SerializeField] private TMP_Text optionTwoCount;

        [SerializeField] private TMP_Text optionOneCount;

        [SerializeField] private Image imageOptionOne;

        [SerializeField] private Image imageOptionTwo;

        [SerializeField] private TMP_Text textOne;

        [SerializeField] private TMP_Text texttwo;

        [Header("Bubble Content")] [SerializeField]
        private TMP_Text _likesCount;

        [SerializeField] private TMP_Text _partCount;

        [SerializeField] private TMP_Text _commetCount;

        [SerializeField] private Image LikeImage = default;

        [SerializeField] private Color LikedPostColor = default;

        [SerializeField] private Color UnLikedPostColor = default;

        [SerializeField] private Image _partiImage = default;

        [SerializeField] private Color _partiPostColor = default;

        [SerializeField] private Color _unPartiPostColor;


        [SerializeField] private UIElementsGroup bubble;

        [SerializeField] private Canvas _canvas;

        [SerializeField] private TMP_Text PriceOrDAteText;

        [SerializeField] private TMP_Text userName;

        [SerializeField] private TMP_Text address;

        [SerializeField] private TMP_Text dateCreated;

        [SerializeField] private TMP_Text details;

        [SerializeField] private Image ImageBody;

        [SerializeField] private RawImage VideoBody;

        [SerializeField] private Image AvatarImage;

        [SerializeField] private VideoPlayer VPlayer;

        [SerializeField] private float AvatarSize = default;

        [SerializeField] private float DefaultAvatarSize = default;

        [SerializeField] private RectTransform AvatarRect = default;

        [SerializeField] private GameObject loading;

        [SerializeField] private UIElementsGroup Comments;

        [SerializeField] private GameObject _partiButton;

        [SerializeField] private Image _hatXP;

        [SerializeField] private Color _blankColor;

        private OnlineMapsMarker3D target3D;

        #endregion

        #region Fields-Data

        [Header("Bubble Data")] private DatabaseReference DRPostParticipateCount;


        private DatabaseReference DRPostLikesCount;

        private bool _isParticipate;

        private bool _canLikePost;

        private bool _isPostLiked;

        private string feedkey;

        private static InfowWindowController _instance;

        private DatabaseReference OptionOneCount;

        private DatabaseReference OptiontwoCount;

        private bool isVoted;

        private bool _canParticipate;
        private bool CanLikePost;

        private bool IsPostLiked;

        private bool _isPostParti;

        [SerializeField] private MessagesDataLoader CommentLoader;

        private string _userID;

        private Feed _feed;


        [Header("loading spinners")] [SerializeField]
        private GameObject _avatarSpinner;

        [SerializeField] private GameObject _postBodySpinner;

        #endregion


        #region Methods

        public void OnParticipantsNumberClick()
        {
            PopupSystem.Instance.ShowPopup(PopupType.Participants, _feed);
        }

        public void OnProfilePictureClick()
        {
            PopupSystem.Instance.ShowPopup(PopupType.Profile, _userID);
        }


        private void DisplayDefaultAvatar()
        {
            if (AppManager.APP_SETTINGS != null)
            {
                Texture2D _defaultAvatar = AppManager.APP_SETTINGS.DefaultAvatarTexture;
                AvatarImage.sprite = Sprite.Create(_defaultAvatar,
                    new Rect(0.0f, 0.0f, _defaultAvatar.width, _defaultAvatar.height), new Vector2(0.5f, 0.5f), 100.0f);
                ResizeAvarar(DefaultAvatarSize);
            }
        }

        private IEnumerator OnLoadGraphicOne(Feed feed)
        {
            string _url = feed.ImageURLOne;
            if (!string.IsNullOrEmpty(_url))
            {
                UnityWebRequest www = UnityWebRequestTexture.GetTexture(_url);
                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    Texture2D _texture = ((DownloadHandlerTexture) www.downloadHandler).texture;
                    if (_texture != null)
                    {
                        imageOptionOne.sprite = Sprite.Create(_texture,
                            new Rect(0.0f, 0.0f, _texture.width, _texture.height), new Vector2(0.5f, 0.5f), 100.0f);
                    }
                }
            }
        }

        private IEnumerator OnLoadGraphicTwo(Feed feed)
        {
            string _url = feed.ImageURLTwo;
            if (!string.IsNullOrEmpty(_url))
            {
                UnityWebRequest www = UnityWebRequestTexture.GetTexture(_url);
                yield return www.SendWebRequest();

                _postBodySpinner.SetActive(false);
                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    Texture2D _texture = ((DownloadHandlerTexture) www.downloadHandler).texture;
                    if (_texture != null)
                    {
                        imageOptionTwo.sprite = Sprite.Create(_texture,
                            new Rect(0.0f, 0.0f, _texture.width, _texture.height), new Vector2(0.5f, 0.5f), 100.0f);
                    }
                }
            }
        }

        private void Start()
        {
            OnlineMaps.instance.OnChangePosition += UpdateBubblePosition;
            OnlineMaps.instance.OnChangeZoom += UpdateBubblePosition;
            OnlineMapsControlBase.instance.OnMapClick += OnMapClick;


            if (OnlineMapsControlBaseDynamicMesh.instance != null)
            {
                OnlineMapsControlBaseDynamicMesh.instance.OnMeshUpdated += UpdateBubblePosition;
            }

            if (OnlineMapsCameraOrbit.instance != null)
            {
                OnlineMapsCameraOrbit.instance.OnCameraControl += UpdateBubblePosition;
            }


            bubble.ChangeVisibility(false);
        }

        private void OnMapClick()
        {
            target3D = null;


            if (bubble.Visible)
            {
                RemoveListeners();

                if (_feed.kind == "Poll")
                {
                    RemovePollListeners();
                }

                if (_feed.kind == "Events")
                {
                    RemoveParticipantsListeners();
                }
            }


            // Hide the popup
            bubble.ChangeVisibility(false);
        }

        private void UpdateBubblePosition()
        {
            // If no marker is selected then exit.
            if (target3D == null) return;

            // Hide the popup if the marker is outside the map view
            if (!target3D.inMapView)
            {
                if (bubble.Visible) bubble.gameObject.SetActive(false);
            }
            else if (!bubble.Visible) bubble.gameObject.SetActive(true);


            Vector3 pos = OnlineMapsTileSetControl.instance.GetWorldPosition(target3D.position) +
                          new Vector3(0, howMuchToLess, howmuchz);
            Vector2 screenPosition = OnlineMapsTileSetControl.instance.activeCamera.WorldToScreenPoint(pos);


            Vector2 point;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform, screenPosition,
                null, out point);

            (bubble.transform as RectTransform).localPosition = point;

            (bubble.transform.GetChild(0) as RectTransform).localPosition = point;
        }

        public void UpdateData(OnlineMapsMarkerBase onlineMapsMarkerBase)
        {
            _avatarSpinner.SetActive(true);
            _postBodySpinner.SetActive(true);

            SetBubblePosition(onlineMapsMarkerBase as OnlineMapsMarker3D);


            Feed feed = onlineMapsMarkerBase[MARKER_DATA] as Feed;
            _feed = feed;
            feedkey = feed.Key;
            RouteManager.Instance.Latitude = feed.Feedlat;
            RouteManager.Instance.Logntitude = feed.Feedlng;


            _userID = feed.OwnerID;

            Debug.Log($"what is lat {RouteManager.Instance.Latitude}");


            AddLikeListeners(feed);
            LoadLikes();
            LoadComments();

            if (feed.Type == FeedType.Poll)
            {
                loadPoll(feedkey);

                AddPollListeners(feedkey);
            }

            if (feed.kind == "Event")
            {
                _partiButton.SetActive(true);
                LoadParti();
                AddParticipateListeners(feedkey);
            }
            else
            {
                _partiButton.SetActive(false);
                _partCount.text = "";
            }


            AppManager.FIREBASE_CONTROLLER.GetUserFullName(feed.OwnerID, (_userName => { userName.text = _userName; }));
            address.text = feed.address;
            dateCreated.text = feed.DateCreated;
            details.text = feed.BodyTXT;
            if (feed.kind == "Sale")
            {
                PriceOrDAteText.text = "price: " + feed.price.ToString() + " $";
            }
            else if (feed.kind == "Event")
            {
                PriceOrDAteText.text = feed.startDate;
            }
            
            AppManager.FIREBASE_CONTROLLER.GetXPLevel(feed.OwnerID,
                xpLevel =>
                {
                    Debug.Log($"xp is {xpLevel}");
                    //_hatImage.color = Color.white;

                    if (xpLevel == 1 || xpLevel == 2 || xpLevel == 3 ||xpLevel==6)
                    {
                        _hatXP.sprite = Resources.Load<Sprite>($"ProductsSprites/Level{xpLevel}");

                        _hatXP.rectTransform.rotation = Quaternion.Euler(0,0,54.1579514f);
                    }

                    else if (xpLevel == 4 || xpLevel == 5)
                    {
                        _hatXP.sprite = Resources.Load<Sprite>($"ProductsSprites/Level{xpLevel}");

                        _hatXP.rectTransform.rotation = Quaternion.Euler(0,0,357.850464f);
                    }

                    else
                    {
                        _hatXP.color = _blankColor;
                    }
                });

            GetProfileImageRequest _request = new GetProfileImageRequest();
            _request.Id = feed.OwnerID;
            _request.Size = ImageSize.Size_512;
            AppManager.FIREBASE_CONTROLLER.GetProfileImage(_request, OnProfileImageGetted);

            if (feed.Type == FeedType.Image)
            {
                SetDataNotPoll(feed);
            }

            if (feed.Type == FeedType.Video)
            {
                SetDataVideo(feed);
            }

            if (feed.Type == FeedType.Poll)
            {
                SetDataPoll(feed);
            }
            //
            // bubble.GetComponent<RectTransform>().localScale = new Vector3(0.5f, 0.5f, 0.5f);
            // bubble.transform.position = cam.ViewportToWorldPoint(new Vector3(theX, theY, distanceFromCamera));

            bubble.ChangeVisibility(true);
        }

        private void SetDataPoll(Feed feed)
        {
            ImageBody.gameObject.SetActive(false);
            VideoBody.gameObject.SetActive(false);

            imageOptionOne.gameObject.SetActive(true);

            imageOptionTwo.gameObject.SetActive(true);

            RadioGrop.gameObject.SetActive(true);

            vote.gameObject.SetActive(true);

            textOne.text = feed.TXTOne;
            texttwo.text = feed.TXTTwo;

            optionOneCount.gameObject.SetActive(true);

            optionTwoCount.gameObject.SetActive(true);

            optionOneText.text = feed.TXTOne;
            optionTwoText.text = feed.TXTTwo;


            StartCoroutine(OnLoadGraphicOne(feed));
            StartCoroutine(OnLoadGraphicTwo(feed));
        }

        private void SetDataVideo(Feed feed)
        {
            ImageBody.gameObject.SetActive(true);

            imageOptionOne.gameObject.SetActive(false);

            imageOptionTwo.gameObject.SetActive(false);

            RadioGrop.gameObject.SetActive(false);

            vote.gameObject.SetActive(false);
          //  ImageBody.gameObject.SetActive(false);
            VideoBody.gameObject.SetActive(true);
            optionOneCount.gameObject.SetActive(false);

            optionTwoCount.gameObject.SetActive(false);

            optionOneText.gameObject.SetActive(false);

            optionTwoText.gameObject.SetActive(false);

            StartCoroutine(OnLoadVideo(feed));
        }

        private void SetDataNotPoll(Feed feed)
        {
            ImageBody.gameObject.SetActive(true);

            StartCoroutine(OnLoadGraphic(feed));

            imageOptionOne.gameObject.SetActive(false);

            imageOptionTwo.gameObject.SetActive(false);

            RadioGrop.gameObject.SetActive(false);

            vote.gameObject.SetActive(false);
            VideoBody.gameObject.SetActive(false);


            optionOneCount.gameObject.SetActive(false);

            optionTwoCount.gameObject.SetActive(false);

            optionOneText.gameObject.SetActive(false);

            optionTwoText.gameObject.SetActive(false);
        }


        private void AddPollListeners(string feedkey)
        {
            OptionOneCount = AppManager.FIREBASE_CONTROLLER.GetsCountOptionOneReferense(feedkey);
            OptionOneCount.ValueChanged += OnOptionOneCountUpdated;


            OptiontwoCount = AppManager.FIREBASE_CONTROLLER.GetsCountOptionTwoReferense(feedkey);
            OptiontwoCount.ValueChanged += OnOptionTwoCountUpdated;
        }

        private void RemovePollListeners()
        {
            OptionOneCount.ValueChanged -= OnOptionOneCountUpdated;


            OptiontwoCount = AppManager.FIREBASE_CONTROLLER.GetsCountOptionTwoReferense(feedkey);
            OptiontwoCount.ValueChanged -= OnOptionTwoCountUpdated;
        }

        private void AddParticipateListeners(string feedkey)
        {
            DRPostParticipateCount = AppManager.FIREBASE_CONTROLLER.GetPostParticipateCountReferense(feedkey);
            DRPostParticipateCount.ValueChanged += OnParticipateCountUpdated;
        }

        private void RemoveParticipantsListeners()
        {
            DRPostParticipateCount.ValueChanged -= OnParticipateCountUpdated;
        }

        private void AddLikeListeners(Feed feed)
        {
            DRPostLikesCount = AppManager.FIREBASE_CONTROLLER.GetPostLikesCountReferense(feed.Key);
            DRPostLikesCount.ValueChanged += OnLikesCountUpdated;
        }


        private void RemoveListeners()
        {
            DRPostLikesCount.ValueChanged -= OnLikesCountUpdated;
        }


        private void SetBubblePosition(OnlineMapsMarker3D onlineMapsMarker3D)
        {
            target3D = onlineMapsMarker3D;


            UpdateBubblePosition();

            Vector2 screenPosition = OnlineMapsControlBase.instance.GetScreenPosition(target3D.position);
            RectTransform rectTransform = bubble.GetComponent<RectTransform>();
            screenPosition.y -= rectTransform.sizeDelta.y * rectTransform.lossyScale.y * howMuchMore;
            OnlineMaps.instance.position = OnlineMapsControlBase.instance.GetCoords(screenPosition);

            bubble.ChangeVisibility(true);
        }

        private void OnProfileImageGetted(GetProfileImageCallback _callback)
        {
            
            _avatarSpinner.SetActive(false);
            
            if (_callback.IsSuccess)
            {
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(_callback.ImageBytes);
                AvatarImage.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f), 100.0f);

                ResizeAvarar(AvatarSize);
            }
            else
            {
                DisplayDefaultAvatar();
            }
        }

        private void ResizeAvarar(float _size)
        {
            float _bodyWidth = _size;
            float _bodyHeight = _size;
            float _imageWidth = (float) AvatarImage.sprite.texture.width;
            float _imageHeight = (float) AvatarImage.sprite.texture.height;
            float _ratio = _imageWidth / _imageHeight;
            if (_imageWidth > _imageHeight)
            {
                _ratio = _imageHeight / _imageWidth;
            }

            float _expectedHeight = _bodyWidth / _ratio;
            if (_imageWidth > _imageHeight)
            {
                AvatarRect.sizeDelta = new Vector2(_expectedHeight, _bodyHeight);
            }
            else
            {
                AvatarRect.sizeDelta = new Vector2(_bodyWidth, _expectedHeight);
            }
        }

        private IEnumerator OnLoadVideo(Feed feed)
        {
            


            string _url = feed.VideoURL;
/*
            if (!string.IsNullOrEmpty(feed.ImageURL))
            {
                UnityWebRequest www = UnityWebRequestTexture.GetTexture(LoadedFeed.ImageURL);
                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    //Debug.Log(www.error);
                }
                else
                {
                    Texture2D _texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                    VideoBody.texture = _texture;
                }
            }*/

            if (!AppManager.APP_SETTINGS.UseOriginVideoFile)
            {
                _url = string.Empty;
                bool _isGettedUrl = false;
                string _tempUrl = string.Empty;
                AppManager.FIREBASE_CONTROLLER.GetFeedVideoFileUrl(feed.VideoFileName, (_gettedUrl =>
                {
                    _isGettedUrl = true;
                    _tempUrl = _gettedUrl;
                }));
                while (!_isGettedUrl)
                {
                    yield return null;
                }

                _url = _tempUrl;
            }

            if (!string.IsNullOrEmpty(_url))
            {
                VPlayer.url = _url;
                VPlayer.Prepare();
                while (!VPlayer.isPrepared)
                {
                    yield return null;
                }

                _postBodySpinner.SetActive(false);
              //  VideoBody.texture = VPlayer.texture;
              VPlayer.Play();
              
              ImageBody.material.mainTexture = VPlayer.texture;

              ImageBody.gameObject.SetActive(false);
              ImageBody.gameObject.SetActive(true);

                
            }
        }

        private IEnumerator OnLoadGraphic(Feed feed)
        {
            string _url = feed.ImageURL;
            if (!string.IsNullOrEmpty(_url))
            {
                UnityWebRequest www = UnityWebRequestTexture.GetTexture(_url);
                yield return www.SendWebRequest();

                _postBodySpinner.SetActive(false);
                
                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    Texture2D _texture = ((DownloadHandlerTexture) www.downloadHandler).texture;
                    if (_texture != null)
                    {
                        ImageBody.sprite = Sprite.Create(_texture,
                            new Rect(0.0f, 0.0f, _texture.width, _texture.height), new Vector2(0.5f, 0.5f), 100.0f);
                    }
                }
            }
        }

        private void loadPoll(string feedkey)
        {
            RadioGrop.gameObject.SetActive(false);
            vote.gameObject.SetActive(false);
            optionOneText.gameObject.SetActive(true);
            optionTwoText.gameObject.SetActive(true);


            AppManager.FIREBASE_CONTROLLER.IsVoted(feedkey, _isvote =>
            {
                isVoted = _isvote;
                if (isVoted)
                {
                    RadioGrop.gameObject.SetActive(false);
                    vote.gameObject.SetActive(false);
                    optionOneText.gameObject.SetActive(true);
                    optionTwoText.gameObject.SetActive(true);
                }
                else
                {
                    Debug.Log("is voted =true?");
                    optionOneText.gameObject.SetActive(false);
                    optionTwoText.gameObject.SetActive(false);

                    RadioGrop.gameObject.SetActive(true);
                    vote.gameObject.SetActive(true);
                }
            });
        }

        private void OnOptionTwoCountUpdated(object sender, ValueChangedEventArgs args)
        {
            if (args.DatabaseError != null)
            {
                Debug.LogError(args.DatabaseError.Message);
                optionTwoCount.text = "0";
                return;
            }

            try
            {
                if (args.Snapshot.Value.ToString() == "0")
                {
                    optionTwoCount.text = "0";
                }
                else
                {
                    optionTwoCount.text = args.Snapshot.Value.ToString();
                }
            }
            catch (Exception e)
            {
                optionTwoCount.text = "0";
                //  Debug.Log(e.ToString());
            }
        }

        private void OnOptionOneCountUpdated(object sender, ValueChangedEventArgs args)
        {
            if (args.DatabaseError != null)
            {
                Debug.LogError(args.DatabaseError.Message);
                optionOneCount.text = "0";
                return;
            }

            try
            {
                if (args.Snapshot.Value.ToString() == "0")
                {
                    optionOneCount.text = "0";
                }
                else
                {
                    optionOneCount.text = args.Snapshot.Value.ToString();
                }
            }
            catch (Exception e)
            {
                optionOneCount.text = "0";
                //  Debug.Log(e.ToString());
            }
        }

        private void LoadComments()
        {
            AppManager.FIREBASE_CONTROLLER.GetPostCommentsCount(feedkey,
                _count => { _commetCount.text = _count; });
        }

        private void OnParticipateCountUpdated(object sender, ValueChangedEventArgs args)
        {
            if (args.DatabaseError != null)
            {
                Debug.LogError(args.DatabaseError.Message);
                _partCount.text = "0";
                return;
            }

            try
            {
                if (args.Snapshot.Value.ToString() == "0")
                {
                    _partCount.text = "0";
                }
                else
                {
                    _partCount.text = $"{args.Snapshot.Value}";
                }
            }
            catch (Exception e)
            {
                _partCount.text = "0";
                //  Debug.Log(e.ToString());
            }
        }

        private void OnLikesCountUpdated(object sender, ValueChangedEventArgs args)
        {
            if (args.DatabaseError != null)
            {
                Debug.LogError(args.DatabaseError.Message);
                _likesCount.text = "0";
                return;
            }

            try
            {
                if (args.Snapshot.Value.ToString() == "0")
                {
                    _likesCount.text = "0";
                }
                else
                {
                    _likesCount.text = $"{args.Snapshot.Value}";
                }
            }
            catch (Exception e)
            {
                _likesCount.text = "0";
                //  Debug.Log(e.ToString());
            }
        }

        public void ClickParti()
        {
            if (_canParticipate)
            {
                if (!_isPostParti)
                {
                    AppManager.FIREBASE_CONTROLLER.Participate(feedkey, success =>
                    {
                        if (success)
                        {
                            LoadParti();
                        }
                    });
                }
                else
                {
                    AppManager.FIREBASE_CONTROLLER.UnParticipate(feedkey, success =>
                    {
                        if (success)
                        {
                            LoadParti();
                        }
                    });
                }
            }
        }

        private void LoadParti()
        {
            _canParticipate = false;

            AppManager.FIREBASE_CONTROLLER.IsPartiPost(feedkey, isParticipate =>
            {
                _canParticipate = true;
                _isParticipate = isParticipate;
                if (isParticipate)
                {
                    _partiImage.color = _partiPostColor;
                }
                else
                {
                    _partiImage.color = _unPartiPostColor;
                }
            });
        }

        public void ClickLike()
        {
            Debug.Log("like is clicked");
            if (CanLikePost)
            {
                if (IsPostLiked)
                {
                    AppManager.FIREBASE_CONTROLLER.UnLikPost(feedkey, success =>
                    {
                        if (success)
                        {
                            LoadLikes();
                        }
                    });
                }
                else
                {
                    AppManager.FIREBASE_CONTROLLER.LikPost(feedkey, success =>
                    {
                        if (success)
                        {
                            LoadLikes();
                        }
                    });
                }
            }
        }

        private void LoadLikes()
        {
            CanLikePost = false;

            AppManager.FIREBASE_CONTROLLER.IsLikedPost(feedkey, _isLike =>
            {
                CanLikePost = true;
                IsPostLiked = _isLike;
                if (IsPostLiked)
                {
                    LikeImage.color = LikedPostColor;
                }
                else
                {
                    LikeImage.color = UnLikedPostColor;
                }
            });
        }


        public void clickVote()
        {
            Toggle tog;


            string kind = RadioGrop.ActiveToggles().First().name;


            AppManager.FIREBASE_CONTROLLER.Vote(feedkey, kind, success =>
            {
                if (success)
                {
                    loadPoll(feedkey);
                }
            });
        }


        public void clickCommnent()
        {
            Comments.ChangeVisibility(true);
            CommentLoader.LoadPostComments(feedkey);
        }

        #endregion


        #region Properties

        public static InfowWindowController Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<InfowWindowController>();
                }

                return _instance;
            }
        }

        #endregion
    }
}