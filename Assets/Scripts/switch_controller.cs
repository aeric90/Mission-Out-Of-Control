using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class switch_controller : ParentControl
{
    public Image switchSprite;

    public TMPro.TextMeshProUGUI screenText;

    public Sprite switchOn;
    public Sprite switchOff;

    private void Awake()
    {
        controlType = "switch";
        minValue = 0;
        maxValue = 1;
        dependantSource = true;
        dependantTarget = true;
    }

    override public void SetValue(string value)
    {
        this.value = value;
        UpdateScreen();
        UpdateSwitch();
    }

    public void OnButtonPress()
    {
        valueChange = true;
        ChangeValue();
        UpdateScreen();
        UpdateSwitch();
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
    public void UpdateScreen()
    {
        if (value == "1") screenText.text = "ON";
        if (value == "0") screenText.text = "OFF";
    }

    public void UpdateSwitch()
    {
        if (value == "1")
        {
            switchSprite.overrideSprite = switchOn;
        }
        else if (value == "0")
        {
            switchSprite.overrideSprite = switchOff;
        }
    }
}
