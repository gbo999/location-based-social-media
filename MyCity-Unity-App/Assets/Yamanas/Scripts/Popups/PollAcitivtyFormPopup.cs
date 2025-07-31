using SocialApp;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Yamanas.Infrastructure.Popups;
using Yamanas.Scripts.MapLoader;

public class PollAcitivtyFormPopup : MonoBehaviour
{
    #region Fields

    [SerializeField] private Image PictureOne;

    [SerializeField] private Image PictureTwo;

    [SerializeField] private Image cameraOne;

    [SerializeField] private Image cameratwo;

    [SerializeField] private TMP_InputField OptionOne;

    [SerializeField] private TMP_InputField OptionTwo;

    [SerializeField] private TMP_InputField Question;

    private string pathOne;

    private string pathTwo;

    #endregion

    #region Methods

    public void OnCloseButtonClick()
    {
        PopupSystem.Instance.CloseAllPopups();
    }

    public void OnPostButtonClick()
    {
        PostProcessController.Instance.OptionOne = OptionOne.text;
        PostProcessController.Instance.Optiontwo = OptionTwo.text;
        PostProcessController.Instance.Question = Question.text;
        PostProcessController.Instance.UploadPoll(pathOne, pathTwo);
    }


    public void SelectPhotoFromGalleryOne()
    {
#if !UNITY_EDITOR && UNITY_ANDROID
            NativeGallery.GetImageFromGallery(OnImagePickedOptionOne);


#else

        OnImagePickedOptionOne("");


#endif
    }

    public void SelectPhotoFromGalleryTwo()
    {
#if !UNITY_EDITOR && UNITY_ANDROID
            NativeGallery.GetImageFromGallery(OnImagePickedOptionTwo);


#else

        OnImagePickedOptionTwo("");


#endif
    }

    public void OnImagePickedOptionOne(string pathFromAndroid)
    {
#if UNITY_EDITOR
        pathOne = EditorUtility.OpenFilePanel("Overwrite with jpg", "", "jpg");

#elif !UNITY_EDITOR && UNITY_ANDROID
            pathOne = pathFromAndroid;
#endif


        if (string.IsNullOrEmpty(pathOne))
            return;

        PostProcessController.Instance.FeedType = FeedType.Poll;
        byte[] fileBytes = System.IO.File.ReadAllBytes(pathOne);

        Texture2D _texture = new Texture2D(2, 2);
        // _texture = NativeCamera.LoadImageAtPath(_url, -1, false);
        _texture.LoadImage(fileBytes);


        ResizeTexture(_texture);


        PictureOne.sprite = Sprite.Create(_texture, new Rect(0.0f, 0.0f, _texture.width, _texture.height),
            new Vector2(0.5f, 0.5f), 100.0f);
        PictureOne.preserveAspect = true;
        cameraOne.gameObject.SetActive(false);
    }

    private void OnImagePickedOptionTwo(string pathFromAndroid)
    {
#if UNITY_EDITOR
        pathTwo = EditorUtility.OpenFilePanel("Overwrite with jpg", "", "jpg");

#elif !UNITY_EDITOR && UNITY_ANDROID
            pathTwo = pathFromAndroid;
#endif


        if (string.IsNullOrEmpty(pathTwo))
            return;


        PostProcessController.Instance.FeedType = FeedType.Poll;

        byte[] fileBytes = System.IO.File.ReadAllBytes(pathTwo);

        Texture2D _texture = new Texture2D(2, 2);
        // _texture = NativeCamera.LoadImageAtPath(_url, -1, false);
        _texture.LoadImage(fileBytes);


        ResizeTexture(_texture);


        PictureTwo.sprite = Sprite.Create(_texture, new Rect(0.0f, 0.0f, _texture.width, _texture.height),
            new Vector2(0.5f, 0.5f), 100.0f);
        PictureTwo.preserveAspect = true;
        cameratwo.gameObject.SetActive(false);
    }

    private void ResizeTexture(Texture2D _texture)
    {
        int maxAllowResoulution = (int) AppManager.APP_SETTINGS.MaxAllowFeedImageQuality;
        int _width = _texture.width;
        int _height = _texture.height;
        if (_width > _height)
        {
            if (_width > maxAllowResoulution)
            {
                float _delta = (float) _width / (float) maxAllowResoulution;
                _height = Mathf.FloorToInt((float) _height / _delta);
                _width = maxAllowResoulution;
            }
        }
        else
        {
            if (_height > maxAllowResoulution)
            {
                float _delta = (float) _height / (float) maxAllowResoulution;
                _width = Mathf.FloorToInt((float) _width / _delta);
                _height = maxAllowResoulution;
            }
        }

        TextureScale.Bilinear(_texture, _width, _height);
    }

    #endregion
}