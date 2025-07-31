using System;
using Firebase.Database;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniTools;

namespace SocialApp
{
    public class MessageViewController : MonoBehaviour
    {
        [SerializeField]
        private Image BubleImage = default;
        [SerializeField]
        private Image ContentImage = default;
        [SerializeField]
        private TextMeshProUGUI BodyText = default;
        [SerializeField]
        private TMP_Text UserNameText = default;
        [SerializeField]
        private TMP_Text DateText = default;

        [SerializeField]
        private RectTransform TextRect = default;
        [SerializeField]
        private RectTransform BubleRect = default;
        [SerializeField]
        private RectTransform ContentRect = default;
        [SerializeField]
        private RectTransform MainRect = default;
        [SerializeField]
        private RectTransform ProfileRect = default;
        [SerializeField]
        private float StartBubbleWidth = default;

        [SerializeField]
        private Color UserBubbleColor = default;
        [SerializeField]
        private Color MyBubbleColor = default;
        [SerializeField]
        private AvatarViewController AvatarView = default;
        [SerializeField]
        private OpenHyperlinks LinksChecker = default;
        [SerializeField]
        private bool CacheAvatar = default;


        private Vector2 TextOffsetMin;
        private Vector2 BubbleOffsetMin;
        private Vector2 BubbleOffsetMax;
        private float MaxContentWidth = 600f;

        
        private Message CurrentMessage;

        
        //**** comment likes******
        
        private bool CanLikeComment = false;
        private bool IsCommentLiked = false;
        [SerializeField] private Image LikeImage = default;
        [SerializeField] private TMP_Text LikesCountBody; 
        [SerializeField] private Color LikedPostColor = default;
        [SerializeField] private Color UnLikedPostColor =default;
        private DatabaseReference DRCommentLikesCount;
        private bool IsActiveListeners;
        
        //****************************
        
        private void Awake()
        {
            SaveResetValue();
        }

        private void SaveResetValue()
        {
            TextOffsetMin = TextRect.offsetMin;
            BubbleOffsetMin = BubleRect.offsetMin;
            BubbleOffsetMax = BubleRect.offsetMax;
        }

        public void LoadMedia(Message _msg)
        {
            AvatarView.SetCacheTexture(CacheAvatar);
            ResetRects();
            CurrentMessage = _msg;
            if (_msg.Type == ContentMessageType.TEXT)
            {
                LoadText();
            }
            else if (_msg.Type == ContentMessageType.IMAGE)
            {
                LoadContent();
            }
            LoadGraphics();
         //   UpdateUIRect();
         
         
            GetProfileImage();
            LoadLikes();
            AddListeners();
        }
        
        
        

        public void LoadGraphics()
        {
            if (isMine())
            {
                BubleImage.color = MyBubbleColor;
            }
            else
            {
                BubleImage.color = UserBubbleColor;
            }
        }

        public void LoadText()
        {
            BodyText.text = CurrentMessage.BodyTXT;
            UserNameText.text = CurrentMessage.FullName;
            DateText.text = CurrentMessage.DateCreated;
            LinksChecker.CheckLinks();
            ContentImage.gameObject.SetActive(false);
        }

        public void LoadContent()
        {
            UserNameText.text = CurrentMessage.FullName;
            DateText.text = CurrentMessage.DateCreated;
            ContentImage.gameObject.SetActive(true);
            ContentImage.color = Color.grey;
            float width = CurrentMessage.MediaInfo.ContentWidth;
            float height = CurrentMessage.MediaInfo.ContentHeight;
            if (width > MaxContentWidth)
            {
                height = MaxContentWidth * height / width;
                width = MaxContentWidth;
            }

            ContentRect.sizeDelta = new Vector2(width, height);
            ContentRect.anchoredPosition = new Vector2(ContentRect.anchoredPosition.x, -height/2f);

            if (!string.IsNullOrEmpty(CurrentMessage.MediaInfo.ContentURL))
            {
                CoroutineExecuter _ce = new CoroutineExecuter();
                ImageService _is = new ImageService(_ce);
                _is.DownloadOrLoadTexture(CurrentMessage.MediaInfo.ContentURL, _texture =>
                {
                    if (_texture != null)
                    {
                        ContentImage.color = Color.white;
                        ContentImage.sprite = Sprite.Create(_texture, new Rect(0.0f, 0.0f, _texture.width, _texture.height), new Vector2(0.5f, 0.5f), 100.0f);
                    }
                });
            }
        }

