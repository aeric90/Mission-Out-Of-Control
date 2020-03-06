using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class keypad_controller : MonoBehaviour
{
    public TMPro.TextMeshProUGUI screenText;
    private string keyPadValue;

    public bool controlActive = true;

    // Start is called before the first frame update
    void Start()
    {
        ClearScreen();
    }

    // Update is called once per frame
    void Update()
    {
        screenText.text = keyPadValue;
    }

    public void KeyClick(string keyName)
    {
        if (controlActive)
        {
            if (keyName == "clear")
            {
                ClearScreen();
            }
            else
            {
                if (keyPadValue.Length < 3)
                {
                    keyPadValue += keyName;
                }
            }
        }
    }

    private void ClearScreen()
    {
        keyPadValue = "";
    }

    public string GetValue()
    {
        return keyPadValue;
    }
}
