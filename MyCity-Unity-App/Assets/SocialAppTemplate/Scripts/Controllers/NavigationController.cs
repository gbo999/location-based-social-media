using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;
using System;
using ARLocation;
using ARLocation.MapboxRoutes;
using SocialApp;
using Yamanas.Scripts.MapLoader;

namespace SocialApp
{
    public class NavigationController : MonoBehaviour
    {

        [SerializeField]
        private Image[] Icons = default;
        [SerializeField]
        private Image[] Backgounds = default;
        [SerializeField]
        private Color ActiveColor = default;
        [SerializeField]
        private Color DefaultColor = default;
        [SerializeField]
        private Color ActiveBackColor = default;
        [SerializeField]
        private GameObject UnreadCountObject = default;
        [SerializeField]
        private Text UnreadCountLabel = default;
        [SerializeField]
        private GameObject FriendFeedsCountObject = default;
        [SerializeField]
        private Text FriendFeedCountLabel = default;
        [SerializeField]
        private GameObject FriendCountObject = default;
        [SerializeField]
        private Text FriendCountLabel = default;

        private DatabaseReference DRRequestFriendsCount;
        private DatabaseReference DRFeedFriendsCount;
        private DatabaseReference DRUnreadAllCount;

        private NavigationTab CurrentTab;

        public event Action OnNavigateAR;


        public void ShowAR()
        {
            CurrentTab = NavigationTab.Profile;
            AppManager.DEVICE_CONTROLLER.UnloadAssets();
            AppManager.VIEW_CONTROLLER.HideNavigationGroup();
            AppManager.VIEW_CONTROLLER.ShowAR();
            AppManager.VIEW_CONTROLLER.HideNavigationPanel();
            PostProcessController.Instance.EnvState = EnvState.AR;
            
        }

        public void NavigateAR()
        {
            CurrentTab = NavigationTab.Profile;
            AppManager.DEVICE_CONTROLLER.UnloadAssets();
            AppManager.VIEW_CONTROLLER.HideNavigationGroup();
            AppManager.VIEW_CONTROLLER.ShowAR();
            AppManager.VIEW_CONTROLLER.HideNavigationPanel();
            PostProcessController.Instance.EnvState = EnvState.AR;
            OnNavigateAR?.Invoke();
        }
        
        public void ShowMaps()
        {

            CurrentTab = NavigationTab.Profile;
            AppManager.DEVICE_CONTROLLER.UnloadAssets();
            AppManager.VIEW_CONTROLLER.HideNavigationGroup();
            AppManager.VIEW_CONTROLLER.ShowMaps();
            Debug.Log("show maps in navigator");

            AppManager.VIEW_CONTROLLER.HideNavigationPanel();


        }




        public void ShowUploadModel()
        {
            CurrentTab = NavigationTab.Profile;
            AppManager.DEVICE_CONTROLLER.UnloadAssets();
            AppManager.VIEW_CONTROLLER.HideNavigationGroup();
            AppManager.VIEW_CONTROLLER.HideMaps();
            AppManager.VIEW_CONTROLLER.HideNavigationPanel();
            AppManager.VIEW_CONTROLLER.ShowUploadModel();


        }




        public void ShowCreateGroup()
        {
            CurrentTab = NavigationTab.Profile;
            AppManager.DEVICE_CONTROLLER.UnloadAssets();
            AppManager.VIEW_CONTROLLER.HideNavigationGroup();
            AppManager.VIEW_CONTROLLER.ShowCreateGroup();


        }

        public void ShowSearchGroup()
        {
            CurrentTab = NavigationTab.Profile;
            AppManager.DEVICE_CONTROLLER.UnloadAssets();
            AppManager.VIEW_CONTROLLER.HideNavigationGroup();
            AppManager.VIEW_CONTROLLER.ShowSearchGroup();
            AppManager.GROUP_UI_CONTROLLER.OnGroups();

        }

