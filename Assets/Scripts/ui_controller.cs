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
}
