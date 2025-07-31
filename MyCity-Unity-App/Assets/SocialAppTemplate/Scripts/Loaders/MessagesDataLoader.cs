using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using Firebase.Database;
using TMPro;

namespace SocialApp
{
    public class MessagesDataLoader : MonoBehaviour
    {
        [SerializeField] private ScrollViewController ScrollView = default;
        [SerializeField] private int AutoLoadCount = 3;
        [SerializeField] private TMP_InputField MessageInput = default;
        [SerializeField] private TMP_Text UsernameLabel = default;
        [SerializeField] private TMP_Text ActivityLabel = default;
        [SerializeField] private MessageType MessangingType = default;
        [SerializeField] private OnlineStatusController OnlineController = default;
        [SerializeField] private GameObject TypingAnimation = default;
        [SerializeField] private GameObject GroupMenuBtn = default;
        [SerializeField] private GameObject GroupMenuWindow = default;
        [SerializeField] private GameObject CallWindow = default;

        [SerializeField] private List<string> MessagesKeys = new List<string>();
        [SerializeField] private int MessagesLoaded = 0;

        private bool FirstMessagesWasLoaded = false;
        [SerializeField] private bool WasReachEnd = false;
        private string LastMessageKey;

        private string loadedID;

        private bool SkipFirstTyping;

        private Query DRMessageRef;
        private DatabaseReference DRTypinRef;
        private Query DRCommentsRef;

        private MessageGroupInfo _currentMessageInfo;

        public void LoadMessageGroup(MessageGroupInfo _chatInfo)
        {
            _currentMessageInfo = _chatInfo;
            MessangingType = _chatInfo.Type;
         //   ActivityLabel.text = string.Empty;
            if (MessangingType == MessageType.Comments) LoadPostComments(_chatInfo.ChatID);
            if (MessangingType == MessageType.Messanging) LoadUserMessages(GetTargetUserId(_currentMessageInfo));
            if (MessangingType == MessageType.Group) LoadGroupChat(_chatInfo.ChatID);
        }

        public void LoadUserMessages(string _userId)
        {
            
            Debug.Log("what loaded id "+ _userId);
            
            
            loadedID = _userId;
            ScrollView.ResetSroll();
            ScrollView.HideAllScrollItems();
            LoadContent(MessagesLoaded, ScrollView.GetContentListCount(), true);
            WasReachEnd = true;
            //OnlineController.SetUser(loadedID);
           // OnlineController.SetUpdateAction(OnOnlineStatusUpdated);
         //   OnlineController.StartCheck();
            AppManager.FIREBASE_CONTROLLER.GetUserFullName(loadedID, _name => { UsernameLabel.text = _name; });
            AppManager.FIREBASE_CONTROLLER.ClearUnreadMessagesWithUser(GetTargetID());
            //TypingAnimation.SetActive(false);
            //ActivityLabel.gameObject.SetActive(true);
            StopAllCoroutines();
            /*if (GroupMenuBtn)
            {
                GroupMenuBtn.SetActive(AppManager.APP_SETTINGS._EnableVideoAudioCalls);
            }*/

            HideGroupMenu();
        }

        public void LoadPostComments(string _postId)
        {
            loadedID = _postId;
            ScrollView.ResetSroll();
            ScrollView.HideAllScrollItems();
            LoadContent(MessagesLoaded, ScrollView.GetContentListCount(), true);
            WasReachEnd = true;
            // UsernameLabel.text = "Comments";
            if (GroupMenuBtn) GroupMenuBtn.SetActive(MessangingType == MessageType.Group);
            HideGroupMenu();
        }

        public void LoadGroupChat(string _groupId)
        {
            loadedID = _groupId;
            ScrollView.ResetSroll();
            ScrollView.HideAllScrollItems();
            LoadContent(MessagesLoaded, ScrollView.GetContentListCount(), true);
            WasReachEnd = true;
            UsernameLabel.text = _currentMessageInfo.ChatName;
            TypingAnimation.SetActive(false);
            ActivityLabel.gameObject.SetActive(true);
            ActivityLabel.text = _currentMessageInfo.Users.Count + " Members";
            GroupMenuBtn.SetActive(MessangingType == MessageType.Group);
            HideGroupMenu();
            AppManager.FIREBASE_CONTROLLER.ClearUnreadMessagesGroup(loadedID);
        }

