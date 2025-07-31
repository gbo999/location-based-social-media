using Firebase.Database;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;

namespace SocialApp
{
    public class FeedViewController : MonoBehaviour
    {

        [SerializeField]
        private TextMeshProUGUI TextBody = default;
        [SerializeField]
        private Text LikesCountBody = default;
        [SerializeField]
        private Text CommentsCountBody = default;
        [SerializeField]
        private Image ImageBody = default;
        [SerializeField]
        private RawImage VideoBody = default;
        [SerializeField]
        private Text DateBody = default;

        [SerializeField]
        private RectTransform TextRect = default;
        [SerializeField]
        private RectTransform ProfileRect = default;
        [SerializeField]
        private RectTransform ImageRect = default;
        [SerializeField]
        private RectTransform VideoRect = default;
        [SerializeField]
        private RectTransform BottomRect = default;
        [SerializeField]
        private RectTransform MainRect = default;
        [SerializeField]
        private Text ProfileUseeNameLabel = default;
        [SerializeField]
        private VideoPlayer VPlayer = default;
        [SerializeField]
        private GameObject PlayBtn = default;
        [SerializeField]
        private AvatarViewController AvatarView = default;
        [SerializeField]
        private bool CacheAvatar = default;
        [SerializeField]
        private Color LikedPostColor = default;
        [SerializeField]
        private Color UnLikedPostColor = default;
        [SerializeField]
        private Image LikeImage = default;
        [SerializeField]
        private OpenHyperlinks LinksChecker = default;
        [SerializeField]
        private float TextHeightOffset = default;

        private ScrollViewController ScrollView;

        private bool VideoFirstRun = true;
        private bool IsPostLiked = false;
        private bool CanLikePost = false;
        private Feed LoadedFeed;

        private bool IsActiveListeners;

        private DatabaseReference DRPostLikesCount;

        void Awake()
        {
            Init();
        }

        void Start()
        {
            UpdateUIRect();
        }

        private void OnDisable()
        {
            RemoveListeners();
            ClearView();
        }

        private void Init()
        {
            ScrollView = gameObject.GetComponentInParent<ScrollViewController>();
            HidePlayBtn();
        }

        private void AddListeners()
        {
            DRPostLikesCount = AppManager.FIREBASE_CONTROLLER.GetPostLikesCountReferense(LoadedFeed.Key);
            DRPostLikesCount.ValueChanged += OnLikesCountUpdated;
            IsActiveListeners = true;
        }

        private void RemoveListeners()
        {
            StopAllCoroutines();
            if (IsActiveListeners)
            {
                if (AppManager.FIREBASE_CONTROLLER != null)
                {
                    DRPostLikesCount.ValueChanged -= OnLikesCountUpdated;
                }
                IsActiveListeners = false;
            }
        }

        private void UpdateUIRect()
        {
            // update text rect
            TextRect.sizeDelta = new Vector2(TextRect.rect.width, TextBody.preferredHeight + TextHeightOffset);
            // update feed rect
            float _height = TextRect.rect.height + ProfileRect.rect.height + ImageRect.rect.height + VideoRect.rect.height + BottomRect.rect.height;
            MainRect.sizeDelta = new Vector2(MainRect.rect.width, _height);
        }

        public void LoadMedia(Feed _feed)
        {
            AvatarView.SetCacheTexture(CacheAvatar);
            ClearView();
            LoadedFeed = _feed;
            if (_feed.Type == FeedType.Image)
            {
                ImageBody.preserveAspect = true;
                float _bodyWidth = ImageRect.rect.width;
                float _imageWidth = (float)_feed.MediaWidth;
                float _imageHeight = (float)_feed.MeidaHeight;
                float _ratio = _imageWidth / _imageHeight;
                float _expectedHeight = _bodyWidth / _ratio;
                ImageRect.sizeDelta = new Vector2(_bodyWidth, _expectedHeight);
            }
            if (_feed.Type == FeedType.Video)
            {
                ImageBody.preserveAspect = true;
                float _bodyWidth = VideoRect.rect.width;
                float _imageWidth = (float)_feed.MediaWidth;
                float _imageHeight = (float)_feed.MeidaHeight;

                float _expectedHeight = _imageHeight * _bodyWidth / _imageWidth;
                VideoRect.sizeDelta = new Vector2(_bodyWidth, _expectedHeight);
            }
            // load view
            LoadView();
            UpdateUIRect();
            AddListeners();
        }

