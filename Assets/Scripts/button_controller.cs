using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class button_controller : ParentControl
{
    public Text onOff;

    private void Awake()
    {
        value = "0";
        maxValue = 1;
        minValue = 0;
        dependantSource = true;
        dependantTarget = true;
        onOff.text = "OFF";
    }

    public void OnButtonPress()
    {
        if(value == "0")
        {
            value = "1";
            onOff.text = "ON";
        }

        else if (value == "1")
        {
            value = "0";
            onOff.text = "OFF";
        }
    }
}
