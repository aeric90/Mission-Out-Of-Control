using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

// This class contains the relevant control and the expected answer
public class GameStep
{
    private int controlID;
    private string answer;

    public GameStep(int controlID)
    {
        this.controlID = controlID;
        this.answer = ui_controller.uiInstance.GetControlRandomAnswer(controlID);
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

    public DependantGameStep(int controlID, int dependantControlID) : base(controlID)
    {
        this.dependantControlID = dependantControlID;
    }

    override public void AddAnswers(string answer1, string answer2)
    {
        answerMapping.Add(new DependantValues(answer1, answer2));
    }

    override public bool CheckStep(string controlValue)
    {
        string dependantControlValue = ui_controller.uiInstance.GetControlValue(dependantControlID);
        string expectedAnswer = answerMapping.Find(x => x.answer2 == dependantControlValue).answer1;
        return controlValue == expectedAnswer;
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
    public void AddStep(int controlID)
    {
        instructionSteps.Add(new GameStep(controlID));
    }
    public int AddStep(int controlID1, int controlID2)
    {
        instructionSteps.Add(new DependantGameStep(controlID1, controlID2));
        return instructionSteps.Count - 1;
    }

    public void AddDependantStep(int controlID)
    {
        int controlID2;

        do
        {
            controlID2 = Random.Range(0, ui_controller.uiInstance.GetControlsCount());

        } while (controlID != controlID2 && ui_controller.uiInstance.GetControlDependantTarget(controlID2));




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

public class puzzle_controller : MonoBehaviour
{
    public static puzzle_controller puzzleInstance;

    private List<GameInstruction> gameInstructions = new List<GameInstruction>();

    private void Awake()
    {
        if (puzzleInstance == null) { puzzleInstance = this; }
    }

    private void Start()
    {
        int dependantStepID;

        // Set METER conrol to a value of 0 and change the label to JAM LEVELS 
        ui_controller.uiInstance.SetControlValue(2, "0");
        ui_controller.uiInstance.SetControlLabel(2, "JAM LEVELS");

        // Sets a random relationship between SWITCH and LIGHT 
        ui_controller.uiInstance.SetControlValue(3, "1");
        ui_controller.uiInstance.SetConnectedControls(3, 4, "random");

        // Sets the defaults value of LIGHT to 3
        ui_controller.uiInstance.SetControlValue(4, "3");

        // Sets a mapped relationship between SLIDER 2 and METER
        ui_controller.uiInstance.SetConnectedControls(7, 2, "mapped");

        // 1st Instruction
        gameInstructions.Add(new GameInstruction("DISABLE AUTOMATIC VACUUM PUMPS"));
        // SWITCH must be set to 0
        gameInstructions[0].AddStep(3);

        // 2nd Instruction
        gameInstructions.Add(new GameInstruction("ACTIVATE STELLAR TRIANGULATION MATRIX"));
        // BUTTON 2 is set to 1
        gameInstructions[1].AddStep(8);
        // SLIDER 1 is set based on the value of LIGHT
        dependantStepID = gameInstructions[1].AddStep(5, 4);
        gameInstructions[1].AddDependantAnswer(dependantStepID, "2", "1");
        gameInstructions[1].AddDependantAnswer(dependantStepID, "3", "2");
        gameInstructions[1].AddDependantAnswer(dependantStepID, "4", "3");
        gameInstructions[1].AddDependantAnswer(dependantStepID, "5", "4");
        // Add functions to trigger when the instruction is completed correctly
        gameInstructions[1].AddSuccessTrigger("UpdateSystem", "POLLUX");
        gameInstructions[1].AddSuccessTrigger("UpdatePlanet", "ALPHA IV");

        gameInstructions.Add(new GameInstruction("JETISON EMERGENCY PUPPIES"));
        gameInstructions[2].AddStep(7);
        dependantStepID = gameInstructions[2].AddStep(6, 2);
        gameInstructions[2].AddDependantAnswer(dependantStepID, "490", "0");
        gameInstructions[2].AddDependantAnswer(dependantStepID, "491", "1");
        gameInstructions[2].AddDependantAnswer(dependantStepID, "492", "2");
        gameInstructions[2].AddDependantAnswer(dependantStepID, "493", "3");
        gameInstructions[2].AddDependantAnswer(dependantStepID, "494", "4");
        dependantStepID = gameInstructions[2].AddStep(1, 5);
        gameInstructions[2].AddDependantAnswer(dependantStepID, "1", "1");
        gameInstructions[2].AddDependantAnswer(dependantStepID, "2", "2");
        gameInstructions[2].AddDependantAnswer(dependantStepID, "3", "3");
        gameInstructions[2].AddDependantAnswer(dependantStepID, "4", "4");
        gameInstructions[2].AddDependantAnswer(dependantStepID, "5", "5");

        gameInstructions.Add(new GameInstruction("FIRE RETRO THRUSTERS"));
        gameInstructions[3].AddStep(5);
        gameInstructions[3].AddStep(7);
        dependantStepID = gameInstructions[3].AddStep(0, 4);
        gameInstructions[3].AddDependantAnswer(dependantStepID, "0", "1");
        gameInstructions[3].AddDependantAnswer(dependantStepID, "1", "2");
        gameInstructions[3].AddDependantAnswer(dependantStepID, "1", "3");
        gameInstructions[3].AddDependantAnswer(dependantStepID, "1", "4");

        gameInstructions.Add(new GameInstruction("SET NAVIGATION COORDINATES"));
        gameInstructions[4].AddStep(1);
        gameInstructions[4].AddStep(9);
        gameInstructions[4].AddStep(6);

        gameInstructions.Add(new GameInstruction("REACTIVATE ENGINES"));
        gameInstructions[5].AddStep(6);
        gameInstructions[5].AddStep(3);
        gameInstructions[5].AddStep(8);
        dependantStepID = gameInstructions[5].AddStep(5, 4);
        gameInstructions[5].AddDependantAnswer(dependantStepID, "2", "1");
        gameInstructions[5].AddDependantAnswer(dependantStepID, "3", "2");
        gameInstructions[5].AddDependantAnswer(dependantStepID, "4", "3");
        gameInstructions[5].AddDependantAnswer(dependantStepID, "5", "4");
        gameInstructions[5].AddStep(2);
    }

    public int GetGameInstructionCount()
    {
        return gameInstructions.Count;
    }

    public string GetGameInstructionTitle(int instructionID)
    {
        return gameInstructions[instructionID].GetTitle();
    }

    public GameInstruction GetGameInstruction(int instructionID)
    {
        return gameInstructions[instructionID];
    }
}