        private void OnOnlineStatusUpdated()
        {
            ActivityLabel.text = OnlineController.GetStringLastActivity();
        }

        private void OnDisable()
        {
            MessagesLoaded = 0;
            MessagesKeys.Clear();
            MessagesKeys.TrimExcess();
            RemoveListeners();
            FirstMessagesWasLoaded = false;
            SkipFirstTyping = false;
            if (MessangingType == MessageType.Messanging)
            {
                if (AppManager.FIREBASE_CONTROLLER != null)
                {
                    AppManager.FIREBASE_CONTROLLER.ClearUnreadMessagesWithUser(GetTargetID());
                }

                TypingAnimation.SetActive(false);
                ActivityLabel.gameObject.SetActive(true);
                StopAllCoroutines();
            }

            if (MessangingType == MessageType.Group)
            {
                if (AppManager.FIREBASE_CONTROLLER != null)
                {
                    AppManager.FIREBASE_CONTROLLER.ClearUnreadMessagesGroup(loadedID);
                }

                StopAllCoroutines();
            }

            HideGroupMenu();
        }

        public void AddListeners()
        {
            if (MessangingType == MessageType.Messanging)
            {
                DRMessageRef = AppManager.FIREBASE_CONTROLLER.GetMessageReferece(loadedID).LimitToLast(1);
                DRTypinRef = AppManager.FIREBASE_CONTROLLER.GetTypingMessageReferece(loadedID);
                DRMessageRef.ChildAdded += HandleMessageAdded;
                DRTypinRef.ValueChanged += HandleMessageTyping;
            }

            if (MessangingType == MessageType.Group)
            {
                DRMessageRef = AppManager.FIREBASE_CONTROLLER.GetGroupChatReferece(loadedID).LimitToLast(1);
                DRMessageRef.ChildAdded += HandleMessageAdded;
                AddNewChatController.OnNewMembersAdded += OnNewMembersAdded;
            }

            if (MessangingType == MessageType.Comments)
            {
                DRCommentsRef = AppManager.FIREBASE_CONTROLLER.GetPostCommentsReferece(loadedID).LimitToLast(1);
                DRCommentsRef.ChildAdded += HandleMessageAdded;
            }
        }

        public void RemoveListeners()
        {
            if (MessangingType == MessageType.Messanging)
            {
                if (AppManager.FIREBASE_CONTROLLER != null)
                {
                    DRMessageRef.ChildAdded -= HandleMessageAdded;
                    DRTypinRef.ValueChanged -= HandleMessageTyping;
                }
            }

            if (MessangingType == MessageType.Group)
            {
                if (AppManager.FIREBASE_CONTROLLER != null)
                {
                    DRMessageRef.ChildAdded -= HandleMessageAdded;
                }

                AddNewChatController.OnNewMembersAdded += OnNewMembersAdded;
            }

            if (MessangingType == MessageType.Comments)
            {
                if (AppManager.FIREBASE_CONTROLLER != null)
                    DRCommentsRef.ChildAdded -= HandleMessageAdded;
            }
        }

        void HandleMessageTyping(object sender, ValueChangedEventArgs args)
        {
            if (!SkipFirstTyping)
            {
                SkipFirstTyping = true;
            }
            else
            {
                StartTypingAnimation();
            }
        }

