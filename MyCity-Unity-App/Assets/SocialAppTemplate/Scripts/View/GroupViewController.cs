using UnityEngine;
using UnityEngine.UI;


namespace SocialApp
{
    public class GroupViewController : MonoBehaviour
    {
        /*  [SerializeField]
          private Text GroupName = default;
          [SerializeField]
          private GameObject AddToFriendBtn = default;
          [SerializeField]
          private GameObject AcceptFriendBtn = default;
          [SerializeField]
          private GameObject DeclineFriendBtn = default;
          [SerializeField]
          private GameObject RemoveFriendBtn = default;*/


        [SerializeField]
        private Text GroupName = default;
        [SerializeField]
        private AvatarViewController AvatarView = default;
        [SerializeField]
        private bool CacheAvatar = default;
      /*  [SerializeField]
        private OnlineStatusController OnlineController = default;
        [SerializeField]
        private Image OnlineImage = default;
        [SerializeField]
        private Color OnlineColor = default;
        [SerializeField]
        private Color OfflineColor = default;
*/
        private Group CurrentGroup;


        private void ClearData()
        {
            /*AvatarView.DisplayDefaultAvatar();
            GroupName.text = string.Empty;
            CurrentGroup = null;
            HideAllBtns();
            OnlineImage.color = OfflineColor;*/
        }

        public void DisplayInfo(Group _group)
        {
            ClearData();
            AvatarView.SetCacheTexture(CacheAvatar);
            CurrentGroup = _group;
            GroupName.text = CurrentGroup.groupName;
            getIcon();
            
            /*  DisplayButtons();
            OnlineController.SetUser(CurrentGroup.UserID);
            OnlineController.SetUpdateAction(OnOnlineStatusUpdated);
            OnlineController.StartCheck();*/
        }


        public void ChosenGroup()
        {

            AppManager.myCityController.groupPostID = CurrentGroup.groupID;
            AppManager.myCityController.currentTag = CurrentGroup.tag;


            Debug.Log(AppManager.myCityController.groupPostID);

            AppManager.NAVIGATION.ShowMaps();

        }






        /* private void OnOnlineStatusUpdated()
         {
             if (OnlineController.IsOnline())
             {
                 OnlineImage.color = OnlineColor;
             }
             else
             {
                 OnlineImage.color = OfflineColor;
             }
         }

         private void DisplayButtons()
         {
             if (AppManager.FRIEND_UI_CONTROLLER.CurrentTabState == FriendsTabState.Search)
                 AppManager.FIREBASE_CONTROLLER.CanAddToFriend(CurrentGroup.UserID, OnCanAddFriend);
             if (AppManager.FRIEND_UI_CONTROLLER.CurrentTabState == FriendsTabState.Friend)
                 RemoveFriendBtn.SetActive(true);
             if (AppManager.FRIEND_UI_CONTROLLER.CurrentTabState == FriendsTabState.Pending)
                 DeclineFriendBtn.SetActive(true);
             if (AppManager.FRIEND_UI_CONTROLLER.CurrentTabState == FriendsTabState.Requested)
                 AcceptFriendBtn.SetActive(true);
         }

         private void OnCanAddFriend(bool _canAdd)
         {
             AddToFriendBtn.SetActive(_canAdd);
         }

         private void HideAllBtns()
         {
             AddToFriendBtn.SetActive(false);
             AcceptFriendBtn.SetActive(false);
             DeclineFriendBtn.SetActive(false);
             RemoveFriendBtn.SetActive(false);
         }

         public void AddToFriend()
         {
             if (CurrentGroup == null)
                 return;
             AppManager.FIREBASE_CONTROLLER.AddToFriends(CurrentGroup.UserID, OnAddedToFriend);
         }

         private void OnAddedToFriend()
         {
             AddToFriendBtn.SetActive(false);
         }

         public void AcceptFriend()
         {
             if (CurrentGroup == null)
                 return;
             AppManager.FIREBASE_CONTROLLER.AcceptFriend(CurrentGroup.UserID, OnFriendAccepted);
         }

         private void OnFriendAccepted()
         {
             AppManager.FRIEND_UI_CONTROLLER.OnRequest();
         }

         public void CancelPending()
         {
             if (CurrentGroup == null)
                 return;
             AppManager.FIREBASE_CONTROLLER.CancelPendingFromFriend(CurrentGroup.UserID, OnPendingCanceled);
         }

         public void RemoveFriend()
         {
             if (CurrentGroup == null)
                 return;
             AppManager.FIREBASE_CONTROLLER.RemoveFromFriend(CurrentGroup.UserID, OnFriendRemoved);
         }

         private void OnFriendRemoved()
         {
             AppManager.FRIEND_UI_CONTROLLER.OnFriends();
         }

         private void OnPendingCanceled()
         {
             AppManager.FRIEND_UI_CONTROLLER.OnPending();
         }*/

        private void getIcon()
        {
            AvatarView.LoadIcon(CurrentGroup.groupID);
        }

       /* public void ShowUserProfile()
        {
            if (AppManager.USER_PROFILE.IsMine(CurrentGroup.UserID))
            {
                AppManager.NAVIGATION.ShowUserProfile();
            }
            else
            {
                AppManager.VIEW_CONTROLLER.HideNavigationGroup();
                AppManager.VIEW_CONTROLLER.ShowAnotherUserProfile(CurrentGroup.UserID);
            }
        }*/
    }
}
