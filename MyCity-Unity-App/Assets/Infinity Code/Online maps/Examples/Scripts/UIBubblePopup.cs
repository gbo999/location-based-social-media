/*/*         INFINITY CODE         #1#
/*   https://infinity-code.com   #1#

using com.draconianmarshmallows.geofire;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using SocialApp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Video;
using System.Linq;
using InfinityCode.uPano;
using InfinityCode.uPano.HotSpots;
using InfinityCode.uPano.InteractiveElements;
using System.Globalization;


namespace InfinityCode.OnlineMapsDemos  
{
    /// <summary>
    /// Example is how to use a combination of data from Google Places API on bubble popup.
    /// </summary>
    [AddComponentMenu("Infinity Code/Online Maps/Demos/UIBubblePopup")]
    public class UIBubblePopup : MonoBehaviour
    {
        #region Fields

        #region Pins

        [SerializeField] private GameObject SharePrefab;

        [SerializeField] private GameObject EventPrefab;

        [SerializeField] private GameObject SalePrefab;

        [SerializeField] private GameObject PollPrefab;

        public GameObject prefab;

        #endregion

        #region Popups

        public UIElementsGroup bubble;

        public UIElementsGroup infoWindow;

        public UIElementsGroup queryMenu;

        [SerializeField] private UIElementsGroup Comments;
        
        [SerializeField] private GameObject CommentsObject = default;

                #endregion

        #region Database

        private DatabaseReference DRPostLikesCount;

        private bool CanLikePost;

        private bool IsPostLiked;

        private DatabaseReference OptionOneCount;

        private DatabaseReference OptiontwoCount;

        List<GeoQuery> geoquries = new List<GeoQuery>();

        DatabaseReference reference;

        List<Feed> feedOfQuery = new List<Feed>();

        bool isVoted;

        private string feedkey;

        #endregion

        #region View

        public TMP_Text CommentsCountBody;

        
        [SerializeField] private Color LikedPostColor = default;

        [SerializeField] private Color UnLikedPostColor = default;

        [SerializeField] private Image LikeImage = default;

        public Image MapAvatar;

        [SerializeField] private MessagesDataLoader CommentLoader;

        public RectTransform MapAvatarRect;

        public TMP_Text LikesCountBody;

        public Camera cam;

        public TMP_Text optionOneCount;

        public TMP_Text optionTwoCount;

        public TMP_Text optionOneText;

        public TMP_Text optionTwoText;

        public VideoPlayer VPlayer;

        public RawImage VideoBody;

        public GameObject loading;

        [SerializeField] private Image ImageBody;

        public Image imageOptionOne;

        public Image imageOptionTwo;

        public ToggleGroup RadioGrop;

        public Button vote;

        public TMP_Text textOne;

        public TMP_Text texttwo;

        [SerializeField] private float AvatarSize = default;
        [SerializeField] private float DefaultAvatarSize = default;
        [SerializeField] private RectTransform AvatarRect = default;

        public Texture2D musicTex;
        public Texture2D enrichTex;
        public Texture2D religionTex;
        public Texture2D sportsTex;

        public Toggle music;

        public Toggle Religion;

        public Toggle sports;

        public Toggle Enrcihment;

        public Slider radiusValue;

        /// </summary>
        public TMP_Text userName;

        /// <summary>
        /// Address text
        /// </summary>
        public TMP_Text address;

        public TMP_Text dateCreated;
        public TMP_Text details;

        public float MapAvatarSize;

        public float mapAvatarDefaultSize;
        public Texture2D _defaultMapAvatar;

        public Image AvatarImage;

        public TMP_Text PriceOrDAteText;

        #endregion

        #region Map

        private OnlineMapsMarker targetMarker;

        private OnlineMapsMarker3D target3D;

        public Vector2 lastPosition;

        OnlineMapsPanoConnector panoConnector;

        double lastTX, lastTY;

        [SerializeField] private float howMuchToLess;
        [SerializeField] private float howMuchMore;

        [SerializeField] private float howmuchz;


        public float theX = 130f;
        public float theY = 130f;

        public float distanceFromCamera = 1f;


        #endregion

        #endregion

        

        [SerializeField] private GameObject search = default;

        public Text CommentsCountBodyMuniciplaity;

        /// <summary>
        /// Root canvas
        /// </summary>
        public Canvas canvas;

        /// <summary>
        /// Title text
        /// <summary>
        /// AvatarImage RawImage
        /// </summary>
        public CData[] datas;


        public GameObject cleaningpano;


        /*        public Texture2D Foodtag;
                public Texture2D arttag;
                public Texture2D religiontag;
                public Texture2D sporttag;
                public Texture2D musictag;
                public Texture2D objtag;
                public Texture2D partytag;
                public Texture2D gametag;
                public Texture2D networkingtag;
                public Texture2D othertag;

                public Texture2D hometag;
                public Texture2D carSales;
                public Texture2D elecsales;
                public Texture2D kitchSales;
                public Texture2D gardSales;
                public Texture2D hobSales;
                public Texture2D kidsSales;
                public Texture2D compSales;
                public Texture2D realSales;

                public Texture2D clothingSales;
        #1#


        #region Methods

        #region Database+view

        public void clickCommnent()
        {
            Comments.ChangeVisibility(true);
            CommentLoader.LoadPostComments(feedkey);
        }


        public void clickVote()
        {
            Toggle tog;


            string kind = RadioGrop.ActiveToggles().First().name;


            AppManager.FIREBASE_CONTROLLER.Vote(feedkey, kind, success =>
            {
                if (success)
                {
                    loadPoll(feedkey);
                }
            });
        }

        public void makeQuery()
        {
            foreach (GeoQuery geo in geoquries)
            {
                geo.reset();
            }


            OnlineMapsMarker3DManager.RemoveAllItems();


            float radius = radiusValue.value;


            if (music.isOn)
            {
                MakeTagQuery(AppSettings.MusicTag, radius);
            }


            if (Religion.isOn)
            {
                MakeTagQuery(AppSettings.religiontag, radius);
            }

            if (sports.isOn)
            {
                MakeTagQuery(AppSettings.sporttag, radius);
            }


            if (Enrcihment.isOn)
            {
                MakeTagQuery(AppSettings.enrichmentTag, radius);
            }
        }


        public void ShowQuery()
        {
            queryMenu.ChangeVisibility(true);
        }

        public void closeQuery()
        {
            queryMenu.ChangeVisibility(false);
        }


        /*
        public void makeUsualQuery()
        {
            float radius = 30;

            foreach (GeoQuery geo in geoquries)
            {
                geo.reset();
            }

            OnlineMapsMarker3DManager.RemoveAllItems();


            MakeTagQuery(AppSettings.ShareTag, radius);


            MakeTagQuery(AppSettings.PollTag, radius);


            MakeTagQuery(AppSettings.EventTag, radius);


            MakeTagQuery(AppSettings.SaleTag, radius);
        }
        #1#


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

        #endregion

        #endregion


        private Texture2D tagToPrefab(string tag)
        {
            switch (tag)
            {
                case "Music":

                    return musicTex;


                case "Enrichment":
                    return enrichTex;

                case "Religion":
                    return religionTex;
                case "Sports":
                    return sportsTex;


                default:
                    return sportsTex;

                /*case "TagFood":
                    return Foodtag;
        
                case "TagArt":
                    return arttag;
        
                case  "TagReligion":
                    return religiontag;
                case "TagSport":
                    return sporttag;
        
                case "TagMusic":
                    return musictag;
        
        
                case "TagObjectives":
                    return objtag;
                case "TagParties":
                    return partytag;
        
                case "TagGames":
                    return gametag;
        
        
                case "TagNetworking":
                    return networkingtag;
                case "TagOther":
                    return othertag;
        
        
        
                case "SaleHome":
                    return hometag;
                case "SaleCars":
                    return carSales;
        
                case "SaleElectronics":
                    return elecsales;
        
        
                case "SaleKithchen":
                    return kitchSales;
        
                case "SaleGardening":
                    return gardSales;
        
        
                case "SaleHobbies":
                    return hobSales;
                case "Salekids":
                    return kidsSales;
        
        
                case "SaleComputers":
                    return compSales;
                case "SaleRealEstate":
                    return realSales;
        
        
        
                case "SaleClothing":
                    return clothingSales;
                default:
                    return othertag;
        #1#
            }
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

            //      yield return new WaitForSeconds(0.75f);


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

                        feedOfQuery.Add(_dataFeed);


                        Debug.Log("datafeed" + _dataFeed.Feedlat);


                        try
                        {
                            UnityMainThreadDispatcher.Instance().Enqueue(() =>
                            {
                                //  Texture2D textureToPrefab = tagToPrefab(_dataFeed.tag);


                                //  prefab.GetComponentInChildren<SpriteRenderer>().sprite = Sprite.Create(textureToPrefab, new Rect(0.0f, 0.0f, textureToPrefab.width, textureToPrefab.height), new Vector2(0.5f, 0.5f), 100.0f);
                                //ResizeAvarar(AvatarSize);


                                /*
                                OnlineMapsMarker3D onlineMapsMarkertoput = OnlineMapsMarker3DManager.CreateItem(_dataFeed.Feedlng, _dataFeed.Feedlat, prefab);

                                onlineMapsMarkertoput.sizeType = OnlineMapsMarker3D.SizeType.scene;
                                onlineMapsMarkertoput.scale =2f;
                               // onlineMapsMarkertoput.rotation = Quaternion.Euler(85.803f, 0, -180.957f);
                               onlineMapsMarkertoput.rotation=Quaternion.identity;
                              
                               
                               
                               
                               
                               
                               
                                onlineMapsMarkertoput["data"] = _dataFeed;
                                onlineMapsMarkertoput.OnClick += OnMarkerClick;
                                #1#


                                if (_dataFeed.tag == AppSettings.ShareTag)
                                {
                                    prefab = SharePrefab;
                                }


                                else if (_dataFeed.tag == AppSettings.SaleTag)
                                {
                                    prefab = SalePrefab;
                                }

                                else if (_dataFeed.tag == AppSettings.EventTag)
                                {
                                    prefab = EventPrefab;
                                }

                                else if (_dataFeed.tag == AppSettings.PollTag)
                                {
                                    prefab = PollPrefab;
                                }

                                else
                                {
                                    prefab = SharePrefab;
                                }

                                GetProfileImageRequest _request = new GetProfileImageRequest();
                                _request.Id = _dataFeed.OwnerID;
                                _request.Size = ImageSize.Size_512;
                                AppManager.FIREBASE_CONTROLLER.GetProfileImageForMarker(_request, _dataFeed, prefab,
                                    OnProfileImageMarkerGetted);

                                //  onlineMapsMarkertoput.OnRollOver += OnMarkerClick;


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

        public void CreateMarker(string arg1)
        {
            Debug.LogError("creating again");
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

                        feedOfQuery.Add(_dataFeed);


                        Debug.Log("datafeed" + _dataFeed.Feedlat);


                        try
                        {
                            UnityMainThreadDispatcher.Instance().Enqueue(() =>
                            {
                                //  Texture2D textureToPrefab = tagToPrefab(_dataFeed.tag);


                                //  prefab.GetComponent<SpriteRenderer>().sprite = Sprite.Create(textureToPrefab, new Rect(0.0f, 0.0f, textureToPrefab.width, textureToPrefab.height), new Vector2(0.5f, 0.5f), 100.0f);
                                //ResizeAvarar(AvatarSize);


                                GetProfileImageRequest _request = new GetProfileImageRequest();
                                _request.Id = _dataFeed.OwnerID;
                                _request.Size = ImageSize.Size_512;
                                AppManager.FIREBASE_CONTROLLER.GetProfileImageForMarker(_request, _dataFeed, prefab,
                                    OnProfileImageMarkerGetted);

                                /*
                                OnlineMapsMarker3D onlineMapsMarkertoput = OnlineMapsMarker3DManager.CreateItem(_dataFeed.Feedlng, _dataFeed.Feedlat, prefab);

                                onlineMapsMarkertoput.sizeType = OnlineMapsMarker3D.SizeType.scene;
                                onlineMapsMarkertoput.scale = 14;
                                onlineMapsMarkertoput.rotation = Quaternion.Euler(85.803f, 0, -180.957f);
                                onlineMapsMarkertoput["data"] = _dataFeed;
                                onlineMapsMarkertoput.OnClick += OnMarkerClick;
                                //  onlineMapsMarkertoput.OnRollOver += OnMarkerClick;
                              


                                Debug.Log("is created");#1#
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



        public GameObject markerToCheck;



        /// <summary>
        /// Bubble popup
        /// </summary>
        //   public GameObject bubble;

        /*  public GameObject municipalityBubble;
  
          public GameObject restpollBubble;
  
          public GameObject fixBubble;#1#

        //public GameObject parkPollBubble;


        // public GameObject defaultBubble;


        /// <summary>
        /// This method is called when downloading AvatarImage texture.
        /// </summary>
        /// <param name="texture2D">AvatarImage texture</param>
        /*   private void OnDownloadAvatarImageComplete(OnlineMapsWWW www)
           {
               Texture2D texture = new Texture2D(1, 1);
               www.LoadImageIntoTexture(texture);

               // Set place texture to bubble popup
               AvatarImage.texture = texture;
           }#1#

        /// <summary>
        /// This method is called by clicking on the map
        /// </summary>
        private void OnMapClick()
        {
            // Remove active marker reference
            targetMarker = null;
            target3D = null;

            // Hide the popup
            bubble.ChangeVisibility(false);
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
            }#1#

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
                    Texture2D _texture = ((DownloadHandlerTexture) www.downloadHandler).texture;
                    if (_texture != null)
                    {
                        ImageBody.sprite = Sprite.Create(_texture,
                            new Rect(0.0f, 0.0f, _texture.width, _texture.height), new Vector2(0.5f, 0.5f), 100.0f);
                    }
                }
            }
        }


        /// <summary>
        /// This method is called by clicking on the marker
        /// </summary>
        /// <param name="marker">The marker on which clicked</param>
        private void OnMarkerClick(OnlineMapsMarkerBase marker)
        {
            // Set active marker reference
            target3D = marker as OnlineMapsMarker3D;

            Feed feed = marker["data"] as Feed;

            feedkey = feed.Key;


            DRPostLikesCount = AppManager.FIREBASE_CONTROLLER.GetPostLikesCountReferense(feedkey);
            DRPostLikesCount.ValueChanged += OnLikesCountUpdated;

            LoadLikes();
            LoadComments();

            if (feed.Type == FeedType.Poll)
            {
                loadPoll(feedkey);


                OptionOneCount = AppManager.FIREBASE_CONTROLLER.GetsCountOptionOneReferense(feedkey);
                OptionOneCount.ValueChanged += OnOptionOneCountUpdated;


                OptiontwoCount = AppManager.FIREBASE_CONTROLLER.GetsCountOptionTwoReferense(feedkey);
                OptiontwoCount.ValueChanged += OnOptionTwoCountUpdated;
            }


            AppManager.FIREBASE_CONTROLLER.GetUserFullName(feed.OwnerID, (_userName => { userName.text = _userName; }));

            address.text = feed.address;

            dateCreated.text = feed.DateCreated;
            details.text = feed.BodyTXT;


            if (feed.kind == "Sale")
            {
                PriceOrDAteText.text = "price: " + feed.price.ToString() + " $";
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


            /*      // Get a result item from instance of the marker
                  CData data = marker["data"] as CData;
                  if (data == null) return;
      #1#


            // Show the popup


            /*
                        OnlineMaps.instance.position = target3D.position;


                        Vector2 vec = OnlineMapsControlBase.instance.GetScreenPosition(OnlineMaps.instance.position);


                        vec.y = vec.y -howMuchMore;

                        OnlineMaps.instance.position = OnlineMapsControlBase.instance.GetCoords(vec);


                       #1#


            UpdateBubblePosition();


            Vector2 screenPosition = OnlineMapsControlBase.instance.GetScreenPosition(target3D.position);
            RectTransform rectTransform = bubble.GetComponent<RectTransform>();
            screenPosition.y -= rectTransform.sizeDelta.y * rectTransform.lossyScale.y * howMuchMore;
            OnlineMaps.instance.position = OnlineMapsControlBase.instance.GetCoords(screenPosition);

            //*


            bubble.ChangeVisibility(true);


            // OnlineMaps.instance.position = new Vector2(target3D.position.x, target3D.position.y +howMuchMoreFromZoom(OnlineMaps.instance.zoom));


            //   LoadLikes();
            //    LoadComments();


            // Set title and address
            /*     title.text = data.title;
                 address.text = data.address;
     #1#
            /* // Destroy the previous AvatarImage
             if (AvatarImage.texture != null)
             {
                 OnlineMapsUtils.Destroy(AvatarImage.texture);
                 AvatarImage.texture = null;
             }

             OnlineMapsWWW www = new OnlineMapsWWW(data.AvatarImage_url);
             www.OnComplete += OnDownloadAvatarImageComplete;
 #1#
            // Initial update position
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
                    Texture2D _texture = ((DownloadHandlerTexture) www.downloadHandler).texture;
                    if (_texture != null)
                    {
                        imageOptionOne.sprite = Sprite.Create(_texture,
                            new Rect(0.0f, 0.0f, _texture.width, _texture.height), new Vector2(0.5f, 0.5f), 100.0f);
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
                    Texture2D _texture = ((DownloadHandlerTexture) www.downloadHandler).texture;
                    if (_texture != null)
                    {
                        imageOptionTwo.sprite = Sprite.Create(_texture,
                            new Rect(0.0f, 0.0f, _texture.width, _texture.height), new Vector2(0.5f, 0.5f), 100.0f);
                    }
                }
            }
        }


        private float howMuchMoreFromZoom(float zoom)
        {
            switch (zoom)
            {
                case 11:
                    return 0.1f;
                    break;
                case 12:
                    return 0.05f;
                    break;

                case 13:
                    return 0.025f;
                    break;
                case 14:
                    return 0.015f;
                    break;

                case 15:
                    return 0.008f;
                    break;
                case 16:
                    return 0.003f;
                    break;

                default:
                    return 0;
            }
        }


        public void MapPostion()
        {
            Debug.Log(OnlineMaps.instance.position.y);
        }


        public void OnProfileImageGetted(GetProfileImageCallback _callback)
        {
            if (_callback.IsSuccess)
            {
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(_callback.ImageBytes);
                AvatarImage.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f), 100.0f);

                ResizeAvarar(AvatarSize);
            }
            else
            {
                DisplayDefaultAvatar();
            }
        }

        public void OnProfileImageMarkerGetted(GetProfileImageCallback _callback)
        {
            try
            {
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(_callback.ImageBytes);

                Debug.Log("length is" + _callback.ImageBytes.Length);

                SpriteRenderer sp = _callback.pref.GetComponentsInChildren<SpriteRenderer>()[1];


                sp.sprite = Sprite.Create(texture,
                    new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f),
                    100.0f);

                _callback.pref.GetComponentsInChildren<SpriteRenderer>()[1].transform.localScale =
                    new Vector3(0.114355631f, 0.114355631f, 0.114355631f);


                Feed _dataFeed = _callback.feed;

                OnlineMapsMarker3D onlineMapsMarkertoput =
                    OnlineMapsMarker3DManager.CreateItem(_dataFeed.Feedlng, _dataFeed.Feedlat, _callback.pref);

                onlineMapsMarkertoput.sizeType = OnlineMapsMarker3D.SizeType.scene;
                onlineMapsMarkertoput.scale = 2f;
                // onlineMapsMarkertoput.rotation = Quaternion.Euler(85.803f, 0, -180.957f);
                onlineMapsMarkertoput.rotation = Quaternion.identity;


                onlineMapsMarkertoput["data"] = _dataFeed;
                onlineMapsMarkertoput.OnClick += OnMarkerClick;


                Debug.Log("Ttttttttttttttttttttttttttttttttttttttttttttt");
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }

        public void OnMapAvatar(GetProfileImageCallback _callback)
        {
            AppManager.VIEW_CONTROLLER.HideLoading();


            if (_callback.IsSuccess)
            {
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(_callback.ImageBytes);
                MapAvatar.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f), 100.0f);


                //  ResizeAvararMap(MapAvatarSize);
            }
            else
            {
                DisplayDefaultMapAvatar();
            }
        }


        private void DisplayDefaultMapAvatar()
        {
            if (AppManager.APP_SETTINGS != null)
            {
                MapAvatar.sprite = Sprite.Create(_defaultMapAvatar,
                    new Rect(0.0f, 0.0f, _defaultMapAvatar.width, _defaultMapAvatar.height), new Vector2(0.5f, 0.5f),
                    100.0f);
                ResizeAvarar(mapAvatarDefaultSize);
            }
        }


        private void DisplayDefaultAvatar()
        {
            if (AppManager.APP_SETTINGS != null)
            {
                Texture2D _defaultAvatar = AppManager.APP_SETTINGS.DefaultAvatarTexture;
                AvatarImage.sprite = Sprite.Create(_defaultAvatar,
                    new Rect(0.0f, 0.0f, _defaultAvatar.width, _defaultAvatar.height), new Vector2(0.5f, 0.5f), 100.0f);
                ResizeAvarar(DefaultAvatarSize);
            }
        }


        private void ResizeAvarar(float _size)
        {
            float _bodyWidth = _size;
            float _bodyHeight = _size;
            float _imageWidth = (float) AvatarImage.sprite.texture.width;
            float _imageHeight = (float) AvatarImage.sprite.texture.height;
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


        private void ResizeAvararMap(float _size)
        {
            float _bodyWidth = _size;
            float _bodyHeight = _size;
            float _imageWidth = (float) AvatarImage.sprite.texture.width;
            float _imageHeight = (float) AvatarImage.sprite.texture.height;
            float _ratio = _imageWidth / _imageHeight;
            if (_imageWidth > _imageHeight)
            {
                _ratio = _imageHeight / _imageWidth;
            }

            float _expectedHeight = _bodyWidth / _ratio;
            if (_imageWidth > _imageHeight)
            {
                MapAvatarRect.sizeDelta = new Vector2(_expectedHeight, _bodyHeight);
            }
            else
            {
                MapAvatarRect.sizeDelta = new Vector2(_bodyWidth, _expectedHeight);
            }
        }


        /// <summary>
        /// Start is called on the frame when a script is enabled just before any of the Update methods are called the first time.
        /// </summary>
        private void Start()
        {
            reference = AppManager.FIREBASE_CONTROLLER.GETDataBase().RootReference;


            UploadMapAvatar();

            panoConnector = GetComponent<OnlineMapsPanoConnector>();


            panoConnector.OnLoaded += LoadHotSpots;

            OnlineMaps.instance.OnChangePosition += PositionQuery;
            lastPosition = OnlineMaps.instance.position;

            OnlineMaps.instance.OnChangePosition += UpdateBubblePosition;
            OnlineMaps.instance.OnChangeZoom += UpdateBubblePosition;
            OnlineMapsControlBase.instance.OnMapClick += OnMapClick;


            OnlineMaps.instance.GetTilePosition(out lastTX, out lastTY);


            if (OnlineMapsControlBaseDynamicMesh.instance != null)
            {
                OnlineMapsControlBaseDynamicMesh.instance.OnMeshUpdated += UpdateBubblePosition;
            }

            if (OnlineMapsCameraOrbit.instance != null)
            {
                OnlineMapsCameraOrbit.instance.OnCameraControl += UpdateBubblePosition;
            }


            bubble.ChangeVisibility(false);
            
            
            makeUsualQuery();




            //  Uri uri = new Uri(firebaseDbUrl);

            //FirebaseApp.DefaultInstance.Options.DatabaseUrl = uri;


            Debug.Log("the ref is " + reference.ToString());
        }

        private void PositionQuery()
        {
            /*
            double tx, ty;
            OnlineMaps.instance.GetTilePosition(out tx, out ty);
            const float threshold = 8;
            if ((tx - lastTX) * (tx - lastTX) + (ty - lastTY) * (ty - lastTY) < threshold * threshold)
            {
                lastTX = tx;
                lastTY = ty;
                makeUsualQuery();
                
            }
            #1#


            if (OnlineMapsUtils.DistanceBetweenPointsD(
                new Vector2(OnlineMaps.instance.position.x, OnlineMaps.instance.position.y),
                new Vector2(lastPosition.x, lastPosition.y)) > 20)
            {
                makeUsualQuery();
                lastPosition = OnlineMaps.instance.position;
            }
        }

        public void UploadMapAvatar()
        {
            if (AppManager.Instance.user != null)
            {
                GetProfileImageRequest _request = new GetProfileImageRequest();


                _request.Id = AppManager.Instance.auth.CurrentUser.UserId;
                _request.Size = ImageSize.Size_512;
                AppManager.FIREBASE_CONTROLLER.GetProfileImage(_request, OnMapAvatar);
            }
        }

        private void LoadHotSpots(Pano obj)
        {
            cam = obj.activeCamera;

            HotSpotManager manager = obj.GetComponent<HotSpotManager>();
            if (manager == null) manager = obj.gameObject.AddComponent<HotSpotManager>();
            manager.Clear();


            for (int i = 0; i < feedOfQuery.Count; i++)
            {
                Debug.Log("feed is " + feedOfQuery.ElementAt(i).panoramaID);

                Debug.Log("pano id is " + panoConnector.meta.id);


                if (feedOfQuery.ElementAt(i).panoramaID.Equals(panoConnector.meta.id))
                {
                    HotSpot hotSpot = manager.Create((float) Convert.ToDouble(feedOfQuery.ElementAt(i).panStreetview),
                        (float) Convert.ToDouble(feedOfQuery.ElementAt(i).tiltStreetview), cleaningpano);

                    //   hotSpot.scale = new Vector3(0.008678579f, 0.008678579f, 0.008678579f);
                    //   hotSpot.distanceMultiplier -= 0.13f;
                    //  Vector4 vec = EulerToQuaternion(new Vector3(-109.407f, -155.85f, 149.303f));
                    // hotSpot.rotation = new Quaternion(vec.x, vec.y, vec.z, vec.w);

                    hotSpot.scale = new Vector3(-0.023141f, -0.023141f, -0.023141f);
                    hotSpot.distanceMultiplier -= 0.09f;
                    hotSpot["feed"] = feedOfQuery.ElementAt(i);
                    hotSpot.OnClick.AddListener(SomeMethod);
                }
            }
        }

        private void SomeMethod(InteractiveElement arg0)
        {
            HotSpot hot = arg0 as HotSpot;

            Feed feed = hot["feed"] as Feed;


            feedkey = feed.Key;


            if (feed.Type == FeedType.Poll)
            {
                loadPoll(feedkey);


                OptionOneCount = AppManager.FIREBASE_CONTROLLER.GetsCountOptionOneReferense(feedkey);
                OptionOneCount.ValueChanged += OnOptionOneCountUpdated;


                OptiontwoCount = AppManager.FIREBASE_CONTROLLER.GetsCountOptionTwoReferense(feedkey);
                OptiontwoCount.ValueChanged += OnOptionTwoCountUpdated;
            }


            AppManager.FIREBASE_CONTROLLER.GetUserFullName(feed.OwnerID, (_userName => { userName.text = _userName; }));

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

            infoWindow.GetComponent<RectTransform>().localScale = new Vector3(0.5f, 0.5f, 0.5f);
            infoWindow.transform.position = cam.ViewportToWorldPoint(new Vector3(theX, theY, distanceFromCamera));

            infoWindow.ChangeVisibility(true);
        }


        //  bubble = pollBubble;


        // Subscribe to events of the map 


        /*  OnlineMapsMarker3D marker = OnlineMapsMarker3DManager.CreateItem(34.7864459913435, 31.8116051562021,markerToCheck);

          if (markerToCheck.GetComponent<MyCityMarker>().markerTexture.width == 32)
          {

              marker.sizeType = OnlineMapsMarker3D.SizeType.scene;
              marker.scale = 100;
              marker.rotation = Quaternion.Euler(85.803f, 0, 0);
          }

          else 
          {

              marker.sizeType = OnlineMapsMarker3D.SizeType.scene;
              marker.scale = 7;
              marker.rotation = Quaternion.Euler(85.803f, 0, 0);
          }#1#


        /* if (datas != null)
         {
             foreach (CData data in datas)
             {
                 OnlineMapsMarker marker = OnlineMapsMarkerManager.CreateItem(data.longitude, data.latitude);
                 marker["data"] = data;
                 marker.OnClick += OnMarkerClick;
             }
         }#1# /*


        // Initial hide popup
        bubble.SetActive(false);


       // loadFromDataBase();


    }#1#

        /// <summary>
        /// Updates the popup position
        /// </summary>
        private void UpdateBubblePosition()
        {
            // If no marker is selected then exit.
            if (target3D == null) return;

            // Hide the popup if the marker is outside the map view
            if (!target3D.inMapView)
            {
                if (bubble.Visible) bubble.gameObject.SetActive(false);
            }
            else if (!bubble.Visible) bubble.gameObject.SetActive(true);


            Vector3 pos = OnlineMapsTileSetControl.instance.GetWorldPosition(target3D.position) +
                          new Vector3(0, howMuchToLess, howmuchz);
            Vector2 screenPosition = OnlineMapsTileSetControl.instance.activeCamera.WorldToScreenPoint(pos);


            Vector2 point;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, screenPosition,
                null, out point);

            (bubble.transform as RectTransform).localPosition = point;

            (bubble.transform.GetChild(0) as RectTransform).localPosition = point;
        }

        private void UpdateBubblePosition3D()
        {
            // If no marker is selected then exit.
            if (target3D == null) return;


            // Hide the popup if the marker is outside the map view
            if (!target3D.inMapView)
            {
                if (bubble.Visible) bubble.ChangeVisibility(false);
            }
            else if (!bubble.Visible) bubble.ChangeVisibility(true);

            // Convert the coordinates of the marker to the screen position.
            Vector2 screenPosition = OnlineMapsControlBase.instance.GetScreenPosition(target3D.position);

            // Add marker height
            //   screenPosition.y += target3D.transform.localScale.y;

            // Get a local position inside the canvas.
            Vector2 point;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, screenPosition,
                null, out point);

            // Set local position of the popup
            (bubble.transform as RectTransform).localPosition = point;


            (bubble.transform.GetChild(0) as RectTransform).localPosition = point;
        }


        [Serializable]
        public class CData
        {
            public string title;
            public string address;
            public string AvatarImage_url;
            public double longitude;
            public double latitude;
        }


        private void onMarkerRollOut(OnlineMapsMarkerBase obj)
        {
            target3D = obj as OnlineMapsMarker3D;


            Debug.Log("this is last print: " + target3D.prefab.gameObject.transform.GetChild(0).ToString());


            foreach (Transform child in target3D.transform)
            {
                if (child.tag == "effect")
                {
                    child.gameObject.SetActive(false);
                }
            }
        }

        /*      private void OnMarker3DClick(OnlineMapsMarkerBase obj)
              {
      
      
                  target3D = obj as OnlineMapsMarker3D;
      
      
                  Debug.Log("this is last print: "+target3D.prefab.gameObject.transform.GetChild(0).ToString());
      
      #1# /*
            foreach (Transform child in target3D.transform)
            {

                if (child.tag == "effect")
                {
                    child.gameObject.SetActive(true);

                }

            }#1#


        /*GameObject gameObject = target3D.prefab.gameObject.transform.GetChild(0).gameObject;
        gameObject.SetActive(true);

        Debug.Log("is active: " + gameObject.activeInHierarchy);#1#


        /* 
         Debug.Log("this is the children gameObject: "+target3D.prefab.GetComponentInChildren<ParticleSystem>(true).ToString());


         GameObject#1# /* gameObject = target3D.prefab.transform.GetChild(0).gameObject;

            Debug.Log("this is the gameobject tostring: "+ gameObject.ToString());
#1# /*


            gameObject.SetActive(true);

#1#
        /* var main=  target3D.prefab.GetComponentInChildren<ParticleSystem>(true).main.loop;

        main = true;
#1# /*

            FirebaseController controller = FindObjectOfType<FirebaseController>();

            Feed feed = obj["feed"] as Feed;

            feedkey = feed.Key;

            DRPostLikesCount = AppManager.FIREBASE_CONTROLLER.GetPostLikesCountReferense(feedkey);
            DRPostLikesCount.ValueChanged += OnLikesCountUpdated;


            controller.GetUserFullName(feed.OwnerID, (_userName =>
            {


                title.text = "created by " + _userName;
                address.text = feed.DateCreated + "\n" + feed.BodyTXT;

            }));

            GetProfileImageRequest _request = new GetProfileImageRequest();
            _request.Id = feed.OwnerID;
            _request.Size = ImageSize.Size_512;
            AppManager.FIREBASE_CONTROLLER.GetProfileImage(_request, OnProfileImageGetted);




            #1# /*      // Get a result item from instance of the marker
                  CData data = marker["data"] as CData;
                  if (data == null) return;
      #1# /*
            // Show the popup

            
           





            LoadLikes();
            LoadComments();




            // Set title and address
            #1# /*     title.text = data.title;
                 address.text = data.address;
     #1#
        /* // Destroy the previous AvatarImage
         if (AvatarImage.texture != null)
         {
             OnlineMapsUtils.Destroy(AvatarImage.texture);
             AvatarImage.texture = null;
         }

         OnlineMapsWWW www = new OnlineMapsWWW(data.AvatarImage_url);
         www.OnComplete += OnDownloadAvatarImageComplete;
#1# /*
            // Initial update position
            UpdateBubblePosition3D();

            bubble.ChangeVisibility(true);



        }#1#

        public void ShowPostComments(string _id)
        {
            search.SetActive(false);
            bubble.ChangeVisibility(false);
            LoadComments();
            CommentsObject.SetActive(true);
            CommentsObject.GetComponentInChildren<MessagesDataLoader>().LoadPostComments(feedkey);
        }

        public void HidePostComments()
        {
            search.SetActive(true);
            bubble.ChangeVisibility(true);
            CommentsObject.SetActive(false);
        }

        private void LoadComments()
        {
            AppManager.FIREBASE_CONTROLLER.GetPostCommentsCount(feedkey,
                _count => { CommentsCountBody.text = _count.ToString(); });


            /*
            AppManager.FIREBASE_CONTROLLER.GetPostCommentsCount(feedkey, _count =>
            {
                CommentsCountBodyMuniciplaity.text = _count.ToString();
            });
            #1#
        }


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
                //  Debug.Log(e.ToString());
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


        private void LoadLikes()
        {
            CanLikePost = false;

            AppManager.FIREBASE_CONTROLLER.IsLikedPost(feedkey, _isLike =>
            {
                CanLikePost = true;
                IsPostLiked = _isLike;
                if (IsPostLiked)
                {
                    LikeImage.color = LikedPostColor;
                }
                else
                {
                    LikeImage.color = UnLikedPostColor;
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


/* in process
        public void changeToInProcess()



        {

            Feed feed = targetMarker["feed"] as Feed;

            Debug.Log("before create cleaning");
            OnlineMapsMarker onlineMapsMarkertoput = OnlineMapsMarkerManager.CreateItem(Convert.ToDouble(feed.Feedlng), Convert.ToDouble(feed.Feedlat), cleaningInprocess, "created by ");
            onlineMapsMarkertoput["feed"] = targetMarker["feed"];
            onlineMapsMarkertoput.OnClick += OnMarkerClick;


            targetMarker.DestroyInstance();


            AppManager.FIREBASE_CONTROLLER.changeStatusToInProcess(feedkey);
           





        }





        public void changeToFixed()



        {

            Feed feed = targetMarker["feed"] as Feed;

            Debug.Log("before create cleaning");
            OnlineMapsMarker onlineMapsMarkertoput = OnlineMapsMarkerManager.CreateItem(Convert.ToDouble(feed.Feedlng), Convert.ToDouble(feed.Feedlat), cleaningFixed, "created by ");
            onlineMapsMarkertoput["feed"] = targetMarker["feed"];
            onlineMapsMarkertoput.OnClick += OnMarkerClick;


            targetMarker.DestroyInstance();


            AppManager.FIREBASE_CONTROLLER.changeStatusToInProcess(feedkey);






        }#1#
    }
}*/