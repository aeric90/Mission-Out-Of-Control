using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ParentControl;

public class keypad_controller : ParentControl
{
    public TMPro.TextMeshProUGUI screenText;

    // Start is called before the first frame update
    void Start()
    {
        ClearScreen();
    }

    // Update is called once per frame
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
