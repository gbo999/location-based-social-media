using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SocialApp
{
    public class SettingsController : MonoBehaviour
    {

        [SerializeField] private TMP_Text debugText;

        [SerializeField] private UIElementsGroup debugWindow;

        public Image avatarImage;

        public Sprite defaultAvatar;

        public UIElementsGroup LoginScreen;

        public UIElementsGroup quitScreen;

        
        

        public UIElementsGroup signupScreen;


        public void showSignup()
        {
            LoginScreen.ChangeVisibility(false);

            signupScreen.ChangeVisibility(true);
            
            
        }

        public void closeSignup()
        {
            
            signupScreen.ChangeVisibility(false);
        }
        
        
        
        public void showQuit()
        {
            
            quitScreen.ChangeVisibility(true);
            
            
        }

        public void closeQuit()
        {
            
            quitScreen.ChangeVisibility(false);
        }
        
        
        public void Logout()
        {

            try
            {
                Debug.Log("nave remove logout");

                // AppManager.NAVIGATION.RemoveListeners();
                PlayerPrefs.DeleteAll();
                // AppManager.VIEW_CONTROLLER.HideAllScreen();
                // AppManager.VIEW_CONTROLLER.ShowLogin();


                AppManager.DEVICE_CONTROLLER.StopOnlineChecker();
                 AppManager.FIREBASE_CONTROLLER.RemoveDeviceTokens();
                AppManager.FIREBASE_CONTROLLER.RemovePushNotificationEvents();

                avatarImage.sprite = defaultAvatar;


                //AppManager.LOGIN_CONTROLLER.OnSignOut();


                //TODO: clean  avatar and profile 






                AppManager.FIREBASE_CONTROLLER.LogOut();
                AppManager.USER_PROFILE.ClearUser();

                closeQuit();


                LoginScreen.ChangeVisibility(true);
            }

            catch (Exception e)
            {
                debugWindow.ChangeVisibility(true);

                debugText.text = e.ToString();
            }
            
        }
    }
}
