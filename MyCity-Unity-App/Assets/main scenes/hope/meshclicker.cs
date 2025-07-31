using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class meshclicker : MonoBehaviour
{


    JellySprite jel;


    // Start is called before the first frame update
    void Start()
    {

        //  jel = GetComponent<JellySprite>();


        //jel.Reset(transform.position,transform.position); 












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

            if (hit.collider.gameObject.GetType() == typeof(JellyMeshReferencePoint))
            {

                hit.collider.gameObject.GetComponent<JellyMeshReferencePoint>().transform.position = hit.collider.gameObject.GetComponent<JellyMeshReferencePoint>().ParentJellyMesh.GetComponent<JellyMesh>().CentralPoint.transform.position;

                // e.transform.parent.position = cam.transform.position + cam.transform.forward * distance;
            }


           // gameObject.GetComponent<JellyMesh>().Scale()



























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
