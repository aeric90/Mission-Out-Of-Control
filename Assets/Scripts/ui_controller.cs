using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ui_controller : MonoBehaviour
{
    public static ui_controller uiInstance;
    public GameObject screenControl;
    public GameObject[] controls;

    public TMPro.TextMeshProUGUI countDownText;
    public TMPro.TextMeshProUGUI computerText;
    public Button confirmButton;

    private void Awake()
    {
        if (uiInstance == null) { uiInstance = this; }
    }

    public void UpdateCountDown(string timerString)
    {
        countDownText.text = timerString;
    }
    
    public int GetControlsCount()
    {
        return controls.Length;
    }
    public void SetControlLabel(int controlID, string label)
    {
        controls[controlID].GetComponent<ParentControl>().SetLabel(label);
    }

    public void SetControlValue(int controlID, string value)
    {
        controls[controlID].GetComponent<ParentControl>().SetValue(value);
    }

    public string GetControlValue(int controlID)
    {
        return controls[controlID].GetComponent<ParentControl>().GetValue();

    }

    public void ClearComputer()
    {
        computerText.GetComponent<computer_text_controller>().ClearText();
    }
    public void AddComputerLine(string lineText)
    {
        computerText.GetComponent<computer_text_controller>().AddText(lineText);
    }
    public void SetComputerLine(string lineText)
    {
        computerText.GetComponent<computer_text_controller>().SetText(lineText);
    }
    public bool GetComputerUpdatingStatus()
    {
        return computerText.GetComponent<computer_text_controller>().GetTextUpdateStatus();
    }
    public void ConfirmPressed()
    {
        game_controller.gameInstance.SetConfirmPressed();
    }
    public void EnableControls(bool setting)
    {
        for(int i = 0; i < controls.Length; i++)
        {
            controls[i].GetComponent<ParentControl>().SetActive(setting);
        }
    }
    public void SetConnectedControls(int controlID1, int controlID2, string type)
    {
        controls[controlID1].GetComponent<ParentControl>().SetConnectedControl(controlID2, type);
    }
    public int GetControlMaxValue(int controlID)
    {
        return controls[controlID].GetComponent<ParentControl>().GetMaxValue();
    }
    public int GetControlMinValue(int controlID)
    {
        return controls[controlID].GetComponent<ParentControl>().GetMinValue();
    }
    public string GetControlRandomAnswer(int controlID)
    {
        return controls[controlID].GetComponent<ParentControl>().GetRandomAnswer();
    }
    public void SetScreenPlanetText(string planetText)
    {
        screenControl.GetComponent<screen_controller>().SetPlanetText(planetText);
    }
    public void SetScreenSystemText(string systemText)
    {
        screenControl.GetComponent<screen_controller>().SetSystemText(systemText);
    }
    public void SetScreenEngineText(string engineText)
    {
        screenControl.GetComponent<screen_controller>().SetEngineText(engineText);
    }
    public void SetScreenEngineNoText(string engingNoText)
    {
        screenControl.GetComponent<screen_controller>().SetNoEngineText(engingNoText);
    }
    public void EnableConfirmButton(bool value)
    {
        confirmButton.interactable = value;
    }
    public bool GetControlDependantTarget(int controlID)
    {
        return controls[controlID].GetComponent<ParentControl>().GetDependantTarget();
    }
    public int GetControlRange(int controlID)
    {
        int minValue = controls[controlID].GetComponent<ParentControl>().GetMinValue();
        int maxValue = controls[controlID].GetComponent<ParentControl>().GetMaxValue();

        return maxValue - minValue + 1;
    }
    public void ClearKeypads()
    {
        for (int i = 0; i < controls.Length; i++)
        { 
            if(controls[i].GetComponent<ParentControl>().GetControlType() == "keypad")
            {
                controls[i].GetComponent<ParentControl>().ClearValue();
            }
        
        }
    }
    public int GetRandomControlOfType(string[] types)
    {
        List<int> checkedControls = new List<int>();

        do
        {
            int controlID = Random.Range(0, controls.Length);

            if (!checkedControls.Contains(controlID))
            {
                for (int i = 0; i < types.Length; i++)
                {

                    string controlType = controls[controlID].GetComponent<ParentControl>().GetControlType();

                    if (controlType == types[i])
                    {
                        return controlID;
                    }
                }

                checkedControls.Add(controlID);
            }

        } while (checkedControls.Count <= GetControlsCount());

        return - 1;
    }

    public int GetRandomControlOfType(string[] types, int notControlID)
    {
        List<int> checkedControls = new List<int>();

        do
        {
            int controlID = Random.Range(0, controls.Length);

            if (!checkedControls.Contains(controlID))
            {
                for (int i = 0; i < types.Length; i++)
                {

                    string controlType = controls[controlID].GetComponent<ParentControl>().GetControlType();

                    if (controlType == types[i] && controlID != notControlID)
                    {
                        return controlID;
                    }
                }

                checkedControls.Add(controlID);
            }

        } while (checkedControls.Count <= GetControlsCount());

        return -1;
    }

    public string GetControlLabel(int controlID)
    {
        return controls[controlID].GetComponent<ParentControl>().GetLabel();
    }
}
