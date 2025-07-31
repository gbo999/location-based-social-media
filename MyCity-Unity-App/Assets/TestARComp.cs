using SocialApp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestARComp : MonoBehaviour
{
    // Start is called before the first frame update

    private ARComponentObject Compo;


    void Start()
    {

        Feed f = new Feed();

        f.Feedlat = 455f;

        Compo = this.gameObject.AddComponent<ARComponentObject>();

        Compo["feed"] = f;


    }


    public void Show()
    {

        
        


        
        Feed feed =Compo["feed"] as Feed;


        Debug.Log("lat is " + feed.Feedlat);


    }


    // Update is called once per frame
    void Update()
    {

     

    }
}
