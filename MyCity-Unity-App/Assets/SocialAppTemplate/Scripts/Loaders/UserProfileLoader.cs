using System;
using System.Collections;
using System.Collections.Generic;
using InfinityCode.OnlineMapsDemos;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using UnityEngine.Networking;
using Yamanas.Infrastructure.Popups;
using Object = UnityEngine.Object;

namespace SocialApp
{
    public class UserProfileLoader : MonoBehaviour, IPopup<string>
    {
        [SerializeField] private ImageSize ProfileImageSize = ImageSize.Size_512;
        [SerializeField] private ProfileType CurrentProfileType = ProfileType.My;
        [SerializeField] private List<ImageSize> SizesToUpload = new List<ImageSize>();
        [SerializeField] private Image ProfileImage = default;
        [SerializeField] private RectTransform ProfileImageRect = default;
        [SerializeField] private TMP_InputField NewsInput = default;
        [SerializeField] private TMP_Text UserNameLabel = default;
        [SerializeField] private Text FriendsCountLabel = default;
        [SerializeField] private GameObject AddToFriendBtn = default;
        [SerializeField] private GameObject SendMessageBtn = default;
        [SerializeField] private GameObject SendPostBlocker = default;
        [SerializeField] private GameObject[] FriendsAvatarObjects = default;
        [SerializeField] private FeedDadaUoloader FeedUploader = default;

        [SerializeField] private GameObject _chatButton;
        
        private string CurrentUserID;

        private float DefaultProfileWidth;
        private float DefaultProfileHeight;

        private List<string> _products;

        [SerializeField] private GameObject _productsHolder;

        [SerializeField] private Object _prodcutPrefab;

        [SerializeField] private TMP_Text _currecnyText;

        [SerializeField] private TMP_Text _scoreText;

        [SerializeField] private Image _customImage;

        [SerializeField] private Color _blankColor;
        
        //[SerializeField] private TMP_Text _xpText;

        //  private UIBubblePopup bubblePopup;


        private void Awake()
        {
            /*DefaultProfileWidth = ProfileImageRect.rect.width;
            DefaultProfileHeight = ProfileImageRect.rect.height;*/
        }

        /*private void OnEnable()
        {
            
            
            if (CurrentProfileType == ProfileType.My)
            {
                LoadMyInfo();
            }
            else
            {
                ProfileImageRect.sizeDelta = new Vector2(DefaultProfileWidth, DefaultProfileHeight);
            }
        }*/

        private void LoadMyInfo()
        {
            //CurrentUserID = AppManager.Instance.auth.CurrentUser.UserId;
            /*if (!AppManager.USER_PROFILE.PROFILE_IMAGE_LOADED)
            {
                Debug.Log("before get profile image");
                
                ProfileImageRect.sizeDelta = new Vector2(DefaultProfileWidth, DefaultProfileHeight);
                DisplayDefaultAvatar();
                GetProfileImage();

                
            }*/


            Debug.Log("before get profile image");

            ProfileImageRect.sizeDelta = new Vector2(DefaultProfileWidth, DefaultProfileHeight);
            DisplayDefaultAvatar();
            GetProfileImage();

            AppManager.FIREBASE_CONTROLLER.GetUserFullName(CurrentUserID,
                userName => { UserNameLabel.text = userName; });
            
            AppManager.FIREBASE_CONTROLLER.GetCurrecny(CurrentUserID,
                currency => { _currecnyText.text = $"{currency}"; });
            
            AppManager.FIREBASE_CONTROLLER.GetXPLevel(CurrentUserID,
                xpLevel =>
                {
                    _customImage.sprite = Resources.Load<Sprite>($"ProductsSprites/Level{xpLevel}");
                    _customImage.color = Color.white;

                    if (xpLevel != 4 && xpLevel != 5)
                    {
                        _customImage.rectTransform.rotation = Quaternion.Euler(0,0,56.7353592f);
                    }
                    else
                    {
                        _customImage.rectTransform.rotation = Quaternion.Euler(0,0,3.10316801f);
                    }

                    if (xpLevel == 0||xpLevel==null)
                    {
                        _customImage.color = _blankColor;
                    }
                    
                });
            
            AppManager.FIREBASE_CONTROLLER.GetScore(CurrentUserID,
                score => { _scoreText.text = $"{score}"; });

            
            
            
            
            AppManager.FIREBASE_CONTROLLER.GetProducts(CurrentUserID,list=>
            {
                _products = list;
                foreach (var product in _products)
                {
                    Debug.Log($"inside get products {product}");

                    try
                    {
                        
                        UnityMainThreadDispatcher.Instance().Enqueue(()=>
                        {     var product_pref= Instantiate(_prodcutPrefab, _productsHolder.transform) as GameObject;
                            product_pref.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>($"ProductsSprites/{product}");
                            product_pref.transform.GetChild(1).GetComponent<TMP_Text>().text = product;
                        });
                        
                   
                    }
                    catch (Exception e)
                    {
                       Debug.Log($"error is {e.ToString()}");
                    }
                    
                }
                
            });
            //NewsInput.text = string.Empty;
            /*AppManager.USER_PROFILE.GetUserFullName(_userName =>
            {
                UserNameLabel.text = _userName;
            });*/
            // GetFriendList();
        }

