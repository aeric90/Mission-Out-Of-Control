using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class computer_text_controller : MonoBehaviour
{
    public TMPro.TextMeshProUGUI computerText;

    private string textBuffer = "";
    private bool textUpdating = false;
    private string inputString;

    IEnumerator UpdateComputerText()
    {
        textUpdating = true;

        while (textUpdating)
        {
            for(int i = 0; i < inputString.Length; i++)
            {
                textBuffer += inputString[i];
                computerText.text = textBuffer;
                yield return new WaitForSeconds(0.10f);
            }
            
            if(inputString == " CORRECT")
            {
                // Do nothing.
            }

            else
            {
                ui_controller.uiInstance.EnableConfirmButton(true);
            }

            textUpdating = false;
        }
    }

    public void AddText(string inputString)
    {
        this.inputString = inputString;
        StartCoroutine(UpdateComputerText());
    }

    public bool GetTextUpdating()
    {
        return textUpdating;
    }
    public void ClearText()
    {
        textBuffer = "";
    }
}
