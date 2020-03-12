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
        numStates = 5;
        minState = 0;
        meterUI = meter.GetComponent<Slider>();
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

