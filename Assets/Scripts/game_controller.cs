using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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

    virtual public bool CheckStep(string controlValue)
    {
        return controlValue == answer;   
    }

    virtual public void AddAnswers(string answer1, string answer2)
    {

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
    private List<DependantValues> answerMapping = new List<DependantValues>();

    public DependantGameStep(int controlID, int dependantControlID) : base (controlID, "")
    {
        this.dependantControlID = dependantControlID;
    }

    override public void AddAnswers(string answer1, string answer2)
    {
        answerMapping.Add(new DependantValues(answer1, answer2));
    }

    override public bool CheckStep(string controlValue)
    {
        string currentAnswer = ui_controller.uiInstance.GetControlValue(dependantControlID);
        string expectedAnswer = answerMapping.Find(x => x.answer1 == controlValue).answer2;

        return currentAnswer==expectedAnswer;
    }

}

// This class contains the title and steps for a game instruction
public class GameInstruction
{
    private string instructionTitle;
    private List<GameStep> instructionSteps = new List<GameStep>();
    private List<string[]> successTriggers = new List<string[]>();

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

    public int AddStep(int controlID1, int controlID2)
    {
        instructionSteps.Add(new DependantGameStep(controlID1, controlID2));
        return instructionSteps.Count - 1;
    }

    public void AddDependantAnswer(int stepID, string answer1, string answer2)
    {
        instructionSteps[stepID].AddAnswers(answer1, answer2);
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

    public void AddSuccessTrigger(string successMethod, string successParams)
    {
        string[] successTrigger = new string[] { successMethod, successParams };
        successTriggers.Add(successTrigger);
    }

    public bool CheckSuccessTrigger()
    {
        return successTriggers.Count > 0;
    }

    public void TriggerSuccess()
    {
        foreach (string[] trigger in successTriggers)
        {
            MethodInfo foundMethod = this.GetType().GetMethod(trigger[0]);
            foundMethod.Invoke(this, new object[] { trigger[1] });
        }
    }

    public void UpdateSystem(string systemText)
    {
        ui_controller.uiInstance.SetScreenSystemText(systemText);
    }

    public void UpdatePlanet(string planetText)
    {
        ui_controller.uiInstance.SetScreenPlanetText(planetText);
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
        int dependantStepID;

        ui_controller.uiInstance.SetControlValue(2, "0");
        ui_controller.uiInstance.SetControlLabel(2, "JAM LEVELS");

        ui_controller.uiInstance.SetConnectedControls(3, 4, "random");
        ui_controller.uiInstance.SetConnectedControls(7, 2, "mapped");

        ui_controller.uiInstance.SetControlValue(4, "3");

        gameInstructions.Add(new GameInstruction("DISABLE AUTOMATIC VACUUM PUMPS"));
        gameInstructions[0].AddStep(3, "0");

        gameInstructions.Add(new GameInstruction("ACTIVATE STELLAR TRIANGULATION MATRIX"));
        gameInstructions[1].AddStep(8, "1");
        dependantStepID = gameInstructions[1].AddStep(5, 4);
            gameInstructions[1].AddDependantAnswer(dependantStepID, "2", "1");
            gameInstructions[1].AddDependantAnswer(dependantStepID, "3", "2");
            gameInstructions[1].AddDependantAnswer(dependantStepID, "4", "3");
            gameInstructions[1].AddDependantAnswer(dependantStepID, "5", "4");
        gameInstructions[1].AddSuccessTrigger("UpdateSystem", "POLLUX");
        gameInstructions[1].AddSuccessTrigger("UpdatePlanet", "ALPHA IV");

        gameInstructions.Add(new GameInstruction("JETISON EMERGENCY PUPPIES"));
        gameInstructions[2].AddStep(7, "3");
        dependantStepID = gameInstructions[2].AddStep(6, 2);
        gameInstructions[2].AddDependantAnswer(dependantStepID, "497", "0");
        gameInstructions[2].AddDependantAnswer(dependantStepID, "512", "1");
        gameInstructions[2].AddDependantAnswer(dependantStepID, "244", "2");
        gameInstructions[2].AddDependantAnswer(dependantStepID, "919", "3");
        gameInstructions[2].AddDependantAnswer(dependantStepID, "711", "4");
        dependantStepID = gameInstructions[2].AddStep(1, 5);
        gameInstructions[2].AddDependantAnswer(dependantStepID, "1", "1");
        gameInstructions[2].AddDependantAnswer(dependantStepID, "2", "2");
        gameInstructions[2].AddDependantAnswer(dependantStepID, "3", "3");
        gameInstructions[2].AddDependantAnswer(dependantStepID, "4", "4");
        gameInstructions[2].AddDependantAnswer(dependantStepID, "5", "5");

        gameInstructions.Add(new GameInstruction("FIRE RETRO THRUSTERS"));

        gameInstructions[3].AddStep(0, "1");

        gameInstructions.Add(new GameInstruction("SET NAVIGATION COORDINATES"));
        gameInstructions[4].AddStep(1, "5");
        gameInstructions[4].AddStep(9, "2");
        gameInstructions[4].AddStep(6, "174");

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
            if (current.CheckSuccessTrigger()) current.TriggerSuccess();
            ui_controller.uiInstance.AddComputerLine(" CORRECT", false);
            StartCoroutine(PauseOnCorrect());
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
            gameOver = true;
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

    private IEnumerator PauseOnCorrect()
    {
        yield return new WaitForSeconds(3.00f);

        currentInstruction++;
        NextInstruction();

        StopCoroutine(PauseOnCorrect());
    }

    public bool GetGameOver()
    {
        return gameOver;
    }

}
