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

    // Start is called before the first frame update
    //0 off, 1 on.
    void Start()
    {
        value = "0";
        switchSprite.overrideSprite = switchOff;
    }

    public void OnButtonPress()
    {
        valueChange = true;

        if (value == "0")
        {
            switchSprite.overrideSprite = switchOn;
            value = "1";
        }
        else if (value == "1")
        {
            switchSprite.overrideSprite = switchOff;
            value = "0";
        }
    }
}
