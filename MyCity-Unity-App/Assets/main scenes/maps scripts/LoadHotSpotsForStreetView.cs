using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Firebase;
using Firebase.Database;
//using Firebase.Database;
using Firebase.Unity.Editor;
using InfinityCode.OnlineMapsDemos;
using InfinityCode.uPano;
using InfinityCode.uPano.Examples;
using InfinityCode.uPano.HotSpots;
using InfinityCode.uPano.InteractiveElements;
using InfinityCode.uPano.Services;
using JetBrains.Annotations;
using SocialApp;
using UnityEngine;
using UnityEngine.UI;

public class LoadHotSpotsForStreetView : MonoBehaviour
{
    public OnlineMapsPanoConnector panoConnector;
    public GameObject[] hotSpotPrefabs;
    public AnimationCurve sizeDistanceCurve = new AnimationCurve(
        new Keyframe(0, 1),
        new Keyframe(10, 0.5f),
        new Keyframe(100, 0.1f),
        new Keyframe(1000, 0.01f)
    );

    private double panoLongitude;
    private double panoLatitude;
    private double panoAltitude;
    public GameObject DebugText;
    public GameObject prefab;

    private HotSpot hotspot1;
    Text debug;

  //  public static List<LocationToFireBase> ListLocations = new List<LocationToFireBase>();
    public static List<HotSpot> hotspots = new List<HotSpot>();
    List<HotSpot> hotSpotsFromMap = new List<HotSpot>();
  //  public static List<HotSpotAndLocation> hotSpotslocations = new List<HotSpotAndLocation>();

    private bool CanLikePost = false;
    private bool IsPostLiked = false;
    [SerializeField]
    private Color LikedPostColor = default;
    [SerializeField]
    private Color UnLikedPostColor = default;
    [SerializeField]
    private Image LikeImage = default;
    [SerializeField]
    private Text LikesCountBody = default;

    private DatabaseReference DRPostLikesCount;
    string feedkey;

    [SerializeField]
    private GameObject CommentsObject = default;

    [SerializeField]
    private GameObject search = default;
    public Text CommentsCountBody;
    public GameObject owner;

    public static bool isStarted;

   /* public GameObject cleaningpano;
    public GameObject fixStreetViewpano;
    public GameObject PesticideStreetViewpano;
    public GameObject dumpsterStreetViewpano;
    public GameObject benchStreetViewpano;
    public GameObject treeStreetViewpano;
    public GameObject signStreetViewpano;
    public GameObject cuttingStreetViewpano;
    public GameObject lightingStreetViewpano;
    public GameObject roundaboutStreetViewpano;
    public GameObject crossingStreetViewpano;
    public GameObject parkStreetViewpano;
    const string cleaningString = "cleaning";
    const string fixString = "fix";
    const string PesticideString = "Pesticide";
    const string dumpsterString = "dumpster";
    const string benchString = "bench";
    const string treeString = "tree";
    const string signString = "traffic sign";
    const string cuttingString = "Collect pruning";
    const string lightingString = "lighting";
    const string roundaboutString = "roundabout";
    const string crossingString = "pedestrian crossing";
    const string parkString = "kids park";*/

    Vector4 EulerToQuaternion(Vector3 p)
    {
        p.x *= Mathf.Deg2Rad;
        p.y *= Mathf.Deg2Rad;
        p.z *= Mathf.Deg2Rad;
        Vector4 q;
        float cy = Mathf.Cos(p.z * 0.5f);
        float sy = Mathf.Sin(p.z * 0.5f);
        float cr = Mathf.Cos(p.y * 0.5f);
        float sr = Mathf.Sin(p.y * 0.5f);
        float cp = Mathf.Cos(p.x * 0.5f);
        float sp = Mathf.Sin(p.x * 0.5f);
        q.w = cy * cr * cp + sy * sr * sp;
        q.x = cy * cr * sp + sy * sr * cp;
        q.y = cy * sr * cp - sy * cr * sp;
        q.z = sy * cr * cp - cy * sr * sp;
        return q;
    }



