using System;
using System.Collections.Generic;
using Firebase.Database;
using UnityEngine;
using Yamanas.Scripts.MapLoader.Shop;

namespace SocialApp
{
    public class MyCityManager : MonoBehaviour
    {
        #region Fields

        public string CurrentModelID;
        public string groupPostID = "NoGroup";
        public string currentTag = "TagSport";
        public double Radius = 5;
        public double lat = 31.8116051562021;
        public double lng = 34.7864459913435;
        public GameObject ModelAsGameObject;


        public Dictionary<ProductType, int> productsValue;

        public Dictionary<string, ProductType> ProductTypes;

        public List<string> ProductTypesHold;


        private Query _store;

        #endregion

        #region Methods

        private void Awake()
        {
            productsValue = new Dictionary<ProductType, int>()
            {
                {ProductType.AddOnLike, 30},
            };
            ProductTypes = new Dictionary<string, ProductType>()
            {
                {"DreamCatcher", ProductType.AddOnLike}
            };

        }

        public void LoadProducts()
        {
            AppManager.FIREBASE_CONTROLLER.GetProducts(AppManager.Instance.auth.CurrentUser.UserId,(list =>
                ProductTypesHold = list));
        }

        public void ListenChildAdded()
        {
            _store = AppManager.FIREBASE_CONTROLLER.GetStoreQuery();
            _store.ChildAdded += AddProduct;
        }

        public void AddProduct(object sender, ChildChangedEventArgs args)
        {
            ProductTypesHold.Add((args.Snapshot.Value.ToString()));

            Debug.Log($"prodcus are {ProductTypesHold.ToString()}");
        }

        #endregion


        /* public static string panoramaID="";
        public static float pan;
        public static float tilt;
        public static string currentID;
        public static string fullName;
        public static bool moveToAnotherScene=false;
        public static bool startFromNet;
        public static double lngFromFeed= 34.7869440045761;
        public static double latFromFeed= 31.8103663431251;
        public static bool isStarted = false;

        public static string userId = "jtzBF6qCcjP07xUokN8kwHzVvWh1";
    */


        /* public static string CurrentID { get => currentID; set => currentID = value; }
         public static string FullName { get => fullName; set => fullName = value; }
         public static bool MoveToAnotherScene { get => moveToAnotherScene; set => moveToAnotherScene = value; }
         public static float Pan { get => pan; set => pan = value; }
         public static float Tilt { get => tilt; set => tilt = value; }
         public static string PanoramaID { get => panoramaID; set => panoramaID = value; }
         public static double LngFromFeed { get => lngFromFeed; set => lngFromFeed = value; }
         public static double LatFromFeed { get => latFromFeed; set => latFromFeed = value; }
         public static bool IsStarted { get => isStarted; set => isStarted = value; }*/
    }
}