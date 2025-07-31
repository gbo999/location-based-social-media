using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cubeTest : MonoBehaviour




{

    

    public Texture2D texture2D;

    // Start is called before the first frame update
    void Start()
    {

        Material mat = GetComponent<Renderer>().material;
        mat.mainTexture = texture2D;



    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
