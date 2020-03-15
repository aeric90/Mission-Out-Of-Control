using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentControl : MonoBehaviour
{
    public string type;
    
    public int maxValue;
    public int minValue;
    public string value;
    public bool valueChange;

    public int connectedControl = -1;
    public string connectionType = "";

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
                string newValue = "";

                if (connectionType == "mapped")
                {
                    int toLow = ui_controller.uiInstance.GetControlMinValue(connectedControl);
                    int toHigh = ui_controller.uiInstance.GetControlMaxValue(connectedControl);

                    newValue = map(int.Parse(value), minValue, maxValue, toLow, toHigh).ToString();
                }

                if (connectionType == "random")
                {
                    int min = ui_controller.uiInstance.GetControlMinValue(connectedControl);
                    int max = ui_controller.uiInstance.GetControlMaxValue(connectedControl);
                    if (min == 0) max -= 1;

                    do
                    {
                        int randomValue = (int)Random.Range(1.0f, max + 1);
                        newValue = randomValue.ToString();
                    } while (newValue == value);
                }

                ui_controller.uiInstance.SetControlValue(connectedControl, newValue);
            }

            valueChange = false;
        }
    }

    public void SetConnectedControl(int controlID, string type)
    {
        connectedControl = controlID;
        connectionType = type;
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

    public int GetMaxValue()
    {
        return maxValue;
    }

    public int GetMinValue()
    {
        return minValue;
    }

    private static int map(int value, int fromLow, int fromHigh, int toLow, int toHigh)
    {
        return (value - fromLow) * (toHigh - toLow) / (fromHigh - fromLow) + toLow;
    }

    virtual public string GetRandomAnswer()
    {
        int randomValue = Random.Range(minValue, maxValue + 1);
        
        return randomValue.ToString();
    }

}