        public void LoadUserInfo(string _id)
        {
            CurrentUserID = _id;
            DisplayDefaultAvatar();
            GetProfileImage();
            CheckFriendsList();
            UserNameLabel.text = string.Empty;
            //   NewsInput.text = string.Empty;
            AppManager.FIREBASE_CONTROLLER.GetUserFullName(CurrentUserID,
                (_userName => { UserNameLabel.text = _userName; }));
            //  GetFriendList();
        }

        private void GetFriendList()
        {
            FriendsCountLabel.text = string.Empty;
            foreach (GameObject _go in FriendsAvatarObjects)
            {
                _go.SetActive(false);
            }

            UsersQuery _usersQuery = new UsersQuery();
            _usersQuery.startIndex = 0;
            _usersQuery.endIndex = 6;
            _usersQuery.callback = OnFriendsLoaded;
            _usersQuery.forward = true;
            _usersQuery.ownerID = CurrentUserID;
            _usersQuery.Type = FriendsTabState.Friend;
            AppManager.FIREBASE_CONTROLLER.GetFriendsAt(_usersQuery);

            AppManager.FIREBASE_CONTROLLER.GetUserFriendsCount(CurrentUserID,
                _count => { FriendsCountLabel.text = "Friends (" + _count + ")"; });
        }

        public void OnFriendsLoaded(UsersCallback _callback)
        {
            if (_callback.IsSuccess)
            {
                for (int i = 0; i < _callback.users.Count; i++)
                {
                    FriendsAvatarObjects[i].SetActive(true);
                    FriendsAvatarObjects[i].GetComponent<AvatarViewController>().LoadAvatar(_callback.users[i].UserID);
                }
            }
        }

        private void CheckFriendsList()
        {
            AddToFriendBtn.SetActive(false);
            SendMessageBtn.SetActive(false);
            SendPostBlocker.SetActive(true);
            AppManager.FIREBASE_CONTROLLER.CanAddToFriend(CurrentUserID, _canAdd =>
            {
                if (_canAdd)
                {
                    AddToFriendBtn.SetActive(true);
                }
                else
                {
                    AppManager.FIREBASE_CONTROLLER.IsInFriendsList(CurrentUserID, _isFriend =>
                    {
                        if (_isFriend)
                        {
                            SendMessageBtn.SetActive(true);
                            SendPostBlocker.SetActive(false);
                        }
                    });
                }
            });
        }

        public void SelectPhotoFromGallery()
        {
#if !UNITY_EDITOR
		NativeGallery.GetImageFromGallery(OnImagePicked);
        return;
#else
            string _path = EditorUtility.OpenFilePanel("Overwrite with png", "", "png");

            if (string.IsNullOrEmpty(_path))
                return;
            StartCoroutine(UploadAvatar(_path));
#endif
        }


        public void selectPhotoFromURL(string url)
        {
            StartCoroutine(onImageFromUrl(url));
        }

        private IEnumerator onImageFromUrl(string url)
        {
            Texture2D _texture = new Texture2D(2, 2);
            AppManager.VIEW_CONTROLLER.ShowLoading();


            System.Uri uri = new System.Uri(url);


            UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(uri);

            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(uwr.error);
            }
            else
            {
                // Get downloaded asset bundle
                _texture = DownloadHandlerTexture.GetContent(uwr);
            }

