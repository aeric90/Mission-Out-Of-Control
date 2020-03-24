using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class computer_text_controller : MonoBehaviour
{
    public TMPro.TextMeshProUGUI computerText;
    private string textBuffer = "";
    public bool textUpdating = false;

    private void Start()
    {
        StartCoroutine(UpdateComputerText());
    }

    IEnumerator UpdateComputerText()
    {
        while (!game_controller.gameInstance.GetGameOver())
        {
            if (computerText.text != textBuffer)
            {
                textUpdating = true;

                for (int i = computerText.text.Length; i < textBuffer.Length; i++)
                {
                    computerText.text += textBuffer[i];
                    yield return new WaitForSeconds(0.05f);
                }
            }
            else
            {
                textUpdating = false;
            }

            yield return null;
        }
    }

    public void AddText(string inputString)
    {
        this.textBuffer += inputString;
    }

    public void SetText(string inputString)
    {
        computerText.text = inputString;
        textBuffer = inputString;
    }

    public void ClearText()
    {
        computerText.text = "";
        textBuffer = "";
    }

    public bool GetTextUpdateStatus()
    {
        return textUpdating;
    }
}
