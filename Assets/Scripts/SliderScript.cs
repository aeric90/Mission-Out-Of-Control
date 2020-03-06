using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderScript : MonoBehaviour
{
    public class Control
    {
        public int value;

        public Control()
        {
            value = 0;
        }

        public int GetValue()
        {
            return value;
        }
    }

    public class SliderControl : Control
    {
        public SliderControl()
        {

        }
    }

    SliderControl slider = new SliderControl();
    public Text valueText;
    public Slider sliderUI;

    void Start()
    {
        valueText = transform.GetChild(0).gameObject.GetComponent<Text>();
        sliderUI = transform.GetChild(1).gameObject.GetComponent<Slider>();
    }

    void Update()
    {
        slider.value = (int)sliderUI.value;
        valueText.text = slider.value.ToString();
    }
}
