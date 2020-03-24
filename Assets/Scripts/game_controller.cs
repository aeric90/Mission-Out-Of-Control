using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class game_controller : MonoBehaviour
{
    public static game_controller gameInstance;

    public int gameTimer;

    private bool gameOver = false;

    private int currentInstruction = 0;

    private string currentInstructionCaption = "";

    private void Awake()
    {
        if (gameInstance == null) { gameInstance = this; }
    }

    void Start()
    {
        StartCoroutine(CountDown());
        NextInstruction();
    }

    private IEnumerator CountDown()
    {

        while(!gameOver)
        {
            if(gameTimer<=0)
            {
                GameOver(false);
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

    public void ConfirmSteps()
    {
        bool instructionSucceeded = true;
        ui_controller.uiInstance.EnableConfirmButton(false);
        ui_controller.uiInstance.EnableControls(false);
        ui_controller.uiInstance.SetComputerLine(currentInstructionCaption);
        GameInstruction current = puzzle_controller.puzzleInstance.GetGameInstruction(currentInstruction);

        // Loop through each step in the instruction and update the succeeded value.
        for (int i = 0; i < current.GetStepCount(); i++)
        {
            instructionSucceeded = current.CheckStep(i, ui_controller.uiInstance.GetControlValue(current.GetStepControl(i)));
            if (instructionSucceeded)
            {
                ui_controller.uiInstance.AddComputerLine("\n\t\t STEP " + (i + 1) + " -- " + " CORRECT");
            }
            else 
            {
                ui_controller.uiInstance.AddComputerLine("\n\t\t STEP " + (i + 1) + " -- " + " INCORRECT");
                break; 
            }
        }

        if (instructionSucceeded) 
        {
            if (current.CheckSuccessTrigger()) current.TriggerSuccess();
            ui_controller.uiInstance.AddComputerLine("\n\t\t\t INSTRUCTION COMPLETE");
            StartCoroutine(PauseOnCorrect());
        }
        else
        {
            ui_controller.uiInstance.AddComputerLine("\n\t\t\t INSTRUCTION INCOMPLETE");
            ui_controller.uiInstance.EnableConfirmButton(true);
            StartCoroutine(PauseOnIncorrect());
        }

        ui_controller.uiInstance.EnableControls(true);
    }

    private void NextInstruction()
    {

        if (currentInstruction >= puzzle_controller.puzzleInstance.GetGameInstructionCount())
        {
            GameOver(true);
        }
        else
        {
            ui_controller.uiInstance.ClearKeypads();
            ui_controller.uiInstance.ClearComputer();

            currentInstructionCaption = (currentInstruction + 1) + " - " + puzzle_controller.puzzleInstance.GetGameInstructionTitle(currentInstruction);

            int stepCount = puzzle_controller.puzzleInstance.GetGameStepCount(currentInstruction);

            if (stepCount > 1)
            {
                currentInstructionCaption += " (" + stepCount + " STEPS)";
            }
            else
            {
                currentInstructionCaption += " (" + stepCount + " STEP)";
            }

            ui_controller.uiInstance.AddComputerLine(currentInstructionCaption);
        }
    }

    private IEnumerator PauseOnCorrect()
    {
        yield return new WaitForSeconds(1.0f);

        while (ui_controller.uiInstance.GetComputerUpdatingStatus())
        {
            yield return new WaitForSeconds(1.0f);
        }

        yield return new WaitForSeconds(3.0f);

        ui_controller.uiInstance.EnableConfirmButton(true);
        currentInstruction++;
        NextInstruction();

        StopCoroutine(PauseOnCorrect());
    }

    private IEnumerator PauseOnIncorrect()
    {
        yield return new WaitForSeconds(1.0f);

        while (ui_controller.uiInstance.GetComputerUpdatingStatus())
        {
            yield return new WaitForSeconds(1.0f);
        }

        Debug.Log("Waiting 3 seconds");
        yield return new WaitForSeconds(3.0f);

        ui_controller.uiInstance.EnableConfirmButton(true);

        StopCoroutine(PauseOnIncorrect());
    }

    public void GameOver(bool win)
    {
        gameOver = true;

        if (win)
        {
            ui_controller.uiInstance.AddComputerLine("\n\n\tYOU WIN!");
        }
        else
        {
            ui_controller.uiInstance.AddComputerLine("\n\n\tYOU LOST!");
        }
    }

    public bool GetGameOver()
    {
        return gameOver;
    }

}
