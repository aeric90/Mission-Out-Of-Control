using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ui_controller : MonoBehaviour
{
    public static ui_controller uiInstance;
    public GameObject[] controls;

    public TMPro.TextMeshProUGUI countDownText;
    public TMPro.TextMeshProUGUI computerText;

    private void Awake()
    {
        if (uiInstance == null) { uiInstance = this; }
    }

    public void UpdateCountDown(string timerString)
    {
        countDownText.text = timerString;
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

    public void AddComputerLine(string lineText, bool newLine)
    {
        if(newLine) lineText += "\n";
        computerText.GetComponent<computer_text_controller>().AddText(lineText);
    }

    public void ConfirmPressed()
    {
        game_controller.gameInstance.ConfirmSteps();
    }

    public void EnableControls(bool setting)
    {
        for(int i = 0; i < controls.Length; i++)
        {
            controls[i].GetComponent<ParentControl>().SetActive(setting);
        }
    }

    public void SetConnectedControls(int controlID1, int controlID2)
    {
        controls[controlID1].GetComponent<ParentControl>().SetConnectedControl(controlID2);
    }

    public int GetControlNumStates(int controlID)
    {
        return controls[controlID].GetComponent<ParentControl>().GetNumStates();
    }

    public int GetControlMinState(int controlID)
    {
        return controls[controlID].GetComponent<ParentControl>().GetMinState();
    }
}
