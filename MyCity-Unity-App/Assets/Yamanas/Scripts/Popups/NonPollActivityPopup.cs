using System;
using System.Collections;
using SocialApp;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Yamanas.Infrastructure.Popups;

namespace Yamanas.Scripts.MapLoader.Popups
{
    public class NonPollActivityPopup : MonoBehaviour
    {
        #region Fields

        private string path;

        [SerializeField] private Image _pictureImage;

        [SerializeField] private Image _cameraImage;

        [SerializeField] private VideoPlayer VPlayer;

        [SerializeField] private TMP_InputField _postText;

        [SerializeField] private DatePickerControl _datePickerControl;

        [SerializeField] private TMP_InputField _priceText;

        #endregion

        #region methods


        public void OnCloseButtonClick()
        {
            PopupSystem.Instance.CloseAllPopups();
            
        }




        public void SelectPhotoFromGallery()
        {
#if !UNITY_EDITOR && UNITY_ANDROID
            NativeGallery.GetImageFromGallery(OnImagePicked);


#else

            OnImagePicked("");


#endif
        }


        public void SelectVideoFromGallery()
        {
#if !UNITY_EDITOR && UNITY_ANDROID
            NativeGallery.GetVideoFromGallery(OnVideoPicked);


#else
            OnVideoPicked("");


#endif
        }

        public void OnImagePicked(string pathFromAndroid)
        {
#if UNITY_EDITOR
            path = EditorUtility.OpenFilePanel("Overwrite with jpg", "", "jpg");

#elif !UNITY_EDITOR && UNITY_ANDROID
            path = pathFromAndroid;
#endif


            if (string.IsNullOrEmpty(path))
                return;

            PostProcessController.Instance.Path = path;
            PostProcessController.Instance.FeedType = FeedType.Image;

            byte[] fileBytes = System.IO.File.ReadAllBytes(path);

            Texture2D _texture = new Texture2D(2, 2);
            _texture.LoadImage(fileBytes);


            ResizeTexture(_texture);


            _pictureImage.sprite = Sprite.Create(_texture, new Rect(0.0f, 0.0f, _texture.width, _texture.height),
                new Vector2(0.5f, 0.5f), 100.0f);
            _pictureImage.preserveAspect = true;
            _cameraImage.gameObject.SetActive(false);
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

        private void OnVideoPicked(string path)
        {
            StartCoroutine(OnVideoPickedEnu(path));
        }

        public IEnumerator OnVideoPickedEnu(string PathFromAndroid)
        {
#if UNITY_EDITOR
            path = EditorUtility.OpenFilePanel("Overwrite with mp4", "", "mp4");

#elif !UNITY_EDITOR && UNITY_ANDROID
        path = PathFromAndroid;

#endif


            if (string.IsNullOrEmpty(path))
                yield break;

            PostProcessController.Instance.FeedType = FeedType.Video;

            byte[] fileBytes = System.IO.File.ReadAllBytes(path);

            byte[] imageBytes = null;
            byte[] videoBytes = null;
            if (!CheckVideoSize(fileBytes))
            {
                AppManager.VIEW_CONTROLLER.ShowPopupMSG(MessageCode.MaxVideoSize);
                yield break;
            }
#if !UNITY_EDITOR && UNITY_ANDROID
         VPlayer.url = "file://" + path;

#else
            VPlayer.url = path;


#endif

            // VPlayer.url = path;


            VPlayer.Prepare();
            while (!VPlayer.isPrepared)
            {
                yield return null;
            }

            VPlayer.Play();
            while (!VPlayer.isPlaying)
            {
                yield return null;
            }

            yield return null;

            NativeCamera.VideoProperties _videoProp = NativeCamera.GetVideoProperties(path);

            float videoWidth = NativeGallery.GetVideoProperties(path).width;
            float videoHeight = NativeGallery.GetVideoProperties(path).height;

            videoBytes = fileBytes;
            while (VPlayer.frame < 2)
            {
                yield return null;
            }

            Texture2D _texture = ReadExternalTexture(VPlayer.texture);

            // rotate image 90 degrees
            // if is vertical
            if (videoHeight < videoWidth)
            {
                _texture = rotateTexture(_texture);
            }

            if (_videoProp.rotation == 270)
            {
                for (int i = 0; i < 2; i++)
                {
                    _texture = rotateTexture(_texture);
                }
            }

            ResizeTexture(_texture);
            imageBytes = _texture.EncodeToJPG(AppManager.APP_SETTINGS.UploadImageQuality);

            VPlayer.Stop();


            _pictureImage.sprite = Sprite.Create(_texture, new Rect(0.0f, 0.0f, _texture.width, _texture.height),
                new Vector2(0.5f, 0.5f), 100.0f);
            _pictureImage.preserveAspect = true;
            //shareCamera.gameObject.SetActive(false);
            Debug.Log("in share");


            //  UploadFile(path);


            // publishPanel.SetActive(false);
        }

        public bool CheckVideoSize(byte[] _bytes)
        {
            int _mb = _bytes.Length / 1024 / 1024;
            return _mb <= AppManager.APP_SETTINGS.MaxUploadVideoSizeMB;
        }

        private Texture2D ReadExternalTexture(Texture externalTexture)
        {
            Texture2D myTexture2D = new Texture2D(externalTexture.width, externalTexture.height);
            if (myTexture2D == null)
            {
                myTexture2D = new Texture2D(externalTexture.width, externalTexture.height);
            }

            //Make RenderTexture type variable
            RenderTexture tmp = RenderTexture.GetTemporary(
                externalTexture.width,
                externalTexture.height,
                0,
                RenderTextureFormat.ARGB32,
                RenderTextureReadWrite.sRGB);

            Graphics.Blit(externalTexture, tmp);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = tmp;

            myTexture2D.ReadPixels(new UnityEngine.Rect(0, 0, tmp.width, tmp.height), 0, 0);
            myTexture2D.Apply();

            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(tmp);

            return myTexture2D;
        }

        public static Texture2D rotateTexture(Texture2D t)
        {
            Texture2D newTexture = new Texture2D(t.height, t.width, t.format, false);

            for (int i = 0; i < t.width; i++)
            {
                for (int j = 0; j < t.height; j++)
                {
                    newTexture.SetPixel(j, i, t.GetPixel(t.width - i, j));
                }
            }

            newTexture.Apply();
            return newTexture;
        }

        public void OnPostButtonClick()
        {
            if (PostProcessController.Instance.Kind == "Sale")
            {
                PostProcessController.Instance.Price = Convert.ToDouble(_priceText.text);
            }

            if (PostProcessController.Instance.Kind == "Event")
            {
                PostProcessController.Instance.EventStartDate = _datePickerControl.fecha.ToString("dd-MM-yyyy HH:mm");
                PostProcessController.Instance.DateTosave = _datePickerControl.fecha.ToString("MM-yyyy");
            }

            if (PostProcessController.Instance.FeedType == FeedType.Video)
            {
                PostProcessController.Instance.VideoPlayer = VPlayer;
                PostProcessController.Instance.ImageVideo = _pictureImage;
            }

            PostProcessController.Instance.PostText = _postText.text;
            
            PostProcessController.Instance.UploadFile(path);


        }

        #endregion
    }
}