    public void ShowPostComments(string _id)
    {
        

        search.SetActive(false);
        owner.SetActive(false);
       //bubble.SetActive(false);
        LoadComments();
        CommentsObject.SetActive(true);
        CommentsObject.GetComponentInChildren<MessagesDataLoader>().LoadPostComments(feedkey);
    }

    public void HidePostComments()
    {
        owner.SetActive(true);
        search.SetActive(true);
        //bubble.SetActive(true);
        CommentsObject.SetActive(false);
    }
    private void LoadComments()
    {
        AppManager.FIREBASE_CONTROLLER.GetPostCommentsCount(feedkey, _count =>
        {
            CommentsCountBody.text = _count.ToString();
        });
    }















    private void Start()
    {



        Pano.OnPanoStarted += OnPanoStarted;

    }

    private void OnPanoStarted(Pano pano)
    {


        markersMode.markerMode = false;

        isStarted = true;






        Debug.Log("HERE");
        Debug.Log(panoConnector);
        Debug.Log("after panoconnetor");

        //GoogleStreetViewMeta meta = panoConnector.meta;
       /* Debug.Log(meta);
        Debug.Log("after meta");

        if (meta == null) return;

        Debug.Log("Pano ID: " + meta.id);

        panoLongitude = meta.longitude;
        panoLatitude = meta.latitude;
        panoAltitude = meta.altitude;
*/

        HotSpotManager manager = pano.GetComponent<HotSpotManager>();
        if (manager == null) manager = pano.gameObject.AddComponent<HotSpotManager>();
        Debug.Log("before manager");
        Debug.Log(manager);

        
        //FirebaseController controller = FindObjectOfType<FirebaseController>();

        //loadFromDatabase(manager);

      // panoConnector.OnLoaded += onLoaded;










        /*if (ListLocations.Count>0) {


            for (int i = 0; i < ListLocations.Count; i++)
            {




                HotSpot hotSpot = manager.Create(0, 0, prefab);
                

                if (hotSpot != null)
                {
                    // Debug.Log("altitude form without altitude"+ ListLocations.ElementAt(i).altitude);

                    if (ListLocations.ElementAt(i) != null) { 


                        if (ListLocations.ElementAt(i).altitude == 15)
                        {
                            hotSpot.rotation = new Quaternion(2.462f, -176.712f, 0, 1);
                            hotspots.Add(hotSpot);
                            UpdatePosition(hotSpot, ListLocations.ElementAt(i).longtitude, ListLocations.ElementAt(i).latitude, 62);
                        }



                        else if (ListLocations.ElementAt(i).altitude == 0)
                        {
                            hotspots.Add(hotSpot);
                            UpdatePosition(hotSpot, ListLocations.ElementAt(i).longtitude, ListLocations.ElementAt(i).latitude, 60);

                        }

                        else if (ListLocations.ElementAt(i).altitude != 0 && ListLocations.ElementAt(i).altitude != 15)
                        {
                            hotspots.Add(hotSpot);
                            UpdatePosition(hotSpot, ListLocations.ElementAt(i).longtitude, ListLocations.ElementAt(i).latitude, ListLocations.ElementAt(i).altitude);

                        }


                }
            }

        }*/

        /*  foreach (Record record in ReadHotSpotsFromDB(meta.id))
          {
              int prefabIndex = record.prefabIndex;
              if (prefabIndex < 0 || prefabIndex >= hotSpotPrefabs.Length) continue;
              GameObject prefab = hotSpotPrefabs[prefabIndex];
              if (prefab == null) continue;

              HotSpot hotSpot = manager.Create(0, 0, prefab);
              UpdatePosition(hotSpot, record.longitude, record.latitude, record.altitude);
          }
*/







    }

