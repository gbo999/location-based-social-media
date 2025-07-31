using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using UnityEngine;
using SocialApp;
using Firebase.Database;

namespace SocialApp
{

    public class AppManager : MonoBehaviour
    {


        public FirebaseUser user;

        public FirebaseAuth auth;

        public UIElementsGroup LoginScreen;

        [SerializeField] private GameObject PinCreator;
        

        // instance
        private static AppManager instance;
        public static AppManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = GameObject.FindObjectOfType<AppManager>();
                }
                return instance;
            }

        }

        // device
        public DeviceController Device;
        public static DeviceController DEVICE_CONTROLLER
        {
            get
            {
                return Instance.Device;
            }
        }


        //myCityManager
        public MyCityManager myCityMananger;

        public static MyCityManager myCityController { 
        
            get
            {
              return Instance.myCityMananger;
            }

        }
              



        // firebase
        public FirebaseController Firebase;
        public static FirebaseController FIREBASE_CONTROLLER
        {
            get
            {
                if (Instance == null)
                {
                    return null;
                }
                else
                {
                    return Instance.Firebase;
                }
            }
        }

        // registration
        public RegistrationController Registration;
        public static RegistrationController REGISTRATION_CONTROLLER
        {
            get
            {
                return Instance.Registration;
            }
        }

        // login
        public LoginController Login;
        public static LoginController LOGIN_CONTROLLER
        {
            get
            {
                return Instance.Login;
            }
        }

        // view
        public ViewController View;
        public static ViewController VIEW_CONTROLLER
        {
            get
            {
                return Instance.View;
            }
        }

        // app setings
        private AppSettings Settings;
        public static AppSettings APP_SETTINGS
        {
            get
            {
                if (Instance == null)
                    return null;
                if (Instance.Settings == null)
                {
                    Instance.Settings = Resources.Load<AppSettings>(AppSettings.AppSettingPath);
                }
                return Instance.Settings;
            }
        }

        // profile
        public ProfileController Profile;
        public static ProfileController USER_PROFILE
        {
            get
            {
                if (Instance == null)
                {
                    return null;
                }
                else
                {
                    return Instance.Profile;
                }

            }
        }

        // navigation
        public NavigationController Navigation;
        public static NavigationController NAVIGATION
        {
            get
            {
                return Instance.Navigation;
            }
        }

        // settings
        public SettingsController UserSettings;
        public static SettingsController USER_SETTINGS
        {
            get
            {
                return Instance.UserSettings;
            }
        }


        public ModelUIController ModelUIController;
        public static ModelUIController Model_UI_CONTROLLER
        {
            get
            {
                return Instance.ModelUIController;
            }
        }



        public GroupUIController GroupUIController;
        public static GroupUIController GROUP_UI_CONTROLLER
        {
            get
            {
                return Instance.GroupUIController;
            }
        }
        // friends ui
        public FriendsUIController FriendsUI;
        public static FriendsUIController FRIEND_UI_CONTROLLER
        {
            get
            {
                return Instance.FriendsUI;
            }
        }

        // settings
        public AgoraController AgoraController;
        public static AgoraController AGORA_CONTROLLER
        {
            get
            {
                return Instance.AgoraController;
            }
        }

        private void Start()
        {
            Init();
            AddListeners();
        }

        private void OnDestroy()
        {
            RemoveListeners();
            RemoveCallListener();
        }

        private void AddListeners()
        {
            LOGIN_CONTROLLER.OnLoginEvent += OnLoginSuccess;
            LOGIN_CONTROLLER.OnLogoutEvent += OnLogoutSuccess;
        }

        private void RemoveListeners()
        {
            LOGIN_CONTROLLER.OnLoginEvent -= OnLoginSuccess;
            LOGIN_CONTROLLER.OnLogoutEvent -= OnLogoutSuccess;
        }

        private void Init()
        {
            
           
           
            
            Application.targetFrameRate = 60;
            VIEW_CONTROLLER.HideAllScreen();
            FIREBASE_CONTROLLER.InitFirebase();
            if (APP_SETTINGS._EnableVideoAudioCalls)
            {
#if PLATFORM_ANDROID
                if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.Microphone))
                {
                    UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.Microphone);
                }
                if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.Camera))
                {
                    UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.Camera);
                }
