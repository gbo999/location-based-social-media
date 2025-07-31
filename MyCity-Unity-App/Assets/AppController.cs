/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Google.XR.ARCoreExtensions;

using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
public class AppController : MonoBehaviour
{



    public GameObject HostedPointPrefab;
    public GameObject ResolvedPointPrefab;
    public ARAnchorManager anchorManager;
    public ARRaycastManager RaycastManager;
    public InputField InputField;
    public Text OutputText;

    private AppMode m_AppMode = AppMode.TouchToHostCloudReferencePoint;
    private ARCloudAnchor m_CloudAnchor;
    private string m_CloudAnchorId;


   //my changes
    private Pose poseToSave;
    private GameObject objectTosave;




    public enum AppMode
    {
        // Wait for user to tap screen to begin hosting a point.
        TouchToHostCloudReferencePoint,

        // Poll hosted point state until it is ready to use.
        WaitingForHostedReferencePoint,

        // Wait for user to tap screen to begin resolving the point.
        TouchToResolveCloudReferencePoint,

        // Poll resolving point state until it is ready to use.
        WaitingForResolvedReferencePoint,
    }


    // Start is called before the first frame update
    void Start()
    {
       // InputField.onEndEdit.AddListener(OnInputEndEdit);
    }




    public void pressToResolve()
    {
       // m_CloudAnchorId = string.Empty;

        m_CloudAnchor = anchorManager.ResolveCloudAnchorId(m_CloudAnchorId);

        if (m_CloudAnchor == null)
        {
            OutputText.text = "Resolve Failed!";
            m_AppMode = AppMode.TouchToHostCloudReferencePoint;
            return;
        }

        // Wait for the reference point to be ready.
        m_AppMode = AppMode.WaitingForResolvedReferencePoint;
    }





    public void pressToHost()
    {



        // Create a reference point at the touch.
        ARAnchor anchor =
            anchorManager.AddAnchor(poseToSave);

        // Create Cloud Reference Point.
        m_CloudAnchor =
            anchorManager.HostCloudAnchor(anchor,7);
        if (m_CloudAnchor == null)
        {
            OutputText.text = "Create Failed!";
            return;
        }

        // Wait for the reference point to be ready.
        m_AppMode = AppMode.WaitingForHostedReferencePoint;



    }














    // Update is called once per frame
    void Update()
    {
        if (m_AppMode == AppMode.TouchToHostCloudReferencePoint)
        {
            OutputText.text = m_AppMode.ToString();

            if (Input.touchCount >= 1
                && Input.GetTouch(0).phase == TouchPhase.Began
                && !EventSystem.current.IsPointerOverGameObject(
                        Input.GetTouch(0).fingerId))
            {
                List<ARRaycastHit> hitResults = new List<ARRaycastHit>();
                RaycastManager.Raycast(Input.GetTouch(0).position, hitResults);
                if (hitResults.Count > 0)
                {


                     poseToSave = hitResults[0].pose;
                    if (objectTosave == null)
                    {

                        objectTosave = Instantiate(HostedPointPrefab, poseToSave.position, poseToSave.rotation);

                        
                    }
                    else
                    {


                        objectTosave.transform.position = poseToSave.position;



                    }
               
                
                
                
                
                
                
                
                }
            }
        }

        else if (m_AppMode == AppMode.WaitingForHostedReferencePoint)
        {
            OutputText.text = m_AppMode.ToString();

            CloudAnchorState cloudReferenceState =
                m_CloudAnchor.cloudAnchorState;
            OutputText.text += " - " + cloudReferenceState.ToString();

            if (cloudReferenceState == CloudAnchorState.Success)
            {
              *//*  GameObject cloudAnchor = Instantiate(
                                             HostedPointPrefab,
                                             Vector3.zero,
                                             Quaternion.identity);
                cloudAnchor.transform.SetParent(
                    m_CloudAnchor.transform, false);
*//*
                m_CloudAnchorId = m_CloudAnchor.cloudAnchorId;
                m_CloudAnchor = null;

                m_AppMode = AppMode.TouchToResolveCloudReferencePoint;
                OutputText.text = m_CloudAnchorId;

            }
        }


       *//* else if (m_AppMode == AppMode.TouchToResolveCloudReferencePoint)
        {
            OutputText.text = m_CloudAnchorId;

            if (Input.touchCount >= 1
                && Input.GetTouch(0).phase == TouchPhase.Began
                && !EventSystem.current.IsPointerOverGameObject(
                        Input.GetTouch(0).fingerId))
            {
                m_CloudAnchor =
                    anchorManager.ResolveCloudAnchorId(
                        m_CloudAnchorId);
                if (m_CloudAnchor == null)
                {
                    OutputText.text = "Resolve Failed!";
                    m_CloudAnchorId = string.Empty;
                    m_AppMode = AppMode.TouchToHostCloudReferencePoint;
                    return;
                }

                m_CloudAnchorId = string.Empty;

                // Wait for the reference point to be ready.
                m_AppMode = AppMode.WaitingForResolvedReferencePoint;
            }
        }
*//*

        else if (m_AppMode == AppMode.WaitingForResolvedReferencePoint)
        {
            OutputText.text = m_AppMode.ToString();

            CloudAnchorState cloudReferenceState =
                m_CloudAnchor.cloudAnchorState;
            OutputText.text += " - " + cloudReferenceState.ToString();

            if (cloudReferenceState == CloudAnchorState.Success)
            {
                GameObject cloudAnchor = Instantiate(
                                             ResolvedPointPrefab,
                                             Vector3.zero,
                                             Quaternion.identity);
                cloudAnchor.transform.SetParent(
                    m_CloudAnchor.transform, false);

                m_CloudAnchor = null;

                m_AppMode = AppMode.TouchToHostCloudReferencePoint;
            }
        }








    }
}*/