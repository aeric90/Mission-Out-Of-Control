using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class switch_controller : ParentControl
{
    public Image switchSprite;
    public Sprite switchOn;
    public Sprite switchOff;

    private void LateUpdate()
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

    public void OnButtonPress()
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
