using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MeterScript : ParentControl
{
    public TMPro.TextMeshProUGUI valueText;
    public GameObject meter;
    private Slider meterUI;

    private void Awake()
    {
        meterUI = meter.GetComponent<Slider>();

        controlType = "meter";
        value = "0";
        maxValue = 4;
        minValue = 0;
        dependantSource = false;
        dependantTarget = true;
    }

    private void Update()
    {
        meterUI.value = float.Parse(value);
        valueText.text = meterUI.value.ToString();
    }
}

