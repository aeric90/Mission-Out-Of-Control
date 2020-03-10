using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ParentControl;

public class SliderScript : ParentControl
{
    public Text valueText;
    public GameObject slider;
    private Slider sliderUI;

    void Start()
    {
        sliderUI = slider.GetComponent<Slider>();
    }

    public void UpdateUI()
    {
        this.value = sliderUI.value.ToString();
        valueText.text = this.value;
    }
}
