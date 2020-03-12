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
    public void SetControlID(int controlID)
    {
        this.controlID = controlID;
    }

    public void SetAnswer(string answer)
    {
        this.answer = answer;
    }

    public bool CheckStep(string controlValue)
    {
        return controlValue == answer;   
    }
}

public struct DependantValues
{
    public string answer1, answer2;

    public DependantValues(string answer1, string answer2)
    {
        this.answer1 = answer1;
        this.answer2 = answer2;
    }
}
public class DependantGameStep : GameStep
{
    private int dependantControlID;
    private List<DependantValues> answerButton = new List<DependantValues>();

    public DependantGameStep(int controlID, int dependantControlID) : base (controlID, "")
    {
        this.dependantControlID = dependantControlID;
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

    public int gameTimer;

    private bool gameOver = false;

    private int currentInstruction = 0;
    private List<GameInstruction> gameInstructions = new List<GameInstruction>();

    private void Awake()
    {
        if (gameInstance == null) { gameInstance = this; }
    }

    void Start()
    {
        ui_controller.uiInstance.SetControlValue(2, "0");
        ui_controller.uiInstance.SetControlLabel(2, "JAM LEVELS");

        ui_controller.uiInstance.SetConnectedControls(7, 2);

        ui_controller.uiInstance.SetControlValue(4, "2");

        gameInstructions.Add(new GameInstruction("DISABLE AUTOMATIC VACUUM PUMPS"));
        gameInstructions[0].AddStep(6, "111");

        gameInstructions.Add(new GameInstruction("ACTIVATE STELLAR TRIANGULATION MATRIX"));
        gameInstructions[1].AddStep(6, "222");
        gameInstructions[1].AddStep(8, "1");

        gameInstructions.Add(new GameInstruction("JETISON EMERGENCY PUPPIES"));
        gameInstructions[2].AddStep(6, "333");
        gameInstructions[2].AddStep(7, "3");

        gameInstructions.Add(new GameInstruction("FIRE RETRO THRUSTERS"));
        gameInstructions[3].AddStep(6, "444");
        gameInstructions[3].AddStep(0, "1");

        gameInstructions.Add(new GameInstruction("SET NAVIGATION COORDINATES"));
        gameInstructions[4].AddStep(1, "5");
        gameInstructions[4].AddStep(9, "2");
        gameInstructions[4].AddStep(6, "555");

        gameInstructions.Add(new GameInstruction("REACTIVATE ENGINES"));
        gameInstructions[5].AddStep(6, "666");
        gameInstructions[5].AddStep(8, "0");
        gameInstructions[5].AddStep(2, "4");

        StartCoroutine(CountDown());

        NextInstruction();
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

        ui_controller.uiInstance.EnableControls(false);

       GameInstruction current = gameInstructions[currentInstruction];

        // Loop through each step in the instruction and update the succeeded value.
        for (int i = 0; i < current.GetStepCount(); i++)
        {
            instructionSucceeded = current.CheckStep(i, ui_controller.uiInstance.GetControlValue(current.GetStepControl(i)));
            if(!instructionSucceeded) { break; }
        }

        if (instructionSucceeded) 
        {
            ui_controller.uiInstance.AddComputerLine(" CORRECT", false);
            currentInstruction++;
            NextInstruction();
        }
        else
        {
            ui_controller.uiInstance.AddComputerLine(" INCORRECT\n\t WAITING FOR INPUT >", false);
        }

        ui_controller.uiInstance.EnableControls(true);
    }

    private void NextInstruction()
    {

        if (currentInstruction >= gameInstructions.Count)
        {
            // GAME OVER WIN!!
        }
        else
        {
            if(currentInstruction > 0)
            {
                ui_controller.uiInstance.ClearComputer();
            }
            string outPutText;
            outPutText = (currentInstruction + 1) + " - " + gameInstructions[currentInstruction].GetTitle();
            outPutText += "\n";
            outPutText += "\t WAITING FOR INPUT >";
            ui_controller.uiInstance.AddComputerLine(outPutText, false);
        }
    }

    public bool GetGameOver()
    {
        return gameOver;
    }

}
