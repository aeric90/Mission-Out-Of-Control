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
        value = "1";

        numStates = 5;
        minState = 1;
        sliderUI = slider.GetComponent<Slider>();
    }

    private void LateUpdate()
    {
        sliderUI.interactable = this.GetActive();
    }

    public void UpdateUI()
    {
       this.valueChange = true;
       this.value = valueText.text = sliderUI.value.ToString();
    }
}
