using DigitalRubyShared;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCheck : MonoBehaviour
{
    private RotateGestureRecognizer rotateGesture;
    public GameObject prefab;



    // Start is called before the first frame update
    void Start()
    {
        rotateGesture = new RotateGestureRecognizer();
        rotateGesture.StateUpdated += RotateGestureCallback;
        FingersScript.Instance.AddGesture(rotateGesture);

    }

    private void RotateGestureCallback(GestureRecognizer gesture)
    {
        if(gesture.State == GestureRecognizerState.Executing)
        {

            prefab.transform.Rotate(rotateGesture.RotationRadiansDelta * Mathf.Rad2Deg, 0.0f, 0.0f);


        }




    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
