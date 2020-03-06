using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ui_controller : MonoBehaviour
{
    public static ui_controller uiInstance;
    public GameObject[] controls;

    public TMPro.TextMeshProUGUI countDownText;
    public TMPro.TextMeshProUGUI computerText;

    void Start()
    {
        if(uiInstance == null) { uiInstance = this; }
    }

    public void UpdateCountDown(string timerString)
    {
        countDownText.text = timerString;
    }

    public string GetControlValue(int controlID)
    {
        return controls[controlID].GetComponent<keypad_controller>().GetValue();

    }

    public void AddComputerLine(string lineText)
    {
        computerText.text += lineText;
    }

    public void ConfirmPressed()
    {
        game_controller.gameInstance.ConfirmSteps();
    }
}
