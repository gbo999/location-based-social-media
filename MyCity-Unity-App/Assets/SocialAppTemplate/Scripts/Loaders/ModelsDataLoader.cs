using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using Firebase.Database;

namespace SocialApp
{

    public class ModelsDataLoader : MonoBehaviour
    {
        [SerializeField]
        private ScrollViewController ScrollView = default;
        [SerializeField]
        private InputField SearchInput = default;
        [SerializeField]
        private int AutoLoadCount = 3;

        [SerializeField]
        private List<string> ModelsKeys = new List<string>();

        private int ModelsLoaded = 0;

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
            ModelsLoaded = 0;
            ModelsKeys.Clear();
            ModelsKeys.TrimExcess();
            ScrollView.ResetSroll();
            ScrollView.HideAllScrollItems();
        }


   /*     public void AutoLoadContent(bool _forward)
        {
            if (AppManager.Model_UI_CONTROLLER.CurrentTabState == GroupTabState.Search)
                return;
            if (_forward)
            {
                int loadCount = ModelsLoaded + AutoLoadCount;
                if (ModelsLoaded <= 0)
                {
                    loadCount = ScrollView.GetContentListCount();
                }
                LoadContent(ModelsLoaded, loadCount, _forward);
            }
            else
            {
                LoadContent(ModelsLoaded - ScrollView.GetContentListCount() - AutoLoadCount, ModelsLoaded - ScrollView.GetContentListCount() - 1, _forward);
            }
        }*/

        private void LoadContent(int _startIndex, int _endIndex, bool _forward)
        {
            ModelsQuery _ModelssQuery = new ModelsQuery();
            _ModelssQuery.startIndex = _startIndex;
            _ModelssQuery.endIndex = _endIndex;
            _ModelssQuery.callback = OnModelsLoaded;
            _ModelssQuery.forward = _forward;
           // _ModelssQuery.ownerID = AppManager.Instance.auth.CurrentUser.UserId;

            string indexKey = string.Empty;
            if (_forward)
            {
                if (ModelsKeys.Count > 0)
                {
                    indexKey = ModelsKeys[ModelsLoaded - 1];
                }
            }
            else
            {
                if (_startIndex < 0)
                {
                    _startIndex = 0;
                    _ModelssQuery.startIndex = _startIndex;
                }
                indexKey = ModelsKeys[_startIndex];
            }

            _ModelssQuery.indexKey = indexKey;
            if (_endIndex >= 0)
            {
                CurrentRequestID++;
                _ModelssQuery.RequestID = CurrentRequestID;
               
             AppManager.FIREBASE_CONTROLLER.SearchModels(_ModelssQuery, SearchInput.text);
                
               /* else
                {
                    Debug.Log("just user groups");
                    _groupsQuery.Type = AppManager.GROUP_UI_CONTROLLER.CurrentTabState;
                    AppManager.FIREBASE_CONTROLLER.GetGroupsAt(_groupsQuery);
                }*/
                ScrollView.BlockScroll();
            }
        }

        public void OnModelsLoaded(ModelCallback _callback)
        {
            ScrollView.UnblockScroll();
            if (_callback.IsSuccess && CurrentRequestID == _callback.RequestID)
            {
                int _modelsCount = _callback.Models.Count;

                _modelsCount = Mathf.Clamp(_callback.Models.Count, 0, ScrollView.GetContentListCount());
                    Debug.Log("how much" + _modelsCount);
                
                List<ScrollViewItem> _itemsList = ScrollView.PushItem(_modelsCount, _callback.forward);
                for (int i = 0; i < _itemsList.Count; i++)
                {
                    Debug.Log("id's" + _callback.Models[i].ModelId);

                    _itemsList[i].gameObject.GetComponent<ModelViewController>().DisplayInfo(_callback.Models[i]);
                    if (_callback.forward)
                    {
                        ModelsLoaded++;
                        AddModelKey(_callback.Models[i].ModelId);
                    }
                    else
                    {
                        ModelsLoaded--;
                    }
                }
                if (!_callback.forward)
                    ScrollView.UpdateScrollViewPosition(_itemsList, _callback.forward);
            }
        }

        public void OnSearch()
        {
            ResetLoader();
            LoadContent(ModelsLoaded, ModelsLoaded + AutoLoadCount, true);
        }

        private void AddModelKey(string _key)
        {
            if (!ModelsKeys.Contains(_key))
            {
                ModelsKeys.Add(_key);
            }
        }
    }

    public class ModelsQuery
    {
        public int startIndex;
        public int endIndex;
        public Action<ModelCallback> callback;
        public bool forward;
        public string indexKey;
      //  public string ownerID;
       // public GroupTabState Type;
        public int RequestID;
    }

    public class ModelCallback
    {
        public List<Model3D> Models;
        public bool forward;
        public bool IsSuccess;
        public int RequestID;
    }
}