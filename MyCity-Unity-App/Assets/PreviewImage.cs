using SocialApp;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PreviewImage : MonoBehaviour
{

    public Image image;

    private Texture2D texture;

    // Start is called before the first frame update
    void Start()
    {

        texture = new Texture2D(2, 2);
    }



    public void apply()
    {

#if UNITY_EDITOR

        string path = EditorUtility.OpenFilePanel("Overwrite with png", "", "png");
        if (path.Length != 0)
        {
            var fileContent = File.ReadAllBytes(path);
            texture.LoadImage(fileContent);
        }

        var sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
        image.sprite = sprite;

#endif



    }

    public void sendPicture()
    {

        StartCoroutine(UploadImage());


    }

    public IEnumerator UploadImage()
    {
        Texture2D tex = image.sprite.texture;


        bool isFinishUpload = false;
        bool isSuccess = false;
        UploadImageRequest uploadRequest = new UploadImageRequest();
        uploadRequest.groupId = AppManager.myCityController.groupPostID;
        ResizeTexture(tex, ImageSize.Size_128);
        byte[] uploadBytes = ImageConversion.EncodeToPNG(tex);
        uploadRequest.ImageBytes = uploadBytes;
        uploadRequest.Size = ImageSize.Size_128;
        AppManager.FIREBASE_CONTROLLER.UploadIcon(uploadRequest, (_callback =>
        {
            isFinishUpload = _callback.IsFinish;
            isSuccess = _callback.IsSuccess;
        }
        ));
        while (!isFinishUpload)
        {
            yield return null;
        }
        if (!isSuccess)
        {
            AppManager.VIEW_CONTROLLER.HideLoading();
            AppManager.VIEW_CONTROLLER.ShowPopupMSG(MessageCode.FailedUploadImage);
            yield break;
        }


        AppManager.NAVIGATION.ShowSetting();



    }
    public void ResizeTexture(Texture2D _texture, ImageSize _size)
    {
        if (_size != ImageSize.Origin)
        {
            int _width = _texture.width;
            int _height = _texture.height;
            if (_width > _height)
            {
                if (_width > (int)_size)
                {
                    float _delta = (float)_width / (float)((int)_size);
                    _height = Mathf.FloorToInt((float)_height / _delta);
                    _width = (int)_size;
                }
            }
            else
            {
                if (_height > (int)_size)
                {
                    float _delta = (float)_height / (float)((int)_size);
                    _width = Mathf.FloorToInt((float)_width / _delta);
                    _height = (int)_size;
                }
            }
            TextureScale.Bilinear(_texture, _width, _height);
        }
    }


}
