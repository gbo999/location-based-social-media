/*
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using Google.XR.ARCoreExtensions;
using SocialApp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using static AppController;

public class instantiateAR : MonoBehaviour
{


    //public GameObject prefab;
   // public static List<LocationMyCity> locations = new List<LocationMyCity>();
    public static List<Feed> feeds = new List<Feed>();

    public TextMeshProUGUI resolveDebug;
    public TextMeshProUGUI HostDebug;
    public TextMeshProUGUI cloudID;


    double lati;
    double longi;
    private GameObject game;

   // public Button debugbtn;

    int counter = 0;


    public static bool createdHasPressed=false;

    public  const string cleaningString = "cleaning";
    public const string fixString = "fix";
    public const string PesticideString = "Pesticide";
    public const string dumpsterString = "dumpster";
    public const string benchString = "bench";
    public const string treeString = "tree";
    public const string signString = "sign";
    public const string cuttingString = "cutting";
    public const string lightingString = "lighting";
    public const string roundaboutString = "roundabout";
    public const string crossingString = "crosswalk";
    public const string parkString = "Playground";


    public GameObject cleaning;
    public GameObject fix;
    public GameObject Pesticide;
    public GameObject dumpster;
    public GameObject bench;
    public GameObject tree;
    public GameObject sign;
    public GameObject cutting;
    public GameObject lighting;
    public GameObject roundabout;
    public GameObject crossing;
    public GameObject park;




    public GameObject selected;
    private string selectedString = "cleaning";

    *//*[SerializeField] GameObject prefab;
    public OnlineMapsPanoConnector panoConnector;
    private double longToSave;
    private double latToSave;

    //public GameObject DebugText;
    Text debug;
*//*

    public ARAnchorManager anchorManager;
    public ARRaycastManager RaycastManager;

    private AppMode m_AppMode = AppMode.TouchToHostCloudReferencePoint;
    private ARCloudAnchor m_CloudAnchor;
    private string m_CloudAnchorId;

    //my changes
    private Pose poseToSave;
    private GameObject objectTosave;



    public void cleaningChosen()
    {

        selected = cleaning;
        selectedString = cleaningString;
        objectTosave = null;


    }

    public void fixChosen()
    {

        selected = fix;
        selectedString = fixString;
        objectTosave = null;


    }


    public void peststicideChosen()
    {

        selected = Pesticide;
        selectedString = PesticideString;

        objectTosave = null;

    }


    public void dumpsterChosen()
    {

        selected = dumpster;
        selectedString = dumpsterString;
        objectTosave = null;


    }

    public void benchChosen()
    {

        selected = bench;
        selectedString = benchString;
        objectTosave = null;


    }




    public void treeChosen()
    {

        selected = tree;
        selectedString = treeString;
        objectTosave = null;


    }



    public void signChosen()
    {

        selected = sign;
        selectedString = signString;
        objectTosave = null;


    }






    public void pruningChosen()
    {

        selected = cutting;
        selectedString = cuttingString;
        objectTosave = null;


    }




    public void lightingChosen()
    {

        selected = lighting;
        selectedString = lightingString;

        objectTosave = null;

    }


    public void roundaboutChosen()
    {

        selected = roundabout;
        selectedString = roundaboutString;

        objectTosave = null;

    }


    public void crossingChosen()
    {

        selected = crossing;
        selectedString = crossingString;

        objectTosave = null;

    }






    public void parkChosen()
    {

        selected = park;
        selectedString = parkString;
        objectTosave = null;

    }



    public void host()
    {

        ARAnchor anchor =
           anchorManager.AddAnchor(poseToSave);

        // Create Cloud Reference Point.
        m_CloudAnchor =
            anchorManager.HostCloudAnchor(anchor,7);
        if (m_CloudAnchor == null)
        {
            HostDebug.text = "Create Failed!";
            return;
        }

        // Wait for the reference point to be ready.
        m_AppMode = AppMode.WaitingForHostedReferencePoint;



    }

    public void save()
    {

                    OnlineMapsGoogleGeocoding request = new OnlineMapsGoogleGeocoding(GPS.longi, GPS.lati, MyCity.Config.APIKeys.GoogleMapsAPIKey);

        request.Send();
        request.OnComplete += OnRequestComplete;






    }

    private void OnRequestComplete(string obj)
    {


      //  locationInfoOfme data = game.GetComponent<locationInfoOfme>();


        FirebaseController controller = FindObjectOfType<FirebaseController>();

        Feed _feed = new Feed();
       *//* _feed.OwnerID = MyCityMananger.currentID;
        _feed.ToUserID = MyCityMananger.currentID;*//*
        _feed.typeOfPrefab = selectedString;






        //  Debug.Log("loctation got " +s);



        XmlDocument xDoc = new XmlDocument();

        xDoc.LoadXml(obj);

        Debug.Log("success");

        XmlNodeList name = xDoc.GetElementsByTagName("formatted_address");

        Debug.Log(name[1].InnerText);

        _feed.BodyTXT = "need" + selectedString + " in" + name[0].InnerText + " availabe on AR";

        _feed.DateCreated = DateTime.Now.ToString("g");

     //   _feed.Feedlng = GPS.longi.ToString();
     //   _feed.Feedlat = GPS.lati.ToString();

        

        _feed.locationType = "AR";
        _feed.typeOfPrefab = selectedString;

        _feed.cloudID = m_CloudAnchorId;


        StartCoroutine(addPostFromAR(_feed, controller));









    }

    
        private IEnumerator addPostFromAR(Feed feed, FirebaseController controller)
        {

            FeedUploadCallback _feedCallback = null;
            controller.AddNewPost(feed, (callback) => {



                _feedCallback = callback;

            });


            while (_feedCallback == null)
            {
                yield return null;
            }

            if (!_feedCallback.IsSuccess)
            {
                Debug.Log("problem uploading to feed");
                yield break;
            }


            else
            {
                Debug.Log("success uploading to feed from maps");

            }


        
    }

    private void Update()
    {



        if (m_AppMode == AppMode.TouchToHostCloudReferencePoint)
        {
            HostDebug.text = m_AppMode.ToString();

            if (Input.touchCount >= 1
                && Input.GetTouch(0).phase == TouchPhase.Began
                && !EventSystem.current.IsPointerOverGameObject(
                        Input.GetTouch(0).fingerId)&& createdHasPressed)
            {
                List<ARRaycastHit> hitResults = new List<ARRaycastHit>();
                RaycastManager.Raycast(Input.GetTouch(0).position, hitResults);
                if (hitResults.Count > 0)
                {


                    poseToSave = hitResults[0].pose;
                    if (objectTosave == null)
                    {

                        objectTosave = Instantiate(selected, poseToSave.position, poseToSave.rotation);


                    }
                    else
                    {


                        objectTosave.transform.position = poseToSave.position;



                    }








                }
            }
        }

        else if (m_AppMode == AppMode.WaitingForHostedReferencePoint)
        {
            HostDebug.text = m_AppMode.ToString();

            CloudAnchorState cloudReferenceState =
                m_CloudAnchor.cloudAnchorState;
            HostDebug.text += " - " + cloudReferenceState.ToString();

            if (cloudReferenceState == CloudAnchorState.Success)
            {
                *//*  GameObject cloudAnchor = Instantiate(
                                               HostedPointPrefab,
                                               Vector3.zero,
                                               Quaternion.identity);
                  cloudAnchor.transform.SetParent(
                      m_CloudAnchor.transform, false);
  *//*
                m_CloudAnchorId = m_CloudAnchor.cloudAnchorId;
                m_CloudAnchor = null;

                //m_AppMode = AppMode.TouchToResolveCloudReferencePoint;
                cloudID.text = m_CloudAnchorId;
                HostDebug.text = "ready to save";

                m_AppMode = AppMode.TouchToHostCloudReferencePoint;

            }
        }


        *//* else if (m_AppMode == AppMode.TouchToResolveCloudReferencePoint)
         {
             debug.text = m_CloudAnchorId;

             if (Input.touchCount >= 1
                 && Input.GetTouch(0).phase == TouchPhase.Began
                 && !EventSystem.current.IsPointerOverGameObject(
                         Input.GetTouch(0).fingerId))
             {
                 m_CloudAnchor =
                     anchorManager.ResolveCloudAnchorId(
                         m_CloudAnchorId);
                 if (m_CloudAnchor == null)
                 {
                     debug.text = "Resolve Failed!";
                     m_CloudAnchorId = string.Empty;
                     m_AppMode = AppMode.TouchToHostCloudReferencePoint;
                     return;
                 }

                 m_CloudAnchorId = string.Empty;

                 // Wait for the reference point to be ready.
                 m_AppMode = AppMode.WaitingForResolvedReferencePoint;
             }
         }
 */

        /*else if (m_AppMode == AppMode.WaitingForResolvedReferencePoint)
        {
            debug.text = m_AppMode.ToString();

            CloudAnchorState cloudReferenceState =
                m_CloudAnchor.cloudAnchorState;
            debug.text += " - " + cloudReferenceState.ToString();

            if (cloudReferenceState == CloudAnchorState.Success)
            {
                GameObject cloudAnchor = Instantiate(
                                             ResolvedPointPrefab,
                                             Vector3.zero,
                                             Quaternion.identity);
                cloudAnchor.transform.SetParent(
                    m_CloudAnchor.transform, false);

                m_CloudAnchor = null;

                m_AppMode = AppMode.TouchToHostCloudReferencePoint;
            }
        }*//*








    }





























    private void Start()
    {

        Screen.autorotateToLandscapeLeft = false;
        Screen.autorotateToLandscapeRight = false;

        Screen.autorotateToPortrait = true;

        Screen.autorotateToPortraitUpsideDown = true;

        Screen.orientation = ScreenOrientation.AutoRotation;





        // game = null;
        //locationInfo[] biso = game.gameObject.GetComponents<locationInfo>();
        try
        {

            loadFromDataBase();

        }
       
        catch(Exception e)
        {

            resolveDebug.text = e.ToString();

        }
    }

    private void loadFromDataBase()
    {

        System.Uri uri = new System.Uri("https://accelerator-67cff.firebaseio.com/");

        FirebaseApp.DefaultInstance.Options.DatabaseUrl = uri;

        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;



        FirebaseDatabase.DefaultInstance
      .GetReference(AppSettings.AllPostsKey)
      .GetValueAsync().ContinueWith(task =>
      {
          if (task.IsFaulted)
          {
              Debug.Log("error in get");


          }
          else if (task.IsCompleted)
          {

              Debug.Log("success bubble");
              DataSnapshot MainSnapshot = task.Result;
              Debug.Log(MainSnapshot.ChildrenCount);

              foreach (DataSnapshot snapshot in MainSnapshot.Children)
              {



                  feeds.Add(JsonUtility.FromJson<Feed>(snapshot.GetRawJsonValue()));

                  resolveDebug.text = "number of feeds" + feeds.Count;

              }


              try
              {
                  Resolve();

              }
              catch (Exception e)
              {
                  resolveDebug.text = e.ToString();
              }


          }
      });


        


        Debug.Log("after first for loop");

       

    }

    public void Resolve()
    {
        resolveDebug.text = "before try";

        try
        {


            resolveDebug.text = "after try";


            if (feeds != null)
            {
                resolveDebug.text = "after feeds";
                foreach (Feed feed in feeds)
                {
                    resolveDebug.text = "after foreach";
               
                    
                    *//*UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
    *//*

                    if (feed.locationType.Equals("AR"))
                    {


                        resolveDebug.text = feed.cloudID;

                        ARCloudAnchor resolve_CloudAnchor;


                        resolve_CloudAnchor =
                                 anchorManager.ResolveCloudAnchorId(feed.cloudID);




                        if (resolve_CloudAnchor == null)
                        {
                            resolveDebug.text = "Resolve Failed!";
                            m_AppMode = AppMode.TouchToHostCloudReferencePoint;
                            return;
                        }

                        while (true)
                        {

                            CloudAnchorState cloudReferenceState =
         resolve_CloudAnchor.cloudAnchorState;
                            resolveDebug.text = " - " + cloudReferenceState.ToString();

                            if (cloudReferenceState == CloudAnchorState.Success)
                            {
                                resolveDebug.text = "success";


                                switch (feed.typeOfPrefab)
                                {



                                    case cleaningString:
                                        game = Instantiate(
                                                             cleaning,
                                                             Vector3.zero,
                                                             Quaternion.identity);
                                        break;
                                    case fixString:
                                        game = Instantiate(
                                                              fix,
                                                              Vector3.zero,
                                                              Quaternion.identity);
                                        break;


                                    case PesticideString:
                                        game = Instantiate(
                                                                Pesticide,
                                                                Vector3.zero,
                                                                Quaternion.identity);
                                        break;


                                    case dumpsterString:
                                        game = Instantiate(
                                                             dumpster,
                                                             Vector3.zero,
                                                             Quaternion.identity);
                                        break;


                                    case benchString:
                                        game = Instantiate(
                                                              bench,
                                                              Vector3.zero,
                                                              Quaternion.identity);
                                        break;


                                    case treeString:
                                        game = Instantiate(
                                                              tree,
                                                              Vector3.zero,
                                                              Quaternion.identity);
                                        break;


                                    case signString:
                                        game = Instantiate(
                                                              sign,
                                                              Vector3.zero,
                                                              Quaternion.identity);
                                        break;


                                    case cuttingString:
                                        game = Instantiate(
                                                             cutting,
                                                             Vector3.zero,
                                                             Quaternion.identity);
                                        break;


                                    case lightingString:
                                        game = Instantiate(
                                                             lighting,
                                                             Vector3.zero,
                                                             Quaternion.identity);
                                        break;


                                    case roundaboutString:
                                        game = Instantiate(
                                                             roundabout,
                                                             Vector3.zero,
                                                             Quaternion.identity);
                                        break;


                                    case crossingString:
                                        game = Instantiate(
                                                             crossing,
                                                             Vector3.zero,
                                                             Quaternion.identity);
                                        break;


                                    case parkString:
                                        game = Instantiate(
                                                             park,
                                                             Vector3.zero,
                                                             Quaternion.identity);
                                        break;





                                }

                                game.transform.SetParent(
                                    m_CloudAnchor.transform, false);

                                m_CloudAnchor = null;

                                //m_AppMode = AppMode.TouchToHostCloudReferencePoint;



                                break;
                            }




                        }


                    }

                }



            }


        }

        catch(Exception e)
        {
            resolveDebug.text = e.ToString();

        }
        
        
       
    }






    public void create()
    {


        GameObject game1 = selected;

        createdHasPressed = true;


        *//*
                locationInfoOfme data = game1.AddComponent<locationInfoOfme>();
                    data.longtitude = GPS.longi;
                    data.latitude = GPS.lati;
                    data.ownerID = MyCityMananger.CurrentID;
                data.typeOfPrefabcompo = selectedString;    

                var loc = new Location()
                {
                    Latitude = GPS.lati,
                    Longitude = GPS.longi,
                    Altitude = -2,
                    AltitudeMode = AltitudeMode.DeviceRelative
                    };


                    var opts = new PlaceAtLocation.PlaceAtOptions()
                    {
                        HideObjectUntilItIsPlaced = true,
                        MaxNumberOfLocationUpdates = 2,
                        MovementSmoothing = 0.1f,
                        UseMovingAverage = false
                    };

                    PlaceAtLocation.CreatePlacedInstance(game1, loc, opts, false);
        *//*





    }
















}


        // Start is called before the first frame update
















    





*/