    /*private void loadFromDatabase(HotSpotManager manager)
    {

       

        if (UIBubblePopup.feeds.Count > 0)
        {
            foreach (Feed feed in UIBubblePopup.feeds)
            {

                if (feed.locationType.Equals("StreetView"))
                {

                    if (feed.panoramaID.Equals(panoConnector.meta.id))
                    {
                        

                        switch (feed.typeOfPrefab)
                        {
                            case cleaningString:
                                HotSpot hotSpot = manager.Create((float)Convert.ToDouble(feed.panStreetview), (float)Convert.ToDouble(feed.tiltStreetview), cleaningpano);

                                hotSpot.scale = new Vector3(0.008678579f, 0.008678579f, 0.008678579f);
                                hotSpot.distanceMultiplier -= 0.13f;
                                Vector4 vec = EulerToQuaternion(new Vector3(-109.407f, -155.85f, 149.303f));
                                hotSpot.rotation = new Quaternion(vec.x, vec.y, vec.z, vec.w);
                                
                                hotSpot["feed"] = feed;
                                hotSpot.OnClick.AddListener(SomeMethod);
                                hotspots.Add(hotSpot);
                                break;


                            case fixString:
                                hotSpot = manager.Create((float)Convert.ToDouble(feed.panStreetview), (float)Convert.ToDouble(feed.tiltStreetview), fixStreetViewpano);
                               
                                
                              //  hotSpot.distanceMultiplier -= 0.09f;
                               
                                
                                hotSpot["feed"] = feed;
                                hotSpot.OnClick.AddListener(SomeMethod);
                                hotspots.Add(hotSpot);
                                break;

                            case PesticideString:
                                hotSpot = manager.Create((float)Convert.ToDouble(feed.panStreetview), (float)Convert.ToDouble(feed.tiltStreetview), PesticideStreetViewpano);
                                hotSpot.scale = new Vector3(0.01f, 0.01f, 0.01f);
                                hotSpot.distanceMultiplier -= 0.09f;

                                vec = EulerToQuaternion(new Vector3(-60.345f, -212.322f, 21.523f));
                                hotSpot.rotation = new Quaternion(vec.x, vec.y, vec.z, vec.w);
                                hotSpot["feed"] = feed;
                                hotSpot.OnClick.AddListener(SomeMethod);
                                hotspots.Add(hotSpot);
                                break;


                            case dumpsterString:
                                 hotSpot = manager.Create((float)Convert.ToDouble(feed.panStreetview), (float)Convert.ToDouble(feed.tiltStreetview), dumpsterStreetViewpano);
                                //hotSpot.distanceMultiplier -= 0.09f;
                                hotSpot["feed"] = feed;
                                hotSpot.OnClick.AddListener(SomeMethod);
                                hotspots.Add(hotSpot);
                                break;
                            case benchString:
                                hotSpot = manager.Create((float)Convert.ToDouble(feed.panStreetview), (float)Convert.ToDouble(feed.tiltStreetview), benchStreetViewpano);
                                hotSpot.scale = new Vector3(0.043272f, 0.043272f, 0.043272f);
                                hotSpot.distanceMultiplier -= 0.09f;

                                vec = EulerToQuaternion(new Vector3(-1.745f, 98.42001f, -1.481f));
                                hotSpot.rotation = new Quaternion(vec.x, vec.y, vec.z, vec.w);
                                hotSpot["feed"] = feed;
                                hotSpot.OnClick.AddListener(SomeMethod);
                                hotspots.Add(hotSpot);
                                break;
                            case treeString:
                                 hotSpot = manager.Create((float)Convert.ToDouble(feed.panStreetview), (float)Convert.ToDouble(feed.tiltStreetview), treeStreetViewpano);
                                hotSpot.scale = new Vector3(-0.02281637f, -0.02281637f, -0.02281637f);
                                hotSpot.distanceMultiplier -= 0.13f;

                                vec = EulerToQuaternion(new Vector3(84.11301f, 82.34801f, 80.79401f));
                                hotSpot.rotation = new Quaternion(vec.x, vec.y, vec.z, vec.w);
                                hotSpot["feed"] = feed;
                                hotSpot.OnClick.AddListener(SomeMethod);
                                hotspots.Add(hotSpot);
                                break;
                            case signString:
                                hotSpot = manager.Create((float)Convert.ToDouble(feed.panStreetview), (float)Convert.ToDouble(feed.tiltStreetview), signStreetViewpano);
                                hotSpot.scale = new Vector3(0.16496f, 0.16496f, 0.16496f);
                                hotSpot.distanceMultiplier -= 0.13f;
                                vec = EulerToQuaternion(new Vector3(-76.354f, 146.303f, -294.724f));
                                hotSpot.rotation = new Quaternion(vec.x, vec.y, vec.z, vec.w);
                                hotSpot["feed"] = feed;
                                hotSpot.OnClick.AddListener(SomeMethod);
                                hotspots.Add(hotSpot);
                                break;
                            case cuttingString:
                                hotSpot = manager.Create((float)Convert.ToDouble(feed.panStreetview), (float)Convert.ToDouble(feed.tiltStreetview), cuttingStreetViewpano);
                                hotSpot.scale = new Vector3(0.12651f, 0.12651f, 0.12651f);
                                hotSpot.distanceMultiplier -= 0.09f;
                                vec = EulerToQuaternion(new Vector3(110.122f, -128.769f, -306.492f));
                                hotSpot.rotation = new Quaternion(vec.x, vec.y, vec.z, vec.w);
                                hotSpot["feed"] = feed;
                                hotSpot.OnClick.AddListener(SomeMethod);
                                hotspots.Add(hotSpot);
                                break;
                            case lightingString:
                                hotSpot = manager.Create((float)Convert.ToDouble(feed.panStreetview), (float)Convert.ToDouble(feed.tiltStreetview), lightingStreetViewpano);
                                hotSpot.scale = new Vector3(0.03044155f, 0.03044155f, 0.03044155f);
                                hotSpot.distanceMultiplier -= 0.13f;
                                vec = EulerToQuaternion(new Vector3(-94.616f, 88.976f, -87.905f));

                                hotSpot.rotation = new Quaternion(vec.x, vec.y, vec.z, vec.w);
                                hotSpot["feed"] = feed;
                                hotSpot.OnClick.AddListener(SomeMethod);
                                hotspots.Add(hotSpot);
                                break;
                            case roundaboutString:
                                hotSpot = manager.Create((float)Convert.ToDouble(feed.panStreetview), (float)Convert.ToDouble(feed.tiltStreetview), roundaboutStreetViewpano);
                                hotSpot.scale = new Vector3(-0.00325625f, -0.00325625f, -0.00325625f);
                                hotSpot.distanceMultiplier -= 0.09f;
                                vec = EulerToQuaternion(new Vector3(-338.851f, 166.778f, 175.575f));
                                hotSpot.rotation = new Quaternion(vec.x, vec.y, vec.z, vec.w);
                                hotSpot["feed"] = feed;
                                hotSpot.OnClick.AddListener(SomeMethod);
                                hotspots.Add(hotSpot);
                                break;
                            case crossingString:
                                hotSpot = manager.Create((float)Convert.ToDouble(feed.panStreetview), (float)Convert.ToDouble(feed.tiltStreetview), crossingStreetViewpano);
                                hotSpot.scale = new Vector3(695.9249f, 695.9249f, 695.9249f);
                                hotSpot.distanceMultiplier -= 0.13f;
                                hotSpot["feed"] = feed;
                                hotSpot.OnClick.AddListener(SomeMethod);
                                hotspots.Add(hotSpot);
                                break;
                            case parkString:
                                hotSpot = manager.Create((float)Convert.ToDouble(feed.panStreetview), (float)Convert.ToDouble(feed.tiltStreetview), parkStreetViewpano);
                                hotSpot.distanceMultiplier -= 0.13f;
                                hotSpot["feed"] = feed;
                                hotSpot.OnClick.AddListener(SomeMethod);
                                hotspots.Add(hotSpot);
                                break;























                        }









                    }





                }

                *//*HotSpot hotSpot = manager.Create(0, 0, prefab);
                hotSpot.distanceMultiplier -= 0.09f;
                


                UpdatePosition(hotSpot, Convert.ToDouble(location.longtitude), Convert.ToDouble(location.longtitude), 62);


                HotSpotAndLocation hotSpotAndLocation = new HotSpotAndLocation(hotSpot, Convert.ToDouble(location.longtitude), Convert.ToDouble(location.latitude));
                hotSpotslocations.Add(hotSpotAndLocation);
*//*


            }





        }
    }*/

