using System;
using System.Collections;
using System.Collections.Generic;
using InfinityCode.OnlineMapsDemos;
using UnityEngine;
using UnityEngine.UI;
using SocialApp;
using TMPro;

namespace SocialApp
{
    public class RegistrationController : MonoBehaviour
    {

        [SerializeField] private UserProfileLoader _userProfileLoader;
        
        
        [SerializeField]
        private TMP_InputField EmailInput = default;
        [SerializeField]
        private TMP_InputField FullName = default;
        //[SerializeField]
        //private InputField LastNameInput = default;
        [SerializeField]
        private TMP_InputField PasswordInput = default;
        [SerializeField]
        private InputField ConfirmPasswordInput = default;

        private void OnEnable()
        {
            ClearFields();
        }

        public void SendRegistration()
        {
            if (CheckError())
                return;
            string _login = EmailInput.text.Trim();
            string _password = PasswordInput.text.Trim();
            AppManager.FIREBASE_CONTROLLER.AddNewUser(_login, _password, OnRegistrationComplete);
            AppManager.VIEW_CONTROLLER.ShowLoading();
        }

        public void CloseWindow()
        {
            AppManager.VIEW_CONTROLLER.HideRegistration();
            AppManager.VIEW_CONTROLLER.ShowLogin();
        }

        public void OnRegistrationComplete(RegistrationMessage _msg)
        {
            if (_msg.IsComplete)
            {
                string _login = EmailInput.text.Trim();
                string _password = PasswordInput.text.Trim();
                AppManager.FIREBASE_CONTROLLER.LogIn(_login, _password, OnLoginComplete, true);
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

        public void OnLoginComplete(LoginMessage _msg)
        {
            if (_msg.IsSuccess)
            {
                User _user = new User();
                _user.UserID = _msg.UserID;
                _user.DataRegistration = AppManager.DEVICE_CONTROLLER.GetSystemDate(); 
                //  _user.FirstName = FirstNameInput.text;
                //_user.LastName = LastNameInput.text;
                _user.FullName = FullName.text;
                AppManager.FIREBASE_CONTROLLER.SetUserData(_user, OnSetUserDataComplete);
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

        public void OnSetUserDataComplete(SetUserDataMessage _msg)
        {
            if (_msg.IsSuccess)
            {
                if (AppManager.APP_SETTINGS.UseEmailConfirm)
                {
                    AppManager.VIEW_CONTROLLER.ShowPopupMSG(MessageCode.RegistrationSuccessWithConfirm, CloseWindow);
                    AppManager.FIREBASE_CONTROLLER.SendVerifcationEmail();
                }
                else
                {
                    AppManager.VIEW_CONTROLLER.ShowPopupMSG(MessageCode.RegistrationSuccess, CloseWindow);

                    
                    _userProfileLoader.selectPhotoFromURL(_msg.PhotoUrl);
                    
                    
                    
                    
                    
                    

                }
                AppManager.VIEW_CONTROLLER.HideLoading();
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
            /*FirstNameInput.text = string.Empty;
            LastNameInput.text = string.Empty;*/
            FullName.text=String.Empty;
            
            PasswordInput.text = string.Empty;
            ConfirmPasswordInput.text = string.Empty;
        }

        private bool CheckError()
        {
            bool IsError = false;
            if (string.IsNullOrEmpty(EmailInput.text))
            {
                AppManager.VIEW_CONTROLLER.ShowPopupMSG(MessageCode.EmptyEmail);
                IsError = true;
            }
            /*if (string.IsNullOrEmpty(FirstNameInput.text))
            {
                AppManager.VIEW_CONTROLLER.ShowPopupMSG(MessageCode.EmptyFirstName);
                IsError = true;
            }
            if (string.IsNullOrEmpty(LastNameInput.text))
            {
                AppManager.VIEW_CONTROLLER.ShowPopupMSG(MessageCode.EmptyLastName);
                IsError = true;
            }*/
            else if (string.IsNullOrEmpty(PasswordInput.text))
            {
                AppManager.VIEW_CONTROLLER.ShowPopupMSG(MessageCode.EmptyPassword);
                IsError = true;
            }
            /*else if (!string.Equals(ConfirmPasswordInput.text, ConfirmPasswordInput.text))
            {
                AppManager.VIEW_CONTROLLER.ShowPopupMSG(MessageCode.PasswordNotMatch);
                IsError = true;
            }*/
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
    }
}


