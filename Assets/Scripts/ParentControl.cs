using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentControl : MonoBehaviour
{

    public string value;
    
    public int numStates;
    public int minState;

    public int connectedControl = -1;

    public bool valueChange;

    public TMPro.TextMeshProUGUI labelText;

    private bool controlActive = true;

    public ParentControl()
    {
        value = "";
    }

    void Update()
    {
        if (valueChange)
        {
            // Do I have a connected control?
            if(connectedControl > 0)
            {
                int toLow = ui_controller.uiInstance.GetControlMinState(connectedControl);
                int toHigh = toLow + ui_controller.uiInstance.GetControlNumStates(connectedControl) - 1;

                string mapedValue = map(int.Parse(value), minState, minState + numStates - 1, toLow, toHigh).ToString();

                ui_controller.uiInstance.SetControlValue(connectedControl, mapedValue);
            }

            valueChange = false;
        }
    }

    public void SetConnectedControl(int controlID)
    {
        connectedControl = controlID;
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

    public int GetNumStates()
    {
        return numStates;
    }

    public int GetMinState()
    {
        return minState;
    }

    private static int map(int value, int fromLow, int fromHigh, int toLow, int toHigh)
    {
        return (value - fromLow) * (toHigh - toLow) / (fromHigh - fromLow) + toLow;
    }

}
