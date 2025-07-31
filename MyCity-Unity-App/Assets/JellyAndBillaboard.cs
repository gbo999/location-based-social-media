using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellyAndBillaboard : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject prefab;

    public Texture2D g;

    public bool isCreated = false;
    
    void Start()
    {
        /*  JellyMesh jel = prefab.transform.gameObject.AddComponent<JellyMesh>();

           jel.m_Mass = 0.1f;
           jel.m_MassStyle = JellyMesh.MassStyle.Global;
           jel.m_Stiffness = 50f;
           jel.m_Style = JellyMesh.PhysicsStyle.Free;
   */

        //prefab.GetComponent<JellyMesh>().m_Style = JellyMesh.PhysicsStyle.Free;


        OnlineMaps map = OnlineMaps.instance;
        OnlineMapsControlBase.instance.OnMapClick += OnMapClick;
        //   marker = OnlineMapsMarkerManager.CreateItem(map.position);

    }

    private void OnMapClick()
    {

       // Debug.Log("map clicked");

        if (isCreated == false)
        {
            isCreated = true;
            double lng, lat;

            try
            {


                OnlineMapsControlBase.instance.GetCoords(out lng, out lat);

                OnlineMapsMarker3D onlineMapsMarkertoput = OnlineMapsMarker3DManager.CreateItem(lng, lat, prefab);


                onlineMapsMarkertoput.sizeType = OnlineMapsMarker3D.SizeType.scene;
                onlineMapsMarkertoput.scale = 14;
                onlineMapsMarkertoput.rotation = Quaternion.Euler(85.803f, 0, -180.957f);

                //onlineMapsMarkertoput.transform.gameObject.transform.position =new Vector3(onlineMapsMarkertoput.transform.gameObject.transform.position.x, 49, onlineMapsMarkertoput.transform.gameObject.transform.position.z);
                
                
                // onlineMapsMarkertoput.relativePosition = new Vector3(49); ;

                /*  onlineMapsMarkertoput.scale =50;
                    onlineMapsMarkertoput.rotation = Quaternion.Euler(188.267f, -451.641f, -216.17f);*/

            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());

            }

            // onlineMapsMarkertoput.rotation = Quaternion.Euler(188.267f, -451.641f, -216.17f);



        }



        /* try
         {
             JellyMesh jel = onlineMapsMarkertoput.transform.gameObject.AddComponent<JellyMesh>();

             jel.m_Mass = 0.1f;
             jel.m_MassStyle = JellyMesh.MassStyle.Global;
             jel.m_Stiffness = 50f;
             jel.m_Style = JellyMesh.PhysicsStyle.Free;
         }

         catch (Exception e)
         {
             Debug.Log(e.ToString());

         }*/




    }

    // Update is called once per frame
    void Update()
    {
    }
}
