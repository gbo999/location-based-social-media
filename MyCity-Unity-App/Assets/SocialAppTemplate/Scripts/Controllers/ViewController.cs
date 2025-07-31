using UnityEngine;
using System;

namespace SocialApp
{
    public class ViewController : MonoBehaviour
    {

        [SerializeField]
        private Camera MainCamera = default;
        [SerializeField]
        private GameObject LoadingScreen = default;
        [SerializeField]
        private GameObject PopupObject = default;
        [SerializeField]
        private GameObject RegistrationObject = default;
        [SerializeField]
        private GameObject UserProfileObject = default;
        [SerializeField]
        private GameObject WorldNewsObject = default;
        [SerializeField]
        private GameObject FriendsNewsObject = default;
        [SerializeField]
        private GameObject LoginObject = default;
        [SerializeField]
        private GameObject FeedPreviewObject = default;
        [SerializeField]
        private GameObject FriendListObject = default;
        [SerializeField]
        private GameObject SettingsObject = default;
        [SerializeField]
        private GameObject NavigationPanelObject = default;
        [SerializeField]
        private GameObject AnotherUserProfileObject = default;
        [SerializeField]
        private GameObject UserFriendsіListObject = default;
        [SerializeField]
        private GameObject MessegesListObject = default;
        [SerializeField]
        private GameObject MessegingObject = default;
        [SerializeField]
        private GameObject CommentsObject = default;
        [SerializeField]
        private GameObject AddNewShatObject = default;
        [SerializeField]
        private GameObject FeedPopupObject = default;
        [SerializeField]
        private GameObject CallWindowObject = default;



        [SerializeField] private UIElementsGroup PopupMessageWindow;
        
        
        
        

        [SerializeField]
        private GameObject GroupCreate = default;

        [SerializeField]
        private GameObject GroupSearch = default;

        [SerializeField]
        private GameObject CHooseGRoupPicture = default;


        [SerializeField]
        private GameObject maps = default;

        [SerializeField]
        private GameObject UploadModel;


        [SerializeField]
        private GameObject ModelSearch;


        [SerializeField]
        private GameObject AR;


        [SerializeField] private GameObject _mapsCamera;
        
        public void ShowAR()
        {
          AR.SetActive(true);
        }

        public void HideAR()
        {
            AR.SetActive(false);
        }

        //maps
        public void ShowMaps()
        {

            Debug.Log("this is showing the maps now");

            _mapsCamera.SetActive(true);
            maps.SetActive(true);
        
        
        }

        public void HideMaps()
        {
            maps.SetActive(false);
        }

        public void ShowUploadModel()
        {
            UploadModel.SetActive(true);
        }

        public void HideUploadModel()
        {
            UploadModel.SetActive(false);
        }





        //search group
        public void ShowSearchGroup()
        {
            GroupSearch.SetActive(true);
        }

        public void HideSearchGroup()
        {
            GroupSearch.SetActive(false);
        }
        
        public void ShowSearchModel()
        {
            ModelSearch.SetActive(true);
        }
/*
        public void HideSearchModel()
        {
            ModelSearch.SetActive(false);
        }*/


        // camera
        public Camera GetMainCamera()
        {
            return MainCamera;
        }

        // popup
        public void ShowPopupMessage(PopupMessage _msg)
        {
            PopupMessageWindow.ChangeVisibility(true);
          //  PopupObject.SetActive(true);
          
            PopupMessageWindow.GetComponent<PopupController>().ShowMessage(_msg);
        }

        public void HidePopupMessage()
        {
            PopupMessageWindow.gameObject.SetActive(false);

           // PopupObject.SetActive(false);
        }

        //create group
        public void ShowCreateGroup()
        {
            GroupCreate.SetActive(true);
        }

        public void HideGroupCreate()
        {
            GroupCreate.SetActive(false);
        }


        //group picture
        public void ShowChooseGroupPicture()
        {
            CHooseGRoupPicture.SetActive(true);
        }

        public void HideChooseGroupPicture()
        {
            CHooseGRoupPicture.SetActive(false);
        }











        // loading
        public void ShowLoading()
        {
            LoadingScreen.SetActive(true);
        }

        public void HideLoading()
        {
            Debug.Log("hide loading");
            
            LoadingScreen.SetActive(false);
        }

        // registration
        public void ShowRegistration()
        {
            RegistrationObject.SetActive(true);
        }

        public void HideRegistration()
        {
            RegistrationObject.SetActive(false);
        }

        // login
        public void ShowLogin()
        {
            LoginObject.SetActive(true);
        }

        public void HideLogin()
        {
            LoginObject.SetActive(false);
        }

        // user profile
        public void ShowUserProfile()
        {
            UserProfileObject.SetActive(true);
        }

        public void HideUserProfile()
        {
            UserProfileObject.SetActive(false);
        }

