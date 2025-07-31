using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleMoveTest : MonoBehaviour
{
    // Start is called before the first frame update





    private List<Vector2> points;

    public GameObject confettivfx;


    int count = 0;

    void Start()
    {
        /*
                line.Reset();*/

        points = new List<Vector2>();

        OnlineMapsControlBase.instance.OnMapClick += OnMapClick;



    }

    private void OnMapClick()
    {

        


/*
        double lng, lat;
        OnlineMapsControlBase.instance.GetCoords(out lng, out lat);

      //  Debug.Log("lng " + lng + " lat " + lat);

        Vector3 v = OnlineMapsTileSetControl.instance.GetWorldPosition(lng, lat);

        


        points.Add(new Vector2((float)lng, (float)lat));

        if (points.Count == 4)
        {

            OnlineMapsDrawingLine p = new OnlineMapsDrawingLine(points,  Color.green, 20);

            


            OnlineMapsDrawingElementManager.AddItem(p);




        }
*/


    }

    void spawn(Vector3 v)
    {

        GameObject spawnedVFX = Instantiate(confettivfx, v, transform.rotation) as GameObject;
        spawnedVFX.transform.localScale=new Vector3(50, 50, 50);
        
        Destroy(spawnedVFX, 50f);

    }




        // Update is called once per frame
        void Update()
    {
        
    }
}