            for (int i = 0; i < SizesToUpload.Count; i++)
            {
                Debug.Log("for loop upload avatar");


                ImageSize _size = SizesToUpload[i];
                bool isFinishUpload = false;
                bool isSuccess = false;
                UploadImageRequest uploadRequest = new UploadImageRequest();
                uploadRequest.OwnerId = AppManager.Instance.auth.CurrentUser.UserId;
                ResizeTexture(_texture, _size);
                byte[] uploadBytes =
                    ImageConversion.EncodeToJPG(_texture, AppManager.APP_SETTINGS.UploadImageQuality);
                uploadRequest.ImageBytes = uploadBytes;
                uploadRequest.Size = _size;
                AppManager.FIREBASE_CONTROLLER.UploadAvatar(uploadRequest, (_callback =>
                        {
                            isFinishUpload = _callback.IsFinish;
                            isSuccess = _callback.IsSuccess;
                        }
                    ));
                while (!isFinishUpload)
                {
                    yield return null;
                }

                if (!isSuccess)
                {
                    Debug.Log("avatar not success");

                    AppManager.VIEW_CONTROLLER.HideLoading();
                    AppManager.VIEW_CONTROLLER.ShowPopupMSG(MessageCode.FailedUploadImage);
                    yield break;
                }
            }

            AppManager.VIEW_CONTROLLER.HideLoading();
            // GetProfileImage();

            /*
            bubblePopup = FindObjectOfType<UIBubblePopup>();


            bubblePopup.UploadMapAvatar();*/
        }


        public void SelectPhotoFromCamera()
        {
            NativeCamera.TakePicture(OnImageTaken);
        }

        private void OnImagePicked(string _path)
        {
            if (string.IsNullOrEmpty(_path))
                return;
            StartCoroutine(UploadAvatar(_path));
        }

        private void OnImageTaken(string _path)
        {
            if (string.IsNullOrEmpty(_path))
                return;
            StartCoroutine(UploadAvatar(_path));
        }

        public void ResizeTexture(Texture2D _texture, ImageSize _size)
        {
            if (_size != ImageSize.Origin)
            {
                int _width = _texture.width;
                int _height = _texture.height;
                if (_width > _height)
                {
                    if (_width > (int) _size)
                    {
                        float _delta = (float) _width / (float) ((int) _size);
                        _height = Mathf.FloorToInt((float) _height / _delta);
                        _width = (int) _size;
                    }
                }
                else
                {
                    if (_height > (int) _size)
                    {
                        float _delta = (float) _height / (float) ((int) _size);
                        _width = Mathf.FloorToInt((float) _width / _delta);
                        _height = (int) _size;
                    }
                }

                TextureScale.Bilinear(_texture, _width, _height);
            }
        }

        private IEnumerator UploadAvatar(string _path)
        {
            Debug.Log("inside upload avatar");


            AppManager.VIEW_CONTROLLER.ShowLoading();

#if UNITY_EDITOR

            byte[] fileBytes = System.IO.File.ReadAllBytes(_path);

            Texture2D _texture = new Texture2D(2, 2);

            _texture.LoadImage(fileBytes);

            Debug.Log("unity edior upload avatar");

#elif !UNITY_EDITOR && UNITY_ANDROID
            Texture2D _texture = NativeGallery.LoadImageAtPath(_path, -1, false, false);
#endif


            for (int i = 0; i < SizesToUpload.Count; i++)
            {
                Debug.Log("for loop upload avatar");


                ImageSize _size = SizesToUpload[i];
                bool isFinishUpload = false;
                bool isSuccess = false;
                UploadImageRequest uploadRequest = new UploadImageRequest();
                uploadRequest.OwnerId = AppManager.Instance.auth.CurrentUser.UserId;
                ResizeTexture(_texture, _size);
                byte[] uploadBytes = ImageConversion.EncodeToJPG(_texture, AppManager.APP_SETTINGS.UploadImageQuality);
                uploadRequest.ImageBytes = uploadBytes;
                uploadRequest.Size = _size;
                AppManager.FIREBASE_CONTROLLER.UploadAvatar(uploadRequest, (_callback =>
                        {
                            isFinishUpload = _callback.IsFinish;
                            isSuccess = _callback.IsSuccess;
                        }
                    ));
                while (!isFinishUpload)
                {
                    yield return null;
                }

                if (!isSuccess)
                {
                    Debug.Log("avatar not success");

                    AppManager.VIEW_CONTROLLER.HideLoading();
                    AppManager.VIEW_CONTROLLER.ShowPopupMSG(MessageCode.FailedUploadImage);
                    yield break;
                }
            }

            AppManager.VIEW_CONTROLLER.HideLoading();
            GetProfileImage();

            /*bubblePopup = FindObjectOfType<UIBubblePopup>();


            bubblePopup.UploadMapAvatar();*/


            //f   FeedUploader.PostProfileUpdate(_path);
        }

