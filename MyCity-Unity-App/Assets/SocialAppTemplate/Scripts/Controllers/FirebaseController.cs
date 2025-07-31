using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Unity;
using Firebase.Storage;
using Firebase;
using Firebase.Unity.Editor;
using System;
using System.Threading.Tasks;
using System.Linq;
using Firebase.Functions;
using Firebase.Extensions;
using com.draconianmarshmallows.geofire;
using TriLibCore.Interfaces;
using UnityEngine.UI;
using Yamanas.Scripts.MapLoader.Shop;

namespace SocialApp
{
    public class FirebaseController : MonoBehaviour
    {
        public Text debugText;


        public FirebaseAuth Auth;
        private FirebaseDatabase Database;
        private FirebaseStorage Storage;
        private FirebaseFunctions Functions;
        private string CurrentDeviceToken;

        private bool FirebaseIsInited = false;

        public FirebaseDatabase GETDataBase()
        {
            return Database;
        }


        public void InitFirebase()
        {
            OnFirebaseInit();
        }

        public static void DontDestroyChildOnLoad(GameObject child)
        {
            Transform parentTransform = child.transform;

            // If this object doesn't have a parent then its the root transform.
            while (parentTransform.parent != null)
            {
                // Keep going up the chain.
                parentTransform = parentTransform.parent;
            }

            GameObject.DontDestroyOnLoad(parentTransform.gameObject);
        }

