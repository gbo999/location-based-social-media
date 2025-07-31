using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SocialApp;
using System;
using InfinityCode.OnlineMapsDemos;
using TMPro;

namespace SocialApp
{
    public class LoginController : MonoBehaviour
    {

        [SerializeField]
        private TMP_InputField EmailInput = default;
        [SerializeField]
        private TMP_InputField PasswordInput = default;

        public UIElementsGroup LoginScreen;

      //  public UIBubblePopup BubblePopup;
        
        public event Action OnLoginEvent;
        public event Action OnLogoutEvent;

        private void OnEnable()
        {
            ClearFields();
        }

        public void SendLogIn()
        {
            if (CheckError())
                return;
            string _login = EmailInput.text.Trim();
            string _password = PasswordInput.text.Trim();
            OnLogin(_login, _password);
        }

        public void AutoLogin(string _mail, string _password)
        {
            OnLogin(_mail, _password);
        }

        private void OnLogin(string _mail, string _password)
        {
            AppManager.VIEW_CONTROLLER.ShowLoading();
            AppManager.FIREBASE_CONTROLLER.LogIn(_mail, _password, OnLoginSuccess);
        }

        public void OnRegistration()
        {
            AppManager.VIEW_CONTROLLER.HideLogin();
            AppManager.VIEW_CONTROLLER.ShowRegistration();
        }

        public void OnLoginSuccess(LoginMessage _msg)
        {
            if (_msg.IsSuccess)
            {
                /*if (!string.IsNullOrEmpty(EmailInput.text))
                {
                    PlayerPrefs.SetString(AppSettings.LoginSaveKey, EmailInput.text.Trim());
                    PlayerPrefs.SetString(AppSettings.PasswordSaveKey, PasswordInput.text.Trim());
                    PlayerPrefs.Save();
                }*/
                
                
                AppManager.USER_PROFILE.FIREBASE_USER = _msg.FUser;
                AppManager.VIEW_CONTROLLER.HideLogin();
                AppManager.VIEW_CONTROLLER.HideLoading();


                /*AppManager.NAVIGATION.ShowUserProfile();
                AppManager.VIEW_CONTROLLER.ShowNavigationPanel();
                */
                LoginScreen.ChangeVisibility(false);
                
                
                AppManager.NAVIGATION.ShowMaps();
                
                //BubblePopup.UploadMapAvatar();

                

                Debug.Log("in login controller show maps");

                AppManager.DEVICE_CONTROLLER.StartOnlineChecker();
                AppManager.FIREBASE_CONTROLLER.InitPushNotificationEvents();

                OnLoginEvent?.Invoke();
            }
            else
            {
                PopupMessage msg = new PopupMessage();
                msg.Title = "Error";
                msg.Message = _msg.ErrorMessage;
                AppManager.VIEW_CONTROLLER.ShowPopupMessage(msg);
                AppManager.VIEW_CONTROLLER.HideLoading();
            }
        }

        private void ClearFields()
        {
            EmailInput.text = string.Empty;
            PasswordInput.text = string.Empty;
        }

        private bool CheckError()
        {
            bool IsError = false;
            if (string.IsNullOrEmpty(EmailInput.text))
            {
                AppManager.VIEW_CONTROLLER.ShowPopupMSG(MessageCode.EmptyEmail);
                IsError = true;
            }
            else if (string.IsNullOrEmpty(PasswordInput.text))
            {
                AppManager.VIEW_CONTROLLER.ShowPopupMSG(MessageCode.EmptyPassword);
                IsError = true;
            }
            else if (!EmailInput.text.Contains(AppManager.APP_SETTINGS.EmailValidationCharacter))
            {
                AppManager.VIEW_CONTROLLER.ShowPopupMSG(MessageCode.EmailNotValid);
                IsError = true;
            }
            else if (PasswordInput.text.Length < AppManager.APP_SETTINGS.MinAllowPasswordCharacters)
            {
                AppManager.VIEW_CONTROLLER.ShowPopupMSG(MessageCode.SmallPassword);
                IsError = true;
            }
            return IsError;
        }

        
        
        
        
        
        
        
        
        public void OnSignOut()
        {
            OnLogoutEvent?.Invoke();
        }
    }
}