    private void OnLikesCountUpdated(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            LikesCountBody.text = "0";
            return;
        }
        try
        {
            if (args.Snapshot.Value.ToString() == "0")
            {
                LikesCountBody.text = "0";
            }
            else
            {
                LikesCountBody.text = args.Snapshot.Value.ToString();
            }
        }
        catch (Exception e)
        {
            LikesCountBody.text = "0";
            Debug.Log(e.ToString());

        }
    }


    private void LoadLikes()
    {
        CanLikePost = false;

        AppManager.FIREBASE_CONTROLLER.IsLikedPost(feedkey, _isLike =>
        {
            CanLikePost = true;
            IsPostLiked = _isLike;
            if (IsPostLiked)
            {

                Debug.Log("true like color");


                try
                {
                    LikeImage.color = LikedPostColor;

                }


                catch (Exception e)
                {

                    Debug.Log(e.ToString());
                }


            }
            else
            {

                Debug.Log("false unlike color");

                try
                {
                    LikeImage.color = UnLikedPostColor;

                }


                catch (Exception e)
                {

                    Debug.Log(e.ToString());
                }




            }
        });
    }
    public void ClickLike()
    {

        Debug.Log("like is clicked");
        if (CanLikePost)
        {
            if (IsPostLiked)
            {
                AppManager.FIREBASE_CONTROLLER.UnLikPost(feedkey, success =>
                {
                    if (success)
                    {
                        LoadLikes();
                    }
                });
            }
            else
            {
                AppManager.FIREBASE_CONTROLLER.LikPost(feedkey, success =>
                {
                    if (success)
                    {
                        LoadLikes();
                    }
                });
            }
        }
    }

    private void SomeMethod(InteractiveElement arg0)
    {
        Debug.Log("Long press");
        hotspot1 = arg0 as HotSpot;

        // Set active marker reference
        //targetMarker = marker as OnlineMapsMarker;

        // Get a result item from instance of the marker
        /*CData data = marker["data"] as CData;
        if (data == null) return;*/


        Feed feed = arg0["feed"] as Feed;

        FirebaseController controller = FindObjectOfType<FirebaseController>();
      //  MapsViewController maps = FindObjectOfType<MapsViewController>();

        feedkey = feed.Key;
        


        DRPostLikesCount = AppManager.FIREBASE_CONTROLLER.GetPostLikesCountReferense(feedkey);
        DRPostLikesCount.ValueChanged += OnLikesCountUpdated;














        controller.GetUserFullName(feed.OwnerID, (_userName =>
        {

            //maps.Text.text = "created by " + _userName;


        }));

        GetProfileImageRequest _request = new GetProfileImageRequest();
        _request.Id = feed.OwnerID;
        _request.Size = ImageSize.Size_512;
        controller.GetProfileImage(_request, _callback =>
        {

            if (_callback.IsSuccess)
            {
                AppManager.USER_PROFILE.PROFILE_IMAGE_LOADED = true;
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(_callback.ImageBytes);
                //maps.Image.texture = texture;

            }

          


        });

        LoadLikes();


/*
        maps.maps.SetActive(false);
        maps.profile.SetActive(true);


*/

        }
    private void onLoaded(Pano obj)
    {


        for(int i = 0; i < hotspots.Count; i++)
        {
            hotspots[i].Destroy();

        }
        hotspots.Clear();

        HotSpotManager manager = obj.GetComponent<HotSpotManager>();
        //loadFromDatabase(manager);


        /* HotSpotManager manager = obj.GetComponent<HotSpotManager>();
         GoogleStreetViewMeta meta = panoConnector.meta;
         if (meta == null) return;
         panoLongitude = meta.longitude;
         panoLatitude = meta.latitude;
         panoAltitude = meta.altitude;

         Debug.Log("number of items in list: " + ListLocations.Count);


         foreach (HotSpotAndLocation location in hotSpotslocations)
         {


             UpdatePosition(location.hotSpot, location.longtitude, location.latitude, 67);


         }





         if (MyCityMananger.PanoramaID != "")
         {




             if (MyCityMananger.PanoramaID.Equals(panoConnector.meta.id))
             {
                 HotSpotManager manager1 = obj.GetComponent<HotSpotManager>();


                 Debug.Log("before create on loaded");

                 HotSpot hotspot2 = manager1.Create(MyCityMananger.Pan, MyCityMananger.Tilt, prefab);
                 hotspot2.distanceMultiplier -= 0.09f;
                 try
                 {

                     Debug.Log("in onloaded  pan:" + hotspot2.pan + " tilt:" + hotspot2.tilt);
                 }

                 catch (Exception e)
                 {
                     Debug.Log(e.ToString());
                 }


             }







         }
     */




        /*
                int countFromMap = 0;


                for (int i = 0; i < ListLocations.Count; i++)
                {

                    if (ListLocations.ElementAt(i).altitude == 15)
                    {


                        UpdatePosition(hotspots[i], ListLocations.ElementAt(i).longtitude, ListLocations.ElementAt(i).latitude, 67);
                    }




                    if (ListLocations.ElementAt(i).altitude == 0)
                    {
                        // HotSpot hotSpot = manager.Create(0, 0, prefab);

                        UpdatePosition(hotspots[i], ListLocations.ElementAt(i).longtitude, ListLocations.ElementAt(i).latitude, 60);

                    }




                    else if (ListLocations.ElementAt(i).altitude != 0)
                    {

                        UpdatePosition(hotspots[i], ListLocations.ElementAt(i).longtitude, ListLocations.ElementAt(i).latitude, ListLocations.ElementAt(i).altitude);
                    }







                }*/

    }

















    private IEnumerable<Record> ReadHotSpotsFromDB(string panoID)
    {
        List<Record> records = new List<Record>();

        // Here you load your hotspots

        records.Add(new Record // Test hotspot
        {
            prefabIndex = 0,
            longitude = 0,
            latitude = 0,
            altitude = 0
        });

        return records;
    }

    private void UpdatePosition(HotSpot hotSpot, double longitude, double latitude, double altitude)
    {

        Debug.Log("is worked in update?");

        double tx1, tx2, ty1, ty2;
        GeoHelper.CoordinatesToTile(panoLongitude, panoLatitude, 20, out tx1, out ty1);
        GeoHelper.CoordinatesToTile(longitude, latitude, 20, out tx2, out ty2);

        double distance = GeoHelper.Distance(panoLongitude, panoLatitude, longitude, latitude) * 1000;

        float pan = 90 + (float)MathHelper.Angle2D(tx1, ty1, tx2, ty2);
        float tilt = 0;

        double offset = altitude - panoAltitude;
        if (Math.Abs(offset) > float.Epsilon)
        {
            tilt = (float)Math.Atan2(offset, distance) * Mathf.Rad2Deg;
        }

        hotSpot.SetPanTilt(pan, tilt);
        float scale = sizeDistanceCurve.Evaluate((float)distance);
        hotSpot.scale = new Vector3(scale, scale, scale);
        hotSpot.rotation = new Quaternion(2.583f, -196.674f, 12.799f, 1);


    }
    public void OnProfileImageGetted(GetProfileImageCallback _callback)
    {
        if (_callback.IsSuccess)
        {
            AppManager.USER_PROFILE.PROFILE_IMAGE_LOADED = true;
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(_callback.ImageBytes);
           

            // photo.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);

        }
        else
        {
            DisplayDefaultAvatar();
        }
    }

    private void DisplayDefaultAvatar()
    {
        Texture2D texture = AppManager.APP_SETTINGS.DefaultAvatarTexture;
        

    }
    internal class Record
    {
        public int prefabIndex;
        public double longitude;
        public double latitude;
        public double altitude;
    }
}