        private void ClearView()
        {
            StopAllCoroutines();
            HidePlayBtn();
            ImageRect.sizeDelta = new Vector2(ImageRect.rect.width, 1f);
            VideoRect.sizeDelta = new Vector2(VideoRect.rect.width, 1f);
            TextBody.text = string.Empty;
            Destroy(ImageBody.sprite);
            ImageBody.sprite = null;
            VPlayer.Stop();
            //VPlayer.url = string.Empty;
            StopAllCoroutines();
            VideoFirstRun = true;
            LoadedFeed = null;
            AvatarView.DisplayDefaultAvatar();
            LikesCountBody.text = "0";
            CommentsCountBody.text = "0";
        }

        private void LoadView()
        {
            LoadText();
            LoadDate();
            LoadUserData();
            LoadUserName();
            LoadLikes();
            LoadComments();
            if (LoadedFeed.Type == FeedType.Image)
                LoadGraphic();
            if (LoadedFeed.Type == FeedType.Video)
                LoadVideo();
        }

        // load user
        private void LoadUserData()
        {
            GetProfileImage();
        }

        // load Likes
        private void LoadLikes()
        {
            CanLikePost = false;

            AppManager.FIREBASE_CONTROLLER.IsLikedPost(LoadedFeed.Key, _isLike =>
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

        // load Likes
        private void LoadComments()
        {
            AppManager.FIREBASE_CONTROLLER.GetPostCommentsCount(LoadedFeed.Key, _count =>
            {
                CommentsCountBody.text = _count.ToString();
            });
        }

        // load date
        private void LoadDate()
        {
            DateBody.text = LoadedFeed.DateCreated;
        }

        // load text
        private void LoadText()
        {
            TextBody.text = LoadedFeed.BodyTXT;
            LinksChecker.CheckLinks();
        }

        // load image
        private void LoadGraphic()
        {
            StartCoroutine(OnLoadGraphic());
        }

        // load user name
        private void LoadUserName()
        {
            if (AppManager.USER_PROFILE.IsMine(LoadedFeed.OwnerID))
            {
                AppManager.USER_PROFILE.GetUserFullName(_userName =>
                {
                    ProfileUseeNameLabel.text = _userName;
                });
            }
            else
            {
                AppManager.FIREBASE_CONTROLLER.GetUserFullName(LoadedFeed.OwnerID, _userName =>
                {
                    ProfileUseeNameLabel.text = _userName;
                });
            }
        }

        private IEnumerator OnLoadGraphic()
        {
            string _url = LoadedFeed.ImageURL;
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
                    Texture2D _texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                    if (_texture != null)
                    {
                        ImageBody.sprite = Sprite.Create(_texture, new Rect(0.0f, 0.0f, _texture.width, _texture.height), new Vector2(0.5f, 0.5f), 100.0f);
                    }
                }
            }
        }

        // load video
        private void LoadVideo()
        {
            StartCoroutine(OnLoadVideo());
        }

        private IEnumerator OnLoadVideo()
        {
            string _url = LoadedFeed.VideoURL;

            if (!string.IsNullOrEmpty(LoadedFeed.ImageURL))
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
            }

