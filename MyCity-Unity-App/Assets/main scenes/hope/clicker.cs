using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class clicker : MonoBehaviour
{


   // JellySprite jel;


    // Start is called before the first frame update
    void Start()
    {

        //  jel = GetComponent<JellySprite>();


        //jel.Reset(transform.position,transform.position); 


        OnlineMapsControlBase.instance.OnMapZoom = resize;

        









    }

    private void resize()
    {

        OnlineMaps map = OnlineMaps.instance;

        double tlx, tly, brx, bry;
        map.GetCorners(out tlx, out tly, out brx, out bry);

       // double ttlx, ttly, tbrx, tbry;
       // map.GetTileCorners(out ttlx, out ttly, out tbrx, out tbry, map.zoom);
        float bestYScale = OnlineMapsElevationManagerBase.GetBestElevationYScale(tlx, tly, brx, bry);



        Debug.Log(bestYScale);

       gameObject.GetComponent<JellySprite>().Scale(bestYScale);

        gameObject.GetComponent<BoxCollider>().size = new Vector3(10f, 10f, 10f);

        gameObject.transform.localScale = new Vector3(10f, 10f, 10f);
    }

    /*  void OnJellyCollisionEnter(JellySprite.JellyCollision collision)
      {




              Debug.Log(collision.ToString()+" clicked");


      }*/




    // Update is called once per frame
    void Update()
    {



        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 10000) && Input.GetMouseButtonDown(0))
        {





            Debug.Log(hit.collider.gameObject);


            if (hit.collider.gameObject.GetType()== typeof( JellySpriteReferencePoint))
            {

                hit.collider.gameObject.GetComponent<JellySpriteReferencePoint>().transform.position = hit.collider.gameObject.GetComponent<JellySpriteReferencePoint>().ParentJellySprite.GetComponent<JellySprite>().CentralPoint.transform.position;

            }



            // e.transform.parent.position = cam.transform.position + cam.transform.forward * distance;































            /*foreach (JellySprite.ReferencePoint refPoint in jel.ReferencePoints)
            {

                refPoint.GameObject.GetComponent<Rigidbody2D>().


                if (refPoint != jel.CentralPoint)
                {
                    Vector2 offset = refPoint.transform.position - jel.CentralPoint.transform.position;
                    float distance = offset.magnitude;
                    float initialDistance = refPoint.InitialOffset.magnitude;

                    // This prevents the physics body from getting too far away from the central point (eg. if it gets stuck behind an object)



                        refPoint.transform.position = (jellySprite.CentralPoint.transform.position) + (Vector3)(offset.normalized * initialDistance);

                }
            }*/
        }

    }






}
