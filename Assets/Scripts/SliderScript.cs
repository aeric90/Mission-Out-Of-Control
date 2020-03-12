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

    private void Update()
    {
        sliderUI.interactable = this.GetActive();
    }

    public void UpdateUI()
    {
       this.value = valueText.text = sliderUI.value.ToString();
    }
}
