using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyCityMarker : MonoBehaviour
{
    
    
    public Texture2D markerTexture;


    private void Awake()
    {

  

        Debug.Log("width" + markerTexture.width);
        Debug.Log("height" + markerTexture.height);

        Sprite sp = Sprite.Create(markerTexture, new Rect(0.0f, 0.0f, markerTexture.width, markerTexture.height), new Vector2(0.5f, 0.5f), 100.0f); ;

        SpriteRenderer s = GetComponent<SpriteRenderer>();
        s.sprite = sp;

    }


    // Start is called before the first frame update
    void Start()
    {

        

        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
