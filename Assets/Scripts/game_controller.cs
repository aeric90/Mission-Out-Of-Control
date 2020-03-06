using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class contains the relevant control and the expected answer
public class GameStep
{
    private int controlID;
    private string answer;

    public GameStep(int controlID, string answer)
    {
        this.controlID = controlID;
        this.answer = answer;
    }

    public int GetControlID()
    {
        return controlID;
    }

    public bool CheckStep(string controlValue)
    {
        return controlValue == answer;   
    }
}

// This class contains the title and steps for a game instruction
public class GameInstruction
{
    private string instructionTitle;
    private List<GameStep> instructionSteps = new List<GameStep>();

    public GameInstruction(string instructionTitle)
    {
        this.instructionTitle = instructionTitle;
    }

    public string GetTitle()
    {
        return instructionTitle;
    }

    public void AddStep(int controlID, string answer)
    {
        instructionSteps.Add(new GameStep(controlID, answer));
    }

    public int GetStepCount()
    {
        return instructionSteps.Count;
    }
    public int GetStepControl(int stepID)
    {
        return instructionSteps[stepID].GetControlID();
    }

    public bool CheckStep(int stepID, string controlValue)
    {
        return instructionSteps[stepID].CheckStep(controlValue);
    }
}

public class game_controller : MonoBehaviour
{
    public static game_controller gameInstance;
    private ui_controller uiController = ui_controller.uiInstance;

    public int gameTimer;

    private bool gameOver = false;

    private int currentInstruction = 0;
    private List<GameInstruction> gameInstructions = new List<GameInstruction>();

    void Start()
    {

        if (gameInstance == null) { gameInstance = this; }

        gameInstructions.Add(new GameInstruction("DISABLE AUTOMATIC VACUUM PUMPS"));
        gameInstructions[0].AddStep(0, "111");

        gameInstructions.Add(new GameInstruction("ACTIVATE STELLAR TRIANGULATION MATRIX"));
        gameInstructions[1].AddStep(0, "222");

        gameInstructions.Add(new GameInstruction("JETISON EMERGENCY PUPPIES"));
        gameInstructions[2].AddStep(0, "333");

        gameInstructions.Add(new GameInstruction("FIRE RETRO THRUSTERS"));
        gameInstructions[3].AddStep(0, "444");

        gameInstructions.Add(new GameInstruction("REACTIVATE ENGINES"));
        gameInstructions[4].AddStep(0, "555");

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

    public void ConfirmSteps()
    {
        bool instructionSucceeded = true;

        // Disable UI Controls

        GameInstruction current = gameInstructions[currentInstruction];

        // Loop through each step in the instruction and update the succeeded value.
        for (int i = 0; i < current.GetStepCount(); i++)
        {
            instructionSucceeded = current.CheckStep(i, ui_controller.uiInstance.GetControlValue(current.GetStepControl(i)));
            if(!instructionSucceeded) { break; }
        }

        if (instructionSucceeded) 
        {
            uiController.AddComputerLine("", true);
            uiController.AddComputerLine("CORRECT", true);
            NextInstruction();
        }
        else
        {
            uiController.AddComputerLine(" INCORRECT", true);
            uiController.AddComputerLine("\t WAITING FOR INPUT >", false);
        }

        // Reactivate the UI
    }

    private void NextInstruction()
    {
        currentInstruction++;

        if (currentInstruction >= gameInstructions.Count)
        {
            // GAME OVER WIN!!
        }
        else
        {
            uiController.AddComputerLine((currentInstruction + 1) + " - " + gameInstructions[currentInstruction].GetTitle(), true);
            uiController.AddComputerLine("\t WAITING FOR INPUT >", false);
        }
    }

}
