using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentControl : MonoBehaviour
{

    public string value;
    private bool controlActive = true;

    public ParentControl()
    {
        value = "";
    }
 
    public void SetValue(string input)
    {
        value = input;
    }

    public string GetValue()
    {
        return value;
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
