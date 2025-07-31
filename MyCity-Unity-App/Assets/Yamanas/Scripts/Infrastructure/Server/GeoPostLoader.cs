using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using com.draconianmarshmallows.geofire;
using Firebase.Database;
using SocialApp;
using UnityEngine;
using Yamanas.Scripts.MapLoader;

namespace Yamanas.Scripts.Infrastructure.Server
{
    public class GeoPostLoader : MonoBehaviour
    {
        #region Fields

        private static GeoPostLoader _instance;

        private DatabaseReference reference;

        private List<GeoQuery> geoquries = new List<GeoQuery>();

        private List<Feed> feedOfQuery = new List<Feed>();

         Dictionary<string, bool> _filteredData;

        private float _filteredRadius;

        #endregion

        #region Methods


        public void ChangeFilterDate(Dictionary<string, bool> filterData, float radius)
        {
            _filteredData = filterData;
            _filteredRadius = radius;
        }
        
        private void Awake()
        {
            _filteredData = new Dictionary<string, bool>()
            {
                {"Sale", true},
                {"Event", true},
                {"Poll", true},
                {"Share", true},
            };
            _filteredRadius = 30;
            reference = AppManager.FIREBASE_CONTROLLER.GETDataBase().RootReference;
        }


        public void makeUsualQuery()
        {
            foreach (GeoQuery geo in geoquries)
            {
                geo.reset();
            }

            OnlineMapsMarker3DManager.RemoveAllItems();


            foreach (var tag in _filteredData)
            {
                if (tag.Value == true)
                {
                    MakeTagQuery(tag.Key, _filteredRadius);
                }
            }
        }

        public void MakeTagQuery(string Tag, float radius)
        {
            Debug.Log("tags is " + AppSettings.tags);

            Debug.Log("current tag " + AppManager.myCityController.currentTag);
            Debug.Log("lat is " + AppManager.myCityController.lat);
            Debug.Log("lng is " + AppManager.myCityController.lng);
            Debug.Log("radius is " + AppManager.myCityController.Radius);


            Debug.Log("the date now is " + DateTime.Now.ToString("MM-yyyy"));


            DateTime d = new DateTime();


            if (reference == null)
            {
                Debug.Log("ref is null " + reference.ToString());
            }
            else
            {
                Debug.Log("ref is not null " + reference.ToString());
            }


            var geofireDbReference =
                reference.Child(AppSettings.tags).Child(Tag).Child(DateTime.Now.ToString("MM-yyyy"));
            var geoFire = new GeoFire(geofireDbReference);


            GeoQuery query =
                geoFire.queryAtLocation(new GeoLocation(OnlineMaps.instance.position.y, OnlineMaps.instance.position.x),
                    radius);


            query.geoQueryReadyListeners += onGeoQueryReady;
            query.geoQueryErrorListeners += onGeoQueryError;
            query.keyEnteredListeners += onKeyEntered;
            query.keyExitedListeners += onKeyExited;
            query.keyMovedListeners += onKeyMoved;

            Debug.Log("inside query");

            query.initializeListeners();


            geoquries.Add(query);
        }

        private void onKeyMoved(string arg1, GeoLocation arg2)
        {
            Debug.Log("moved");
        }

        private void onKeyExited(string obj)
        {
            Debug.Log("exit");
        }

        private void onGeoQueryError(DatabaseError obj)
        {
            Debug.Log("Error in " + AppManager.myCityController.currentTag);
        }

        private void onGeoQueryReady()
        {
            Debug.Log(AppManager.myCityController.currentTag);
        }

        private void onKeyEntered(string arg1, GeoLocation arg2)
        {
            //      yield return new WaitForSeconds(0.75f);


            DatabaseReference _feedRef = reference.Child(AppSettings.AllPostsKey).Child(arg1);
            _feedRef.GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    CleanTask(task);
                }


                else
                {
                    string feed = task.Result.Key;
                    string jsonFeed = task.Result.GetRawJsonValue();


                    if (!string.IsNullOrEmpty(jsonFeed))
                    {
                        Feed _dataFeed = JsonUtility.FromJson<Feed>(jsonFeed);

                        feedOfQuery.Add(_dataFeed);


                        try
                        {
                            UnityMainThreadDispatcher.Instance().Enqueue(() =>


                            {
                                PinFactory.Instance.CreatePin(_dataFeed);

                                Debug.Log("pin suppose to be created");
                            });
                        }

                        catch (Exception e)
                        {
                            Debug.Log(e.ToString());
                        }
                    }
                }
            });
        }


        private void CleanTask(Task<DataSnapshot> task)
        {
            task.Dispose();
            task = null;
        }

        #endregion


        #region Properties

        public static GeoPostLoader Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<GeoPostLoader>();
                }

                return _instance;
            }
        }

        #endregion
    }
}