using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

namespace SocialApp
{

    public class FeedsDataLoader : MonoBehaviour
    {

        [SerializeField]
        private ScrollViewController ScrollView = default;
        [SerializeField]
        private int AutoLoadCount = 3;
        [SerializeField]
        private FeedDataType LoaderType = default;
        [SerializeField]
        private Text UsernameLabel = default;

        [SerializeField]
        private List<string> FeedsKeys = new List<string>();

        private int FeedsLoaded = 0;

        private string CurrentUserID;

        private void OnEnable()
        {
            ScrollView.ResetSroll();
            ScrollView.HideAllScrollItems();
            if (LoaderType != FeedDataType.User)
            {
                LoadContent(FeedsLoaded, ScrollView.GetContentListCount(), true);
            }
        }

        private void OnDisable()
        {
            FeedsLoaded = 0;
            FeedsKeys.Clear();
            FeedsKeys.TrimExcess();
        }

        private void AutoLoadContent(bool _forward)
        {
            if (_forward)
            {
                LoadContent(FeedsLoaded, FeedsLoaded + AutoLoadCount, _forward);
            }
            else
            {
                LoadContent(FeedsLoaded - ScrollView.GetContentListCount() - AutoLoadCount, FeedsLoaded - ScrollView.GetContentListCount() - 1, _forward);
            }
        }

        public void ResetLoader()
        {
            FeedsLoaded = 0;
            FeedsKeys.Clear();
            FeedsKeys.TrimExcess();
            ScrollView.ResetSroll();
            ScrollView.HideAllScrollItems();
            LoadContent(FeedsLoaded, ScrollView.GetContentListCount(), true);
        }

        public void LoadUserContent(string _id)
        {
            CurrentUserID = _id;
            LoadContent(FeedsLoaded, ScrollView.GetContentListCount(), true);
            AppManager.FIREBASE_CONTROLLER.GetUserFullName(CurrentUserID, _name =>
            {
                UsernameLabel.text = _name;
            });
        }

        private void LoadContent(int _startIndex, int _endIndex, bool _forward)
        {
            FeedQuery _feedQuery = new FeedQuery();
            _feedQuery.startIndex = _startIndex;
            _feedQuery.endIndex = _endIndex;
            _feedQuery.callback = OnFeedsLoaded;
            _feedQuery.forward = _forward;
            if (LoaderType == FeedDataType.User)
            {
                _feedQuery.ownerID = CurrentUserID;
            }
            else
            {
                _feedQuery.ownerID = AppManager.Instance.auth.CurrentUser.UserId;
            }


            string indexKey = string.Empty;
            if (_forward)
            {
                if (FeedsKeys.Count > 0)
                {
                    indexKey = FeedsKeys[FeedsLoaded - 1];
                }
            }
            else
            {
                if (_startIndex < 0)
                {
                    _startIndex = 0;
                    _feedQuery.startIndex = _startIndex;
                }
                indexKey = FeedsKeys[_startIndex];
            }

            _feedQuery.indexKey = indexKey;
            if (_endIndex >= 0)
            {
                if (LoaderType == FeedDataType.Profile || LoaderType == FeedDataType.User)
                {
                    AppManager.FIREBASE_CONTROLLER.GetFeedsAt(_feedQuery);
                }
                if (LoaderType == FeedDataType.World)
                {
                    AppManager.FIREBASE_CONTROLLER.GetWorldFeedsAt(_feedQuery);
                }
                if (LoaderType == FeedDataType.Friend)
                {
                    AppManager.FIREBASE_CONTROLLER.GetFriendsFeedsAt(_feedQuery);
                }
                ScrollView.BlockScroll();
            }
        }

        public void OnFeedsLoaded(FeedCallback _callback)
        {
            ScrollView.UnblockScroll();
            if (_callback.IsSuccess)
            {                
                List<ScrollViewItem> _itemsList = ScrollView.PushItem(_callback.feeds.Count, _callback.forward);
                for (int i = 0; i < _itemsList.Count; i++)
                {
                    _itemsList[i].gameObject.GetComponent<FeedViewController>().LoadMedia(_callback.feeds[i]);
                    if (_callback.forward)
                    {
                        FeedsLoaded++;
                        AddFeedKey(_callback.feeds[i].Key);
                    }
                    else
                    {
                        FeedsLoaded--;
                    }
                }
                if (!_callback.forward)
                    ScrollView.UpdateScrollViewPosition(_itemsList, _callback.forward);
            }
        }

        private void AddFeedKey(string _key)
        {
            if (!FeedsKeys.Contains(_key))
            {
                FeedsKeys.Add(_key);
            }
        }

        public string GetUserID()
        {
            if (LoaderType == FeedDataType.User)
            {
                return CurrentUserID;
            }
            else
            {
                return AppManager.Instance.auth.CurrentUser.UserId;
            }
        }

        public enum FeedDataType
        {
            Friend,
            Profile,
            World,
            User
        }
    }

    public class FeedQuery
    {
        public int startIndex;
        public int endIndex;
        public Action<FeedCallback> callback;
        public bool forward;
        public string indexKey;
        public string ownerID;
    }

    public class FeedCallback
    {
        public List<Feed> feeds;
        public bool forward;
        public bool IsSuccess;
    }

    public class ModelretCallback
    {
        public List<Model3D> Models;
        public bool forward;
        public bool IsSuccess;

    }



    [System.Serializable]
    public class Feed
    {
        public string BodyTXT;
        public string ImageURL;
        public string VideoURL;
        public string VideoFileName;
        public int MediaWidth;
        public int MeidaHeight;
        public string DateCreated;
        public FeedType Type;
        public string Key;
        public string OwnerID;
        public string ToUserID;
        
        //MyCity
        public string tag;
        public string groupPostID;
        public string kind;
        public string startDate;
        public string dateToSave;
        public double price;







        //location attributes//

        
        public string address;
        public string status;
        public double Feedlng;
        public double Feedlat;
        public string cloudID;
        public string panoramaID;
        public string locationType;
        public string typeOfPrefab;
        public string panStreetview;
        public string tiltStreetview;
        public int MediaWidthOne;
        public int MeidaHeightOne;
        public int MediaWidthTwo;
        public int MeidaHeightTwo;
        public string TXTOne;
        public string TXTTwo;
        public string ImageURLOne;
        public string ImageURLTwo;
    }

    public enum FeedType
    {
        Text,
        Image,
        Video,
        Poll,
        thumb

    }
}