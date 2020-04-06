using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class game_controller : MonoBehaviour
{
    public static game_controller gameInstance;

    public GameObject cameraMain;

    public int gameTimer;

    private bool confirmPressed = false;
    private bool gameOver = false;

    private int currentInstruction = -1;

    private string currentInstructionCaption = "";

    private void Awake()
    {
        if (gameInstance == null) { gameInstance = this; }
    }

    /* public void Start()
    {
        StartCoroutine(GameLoop());
        MoveTowardsShip.moveTowardsShipInstance.StartPlanetMovement();
    } */

    public void StartGame()
    {
        StartCoroutine(GameLoop());
        MoveTowardsShip.moveTowardsShipInstance.StartPlanetMovement();
    }

    private IEnumerator GameLoop()
    {
        StartCoroutine(CountDown());
        NextInstruction();

        while (!gameOver)
        {
            if (confirmPressed)
            { 
                ui_controller.uiInstance.EnableConfirmButton(false);
                ui_controller.uiInstance.EnableControls(false);
                ui_controller.uiInstance.SetComputerLine(currentInstructionCaption);

                bool instructionComplete = ConfirmSteps();

                yield return new WaitForSeconds(1.0f);
                while (ui_controller.uiInstance.GetComputerUpdatingStatus()) yield return null;
                yield return new WaitForSeconds(3.0f);

                if (instructionComplete) NextInstruction();

                yield return new WaitForSeconds(1.0f);
                while (ui_controller.uiInstance.GetComputerUpdatingStatus()) yield return null;

                ui_controller.uiInstance.EnableControls(true);
                ui_controller.uiInstance.EnableConfirmButton(true);
                confirmPressed = false;
            }
            yield return null;
        }
    }

    private IEnumerator CountDown()
    {
        bool played = false;

        while (!gameOver)
        {
            if (gameTimer <= 0)
            {
                GameOver(false);
            }

            if (gameTimer <= 14)
            {
                if (!played)
                {
                    AudioPlayer.audioPlayerInstance.PlayClip(AudioPlayer.audioPlayerInstance.audioClips[0], true, 0.05f);
                    played = true;
                }
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

    public bool ConfirmSteps()
    {
        bool instructionSucceeded = true;

        GameInstruction current = puzzle_controller.puzzleInstance.GetGameInstruction(currentInstruction);

        for (int i = 0; i < current.GetStepCount(); i++)
        {
            instructionSucceeded = current.CheckStep(i, ui_controller.uiInstance.GetControlValue(current.GetStepControl(i)));
            if (instructionSucceeded)
            {
                ui_controller.uiInstance.AddComputerLine("\n\t\t STEP " + (i + 1) + " -- " + " CORRECT");
            }
            else 
            {
                ui_controller.uiInstance.AddComputerLineTag("<color=#FF0000>");
                ui_controller.uiInstance.AddComputerLine("\n\t\t STEP " + (i + 1) + " -- ");
                ui_controller.uiInstance.AddComputerLine("INCORRECT");
                break; 
            }
        }

        if (instructionSucceeded) 
        {
            if (current.CheckSuccessTrigger()) current.TriggerSuccess();
            ui_controller.uiInstance.AddComputerLine("\n\t\t\t INSTRUCTION COMPLETE");
        }
        else
        {
            cameraMain.GetComponent<Animator>().Play("Shake");
            AudioPlayer.audioPlayerInstance.PlayClip(AudioPlayer.audioPlayerInstance.audioClips[1], false, 0.3f);
            ui_controller.uiInstance.AddComputerLine("\n\t\t\t INSTRUCTION INCOMPLETE");
        }

        return instructionSucceeded;
    }

    private void NextInstruction()
    {
        currentInstruction++;

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

    public void GameOver(bool win)
    {
        gameOver = true;

        if (win)
        {
            ui_controller.uiInstance.AddComputerLine("\n\n\tYOU WIN!"); // Won't type to screen.
            WinLossMessage.winLossInstance.won = true;
            StartCoroutine(GameOverCo());
        }
        else
        {
            ui_controller.uiInstance.AddComputerLine("\n\n\tYOU LOST!"); // Won't type to screen.
            WinLossMessage.winLossInstance.won = false;
            StartCoroutine(GameOverCo());
        }
    }

    public bool GetGameOver()
    {
        return gameOver;
    }

    public void SetConfirmPressed()
    {
        confirmPressed = true;
    }

    private IEnumerator GameOverCo()
    {
        yield return new WaitForSeconds(5.0f);

        SceneManager.LoadScene(3);
    }
}
