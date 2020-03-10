using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MeterScript : ParentControl
{
    public Text valueText;
    public GameObject meter;
    private Slider meterUI;

    void Start()
    {
        meterUI = meter.GetComponent<Slider>();
    }

    public void SetMeterValue(int value)
    {
        meterUI.value = value;
    }

    public void UpdateUI()
    {
        this.value = valueText.text = meterUI.value.ToString();
    }
}

