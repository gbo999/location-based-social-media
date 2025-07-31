using ChartAndGraph;
using Firebase;
using Firebase.Database;
using SocialApp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks; 

public class AnalizeScript : MonoBehaviour
{
    // Start is called before the first frame update


    public PieChart pie;

   public  Material materialCleaning;

    public Material fixingMaterial;

    public static List<Feed> feeds = new List<Feed>();

    private double feedNum;

    private double cleaningNum = 0;

    private double benchNum = 0;



    const string cleaningString = "cleaning";
    const string benchString = "bench";




    void Start()
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



              }

              if (feeds != null)
              {
                  feedNum = feeds.Count;

                  Debug.Log("feed Number "+ feedNum);

                  



                  foreach (Feed feed in feeds)
                  {





                          Debug.Log("inside main dispatcher");

                          switch (feed.typeOfPrefab)
                          {

                              case cleaningString:

                                

                                  cleaningNum++;

                                  Debug.Log("cleaning part" + cleaningNum);

                                  break;

                              case benchString:
                                  benchNum++;

                                  break;



                          }



                   
              }



                  Debug.Log("cleaning number " + cleaningNum);

                  Debug.Log("bench number " + benchNum);

                  Debug.Log("feed Number second time " + feedNum);

                  pie.DataSource.AddCategory("cleaning", materialCleaning);

                  pie.DataSource.SetValue("cleaning", (cleaningNum / feedNum) * 100);




                  pie.DataSource.AddCategory("bench", fixingMaterial);

                  pie.DataSource.SetValue("bench", (benchNum / feedNum) * 100);







              }
          } });














    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