            if (!AppManager.APP_SETTINGS.UseOriginVideoFile)
            {
                _url = string.Empty;
                bool _isGettedUrl = false;
                string _tempUrl = string.Empty;
                AppManager.FIREBASE_CONTROLLER.GetFeedVideoFileUrl(LoadedFeed.VideoFileName, (_gettedUrl =>
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
                ShowPlayBtn();
            }
        }

        private void GetProfileImage()
        {
            AvatarView.LoadAvatar(LoadedFeed.OwnerID);
        }



        public void OnClickVideo()
        {
            if (string.IsNullOrEmpty(VPlayer.url))
            {
                AppManager.VIEW_CONTROLLER.ShowPopupMSG(MessageCode.VideoProcessing);
            }

            if (!VPlayer.isPrepared)
            {
                VPlayer.Prepare();
                return;
            }
            StopAllCoroutines();
            StartCoroutine(CheckFeedVisibility());
            if (VPlayer.isPlaying)
            {
                VPlayer.Pause();
                ShowPlayBtn();
            }
            else
            {
                if (VideoFirstRun)
                {
                    VideoFirstRun = false;
                    VPlayer.frame = 0;
                }
                VideoBody.texture = VPlayer.texture;
                VPlayer.Play();
                HidePlayBtn();
            }
        }

        public void ClickLike()
        {
            if (CanLikePost)
            {
                if (IsPostLiked)
                {
                    AppManager.FIREBASE_CONTROLLER.UnLikPost(LoadedFeed.Key, success =>
                    {
                        if (success)
                        {
                            LoadLikes();
                        }
                    });
                }
                else
                {
                    AppManager.FIREBASE_CONTROLLER.LikPost(LoadedFeed.Key, success =>
                    {
                        if (success)
                        {
                            LoadLikes();
                        }
                    });
                }
            }
        }

        private void OnLikesCountUpdated(object sender, ValueChangedEventArgs args)
        {
            if (args.DatabaseError != null)
            {
                Debug.LogError(args.DatabaseError.Message);
                LikesCountBody.text = "0";
                return;
            }
            try
            {
                if (args.Snapshot.Value.ToString() == "0")
                {
                    LikesCountBody.text = "0";
                }
                else
                {
                    LikesCountBody.text = args.Snapshot.Value.ToString();
                }
            }
            catch (Exception)
            {
                LikesCountBody.text = "0";
            }
        }

        public void ClickComments()
        {
            AppManager.VIEW_CONTROLLER.ShowPostComments(LoadedFeed.Key);
        }

        public void ShowUserProfile()
        {
            string _userId = LoadedFeed.OwnerID;
            if (AppManager.USER_PROFILE.IsMine(_userId))
            {
                AppManager.NAVIGATION.ShowUserProfile();
            }
            else
            {
                AppManager.VIEW_CONTROLLER.HideNavigationGroup();
                AppManager.VIEW_CONTROLLER.ShowAnotherUserProfile(_userId);
            }
        }

        public bool IsMine()
        {
            return LoadedFeed.OwnerID == AppManager.Instance.auth.CurrentUser.UserId;
        }

        public string GetFeedKey()
        {
            return LoadedFeed.Key;
        }

        private void ShowPlayBtn()
        {
            PlayBtn.SetActive(true);
        }

        private void HidePlayBtn()
        {
            PlayBtn.SetActive(false);
        }

        private IEnumerator CheckFeedVisibility()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                if (VideoRect.IsRectTransformsOverlap(AppManager.VIEW_CONTROLLER.GetMainCamera()))
                {
                }
                else
                {
                    if (VPlayer.isPlaying)
                    {
                        VPlayer.Pause();
                        ShowPlayBtn();
                    }
                }

            }
        }

        public void ShowAdditionalMenu()
        {
            AppManager.VIEW_CONTROLLER.ShowFeedPopup( (actionType)=> { 
                if (actionType == FeedPopupAction.DELETE)
                {
                    if (IsMine())
                    {
                        AppManager.FIREBASE_CONTROLLER.RemovePost(LoadedFeed.Key, ()=>{
                            GetComponentInParent<ScrollViewController>().GetDataLoaderObject().GetComponent<FeedsDataLoader>().ResetLoader();
                        });
                    }
                    else
                    {
                        AppManager.VIEW_CONTROLLER.ShowPopupMSG(MessageCode.DeleteFeedOwnerError);
                    }
                }
            });
        }
    }
}