        public void ShowSearchModel()
        {
            CurrentTab = NavigationTab.Profile;
            AppManager.DEVICE_CONTROLLER.UnloadAssets();
            AppManager.VIEW_CONTROLLER.HideNavigationGroup();
            AppManager.VIEW_CONTROLLER.ShowSearchModel();
           // AppManager.GROUP_UI_CONTROLLER.OnGroups();

        }
        public void ShowUserProfile()
        {
            CurrentTab = NavigationTab.Profile;
            AppManager.DEVICE_CONTROLLER.UnloadAssets();
            AppManager.VIEW_CONTROLLER.HideNavigationGroup();
            AppManager.VIEW_CONTROLLER.ShowUserProfile();

            ToDefault();
            Icons[0].color = ActiveColor;
            Backgounds[0].color = ActiveBackColor;
        }

        public void ShowSetting()
        {
            CurrentTab = NavigationTab.Setting;
            AppManager.DEVICE_CONTROLLER.UnloadAssets();
            AppManager.VIEW_CONTROLLER.HideNavigationGroup();
            AppManager.VIEW_CONTROLLER.ShowSettings();

            ToDefault();
            Icons[5].color = ActiveColor;
            Backgounds[5].color = ActiveBackColor;
        }

        public void ShowFriends()
        {
            CurrentTab = NavigationTab.Friends;
            AppManager.DEVICE_CONTROLLER.UnloadAssets();
            AppManager.VIEW_CONTROLLER.HideNavigationGroup();
            AppManager.VIEW_CONTROLLER.ShowFriendsList();
            AppManager.FRIEND_UI_CONTROLLER.OnFriends();

            ToDefault();
            Icons[3].color = ActiveColor;
            Backgounds[3].color = ActiveBackColor;
        }

        public void ShowFriendsNews()
        {
            CurrentTab = NavigationTab.FriendsNews;
            AppManager.DEVICE_CONTROLLER.UnloadAssets();
            AppManager.VIEW_CONTROLLER.HideNavigationGroup();
            AppManager.VIEW_CONTROLLER.ShowFriendsNews();
            ToDefault();
            Icons[2].color = ActiveColor;
            Backgounds[2].color = ActiveBackColor;
            AppManager.FIREBASE_CONTROLLER.ClearUnreadFriendsFeed();
        }

        public void ShowAllNews()
        {
            CurrentTab = NavigationTab.WorldNews;
            AppManager.DEVICE_CONTROLLER.UnloadAssets();
            AppManager.VIEW_CONTROLLER.HideNavigationGroup();
            AppManager.VIEW_CONTROLLER.ShowWorldNews();
            ToDefault();
            Icons[1].color = ActiveColor;
            Backgounds[1].color = ActiveBackColor;
        }

        public void ShowMessanging()
        {
            CurrentTab = NavigationTab.Messanging;
            AppManager.DEVICE_CONTROLLER.UnloadAssets();
            AppManager.VIEW_CONTROLLER.HideNavigationGroup();
            AppManager.VIEW_CONTROLLER.ShowMessageList();

            ToDefault();
            Icons[4].color = ActiveColor;
            Backgounds[4].color = ActiveBackColor;
        }

        public void AddListeners()
        {
            if (AppManager.FIREBASE_CONTROLLER.IsFirebaseInited())
            {
                DRRequestFriendsCount = AppManager.FIREBASE_CONTROLLER.GetRequestFriendCountReferece(AppManager.Instance.auth.CurrentUser.UserId);
                DRFeedFriendsCount = AppManager.FIREBASE_CONTROLLER.GetFriendFeedCountReferece(AppManager.Instance.auth.CurrentUser.UserId);
                DRUnreadAllCount = AppManager.FIREBASE_CONTROLLER.GetAllUnreadCountReferece(AppManager.Instance.auth.CurrentUser.UserId);
                DRRequestFriendsCount.ValueChanged += OnRequestCountUpdated;
                DRFeedFriendsCount.ValueChanged += OnFriendsCountUpdated;
                DRUnreadAllCount.ValueChanged += OnUnreadCountUpdated;
            }
            UnreadCountObject.SetActive(false);
            FriendFeedsCountObject.SetActive(false);
            FriendCountObject.SetActive(false);
        }

