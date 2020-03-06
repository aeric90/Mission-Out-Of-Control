using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class game_controller : MonoBehaviour
{
    public int gameTimer;

    private bool gameOver = false;

    void Start()
    {
        StartCoroutine(CountDown());
    }

    private IEnumerator CountDown()
    {

        while(!gameOver)
        {
            if(gameTimer<=0)
            {
                gameOver = true;
            }

            yield return new WaitForSeconds(1.0f);
            ui_controller.uiInstance.UpdateCountDown(TimerString());
            gameTimer--;
        }
    }

    private string TimerString()
    {
        int timerMinutes = gameTimer / 60;
        int timerSeconds = gameTimer - (timerMinutes * 60);

        return timerMinutes.ToString("00") + ":" + timerSeconds.ToString("00");
    }

}