        private void ResetRects()
        {
            TextRect.offsetMin = TextOffsetMin;
            BubleRect.offsetMin = BubbleOffsetMin;
            BubleRect.offsetMax = BubbleOffsetMax;
        }

        private bool isMine()
        {
            return CurrentMessage.UserID == AppManager.Instance.auth.CurrentUser.UserId;
        }

        private void UpdateUIRect()
        {
            if (CurrentMessage.Type == ContentMessageType.TEXT)
            {
                // update buble text rect
                float txtPreferredWidth = BodyText.preferredWidth;
                if (txtPreferredWidth > TextRect.rect.width)
                    txtPreferredWidth = TextRect.rect.width;
                TextRect.offsetMin = new Vector2(TextRect.offsetMin.x, TextRect.offsetMin.y - BodyText.preferredHeight + (float)BodyText.fontSize);
                BubleRect.offsetMin = new Vector2(BubleRect.offsetMin.x, BubleRect.offsetMin.y - TextRect.rect.height + StartBubbleWidth);
                BubleRect.offsetMax = new Vector2(BubleRect.offsetMax.x + txtPreferredWidth - StartBubbleWidth, BubleRect.offsetMax.y);
            }
            
            ///****************************************************************************//
            else if (CurrentMessage.Type == ContentMessageType.IMAGE)
            {
                // update buble content rect
                BubleRect.offsetMin = new Vector2(BubleRect.offsetMin.x, BubleRect.offsetMin.y - ContentRect.rect.height + StartBubbleWidth);
                BubleRect.offsetMax = new Vector2(BubleRect.offsetMax.x + ContentRect.rect.width - StartBubbleWidth, BubleRect.offsetMax.y );
            }
            // update message rect
            float _height = BubleRect.rect.height;
            
            Debug.Log("the height is "+ _height);
            
                //+ ProfileRect.rect.height;
            MainRect.sizeDelta = new Vector2(MainRect.rect.width, _height);
            
            
        }

        
        private void GetProfileImage()
        {
            AvatarView.LoadAvatar(CurrentMessage.UserID);
        }
        
        
        public void ClickLike()
        {
            if (CanLikeComment)
            {
                if (IsCommentLiked)
                {
                    AppManager.FIREBASE_CONTROLLER.UnLikComment(CurrentMessage.Key, success =>
                    {
                        if (success)
                        {
                            LoadLikes();
                        }
                    });
                }
                else
                {
                    AppManager.FIREBASE_CONTROLLER.LikeCommet(CurrentMessage.Key, success =>
                    {
                        if (success)
                        {
                            LoadLikes();
                        }
                    });
                }
            }
        }
        
        private void OnDisable()
        {
            RemoveListeners();
            ClearView();
        }
        private void AddListeners()
        {
            if (LikesCountBody != null)
            {
                
                DRCommentLikesCount = AppManager.FIREBASE_CONTROLLER.GetCommentLikesCountReferense(CurrentMessage.Key);
                DRCommentLikesCount.ValueChanged += OnLikesCountUpdated;
                IsActiveListeners = true;
            }
            
        }
        private void RemoveListeners()
        {
            StopAllCoroutines();
            if (IsActiveListeners)
            {
                if (AppManager.FIREBASE_CONTROLLER != null)
                {
                    DRCommentLikesCount.ValueChanged -= OnLikesCountUpdated;
                }
                IsActiveListeners = false;
            }
        }

        private void LoadLikes()
        {
            if (LikesCountBody != null)
            {
                CanLikeComment = false;

                AppManager.FIREBASE_CONTROLLER.IsLikedComment(CurrentMessage.Key, _isLike =>
                {
                    CanLikeComment = true;
                    IsCommentLiked = _isLike;
                    if (IsCommentLiked)
                    {
                        LikeImage.color = LikedPostColor;
                    }
                    else
                    {
                        LikeImage.color = UnLikedPostColor;
                    }
                });
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

        private void ClearView()
        {
            
            LikesCountBody.text = "0";

            
        }
        
        
        
        
        
    }
}
