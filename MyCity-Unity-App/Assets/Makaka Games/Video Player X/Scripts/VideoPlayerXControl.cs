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
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.Events;

using System.Collections;

[HelpURL("https://makaka.org/unity-assets")]
public class VideoPlayerXControl : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    public RawImage rawImage;

    public int waitCyclesForVideoPreparingMax = 500;
    public float waitSecondsToShowRenderTextureAfterPlayStart = 0.1f;

    private int waitCyclesForVideoPreparingCurrent;

    [Header("Controls")]
    public GameObject controls;

    public Toggle togglePlayPause;
    private Graphic graphicPlayTransparentToggle;

    public Slider slider;

    [Space]
    [SerializeField]
    private UnityEvent OnInitialized;

    [Space]
    [SerializeField]
    private UnityEvent OnShowingAndPlaying;

    [Space]
    [SerializeField]
    private UnityEvent OnClosing;

    private void Awake()
    {
        graphicPlayTransparentToggle = togglePlayPause.targetGraphic;
        
        videoPlayer.loopPointReached += OnloopPointReached;

        OnInitialized.Invoke();
    }

    private void OnloopPointReached(VideoPlayer source)
    {
        togglePlayPause.isOn = false;
    }
    
    private void Update()
    {
        if (videoPlayer.isPlaying)
        {
            slider.SetValueWithoutNotify((float) videoPlayer.time);
        }
    }

    private void ShowAndPlayBase()
    {
        OnShowingAndPlaying.Invoke();

        gameObject.SetActive(true);

        controls.SetActive(true);

        togglePlayPause.isOn = true;
    }

    public void ShowAndPlayWithStreamingAssetsPath(string path)
    {
        ShowAndPlayBase();

        Play(path);
    }

    public void ShowAndPlay(VideoClip videoClip)
    {
        ShowAndPlayBase();

        Play(videoClip);
    }

    public void Play(string path)
    {
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = System.IO.Path.Combine(
            Application.streamingAssetsPath,
            path);

        StartCoroutine(PlayBase());
    }

    public void Play(VideoClip videoClip)
    {
        videoPlayer.source = VideoSource.VideoClip;
        videoPlayer.clip = videoClip;

        StartCoroutine(PlayBase());
    }

    private IEnumerator PlayBase()
    {
        videoPlayer.Prepare();

        waitCyclesForVideoPreparingCurrent = 0;

        while (!videoPlayer.isPrepared
            && waitCyclesForVideoPreparingCurrent
                < waitCyclesForVideoPreparingMax)
        {
            waitCyclesForVideoPreparingCurrent++;
            
            //print(waitCyclesForVideoPreparingCurrent);
            
            yield return null;
        }

        slider.maxValue = (float)videoPlayer.length;
        slider.value = 0f;

        //print("VideoPlayer is prepared ot wait time for prepare is over");
        
        videoPlayer.Play();

        yield return new WaitForSeconds(waitSecondsToShowRenderTextureAfterPlayStart);

        SetRawImageColor(Color.white);
    }

    public void PlayPauseToggle(bool isOff)
    {   
        if (graphicPlayTransparentToggle)
        {
            graphicPlayTransparentToggle.enabled = !isOff;

            if (isOff)
            {
                videoPlayer.Play();
            }
            else
            {
                videoPlayer.Pause();
            }
        }
    }

    public void Close()
    {
        OnClosing.Invoke();

        controls.SetActive(false);

        if (gameObject.activeSelf)
        {
            StartCoroutine(CloseCoroutine());
        }
    }

    private IEnumerator CloseCoroutine()
    {
        videoPlayer.Pause();
        
        yield return null;

        slider.value = 0f;

        SetRawImageColor(Color.clear);

        gameObject.SetActive(false);
    }

    private void SetRawImageColor(Color color)
    {
        if (rawImage)
        {
            rawImage.color = color;
        }
    }

    public void OnSliderValueChanged()
    {
        videoPlayer.time = slider.value;
    }
    
    private void OnDestroy()
	{
        if (videoPlayer.targetTexture
            && videoPlayer.renderMode == VideoRenderMode.RenderTexture)
        {
            videoPlayer.targetTexture.Release();
        }
	}
}
