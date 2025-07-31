using System;
using System.Collections;
using Firebase.Database;
using SocialApp;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Yamanas.Infrastructure.Popups;
using Yamanas.Scripts.MapLoader.Gamification;

namespace Yamanas.Scripts.MainView
{
    public class MainViewAvatar : MonoBehaviour
    {
        #region Fields

        [SerializeField] private Image _avatarImage;

        [SerializeField] private Texture2D _defaultMapAvatar;

        [SerializeField] private float mapAvatarDefaultSize;

        [SerializeField] private RectTransform _avatarRect = default;

        [SerializeField] private GameObject _loadingSpinner;

        [SerializeField] private Image _progressIamge;

        [SerializeField] private TMP_Text _scoreText;

        [SerializeField] private TMP_Text _currecnyText;

        [SerializeField] private TMP_Text _XPLevel;

        [SerializeField] private Image _customImage;

        [SerializeField] private Color _color;

        private DatabaseReference _scoreRef;

        private DatabaseReference _currecnyRef;

        private DatabaseReference _XPRef;

        private double res;

        private double CurrRes;

        #endregion

        #region Methods

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                FeatherFactory.Instance.CreateFive();
            }
           
        }

        public void OnCoinClicked()
        {
            PopupSystem.Instance.ShowPopup(PopupType.Shop, CurrRes.ToString());
        }

        private void Start()
        {
          // AppManager.FIREBASE_CONTROLLER.setXP(1);
            UploadMapAvatar();

            res = 0;
            CurrRes = 0;

            _currecnyRef = AppManager.FIREBASE_CONTROLLER.GetCurrentCurrency();
            _currecnyRef.ValueChanged += OnCurrencyChanged;
            _scoreRef = AppManager.FIREBASE_CONTROLLER.GetCurrentScore();
            _scoreRef.ValueChanged += OnScoreChanged;
            _XPRef = AppManager.FIREBASE_CONTROLLER.GetXP();
            _XPRef.ValueChanged += OnXPChanged;
        }

        private void OnXPChanged(object sender, ValueChangedEventArgs e)
        {
            if (e.Snapshot.Value != null)
            {
                int value = Convert.ToInt32(e.Snapshot.Value);

                _XPLevel.text = e.Snapshot.Value.ToString();
                _customImage.sprite = Resources.Load<Sprite>($"ProductsSprites/Level{e.Snapshot.Value}");
                _customImage.color = Color.white;

                if (value != 4 && value != 5)
                {
                    _customImage.rectTransform.rotation = Quaternion.Euler(0, 0, 51.7116165f);
                }
                else
                {
                    _customImage.rectTransform.rotation = Quaternion.Euler(0f, 0f, 7.38043022f);
                }
            }
            else
            {
                _XPLevel.text = 0.ToString();
                _customImage.color = _color;
            }
        }

        private void OnCurrencyChanged(object sender, ValueChangedEventArgs args)
        {
            if (args.DatabaseError != null)
            {
                Debug.LogError(args.DatabaseError.Message);
                return;
            }


            StartCoroutine(addCurrecny(args.Snapshot.Value));
        }

        IEnumerator addCurrecny(object currency)
        {
            int currencyRes;
            if (currency == null)
            {
                currencyRes = 0;
            }

            else
            {
                currencyRes = Convert.ToInt32(currency);
            }

            Debug.Log($"currecny is {currencyRes}");


            while (CurrRes < currencyRes)
            {
                CurrRes += 1;
                _currecnyText.text = $"{CurrRes}k";

                yield return new WaitForSeconds(0.1f);
            }

            CurrRes = currencyRes;
        }

        private void OnScoreChanged(object sender, ValueChangedEventArgs args)
        {
            if (args.DatabaseError != null)
            {
                Debug.LogError(args.DatabaseError.Message);
                return;
            }


            AppManager.FIREBASE_CONTROLLER.GetXPLevel(AppManager.Instance.auth.CurrentUser.UserId,
                i => { StartCoroutine(addSCore(args.Snapshot.Value, i)); });
        }

        IEnumerator addSCore(object score, int xp)
        {
            
            int scoreres;
            if (score == null)
            {
                scoreres = 0;
            }

            else
            {
                scoreres = Mathf.Clamp(Convert.ToInt32(score)-xp*100, 0, 100);
            }

            Debug.Log($"score is {scoreres}");


            while (res < Math.Round(Convert.ToDouble(scoreres) / 100, 2))
            {
                _progressIamge.fillAmount += 0.1f * Time.deltaTime;
                res += 0.1f * Time.deltaTime;
                _scoreText.text = $"{System.Math.Floor(res * 100)}/100";

                yield return null;
            }

            res = Math.Round(Convert.ToDouble(scoreres) / 100, 2);
        }

        public void OnAvatarPictureClick()
        {
            PopupSystem.Instance.ShowPopup(PopupType.Profile, AppManager.Instance.auth.CurrentUser.UserId);
            //AppManager.FIREBASE_CONTROLLER.AddCurrencyAndPoints(5);
        }

        
        
        public void UploadMapAvatar()
        {
            if (AppManager.Instance.user != null)
            {
                GetProfileImageRequest _request = new GetProfileImageRequest();


                _request.Id = AppManager.Instance.auth.CurrentUser.UserId;
                _request.Size = ImageSize.Size_512;
                AppManager.FIREBASE_CONTROLLER.GetProfileImage(_request, OnMapAvatar);
            }
        }

        public void OnMapAvatar(GetProfileImageCallback _callback)
        {
            AppManager.VIEW_CONTROLLER.HideLoading();


            _loadingSpinner.SetActive(false);

            if (_callback.IsSuccess)
            {
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(_callback.ImageBytes);
                _avatarImage.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f), 100.0f);


                //  ResizeAvararMap(MapAvatarSize);
            }
            else
            {
                DisplayDefaultMapAvatar();
            }
        }

        private void DisplayDefaultMapAvatar()
        {
            if (AppManager.APP_SETTINGS != null)
            {
                _avatarImage.sprite = Sprite.Create(_defaultMapAvatar,
                    new Rect(0.0f, 0.0f, _defaultMapAvatar.width, _defaultMapAvatar.height), new Vector2(0.5f, 0.5f),
                    100.0f);
                ResizeAvarar(mapAvatarDefaultSize);
            }
        }

        private void ResizeAvarar(float _size)
        {
            float _bodyWidth = _size;
            float _bodyHeight = _size;
            float _imageWidth = (float) _avatarImage.sprite.texture.width;
            float _imageHeight = (float) _avatarImage.sprite.texture.height;
            float _ratio = _imageWidth / _imageHeight;
            if (_imageWidth > _imageHeight)
            {
                _ratio = _imageHeight / _imageWidth;
            }

            float _expectedHeight = _bodyWidth / _ratio;
            if (_imageWidth > _imageHeight)
            {
                _avatarRect.sizeDelta = new Vector2(_expectedHeight, _bodyHeight);
            }
            else
            {
                _avatarRect.sizeDelta = new Vector2(_bodyWidth, _expectedHeight);
            }
        }

        #endregion
    }
}