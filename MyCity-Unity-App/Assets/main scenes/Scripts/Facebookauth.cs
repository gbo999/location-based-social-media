using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using Firebase.Auth;
using System;
using SocialApp;
using UnityEngine.UI;
using System.Threading.Tasks;

public class Facebookauth : MonoBehaviour
{

    [SerializeField] private UIElementsGroup FacebookTestWindow;
    [SerializeField] private Text debugText;
    
  //  FirebaseAuth auth;
    private void Awake()
    {
        if (!FB.IsInitialized)
        {
            FB.Init(InitCallBack, OnHideUnity);
        }
        else
        {
            FB.ActivateApp();
        }
    }
    private void InitCallBack()
    {
        if (FB.IsInitialized)
        {
            FB.ActivateApp();
        }
        else
        {
            debugText.text += "Failed to initialize";
            Debug.Log("Failed to initialize");
        }
    }
    private void OnHideUnity(bool isgameshown)
    {
        if (!isgameshown)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    public void Facebook_Login()
    {
        FacebookTestWindow.ChangeVisibility(true);
        var permission = new List<string>() { "public_profile", "email" };
        FB.LogInWithReadPermissions(permission, AuthCallBack);
        
        
        
    }

    private void AuthCallBack(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
          //  debug.text = (aToken.UserId);

            string accesstoken;
            string[] data;
            string acc;
            string[] some;
#if UNITY_EDITOR
            
            Debug.Log("this is raw access " + result.RawResult);
            
            debugText.text+="this is raw access " + result.RawResult;

            data = result.RawResult.Split(',');
            debugText.text += "this is access" + data[3];

            Debug.Log("this is access" + data[3]);
            acc = data[3];
            some = acc.Split('"');
            debugText.text += "this is access " + some[3];

            Debug.Log("this is access " + some[3]);
            accesstoken = some[3];
#elif UNITY_ANDROID
            Debug.Log("this is raw access "+result.RawResult);
            debugText.text += "this is raw access "+result.RawResult;

 data = result.RawResult.Split(',');
            Debug.Log("this is access"+data[0]);
            debugText.text += "this is access"+data[0];

             acc = data[0];
             some = acc.Split('"');
            Debug.Log("this is access " + some[3]);
            debugText.text += "this is access " + some[3];


             accesstoken = some[3];
#endif
            try
            {
                authwithfirebase(accesstoken);

                debugText.text += '\n' + "is auth firebase";

            }

            catch (Exception e)
            {
                debugText.text += '\n' + e.ToString();
            }

        }
        else
        {
            
            debugText.text += '\n'+"User Cancelled login";
            
          Debug.Log("User Cancelled login");
        }
    }

    public void authwithfirebase(string accesstoken)
    {

        Credential credential = FacebookAuthProvider.GetCredential(accesstoken);

        debugText.text += "\n" + "after cred";



        AppManager.Instance.auth.SignInWithCredentialAsync(credential).ContinueWith(onresult, TaskScheduler.FromCurrentSynchronizationContext());
            
        
               
          
}

    void onresult(Task<FirebaseUser> task)
    {
        
        if (task.IsFaulted)
        {
            debugText.text += '\n' + "singin encountered error" + task.Exception;

            Debug.Log("singin encountered error" + task.Exception);
            return;
        }

        if (task.IsFaulted)
        {

            debugText.text += '\n' + "SignInWithCredentialAsync encountered an error: " + task.Exception;

            Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
            return;
        }


        Firebase.Auth.FirebaseUser newuser = task.Result;

        debugText.text += '\n' + "username: " + newuser.DisplayName;
        debugText.text += '\n' + "photo: " + newuser.PhotoUrl;
        debugText.text += '\n' + "email: " + newuser.Email;





        // Debug.Log("usernewuser.DisplayName);
    }
        
    }