using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Google;
using SocialApp;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GoogleSignInDemo : MonoBehaviour
{
    public Text infoText;
    public string webClientId = "<your client id here>";
    [SerializeField] private UIElementsGroup testWindow;

    private FirebaseAuth auth;

    private User _user;
    
    
   // private FirebaseAuth auth;
    private GoogleSignInConfiguration configuration;

    private void Awake()
    {
        //CheckFirebaseDependencies();
    }

    /*private void CheckFirebaseDependencies()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                if (task.Result == DependencyStatus.Available)
                    auth = FirebaseAuth.DefaultInstance;
                else
                    AddToInformation("Could not resolve all Firebase dependencies: " + task.Result.ToString());
            }
            else
            {
                AddToInformation("Dependency check was not completed. Error : " + task.Exception.Message);
            }
        });
    }*/

    public void SignInWithGoogle()
    {
        testWindow.ChangeVisibility(true);
        OnSignIn();
    }
    public void SignOutFromGoogle() { OnSignOut(); }

    private void OnSignIn()
    {
        configuration = new GoogleSignInConfiguration { WebClientId = webClientId, RequestEmail = true, RequestIdToken = true };

        
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        
        AddToInformation("Calling SignOut");
   
        try
        { 
            
            
            
        AddToInformation("acount name: "+configuration.AccountName);




        if (GoogleSignIn.DefaultInstance == null)
        {
            
            AddToInformation("it is null");
            
        }
        
        
        
        

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnAuthenticationFinished, TaskScheduler.FromCurrentSynchronizationContext());
            
            AddToInformation("after task");
            
            
        }
        catch (Exception e)
        {
            
            AddToInformation(e.ToString());
        }
        
    }

    private void OnSignOut()
    {
        AddToInformation("Calling SignOut");
        GoogleSignIn.DefaultInstance.SignOut();
    }

    public void OnDisconnect()
    {
        AddToInformation("Calling Disconnect");
        GoogleSignIn.DefaultInstance.Disconnect();
    }

    internal void OnAuthenticationFinished(Task<GoogleSignInUser> task)
    {

        try
        {

            if (task.IsFaulted)
            {
                using (IEnumerator<Exception> enumerator = task.Exception.InnerExceptions.GetEnumerator())
                {
                    if (enumerator.MoveNext())
                    {
                        GoogleSignIn.SignInException error = (GoogleSignIn.SignInException) enumerator.Current;
                        AddToInformation("Got Error: " + error.Status + " " + error.Message);
                    }
                    else
                    {
                        AddToInformation("Got Unexpected Exception?!?" + task.Exception);
                    }
                }
            }
            else if (task.IsCanceled)
            {
                AddToInformation("Canceled");
            }
            else
            {
                AddToInformation("Welcome: " + task.Result.DisplayName + "!");
                AddToInformation("Email = " + task.Result.Email);
             //   AddToInformation("Google ID Token = " + task.Result.IdToken);
              //  AddToInformation("Email = " + task.Result.Email);
                AddToInformation("photo" + task.Result.ImageUrl);

             
                
                
                
                
                _user = new User
                {
                    FullName = task.Result.DisplayName,
                    imageURL = task.Result.ImageUrl.ToString()
                };


                SignInWithGoogleOnFirebase(task.Result.IdToken);
            }
        }

        catch (Exception e)
        {
            
            AddToInformation(e.ToString());
        }




    }

    private void SignInWithGoogleOnFirebase(string idToken)
    {
        Credential credential = GoogleAuthProvider.GetCredential(idToken, null);

      AppManager.Instance.auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
        {
            AggregateException ex = task.Exception;
            if (ex != null)
            {
                if (ex.InnerExceptions[0] is FirebaseException inner && (inner.ErrorCode != 0))
                    AddToInformation("\nError code = " + inner.ErrorCode + " Message = " + inner.Message);
            }
            else
            {
                AddToInformation("Sign In Successful.");

                
                
                
             //   AppManager.VIEW_CONTROLLER.ShowLoading();
                
                _user.DataRegistration = AppManager.DEVICE_CONTROLLER.GetSystemDate();
                _user.UserID = task.Result.UserId;

                PopupMessage msg = new PopupMessage();

               AddToInformation("after userID"+_user.UserID);

               try
               {
                   AppManager.FIREBASE_CONTROLLER.SetUserData(_user, AppManager.REGISTRATION_CONTROLLER.OnSetUserDataComplete);


               }


               catch (Exception e)
               {
                   
                   AddToInformation(e.ToString());
               }
            //    testWindow.ChangeVisibility(false);
                
            }
        });
    }


    
    
    public void OnSignInSilently()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        AddToInformation("Calling SignIn Silently");

        GoogleSignIn.DefaultInstance.SignInSilently().ContinueWith(OnAuthenticationFinished);
    }

    public void OnGamesSignIn()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = true;
        GoogleSignIn.Configuration.RequestIdToken = false;

        AddToInformation("Calling Games SignIn");

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnAuthenticationFinished);
    }

    private void AddToInformation(string str) { infoText.text += "\n" + str; }
}