        // message list
        public void ShowMessageList()
        {
            MessegesListObject.SetActive(true);
        }

        public void HideMessageList()
        {
            MessegesListObject.SetActive(false);
        }

        // another user profile
        public void ShowAnotherUserProfile(string _id)
        {
            AnotherUserProfileObject.SetActive(true);
            AnotherUserProfileObject.GetComponentInChildren<UserProfileLoader>().LoadUserInfo(_id);
            AnotherUserProfileObject.GetComponentInChildren<FeedsDataLoader>().LoadUserContent(_id);
        }

        public void HideAnotherUserProfile()
        {
            AnotherUserProfileObject.SetActive(false);
        }

        // show user friends
        public void ShowUserFriend(string _id)
        {
            UserFriendsіListObject.SetActive(true);
            UserFriendsіListObject.GetComponentInChildren<FriendsListLoader>().LoadUserFriends(_id);
        }

        public void HideUserFriends()
        {
            UserFriendsіListObject.SetActive(false);
        }

        // feed preview
        public void ShowFeedPreview(FeedPreviewRequest _request)
        {
            FeedPreviewObject.SetActive(true);
            FeedPreviewObject.GetComponent<FeedPreviewController>().DisplayPreview(_request);
        }

        public void HideFeedPreview()
        {
            FeedPreviewObject.SetActive(false);
        }

        // friend list
        public void ShowFriendsList()
        {
            FriendListObject.SetActive(true);
        }

        public void HideFriendsList()
        {
            FriendListObject.SetActive(false);
        }

        // settings
        public void ShowSettings()
        {
            SettingsObject.SetActive(true);
        }

        public void HideSettings()
        {
            SettingsObject.SetActive(false);
        }

        // navigation
        public void ShowNavigationPanel()
        {
            NavigationPanelObject.SetActive(true);
            AppManager.NAVIGATION.AddListeners();
        }

        public void HideNavigationPanel()
        {
            NavigationPanelObject.SetActive(false);
            
            Debug.Log("nav remove view");
           // AppManager.NAVIGATION.RemoveListeners();
        }

        // world news
        public void ShowWorldNews()
        {
            WorldNewsObject.SetActive(true);
        }

        public void HideWorldNews()
        {
            WorldNewsObject.SetActive(false);
        }

        // friends news
        public void ShowFriendsNews()
        {
            FriendsNewsObject.SetActive(true);
        }

        public void HideFriendsNews()
        {
            FriendsNewsObject.SetActive(false);
        }

        // show messages with
        public void ShowMessagingWith(string _id)
        {
            HideNavigationPanel();
            MessegingObject.SetActive(true);
            MessegingObject.GetComponentInChildren<MessagesDataLoader>().LoadUserMessages(_id);
        }

        public void ShowMessagingWith(MessageGroupInfo _groupID)
        {
            HideNavigationPanel();
            MessegingObject.SetActive(true);
            MessegingObject.GetComponentInChildren<MessagesDataLoader>().LoadMessageGroup(_groupID);
        }

        public void HideUserMessanging()
        {
            MessegingObject.SetActive(false);
        }

        // show post comments
        public void ShowPostComments(string _id)
        {
            HideNavigationPanel();
            CommentsObject.SetActive(true);
            CommentsObject.GetComponentInChildren<MessagesDataLoader>().LoadPostComments(_id);
        }

        public void HidePostComments()
        {
            CommentsObject.SetActive(false);
        }

        // show add chat window
        public void ShowAddNewChat()
        {
            AddNewShatObject.SetActive(true);
            AddNewShatObject.GetComponentInChildren<SelectFromFriendsLoader>().LoadWindow(AddNewChatType.ADD_NEW_CHAT);
            HideNavigationPanel();
        }

        public void ShowAddNewChatMembers(MessageGroupInfo _group)
        {
            AddNewShatObject.SetActive(true);
            AddNewShatObject.GetComponentInChildren<SelectFromFriendsLoader>().LoadWindow(AddNewChatType.ADD_NEW_MEMBERS, _group);
        }

        public void ShowChatMembers(MessageGroupInfo _group)
        {
            AddNewShatObject.SetActive(true);
            AddNewShatObject.GetComponentInChildren<SelectFromFriendsLoader>().LoadWindow(AddNewChatType.SHOW_CHAT_MEMBERS, _group);
        }

        public void HideAddNEwChat()
        {
            AddNewShatObject.SetActive(false);
        }

        public void ShowFeedPopup(Action<FeedPopupAction> _action)
        {
            FeedPopupObject.SetActive(true);
            FeedPopupObject.GetComponent<FeedPopupViewController>().SetupWindows(_action);
        }

        public void HideFeedPopup()
        {
            FeedPopupObject.SetActive(false);
        }