        void HandleMessageAdded(object sender, ChildChangedEventArgs args)
        {
            if (args.DatabaseError != null)
            {
                return;
            }
            else
            {
                Message _dataMsg = JsonUtility.FromJson<Message>(args.Snapshot.GetRawJsonValue());
                _dataMsg.Key = args.Snapshot.Key;
                if (!WasReachEnd)
                {
                    LastMessageKey = args.Snapshot.Key;
                }
                else
                {
                    if (!MessagesKeys.Contains(args.Snapshot.Key))
                    {
                        AddMessageKey(args.Snapshot.Key);

                        MessagesCallback _callback = new MessagesCallback();
                        _callback.forward = true;
                        _callback.IsSuccess = true;
                        _callback.messages = new List<Message>();
                        _callback.messages.Add(_dataMsg);
                        UnityMainThreadDispatcher.Instance().Enqueue(() => OnMessagesLoaded(_callback));
                        if (MessangingType == MessageType.Messanging || MessangingType == MessageType.Group)
                        {
                            AppManager.FIREBASE_CONTROLLER.ClearUnreadMessagesWithUser(GetTargetID());
                        }
                    }
                }
            }
        }

        private void AutoLoadContent(bool _forward)
        {
            if (_forward && !WasReachEnd)
            {
                LoadContent(ScrollView.GetContentListCount(), ScrollView.GetContentListCount() + AutoLoadCount,
                    _forward);
            }
            else if (!_forward)
            {
                LoadContent(0, AutoLoadCount, _forward);
            }
        }

        private void LoadContent(int _startIndex, int _endIndex, bool _forward)
        {
            MessagesQuery _messageQuery = new MessagesQuery();
            _messageQuery.startIndex = _startIndex;
            _messageQuery.endIndex = _endIndex;
            _messageQuery.callback = OnMessagesLoaded;
            _messageQuery.forward = _forward;
            _messageQuery.UserId = loadedID;

            string indexKey = string.Empty;
            if (_forward)
            {
                if (MessagesKeys.Count > 0)
                {
                    indexKey = MessagesKeys[_startIndex - 1];
                }
            }
            else
            {
                if (_startIndex < 0)
                {
                    _startIndex = 0;
                    _messageQuery.startIndex = _startIndex;
                }

                indexKey = MessagesKeys[_startIndex];
            }

            _messageQuery.indexKey = indexKey;
            if (_endIndex >= 0)
            {
                if (MessangingType == MessageType.Messanging)
                {
                    AppManager.FIREBASE_CONTROLLER.GetMessagesAt(_messageQuery);
                }

                if (MessangingType == MessageType.Group)
                {
                    AppManager.FIREBASE_CONTROLLER.GetGroupMessagesAt(_messageQuery);
                }

                if (MessangingType == MessageType.Comments)
                {
                    AppManager.FIREBASE_CONTROLLER.GetCommentsAt(_messageQuery);
                }

                ScrollView.BlockScroll();
            }
        }

        public void OnMessagesLoaded(MessagesCallback _callback)
        {
            ScrollView.UnblockScroll();
            if (_callback.IsSuccess)
            {
                bool isScrollViewFull = ScrollView.IsListFull();
                List<ScrollViewItem> _itemsList = ScrollView.PushItem(_callback.messages.Count, _callback.forward);
                for (int i = 0; i < _itemsList.Count; i++)
                {
                    _itemsList[i].gameObject.GetComponent<MessageViewController>().LoadMedia(_callback.messages[i]);
                    if (_callback.forward)
                    {
                        if (!FirstMessagesWasLoaded || !isScrollViewFull)

                        {
                            MessagesLoaded++;
                            AddMessageKey(_callback.messages[i].Key);
                        }
                        else
                        {
                            AddMessageKey(_callback.messages[i].Key);
                            RemoveKeyAt(0);
                            MessagesLoaded--;
                            if (_callback.messages[i].Key == LastMessageKey)
                            {
                                WasReachEnd = true;
                            }
                        }
                    }
                    else
                    {
                        if (WasReachEnd)
                        {
                            LastMessageKey = MessagesKeys[MessagesKeys.Count - 1];
                        }

                        WasReachEnd = false;
                        if (FirstMessagesWasLoaded)
                        {
                            bool _added = AddMessageKey(_callback.messages[i].Key, true);
                            if (_added)
                            {
                                MessagesLoaded++;
                                RemoveKeyAt(MessagesKeys.Count - 1);
                            }
                        }
                        else
                        {
                            MessagesLoaded--;
                        }
                    }
                }

                if (!_callback.forward)
                    ScrollView.UpdateScrollViewPosition(_itemsList, _callback.forward);
            }

            if (WasReachEnd)
            {
                ScrollView.ScrollToBottom();
            }

            if (FirstMessagesWasLoaded == false)
            {
                AddListeners();
                FirstMessagesWasLoaded = true;
            }
        }

