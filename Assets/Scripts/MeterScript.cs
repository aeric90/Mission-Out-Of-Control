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
        meterUI.value = float.Parse(GetValue());
    }

    public void UpdateUI()
    {
        this.value = valueText.text = meterUI.value.ToString();
    }
}