        public void StartCall(IncommingType _type, CallObject _call)
        {
            CallWindowObject.SetActive(true);
            CallWindowObject.GetComponent<CallController>().ShowIncomming(_type, _call);
        }

        public void HideCall()
        {
            CallWindowObject.SetActive(false);
        }

        public bool IsCallWindowActive()
        {
            return CallWindowObject.activeInHierarchy;
        }

        // all
        public void HideAllScreen()
        {
            HideLogin();
            HidePopupMessage();
            HideRegistration();
            HideLoading();
            HideFeedPreview();
            HideUserProfile();
            HideSettings();
            HideFriendsList();
            HideNavigationPanel();
            HideWorldNews();
            HideFriendsNews();
            HideAnotherUserProfile();
            HideUserFriends();
            HideUserMessanging();
            HidePostComments();
            HideAddNEwChat();
            HideFeedPopup();
            HideCall();
        }

        // hide navigation group objects
        public void HideNavigationGroup()
        {
            HideUserProfile();
            HideSettings();
            HideFriendsList();
            HideWorldNews();
            HideFriendsNews();
            HideAnotherUserProfile();
            HideUserFriends();
            HideMessageList();
            HideUserMessanging();
            HidePostComments();
            HideSearchGroup();
            HideMaps();
            //HideSearchModel();
            HideAR();
        }

        public void ShowPopupMSG(MessageCode _code, Action _callback = null)
        {
            PopupMessage msg = new PopupMessage();
            msg.Callback = _callback;
            switch (_code)
            {
                case MessageCode.EmptyEmail:
                    msg.Title = "Error";
                    msg.Message = "Email is empty";
                    break;
                case MessageCode.EmptyFirstName:
                    msg.Title = "Error";
                    msg.Message = "First Name is empty";
                    break;
                case MessageCode.EmptyLastName:
                    msg.Title = "Error";
                    msg.Message = "Last Name is empty";
                    break;
                case MessageCode.EmptyPassword:
                    msg.Title = "Error";
                    msg.Message = "Password is empty";
                    break;
                case MessageCode.PasswordNotMatch:
                    msg.Title = "Error";
                    msg.Message = "Passwords do not match";
                    break;
                case MessageCode.EmailNotValid:
                    msg.Title = "Error";
                    msg.Message = "Email is not valid";
                    break;
                case MessageCode.SmallPassword:
                    msg.Title = "Error";
                    msg.Message = "Password is too small. Min value is " + AppManager.APP_SETTINGS.MinAllowPasswordCharacters.ToString();
                    break;
                case MessageCode.RegistrationSuccess:
                    msg.Title = "Success";
                    msg.Message = "Registration Success!";
                    break;
                case MessageCode.RegistrationSuccessWithConfirm:
                    msg.Title = "Success";
                    msg.Message = "Registration Success! Please confirm your email address";
                    break;
                case MessageCode.VideoProcessing:
                    msg.Title = "Error";
                    msg.Message = "Video processing ...";
                    break;
                case MessageCode.MaxVideoSize:
                    msg.Title = "Error";
                    msg.Message = "Max allowed size is " + AppManager.APP_SETTINGS.MaxUploadVideoSizeMB.ToString() + " mb";
                    break;
                case MessageCode.FailedUploadFeed:
                    msg.Title = "Error";
                    msg.Message = "Fail to upload feed. Try again";
                    break;
                case MessageCode.EmailConfirm:
                    msg.Title = "Error";
                    msg.Message = "Please confirm your email address";
                    break;
                case MessageCode.FailedUploadImage:
                    msg.Title = "Error";
                    msg.Message = "Fail to upload image. Try again";
                    break;
                case MessageCode.SuccessPost:
                    msg.Title = "Success";
                    msg.Message = "Post add success";
                    break;
                case MessageCode.DeleteFeedOwnerError:
                    msg.Title = "Error";
                    msg.Message = "You are not the owner of this post";
                    break;
                case MessageCode.CallIsBisy:
                    msg.Title = "Line is bisy";
                    msg.Message = "User cannot speak now";
                    break;
                case MessageCode.AddGroupSuccess:
                    msg.Title = "successs";
                    msg.Message = "now choose icon";
                    break;


                default:
                    Debug.Log("NOTHING");
                    break;



            }
            ShowPopupMessage(msg);
        }
    }

    public enum MessageCode
    {

        AddGroupSuccess,
        EmptyEmail,
        EmptyFirstName,
        EmptyLastName,
        EmptyPassword,
        PasswordNotMatch,
        EmailNotValid,
        SmallPassword,
        RegistrationSuccess,
        RegistrationSuccessWithConfirm,
        VideoProcessing,
        MaxVideoSize,
        FailedUploadFeed,
        FailedUploadImage,
        SuccessPost,
        EmailConfirm,
        DeleteFeedOwnerError,
        CallIsBisy
    }
}