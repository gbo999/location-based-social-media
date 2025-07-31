using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class markersMode : MonoBehaviour
{
    // Start is called before the first frame update
    public static bool markerMode;
    public Text text;

    public void markersMOde()
    {

        markerMode = true;
      //  text.text = "markers mode one";

    }


    public void exitMArkersMode()
    {

        markerMode = false;
       // text.text = "markers mode off";

    }


}
