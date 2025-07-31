using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class datepickertimetest : MonoBehaviour
{

    public DatePickerControl p;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }



    public void click()
    {
        Debug.Log(p.fecha.ToString("dd-MM-yyyy HH:mm"));



    }








}
