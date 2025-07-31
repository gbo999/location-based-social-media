using System;
using System.Collections;
using System.Collections.Generic;
using SocialApp;
using UnityEngine;

public class SignInController : MonoBehaviour
{
    [SerializeField] private UIElementsGroup LoginScreen;
    [SerializeField] private GameObject pinCreator;
    
    
    
    
    
    
    // Start is called before the first frame update
    public void SignINAnonymous()
    {
        AppManager.VIEW_CONTROLLER.ShowLoading();

        
        AppManager.Instance.auth.SignInAnonymouslyAsync().ContinueWith(task => {
           
            
            if (task.IsCanceled) {
                Debug.LogError("SignInAnonymouslyAsync was canceled.");
                return;
            }
            if (task.IsFaulted) {
                Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;

            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);


            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                
                AppManager.VIEW_CONTROLLER.HideLoading();
                
                LoginScreen.ChangeVisibility(false);
                pinCreator.SetActive(false);     
                
            });


                  
        });
        
        
    }
    
    
    
    
    
    
    
    
}


