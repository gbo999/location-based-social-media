using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using Firebase.Database;

namespace SocialApp
{

    public class GroupsDataLoader : MonoBehaviour
    {
        [SerializeField]
        private ScrollViewController ScrollView = default;
        [SerializeField]
        private InputField SearchInput = default;
        [SerializeField]
        private int AutoLoadCount = 3;

        [SerializeField]
        private List<string> GroupsKeys = new List<string>();

        private int GroupsLoaded = 0;

        private int CurrentRequestID = 0;
/*
        private DatabaseReference DRFriendsCount;
        private DatabaseReference DRRequestFriendsCount;
        private DatabaseReference DRPendingFriendsCount;
*/
      /*  private void OnEnable()
        {
            AddListeners();
        }

        private void OnDisable()
        {
            RemoveListeners();
        }

        private void AddListeners()
        {
            DRFriendsCount = AppManager.FIREBASE_CONTROLLER.GetFriendCountReferece(AppManager.Instance.auth.CurrentUser.UserId);
            DRRequestFriendsCount = AppManager.FIREBASE_CONTROLLER.GetRequestFriendCountReferece(AppManager.Instance.auth.CurrentUser.UserId);
            DRPendingFriendsCount = AppManager.FIREBASE_CONTROLLER.GetPendingFriendCountReferece(AppManager.Instance.auth.CurrentUser.UserId);
            DRFriendsCount.ValueChanged += OnFriendsCountUpdated;
            DRRequestFriendsCount.ValueChanged += OnRequestCountUpdated;
            DRPendingFriendsCount.ValueChanged += OnPendingCountUpdated;
        }

        private void RemoveListeners()
        {
            if (AppManager.FIREBASE_CONTROLLER != null)
            {
                DRFriendsCount.ValueChanged -= OnFriendsCountUpdated;
                DRRequestFriendsCount.ValueChanged -= OnRequestCountUpdated;
                DRPendingFriendsCount.ValueChanged -= OnPendingCountUpdated;
            }
        }

        private void OnFriendsCountUpdated(object sender, ValueChangedEventArgs args)
        {
            if (args.DatabaseError != null)
            {
                AppManager.FRIEND_UI_CONTROLLER.UpdateFriendsCount(0);
                return;
            }
            try
            {
                AppManager.FRIEND_UI_CONTROLLER.UpdateFriendsCount(int.Parse(args.Snapshot.Value.ToString()));
            }
            catch (Exception)
            {
                AppManager.FRIEND_UI_CONTROLLER.UpdateFriendsCount(0);
            }
        }

        private void OnRequestCountUpdated(object sender, ValueChangedEventArgs args)
        {
            if (args.DatabaseError != null)
            {
                AppManager.FRIEND_UI_CONTROLLER.UpdateRequestCount(0);
                return;
            }
            try
            {
                AppManager.FRIEND_UI_CONTROLLER.UpdateRequestCount(int.Parse(args.Snapshot.Value.ToString()));
            }
            catch (Exception)
            {
                AppManager.FRIEND_UI_CONTROLLER.UpdateRequestCount(0);
            }
        }

        private void OnPendingCountUpdated(object sender, ValueChangedEventArgs args)
        {
            if (args.DatabaseError != null)
            {
                AppManager.FRIEND_UI_CONTROLLER.UpdatePendingCount(0);
                return;
            }
            try
            {
                AppManager.FRIEND_UI_CONTROLLER.UpdatePendingCount(int.Parse(args.Snapshot.Value.ToString()));
            }
            catch (Exception)
            {
                AppManager.FRIEND_UI_CONTROLLER.UpdatePendingCount(0);
            }
        }*/

        public void ResetLoader()
        {
            GroupsLoaded = 0;
            GroupsKeys.Clear();
            GroupsKeys.TrimExcess();
            ScrollView.ResetSroll();
            ScrollView.HideAllScrollItems();
        }


