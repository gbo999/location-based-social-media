using System;
using System.Collections;
using System.Collections.Generic;
using UI.Dates;
using UnityEngine;

public class DateTimePickerTest : MonoBehaviour
{
    // Start is called before the first frame update


    public DatePicker p;
    private DateTime t;

    void Start()
    {
        
        



    }



    public void getDate()
    {
        t = p.SelectedDate.Date;
        Debug.Log(t.ToString());
        

    }

    // Update is called once per frame
    void Update()
    {

        
    }
}
