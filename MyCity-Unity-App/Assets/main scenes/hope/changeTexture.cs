using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class changeTexture : MonoBehaviour
{
    // Start is called before the first frame update

    Renderer m_rend;
    
    public Texture m_MainTexture, m_Metal;

    public VideoPlayer video;
   
    public Vector3 RotateAmount;

    void Start()
    {
        m_rend = GetComponent<Renderer>();

        m_rend.materials[0].mainTexture = m_MainTexture;
        //m_rend.materials[1].mainTexture = m_Normal;
        m_rend.materials[2].mainTexture = m_Metal;


    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(RotateAmount * Time.deltaTime);
    }
}
