using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAR : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject AR;

    public GameObject SEttings;

    public GameObject bottom;


    public void AllowAR()
    {

        AR.SetActive(true);

        SEttings.SetActive(false);
        bottom.SetActive(false);


        Debug.Log("AR not working");
    }




    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
