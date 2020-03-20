using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MeterScript : ParentControl
{
    public Text valueText;
    public GameObject meter;
    private Slider meterUI;

    private void Awake()
    {
        value = "0";
        maxValue = 4;
        minValue = 0;
        dependantSource = false;
        dependantTarget = true;

        meterUI = meter.GetComponent<Slider>();
    }

    void Start()
    {

    }

    private void Update()
    {
        meterUI.value = float.Parse(value);
    }

    public void UpdateUI()
    {
        valueText.text = meterUI.value.ToString();
    }
}

