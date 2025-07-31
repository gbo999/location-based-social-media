using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PreviewController : MonoBehaviour
{
    // Start is called before the first frame update


    public Image im;

    public GameObject prefab;
    
    
    
    
    
    void Start()
    {
        RuntimePreviewGenerator.BackgroundColor = Color.white;


Texture2D _texture = RuntimePreviewGenerator.GenerateModelPreview(prefab.transform);


       /* Texture2D _texture = AssetPreview.GetAssetPreview(prefab);
*/
        im.sprite = Sprite.Create(_texture, new Rect(0.0f, 0.0f, _texture.width, _texture.height), new Vector2(0.5f, 0.5f), 100.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