        // init
        private void OnFirebaseInit()
        {
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
                    Auth = FirebaseAuth.DefaultInstance;
                    Database = FirebaseDatabase.DefaultInstance;
                    Storage = FirebaseStorage.DefaultInstance;
                    Functions = FirebaseFunctions.DefaultInstance;
                    FirebaseIsInited = true;
                    CleanTask(task);
                }
                else
                {
                    CleanTask(task);
                }
            });

            DontDestroyChildOnLoad(this.gameObject);
        }

        public void InitPushNotificationEvents()
        {
            Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
            Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
        }

        public void RemovePushNotificationEvents()
        {
            Firebase.Messaging.FirebaseMessaging.TokenReceived -= OnTokenReceived;
            Firebase.Messaging.FirebaseMessaging.MessageReceived -= OnMessageReceived;
        }

        public void Vote(string feedkey, string kind, Action<bool> _callback)
        {
            Task fTask = Database.RootReference.Child(AppSettings.PollAnswers).Child(feedkey).Child(kind)
                .Child(AppSettings.ContainerListKey).Child(AppManager.Instance.auth.CurrentUser.UserId)
                .SetValueAsync(0);

            fTask.ContinueWith(task2 =>
            {
                if (task2.IsCompleted)
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(true));
                    CleanTask(task2);
                }
                else
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(false));
                    CleanTask(task2);
                }
            });
        }

        public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
        {
            CurrentDeviceToken = token.Token;
            if (AppManager.USER_PROFILE.IsLogined())
            {
                RegisterDeviceTokens();
            }
        }


        public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
        {
        }

        public void RegisterDeviceTokens()
        {
            if (string.IsNullOrEmpty(CurrentDeviceToken))
                return;
            DatabaseReference _tokenRef = Database.RootReference.Child(AppSettings.DeviceTokensKey)
                .Child(AppManager.Instance.auth.CurrentUser.UserId).Child(CurrentDeviceToken);
            _tokenRef.SetValueAsync(0);
        }

        public void RemoveDeviceTokens()
        {
            if (string.IsNullOrEmpty(CurrentDeviceToken))
                return;
            DatabaseReference _tokenRef = Database.RootReference.Child(AppSettings.DeviceTokensKey)
                .Child(AppManager.Instance.auth.CurrentUser.UserId).Child(CurrentDeviceToken);
            _tokenRef.RemoveValueAsync();
        }

        public void ClearDeviceToken()
        {
            CurrentDeviceToken = string.Empty;
        }

        public void GetUserDeviceTokes(string _userId, Action<List<string>> _callback)
        {
            DatabaseReference _tokenRef = Database.RootReference.Child(AppSettings.DeviceTokensKey).Child(_userId);


            _tokenRef.GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                }
                else if (task.IsCompleted && task.Result.Exists)
                {
                    List<string> _ids = new List<string>();
                    DataSnapshot snapshot = task.Result;
                    foreach (DataSnapshot _data in snapshot.Children)
                    {
                        _ids.Add(_data.Key);
                    }

                    UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(_ids));
                }
            });
        }

        public void SendPushNotification(NotificationMessage _message)
        {
            string _userID = _message.UserId;
            var data = new Dictionary<string, object>();
            data["_userId"] = _userID;
            data["_title"] = _message.Title;
            data["_body"] = _message.Body;

            // Call the function and extract the operation from the result.
            HttpsCallableReference _function = Functions.GetHttpsCallable("SendFCM");
            _function.CallAsync(data);
        }


        // register with google
        public void RegisterWithGoogle(string googleIdToken, string googleAccessToken,
            Action<RegistrationMessage> _callback)
        {
            Credential credential =
                GoogleAuthProvider.GetCredential(googleIdToken, googleAccessToken);


            RegistrationMessage _regMsg = new RegistrationMessage();
            Auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    _regMsg.ErrorMessage = "CreateUserGoogle was canceled";
                    UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(_regMsg));
                    CleanTask(task);
                    return;
                }

                if (task.IsFaulted)
                {
                    _regMsg.ErrorMessage = "CreateUserGoogle encountered an error: " + task.Exception;
                    UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(_regMsg));
                    CleanTask(task);
                    return;
                }

                Firebase.Auth.FirebaseUser newUser = task.Result;
                _regMsg.UserID = newUser.UserId;
                _regMsg.IsComplete = true;
                UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(_regMsg));
                CleanTask(task);
            });
        }

        public void GetGroupsAt(GroupQuery groupsQuery)
        {
            Query databaseQuery;
            string rootKey = AppSettings.UserGroupsKey;

            if (string.IsNullOrEmpty(groupsQuery.indexKey))
            {
                databaseQuery = Database.RootReference.Child(rootKey).Child(groupsQuery.ownerID)
                    .Child(AppSettings.ContainerListKey).OrderByKey().LimitToLast(groupsQuery.endIndex);
            }
            else if (groupsQuery.forward)
            {
                int count = groupsQuery.endIndex - groupsQuery.startIndex + 1;
                databaseQuery = Database.RootReference.Child(rootKey).Child(groupsQuery.ownerID)
                    .Child(AppSettings.ContainerListKey).OrderByKey().EndAt(groupsQuery.indexKey).LimitToLast(count);
            }
            else
            {
                int count = groupsQuery.endIndex - groupsQuery.startIndex + 1;
                databaseQuery = Database.RootReference.Child(rootKey).Child(groupsQuery.ownerID)
                    .Child(AppSettings.ContainerListKey).OrderByKey().EndAt(groupsQuery.indexKey).LimitToLast(count);
            }


            GroupCallback _callback = new GroupCallback();
            databaseQuery
                .GetValueAsync().ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        _callback.IsSuccess = false;
                        // Handle the error...
                        UnityMainThreadDispatcher.Instance().Enqueue(() => AppManager.VIEW_CONTROLLER.HideLoading());
                        UnityMainThreadDispatcher.Instance().Enqueue(() => groupsQuery.callback.Invoke(_callback));
                        CleanTask(task);
                    }
                    else if (task.IsCompleted && task.Result.Exists)
                    {
                        DataSnapshot snapshot = task.Result;
                        List<Group> groups = new List<Group>();

                        int _groupsCount = (int) snapshot.ChildrenCount;

                        List<Task> TaskList = new List<Task>();

                        for (int i = 0; i < _groupsCount; i++)
                        {
                            DataSnapshot groupsnapshot = snapshot.Children.ElementAt(i);
                            string groupID = groupsnapshot.Key;
                            DatabaseReference _groupRef =
                                Database.RootReference.Child(AppSettings.GroupsKey).Child(groupID);
                            Task<DataSnapshot> _task = _groupRef.GetValueAsync();
                            TaskList.Add(_task);
                        }

                        Task.WhenAll(TaskList).ContinueWith(task2 =>
                        {
                            if (task2.IsCompleted)
                            {
                                foreach (Task<DataSnapshot> t in TaskList)
                                {
                                    if (t.IsCompleted && t.Result.Exists)
                                    {
                                        string _groupId = t.Result.Key;
                                        string jsonGroup = t.Result.GetRawJsonValue();
                                        if (!string.IsNullOrEmpty(jsonGroup))
                                        {
                                            Group _dataGroup = JsonUtility.FromJson<Group>(jsonGroup);

                                            if (_dataGroup != null)
                                            {
                                                _dataGroup.groupID = _groupId;
                                                if (groupsQuery.forward)
                                                {
                                                    if (_groupId != groupsQuery.indexKey)
                                                    {
                                                        groups.Add(_dataGroup);
                                                    }
                                                }
                                                else
                                                {
                                                    groups.Add(_dataGroup);
                                                }
                                            }
                                        }

                                        CleanTask(t);
                                    }
                                }
                            }

                            _callback.IsSuccess = true;
                            groups.Reverse();
                            if (!groupsQuery.forward)
                            {
                                groups.Reverse();
                            }

                            _callback.RequestID = groupsQuery.RequestID;
                            _callback.groups = groups;
                            _callback.forward = groupsQuery.forward;
                            UnityMainThreadDispatcher.Instance().Enqueue(() => groupsQuery.callback.Invoke(_callback));
                            CleanTask(task2);
                        });
                        CleanTask(task);
                    }
                });
        }


        public void SearchModels(ModelsQuery ModelsQuery, string _search)
        {
            Query databaseQuery = Database.RootReference.Child(AppSettings.AllModelsKey)
                .OrderByChild(AppSettings.ModelName).StartAt(_search).EndAt(_search + "\uf8ff");


            ModelCallback _callback = new ModelCallback();
            databaseQuery
                .GetValueAsync().ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        Debug.Log("something wrongs");

                        _callback.IsSuccess = false;
                        // Handle the error...
                        UnityMainThreadDispatcher.Instance().Enqueue(() => AppManager.VIEW_CONTROLLER.HideLoading());
                        UnityMainThreadDispatcher.Instance().Enqueue(() => ModelsQuery.callback.Invoke(_callback));
                    }
                    else if (task.IsCompleted && task.Result.Exists)
                    {
                        Debug.Log("something right");
                        _callback.IsSuccess = true;
                        DataSnapshot snapshot = task.Result;
                        List<Model3D> models = new List<Model3D>();

                        int _modelsCount = (int) snapshot.ChildrenCount;

                        for (int i = 0; i < _modelsCount; i++)
                        {
                            DataSnapshot modelSnapshot = snapshot.Children.ElementAt(i);
                            string _modelId = modelSnapshot.Key;

                            string jsonFeed = modelSnapshot.GetRawJsonValue();
                            if (!string.IsNullOrEmpty(jsonFeed))
                            {
                                Model3D _dataModel = JsonUtility.FromJson<Model3D>(jsonFeed);

                                if (_dataModel != null)
                                {
                                    _dataModel.ModelId = _modelId;
                                    if (ModelsQuery.forward)
                                    {
                                        if (_modelId != ModelsQuery.indexKey)
                                        {
                                            models.Add(_dataModel);
                                        }
                                    }
                                    else
                                    {
                                        models.Add(_dataModel);
                                    }
                                }
                            }
                        }

                        _callback.IsSuccess = true;
                        models.Reverse();
                        if (!ModelsQuery.forward)
                        {
                            models.Reverse();
                        }

                        Debug.Log("Model Id" + models.ElementAt(0).ModelId);
                        _callback.RequestID = ModelsQuery.RequestID;
                        _callback.Models = models;
                        _callback.forward = ModelsQuery.forward;
                        UnityMainThreadDispatcher.Instance().Enqueue(() => ModelsQuery.callback.Invoke(_callback));
                    }

                    CleanTask(task);
                });
        }


        public void Searchgroups(GroupQuery groupsQuery, string _search)
        {
            Query databaseQueryGRoupName = Database.RootReference.Child(AppSettings.GroupsKey)
                .OrderByChild(AppSettings.GroupName).StartAt(_search).EndAt(_search + "\uf8ff");
            Query databaseQueryGRoupDesc = Database.RootReference.Child(AppSettings.GroupsKey)
                .OrderByChild(AppSettings.GroupDesc).StartAt(_search).EndAt(_search + "\uf8ff");

            /*  Query databaseQueryLastName = Database.RootReference.Child(AppSettings.RootUserKey).OrderByChild(AppSettings.UserLastNameKey).StartAt(_search).EndAt(_search + "\uf8ff");
              Query databaseQueryFullName = Database.RootReference.Child(AppSettings.RootUserKey).OrderByChild(AppSettings.UserFullNameKey).StartAt(_search).EndAt(_search + "\uf8ff");
              Query databaseQueryPhone = Database.RootReference.Child(AppSettings.RootUserKey).OrderByChild(AppSettings.UserPhoneKey).StartAt(_search).EndAt(_search + "\uf8ff");*/


            GroupCallback _callback = new GroupCallback();

            List<Task> TaskList = new List<Task>();

            TaskList.Add(databaseQueryGRoupName.GetValueAsync());
            TaskList.Add(databaseQueryGRoupDesc.GetValueAsync());
            /*  TaskList.Add(databaseQueryFullName.GetValueAsync());
              TaskList.Add(databaseQueryPhone.GetValueAsync());*/

            Task.WhenAll(TaskList).ContinueWith(task2 =>
            {
                if (task2.IsCompleted)
                {
                    _callback.IsSuccess = true;
                    List<Group> groups = new List<Group>();
                    List<string> groupsKeys = new List<string>();
                    foreach (Task<DataSnapshot> t in TaskList)
                    {
                        if (t.IsCompleted && t.Result.Exists)
                        {
                            DataSnapshot snapshot = t.Result;

                            for (int i = 0; i < snapshot.ChildrenCount; i++)
                            {
                                DataSnapshot groupSnapshot = snapshot.Children.ElementAt(i);
                                string jsonMessage = groupSnapshot.GetRawJsonValue();
                                if (!string.IsNullOrEmpty(jsonMessage))
                                {
                                    Group _dataGroup = JsonUtility.FromJson<Group>(jsonMessage);
                                    //_dataUser.Key = userSnapshot.Key;
                                    if (_dataGroup != null)
                                    {
                                        if (groupSnapshot.Key != groupsQuery.indexKey)
                                        {
                                            if (!groupsKeys.Contains(groupSnapshot.Key))
                                            {
                                                groups.Add(_dataGroup);
                                                groupsKeys.Add(groupSnapshot.Key);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (!groupsQuery.forward)
                    {
                        groups.Reverse();
                    }

                    _callback.RequestID = groupsQuery.RequestID;
                    _callback.groups = groups;
                    _callback.forward = groupsQuery.forward;
                    UnityMainThreadDispatcher.Instance().Enqueue(() => groupsQuery.callback.Invoke(_callback));
                    CleanTask(task2);
                }
            });
        }

        // add new user
        public void AddNewUser(string _email, string _password, Action<RegistrationMessage> _callback)
        {
            RegistrationMessage _regMsg = new RegistrationMessage();
            Auth.CreateUserWithEmailAndPasswordAsync(_email, _password).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    _regMsg.ErrorMessage = "CreateUserWithEmailAndPasswordAsync was canceled";
                    UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(_regMsg));
                    CleanTask(task);
                    return;
                }

                if (task.IsFaulted)
                {
                    _regMsg.ErrorMessage =
                        "CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception;
                    UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(_regMsg));
                    CleanTask(task);
                    return;
                }

                Firebase.Auth.FirebaseUser newUser = task.Result;
                _regMsg.UserID = newUser.UserId;
                _regMsg.IsComplete = true;
                UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(_regMsg));
                CleanTask(task);
            });
        }

        // get user data
        public void GetUserData(string _userID, Action<User> _callback)
        {
            Query databaseQuery = Database.RootReference.Child(AppSettings.RootUserKey).Child(_userID);
            databaseQuery.GetValueAsync().ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    User _user = JsonUtility.FromJson<User>(task.Result.GetRawJsonValue().ToString());

                    UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(_user));
                    CleanTask(task);
                }
                else
                {
                    CleanTask(task);
                }
            });
        }


        // set user data
        public void SetUserData(User _user, Action<SetUserDataMessage> _callback)
        {
            debugText.text += "in set user data";

            string json = JsonUtility.ToJson(_user);
            SetUserDataMessage _logMsg = new SetUserDataMessage();
            Database.RootReference.Child(AppSettings.RootUserKey).Child(_user.UserID).SetRawJsonValueAsync(json)
                .ContinueWith(task =>
                {
                    debugText.text = "inside ref";


                    if (task.IsCanceled)
                    {
                        _logMsg.ErrorMessage = "Set user data was canceled";
                        UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(_logMsg));
                        CleanTask(task);
                        return;
                    }

                    if (task.IsFaulted)
                    {
                        _logMsg.ErrorMessage = "Set User Data encountered an error: " + task.Exception;
                        UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(_logMsg));
                        CleanTask(task);
                        return;
                    }

                    _logMsg.IsSuccess = true;
                    _logMsg.UserID = _user.UserID;
                    _logMsg.PhotoUrl = _user.imageURL;

                    /*                MyCityMananger.currentID = _logMsg.UserID;*/
                    UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(_logMsg));
                    CleanTask(task);
                });
        }

        // get users who like post
        public void GetUsersWhoLikePost(string _postID, Action<List<User>> _callback)
        {
            Query databaseQuery;
            string rootKey = AppSettings.PostLikesKey;
            databaseQuery = Database.RootReference.Child(rootKey).Child(_postID).Child(AppSettings.ContainerListKey);


            List<User> _users = new List<User>();

            databaseQuery
                .GetValueAsync().ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        UnityMainThreadDispatcher.Instance().Enqueue(() => AppManager.VIEW_CONTROLLER.HideLoading());
                        CleanTask(task);
                    }
                    else if (task.IsCompleted && task.Result.Exists)
                    {
                        DataSnapshot snapshot = task.Result;


                        int _userCount = (int) snapshot.ChildrenCount;

                        List<Task> TaskList = new List<Task>();

                        for (int i = 0; i < _userCount; i++)
                        {
                            DataSnapshot userSnapshot = snapshot.Children.ElementAt(i);
                            string _userID = userSnapshot.Key;
                            DatabaseReference _friendRef =
                                Database.RootReference.Child(AppSettings.RootUserKey).Child(_userID);
                            Task<DataSnapshot> _task = _friendRef.GetValueAsync();
                            TaskList.Add(_task);
                        }

                        Task.WhenAll(TaskList).ContinueWith(task2 =>
                        {
                            if (task2.IsCompleted)
                            {
                                foreach (Task<DataSnapshot> t in TaskList)
                                {
                                    if (t.IsCompleted && t.Result.Exists)
                                    {
                                        string _userId = t.Result.Key;
                                        string jsonUser = t.Result.GetRawJsonValue();
                                        if (!string.IsNullOrEmpty(jsonUser))
                                        {
                                            User _dataUser = JsonUtility.FromJson<User>(jsonUser);
                                            _users.Add(_dataUser);
                                        }

                                        CleanTask(t);
                                    }
                                }
                            }

                            UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(_users));
                            CleanTask(task2);
                        });
                        CleanTask(task);
                    }
                });
        }

        public void GetProducts(string userID, Action<List<string>> _callback)
        {
            Query databaseQuery;
            //  string rootKey = AppSettings.PostLikesKey;
            databaseQuery = Database.RootReference.Child(AppSettings.UsersStore)
                .Child(userID).Child(AppSettings.ContainerListKey);

            databaseQuery.GetValueAsync().ContinueWith(task =>
            {
                DataSnapshot snapshot = task.Result;


                int productCount = (int) snapshot.ChildrenCount;

                Debug.Log($"productcount is {productCount}");


                List<string> products = new List<string>();


                for (int i = 0; i < productCount; i++)
                {
                    //  Debug.Log($"productID is {snapshot.Children.ElementAt(i)}");
                    //  DataSnapshot userSnapshot = snapshot.Children.ElementAt(i);
                    string productID = snapshot.Children.ElementAt(i).Key;
                    products.Add(productID);
                }

                _callback?.Invoke(products);
            });
        }

        public void GetUsersWhoParticipatePost(string _postID, Action<List<User>> _callback)
        {
            Query databaseQuery;
            //  string rootKey = AppSettings.PostLikesKey;
            databaseQuery = Database.RootReference.Child(AppSettings.PostParticipates).Child(_postID)
                .Child(AppSettings.ContainerListKey);


            List<User> _users = new List<User>();

            databaseQuery
                .GetValueAsync().ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        UnityMainThreadDispatcher.Instance().Enqueue(() => AppManager.VIEW_CONTROLLER.HideLoading());
                        CleanTask(task);
                    }
                    else if (task.IsCompleted && task.Result.Exists)
                    {
                        DataSnapshot snapshot = task.Result;


                        int _userCount = (int) snapshot.ChildrenCount;

                        List<Task> TaskList = new List<Task>();


                        for (int i = 0; i < _userCount; i++)
                        {
                            DataSnapshot userSnapshot = snapshot.Children.ElementAt(i);
                            string _userID = userSnapshot.Key;
                            DatabaseReference _friendRef =
                                Database.RootReference.Child(AppSettings.RootUserKey).Child(_userID);
                            Task<DataSnapshot> _task = _friendRef.GetValueAsync();
                            TaskList.Add(_task);
                        }

                        Task.WhenAll(TaskList).ContinueWith(task2 =>
                        {
                            if (task2.IsCompleted)
                            {
                                foreach (Task<DataSnapshot> t in TaskList)
                                {
                                    if (t.IsCompleted && t.Result.Exists)
                                    {
                                        string _userId = t.Result.Key;
                                        string jsonUser = t.Result.GetRawJsonValue();
                                        if (!string.IsNullOrEmpty(jsonUser))
                                        {
                                            User _dataUser = JsonUtility.FromJson<User>(jsonUser);
                                            _users.Add(_dataUser);
                                        }

                                        CleanTask(t);
                                    }
                                }
                            }

                            UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(_users));
                            CleanTask(task2);
                        });
                        CleanTask(task);
                    }
                });
        }

        public void IsVoted(string feedkey, Action<bool> p)
        {
            bool result = false;

            Task<DataSnapshot> fTaskone = Database.RootReference.Child(AppSettings.PollAnswers).Child(feedkey)
                .Child(AppSettings.One).Child(AppSettings.ContainerListKey)
                .Child(AppManager.Instance.auth.CurrentUser.UserId).GetValueAsync();

            Task<DataSnapshot> fTasktwo = Database.RootReference.Child(AppSettings.PollAnswers).Child(feedkey)
                .Child(AppSettings.Two).Child(AppSettings.ContainerListKey)
                .Child(AppManager.Instance.auth.CurrentUser.UserId).GetValueAsync();


            List<Task> TaskList = new List<Task>();

            TaskList.Add(fTaskone);
            TaskList.Add(fTasktwo);

            Task.WhenAll(TaskList).ContinueWith(task2 =>
            {
                if (task2.IsCompleted)
                {
                    foreach (Task<DataSnapshot> t in TaskList)
                    {
                        if (t.IsCompleted && t.Result.Exists)
                        {
                            Debug.Log("is completed?");


                            if (t.Result != null || t.Result.Value != null)
                            {
                                result = true;

                                UnityMainThreadDispatcher.Instance().Enqueue(() => p.Invoke(true));

                                CleanTask(t);
                                CleanTask(task2);
                                //  break;
                            }
                        }

                        CleanTask(t);
                    }


                    if (!result)
                    {
                        UnityMainThreadDispatcher.Instance().Enqueue(() => p.Invoke(false));
                    }
                }

                else
                {
                    Debug.Log("problem");


                    UnityMainThreadDispatcher.Instance().Enqueue(() => p.Invoke(false));
                }


                CleanTask(task2);
            });
        }


        public void LogInGoogle(string googleIdToken, string googleAccessToken, Action<LoginMessage> _callback,
            bool _silentMode = false)
        {
            Credential credential =
                GoogleAuthProvider.GetCredential(googleIdToken, googleAccessToken);

            LoginMessage _logMsg = new LoginMessage();
            Auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    _logMsg.IsSuccess = false;
                    _logMsg.ErrorMessage = "SignInWithEmailAndPasswordAsync was canceled.";
                    UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(_logMsg));
                    CleanTask(task);
                    return;
                }

                if (task.IsFaulted)
                {
                    _logMsg.IsSuccess = false;
                    _logMsg.ErrorMessage = task.Exception.Message;
                    UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(_logMsg));
                    CleanTask(task);
                    return;
                }

                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    FirebaseUser newUser = task.Result;
                    if (AppManager.APP_SETTINGS.UseEmailConfirm && !_silentMode)
                    {
                        if (newUser.IsEmailVerified)
                        {
                            _logMsg.IsSuccess = true;
                            _logMsg.UserID = newUser.UserId;
                            _logMsg.FUser = newUser;
                            UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(_logMsg));
                        }
                        else
                        {
                            AppManager.VIEW_CONTROLLER.ShowPopupMSG(MessageCode.EmailConfirm,
                                AppManager.VIEW_CONTROLLER.ShowLogin);
                            AppManager.VIEW_CONTROLLER.HideLoading();
                            LogOut();
                        }

                        CleanTask(task);
                    }
                    else
                    {
                        _logMsg.IsSuccess = true;
                        _logMsg.UserID = newUser.UserId;
                        _logMsg.FUser = newUser;
                        UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(_logMsg));
                        CleanTask(task);
                    }
                });
            });
        }


        // login
        public void LogIn(string _login, string _password, Action<LoginMessage> _callback, bool _silentMode = false)
        {
            LoginMessage _logMsg = new LoginMessage();
            Auth.SignInWithEmailAndPasswordAsync(_login, _password).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    _logMsg.IsSuccess = false;
                    _logMsg.ErrorMessage = "SignInWithEmailAndPasswordAsync was canceled.";
                    UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(_logMsg));
                    CleanTask(task);
                    return;
                }

                if (task.IsFaulted)
                {
                    _logMsg.IsSuccess = false;
                    _logMsg.ErrorMessage = task.Exception.Message;
                    UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(_logMsg));
                    CleanTask(task);
                    return;
                }

                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    FirebaseUser newUser = task.Result;
                    if (AppManager.APP_SETTINGS.UseEmailConfirm && !_silentMode)
                    {
                        if (newUser.IsEmailVerified)
                        {
                            _logMsg.IsSuccess = true;
                            _logMsg.UserID = newUser.UserId;
                            _logMsg.FUser = newUser;
/*                            MyCityMananger.currentID = _logMsg.UserID;*/
                            UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(_logMsg));
                        }
                        else
                        {
                            AppManager.VIEW_CONTROLLER.ShowPopupMSG(MessageCode.EmailConfirm,
                                AppManager.VIEW_CONTROLLER.ShowLogin);
                            AppManager.VIEW_CONTROLLER.HideLoading();
                            LogOut();
                        }

                        CleanTask(task);
                    }
                    else
                    {
                        _logMsg.IsSuccess = true;
                        _logMsg.UserID = newUser.UserId;
/*                        MyCityMananger.currentID = _logMsg.UserID;
*/
                        _logMsg.FUser = newUser;
                        UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(_logMsg));
                        CleanTask(task);
                    }
                });
            });
        }

        public void LogOut()
        {
            FirebaseAuth.DefaultInstance.SignOut();
        }

        public void SendVerifcationEmail()
        {
            FirebaseAuth.DefaultInstance.CurrentUser.SendEmailVerificationAsync().ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("SendEmailVerificationAsync was canceled.");
                    return;
                }

                if (task.IsFaulted)
                {
                    Debug.LogError("SendEmailVerificationAsync encountered an error: " + task.Exception);
                    return;
                }

                Debug.Log("Email sent successfully.");
            });
        }

        public void UploadAvatar(UploadImageRequest _request, Action<UploadImageCallBack> _callback)
        {
            Debug.Log("firebase controller avatar");

            UploadImageCallBack uploadCallback = new UploadImageCallBack();
            Firebase.Storage.StorageReference avatar_ref = Storage.RootReference.Child(AppSettings.RootUserStorageKey)
                .Child(_request.OwnerId)
                .Child(AppSettings.UserAvatarKey + "/" + "Image_" + _request.Size.ToString() + ".jpg");
            avatar_ref.PutBytesAsync(_request.ImageBytes)
                .ContinueWith((task) =>
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        uploadCallback.IsFinish = true;
                        uploadCallback.IsSuccess = false;
                        UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(uploadCallback));
                        Debug.Log(("if"));
                        CleanTask(task);
                    }
                    else
                    {
                        Debug.Log(("else"));


                        uploadCallback.IsFinish = true;
                        uploadCallback.IsSuccess = true;
                        UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(uploadCallback));
                        CleanTask(task);
                    }
                });
        }


        public void UploadIcon(UploadImageRequest _request, Action<UploadImageCallBack> _callback)
        {
            UploadImageCallBack uploadCallback = new UploadImageCallBack();
            Firebase.Storage.StorageReference icon_ref = Storage.RootReference.Child(AppSettings.GroupsStorage)
                .Child(_request.groupId).Child(AppSettings.GroupIcon);
            icon_ref.PutBytesAsync(_request.ImageBytes)
                .ContinueWith((task) =>
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        uploadCallback.IsFinish = true;
                        uploadCallback.IsSuccess = false;
                        UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(uploadCallback));
                        CleanTask(task);
                    }
                    else
                    {
                        uploadCallback.IsFinish = true;
                        uploadCallback.IsSuccess = true;
                        UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(uploadCallback));
                        CleanTask(task);
                    }
                });
        }


        public void GetThumb(GetProfileImageRequest _request, Action<GetProfileImageCallback> _callback)
        {
            GetProfileImageCallback _profileCallback = new GetProfileImageCallback();
            const long maxAllowedSize = 1 * 2048 * 2048;
            Firebase.Storage.StorageReference icon_ref = Storage.RootReference.Child(AppSettings.GroupsStorage)
                .Child(_request.groupID).Child(AppSettings.GroupIcon);

            icon_ref.GetBytesAsync(maxAllowedSize).ContinueWith((Task<byte[]> task) =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    _profileCallback.IsSuccess = false;
                    UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(_profileCallback));
                    CleanTask(task);
                }
                else
                {
                    byte[] fileContents = task.Result;
                    _profileCallback.IsSuccess = true;
                    _profileCallback.ImageBytes = fileContents;
                    UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(_profileCallback));
                    CleanTask(task);
                }
            });
        }

        public void GetIcon(GetProfileImageRequest _request, Action<GetProfileImageCallback> _callback)
        {
            GetProfileImageCallback _profileCallback = new GetProfileImageCallback();
            const long maxAllowedSize = 1 * 2048 * 2048;
            Firebase.Storage.StorageReference icon_ref = Storage.RootReference.Child(AppSettings.GroupsStorage)
                .Child(_request.groupID).Child(AppSettings.GroupIcon);

            icon_ref.GetBytesAsync(maxAllowedSize).ContinueWith((Task<byte[]> task) =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    _profileCallback.IsSuccess = false;
                    UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(_profileCallback));
                    CleanTask(task);
                }
                else
                {
                    byte[] fileContents = task.Result;
                    _profileCallback.IsSuccess = true;
                    _profileCallback.ImageBytes = fileContents;
                    UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(_profileCallback));
                    CleanTask(task);
                }
            });


            /*icon_ref.GetDownloadUrlAsync().ContinueWith(( task) =>
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        _profileCallback.IsSuccess = false;
                        UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(_profileCallback));
                        CleanTask(task);
                    }
                    else
                    {
                        Uri uri = task.Result;
                        _profileCallback.IsSuccess = true;

                        UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(_profileCallback));
                        CleanTask(task);
                    }
                });
    */
        }


        public void GetProfileImage(GetProfileImageRequest _request, Action<GetProfileImageCallback> _callback)
        {
            GetProfileImageCallback _profileCallback = new GetProfileImageCallback();
            const long maxAllowedSize = 1 * 2048 * 2048;
            Firebase.Storage.StorageReference avatar_ref = Storage.RootReference.Child(AppSettings.RootUserStorageKey)
                .Child(_request.Id)
                .Child(AppSettings.UserAvatarKey + "/" + "Image_" + _request.Size.ToString() + ".jpg");
            avatar_ref.GetBytesAsync(maxAllowedSize).ContinueWith((Task<byte[]> task) =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    _profileCallback.IsSuccess = false;
                    UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(_profileCallback));
                    CleanTask(task);
                }
                else
                {
                    byte[] fileContents = task.Result;
                    _profileCallback.IsSuccess = true;
                    _profileCallback.ImageBytes = fileContents;
                    UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(_profileCallback));
                    CleanTask(task);
                }
            });
        }

        public void GetProfileImageForMarker(GetProfileImageRequest _request, Feed data, GameObject pref,
            Action<GetProfileImageCallback> _callback)
        {
            Debug.Log("wwwwttttfff");


            GetProfileImageCallback _profileCallback = new GetProfileImageCallback();
            const long maxAllowedSize = 1 * 2048 * 2048;
            Firebase.Storage.StorageReference avatar_ref = Storage.RootReference.Child(AppSettings.RootUserStorageKey)
                .Child(_request.Id)
                .Child(AppSettings.UserAvatarKey + "/" + "Image_" + _request.Size.ToString() + ".jpg");


            avatar_ref.GetBytesAsync(maxAllowedSize).ContinueWith((Task<byte[]> task) =>
            {
                try
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        Debug.Log(task.Exception.ToString());

                        ;
                        Debug.Log("taigu ddddddddddddddddddddddddddddddddddd");

                        _profileCallback.IsSuccess = false;
                        CleanTask(task);
                    }
                    else
                    {
                        byte[] fileContents = task.Result;
                        _profileCallback.IsSuccess = true;
                        _profileCallback.ImageBytes = fileContents;
                        _profileCallback.pref = pref;

                        _profileCallback.feed = data;

                        UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(_profileCallback));


                        Debug.Log("gggggggggggggggggggggggg");

                        /*
                        prefab.GetComponentsInChildren<SpriteRenderer>()[2].transform.position =
                            prefab.GetComponentInChildren<GameObject>().transform.position;*/
                        CleanTask(task);
                    }
                }


                catch (Exception e)
                {
                    Debug.Log("fffffffffffffffffffffff");


                    Debug.Log(e.ToString());
                }
            });
        }

        public void GetProfileImageUrl(GetProfileImageRequest _request, Action<GetProfileImageCallback> _callback)
        {
            GetProfileImageCallback _profileCallback = new GetProfileImageCallback();
            Firebase.Storage.StorageReference avatar_ref = Storage.RootReference.Child(AppSettings.RootUserStorageKey)
                .Child(_request.Id)
                .Child(AppSettings.UserAvatarKey + "/" + "Image_" + _request.Size.ToString() + ".jpg");
            avatar_ref.GetDownloadUrlAsync().ContinueWith((Task<Uri> task) =>
            {
                if (!task.IsFaulted && !task.IsCanceled)
                {
                    _profileCallback.IsSuccess = true;
                    _profileCallback.DownloadUrl = task.Result.ToString();

                    Debug.Log("download url: " + _profileCallback.DownloadUrl);

                    UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(_profileCallback));
                    CleanTask(task);
                }
                else
                {
                    _profileCallback.IsSuccess = false;
                    UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(_profileCallback));
                    CleanTask(task);
                }
            });
        }

        public void GetFeedsAt(FeedQuery _feedQuery)
        {
            Query databaseQuery;
            if (string.IsNullOrEmpty(_feedQuery.indexKey))
            {
                databaseQuery = Database.RootReference.Child(AppSettings.UserPostsKey).Child(_feedQuery.ownerID)
                    .OrderByKey().LimitToLast(_feedQuery.endIndex);
            }
            else if (_feedQuery.forward)
            {
                int count = _feedQuery.endIndex - _feedQuery.startIndex + 1;
                databaseQuery = Database.RootReference.Child(AppSettings.UserPostsKey).Child(_feedQuery.ownerID)
                    .OrderByKey().EndAt(_feedQuery.indexKey).LimitToLast(count);
            }
            else
            {
                int count = _feedQuery.endIndex - _feedQuery.startIndex + 1;
                databaseQuery = Database.RootReference.Child(AppSettings.UserPostsKey).Child(_feedQuery.ownerID)
                    .OrderByKey().EndAt(_feedQuery.indexKey).LimitToLast(count);
            }

            FeedCallback _callback = new FeedCallback();
            databaseQuery
                .GetValueAsync().ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        _callback.IsSuccess = false;
                        // Handle the error...
                        UnityMainThreadDispatcher.Instance().Enqueue(() => AppManager.VIEW_CONTROLLER.HideLoading());
                        UnityMainThreadDispatcher.Instance().Enqueue(() => _feedQuery.callback.Invoke(_callback));
                        CleanTask(task);
                    }
                    else if (task.IsCompleted && task.Result.Exists)
                    {
                        DataSnapshot snapshot = task.Result;
                        List<Feed> feeds = new List<Feed>();

                        int _feedCount = (int) snapshot.ChildrenCount;

                        List<Task> TaskList = new List<Task>();

                        for (int i = 0; i < _feedCount; i++)
                        {
                            DataSnapshot feedSnapshot = snapshot.Children.ElementAt(i);
                            string _feedId = feedSnapshot.Key;
                            DatabaseReference _feedRef =
                                Database.RootReference.Child(AppSettings.AllPostsKey).Child(_feedId);
                            Task<DataSnapshot> _task = _feedRef.GetValueAsync();
                            TaskList.Add(_task);
                        }

                        Task.WhenAll(TaskList).ContinueWith(task2 =>
                        {
                            if (task2.IsCompleted)
                            {
                                foreach (Task<DataSnapshot> t in TaskList)
                                {
                                    if (t.IsCompleted && t.Result.Exists)
                                    {
                                        string _feedId = t.Result.Key;
                                        string jsonFeed = t.Result.GetRawJsonValue();
                                        if (!string.IsNullOrEmpty(jsonFeed))
                                        {
                                            Feed _dataFeed = JsonUtility.FromJson<Feed>(jsonFeed);

                                            if (_dataFeed != null)
                                            {
                                                _dataFeed.Key = _feedId;
                                                if (_feedQuery.forward)
                                                {
                                                    if (_feedId != _feedQuery.indexKey)
                                                    {
                                                        feeds.Add(_dataFeed);
                                                    }
                                                }
                                                else
                                                {
                                                    feeds.Add(_dataFeed);
                                                }
                                            }
                                        }

                                        CleanTask(t);
                                    }
                                }

                                CleanTask(task);
                            }

                            _callback.IsSuccess = true;
                            feeds = feeds.OrderBy(p => DateTime.Parse(p.DateCreated)).ToList();
                            feeds.Reverse();
                            if (!_feedQuery.forward)
                            {
                                feeds.Reverse();
                            }

                            _callback.feeds = feeds;
                            _callback.forward = _feedQuery.forward;
                            UnityMainThreadDispatcher.Instance().Enqueue(() => _feedQuery.callback.Invoke(_callback));
                        });
                        CleanTask(task);
                    }
                });
        }

        internal void changeStatusToInProcess(string feedkey)
        {
            Database.RootReference.Child(AppSettings.AllPostsKey).Child(feedkey).Child("status")
                .SetValueAsync("in process").ContinueWith(task =>
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        Debug.Log("couldn't put my location");
                        CleanTask(task);
                    }

                    else
                    {
                        Debug.Log("sucess");

                        CleanTask(task);
                    }
                });
        }


        internal void changeStatusFixed(string feedkey)
        {
            Database.RootReference.Child(AppSettings.AllPostsKey).Child(feedkey).Child("status").SetValueAsync("fixed")
                .ContinueWith(task =>
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        Debug.Log("couldn't put my location");
                        CleanTask(task);
                    }

                    else
                    {
                        Debug.Log("sucess");

                        CleanTask(task);
                    }
                });
        }


        public void GetFriendsFeedsAt(FeedQuery _feedQuery)
        {
            Query databaseQuery;
            if (string.IsNullOrEmpty(_feedQuery.indexKey))
            {
                databaseQuery = Database.RootReference.Child(AppSettings.FriendsPostsKey).Child(_feedQuery.ownerID)
                    .Child(AppSettings.ContainerListKey).OrderByKey().LimitToLast(_feedQuery.endIndex);
            }
            else if (_feedQuery.forward)
            {
                int count = _feedQuery.endIndex - _feedQuery.startIndex + 1;
                databaseQuery = Database.RootReference.Child(AppSettings.FriendsPostsKey).Child(_feedQuery.ownerID)
                    .Child(AppSettings.ContainerListKey).OrderByKey().EndAt(_feedQuery.indexKey).LimitToLast(count);
            }
            else
            {
                int count = _feedQuery.endIndex - _feedQuery.startIndex + 1;
                databaseQuery = Database.RootReference.Child(AppSettings.FriendsPostsKey).Child(_feedQuery.ownerID)
                    .Child(AppSettings.ContainerListKey).OrderByKey().EndAt(_feedQuery.indexKey).LimitToLast(count);
            }

            FeedCallback _callback = new FeedCallback();
            databaseQuery
                .GetValueAsync().ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        _callback.IsSuccess = false;
                        // Handle the error...
                        UnityMainThreadDispatcher.Instance().Enqueue(() => AppManager.VIEW_CONTROLLER.HideLoading());
                        UnityMainThreadDispatcher.Instance().Enqueue(() => _feedQuery.callback.Invoke(_callback));
                        CleanTask(task);
                    }
                    else if (task.IsCompleted && task.Result.Exists)
                    {
                        DataSnapshot snapshot = task.Result;
                        List<Feed> feeds = new List<Feed>();

                        int _feedCount = (int) snapshot.ChildrenCount;

                        List<Task> TaskList = new List<Task>();

                        for (int i = 0; i < _feedCount; i++)
                        {
                            DataSnapshot feedSnapshot = snapshot.Children.ElementAt(i);
                            string _feedId = feedSnapshot.Key;
                            DatabaseReference _feedRef =
                                Database.RootReference.Child(AppSettings.AllPostsKey).Child(_feedId);
                            Task<DataSnapshot> _task = _feedRef.GetValueAsync();
                            TaskList.Add(_task);
                        }

                        Task.WhenAll(TaskList).ContinueWith(task2 =>
                        {
                            if (task2.IsCompleted)
                            {
                                foreach (Task<DataSnapshot> t in TaskList)
                                {
                                    if (t.IsCompleted && t.Result.Exists)
                                    {
                                        string _feedId = t.Result.Key;
                                        string jsonFeed = t.Result.GetRawJsonValue();
                                        if (!string.IsNullOrEmpty(jsonFeed))
                                        {
                                            Feed _dataFeed = JsonUtility.FromJson<Feed>(jsonFeed);

                                            if (_dataFeed != null)
                                            {
                                                _dataFeed.Key = _feedId;
                                                if (_feedQuery.forward)
                                                {
                                                    if (_feedId != _feedQuery.indexKey)
                                                    {
                                                        feeds.Add(_dataFeed);
                                                    }
                                                }
                                                else
                                                {
                                                    feeds.Add(_dataFeed);
                                                }
                                            }
                                        }

                                        CleanTask(t);
                                    }
                                }

                                CleanTask(task);
                            }

                            _callback.IsSuccess = true;
                            feeds = feeds.OrderBy(p => DateTime.Parse(p.DateCreated)).ToList();
                            feeds.Reverse();
                            if (!_feedQuery.forward)
                            {
                                feeds.Reverse();
                            }

                            _callback.feeds = feeds;
                            _callback.forward = _feedQuery.forward;
                            UnityMainThreadDispatcher.Instance().Enqueue(() => _feedQuery.callback.Invoke(_callback));
                        });
                        CleanTask(task);
                    }
                });
        }

        public void GetFriendsAt(UsersQuery _friendQuery)
        {
            Query databaseQuery;
            string rootKey = AppSettings.UserFriendsKey;
            if (_friendQuery.Type == FriendsTabState.Pending)
                rootKey = AppSettings.UserPendingFriendsKey;
            if (_friendQuery.Type == FriendsTabState.Requested)
                rootKey = AppSettings.UserRequestedFriendsKey;
            if (string.IsNullOrEmpty(_friendQuery.indexKey))
            {
                databaseQuery = Database.RootReference.Child(rootKey).Child(_friendQuery.ownerID)
                    .Child(AppSettings.ContainerListKey).OrderByKey().LimitToLast(_friendQuery.endIndex);
            }
            else if (_friendQuery.forward)
            {
                int count = _friendQuery.endIndex - _friendQuery.startIndex + 1;
                databaseQuery = Database.RootReference.Child(rootKey).Child(_friendQuery.ownerID)
                    .Child(AppSettings.ContainerListKey).OrderByKey().EndAt(_friendQuery.indexKey).LimitToLast(count);
            }
            else
            {
                int count = _friendQuery.endIndex - _friendQuery.startIndex + 1;
                databaseQuery = Database.RootReference.Child(rootKey).Child(_friendQuery.ownerID)
                    .Child(AppSettings.ContainerListKey).OrderByKey().EndAt(_friendQuery.indexKey).LimitToLast(count);
            }

            UsersCallback _callback = new UsersCallback();
            databaseQuery
                .GetValueAsync().ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        _callback.IsSuccess = false;
                        // Handle the error...
                        UnityMainThreadDispatcher.Instance().Enqueue(() => AppManager.VIEW_CONTROLLER.HideLoading());
                        UnityMainThreadDispatcher.Instance().Enqueue(() => _friendQuery.callback.Invoke(_callback));
                        CleanTask(task);
                    }
                    else if (task.IsCompleted && task.Result.Exists)
                    {
                        DataSnapshot snapshot = task.Result;
                        List<User> friends = new List<User>();

                        int _friendsCount = (int) snapshot.ChildrenCount;

                        List<Task> TaskList = new List<Task>();

                        for (int i = 0; i < _friendsCount; i++)
                        {
                            DataSnapshot friendSnapshot = snapshot.Children.ElementAt(i);
                            string _friendId = friendSnapshot.Key;
                            DatabaseReference _friendRef =
                                Database.RootReference.Child(AppSettings.RootUserKey).Child(_friendId);
                            Task<DataSnapshot> _task = _friendRef.GetValueAsync();
                            TaskList.Add(_task);
                        }

                        Task.WhenAll(TaskList).ContinueWith(task2 =>
                        {
                            if (task2.IsCompleted)
                            {
                                foreach (Task<DataSnapshot> t in TaskList)
                                {
                                    if (t.IsCompleted && t.Result.Exists)
                                    {
                                        string _friendId = t.Result.Key;
                                        string jsonFriend = t.Result.GetRawJsonValue();
                                        if (!string.IsNullOrEmpty(jsonFriend))
                                        {
                                            User _dataFriend = JsonUtility.FromJson<User>(jsonFriend);

                                            if (_dataFriend != null)
                                            {
                                                _dataFriend.UserID = _friendId;
                                                if (_friendQuery.forward)
                                                {
                                                    if (_friendId != _friendQuery.indexKey)
                                                    {
                                                        friends.Add(_dataFriend);
                                                    }
                                                }
                                                else
                                                {
                                                    friends.Add(_dataFriend);
                                                }
                                            }
                                        }

                                        CleanTask(t);
                                    }
                                }
                            }

                            _callback.IsSuccess = true;
                            friends.Reverse();
                            if (!_friendQuery.forward)
                            {
                                friends.Reverse();
                            }

                            _callback.RequestID = _friendQuery.RequestID;
                            _callback.users = friends;
                            _callback.forward = _friendQuery.forward;
                            UnityMainThreadDispatcher.Instance().Enqueue(() => _friendQuery.callback.Invoke(_callback));
                            CleanTask(task2);
                        });
                        CleanTask(task);
                    }
                });
        }


        public void GetGroupUsersAt(UsersQuery _friendQuery)
        {
            Query databaseQuery;
            string rootKey = AppSettings.UserMessagesGroups;
            if (string.IsNullOrEmpty(_friendQuery.indexKey))
            {
                databaseQuery = Database.RootReference.Child(rootKey).Child(_friendQuery.ownerID)
                    .Child(AppSettings.RootUserKey).OrderByKey().LimitToLast(_friendQuery.endIndex);
            }
            else if (_friendQuery.forward)
            {
                int count = _friendQuery.endIndex - _friendQuery.startIndex + 1;
                databaseQuery = Database.RootReference.Child(rootKey).Child(_friendQuery.ownerID)
                    .Child(AppSettings.RootUserKey).OrderByKey().EndAt(_friendQuery.indexKey).LimitToLast(count);
            }
            else
            {
                int count = _friendQuery.endIndex - _friendQuery.startIndex + 1;
                databaseQuery = Database.RootReference.Child(rootKey).Child(_friendQuery.ownerID)
                    .Child(AppSettings.RootUserKey).OrderByKey().EndAt(_friendQuery.indexKey).LimitToLast(count);
            }

            UsersCallback _callback = new UsersCallback();
            databaseQuery
                .GetValueAsync().ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        _callback.IsSuccess = false;
                        // Handle the error...
                        UnityMainThreadDispatcher.Instance().Enqueue(() => AppManager.VIEW_CONTROLLER.HideLoading());
                        UnityMainThreadDispatcher.Instance().Enqueue(() => _friendQuery.callback.Invoke(_callback));
                        CleanTask(task);
                    }
                    else if (task.IsCompleted && task.Result.Exists)
                    {
                        DataSnapshot snapshot = task.Result;
                        List<User> friends = new List<User>();

                        int _friendsCount = (int) snapshot.ChildrenCount;

                        List<Task> TaskList = new List<Task>();

                        for (int i = 0; i < _friendsCount; i++)
                        {
                            DataSnapshot friendSnapshot = snapshot.Children.ElementAt(i);
                            string _friendId = friendSnapshot.Value.ToString();
                            DatabaseReference _friendRef =
                                Database.RootReference.Child(AppSettings.RootUserKey).Child(_friendId);
                            Task<DataSnapshot> _task = _friendRef.GetValueAsync();
                            TaskList.Add(_task);
                        }

                        Task.WhenAll(TaskList).ContinueWith(task2 =>
                        {
                            if (task2.IsCompleted)
                            {
                                foreach (Task<DataSnapshot> t in TaskList)
                                {
                                    if (t.IsCompleted && t.Result.Exists)
                                    {
                                        string _friendId = t.Result.Key;
                                        string jsonFriend = t.Result.GetRawJsonValue();
                                        if (!string.IsNullOrEmpty(jsonFriend))
                                        {
                                            User _dataFriend = JsonUtility.FromJson<User>(jsonFriend);

                                            if (_dataFriend != null)
                                            {
                                                _dataFriend.UserID = _friendId;
                                                if (_friendQuery.forward)
                                                {
                                                    if (_friendId != _friendQuery.indexKey)
                                                    {
                                                        friends.Add(_dataFriend);
                                                    }
                                                }
                                                else
                                                {
                                                    friends.Add(_dataFriend);
                                                }
                                            }
                                        }

                                        CleanTask(t);
                                    }
                                }
                            }

                            _callback.IsSuccess = true;
                            friends.Reverse();
                            if (!_friendQuery.forward)
                            {
                                friends.Reverse();
                            }

                            _callback.RequestID = _friendQuery.RequestID;
                            _callback.users = friends;
                            _callback.forward = _friendQuery.forward;
                            UnityMainThreadDispatcher.Instance().Enqueue(() => _friendQuery.callback.Invoke(_callback));
                            CleanTask(task2);
                        });
                        CleanTask(task);
                    }
                });
        }

        public void GetWorldFeedsAt(FeedQuery _feedQuery)
        {
            Query databaseQuery;
            if (string.IsNullOrEmpty(_feedQuery.indexKey))
            {
                databaseQuery = Database.RootReference.Child(AppSettings.AllPostsKey).OrderByKey()
                    .LimitToLast(_feedQuery.endIndex);
            }
            else if (_feedQuery.forward)
            {
                int count = _feedQuery.endIndex - _feedQuery.startIndex + 1;
                databaseQuery = Database.RootReference.Child(AppSettings.AllPostsKey).OrderByKey()
                    .EndAt(_feedQuery.indexKey).LimitToLast(count);
            }
            else
            {
                int count = _feedQuery.endIndex - _feedQuery.startIndex + 1;
                databaseQuery = Database.RootReference.Child(AppSettings.AllPostsKey).OrderByKey()
                    .EndAt(_feedQuery.indexKey).LimitToLast(count);
            }

            FeedCallback _callback = new FeedCallback();
            databaseQuery
                .GetValueAsync().ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        Debug.Log("something wrongs");

                        _callback.IsSuccess = false;
                        // Handle the error...
                        UnityMainThreadDispatcher.Instance().Enqueue(() => AppManager.VIEW_CONTROLLER.HideLoading());
                        UnityMainThreadDispatcher.Instance().Enqueue(() => _feedQuery.callback.Invoke(_callback));
                    }
                    else if (task.IsCompleted && task.Result.Exists)
                    {
                        Debug.Log("something right");
                        _callback.IsSuccess = true;
                        DataSnapshot snapshot = task.Result;
                        List<Feed> feeds = new List<Feed>();

                        int _feedCount = (int) snapshot.ChildrenCount;

                        for (int i = 0; i < _feedCount; i++)
                        {
                            DataSnapshot feedSnapshot = snapshot.Children.ElementAt(i);
                            string _feedId = feedSnapshot.Key;

                            string jsonFeed = feedSnapshot.GetRawJsonValue();
                            if (!string.IsNullOrEmpty(jsonFeed))
                            {
                                Feed _dataFeed = JsonUtility.FromJson<Feed>(jsonFeed);

                                if (_dataFeed != null)
                                {
                                    _dataFeed.Key = _feedId;
                                    if (_feedQuery.forward)
                                    {
                                        if (_feedId != _feedQuery.indexKey)
                                        {
                                            feeds.Add(_dataFeed);
                                        }
                                    }
                                    else
                                    {
                                        feeds.Add(_dataFeed);
                                    }
                                }
                            }
                        }

                        _callback.IsSuccess = true;
                        feeds.Reverse();
                        if (!_feedQuery.forward)
                        {
                            feeds.Reverse();
                        }

                        _callback.feeds = feeds;
                        _callback.forward = _feedQuery.forward;
                        UnityMainThreadDispatcher.Instance().Enqueue(() => _feedQuery.callback.Invoke(_callback));
                    }

                    CleanTask(task);
                });
        }

        public void GetMessageListAt(MessageListQuery _listQuery)
        {
            Query databaseQuery;
            if (string.IsNullOrEmpty(_listQuery.indexKey))
            {
                databaseQuery = Database.RootReference.Child(AppSettings.UserMessagesList).Child(_listQuery.ownerID)
                    .OrderByKey().LimitToLast(_listQuery.endIndex);
            }
            else if (_listQuery.forward)
            {
                int count = _listQuery.endIndex - _listQuery.startIndex + 1;
                databaseQuery = Database.RootReference.Child(AppSettings.UserMessagesList).Child(_listQuery.ownerID)
                    .OrderByKey().EndAt(_listQuery.indexKey).LimitToLast(count);
            }
            else
            {
                int count = _listQuery.endIndex - _listQuery.startIndex + 1;
                databaseQuery = Database.RootReference.Child(AppSettings.UserMessagesList).Child(_listQuery.ownerID)
                    .OrderByKey().EndAt(_listQuery.indexKey).LimitToLast(count);
            }

            MessageListCallback _callback = new MessageListCallback();
            databaseQuery
                .GetValueAsync().ContinueWithOnMainThread(task =>
                {
                    if (task.IsCompleted && task.Result.Exists)
                    {
                        _callback.IsSuccess = true;
                        DataSnapshot snapshot = task.Result;
                        List<string> messages = new List<string>();

                        int _feedCount = (int) snapshot.ChildrenCount;

                        for (int i = 0; i < _feedCount; i++)
                        {
                            DataSnapshot userSnapshot = snapshot.Children.ElementAt(i);
                            string _childId = userSnapshot.Key;
                            string _userId = userSnapshot.Value.ToString();

                            string jsonFeed = userSnapshot.GetRawJsonValue();
                            if (!string.IsNullOrEmpty(jsonFeed))
                            {
                                if (_listQuery.forward)
                                {
                                    if (_childId != _listQuery.indexKey)
                                    {
                                        messages.Add(_userId);
                                    }
                                }
                                else
                                {
                                    messages.Add(_userId);
                                }
                            }
                        }

                        _callback.IsSuccess = true;
                        messages.Reverse();
                        if (!_listQuery.forward)
                        {
                            messages.Reverse();
                        }

                        _callback.usersIds = messages;
                        _callback.forward = _listQuery.forward;
                        _listQuery.callback.Invoke(_callback);
                        CleanTask(task);
                    }
                    else
                    {
                        Debug.Log("fail to get form DB ");
                        _callback.IsSuccess = false;
                        // Handle the error...
                        AppManager.VIEW_CONTROLLER.HideLoading();
                        _listQuery.callback.Invoke(_callback);
                        CleanTask(task);
                    }
                });
        }

        public void UploadFile(FileUploadRequset _request, Action<FileUploadCallback> _callback)
        {
            FileUploadCallback _uploadCallback = new FileUploadCallback();

            string pathKey = string.Empty;
            if (_request.FeedType == FeedType.Video)
            {
                pathKey = AppSettings.FeedUploadVideoPath;
            }
            else if (_request.FeedType == FeedType.Image)
            {
                pathKey = AppSettings.FeedUploadImagePath;
            }

            else if (_request.FeedType == FeedType.Poll)
            {
                pathKey = "Feeds/Image/Polls";
            }

            else if (_request.FeedType == FeedType.thumb)
            {
                pathKey = "Models/thumbnails/";
            }


            Firebase.Storage.StorageReference upload_ref = Storage.RootReference.Child(pathKey + _request.FileName);

            upload_ref.PutBytesAsync(_request.UploadBytes)
                .ContinueWith((Task<StorageMetadata> task) =>
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        _uploadCallback.IsComplete = true;
                        _uploadCallback.IsSuccess = false;
                        UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(_uploadCallback));
                        CleanTask(task);
                    }
                    else
                    {
                        Firebase.Storage.StorageMetadata metadata = task.Result;
                        metadata.Reference.GetDownloadUrlAsync().ContinueWith((Task<Uri> task2) =>
                        {
                            if (!task2.IsFaulted && !task2.IsCanceled)
                            {
                                _uploadCallback.IsComplete = true;
                                _uploadCallback.IsSuccess = true;
                                _uploadCallback.DownloadUrl = task2.Result.ToString();
                                UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(_uploadCallback));
                                CleanTask(task2);
                            }
                            else
                            {
                                _uploadCallback.IsComplete = true;
                                _uploadCallback.IsSuccess = false;
                                UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(_uploadCallback));
                                CleanTask(task2);
                            }
                        });
                        CleanTask(task);
                    }
                });
        }


        public void UploadModel(Byte[] fileBytes, Action<ModelUploadCallback> _callback)
        {
            ModelUploadCallback _uploadCallback = new ModelUploadCallback();

            string pathKey = string.Empty;

            pathKey = "Models/";

            string fileName = System.Guid.NewGuid().ToString();

            Firebase.Storage.StorageReference upload_ref = Storage.RootReference.Child(pathKey + fileName);

            upload_ref.PutBytesAsync(fileBytes)
                .ContinueWith((Task<StorageMetadata> task) =>
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        Debug.Log(task.Exception.ToString());
                        ;


                        _uploadCallback.IsComplete = true;
                        _uploadCallback.IsSuccess = false;
                        UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(_uploadCallback));
                        CleanTask(task);
                    }
                    else
                    {
                        Firebase.Storage.StorageMetadata metadata = task.Result;
                        metadata.Reference.GetDownloadUrlAsync().ContinueWith((Task<Uri> task2) =>
                        {
                            if (!task2.IsFaulted && !task2.IsCanceled)
                            {
                                _uploadCallback.IsComplete = true;
                                _uploadCallback.IsSuccess = true;
                                _uploadCallback.DownloadUrl = task2.Result.ToString();
                                UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(_uploadCallback));
                                CleanTask(task2);
                            }
                            else
                            {
                                Debug.Log("task is fault 2");
                                _uploadCallback.IsComplete = true;
                                _uploadCallback.IsSuccess = false;
                                UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(_uploadCallback));
                                CleanTask(task2);
                            }
                        });
                        CleanTask(task);
                    }
                });
        }


        public void UploadModelData(Model3D model, Action<uploadModelDataCallback> _callback)
        {
            DatabaseReference _feedrefo = Database.RootReference.Child(AppSettings.AllModelsKey).Push();


            model.ModelId = _feedrefo.Key;

            string json = JsonUtility.ToJson(model);


            uploadModelDataCallback _uplopadCallback = new uploadModelDataCallback();


            _feedrefo.SetRawJsonValueAsync(json).ContinueWith(task0 =>
            {
                if (task0.IsFaulted || task0.IsCanceled)
                {
                    Debug.Log("is fault allposts");

                    _uplopadCallback.IsSuccess = false;
                    _uplopadCallback.IsComplete = true;
                    UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(_uplopadCallback));
                    CleanTask(task0);
                }
                else
                {
                    Debug.Log("Upload to my feed complete");
                    _uplopadCallback.IsSuccess = true;
                    _uplopadCallback.IsComplete = true;

                    UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(_uplopadCallback));
                    CleanTask(task0);
                }
            });
        }

        public void AddNewPost(Feed _feed, Action<FeedUploadCallback> _callback)
        {
            DatabaseReference _feedrefo = Database.RootReference.Child(AppSettings.AllPostsKey).Push();


            string _feedKey = _feedrefo.Key;


            _feed.Key = _feedKey;


            FeedUploadCallback _uploadCallback = new FeedUploadCallback();

            _uploadCallback.postID = _feed.Key;

            string json = JsonUtility.ToJson(_feed);

            _feedrefo.SetRawJsonValueAsync(json).ContinueWith(task0 =>
            {
                if (task0.IsFaulted || task0.IsCanceled)
                {
                    Debug.Log("is fault allposts");

                    _uploadCallback.IsSuccess = false;
                    _uploadCallback.IsComplete = true;
                    UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(_uploadCallback));
                    CleanTask(task0);
                }


                else
                {
                    DatabaseReference _feedref = Database.RootReference.Child(AppSettings.tags).Child(_feed.tag)
                        .Child(_feed.dateToSave).Child(_feedKey);


                    var georef = Database.RootReference.Child(AppSettings.tags).Child(_feed.tag)
                        .Child(_feed.dateToSave);

                    var geoFire = new GeoFire(georef);


                    _feedref.SetValueAsync(0).ContinueWith(task =>
                    {
                        if (task.IsFaulted || task.IsCanceled)
                        {
                            Debug.Log("is fault allposts");

                            _uploadCallback.IsSuccess = false;
                            _uploadCallback.IsComplete = true;
                            UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(_uploadCallback));
                            CleanTask(task);
                        }
                        else
                        {
                            geoFire.setLocation(_feed.Key, new GeoLocation(_feed.Feedlat, _feed.Feedlng), null);

                            Debug.Log("success allposts");

                            DatabaseReference _feedLinkRef = Database.RootReference.Child(AppSettings.UserPostsKey)
                                .Child(_feed.OwnerID).Child(_feedKey);
                            _feedLinkRef.SetValueAsync(0).ContinueWith(task2 =>
                            {
                                if (task2.IsFaulted || task2.IsCanceled)
                                {
                                    Debug.Log("is fault userposts");
                                    _uploadCallback.IsSuccess = false;
                                    _uploadCallback.IsComplete = true;
                                    UnityMainThreadDispatcher.Instance()
                                        .Enqueue(() => _callback.Invoke(_uploadCallback));
                                    CleanTask(task2);
                                }


                                else
                                {
                                    Debug.Log("success userposts");


                                    Debug.Log("infirebase " + _feed.groupPostID);


                                    DatabaseReference _feedGroupRef = Database.RootReference
                                        .Child(AppSettings.GroupPostskey).Child(_feed.groupPostID)
                                        .Child(_feed.dateToSave).Child(_feedKey);


                                    var georef1 = Database.RootReference.Child(AppSettings.GroupPostskey)
                                        .Child(_feed.groupPostID).Child(_feed.dateToSave);

                                    var geoFire1 = new GeoFire(georef1);

                                    _feedGroupRef.SetValueAsync(0).ContinueWith(task3 =>
                                        {
                                            if (task3.IsFaulted || task3.IsCanceled)
                                            {
                                                Debug.Log("is fault in grouposts");

                                                _uploadCallback.IsSuccess = false;
                                                _uploadCallback.IsComplete = true;
                                                UnityMainThreadDispatcher.Instance()
                                                    .Enqueue(() => _callback.Invoke(_uploadCallback));
                                                CleanTask(task3);
                                            }


                                            else
                                            {
                                                geoFire1.setLocation(_feed.Key,
                                                    new GeoLocation(_feed.Feedlat, _feed.Feedlng), null);


                                                Debug.Log("Upload to my feed complete");
                                                _uploadCallback.IsSuccess = true;
                                                _uploadCallback.IsComplete = true;

                                                UnityMainThreadDispatcher.Instance()
                                                    .Enqueue(() => _callback.Invoke(_uploadCallback));
                                                CleanTask(task3);
                                            }
                                        }
                                    );


                                    CleanTask(task2);
                                }
                            });


                            CleanTask(task);
                        }
                    });

                    CleanTask(task0);
                }
            });
        }

        public void AddNewGroup(Group _group, Action<AddGroupMessage> _callback)
        {
            AddGroupMessage _groupMsg = new AddGroupMessage();
            DatabaseReference _groupref = Database.RootReference.Child(AppSettings.GroupsKey).Push();

            string _groupKey = _groupref.Key;
            _group.groupID = _groupKey;
            _groupMsg.groupID = _groupKey;
            string json = JsonUtility.ToJson(_group);
            _groupref.SetRawJsonValueAsync(json).ContinueWith(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    _groupMsg.IsSuccess = false;
                    _groupMsg.IsComplete = true;
                    UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(_groupMsg));
                    CleanTask(task);
                }
                else
                {
                    DatabaseReference userGroups = Database.RootReference.Child(AppSettings.UserGroupsKey)
                        .Child(_group.ownerID).Child(AppSettings.ContainerListKey).Child(_group.groupID);

                    userGroups.SetValueAsync(0).ContinueWith(task2 =>
                    {
                        if (task2.IsFaulted || task2.IsCanceled)
                        {
                            _groupMsg.IsSuccess = false;
                            _groupMsg.IsComplete = true;
                            UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(_groupMsg));
                            CleanTask(task2);
                        }
                        else
                        {
                            DatabaseReference groupUsers = Database.RootReference.Child(AppSettings.GroupUsersKey)
                                .Child(_group.groupID).Child(AppSettings.ContainerListKey).Child(_group.ownerID);

                            groupUsers.SetValueAsync(0).ContinueWith(task3 =>
                            {
                                if (task2.IsFaulted || task2.IsCanceled)
                                {
                                    _groupMsg.IsSuccess = false;
                                    _groupMsg.IsComplete = true;
                                    UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(_groupMsg));
                                    CleanTask(task3);
                                }
                                else
                                {
                                    Debug.Log("Upload new group complete");
                                    _groupMsg.IsSuccess = true;
                                    _groupMsg.IsComplete = true;

                                    UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(_groupMsg));
                                    CleanTask(task3);
                                }


                                CleanTask(task2);
                            });
                        }
                    });
                }

                CleanTask(task);
            });
        }


      
            
        public void GetleaderBoard(Action<List<User>> _callback)
        {
            //  string rootKey = AppSettings.PostLikesKey;
            Query databaseQuery = Database.RootReference.Child(AppSettings.RootUserKey)
                .OrderByChild(AppSettings.Score).LimitToLast(5);

            List<User> _users = new List<User>();

            databaseQuery
                .GetValueAsync().ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        UnityMainThreadDispatcher.Instance().Enqueue(() => AppManager.VIEW_CONTROLLER.HideLoading());
                        CleanTask(task);
                    }
                    else if (task.IsCompleted && task.Result.Exists)
                    {
                        DataSnapshot snapshot = task.Result;


                        int _userCount = (int) snapshot.ChildrenCount;

                        List<Task> TaskList = new List<Task>();


                        for (int i = 0; i < _userCount; i++)
                        {
                            DataSnapshot userSnapshot = snapshot.Children.ElementAt(i);
                            string _userID = userSnapshot.Key;
                            DatabaseReference _friendRef =
                                Database.RootReference.Child(AppSettings.RootUserKey).Child(_userID);
                            Task<DataSnapshot> _task = _friendRef.GetValueAsync();
                            TaskList.Add(_task);
                        }

                        Task.WhenAll(TaskList).ContinueWith(task2 =>
                        {
                            if (task2.IsCompleted)
                            {
                                foreach (Task<DataSnapshot> t in TaskList)
                                {
                                    if (t.IsCompleted && t.Result.Exists)
                                    {
                                        string _userId = t.Result.Key;
                                        string jsonUser = t.Result.GetRawJsonValue();
                                        if (!string.IsNullOrEmpty(jsonUser))
                                        {
                                            User _dataUser = JsonUtility.FromJson<User>(jsonUser);
                                            _users.Add(_dataUser);
                                        }

                                        CleanTask(t);
                                    }
                                }
                            }

                            _users.Reverse();

                            UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(_users));
                            CleanTask(task2);
                        });
                        CleanTask(task);
                    }
                });
        
            
        }
        
        
        public void SearchUsers(UsersQuery _userQuery, string _search)
        {
            Query databaseQueryFirstName = Database.RootReference.Child(AppSettings.RootUserKey)
                .OrderByChild(AppSettings.UserFirstNameKey).StartAt(_search).EndAt(_search + "\uf8ff");
            Query databaseQueryLastName = Database.RootReference.Child(AppSettings.RootUserKey)
                .OrderByChild(AppSettings.UserLastNameKey).StartAt(_search).EndAt(_search + "\uf8ff");
            Query databaseQueryFullName = Database.RootReference.Child(AppSettings.RootUserKey)
                .OrderByChild(AppSettings.UserFullNameKey).StartAt(_search).EndAt(_search + "\uf8ff");
            Query databaseQueryPhone = Database.RootReference.Child(AppSettings.RootUserKey)
                .OrderByChild(AppSettings.UserPhoneKey).StartAt(_search).EndAt(_search + "\uf8ff");


            UsersCallback _callback = new UsersCallback();

            List<Task> TaskList = new List<Task>();

            TaskList.Add(databaseQueryFirstName.GetValueAsync());
            TaskList.Add(databaseQueryLastName.GetValueAsync());
            TaskList.Add(databaseQueryFullName.GetValueAsync());
            TaskList.Add(databaseQueryPhone.GetValueAsync());

            Task.WhenAll(TaskList).ContinueWith(task2 =>
            {
                if (task2.IsCompleted)
                {
                    _callback.IsSuccess = true;
                    List<User> users = new List<User>();
                    List<string> usersKeys = new List<string>();
                    foreach (Task<DataSnapshot> t in TaskList)
                    {
                        if (t.IsCompleted && t.Result.Exists)
                        {
                            DataSnapshot snapshot = t.Result;

                            for (int i = 0; i < snapshot.ChildrenCount; i++)
                            {
                                DataSnapshot userSnapshot = snapshot.Children.ElementAt(i);
                                string jsonMessage = userSnapshot.GetRawJsonValue();
                                if (!string.IsNullOrEmpty(jsonMessage))
                                {
                                    User _dataUser = JsonUtility.FromJson<User>(jsonMessage);
                                    //_dataUser.Key = userSnapshot.Key;
                                    if (_dataUser != null)
                                    {
                                        if (userSnapshot.Key != _userQuery.indexKey &&
                                            !AppManager.USER_PROFILE.IsMine(_dataUser.UserID))
                                        {
                                            if (!usersKeys.Contains(userSnapshot.Key))
                                            {
                                                users.Add(_dataUser);
                                                usersKeys.Add(userSnapshot.Key);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (!_userQuery.forward)
                    {
                        users.Reverse();
                    }

                    _callback.RequestID = _userQuery.RequestID;
                    _callback.users = users;
                    _callback.forward = _userQuery.forward;
                    UnityMainThreadDispatcher.Instance().Enqueue(() => _userQuery.callback.Invoke(_callback));
                    CleanTask(task2);
                }
            });
        }

        public void GetUserFullName(string _userID, Action<string> _callback)
        {
            Debug.Log(_userID);
            if (_userID != null)
            {
                Query databaseQuery = Database.RootReference.Child(AppSettings.RootUserKey).Child(_userID)
                    .Child(AppSettings.UserFullNameKey);
                databaseQuery.GetValueAsync().ContinueWith(task =>
                {
                    if (task.IsCompleted)
                    {
                        if (task.Result.Exists)
                        {
                            UnityMainThreadDispatcher.Instance()
                                .Enqueue(() => _callback.Invoke(task.Result.Value.ToString()));
                        }

                        CleanTask(task);
                    }
                    else
                    {
                        CleanTask(task);
                    }
                });
            }
        }

        public void GetUserFriendsCount(string _userID, Action<string> _callback)
        {
            Query databaseQuery = Database.RootReference.Child(AppSettings.UserFriendsKey).Child(_userID)
                .Child(AppSettings.ListCountKey);
            databaseQuery.GetValueAsync().ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    if (task.Result != null && task.Result.Value != null)
                    {
                        UnityMainThreadDispatcher.Instance()
                            .Enqueue(() => _callback.Invoke(task.Result.Value.ToString()));
                    }

                    CleanTask(task);
                }
                else
                {
                    CleanTask(task);
                }
            });
        }

        public void GetPostLikesCount(string _postID, Action<string> _callback)
        {
            Query databaseQuery = Database.RootReference.Child(AppSettings.PostLikesKey).Child(_postID)
                .Child(AppSettings.ListCountKey);
            databaseQuery.GetValueAsync().ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    if (task.Result != null && task.Result.Value != null)
                    {
                        UnityMainThreadDispatcher.Instance()
                            .Enqueue(() => _callback.Invoke(task.Result.Value.ToString()));
                    }

                    CleanTask(task);
                }
                else
                {
                    CleanTask(task);
                }
            });
        }


        public void LikeCommet(string commentId, Action<bool> _callback)
        {
            Task fTask = Database.RootReference.Child(AppSettings.CommentsLikesKey).Child(commentId)
                .Child(AppSettings.ContainerListKey).Child(AppManager.Instance.auth.CurrentUser.UserId)
                .SetValueAsync(0);

            fTask.ContinueWith(task2 =>
            {
                if (task2.IsCompleted)
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(true));
                    CleanTask(task2);
                }
                else
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(false));
                    CleanTask(task2);
                }
            });
        }

        public void UnLikComment(string comentID, Action<bool> _callback)
        {
            Task fTask = Database.RootReference.Child(AppSettings.CommentsLikesKey).Child(comentID)
                .Child(AppSettings.ContainerListKey).Child(AppManager.Instance.auth.CurrentUser.UserId)
                .RemoveValueAsync();

            fTask.ContinueWith(task2 =>
            {
                if (task2.IsCompleted)
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(true));
                    CleanTask(task2);
                }
                else
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(false));
                    CleanTask(task2);
                }
            });
        }


        public void IsLikedComment(string commentID, Action<bool> _callback)
        {
            Task<DataSnapshot> fTask = Database.RootReference.Child(AppSettings.CommentsLikesKey).Child(commentID)
                .Child(AppSettings.ContainerListKey).Child(AppManager.Instance.auth.CurrentUser.UserId).GetValueAsync();


            fTask.ContinueWith(task2 =>
            {
                Debug.Log("continue task");

                if (task2.IsCompleted)
                {
                    if (fTask.Result == null || fTask.Result.Value == null)
                    {
                        UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(false));
                    }
                    else
                    {
                        UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(true));
                    }

                    CleanTask(task2);
                }
                else
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(false));
                    CleanTask(task2);
                }
            });
        }

        public void IsProdcutBought(string productID, Action<bool> _callback)
        {
            Task<DataSnapshot> fTask = Database.RootReference.Child(AppSettings.UsersStore)
                .Child(AppManager.Instance.auth.CurrentUser.UserId).Child(productID).GetValueAsync();


            fTask.ContinueWith(task2 =>
            {
                Debug.Log("continue task");

                if (task2.IsCompleted)
                {
                    if (fTask.Result == null || fTask.Result.Value == null)
                    {
                        UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(false));
                    }
                    else
                    {
                        UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(true));
                    }

                    CleanTask(task2);
                }
                else
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(false));
                    CleanTask(task2);
                }
            });
        }

        public void Participate(string _postID, Action<bool> _callback)
        {
            Task fTask = Database.RootReference.Child(AppSettings.PostParticipates).Child(_postID)
                .Child(AppSettings.ContainerListKey).Child(AppManager.Instance.auth.CurrentUser.UserId)
                .SetValueAsync(0);

            fTask.ContinueWith(task2 =>
            {
                if (task2.IsCompleted)
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(true));
                    CleanTask(task2);
                }
                else
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(false));
                    CleanTask(task2);
                }
            });
        }

        public void UnParticipate(string _postID, Action<bool> _callback)
        {
            Task fTask = Database.RootReference.Child(AppSettings.PostParticipates).Child(_postID)
                .Child(AppSettings.ContainerListKey).Child(AppManager.Instance.auth.CurrentUser.UserId)
                .RemoveValueAsync();

            fTask.ContinueWith(task2 =>
            {
                if (task2.IsCompleted)
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(true));
                    CleanTask(task2);
                }
                else
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(false));
                    CleanTask(task2);
                }
            });
        }

        public void IsPartiPost(string _postID, Action<bool> _callback)
        {
            Task<DataSnapshot> fTask = Database.RootReference.Child(AppSettings.PostParticipates).Child(_postID)
                .Child(AppSettings.ContainerListKey).Child(AppManager.Instance.auth.CurrentUser.UserId).GetValueAsync();


            fTask.ContinueWith(task2 =>
            {
                Debug.Log("continue task");

                if (task2.IsCompleted)
                {
                    if (fTask.Result == null || fTask.Result.Value == null)
                    {
                        UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(false));
                    }
                    else
                    {
                        UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(true));
                    }

                    CleanTask(task2);
                }
                else
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(false));
                    CleanTask(task2);
                }
            });
        }

        public void LikPost(string _postID, Action<bool> _callback)
        {
            Task fTask = Database.RootReference.Child(AppSettings.PostLikesKey).Child(_postID)
                .Child(AppSettings.ContainerListKey).Child(AppManager.Instance.auth.CurrentUser.UserId)
                .SetValueAsync(0);

            fTask.ContinueWith(task2 =>
            {
                if (task2.IsCompleted)
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(true));
                    CleanTask(task2);
                }
                else
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(false));
                    CleanTask(task2);
                }
            });
        }

        public void PurchaseProduct(string productID, Action _callback)
        {
            Task fTask = Database.RootReference.Child(AppSettings.UsersStore)
                .Child(AppManager.Instance.auth.CurrentUser.UserId).Child(AppSettings.ContainerListKey)
                .Child(productID).SetValueAsync(0);

            fTask.ContinueWith(task2 =>
            {
                if (task2.IsCompleted)
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke());
                    CleanTask(task2);
                }
                else
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke());
                    CleanTask(task2);
                }
            });
        }

        public void UnLikPost(string _postID, Action<bool> _callback)
        {
            Task fTask = Database.RootReference.Child(AppSettings.PostLikesKey).Child(_postID)
                .Child(AppSettings.ContainerListKey).Child(AppManager.Instance.auth.CurrentUser.UserId)
                .RemoveValueAsync();

            fTask.ContinueWith(task2 =>
            {
                if (task2.IsCompleted)
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(true));
                    CleanTask(task2);
                }
                else
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(false));
                    CleanTask(task2);
                }
            });
        }

        public void IsLikedPost(string _postID, Action<bool> _callback)
        {
            Task<DataSnapshot> fTask = Database.RootReference.Child(AppSettings.PostLikesKey).Child(_postID)
                .Child(AppSettings.ContainerListKey).Child(AppManager.Instance.auth.CurrentUser.UserId).GetValueAsync();


            fTask.ContinueWith(task2 =>
            {
                Debug.Log("continue task");

                if (task2.IsCompleted)
                {
                    if (fTask.Result == null || fTask.Result.Value == null)
                    {
                        UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(false));
                    }
                    else
                    {
                        UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(true));
                    }

                    CleanTask(task2);
                }
                else
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(false));
                    CleanTask(task2);
                }
            });
        }

        public void GetPostCommentsCount(string _postID, Action<string> _callback)
        {
            Query databaseQuery = Database.RootReference.Child(AppSettings.PostCommentsKey).Child(_postID)
                .Child(AppSettings.ListCountKey);
            databaseQuery.GetValueAsync().ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    if (task.Result != null && task.Result.Value != null)
                    {
                        UnityMainThreadDispatcher.Instance()
                            .Enqueue(() => _callback.Invoke(task.Result.Value.ToString()));
                    }

                    CleanTask(task);
                }
                else
                {
                    CleanTask(task);
                }
            });
        }

        public void GetLastMessageWithUser(string _userId, Action<string> _callback)
        {
            GetMessageReferece(_userId).LimitToLast(1).GetValueAsync().ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    Message _dataMsg =
                        JsonUtility.FromJson<Message>(task.Result.Children.ElementAt<DataSnapshot>(0)
                            .GetRawJsonValue());
                    string _body = string.Empty;
                    if (_dataMsg.Type == ContentMessageType.TEXT) _body = _dataMsg.BodyTXT;
                    if (_dataMsg.Type == ContentMessageType.IMAGE) _body = _dataMsg.Type.ToString();
                    UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(_body));
                    CleanTask(task);
                }
                else
                {
                    CleanTask(task);
                }
            });
        }

        public void GetLastMessageAtGroup(string _chatId, Action<string> _callback)
        {
            GetGroupChatReferece(_chatId).LimitToLast(1).GetValueAsync().ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    Message _dataMsg =
                        JsonUtility.FromJson<Message>(task.Result.Children.ElementAt<DataSnapshot>(0)
                            .GetRawJsonValue());
                    string _body = string.Empty;
                    if (_dataMsg.Type == ContentMessageType.TEXT) _body = _dataMsg.BodyTXT;
                    if (_dataMsg.Type == ContentMessageType.IMAGE) _body = _dataMsg.Type.ToString();
                    UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(_body));
                    CleanTask(task);
                }
                else
                {
                    CleanTask(task);
                }
            });
        }


        public void GetGroupMessagesAt(MessagesQuery _messageQuery)
        {
            string messageKey = _messageQuery.UserId;
            Query databaseQuery;
            if (string.IsNullOrEmpty(_messageQuery.indexKey))
            {
                databaseQuery = Database.RootReference.Child(AppSettings.UserMessagesKey).Child(messageKey)
                    .Child(AppSettings.ContainerListKey).OrderByKey().LimitToLast(_messageQuery.endIndex);
            }
            else if (_messageQuery.forward)
            {
                int count = _messageQuery.endIndex - _messageQuery.startIndex + 1;
                databaseQuery = Database.RootReference.Child(AppSettings.UserMessagesKey).Child(messageKey)
                    .Child(AppSettings.ContainerListKey).OrderByKey().StartAt(_messageQuery.indexKey)
                    .LimitToFirst(count);
            }
            else
            {
                int count = _messageQuery.endIndex - _messageQuery.startIndex + 1;
                databaseQuery = Database.RootReference.Child(AppSettings.UserMessagesKey).Child(messageKey)
                    .Child(AppSettings.ContainerListKey).OrderByKey().EndAt(_messageQuery.indexKey).LimitToLast(count);
            }

            MessagesCallback _callback = new MessagesCallback();
            databaseQuery
                .GetValueAsync().ContinueWith(task =>
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        _callback.IsSuccess = false;
                        UnityMainThreadDispatcher.Instance().Enqueue(() => AppManager.VIEW_CONTROLLER.HideLoading());
                        UnityMainThreadDispatcher.Instance().Enqueue(() => _messageQuery.callback.Invoke(_callback));
                        CleanTask(task);
                    }
                    else if (task.IsCompleted && task.Result != null && task.Result.ChildrenCount > 0)
                    {
                        _callback.IsSuccess = true;
                        DataSnapshot snapshot = task.Result;
                        List<Message> feeds = new List<Message>();
                        for (int i = 0; i < snapshot.ChildrenCount; i++)
                        {
                            DataSnapshot feedSnapshot = snapshot.Children.ElementAt(i);

                            string jsonMessage = feedSnapshot.GetRawJsonValue();
                            Message _dataFeed = JsonUtility.FromJson<Message>(jsonMessage);
                            _dataFeed.Key = feedSnapshot.Key;
                            if (_dataFeed != null)
                            {
                                if (feedSnapshot.Key != _messageQuery.indexKey)
                                {
                                    feeds.Add(_dataFeed);
                                }
                            }
                        }

                        if (!_messageQuery.forward)
                        {
                            feeds.Reverse();
                        }

                        _callback.messages = feeds;
                        _callback.forward = _messageQuery.forward;
                        UnityMainThreadDispatcher.Instance().Enqueue(() => _messageQuery.callback.Invoke(_callback));
                        CleanTask(task);
                    }
                    else
                    {
                        _callback.IsSuccess = false;
                        UnityMainThreadDispatcher.Instance().Enqueue(() => AppManager.VIEW_CONTROLLER.HideLoading());
                        UnityMainThreadDispatcher.Instance().Enqueue(() => _messageQuery.callback.Invoke(_callback));
                        CleanTask(task);
                    }
                });
        }

        public void GetMessagesAt(MessagesQuery _messageQuery)
        {
            string messageKey = GetUserMessageKey(AppManager.Instance.auth.CurrentUser.UserId, _messageQuery.UserId);
            Query databaseQuery;
            if (string.IsNullOrEmpty(_messageQuery.indexKey))
            {
                databaseQuery = Database.RootReference.Child(AppSettings.UserMessagesKey).Child(messageKey)
                    .Child(AppSettings.ContainerListKey).OrderByKey().LimitToLast(_messageQuery.endIndex);
            }
            else if (_messageQuery.forward)
            {
                int count = _messageQuery.endIndex - _messageQuery.startIndex + 1;
                databaseQuery = Database.RootReference.Child(AppSettings.UserMessagesKey).Child(messageKey)
                    .Child(AppSettings.ContainerListKey).OrderByKey().StartAt(_messageQuery.indexKey)
                    .LimitToFirst(count);
            }
            else
            {
                int count = _messageQuery.endIndex - _messageQuery.startIndex + 1;
                databaseQuery = Database.RootReference.Child(AppSettings.UserMessagesKey).Child(messageKey)
                    .Child(AppSettings.ContainerListKey).OrderByKey().EndAt(_messageQuery.indexKey).LimitToLast(count);
            }

            MessagesCallback _callback = new MessagesCallback();
            databaseQuery
                .GetValueAsync().ContinueWith(task =>
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        _callback.IsSuccess = false;
                        UnityMainThreadDispatcher.Instance().Enqueue(() => AppManager.VIEW_CONTROLLER.HideLoading());
                        UnityMainThreadDispatcher.Instance().Enqueue(() => _messageQuery.callback.Invoke(_callback));
                        CleanTask(task);
                    }
                    else if (task.IsCompleted && task.Result != null && task.Result.ChildrenCount > 0)
                    {
                        _callback.IsSuccess = true;
                        DataSnapshot snapshot = task.Result;
                        List<Message> feeds = new List<Message>();
                        for (int i = 0; i < snapshot.ChildrenCount; i++)
                        {
                            DataSnapshot feedSnapshot = snapshot.Children.ElementAt(i);

                            string jsonMessage = feedSnapshot.GetRawJsonValue();
                            Message _dataFeed = JsonUtility.FromJson<Message>(jsonMessage);
                            _dataFeed.Key = feedSnapshot.Key;
                            if (_dataFeed != null)
                            {
                                if (feedSnapshot.Key != _messageQuery.indexKey)
                                {
                                    feeds.Add(_dataFeed);
                                }
                            }
                        }

                        if (!_messageQuery.forward)
                        {
                            feeds.Reverse();
                        }

                        _callback.messages = feeds;
                        _callback.forward = _messageQuery.forward;
                        UnityMainThreadDispatcher.Instance().Enqueue(() => _messageQuery.callback.Invoke(_callback));
                        CleanTask(task);
                    }
                    else
                    {
                        _callback.IsSuccess = false;
                        UnityMainThreadDispatcher.Instance().Enqueue(() => AppManager.VIEW_CONTROLLER.HideLoading());
                        UnityMainThreadDispatcher.Instance().Enqueue(() => _messageQuery.callback.Invoke(_callback));
                        CleanTask(task);
                    }
                });
        }

        public void GetCommentsAt(MessagesQuery _messageQuery)
        {
            string messageKey = _messageQuery.UserId;
            Query databaseQuery;
            if (string.IsNullOrEmpty(_messageQuery.indexKey))
            {
                databaseQuery = Database.RootReference.Child(AppSettings.PostCommentsKey).Child(messageKey)
                    .Child(AppSettings.ContainerListKey).OrderByKey().LimitToLast(_messageQuery.endIndex);
            }
            else if (_messageQuery.forward)
            {
                int count = _messageQuery.endIndex - _messageQuery.startIndex + 1;
                databaseQuery = Database.RootReference.Child(AppSettings.PostCommentsKey).Child(messageKey)
                    .Child(AppSettings.ContainerListKey).OrderByKey().StartAt(_messageQuery.indexKey)
                    .LimitToFirst(count);
            }
            else
            {
                int count = _messageQuery.endIndex - _messageQuery.startIndex + 1;
                databaseQuery = Database.RootReference.Child(AppSettings.PostCommentsKey).Child(messageKey)
                    .Child(AppSettings.ContainerListKey).OrderByKey().EndAt(_messageQuery.indexKey).LimitToLast(count);
            }

            MessagesCallback _callback = new MessagesCallback();
            databaseQuery
                .GetValueAsync().ContinueWith(task =>
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        _callback.IsSuccess = false;
                        UnityMainThreadDispatcher.Instance().Enqueue(() => AppManager.VIEW_CONTROLLER.HideLoading());
                        UnityMainThreadDispatcher.Instance().Enqueue(() => _messageQuery.callback.Invoke(_callback));
                        CleanTask(task);
                    }
                    else if (task.IsCompleted && task.Result != null && task.Result.ChildrenCount > 0)
                    {
                        _callback.IsSuccess = true;
                        DataSnapshot snapshot = task.Result;
                        List<Message> feeds = new List<Message>();
                        for (int i = 0; i < snapshot.ChildrenCount; i++)
                        {
                            DataSnapshot feedSnapshot = snapshot.Children.ElementAt(i);

                            string jsonMessage = feedSnapshot.GetRawJsonValue();
                            Message _dataFeed = JsonUtility.FromJson<Message>(jsonMessage);
                            _dataFeed.Key = feedSnapshot.Key;
                            if (_dataFeed != null)
                            {
                                if (feedSnapshot.Key != _messageQuery.indexKey)
                                {
                                    feeds.Add(_dataFeed);
                                }
                            }
                        }

                        if (!_messageQuery.forward)
                        {
                            feeds.Reverse();
                        }

                        _callback.messages = feeds;
                        _callback.forward = _messageQuery.forward;
                        UnityMainThreadDispatcher.Instance().Enqueue(() => _messageQuery.callback.Invoke(_callback));
                        CleanTask(task);
                    }
                    else
                    {
                        _callback.IsSuccess = false;
                        UnityMainThreadDispatcher.Instance().Enqueue(() => AppManager.VIEW_CONTROLLER.HideLoading());
                        UnityMainThreadDispatcher.Instance().Enqueue(() => _messageQuery.callback.Invoke(_callback));
                        CleanTask(task);
                    }
                });
        }

        public void GetFeedVideoFileUrl(string _fileName, Action<string> _callback)
        {
            StorageReference video_ref =
                Storage.RootReference.Child(AppSettings.FeedUploadVideoPath + _fileName +
                                            AppSettings.ConvertedVideoOutPath);
            video_ref.GetDownloadUrlAsync().ContinueWith((Task<Uri> task2) =>
            {
                if (!task2.IsFaulted && !task2.IsCanceled)
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(task2.Result.ToString()));
                    CleanTask(task2);
                }
                else
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(string.Empty));
                    CleanTask(task2);
                }
            });
        }

        public void UploadAndCompressVideo(string _path, string _databasePath)
        {
            // Create the arguments to the callable function.
            var data = new Dictionary<string, object>();
            data["uploadPath"] = _path;
            data["bucketUrl"] = Storage.App.Options.StorageBucket;
            data["databasePath"] = _databasePath;

            // Call the function and extract the operation from the result.
            HttpsCallableReference _function = Functions.GetHttpsCallable("UploadAndCompressVideo");
            _function.CallAsync(data).ContinueWith((task) =>
            {
                if (!task.IsFaulted && !task.IsCanceled)
                {
                    CleanTask(task);
                }
                else
                {
                    CleanTask(task);
                }
            });
        }

        public void SharePostWithFriends(string _userId, string _postId)
        {
            Debug.Log("SharePostWithFriends " + _userId + " " + _postId);
            // Create the arguments to the callable function.
            var data = new Dictionary<string, object>();
            data["_userId"] = _userId;
            data["_postId"] = _postId;

            // Call the function and extract the operation from the result.
            HttpsCallableReference _function = Functions.GetHttpsCallable("SharePostWithFriends");
            _function.CallAsync(data).ContinueWith((task) =>
            {
                if (!task.IsFaulted && !task.IsCanceled && task.IsCompleted)
                {
                    Debug.Log("Success SharePostWithFriends");
                    CleanTask(task);
                }
                else
                {
                    Debug.Log("Failed SharePostWithFriends " + task.Exception.StackTrace);
                    CleanTask(task);
                }
            });
        }

        public DatabaseReference GetMessageReferece(string _userId)
        {
            string messageKey = GetUserMessageKey(AppManager.Instance.auth.CurrentUser.UserId, _userId);
            return Database.RootReference.Child(AppSettings.UserMessagesKey).Child(messageKey)
                .Child(AppSettings.ContainerListKey);
        }

        public DatabaseReference GetGroupChatReferece(string _chatId)
        {
            string messageKey = _chatId;
            return Database.RootReference.Child(AppSettings.UserMessagesKey).Child(messageKey)
                .Child(AppSettings.ContainerListKey);
        }

        public DatabaseReference GetTypingMessageReferece(string _userId)
        {
            string messageKey = GetUserMessageKey(AppManager.Instance.auth.CurrentUser.UserId, _userId);
            return Database.RootReference.Child(AppSettings.UserMessagesKey).Child(messageKey)
                .Child(AppSettings.TypingMSGKey).Child(_userId);
        }

        public void UpdateTypingMessage(string _userId, string _s)
        {
            string messageKey = GetUserMessageKey(AppManager.Instance.auth.CurrentUser.UserId, _userId);
            Database.RootReference.Child(AppSettings.UserMessagesKey).Child(messageKey).Child(AppSettings.TypingMSGKey)
                .Child((AppManager.Instance.auth.CurrentUser.UserId)).SetValueAsync(_s);
        }

        public DatabaseReference GetPostCommentsReferece(string _postId)
        {
            return Database.RootReference.Child(AppSettings.PostCommentsKey).Child(_postId)
                .Child(AppSettings.ContainerListKey);
        }

        public DatabaseReference GetMessageListReferece()
        {
            return Database.RootReference.Child(AppSettings.UserMessagesList)
                .Child(AppManager.Instance.auth.CurrentUser.UserId);
        }

        public DatabaseReference GetUnreadMessageWithUserReferense(string _userID)
        {
            return Database.RootReference.Child(AppSettings.UnreadMessagesKey)
                .Child(AppManager.Instance.auth.CurrentUser.UserId).Child(AppSettings.ContainerListKey).Child(_userID);
        }

        public void CheckAndAddNewChatInfo(MessageGroupInfo _info, Action<ChatInfoMessage> _callback = null)
        {
            Task<DataSnapshot> fTask = Database.RootReference.Child(AppSettings.UserMessagesGroups).Child(_info.ChatID)
                .GetValueAsync();
            ChatInfoMessage _callbackInfo = new ChatInfoMessage();

            fTask.ContinueWith(task2 =>
            {
                if (task2.IsCompleted)
                {
                    if (fTask.Result == null || fTask.Result.Value == null)
                    {
                        AddOrUpdateChatInfo(_info, _callback);
                    }
                    else
                    {
                        _callbackInfo.IsSuccess = false;
                        UnityMainThreadDispatcher.Instance().Enqueue(() => _callback?.Invoke(_callbackInfo));
                    }

                    CleanTask(task2);
                }
                else
                {
                    _callbackInfo.IsSuccess = false;
                    UnityMainThreadDispatcher.Instance().Enqueue(() => _callback?.Invoke(_callbackInfo));
                    CleanTask(task2);
                }
            });
        }

        public void AddOrUpdateChatInfo(MessageGroupInfo _info, Action<ChatInfoMessage> _callback = null)
        {
            string json = JsonUtility.ToJson(_info);
            ChatInfoMessage _callbackInfo = new ChatInfoMessage();

            Database.RootReference.Child(AppSettings.UserMessagesGroups).Child(_info.ChatID).SetRawJsonValueAsync(json)
                .ContinueWith(task =>
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        _callbackInfo.IsSuccess = false;
                        UnityMainThreadDispatcher.Instance().Enqueue(() => _callback?.Invoke(_callbackInfo));
                        CleanTask(task);
                        // Uh-oh, an error occurred!
                    }
                    else
                    {
                        _callbackInfo.IsSuccess = true;
                        _callbackInfo.Info = _info;
                        UnityMainThreadDispatcher.Instance().Enqueue(() => _callback?.Invoke(_callbackInfo));
                        CleanTask(task);
                    }
                });
        }

        public void GetGroupChatInfo(string _chatId, Action<ChatInfoMessage> _callback)
        {
            ChatInfoMessage _callbackInfo = new ChatInfoMessage();

            DatabaseReference _tokenRef = Database.RootReference.Child(AppSettings.UserMessagesGroups).Child(_chatId);
            _tokenRef.GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    _callbackInfo.IsSuccess = false;
                    UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(_callbackInfo));
                }
                else if (task.IsCompleted && task.Result.Exists)
                {
                    _callbackInfo.IsSuccess = true;

                    DataSnapshot snapshot = task.Result;
                    string _json = snapshot.GetRawJsonValue();
                    MessageGroupInfo _info = JsonUtility.FromJson<MessageGroupInfo>(_json);
                    _callbackInfo.Info = _info;

                    UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(_callbackInfo));
                }
                else
                {
                    _callbackInfo.IsSuccess = false;
                    UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(_callbackInfo));
                }
            });
        }

        public void UploadMessage(Message _msg, MessageGroupInfo _info)
        {
            string messageKey = _info.ChatID;
            string json = JsonUtility.ToJson(_msg);
            Database.RootReference.Child(AppSettings.UserMessagesKey).Child(messageKey)
                .Child(AppSettings.ContainerListKey).Push().SetRawJsonValueAsync(json).ContinueWith(task =>
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        CleanTask(task);
                        // Uh-oh, an error occurred!
                    }
                    else
                    {
                        UpdateMessagesList(_info);
                        CleanTask(task);
                    }
                });
        }

        public void UploadGroupMessage(Message _msg, MessageGroupInfo _info)
        {
            string messageKey = _info.ChatID;
            string json = JsonUtility.ToJson(_msg);
            Database.RootReference.Child(AppSettings.UserMessagesKey).Child(messageKey)
                .Child(AppSettings.ContainerListKey).Push().SetRawJsonValueAsync(json).ContinueWith(task =>
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        CleanTask(task);
                        // Uh-oh, an error occurred!
                    }
                    else
                    {
                        // to do : update group list
                        UpdateMessagesList(_info);
                        CleanTask(task);
                    }
                });
        }

        public void UploadComments(Message _msg, string _postId)
        {
            string messageKey = _postId;
            string json = JsonUtility.ToJson(_msg);
            Database.RootReference.Child(AppSettings.PostCommentsKey).Child(messageKey)
                .Child(AppSettings.ContainerListKey).Push().SetRawJsonValueAsync(json).ContinueWith(task =>
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        CleanTask(task);
                        // Uh-oh, an error occurred!
                    }
                    else
                    {
                        CleanTask(task);
                    }
                });
        }

        public void RemoveFromMessageList(MessageGroupInfo _info)
        {
            Database.RootReference.Child(AppSettings.UserMessagesList)
                .Child(AppManager.Instance.auth.CurrentUser.UserId).OrderByValue().EqualTo(_info.ChatID).GetValueAsync()
                .ContinueWithOnMainThread(task1 =>
                {
                    if (task1.IsFaulted || task1.IsCanceled)
                    {
                        CleanTask(task1);
                    }
                    else
                    {
                        List<Task> TaskList = new List<Task>();
                        if (task1.Result != null)
                        {
                            foreach (DataSnapshot _data in task1.Result.Children)
                            {
                                Debug.Log(_data.Key);
                                Task _task = Database.RootReference.Child(AppSettings.UserMessagesList)
                                    .Child(AppManager.Instance.auth.CurrentUser.UserId).Child(_data.Key)
                                    .RemoveValueAsync();
                                TaskList.Add(_task);
                            }
                        }

                        if (TaskList.Count > 0)
                        {
                            Task.WhenAll(TaskList).ContinueWithOnMainThread(task2 =>
                            {
                                if (task2.IsCompleted)
                                {
                                }
                            });
                        }

                        CleanTask(task1);
                    }
                });
        }

        public void UpdateMessagesList(MessageGroupInfo _info)
        {
            //string _userId1 = _info.Users[0];
            //string _userId2 = _info.Users[1];

            for (int i = 0; i < _info.Users.Count; i++)
            {
                string _userID = _info.Users[i];

                FirebaseDatabase.DefaultInstance.RootReference.Child(AppSettings.UserMessagesList).Child(_userID)
                    .OrderByValue().EqualTo(_info.ChatID).GetValueAsync().ContinueWithOnMainThread(task1 =>
                    {
                        if (task1.IsFaulted || task1.IsCanceled)
                        {
                            CleanTask(task1);
                        }
                        else
                        {
                            List<Task> TaskList = new List<Task>();
                            if (task1.Result != null)
                            {
                                foreach (DataSnapshot _data in task1.Result.Children)
                                {
                                    Task _task = Database.RootReference.Child(AppSettings.UserMessagesList)
                                        .Child(_userID).Child(_data.Key).RemoveValueAsync();
                                    TaskList.Add(_task);
                                }
                            }

                            if (TaskList.Count > 0)
                            {
                                Task.WhenAll(TaskList).ContinueWithOnMainThread(task2 =>
                                {
                                    if (task2.IsCompleted)
                                    {
                                        Database.RootReference.Child(AppSettings.UserMessagesList).Child(_userID).Push()
                                            .SetValueAsync(_info.ChatID);
                                    }
                                });
                            }
                            else
                            {
#if !UNITY_EDITOR
                            Debug.Log("push new message to list without removing");
                            Database.RootReference.Child(AppSettings.UserMessagesList).Child(_userID).Push().SetValueAsync((_info.ChatID));
#endif
                            }

                            CleanTask(task1);
                        }
                    });
            }
        }

        public void AddToFriends(string _userId, Action _callback)
        {
            DatabaseReference _requestRef = Database.RootReference.Child(AppSettings.UserRequestedFriendsKey)
                .Child(_userId).Child(AppSettings.ContainerListKey).Child(AppManager.Instance.auth.CurrentUser.UserId);
            DatabaseReference _pendingRef = Database.RootReference.Child(AppSettings.UserPendingFriendsKey)
                .Child(AppManager.Instance.auth.CurrentUser.UserId).Child(AppSettings.ContainerListKey).Child(_userId);
            _requestRef.SetValueAsync(0).ContinueWith(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    CleanTask(task);
                }
                else
                {
                    _pendingRef.SetValueAsync(0).ContinueWith(task2 =>
                    {
                        if (task2.IsFaulted || task2.IsCanceled)
                        {
                            CleanTask(task2);
                        }
                        else
                        {
                            if (_callback != null)
                            {
                                UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke());
                            }

                            AppManager.USER_PROFILE.GetUserFullName(_name =>
                            {
                                NotificationMessage _msg = new NotificationMessage();
                                _msg.UserId = _userId;
                                _msg.Title = _name;
                                _msg.Body = "Wants to be your friend";
                                SendPushNotification(_msg);
                            });

                            CleanTask(task2);
                        }
                    });
                    CleanTask(task);
                }
            });
        }

        public void RemoveFromFriend(string _userId, Action _callback)
        {
            DatabaseReference _requestRef = Database.RootReference.Child(AppSettings.UserFriendsKey).Child(_userId)
                .Child(AppSettings.ContainerListKey).Child(AppManager.Instance.auth.CurrentUser.UserId);
            DatabaseReference _pendingRef = Database.RootReference.Child(AppSettings.UserFriendsKey)
                .Child(AppManager.Instance.auth.CurrentUser.UserId).Child(AppSettings.ContainerListKey).Child(_userId);
            _requestRef.RemoveValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    CleanTask(task);
                }
                else
                {
                    _pendingRef.RemoveValueAsync().ContinueWith(task2 =>
                    {
                        if (task2.IsFaulted || task2.IsCanceled)
                        {
                            CleanTask(task2);
                        }
                        else
                        {
                            if (_callback != null)
                                UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke());
                            CleanTask(task2);
                        }
                    });
                    CleanTask(task);
                }
            });
        }

        public void CancelPendingFromFriend(string _userId, Action _callback)
        {
            DatabaseReference _requestRef = Database.RootReference.Child(AppSettings.UserRequestedFriendsKey)
                .Child(_userId).Child(AppSettings.ContainerListKey).Child(AppManager.Instance.auth.CurrentUser.UserId);
            DatabaseReference _pendingRef = Database.RootReference.Child(AppSettings.UserPendingFriendsKey)
                .Child(AppManager.Instance.auth.CurrentUser.UserId).Child(AppSettings.ContainerListKey).Child(_userId);
            _requestRef.RemoveValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    CleanTask(task);
                }
                else
                {
                    _pendingRef.RemoveValueAsync().ContinueWith(task2 =>
                    {
                        if (task2.IsFaulted || task2.IsCanceled)
                        {
                            CleanTask(task2);
                        }
                        else
                        {
                            if (_callback != null)
                                UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke());
                            CleanTask(task2);
                        }
                    });
                    CleanTask(task);
                }
            });
        }

        public void AcceptFriend(string _userId, Action _callback)
        {
            DatabaseReference _requestRef = Database.RootReference.Child(AppSettings.UserPendingFriendsKey)
                .Child(_userId).Child(AppSettings.ContainerListKey).Child(AppManager.Instance.auth.CurrentUser.UserId);
            DatabaseReference _pendingRef = Database.RootReference.Child(AppSettings.UserRequestedFriendsKey)
                .Child(AppManager.Instance.auth.CurrentUser.UserId).Child(AppSettings.ContainerListKey).Child(_userId);
            DatabaseReference _friendRef1 = Database.RootReference.Child(AppSettings.UserFriendsKey).Child(_userId)
                .Child(AppSettings.ContainerListKey).Child(AppManager.Instance.auth.CurrentUser.UserId);
            DatabaseReference _friendRef2 = Database.RootReference.Child(AppSettings.UserFriendsKey)
                .Child(AppManager.Instance.auth.CurrentUser.UserId).Child(AppSettings.ContainerListKey).Child(_userId);
            Task _task = _requestRef.RemoveValueAsync();
            Task _task2 = _pendingRef.RemoveValueAsync();
            Task _task3 = _friendRef1.SetValueAsync(0);
            Task _task4 = _friendRef2.SetValueAsync(0);
            List<Task> taskList = new List<Task>();
            taskList.Add(_task);
            taskList.Add(_task2);
            taskList.Add(_task3);
            taskList.Add(_task4);
            Task.WhenAll(taskList).ContinueWith(task2 =>
            {
                if (task2.IsCompleted)
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke());
                    CleanTask(task2);
                    CleanTask(_task);
                    CleanTask(_task2);
                    CleanTask(_task3);
                    CleanTask(_task4);
                }
            });
        }

        public void AddCurrencyAndPoints(int amount)
        {
            AddCurrecny(amount);
            AddScore(amount);
        }

        public void AddCurrecny(int amount)
        {
            Database.RootReference.Child(AppSettings.RootUserKey).Child(AppManager.Instance.auth.CurrentUser.UserId)
                .Child(AppSettings.Currecny).RunTransaction(data =>
                {
                    if (data.Value != null)
                    {
                        data.Value = Mathf.Clamp(Convert.ToInt32(data.Value) + amount, 0, Int32.MaxValue);
                    }

                    else
                    {
                        data.Value = 0 + amount;
                    }

                    return TransactionResult.Success(data);
                });
        }

        private void AddXp()
        {
            Database.RootReference.Child(AppSettings.RootUserKey).Child(AppManager.Instance.auth.CurrentUser.UserId)
                .Child(AppSettings.XPLevel).RunTransaction(data =>
                {
                    if (data.Value != null)
                    {
                        data.Value = (Convert.ToInt32(data.Value)) + 1;
                    }

                    else
                    {
                        data.Value = 0 + 1;
                    }


                    return TransactionResult.Success(data);
                });
        }

        public void setXP(int xp)
        {
            Task refe = Database.RootReference.Child(AppSettings.RootUserKey)
                .Child(AppManager.Instance.auth.CurrentUser.UserId)
                .Child(AppSettings.XPLevel).SetValueAsync(xp);
            
            refe.ContinueWith(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    CleanTask(task);
                }
                else
                {
                    
                    CleanTask(task);
                }
            });
        }
        
        private void AddScore(int amount)
        {
            Database.RootReference.Child(AppSettings.RootUserKey).Child(AppManager.Instance.auth.CurrentUser.UserId)
                .Child(AppSettings.Score).RunTransaction(data =>
                {
                    if (data.Value != null)
                    {
                        data.Value = Convert.ToInt32(data.Value) + amount;
                        if (Convert.ToInt32(data.Value) % 100 == 0)
                        {
                            AddXp();
                        }
                    }

                    else
                    {
                        data.Value = 0 + amount;
                    }


                    return TransactionResult.Success(data);
                });
        }

        public void GetScore(string userID,Action<int> _callback)
        {
            Query databaseQuery = Database.RootReference.Child(AppSettings.RootUserKey)
                .Child(userID)
                .Child(AppSettings.Score);
            databaseQuery.GetValueAsync().ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    if (task.Result != null && task.Result.Value != null)
                    {
                        UnityMainThreadDispatcher.Instance()
                            .Enqueue(() => _callback.Invoke(Convert.ToInt32(task.Result.Value)));
                    }

                    CleanTask(task);
                }
                else
                {
                    CleanTask(task);
                }
            });
        }
        public void GetXPLevel(string userID, Action<int> _callback)
        {
            Query databaseQuery = Database.RootReference.Child(AppSettings.RootUserKey)
                .Child(userID)
                .Child(AppSettings.XPLevel);
            databaseQuery.GetValueAsync().ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    if (task.Result != null && task.Result.Value != null)
                    {
                        UnityMainThreadDispatcher.Instance()
                            .Enqueue(() => _callback.Invoke(Convert.ToInt32(task.Result.Value)));
                    }
                    else
                    {
                        UnityMainThreadDispatcher.Instance()
                            .Enqueue(() => _callback.Invoke(Convert.ToInt32(0)));
                    }

                    CleanTask(task);
                }
                else
                {
                    UnityMainThreadDispatcher.Instance()
                        .Enqueue(() => _callback.Invoke(Convert.ToInt32(0)));
                    CleanTask(task);
                }
            });
        }
        public void GetCurrecny(string userID, Action<int> _callback)
        {
            Query databaseQuery = Database.RootReference.Child(AppSettings.RootUserKey)
                .Child(userID)
                .Child(AppSettings.Currecny);
            databaseQuery.GetValueAsync().ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    if (task.Result != null && task.Result.Value != null)
                    {
                        UnityMainThreadDispatcher.Instance()
                            .Enqueue(() => _callback.Invoke(Convert.ToInt32(task.Result.Value)));
                    }

                    CleanTask(task);
                }
                else
                {
                    CleanTask(task);
                }
            });
        }

        public DatabaseReference GetStoreQuery()
        {
            return Database.RootReference.Child(AppSettings.UsersStore)
                .Child(AppManager.Instance.auth.CurrentUser.UserId).Child(AppSettings.ContainerListKey);
        }

        public DatabaseReference GetCurrentScore()
        {
            return Database.RootReference.Child(AppSettings.RootUserKey)
                .Child(AppManager.Instance.auth.CurrentUser.UserId).Child(AppSettings.Score);
        }

        public DatabaseReference GetXP()
        {
            return Database.RootReference.Child(AppSettings.RootUserKey)
                .Child(AppManager.Instance.auth.CurrentUser.UserId).Child(AppSettings.XPLevel);
        }

        public DatabaseReference GetCurrentCurrency()
        {
            return Database.RootReference.Child(AppSettings.RootUserKey)
                .Child(AppManager.Instance.auth.CurrentUser.UserId).Child(AppSettings.Currecny);
        }

        public DatabaseReference GetFriendReferece(string _userID)
        {
            return Database.RootReference.Child(AppSettings.UserFriendsKey)
                .Child(AppManager.Instance.auth.CurrentUser.UserId).Child(AppSettings.ContainerListKey).Child(_userID);
        }

        public DatabaseReference GetRequestFriendReferece(string _userID)
        {
            return Database.RootReference.Child(AppSettings.UserRequestedFriendsKey)
                .Child(AppManager.Instance.auth.CurrentUser.UserId).Child(AppSettings.ContainerListKey).Child(_userID);
        }

        public DatabaseReference GetPendingFriendReferece(string _userID)
        {
            return Database.RootReference.Child(AppSettings.UserPendingFriendsKey)
                .Child(AppManager.Instance.auth.CurrentUser.UserId).Child(AppSettings.ContainerListKey).Child(_userID);
        }

        public DatabaseReference GetFriendCountReferece(string _userID)
        {
            return Database.RootReference.Child(AppSettings.UserFriendsKey).Child(_userID)
                .Child(AppSettings.ListCountKey);
        }

        public DatabaseReference GetRequestFriendCountReferece(string _userID)
        {
            return Database.RootReference.Child(AppSettings.UserRequestedFriendsKey).Child(_userID)
                .Child(AppSettings.ListCountKey);
        }

        public DatabaseReference GetPendingFriendCountReferece(string _userID)
        {
            return Database.RootReference.Child(AppSettings.UserPendingFriendsKey).Child(_userID)
                .Child(AppSettings.ListCountKey);
        }

        public DatabaseReference GetFriendFeedCountReferece(string _userID)
        {
            return Database.RootReference.Child(AppSettings.FriendsPostsKey).Child(_userID)
                .Child(AppSettings.ListCountKey);
        }

        public DatabaseReference GetAllUnreadCountReferece(string _userID)
        {
            return Database.RootReference.Child(AppSettings.UnreadMessagesKey).Child(_userID)
                .Child(AppSettings.ListCountKey);
        }

        public DatabaseReference GetUnreadCountWithUserReferece(string _userID)
        {
            return Database.RootReference.Child(AppSettings.UnreadMessagesKey)
                .Child(AppManager.Instance.auth.CurrentUser.UserId).Child(AppSettings.ContainerListKey).Child(_userID);
        }

        public DatabaseReference GetUnreadCountInGroupReferece(string _groupID)
        {
            return Database.RootReference.Child(AppSettings.UnreadMessagesKey)
                .Child(AppManager.Instance.auth.CurrentUser.UserId).Child(AppSettings.ContainerListKey).Child(_groupID);
        }

        public DatabaseReference GetPostParticipateCountReferense(string _postID)
        {
            return Database.RootReference.Child(AppSettings.PostParticipates).Child(_postID)
                .Child(AppSettings.ListCountKey);
        }

        public DatabaseReference GetPostLikesCountReferense(string _postID)
        {
            return Database.RootReference.Child(AppSettings.PostLikesKey).Child(_postID)
                .Child(AppSettings.ListCountKey);
        }

        public DatabaseReference GetCommentLikesCountReferense(string commentId)
        {
            return Database.RootReference.Child(AppSettings.CommentsLikesKey).Child(commentId)
                .Child(AppSettings.ListCountKey);
        }


        public DatabaseReference GetsCountOptionOneReferense(string _postID)
        {
            return Database.RootReference.Child(AppSettings.PollAnswers).Child(_postID).Child(AppSettings.One)
                .Child(AppSettings.ListCountKey);
        }

        public DatabaseReference GetsCountOptionTwoReferense(string _postID)
        {
            return Database.RootReference.Child(AppSettings.PollAnswers).Child(_postID).Child(AppSettings.Two)
                .Child(AppSettings.ListCountKey);
        }


        public void ClearUnreadMessagesWithUser(string _userID)
        {
            GetUnreadCountWithUserReferece(_userID).RemoveValueAsync();
        }

        public void ClearUnreadMessagesGroup(string _groupID)
        {
            GetUnreadCountInGroupReferece(_groupID).RemoveValueAsync();
        }

        public void ClearUnreadFriendsFeed()
        {
            GetFriendFeedCountReferece(AppManager.Instance.auth.CurrentUser.UserId).RemoveValueAsync();
        }

        public void CanAddToFriend(string _userID, Action<bool> _callback)
        {
            Task<DataSnapshot> fTask = GetFriendReferece(_userID).GetValueAsync();
            Task<DataSnapshot> rTask = GetRequestFriendReferece(_userID).GetValueAsync();
            Task<DataSnapshot> pTask = GetPendingFriendReferece(_userID).GetValueAsync();
            List<Task> TaskList = new List<Task>();
            TaskList.Add(fTask);
            TaskList.Add(rTask);
            TaskList.Add(pTask);
            Task.WhenAll(TaskList).ContinueWith(task2 =>
            {
                if (task2.IsCompleted)
                {
                    if ((fTask.Result == null && rTask.Result == null && pTask.Result == null) ||
                        (fTask.Result.Value == null && rTask.Result.Value == null && pTask.Result.Value == null))
                    {
                        UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(true));
                    }
                    else
                    {
                        UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(false));
                    }

                    CleanTask(task2);
                }
                else
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(false));
                    CleanTask(task2);
                }
            });
        }

        public void IsInFriendsList(string _userID, Action<bool> _callback)
        {
            Task<DataSnapshot> fTask = GetFriendReferece(_userID).GetValueAsync();

            fTask.ContinueWith(task2 =>
            {
                if (task2.IsCompleted)
                {
                    if (fTask.Result == null || fTask.Result.Value == null)
                    {
                        UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(false));
                    }
                    else
                    {
                        UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(true));
                    }

                    CleanTask(task2);
                }
                else
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke(false));
                    CleanTask(task2);
                }
            });
        }

        public void SendActivity()
        {
            Database.RootReference.Child(AppSettings.RootUserKey).Child(AppManager.Instance.auth.CurrentUser.UserId)
                .Child(AppSettings.ActivityKey).SetValueAsync(ServerValue.Timestamp);
        }

        public void GetActivity(string _userID, Action<string> _callback)
        {
            Database.RootReference.Child(AppSettings.RootUserKey).Child(_userID).Child(AppSettings.ActivityKey)
                .GetValueAsync().ContinueWith(task =>
                {
                    if (task.IsCompleted)
                    {
                        if (task.Result != null && task.Result.Value != null &&
                            !string.IsNullOrEmpty(task.Result.Value.ToString()))
                        {
                            UnityMainThreadDispatcher.Instance()
                                .Enqueue(() => _callback.Invoke(task.Result.Value.ToString()));
                        }

                        CleanTask(task);
                    }
                    else
                    {
                        CleanTask(task);
                    }
                });
        }

        public void RemovePost(string _PostID, Action _callback)
        {
            Database.RootReference.Child(AppSettings.AllPostsKey).Child(_PostID).RemoveValueAsync().ContinueWith(
                task2 =>
                {
                    if (task2.IsCompleted)
                    {
                        UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke());
                    }
                });
        }

        /// <summary>
        /// Update user last activity value
        /// </summary>
        /// <param name="_callback">Add completion method</param>
        public void UpdateUserActivity(Action<CallbackSetUserActivity> _callback = null)
        {
            CallbackSetUserActivity _response = new CallbackSetUserActivity();
            DatabaseReference databaseReferense = Database.RootReference.Child(AppSettings.RootUserKey)
                .Child(AppManager.Instance.auth.CurrentUser.UserId).Child(AppSettings.UserActivityKey);
            databaseReferense.SetValueAsync(ServerValue.Timestamp).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    _response.IsSuccess = true;
                    CleanTask(task);
                }
                else
                {
                    _response.ErrorMessage = task.Exception.Message;
                    CleanTask(task);
                }

                if (_callback != null)
                {
                    _callback.Invoke(_response);
                }
            });
        }

        /// <summary>
        /// Get current server time. Availbale when user is logined
        /// </summary>
        /// <param name="_callback">Add completion method</param>
        public void GetServerTimestamp(Action<CallbackGetServerTimestamp> _callback)
        {
            CallbackGetServerTimestamp _response = new CallbackGetServerTimestamp();
            UpdateUserActivity(_msg =>
            {
                if (_msg.IsSuccess)
                {
                    DatabaseReference databaseReferense = Database.RootReference.Child(AppSettings.RootUserKey)
                        .Child(AppManager.Instance.auth.CurrentUser.UserId).Child(AppSettings.UserActivityKey);
                    databaseReferense.GetValueAsync().ContinueWithOnMainThread(task =>
                    {
                        if (task.IsCompleted)
                        {
                            _response.IsSuccess = true;
                            _response.Data = task.Result.Value.ToString();
                            CleanTask(task);
                        }
                        else
                        {
                            _response.ErrorMessage = task.Exception.Message;
                            CleanTask(task);
                        }

                        if (_callback != null)
                        {
                            _callback.Invoke(_response);
                        }
                    });
                }
                else
                {
                    if (_callback != null)
                    {
                        _callback.Invoke(_response);
                    }
                }
            });
        }

        /// <summary>
        /// Get user last activity by user id.
        /// </summary>
        /// <param name="_userID">User ID</param>
        /// <param name="_callback">Add completion method</param>
        public void GetUserLastActivity(string _userID, Action<CallbackGetUserActivity> _callback)
        {
            CallbackGetUserActivity _response = new CallbackGetUserActivity();
            DatabaseReference databaseReferense = Database.RootReference.Child(AppSettings.RootUserKey)
                .Child(_userID).Child(AppSettings.UserActivityKey);
            databaseReferense.GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    _response.IsSuccess = true;
                    _response.Data = task.Result.Value.ToString();
                    CleanTask(task);
                }
                else
                {
                    _response.ErrorMessage = task.Exception.Message;
                    CleanTask(task);
                }

                if (_callback != null)
                {
                    _callback.Invoke(_response);
                }
            });
        }

        public void MakeCallOffer(CallObject _call, Action<CallbackMakeCallMessage> _callback)
        {
            CallbackMakeCallMessage _response = new CallbackMakeCallMessage();

            GetServerTimestamp(_msg =>
            {
                if (_msg.IsSuccess)
                {
                    string _timeStamp = _msg.Data;
                    _call.CreateTimeStamp = _timeStamp;
                    //string _callKey = System.Guid.NewGuid().ToString();
                    DatabaseReference _callref = Database.RootReference.Child(AppSettings.UserCallList)
                        .Child(_call.TargetID).Push();
                    _call.DataKey = _callref.Key;
                    string json = JsonUtility.ToJson(_call);

                    _callref.SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
                    {
                        if (task.IsFaulted || task.IsCanceled)
                        {
                            CleanTask(task);
                            _callback.Invoke(_response);
                            // Uh-oh, an error occurred!
                        }
                        else
                        {
                            _response.IsSuccess = true;
                            _callback.Invoke(_response);
                            CleanTask(task);
                        }
                    });
                }
                else
                {
                    _callback.Invoke(_response);
                }
            });
        }

        public void CancelCallOffer(CallObject _call, Action _onComplete = null)
        {
            GetCallActiveStateReference(_call).SetValueAsync(false).ContinueWithOnMainThread(task =>
            {
                if (_onComplete != null)
                {
                    _onComplete.Invoke();
                }
            });
        }

        public void AnswerCallOffer(CallObject _call, Action _onComplete = null)
        {
            GetCallAnswerStateReference(_call).SetValueAsync(true).ContinueWithOnMainThread(task =>
            {
                if (_onComplete != null)
                {
                    _onComplete.Invoke();
                }
            });
        }

        public void SetCallBisy(CallObject _call, Action _onComplete = null)
        {
            GetCallBisyStateReference(_call).SetValueAsync(true).ContinueWithOnMainThread(task =>
            {
                if (_onComplete != null)
                {
                    _onComplete.Invoke();
                }
            });
        }

        public DatabaseReference GetCallReference()
        {
            return Database.RootReference.Child(AppSettings.UserCallList)
                .Child(AppManager.Instance.auth.CurrentUser.UserId);
        }

        public DatabaseReference GetCallActiveStateReference(CallObject _call)
        {
            return Database.RootReference.Child(AppSettings.UserCallList).Child(_call.TargetID).Child(_call.DataKey)
                .Child(AppSettings.CallActiveKey);
        }

        public DatabaseReference GetCallBisyStateReference(CallObject _call)
        {
            return Database.RootReference.Child(AppSettings.UserCallList).Child(_call.TargetID).Child(_call.DataKey)
                .Child(AppSettings.CallBisyKey);
        }

        public DatabaseReference GetCallAnswerStateReference(CallObject _call)
        {
            return Database.RootReference.Child(AppSettings.UserCallList).Child(_call.TargetID).Child(_call.DataKey)
                .Child(AppSettings.CallHasAnswerKey);
        }

        public string GetUserMessageKey(string _id1, string _id2)
        {
            List<string> sList = new List<string>();
            sList.Add(_id1);
            sList.Add(_id2);
            sList.Sort();
            return sList[0] + "-" + sList[1];
        }

        public bool IsFirebaseInited()
        {
            return FirebaseIsInited;
        }

        private void CleanTask(Task _task)
        {
            _task.Dispose();
            _task = null;
        }
    }

    public class RegistrationMessage
    {
        public bool IsComplete;
        public string ErrorMessage;
        public string UserID;
    }

    public class SetUserDataMessage
    {
        public bool IsSuccess;
        public string ErrorMessage;
        public string UserID;
        public string PhotoUrl;
    }

    public class AddGroupMessage
    {
        public bool IsComplete;
        public bool IsSuccess;
        public string groupID;
    }


    public class LoginMessage
    {
        public bool IsSuccess;
        public string ErrorMessage;
        public string UserID;
        public FirebaseUser FUser;
    }

    public class NotificationMessage
    {
        public string UserToken;
        public string Title;
        public string Body;
        public string UserId;
    }

    public class ChatInfoMessage
    {
        public bool IsSuccess;
        public MessageGroupInfo Info;
    }

    [System.Serializable]
    public class MessageGroupInfo
    {
        public string ChatID;
        public string ChatName;
        public List<string> Users;
        public MessageType Type;
    }

    /// <summary>
    /// Get user activity callback class
    /// </summary>
    public class CallbackGetUserActivity
    {
        public bool IsSuccess;
        public string ErrorMessage;
        public string Data;
    }

    /// <summary>
    /// Get server time stamp callback class
    /// </summary>
    public class CallbackGetServerTimestamp
    {
        public bool IsSuccess;
        public string ErrorMessage;
        public string Data;
    }

    /// <summary>
    /// Set user activity callback class
    /// </summary>
    public class CallbackSetUserActivity
    {
        public bool IsSuccess;
        public string ErrorMessage;
    }

    public class CallbackMakeCallMessage
    {
        public bool IsSuccess;
    }
}