using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ParentControl;

public class SliderScript : MonoBehaviour
{
    public class SliderControl : Control
    {
        public SliderControl()
        {

        }
    }

    public SliderControl slider = new SliderControl();
    public Text valueText;
    private Slider sliderUI;

    void Start()
    {
        sliderUI = GetComponent<Slider>();
    }

    public void UpdateUI()
    {
        slider.value = sliderUI.value.ToString();
        valueText.text = slider.value;
    }
}