#endif
                AgoraController.Init();
            }
            StartCoroutine(WaitForFirebaseReady());
        }

        private IEnumerator WaitForFirebaseReady()
        {
            VIEW_CONTROLLER.ShowLoading();

           // Screen.orientation = ScreenOrientation.Portrait;

           Screen.orientation = ScreenOrientation.Landscape;
           
            while (!FIREBASE_CONTROLLER.IsFirebaseInited())
            {
                yield return new WaitForFixedUpdate();
            }
            
            
            
            Navigation.ShowMaps();
            
            VIEW_CONTROLLER.HideLoading();


            auth = FIREBASE_CONTROLLER.Auth;
            
            
            InitUser();







            // CheckLogin();



        }

        private void InitUser()
        {
            
            
            StartCoroutine(CheckAutoLogin());

            auth.StateChanged += AuthStateChanged;
            AuthStateChanged(this, null);
            
            
        }

        private IEnumerator CheckAutoLogin()
        {
            yield return new WaitForEndOfFrame();
            if(user != null)
            {
                var reloadUserTask = user.ReloadAsync();

                yield return new WaitUntil(predicate: () => reloadUserTask.IsCompleted);

                AutoLoginTornadolly();
            }
            else
            {
                
                Debug.Log("in check aut login");
                
                Navigation.ShowMaps();
                LoginScreen.ChangeVisibility(true);
            }
        }
        
        private void AutoLoginTornadolly()
        {
            if(user != null)
            {
                Navigation.ShowMaps();

                if (user.IsAnonymous)
                {
                    PinCreator.SetActive(false);

                }

                myCityController.ListenChildAdded();
                myCityController.LoadProducts();

               // AppManager.DEVICE_CONTROLLER.StartOnlineChecker();
                AppManager.FIREBASE_CONTROLLER.InitPushNotificationEvents();
                
                
                
                
                
            }
            else
            {
                
                Debug.Log("in autlogin tornadolly");

                
                Navigation.ShowMaps();
                
                LoginScreen.ChangeVisibility(true);
                
                //  AuthUIManager.instance.LoginScreen();
            }
        }


        private void AuthStateChanged(object sender, System.EventArgs eventArgs)
        {
            if (auth.CurrentUser != user)
            {
                bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;

                if (!signedIn && user != null)
                {
                    Debug.Log("Signed Out");
                }

                user = auth.CurrentUser;

                if (signedIn)
                {
                    Debug.Log($"Signed In: {user.DisplayName}");
                }
            }
        }
        
        
        private void CheckLogin()
        {


            string savedEmail = PlayerPrefs.GetString(AppSettings.LoginSaveKey);
            if (string.IsNullOrEmpty(savedEmail))
            {
                VIEW_CONTROLLER.ShowLogin();
            }
            else
            {
                AutoLogin();
            }
        }

        private void AutoLogin()
        {
            string savedEmail = PlayerPrefs.GetString(AppSettings.LoginSaveKey);
            string savedPassword = PlayerPrefs.GetString(AppSettings.PasswordSaveKey);
            LOGIN_CONTROLLER.AutoLogin(savedEmail, savedPassword);
        }

        private void OnLoginSuccess()
        {
            AddCallListener();
        }

        private void OnLogoutSuccess()
        {
            RemoveCallListener();
        }

        Query CallReference;

        private void AddCallListener()
        {
            if (APP_SETTINGS._EnableVideoAudioCalls)
            {
                CallReference = FIREBASE_CONTROLLER.GetCallReference().LimitToLast(1);
                CallReference.ChildAdded += HandleCallAdded;
            }
        }

        private void RemoveCallListener()
        {
            if (APP_SETTINGS._EnableVideoAudioCalls && CallReference != null)
            {
                CallReference.ChildAdded -= HandleCallAdded;
                CallReference = null;
            }
        }

        void HandleCallAdded(object sender, ChildChangedEventArgs args)
        {
            if (args.DatabaseError != null)
            {
                return;
            }
            else
            {
                CallObject _callMsg = JsonUtility.FromJson<CallObject>(args.Snapshot.GetRawJsonValue());

                // check is call valid
                AppManager.FIREBASE_CONTROLLER.GetServerTimestamp(_msg =>
                {
                    string _time = _msg.Data;
                    long timeStamp;
                    bool isInteger = long.TryParse(_time, out timeStamp);

                    if (isInteger)
                    {
                        long callTimeStamp;
                        bool IsSucces = long.TryParse(_callMsg.CreateTimeStamp, out callTimeStamp);
                        if (IsSucces)
                        {
                            long _timePassed = (long)Mathf.Abs(timeStamp - callTimeStamp);
                            int _timePassedSeconds = (int)_timePassed / 100;
                            if (_timePassedSeconds < (long)AppSettings.IncomingCallMaxTime)
                            {
                                if (_callMsg.IsActive)
                                {
                                    if (VIEW_CONTROLLER.IsCallWindowActive())
                                    {
                                        // send bisy
                                        FIREBASE_CONTROLLER.SetCallBisy(_callMsg);
                                    }
                                    else
                                    {
                                        VIEW_CONTROLLER.StartCall(IncommingType.ANSWERS, _callMsg);
                                    }
                                }
                            }
                        }
                    }
                });
            }
        }
    }
}
