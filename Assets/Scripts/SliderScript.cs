using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ParentControl;

public class SliderScript : MonoBehaviour
{
    public Control slider = new Control();

    private Text valueText;
    private Slider sliderUI;

    void Start()
    {
        sliderUI = GetComponent<Slider>();
        valueText = transform.GetChild(1).gameObject.GetComponent<Text>();
    }

    public void UpdateUI()
    {
        slider.value = valueText.text = sliderUI.value.ToString();
    }
}