        public void RemoveListeners()
        {
            if (AppManager.FIREBASE_CONTROLLER.IsFirebaseInited())
            {
                DRRequestFriendsCount.ValueChanged -= OnRequestCountUpdated;
                DRFeedFriendsCount.ValueChanged -= OnFriendsCountUpdated;
                DRUnreadAllCount.ValueChanged -= OnUnreadCountUpdated;
            }
        }

        private void ToDefault()
        {
            foreach (Image _background in Backgounds)
            {
                _background.color = ActiveColor;
            }
            foreach (Image _icon in Icons)
            {
                _icon.color = DefaultColor;
            }
        }

        private void OnRequestCountUpdated(object sender, ValueChangedEventArgs args)
        {
            if (args.DatabaseError != null)
            {
                FriendCountLabel.text = "0";
                FriendCountObject.SetActive(false);
                return;
            }
            try
            {
                if (args.Snapshot.Value.ToString() == "0")
                {
                    FriendCountLabel.text = "0";
                    FriendCountObject.SetActive(false);
                }
                else
                {
                    FriendCountLabel.text = args.Snapshot.Value.ToString();
                    FriendCountObject.SetActive(true);
                }
            }
            catch (Exception)
            {
                FriendCountLabel.text = "0";
                FriendCountObject.SetActive(false);
            }
        }

        private void OnFriendsCountUpdated(object sender, ValueChangedEventArgs args)
        {
            if (args.DatabaseError != null)
            {
                FriendFeedCountLabel.text = "0";
                FriendFeedsCountObject.SetActive(false);
                return;
            }
            try
            {
                if (args.Snapshot.Value.ToString() == "0")
                {
                    FriendFeedCountLabel.text = "0";
                    FriendFeedsCountObject.SetActive(false);
                }
                else
                {
                    FriendFeedCountLabel.text = args.Snapshot.Value.ToString();
                    FriendFeedsCountObject.SetActive(true);
                }
            }
            catch (Exception)
            {
                FriendFeedCountLabel.text = "0";
                FriendFeedsCountObject.SetActive(false);
            }
        }

        private void OnUnreadCountUpdated(object sender, ValueChangedEventArgs args)
        {
            if (args.DatabaseError != null)
            {
                UnreadCountLabel.text = "0";
                UnreadCountObject.SetActive(false);
                return;
            }
            try
            {
                if (args.Snapshot.Value.ToString() == "0")
                {
                    UnreadCountLabel.text = "0";
                    UnreadCountObject.SetActive(false);
                }
                else
                {
                    UnreadCountLabel.text = args.Snapshot.Value.ToString();
                    UnreadCountObject.SetActive(true);
                }
            }
            catch (Exception)
            {
                UnreadCountLabel.text = "0";
                UnreadCountObject.SetActive(false);
            }
        }

        public void ShowLastTab()
        {
            switch (CurrentTab)
            {
                case NavigationTab.Profile:
                    ShowUserProfile();
                    break;
                case NavigationTab.WorldNews:
                    ShowAllNews();
                    break;
                case NavigationTab.FriendsNews:
                    ShowFriendsNews();
                    break;
                case NavigationTab.Friends:
                    ShowFriends();
                    break;
                case NavigationTab.Messanging:
                    ShowMessanging();
                    break;
                case NavigationTab.Setting:
                    ShowSetting();
                    break;
                default:
                    Debug.Log("NOTHING");
                    break;
            }


        }

        public enum NavigationTab
        {
            Profile,
            WorldNews,
            FriendsNews,
            Friends,
            Messanging,
            Setting
        }
    }
}