        private void GetProfileImage()
        {
            Debug.Log("get profile image func");

            GetProfileImageRequest _request = new GetProfileImageRequest();
            _request.Id = CurrentUserID;
            _request.Size = ProfileImageSize;
            AppManager.FIREBASE_CONTROLLER.GetProfileImage(_request, OnProfileImageGetted);
        }

        public void OnProfileImageGetted(GetProfileImageCallback _callback)
        {
            if (_callback.IsSuccess)
            {
                Debug.Log("callback profile success");

                AppManager.USER_PROFILE.PROFILE_IMAGE_LOADED = true;
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(_callback.ImageBytes);
                ProfileImage.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f), 100.0f);
                UpdateImageRect();
            }
            else
            {
                Debug.Log("callback failed");

                DisplayDefaultAvatar();
            }
        }

        private void DisplayDefaultAvatar()
        {
            Texture2D texture = AppManager.APP_SETTINGS.DefaultAvatarTexture;
            ProfileImage.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height),
                new Vector2(0.5f, 0.5f), 100.0f);
            UpdateImageRect();
        }

        public void ShowUserFriends()
        {
            AppManager.VIEW_CONTROLLER.HideNavigationGroup();
            if (CurrentProfileType == ProfileType.My)
            {
                AppManager.VIEW_CONTROLLER.ShowUserFriend(AppManager.Instance.auth.CurrentUser.UserId);
            }
            else if (CurrentProfileType == ProfileType.Another)
            {
                AppManager.VIEW_CONTROLLER.ShowUserFriend(CurrentUserID);
            }
        }

        public void ShowMessagesWithUser()
        {
            MessageGroupInfo _groupInfo = new MessageGroupInfo();
            _groupInfo.ChatID =
                AppManager.FIREBASE_CONTROLLER.GetUserMessageKey(AppManager.Instance.auth.CurrentUser.UserId,
                    CurrentUserID);
            _groupInfo.Users = new List<string>();
            _groupInfo.Users.Add(AppManager.Instance.auth.CurrentUser.UserId);
            _groupInfo.Users.Add(CurrentUserID);
            Debug.Log("is current what " + CurrentUserID);
            _groupInfo.Type = MessageType.Messanging;
            AppManager.FIREBASE_CONTROLLER.CheckAndAddNewChatInfo(_groupInfo);
            PopupSystem.Instance.ShowPopup(PopupType.Chat, _groupInfo);

            // AppManager.VIEW_CONTROLLER.ShowMessagingWith(_groupInfo);
        }

        private void UpdateImageRect()
        {
            ProfileImage.preserveAspect = true;
            float _bodyWidth = ProfileImageRect.rect.width;
            float _bodyHeight = ProfileImageRect.rect.height;
            float _imageWidth = (float) ProfileImage.sprite.texture.width;
            float _imageHeight = (float) ProfileImage.sprite.texture.height;
            float _ratio = _imageWidth / _imageHeight;
            if (_imageWidth > _imageHeight)
            {
                _ratio = _imageHeight / _imageWidth;
            }

            float _expectedHeight = _bodyWidth / _ratio;
            if (_imageWidth > _imageHeight)
            {
                ProfileImageRect.sizeDelta = new Vector2(_expectedHeight, _bodyHeight);
            }
            else
            {
                ProfileImageRect.sizeDelta = new Vector2(_bodyWidth, _expectedHeight);
            }
        }

        public void AddToFriend()
        {
            if (string.IsNullOrEmpty(CurrentUserID))
                return;
            AppManager.FIREBASE_CONTROLLER.AddToFriends(CurrentUserID, OnAddedToFriend);
        }

        private void OnAddedToFriend()
        {
            AddToFriendBtn.SetActive(false);
        }

        public void SetData(string userID)
        {
            CurrentUserID = userID;

            if (CurrentUserID == AppManager.Instance.auth.CurrentUser.UserId)
            {
                _chatButton.SetActive(false);
            }
            else
            {
                _chatButton.SetActive(true);
            }
            
            LoadMyInfo();
        }
    }

    public enum ProfileType
    {
        My,
        Another
    }

    public class UploadImageRequest
    {
        public byte[] ImageBytes;
        public string OwnerId;
        public string groupId;
        public ImageSize Size;
    }

    public class UploadImageCallBack
    {
        public bool IsFinish = false;
        public bool IsSuccess = false;
    }

    public class GetProfileImageRequest
    {
        public string Id;
        public ImageSize Size;
        public string groupID;
    }

    public class GetProfileImageCallback
    {
        public Feed feed;

        public GameObject pref;
        public bool IsSuccess;
        public byte[] ImageBytes;
        public string DownloadUrl;
    }
}