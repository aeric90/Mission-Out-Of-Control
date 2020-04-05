﻿
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class button_controller : ParentControl
{
    public TMPro.TextMeshProUGUI screenText;

    void Awake()
    {
        controlType = "button";
        value = "0";
        maxValue = 1;
        minValue = 0;
        dependantSource = true;
        dependantTarget = true;
        screenText.text = "OFF";
    }

    public void OnButtonPress()
    {
        ChangeValue();
        UpdateScreen();
    }

    public void UpdateScreen()
    {
        if (value == "1") screenText.text = "ON";
        if (value == "0") screenText.text = "OFF";
    }
    override public void SetValue(string value)
    {
        this.value = value;
        UpdateScreen();
    }
    public void ChangeValue()
    {
        valueChange = true;
        if (value == "0")
        {
            value = "1";
        }
        else if (value == "1")
        {
            value = "0";
        }
    }
}
