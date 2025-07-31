using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoftBody2D;
public class tryagain : MonoBehaviour
{



    /*  void OnJellyCollisionEnter(JellySprite.JellyCollision collision)
      {




              Debug.Log(collision.ToString()+" clicked");


      }*/




    // Update is called once per frame
    void Update()
    {
        /* Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
         RaycastHit hit;

         if (Physics.Raycast(ray, out hit, 10000) && Input.GetMouseButtonDown(0))
         {





             Debug.Log(hit.collider.gameObject);





         }
 */

        RaycastHit2D hit2D = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Camera.main.transform.forward * 1000);

        if (hit2D.collider != null && Input.GetMouseButtonDown(0))
        {
           // Debug.Log(hit2D.collider.gameObject);
            //Do something...




            Debug.Log(" the position " + hit2D.collider.gameObject.transform.position.ToString());


            hit2D.collider.gameObject.transform.position = this.gameObject.transform.position;


        }




    }






}





/*  public void resize()
  {




    gameObject.GetComponent<JellySprite>().Scale(f);


  }*/




