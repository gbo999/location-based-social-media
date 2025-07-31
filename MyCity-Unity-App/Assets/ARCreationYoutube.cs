using DigitalRubyShared;
using Lean.Common;
using Lean.Touch;
using Input = ARFoundationRemote.Input;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.Events;
using SocialApp;
using TouchPhase = UnityEngine.TouchPhase;


public class ARCreationYoutube : MonoBehaviour
{

    public GameObject arObjectToSpawn;
    public GameObject placementIndicator;
    //public FingersPanARComponentScript s;
    private GameObject spawnedObject;
    private Pose PlacementPose;
    private ARRaycastManager aRRaycastManager;
    private bool placementPoseIsValid = false;
    public Camera cam;

    public GameObject mainCamera;

    private bool isSaved=false;
    //public LeanFingerUp g;



    void Start()
    {
      

        aRRaycastManager = FindObjectOfType<ARRaycastManager>();
        placementIndicator.SetActive(true);

             //arObjectToSpawn = AppManager.myCityController.ModelAsGameObject;

        mainCamera.SetActive(false);
    }

    // need to update placement indicator, placement pose and spawn 
    void Update()
    {
        if (placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Debug.Log("in touch");
            
            
            ARPlaceObject();
        }

       
            UpdatePlacementPose();
        

        UpdatePlacementIndicator();







    }
    void UpdatePlacementIndicator()
    {
        
           
            placementIndicator.transform.SetPositionAndRotation(PlacementPose.position, PlacementPose.rotation);
        
        
    }

    void UpdatePlacementPose()
    {
        var screenCenter = cam.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        aRRaycastManager.Raycast(screenCenter, hits, TrackableType.Planes);

        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid)
        {
            PlacementPose = hits[0].pose;
        }
    }

    public void Save()
    {

        isSaved=true;
    }

    void ARPlaceObject()
    {
        if (!isSaved)
        {
            Debug.Log("in creation AR");
            
            
            spawnedObject = Instantiate(arObjectToSpawn, PlacementPose.position, new Quaternion(0.0f,0.5f,0.0f,0.9f));

            //spawnedObject.SetActive(true);

            LeanSelectableByFinger l = spawnedObject.AddComponent<LeanSelectableByFinger>();

            spawnedObject.tag = "spawned";

        

            spawnedObject.AddComponent<LeanTwistRotateAxis>();
            spawnedObject.AddComponent<LeanPinchScale>();
            LeanDragTranslate d= spawnedObject.AddComponent<LeanDragTranslate>();

            d.Camera = cam;

            isSaved = true;

            //s.Targets.Add(spawnedObject.transform);

        }

       


    }

    

   
}