        public void AutoLoadContent(bool _forward)
        {
            if (AppManager.GROUP_UI_CONTROLLER.CurrentTabState == GroupTabState.Search)
                return;
            if (_forward)
            {
                int loadCount = GroupsLoaded + AutoLoadCount;
                if (GroupsLoaded <= 0)
                {
                    loadCount = ScrollView.GetContentListCount();
                }
                LoadContent(GroupsLoaded, loadCount, _forward);
            }
            else
            {
                LoadContent(GroupsLoaded - ScrollView.GetContentListCount() - AutoLoadCount, GroupsLoaded - ScrollView.GetContentListCount() - 1, _forward);
            }
        }

        private void LoadContent(int _startIndex, int _endIndex, bool _forward)
        {
            GroupQuery _groupsQuery = new GroupQuery();
            _groupsQuery.startIndex = _startIndex;
            _groupsQuery.endIndex = _endIndex;
            _groupsQuery.callback = OnGroupsLoaded;
            _groupsQuery.forward = _forward;
            _groupsQuery.ownerID = AppManager.Instance.auth.CurrentUser.UserId;

            string indexKey = string.Empty;
            if (_forward)
            {
                if (GroupsKeys.Count > 0)
                {
                    indexKey = GroupsKeys[GroupsLoaded - 1];
                }
            }
            else
            {
                if (_startIndex < 0)
                {
                    _startIndex = 0;
                    _groupsQuery.startIndex = _startIndex;
                }
                indexKey = GroupsKeys[_startIndex];
            }

            _groupsQuery.indexKey = indexKey;
            if (_endIndex >= 0)
            {
                CurrentRequestID++;
                _groupsQuery.RequestID = CurrentRequestID;
                if (AppManager.GROUP_UI_CONTROLLER.CurrentTabState == GroupTabState.Search)
                {
                    AppManager.FIREBASE_CONTROLLER.Searchgroups(_groupsQuery, SearchInput.text);
                }
                else
                {
                    Debug.Log("just user groups");
                    _groupsQuery.Type = AppManager.GROUP_UI_CONTROLLER.CurrentTabState;
                    AppManager.FIREBASE_CONTROLLER.GetGroupsAt(_groupsQuery);
                }
                ScrollView.BlockScroll();
            }
        }

        public void OnGroupsLoaded(GroupCallback _callback)
        {
            ScrollView.UnblockScroll();
            if (_callback.IsSuccess && CurrentRequestID == _callback.RequestID)
            {
                int _groupsCount = _callback.groups.Count;
                if (AppManager.GROUP_UI_CONTROLLER.CurrentTabState == GroupTabState.Search)
                {
                    _groupsCount = Mathf.Clamp(_callback.groups.Count, 0, ScrollView.GetContentListCount());
                    Debug.Log("how much" + _groupsCount);
                }
                List<ScrollViewItem> _itemsList = ScrollView.PushItem(_groupsCount, _callback.forward);
                for (int i = 0; i < _itemsList.Count; i++)
                {
                    Debug.Log("id's" + _callback.groups[i].groupID);

                    _itemsList[i].gameObject.GetComponent<GroupViewController>().DisplayInfo(_callback.groups[i]);
                    if (_callback.forward)
                    {
                        GroupsLoaded++;
                        AddGroupKey(_callback.groups[i].groupID);
                    }
                    else
                    {
                        GroupsLoaded--;
                    }
                }
                if (!_callback.forward)
                    ScrollView.UpdateScrollViewPosition(_itemsList, _callback.forward);
            }
        }

        public void OnSearch()
        {
            ResetLoader();
            LoadContent(GroupsLoaded, GroupsLoaded + AutoLoadCount, true);
        }

        private void AddGroupKey(string _key)
        {
            if (!GroupsKeys.Contains(_key))
            {
                GroupsKeys.Add(_key);
            }
        }
    }

    public class GroupQuery
    {
        public int startIndex;
        public int endIndex;
        public Action<GroupCallback> callback;
        public bool forward;
        public string indexKey;
        public string ownerID;
        public GroupTabState Type;
        public int RequestID;
    }

    public class GroupCallback
    {
        public List<Group> groups;
        public bool forward;
        public bool IsSuccess;
        public int RequestID;
    }
}