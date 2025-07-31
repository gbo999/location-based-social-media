using ARLocation;
using com.draconianmarshmallows.geofire;
using Firebase;
using Firebase.Database;
using SocialApp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;
using Input = ARFoundationRemote.Input;


public class ARLoader : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    private float AvatarSize = default;
    [SerializeField]
    private float DefaultAvatarSize = default;
    [SerializeField]
    private RectTransform AvatarRect = default;

    private string feedkey;

    [SerializeField] private string firebaseDbUrl;
    DatabaseReference reference;

    public Camera cam;
    public GameObject prefab;
    private GameObject game;

    public UIElementsGroup e;
    public float distance = 0.5f;


    private DatabaseReference OptionOneCount;
    private DatabaseReference OptiontwoCount;

    public TMP_Text optionOneCount;

    public TMP_Text optionTwoCount;


    public TMP_Text optionOneText;

    public TMP_Text optionTwoText;

   // public UIElementsGroup bubble;


    public VideoPlayer VPlayer;
    public RawImage VideoBody;

    public GameObject loading;

    [SerializeField]
    private Image ImageBody;

    public TMP_Text userName;

    /// <summary>
    /// Address text
    /// </summary>
    public TMP_Text address;
    public TMP_Text dateCreated;
    public TMP_Text details;
    /// <summary>
    /// AvatarImage RawImage
    /// </summary>
    public Image AvatarImage;

    public Image imageOptionOne;

    public Image imageOptionTwo;

    public ToggleGroup RadioGrop;

    public Button vote;


    public Text textOne;

    public Text texttwo;

    bool isVoted;


    public TMP_Text PriceOrDAteText;



    void Start()
    {

        Uri uri = new Uri(firebaseDbUrl);

        FirebaseApp.DefaultInstance.Options.DatabaseUrl = uri;

        reference = FirebaseDatabase.DefaultInstance.RootReference;
    }


    public void WatchMycityCons()
    {


        Debug.Log("tags is " + AppSettings.tags);

        Debug.Log("current tag " + AppManager.myCityController.currentTag);
        Debug.Log("lat is " + AppManager.myCityController.lat);
        Debug.Log("lng is " + AppManager.myCityController.lng);
        Debug.Log("radius is " + AppManager.myCityController.Radius);


        Debug.Log("the date now is " + DateTime.Now.ToString("dd-MM-yyyy"));


        if (reference == null)
        {


            Debug.Log("ref is null " + reference.ToString());


        }
        else
        {

            Debug.Log("ref is not null " + reference.ToString());

        }


        var geofireDbReference = reference.Child(AppSettings.tags).Child(AppManager.myCityController.currentTag).Child(DateTime.Now.ToString("dd-MM-yyyy"));
        var geoFire = new GeoFire(geofireDbReference);


        GeoQuery query = geoFire.queryAtLocation(new GeoLocation(AppManager.myCityController.lat, AppManager.myCityController.lng), AppManager.myCityController.Radius);


        query.geoQueryReadyListeners += onGeoQueryReady;
        query.geoQueryErrorListeners += onGeoQueryError;
        query.keyEnteredListeners += onKeyEntered;
        query.keyExitedListeners += onKeyExited;
        query.keyMovedListeners += onKeyMoved;



        query.initializeListeners();

    }






    private void onKeyMoved(string arg1, GeoLocation arg2)
    {
        Debug.Log("moved");
    }

    private void onKeyExited(string obj)
    {
        Debug.Log("exit");
    }

    private void onKeyEntered(string arg1, GeoLocation arg2)
    {

        Debug.Log("enetered " + arg1);




        DatabaseReference _feedRef = reference.Child(AppSettings.AllPostsKey).Child(arg1);
        _feedRef.GetValueAsync().ContinueWith(task =>
        {

            if (task.IsFaulted)
            {

                Debug.Log("not success on key entered");
                CleanTask(task);
            }


            else
            {

                Debug.Log("something please");

                string feed = task.Result.Key;
                string jsonFeed = task.Result.GetRawJsonValue();


                if (!string.IsNullOrEmpty(jsonFeed))
                {
                    Feed _dataFeed = JsonUtility.FromJson<Feed>(jsonFeed);

                   

                    try
                    {
                        UnityMainThreadDispatcher.Instance().Enqueue(() =>
                        {




                            var loc = new Location()
                            {


                                Latitude = _dataFeed.Feedlat,
                                Longitude = _dataFeed.Feedlng,

                                Altitude = 0.5,
                                AltitudeMode = AltitudeMode.DeviceRelative
                            };





                            var opts = new PlaceAtLocation.PlaceAtOptions()
                            {
                                HideObjectUntilItIsPlaced = true,
                                MaxNumberOfLocationUpdates = 2,
                                MovementSmoothing = 0.1f,
                                UseMovingAverage = false
                            };
                            game = PlaceAtLocation.CreatePlacedInstance(prefab, loc, opts, false);

                            game.tag = "ARObject";

                            ARComponentObject comp = game.AddComponent<ARComponentObject>();

                            comp["feed"] = _dataFeed;


                            Debug.Log("is created");
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

    private void onGeoQueryError(DatabaseError obj)
    {
        Debug.Log("Error in " + AppManager.myCityController.currentTag);
    }

    private void onGeoQueryReady()
    {
        Debug.Log(AppManager.myCityController.currentTag);
    }


    // Update is called once per frame
    void Update()
    {





        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Update the Text on the screen depending on current position of the touch each frame


            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = cam.ScreenPointToRay(touch.position);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 10000))
                {

                    if (hit.collider.gameObject.tag == "ARObject")
                    {



                        DealWithGame(hit.collider.gameObject);


                       // e.transform.parent.position = cam.transform.position + cam.transform.forward * distance;








                    }

                  //  Debug.Log(hit.collider.gameObject.ToString());





                }








            }









        }

















    }

    private void DealWithGame(GameObject gameObject)
    {

        ARComponentObject comp = gameObject.GetComponent<ARComponentObject>();

        Feed feed = comp["feed"] as Feed;


        Debug.Log("latitue is "+ feed.Feedlat);



        feedkey = feed.Key;


        if (feed.Type == FeedType.Poll)
        {

            loadPoll(feedkey);


            OptionOneCount = AppManager.FIREBASE_CONTROLLER.GetsCountOptionOneReferense(feedkey);
            OptionOneCount.ValueChanged += OnOptionOneCountUpdated;


            OptiontwoCount = AppManager.FIREBASE_CONTROLLER.GetsCountOptionTwoReferense(feedkey);
            OptiontwoCount.ValueChanged += OnOptionTwoCountUpdated;


        }













        AppManager.FIREBASE_CONTROLLER.GetUserFullName(feed.OwnerID, (_userName =>
        {


            userName.text = _userName;





        }));

        address.text = feed.address;

        dateCreated.text = feed.DateCreated;
        details.text = feed.BodyTXT;


        if (feed.kind == "Sale")
        {
            PriceOrDAteText.text = feed.price.ToString();


        }

        else if (feed.kind == "Event")
        {


            PriceOrDAteText.text = feed.startDate;



        }








        GetProfileImageRequest _request = new GetProfileImageRequest();
        _request.Id = feed.OwnerID;
        _request.Size = ImageSize.Size_512;
        AppManager.FIREBASE_CONTROLLER.GetProfileImage(_request, OnProfileImageGetted);

        if (feed.Type == FeedType.Image)
        {
            imageOptionOne.gameObject.SetActive(false);

            imageOptionTwo.gameObject.SetActive(false);

            RadioGrop.gameObject.SetActive(false);

            vote.gameObject.SetActive(false);
            ImageBody.gameObject.SetActive(true);
            VideoBody.gameObject.SetActive(false);

            StartCoroutine(OnLoadGraphic(feed));

            optionOneCount.gameObject.SetActive(false);

            optionTwoCount.gameObject.SetActive(false);

            optionOneText.gameObject.SetActive(false);

            optionTwoText.gameObject.SetActive(false);
        }


        if (feed.Type == FeedType.Video)


        {
            imageOptionOne.gameObject.SetActive(false);

            imageOptionTwo.gameObject.SetActive(false);

            RadioGrop.gameObject.SetActive(false);

            vote.gameObject.SetActive(false);
            ImageBody.gameObject.SetActive(false);
            VideoBody.gameObject.SetActive(true);
            optionOneCount.gameObject.SetActive(false);

            optionTwoCount.gameObject.SetActive(false);

            optionOneText.gameObject.SetActive(false);

            optionTwoText.gameObject.SetActive(false);

            StartCoroutine(OnLoadVideo(feed));





        }

        if (feed.Type == FeedType.Poll)
        {

            ImageBody.gameObject.SetActive(false);
            VideoBody.gameObject.SetActive(false);

            imageOptionOne.gameObject.SetActive(true);

            imageOptionTwo.gameObject.SetActive(true);

            RadioGrop.gameObject.SetActive(true);

            vote.gameObject.SetActive(true);

            textOne.text = feed.TXTOne;
            texttwo.text = feed.TXTTwo;

            optionOneCount.gameObject.SetActive(true);

            optionTwoCount.gameObject.SetActive(true);

            optionOneText.text = feed.TXTOne;
            optionTwoText.text = feed.TXTTwo;


            StartCoroutine(OnLoadGraphicOne(feed));
            StartCoroutine(OnLoadGraphicTwo(feed));





        }











































        e.transform.position = cam.transform.position + cam.transform.forward * distance;

        e.transform.rotation = cam.transform.rotation;

        e.ChangeVisibility(true);





    }


    public void OnProfileImageGetted(GetProfileImageCallback _callback)
    {
        if (_callback.IsSuccess)
        {


            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(_callback.ImageBytes);
            AvatarImage.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);

            ResizeAvarar(AvatarSize);
        }
        else
        {
            DisplayDefaultAvatar();
        }
    }
    private void DisplayDefaultAvatar()
    {
        if (AppManager.APP_SETTINGS != null)
        {
            Texture2D _defaultAvatar = AppManager.APP_SETTINGS.DefaultAvatarTexture;
            AvatarImage.sprite = Sprite.Create(_defaultAvatar, new Rect(0.0f, 0.0f, _defaultAvatar.width, _defaultAvatar.height), new Vector2(0.5f, 0.5f), 100.0f);
            ResizeAvarar(DefaultAvatarSize);
        }

    }





    private void ResizeAvarar(float _size)
    {
        float _bodyWidth = _size;
        float _bodyHeight = _size;
        float _imageWidth = (float)AvatarImage.sprite.texture.width;
        float _imageHeight = (float)AvatarImage.sprite.texture.height;
        float _ratio = _imageWidth / _imageHeight;
        if (_imageWidth > _imageHeight)
        {
            _ratio = _imageHeight / _imageWidth;
        }
        float _expectedHeight = _bodyWidth / _ratio;
        if (_imageWidth > _imageHeight)
        {
            AvatarRect.sizeDelta = new Vector2(_expectedHeight, _bodyHeight);
        }
        else
        {
            AvatarRect.sizeDelta = new Vector2(_bodyWidth, _expectedHeight);
        }
    }
    private void OnOptionTwoCountUpdated(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            optionTwoCount.text = "0";
            return;
        }
        try
        {
            if (args.Snapshot.Value.ToString() == "0")
            {
                optionTwoCount.text = "0";
            }
            else
            {
                optionTwoCount.text = args.Snapshot.Value.ToString();
            }
        }
        catch (Exception e)
        {
            optionTwoCount.text = "0";
            //  Debug.Log(e.ToString());

        }
    }



    private void OnOptionOneCountUpdated(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            optionOneCount.text = "0";
            return;
        }
        try
        {
            if (args.Snapshot.Value.ToString() == "0")
            {
                optionOneCount.text = "0";
            }
            else
            {
                optionOneCount.text = args.Snapshot.Value.ToString();
            }
        }
        catch (Exception e)
        {
            optionOneCount.text = "0";
            //  Debug.Log(e.ToString());

        }
    }


    private void loadPoll(string feedkey)
    {
        RadioGrop.gameObject.SetActive(false);
        vote.gameObject.SetActive(false);
        optionOneText.gameObject.SetActive(true);
        optionTwoText.gameObject.SetActive(true);


        AppManager.FIREBASE_CONTROLLER.IsVoted(feedkey, _isvote =>
        {

            isVoted = _isvote;
            if (isVoted)
            {
                RadioGrop.gameObject.SetActive(false);
                vote.gameObject.SetActive(false);
                optionOneText.gameObject.SetActive(true);
                optionTwoText.gameObject.SetActive(true);
            }
            else
            {

                Debug.Log("is voted =true?");
                optionOneText.gameObject.SetActive(false);
                optionTwoText.gameObject.SetActive(false);

                RadioGrop.gameObject.SetActive(true);
                vote.gameObject.SetActive(true);
            }
        });
    }

    private IEnumerator OnLoadGraphicOne(Feed feed)
    {
        string _url = feed.ImageURLOne;
        if (!string.IsNullOrEmpty(_url))
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(_url);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Texture2D _texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                if (_texture != null)
                {
                    imageOptionOne.sprite = Sprite.Create(_texture, new Rect(0.0f, 0.0f, _texture.width, _texture.height), new Vector2(0.5f, 0.5f), 100.0f);
                }
            }
        }
    }


    private IEnumerator OnLoadGraphicTwo(Feed feed)
    {
        string _url = feed.ImageURLTwo;
        if (!string.IsNullOrEmpty(_url))
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(_url);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Texture2D _texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                if (_texture != null)
                {
                    imageOptionTwo.sprite = Sprite.Create(_texture, new Rect(0.0f, 0.0f, _texture.width, _texture.height), new Vector2(0.5f, 0.5f), 100.0f);
                }
            }
        }
    }



    private IEnumerator OnLoadVideo(Feed feed)
    {

        loading.SetActive(true);


        string _url = feed.VideoURL;
        /*
                    if (!string.IsNullOrEmpty(feed.ImageURL))
                    {
                        UnityWebRequest www = UnityWebRequestTexture.GetTexture(LoadedFeed.ImageURL);
                        yield return www.SendWebRequest();

                        if (www.isNetworkError || www.isHttpError)
                        {
                            //Debug.Log(www.error);
                        }
                        else
                        {
                            Texture2D _texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                            VideoBody.texture = _texture;
                        }
                    }*/

        if (!AppManager.APP_SETTINGS.UseOriginVideoFile)
        {
            _url = string.Empty;
            bool _isGettedUrl = false;
            string _tempUrl = string.Empty;
            AppManager.FIREBASE_CONTROLLER.GetFeedVideoFileUrl(feed.VideoFileName, (_gettedUrl =>
            {
                _isGettedUrl = true;
                _tempUrl = _gettedUrl;
            }));
            while (!_isGettedUrl)
            {
                yield return null;
            }
            _url = _tempUrl;
        }

        if (!string.IsNullOrEmpty(_url))
        {
            VPlayer.url = _url;
            VPlayer.Prepare();
            while (!VPlayer.isPrepared)
            {
                yield return null;
            }

            loading.SetActive(false);
            VideoBody.texture = VPlayer.texture;

            VPlayer.Play();

        }
    }



    private IEnumerator OnLoadGraphic(Feed feed)
    {
        string _url = feed.ImageURL;
        if (!string.IsNullOrEmpty(_url))
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(_url);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Texture2D _texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                if (_texture != null)
                {
                    ImageBody.sprite = Sprite.Create(_texture, new Rect(0.0f, 0.0f, _texture.width, _texture.height), new Vector2(0.5f, 0.5f), 100.0f);
                }
            }
        }
    }













}
