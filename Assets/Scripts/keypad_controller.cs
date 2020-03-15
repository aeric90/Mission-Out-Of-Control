using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ParentControl;

public class keypad_controller : ParentControl
{
    public TMPro.TextMeshProUGUI screenText;

    private void Awake()
    {
        maxValue = 999;
        minValue = 0;
    }

    void Start()
    {
        ClearScreen();
    }

    void Update()
    {
        screenText.text = this.value;
    }

    public void KeyClick(string keyName)
    {
        if (this.GetActive())
        {
            if (keyName == "clear")
            {
                ClearScreen();
            }
            else
            {
                if (this.value.Length < 3)
                {
                    this.value += keyName;
                }
            }
        }
    }

    private void ClearScreen()
    {
        this.value = "";
    }
}
