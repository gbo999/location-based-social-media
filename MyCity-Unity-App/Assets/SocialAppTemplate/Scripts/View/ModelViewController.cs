using TriLibCore;
using UnityEngine;
using UnityEngine.UI;


namespace SocialApp
{
    public class ModelViewController : MonoBehaviour
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
        private Text ModelName = default;
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
        private Model3D CurrentModel;


        private void ClearData()
        {
            /*AvatarView.DisplayDefaultAvatar();
            GroupName.text = string.Empty;
            CurrentGroup = null;
            HideAllBtns();
            OnlineImage.color = OfflineColor;*/
        }

        public void DisplayInfo(Model3D _model)
        {
            ClearData();
            AvatarView.SetCacheTexture(CacheAvatar);
            CurrentModel = _model;
            ModelName.text = CurrentModel.Name;
            getThumb();
            
            /*  DisplayButtons();
            OnlineController.SetUser(CurrentGroup.UserID);
            OnlineController.SetUpdateAction(OnOnlineStatusUpdated);
            OnlineController.StartCheck();*/
        }


        public void ChosenGroup()
        {

            AppManager.myCityController.CurrentModelID = CurrentModel.ModelURl;
           // AppManager.myCityController.currentTag = CurrentModel.tag;


            Debug.Log(AppManager.myCityController.CurrentModelID);

            //  AppManager.NAVIGATION.ShowMaps();


            var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions();
            var webRequest = AssetDownloader.CreateWebRequest(CurrentModel.ModelURl);
            AssetDownloader.LoadModelFromUri(webRequest, OnLoad, OnMaterialsLoad, OnProgress, OnError, null, assetLoaderOptions, fileExtension: CurrentModel.FileExtension);



        }

        private void OnError(IContextualizedError obj)
        {
            Debug.LogError($"An error occurred while loading your Model: {obj.GetInnerException()}");
        }

        /// <summary>
        /// Called when the Model loading progress changes.
        /// </summary>
        /// <param name="assetLoaderContext">The context used to load the Model.</param>
        /// <param name="progress">The loading progress.</param>
        private void OnProgress(AssetLoaderContext assetLoaderContext, float progress)
        {
            Debug.Log($"Loading Model. Progress: {progress:P}");
        }

        /// <summary>
        /// Called when the Model (including Textures and Materials) has been fully loaded.
        /// </summary>
        /// <remarks>The loaded GameObject is available on the assetLoaderContext.RootGameObject field.</remarks>
        /// <param name="assetLoaderContext">The context used to load the Model.</param>
        private void OnMaterialsLoad(AssetLoaderContext assetLoaderContext)
        {
            Debug.Log("Materials loaded. Model fully loaded.");

            AppManager.myCityController.ModelAsGameObject = assetLoaderContext.RootGameObject;

            AppManager.myCityController.ModelAsGameObject.AddComponent<BoxCollider>();

            //AppManager.myCityController.ModelAsGameObject.GetComponent<AssetDownloaderBehaviour>();


            GameObject g = GameObject.Find("Root");

            g.transform.position = new Vector3(500, 500);
           

            // Destroy(assetLoaderContext.RootGameObject);

        }

        /// <summary>
        /// Called when the Model Meshes and hierarchy are loaded.
        /// </summary>
        /// <remarks>The loaded GameObject is available on the assetLoaderContext.RootGameObject field.</remarks>
        /// <param name="assetLoaderContext">The context used to load the Model.</param>
        private void OnLoad(AssetLoaderContext assetLoaderContext)
        {
            Debug.Log("Model loaded. Loading materials.");
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

        private void getThumb()
        {
            AvatarView.LoadThumb(CurrentModel);
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
