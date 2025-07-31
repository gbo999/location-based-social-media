using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderValueText : MonoBehaviour
{
    public Slider sliderUI;
    public TMP_Text textSliderValue;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void ShowSliderValue()
    {
        string sliderMessage = ((int)sliderUI.value).ToString();
        textSliderValue.text = sliderMessage;
    }



}
