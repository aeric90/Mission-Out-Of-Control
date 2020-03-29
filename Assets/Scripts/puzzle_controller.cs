using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.IO;
using System.Xml;


public class FileInstructions
{
    private string title;
    private int difficulty;
    private FileSteps[] fileSteps;
    private FileSuccessTriggers[] fileSucessTriggers;
}

public class FileSteps
{
    private string[] controls;
    private string defaultAnswer;
    private string answerType;
    private string answerValue;
}

public class FileSuccessTriggers
{
    private string function;
    private string parameter;
}

// This class contains the relevant control and the expected answer
public class GameStep
{
    protected int controlID;
    protected string answer;

    public GameStep(int controlID)
    {
        this.controlID = controlID;
        this.answer = ui_controller.uiInstance.GetControlRandomAnswer(controlID);

        Debug.Log(controlID + " " + answer);
    }
    public GameStep(int controlID, string answer)
    {
        this.controlID = controlID;
        this.answer = answer;

        Debug.Log(controlID + " " + this.answer);
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

    public DependantGameStep(int controlID, int dependantControlID) : base(controlID, "")
    {
        this.dependantControlID = dependantControlID;
    }

    override public void AddAnswers(string answer1, string answer2)
    {
        answerMapping.Add(new DependantValues(answer1, answer2));
    }
    override public bool CheckStep(string controlValue)
    {
        Debug.Log("CHECKING DEPENDANT STEP");
        Debug.Log("Value of source control " + controlID + " =  " + controlValue);
        string dependantControlValue = ui_controller.uiInstance.GetControlValue(dependantControlID);
        Debug.Log("Value of target control " + dependantControlID + " =  " + dependantControlValue);
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
    public void AddStep(int controlID, string answer)
    {
        instructionSteps.Add(new GameStep(controlID, answer));
    }
    public int AddStep(int controlID1, int controlID2)
    {
        instructionSteps.Add(new DependantGameStep(controlID1, controlID2));
        return instructionSteps.Count - 1;
    }
    public void AddDependantSteps(int controlID1)
    {
        int controlID2 = 0;
        List<int> checkedControls = new List<int>();

        do
        {
            controlID2 = Random.Range(0, ui_controller.uiInstance.GetControlsCount());

            if(!checkedControls.Contains(controlID2))
            {
                if(controlID1 == controlID2 || !ui_controller.uiInstance.GetControlDependantTarget(controlID2))
                {
                    checkedControls.Add(controlID2);
                    if(checkedControls.Count >= ui_controller.uiInstance.GetControlsCount())
                    {
                        // No available control to depend on, generate an independant step.
                        AddStep(controlID1);
                        return;
                    }
                    controlID2 = -1;
                }
            }
            else
            {
                controlID2 = -1;
            }

        } while (controlID2 < 0);

        int control2Range = ui_controller.uiInstance.GetControlRange(controlID2);

        switch (ui_controller.uiInstance.GetControlRange(controlID1))
        {
            case 2:
                if(control2Range == 2)
                {
                    GenerateMappingDependancy(controlID1, controlID2);
                }
                else if(control2Range == 5)
                {
                    GenerateRangeDependancy(controlID1, controlID2);
                }
                else
                {
                    AddStep(controlID1);
                    return;
                }
                break;
            case 5:
                if (control2Range == 2)
                {
                    GenerateRandomDependancy(controlID1, controlID2);
                }
                else if (control2Range == 5)
                {
                    GenerateMappingDependancy(controlID1, controlID2);
                }
                else
                {
                    AddStep(controlID1);
                    return;
                }
                break;
            case 1000:
                if (control2Range == 2)
                {
                    GenerateRandomDependancy(controlID1, controlID2);
                }
                else if (control2Range == 5)
                {
                    GenerateReplacemnentDependancy(controlID1, controlID2);
                }
                else
                {
                    AddStep(controlID1);
                    return;
                }
                break;
        }
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
    private void GenerateMappingDependancy(int controlID1, int controlID2)
    {
        Debug.Log("MAPPING DEPENDANCY BEWTEEN " + controlID1 + " AND " + controlID2);
        int dependantStepID = AddStep(controlID1, controlID2);

        int direction = Random.Range(0, 2);

        int control1min = ui_controller.uiInstance.GetControlMinValue(controlID1);
        int control1max = ui_controller.uiInstance.GetControlMaxValue(controlID1);
        int control2min = ui_controller.uiInstance.GetControlMinValue(controlID2);
        int control2max = ui_controller.uiInstance.GetControlMaxValue(controlID2);


        for (int value1 = 0; value1 < control1max; value1++)
        {
            int answer1 = control1min + value1;
            int answer2 = 0;

            if(direction == 0)
            {
                answer2 = control2max - value1;
            }
            else
            {
                answer2 = control2min + value1;
            }
            Debug.Log("ADDING ANSWER WHERE " + answer1.ToString() + " = " + answer2.ToString());
            AddDependantAnswer(dependantStepID, answer1.ToString(), answer2.ToString());
        }
    }
    private void GenerateRangeDependancy(int controlID1, int controlID2)
    {
        Debug.Log("RANGE DEPENDANCY - " + controlID1 + " " + controlID2);
        int dependantStepID = AddStep(controlID1, controlID2);

        int control1min = ui_controller.uiInstance.GetControlMinValue(controlID1);
        int control1max = ui_controller.uiInstance.GetControlMaxValue(controlID1);
        int control2min = ui_controller.uiInstance.GetControlMinValue(controlID2);
        int control2max = ui_controller.uiInstance.GetControlMaxValue(controlID2);

        int pivot = Random.Range(control2min, control2max) + 1;

        for (int value = control2min; value <= (control2max - control2min) + 1; value++)
        {
            int answer1 = 0;

            if (value < pivot)
            {
                answer1 = control1min;
            }
            else
            {
                answer1 = control1max;
            }

            Debug.Log(answer1.ToString() + " " + value.ToString());
            AddDependantAnswer(dependantStepID, answer1.ToString(), value.ToString());
        }
    }
    private void GenerateRandomDependancy(int controlID1, int controlID2)
    {
        Debug.Log("RANDOM DEPENDANCY BETWEEN " + controlID1 + " AND " + controlID2);
        int dependantStepID = AddStep(controlID1, controlID2);

        int control2min = ui_controller.uiInstance.GetControlMinValue(controlID2);
        int control2max = ui_controller.uiInstance.GetControlMaxValue(controlID2);

        List<string> answerList = new List<string>();
        string answer1 = "";


        for (int value = control2min; value < (control2max - control2min) + 1; value++)
        {
            do
            {
                answerList.Add(answer1);
                answer1 = ui_controller.uiInstance.GetControlRandomAnswer((controlID1));
            } while (answerList.Contains(answer1));

            Debug.Log("ADDING ANSWER WHERE " + answer1.ToString() + " = " + value.ToString());
            AddDependantAnswer(dependantStepID, answer1, value.ToString());
        }
    }
    private void GenerateReplacemnentDependancy(int controlID1, int controlID2)
    {
        Debug.Log("REPLACEMENT DEPENDANCY - " + controlID1 + " " + controlID2);
        int dependantStepID = AddStep(controlID1, controlID2);

        int control2min = ui_controller.uiInstance.GetControlMinValue(controlID2);
        int control2max = ui_controller.uiInstance.GetControlMaxValue(controlID2);

        string answer = ui_controller.uiInstance.GetControlRandomAnswer((controlID1));

        int replacementChar = Random.Range(0, answer.Length);
                
        for (int value = control2min; value <= (control2max - control2min) + 1; value++)
        {
            string answer1 = "";
            for(int i = 0; i < answer.Length; i++)
            {
                if(i == replacementChar)
                {
                    answer1 += value.ToString();
                }
                else
                {
                    answer1 += answer[i];
                }
            }
            
            Debug.Log(answer1 + " " + value.ToString());
            AddDependantAnswer(dependantStepID, answer1, value.ToString());
        }
    }
}

public class puzzle_controller : MonoBehaviour
{
    public static puzzle_controller puzzleInstance;

    private List<GameInstruction> gameInstructions = new List<GameInstruction>();

    private string modelNo;
    private string engineType;
    private string engineNo;
    private string navSystem;
    private string navPlanet;

    private void Awake()
    {
        if (puzzleInstance == null) { puzzleInstance = this; }
    }
    private void Start()
    {
        engineType = "Pulse Ion";
        engineNo = "4";
        navSystem = "POLLUX";
        navPlanet = "ALPHA IV";

        ui_controller.uiInstance.SetScreenEngineText(engineType);
        ui_controller.uiInstance.SetScreenEngineNoText(engineNo);

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
        Debug.Log("INSTRUCTION 1");
        gameInstructions.Add(new GameInstruction("DISABLE AUTOMATIC VACUUM PUMPS"));
        gameInstructions[0].AddStep(3, "0");

        // 2nd Instruction
        Debug.Log("INSTRUCTION 2");
        gameInstructions.Add(new GameInstruction("ACTIVATE STELLAR TRIANGULATION MATRIX"));
        gameInstructions[1].AddStep(8);
        gameInstructions[1].AddDependantSteps(5);
        gameInstructions[1].AddSuccessTrigger("UpdateSystem", navSystem);
        gameInstructions[1].AddSuccessTrigger("UpdatePlanet", navPlanet);

        Debug.Log("INSTRUCTION 3");
        gameInstructions.Add(new GameInstruction("JETISON EMERGENCY PUPPIES"));
        gameInstructions[2].AddStep(7);
        gameInstructions[2].AddDependantSteps(6);
        gameInstructions[2].AddDependantSteps(1);

        Debug.Log("INSTRUCTION 4");
        gameInstructions.Add(new GameInstruction("FIRE RETRO THRUSTERS"));
        gameInstructions[3].AddStep(5);
        gameInstructions[3].AddStep(7);
        gameInstructions[3].AddDependantSteps(0);

        Debug.Log("INSTRUCTION 5");
        gameInstructions.Add(new GameInstruction("SET NAVIGATION COORDINATES"));
        gameInstructions[4].AddStep(1);
        gameInstructions[4].AddStep(9);
        gameInstructions[4].AddStep(6);

        Debug.Log("INSTRUCTION 6");
        gameInstructions.Add(new GameInstruction("REACTIVATE ENGINES"));
        gameInstructions[5].AddStep(6);
        gameInstructions[5].AddStep(3);
        gameInstructions[5].AddStep(8);
        gameInstructions[5].AddDependantSteps(5);
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

    public int GetGameStepCount(int instructionID)
    {
        return gameInstructions[instructionID].GetStepCount();
    }

    public GameInstruction GetGameInstruction(int instructionID)
    {
        return gameInstructions[instructionID];
    }

    public void XMLTest()
    {
        XmlDocument doc = new XmlDocument();
        doc.Load("/assets/instructions.xml");

        XmlNodeList nodeList;
        XmlNode root = doc.DocumentElement;


    }
}
