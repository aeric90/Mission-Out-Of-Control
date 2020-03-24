using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SliderScript : ParentControl
{
    public GameObject slider;
    private Slider sliderUI;

    public TMPro.TextMeshProUGUI screenText;

    private void Awake()
    {
        value = "1";
        maxValue = 5;
        minValue = 1;
        dependantSource = true;
        dependantTarget = true;
        sliderUI = slider.GetComponent<Slider>();
    }

    private void LateUpdate()
    {
        sliderUI.interactable = this.GetActive();
    }

    public void UpdateUI()
    {
        this.valueChange = true;

        try
        {
            this.value = screenText.text = sliderUI.value.ToString();
        }

        catch (NullReferenceException e)
        {
            //
        }
        
    }
}
