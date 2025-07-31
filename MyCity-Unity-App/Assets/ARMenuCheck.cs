using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARMenuCheck : MonoBehaviour
{
    // Start is called before the first frame update

    public UIElementsGroup e;

    public Camera cam;

    public float distance;

    public float scale;


    private void Update()
  
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

                    if(hit.collider.gameObject.tag== "spawned")
                    {


                        e.transform.parent.position = cam.transform.position + cam.transform.forward * distance;

                        e.transform.parent.localScale = hit.collider.gameObject.transform.localScale * scale;




                        if (e.Visible == true)
                        {

                            e.ChangeVisibility(false);
                        }

                        else
                        {
                            e.ChangeVisibility(true);

                        }



                    }

                    Debug.Log(hit.collider.gameObject.ToString());





                }

           
            
            
            
            
            
            
            }









        }






        /*
                var screenCenter = cam.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));

                menu.transform.SetPositionAndRotation(s

        */










    }






}
