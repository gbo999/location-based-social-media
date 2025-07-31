/*
===================================================================
Unity Assets by MAKAKA GAMES: https://makaka.org/o/all-unity-assets
===================================================================

Online Docs (Latest): https://makaka.org/unity-assets
Offline Docs: You have a PDF file in the package folder.

=======
SUPPORT
=======

First of all, read the docs. If it didn’t help, get the support.

Web: https://makaka.org/support
Email: info@makaka.org

If you find a bug or you can’t use the asset as you need, 
please first send email to info@makaka.org
before leaving a review to the asset store.

I am here to help you and to improve my products for the best.
*/

using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[HelpURL("https://makaka.org/unity-assets")]
public class ARTrackedImageManagerControl : MonoBehaviour
{
    [SerializeField]
    private ARTrackedImageManager arTrackedImageManager = null;

    [Space]

    [SerializeField]
    private bool isDeviceAwakeKeepingOnTracking = true;

    [Header("Content Showing & Hiding on Tracked Images Changed")]
    [SerializeField]
    private bool areRenderersEnabledAndDisabled = true;

    [SerializeField]
    private bool areCanvasGroupsShowedAndHidden = true;

    /// <summary>
    /// Flag to Improve Performance
    /// </summary>
    private bool isContentShowing = false;

    private void OnEnable()
    {
        arTrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    private void OnDisable()
    {
        arTrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (ARTrackedImage arTrackedImage in args.added)
        {
            print("arTrackedImage added: " + arTrackedImage.name);

            ShowContent(arTrackedImage.gameObject, true);
        }

        foreach (ARTrackedImage arTrackedImage in args.updated)
        {
            if (arTrackedImage.trackingState == TrackingState.Tracking)
            {
                //print("arTrackedImage updated (TrackingState =! None): "
                //    + arTrackedImage.name);

                ShowContent(arTrackedImage.gameObject, true);
            }
            else
            {
                //print("arTrackedImage updated (TrackingState != None): "
                //    + arTrackedImage.name);

                ShowContent(arTrackedImage.gameObject, false);
            }
        }

        foreach (ARTrackedImage arTrackedImage in args.removed)
        {
            print("arTrackedImage removed: " + arTrackedImage.name);

            ShowContent(arTrackedImage.gameObject, false);
        }
    }

    private void ShowContent(GameObject gameObj, bool isShowing)
    {
        if (isContentShowing != isShowing)
        {
            if (isDeviceAwakeKeepingOnTracking)
            {
                Screen.sleepTimeout = isShowing
                    ? SleepTimeout.NeverSleep
                    : SleepTimeout.SystemSetting;
            }

            print("Show Content Now: " + isShowing);

            isContentShowing = isShowing;

            if (areCanvasGroupsShowedAndHidden)
            {
                SetCanvasGroupsShowed(gameObj, isShowing);
            }

            if (areRenderersEnabledAndDisabled)
            {
                SetRenderersEnabled(gameObj, isShowing);
            }
        }
    }

    private void SetCanvasGroupsShowed(GameObject gameObj, bool isShowed)
    {
        CanvasGroup[] canvasGroups =
            gameObj.GetComponentsInChildren<CanvasGroup>();

        foreach (CanvasGroup childCanvasGroup in canvasGroups)
        {
            childCanvasGroup.alpha = isShowed ? 1f : 0f;
            childCanvasGroup.blocksRaycasts = isShowed;
        }
    }

    private void SetRenderersEnabled(GameObject gameObj, bool isEnabled)
    {
        MeshRenderer[] renderers =
            gameObj.GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer childRenderer in renderers)
        {
            childRenderer.enabled = isEnabled;
        }
    }
}
