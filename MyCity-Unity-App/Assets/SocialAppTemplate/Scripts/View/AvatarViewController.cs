using UnityEngine;
using UnityEngine.UI;
using UniTools;
using System;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Collections;
using Yamanas.Infrastructure.Popups;

namespace SocialApp
{
    public class AvatarViewController : MonoBehaviour
    {
        #region Fields

        [SerializeField] private Image AvatarImage = default;
        [SerializeField] private RectTransform AvatarRect = default;
        private float AvatarSize = default;
        [SerializeField] private float DefaultAvatarSize = default;
        [SerializeField] private bool CacheTexture = default;

        [SerializeField] private Image _hatImage;

        [SerializeField] private GameObject _avatarSpinner;

        [SerializeField] private Color _blankColor;

        private string CurrentUserID;

        private string CurrentGroupID;

        private Model3D CurrentModel;

        #endregion


        public void OnImageClick()
        {
            PopupSystem.Instance.ShowPopup(PopupType.Profile, CurrentUserID);
        }


        private void OnEnable()
        {
            if (PopupSystem.Instance.IsVisible(PopupType.PostComments) ||
                PopupSystem.Instance.IsVisible(PopupType.Chat))
            {
                AvatarSize = 170;
            }
            else if (PopupSystem.Instance.IsVisible(
                PopupType.Leaderboard))
            {
                AvatarSize = 50;
            }

            _avatarSpinner.SetActive(true);
        }


        public void LoadBigAvatar(string _id)
        {
            ClearData();
            CurrentUserID = _id;
            GetProfileImage(ImageSize.Size_512);
        }

        public void LoadAvatar(string _id)
        {
            ClearData();
            CurrentUserID = _id;

            AppManager.FIREBASE_CONTROLLER.GetXPLevel(CurrentUserID,
                xpLevel =>
                {
                    Debug.Log($"xp is {xpLevel}");
                    //_hatImage.color = Color.white;

                    if (xpLevel == 1 || xpLevel == 2 || xpLevel == 3 || xpLevel == 6)
                    {
                        _hatImage.sprite = Resources.Load<Sprite>($"ProductsSprites/Level{xpLevel}");

                        _hatImage.rectTransform.rotation = Quaternion.Euler(0, 0, 41.7177773f);
                    }

                    else if (xpLevel == 4 || xpLevel == 5)
                    {
                        _hatImage.sprite = Resources.Load<Sprite>($"ProductsSprites/Level{xpLevel}");

                        _hatImage.rectTransform.rotation = Quaternion.Euler(0, 0, 354.467682f);
                    }

                    else
                    {
                        _hatImage.color = _blankColor;
                    }
                });
            GetProfileImage(ImageSize.Size_128);
        }

        /*  public void LoadThumb(Model3D _model)
          {
              ClearData();
              CurrentModel = _model;
              GetThumb(ImageSize.Size_256);
          }*/

        private void GetThumb(ImageSize size_256)
        {
            GetProfileImageRequest _request = new GetProfileImageRequest();
            _request.groupID = CurrentGroupID;
            _request.Size = size_256;

            AppManager.FIREBASE_CONTROLLER.GetIcon(_request, OnProfileImageGetted);
        }

        public void LoadIcon(string _id)
        {
            ClearData();
            CurrentGroupID = _id;
            GetIcon(ImageSize.Size_256);
        }

        private void GetIcon(ImageSize size_256)
        {
            GetProfileImageRequest _request = new GetProfileImageRequest();
            _request.groupID = CurrentGroupID;
            _request.Size = size_256;

            AppManager.FIREBASE_CONTROLLER.GetIcon(_request, OnProfileImageGetted);
        }


        private void OnDisable()
        {
            AvatarImage.sprite = null;
        }

        private void ClearData()
        {
            DisplayDefaultAvatar();
            CurrentUserID = string.Empty;
            ;
        }

        private void GetProfileImage(ImageSize _size)
        {
            GetProfileImageRequest _request = new GetProfileImageRequest();
            _request.Id = CurrentUserID;
            _request.Size = _size;
            /*if (CacheTexture || AppManager.USER_PROFILE.IsMine(CurrentUserID))
            {
                AppManager.FIREBASE_CONTROLLER.GetProfileImageUrl(_request, OnProfileImageUrlGetted);
            }*/
            //  else
            //  {
            AppManager.FIREBASE_CONTROLLER.GetProfileImage(_request, OnProfileImageGetted);
            // }
        }


        public void LoadThumb(Model3D model)
        {
            CurrentModel = model;

            StartCoroutine(OnLoadGraphic());
        }


        private IEnumerator OnLoadGraphic()
        {
            string _url = CurrentModel.thumbnailUrl;
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
                        AvatarImage.sprite = Sprite.Create(_texture,
                            new Rect(0.0f, 0.0f, _texture.width, _texture.height), new Vector2(0.5f, 0.5f), 100.0f);
                    }
                }
            }
        }


        public void OnProfileImageGetted(GetProfileImageCallback _callback)
        {
            Debug.Log("is callbak " + _callback.IsSuccess);

            if (_callback.IsSuccess)
            {
                _avatarSpinner.SetActive(false);

                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(_callback.ImageBytes);
                AvatarImage.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f), 100.0f);

                ResizeAvarar(AvatarSize);
            }
            else
            {
                _avatarSpinner.SetActive(false);
                DisplayDefaultAvatar();
            }
        }

        public void OnProfileImageUrlGetted(GetProfileImageCallback _callback)
        {
            if (_callback.IsSuccess)
            {
                if (_callback.DownloadUrl != null)
                {
                    CoroutineExecuter _ce = new CoroutineExecuter();
                    ImageService _is = new ImageService(_ce);
                    _is.DownloadOrLoadTexture(_callback.DownloadUrl, _texture =>
                    {
                        if (_texture != null)
                        {
                            AvatarImage.sprite = Sprite.Create(_texture,
                                new Rect(0.0f, 0.0f, _texture.width, _texture.height), new Vector2(0.5f, 0.5f), 100.0f);
                            ResizeAvarar(AvatarSize);

                            Debug.Log(" avatar not empty");
                        }
                        else
                        {
                            DisplayDefaultAvatar();
                            Debug.Log("empty avatar");
                        }
                    });
                }
                else
                {
                    DisplayDefaultAvatar();
                    Debug.Log("empty avatar");
                }
            }
            else
            {
                DisplayDefaultAvatar();
                Debug.Log("empty avatar");
            }
        }

        public void DisplayDefaultAvatar()
        {
            if (AppManager.APP_SETTINGS != null)
            {
                Texture2D _defaultAvatar = AppManager.APP_SETTINGS.DefaultAvatarTexture;
                AvatarImage.sprite = Sprite.Create(_defaultAvatar,
                    new Rect(0.0f, 0.0f, _defaultAvatar.width, _defaultAvatar.height), new Vector2(0.5f, 0.5f), 100.0f);
                ResizeAvarar(DefaultAvatarSize);
            }
        }

        public void DisplayGroupChatAvatar()
        {
            if (AppManager.APP_SETTINGS != null)
            {
                Texture2D _defaultAvatar = AppManager.APP_SETTINGS.DefaultGroupChatTexture;
                AvatarImage.sprite = Sprite.Create(_defaultAvatar,
                    new Rect(0.0f, 0.0f, _defaultAvatar.width, _defaultAvatar.height), new Vector2(0.5f, 0.5f), 100.0f);
                ResizeAvarar(AppSettings.DefaultGroupChatIconSize);
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

        public void SetCacheTexture(bool _value)
        {
            CacheTexture = _value;
        }
    }
}