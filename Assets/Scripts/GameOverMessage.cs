using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverMessage : MonoBehaviour
{
    public TMPro.TMP_Text gOverMessageRed;
    public TMPro.TMP_Text gOverMessage;

    void Awake()
    {
        if (WinLossMessage.winLossInstance.won)
        {
            gOverMessage.text = "YOU WON!";
            gOverMessageRed.text = "YOU WON!";
        }
        else
        {
            gOverMessage.text = "YOU LOST!";
            gOverMessageRed.text = "YOU LOST!";
        }
    }
}
