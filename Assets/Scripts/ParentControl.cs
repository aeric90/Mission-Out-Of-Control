using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentControl : MonoBehaviour
{

    public string value;
    public TMPro.TextMeshProUGUI labelText;

    private bool controlActive = true;

    public ParentControl()
    {
        value = "";
    }

    public void SetLabel(string label)
    {
        labelText.text = label;
    }

    public string GetValue()
    {
        return value;
    }

    public void SetValue(string input)
    {
        value = input;
    }

    public bool GetActive()
    {
        return controlActive;
    }

    public void SetActive(bool setting)
    {
        controlActive = setting;
    }

}