        private bool AddMessageKey(string _key, bool _inTop = false)
        {
            if (!MessagesKeys.Contains(_key))
            {
                if (_inTop)
                {
                    MessagesKeys.Insert(0, _key);
                }
                else
                {
                    MessagesKeys.Add(_key);
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        private void RemoveKeyAt(int _index)
        {
            MessagesKeys.RemoveAt(_index);
            MessagesKeys.TrimExcess();
        }

        private void FinalSendMessage(Message _msg)
        {
            AppManager.USER_PROFILE.GetUserFullName(_name =>
            {
                if (string.IsNullOrEmpty(_name))
                {
                    _name = "Unknown";
                }

                _msg.FullName = _name;
                if (MessangingType == MessageType.Messanging)
                {
                    AppManager.FIREBASE_CONTROLLER.UploadMessage(_msg, _currentMessageInfo);

                    // notification
                    /*string _bodyTXT = string.Empty;

                    if (_msg.Type == ContentMessageType.TEXT) _bodyTXT = _msg.BodyTXT;
                    if (_msg.Type == ContentMessageType.IMAGE) _bodyTXT = _msg.Type.ToString();

                    /*NotificationMessage _FCMMessage = new NotificationMessage();
                    _FCMMessage.UserId = loadedID;
                    _FCMMessage.Title = _name;
                    _FCMMessage.Body = _bodyTXT;
                    AppManager.FIREBASE_CONTROLLER.SendPushNotification(_FCMMessage);#1#*/
                }

                if (MessangingType == MessageType.Group)
                {
                    AppManager.FIREBASE_CONTROLLER.UploadGroupMessage(_msg, _currentMessageInfo);

                    for (int i = 0; i < _currentMessageInfo.Users.Count; i++)
                    {
                        string _userID = _currentMessageInfo.Users[i];
                        if (!AppManager.USER_PROFILE.IsMine(_userID))
                        {
                            string _bodyTXT = string.Empty;

                            if (_msg.Type == ContentMessageType.TEXT) _bodyTXT = _msg.BodyTXT;
                            if (_msg.Type == ContentMessageType.IMAGE) _bodyTXT = _msg.Type.ToString();

                            NotificationMessage _FCMMessage = new NotificationMessage();
                            _FCMMessage.UserId = _userID;
                            _FCMMessage.Title = _currentMessageInfo.ChatName;
                            _FCMMessage.Body = _bodyTXT;
                            AppManager.FIREBASE_CONTROLLER.SendPushNotification(_FCMMessage);
                        }
                    }
                }

                if (MessangingType == MessageType.Comments)
                {
                    AppManager.FIREBASE_CONTROLLER.UploadComments(_msg, loadedID);
                }

                MessageInput.text = string.Empty;
            });
        }

        public void UploadMessage()
        {
            if (!string.IsNullOrEmpty(MessageInput.text))
            {
                Message _msg = new Message();
                _msg.BodyTXT = MessageInput.text;
                _msg.UserID = AppManager.Instance.auth.CurrentUser.UserId;
                _msg.DateCreated = AppManager.DEVICE_CONTROLLER.GetSystemDate();
                _msg.TargetId = GetTargetID();
                _msg.Type = ContentMessageType.TEXT;
                FinalSendMessage(_msg);
            }
        }


        public void OnInputChanged(string _s)
        {
            _s = MessageInput.text;
            AppManager.FIREBASE_CONTROLLER.UpdateTypingMessage(loadedID, _s);
        }

        private void StartTypingAnimation()
        {
            TypingAnimation.SetActive(true);
            ActivityLabel.gameObject.SetActive(false);
            StopAllCoroutines();
            StartCoroutine(OnStartTypingAnimation());
        }

        private IEnumerator OnStartTypingAnimation()
        {
            yield return new WaitForSeconds(3f);
            TypingAnimation.SetActive(false);
            ActivityLabel.gameObject.SetActive(true);
            StopAllCoroutines();
        }

        public static string GetTargetUserId(MessageGroupInfo _currentMessageInfo)
        {
            if (_currentMessageInfo == null)
                return string.Empty;
            if (_currentMessageInfo.Type != MessageType.Messanging)
                return string.Empty;
            if (_currentMessageInfo.Users.Count < 2)
                return string.Empty;
            string _tempUserId = string.Empty;
            string myID = AppManager.Instance.auth.CurrentUser.UserId;
            if (_currentMessageInfo.Users[0] == myID)
            {
                _tempUserId = _currentMessageInfo.Users[1];
            }
            else
            {
                _tempUserId = _currentMessageInfo.Users[0];
            }

            return _tempUserId;
        }

        public void ShowGroupMenu()
        {
            if (GroupMenuBtn != null)
            {
                if (_currentMessageInfo.Type == MessageType.Group)
                {
                    GroupMenuWindow.SetActive(true);
                }
                else if (_currentMessageInfo.Type == MessageType.Messanging)
                {
                    CallWindow.SetActive(true);
                }
            }
        }

        public void HideGroupMenu()
        {
            if (GroupMenuBtn != null)
            {
                if (_currentMessageInfo.Type == MessageType.Group)
                {
                    GroupMenuWindow.SetActive(false);
                }
                else if (_currentMessageInfo.Type == MessageType.Messanging)
                {
                    CallWindow.SetActive(false);
                }
            }
        }

        public void OnShowMembers()
        {
            HideGroupMenu();
            AppManager.VIEW_CONTROLLER.ShowChatMembers(_currentMessageInfo);
        }

        public void OnAddMembers()
        {
            HideGroupMenu();
            AppManager.VIEW_CONTROLLER.ShowAddNewChatMembers(_currentMessageInfo);
        }

        public void OnLeaveGroup()
        {
            if (_currentMessageInfo != null)
            {
                if (_currentMessageInfo.Users.Contains(AppManager.Instance.auth.CurrentUser.UserId))
                {
                    _currentMessageInfo.Users.Remove(AppManager.Instance.auth.CurrentUser.UserId);

                    _currentMessageInfo.Users.TrimExcess();

                    AppManager.FIREBASE_CONTROLLER.ClearUnreadMessagesGroup(loadedID);
                    AppManager.FIREBASE_CONTROLLER.RemoveFromMessageList(_currentMessageInfo);
                    AppManager.FIREBASE_CONTROLLER.AddOrUpdateChatInfo(_currentMessageInfo, (_msg =>
                    {
                        if (_msg.IsSuccess)
                        {
                            AppManager.VIEW_CONTROLLER.HideUserMessanging();
                            AppManager.VIEW_CONTROLLER.ShowNavigationPanel();
                            AppManager.NAVIGATION.ShowLastTab();
                        }
                    }));
                }
            }

            HideGroupMenu();
        }

        public void OnNewMembersAdded(MessageGroupInfo _newGroupInfo)
        {
            _currentMessageInfo = _newGroupInfo;
            ActivityLabel.text = _currentMessageInfo.Users.Count + " Members";
        }

        public void AddImageFromGallery()
        {
            NativeGallery.GetImageFromGallery(OnImagePicked);
        }

        public void AddImageFromCamera()
        {
            NativeCamera.TakePicture(OnImagePicked);
        }

        private void OnImagePicked(string _path)
        {
            if (string.IsNullOrEmpty(_path))
                return;
            UploadMediaMessage(_path);
        }

        private void UploadMediaMessage(string _contentUrl)
        {
            Texture2D _texture = new Texture2D(2, 2);
            _texture = NativeCamera.LoadImageAtPath(_contentUrl, -1, false);
            byte[] fileBytes = _texture.EncodeToJPG(AppManager.APP_SETTINGS.UploadImageQuality);
            string fileName = Guid.NewGuid().ToString();

            FileUploadRequset _imageUploadRequest = new FileUploadRequset();
            _imageUploadRequest.FeedType = FeedType.Image;
            _imageUploadRequest.FileName = fileName + "." + Utils.GetFileExtension(_contentUrl);
            _imageUploadRequest.UploadBytes = fileBytes;

            FileUploadCallback _callBack = new FileUploadCallback();
            AppManager.FIREBASE_CONTROLLER.UploadFile(_imageUploadRequest, callback =>
            {
                if (callback.IsSuccess)
                {
                    Message _msg = new Message();
                    _msg.UserID = AppManager.Instance.auth.CurrentUser.UserId;
                    _msg.DateCreated = AppManager.DEVICE_CONTROLLER.GetSystemDate();
                    _msg.TargetId = GetTargetID();
                    _msg.Type = ContentMessageType.IMAGE;

                    MediaMessageDetaill _detail = new MediaMessageDetaill();
                    _detail.ContentURL = callback.DownloadUrl;
                    _detail.ContentHeight = _texture.height;
                    _detail.ContentWidth = _texture.width;

                    _msg.MediaInfo = _detail;

                    FinalSendMessage(_msg);
                }
                else
                {
                    AppManager.VIEW_CONTROLLER.ShowPopupMSG(MessageCode.FailedUploadImage);
                }
            });
        }

        public string GetTargetID()
        {
            if (MessangingType == MessageType.Comments)
            {
                return loadedID;
            }
            else if (_currentMessageInfo.Type == MessageType.Messanging)
            {
                return AppManager.FIREBASE_CONTROLLER.GetUserMessageKey(AppManager.Instance.auth.CurrentUser.UserId,
                    loadedID);
            }
            else
            {
                return loadedID;
            }
        }

        public void VideoCall()
        {
            HideGroupMenu();
            CallObject _callObject = new CallObject();
            _callObject.CallID =
                AppManager.FIREBASE_CONTROLLER.GetUserMessageKey(AppManager.Instance.auth.CurrentUser.UserId, loadedID);
            _callObject.UserID = AppManager.Instance.auth.CurrentUser.UserId;
            _callObject.TargetID = loadedID;
            _callObject.CallType = CallType.VIDEO;

            AppManager.FIREBASE_CONTROLLER.MakeCallOffer(_callObject, _callback =>
            {
                if (_callback.IsSuccess)
                {
                    AppManager.VIEW_CONTROLLER.StartCall(IncommingType.CALLER, _callObject);
                }
            });
        }

        public void AudioCall()
        {
            HideGroupMenu();
            CallObject _callObject = new CallObject();
            _callObject.CallID =
                AppManager.FIREBASE_CONTROLLER.GetUserMessageKey(AppManager.Instance.auth.CurrentUser.UserId, loadedID);
            _callObject.UserID = AppManager.Instance.auth.CurrentUser.UserId;
            _callObject.TargetID = loadedID;
            _callObject.CallType = CallType.AUDIO;

            AppManager.FIREBASE_CONTROLLER.MakeCallOffer(_callObject, _callback =>
            {
                if (_callback.IsSuccess)
                {
                    AppManager.VIEW_CONTROLLER.StartCall(IncommingType.CALLER, _callObject);
                }
            });
        }
    }

    public enum MessageType
    {
        Messanging,
        Comments,
        Group
    }

    public class MessagesQuery
    {
        public int startIndex;
        public int endIndex;
        public Action<MessagesCallback> callback;
        public bool forward;
        public string indexKey;
        public string UserId;
    }

    public class MessagesCallback
    {
        public List<Message> messages;
        public bool forward;
        public bool IsSuccess;
    }

    public class Message
    {
        public string Key;
        public string BodyTXT;
        public string DateCreated;
        public string UserID;
        public string FullName;
        public string TargetId;
        public ContentMessageType Type;
        public MediaMessageDetaill MediaInfo;
    }

    public enum ContentMessageType
    {
        TEXT,
        IMAGE
    }

    [System.Serializable]
    public class MediaMessageDetaill
    {
        public string ContentURL;
        public int ContentWidth;
        public int ContentHeight;
    }
}