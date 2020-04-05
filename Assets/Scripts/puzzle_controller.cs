using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

[XmlRoot("instructionCollection")]
public class InstructionContainer
{
    [XmlArray("instructions"), XmlArrayItem("instruction")]
    public List<Instruction> instructions = new List<Instruction>();

    public string FindReverseTitle(string instructionTitle)
    {
        for(int i = 0; i < instructions.Count; i++)
        {
            if(instructions[i].title == instructionTitle)
            {
                return instructions[i].reverseTitle;
            }
        }

        return "";
    }
}
public class Instruction
{
    public string order;
    public string title;
    public bool reversible;
    public string reverseTitle;
    public string instructionHTML;

    [XmlArray("steps"), XmlArrayItem("step")]
    public List<Step> steps = new List<Step>();

    [XmlArray("loadTriggers"), XmlArrayItem("loadTrigger")]
    public List<InstructionTrigger> loadTriggers = new List<InstructionTrigger>();

    [XmlArray("successTriggers"), XmlArrayItem("successTrigger")]
    public List<InstructionTrigger> successTriggers = new List<InstructionTrigger>();
    
    public string[] GetStepControlTypes(int stepID)
    {
        string controlList = "";

        for(int i = 0; i < steps[stepID].stepControls.Count; i++)
        {
            if (controlList != "") controlList += ",";
            controlList += steps[stepID].stepControls[i].controlType;
        }

        return controlList.Split(',');
    }

    public string[] GetDependantStepControlTypes(int stepID)
    {
        string controlList = "";

        for (int i = 0; i < steps[stepID].dependantControls.Count; i++)
        {
            if (controlList != "") controlList += ",";
            controlList += steps[stepID].dependantControls[i].controlType;
        }

        return controlList.Split(',');
    }
}
public class Step
{
    [XmlArray("stepControls"), XmlArrayItem("stepControl")]
    public List<StepControl> stepControls = new List<StepControl>();
    public string answerType;
    public string answer;
    public string dependantType;
    [XmlArray("dependantControls"), XmlArrayItem("stepControl")]
    public List<StepControl> dependantControls = new List<StepControl>();
    public string defaultValue;
    public bool reverseControl;
}
public class StepControl
{
    public string controlType;
}
public class InstructionTrigger
{
    public string function;
}

public class GameStep
{
    protected int controlID;
    protected string answer;
    protected bool reversible;

    public GameStep(int controlID)
    {
        Debug.Log("ADDING STEP WITH control ID " + controlID);
        this.controlID = controlID;
        this.answer = ui_controller.uiInstance.GetControlRandomAnswer(controlID);
        Debug.Log("ANSWER IS - " + answer);

    }
    public GameStep(int controlID, string answer)
    {
        this.controlID = controlID;
        this.answer = answer;
    }
    public GameStep(int controlID, string answer, bool reversible)
    {
        this.controlID = controlID;
        this.answer = answer;
        this.reversible = reversible;
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
    public string GetAnswer()
    {
        return answer;
    }
    public void SetReversible(bool reversible)
    {
        this.reversible = reversible;
    }
    public bool GetReversible()
    {
        return reversible;
    }
    virtual public bool CheckStep(string controlValue)
    {
        return controlValue == answer;
    }
    virtual public void AddAnswers(string answer1, string answer2)
    {

    }
    virtual public int GetDependantControlID()
    {
        return -1;
    }
    virtual public string GetDependantType()
    {
        return "NONE";
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
    private string dependantType;
    private List<DependantValues> answerMapping = new List<DependantValues>();

    public DependantGameStep(int controlID, int dependantControlID, string dependantType) : base(controlID, "")
    {
        this.dependantControlID = dependantControlID;
        this.dependantType = dependantType;
    }

    override public string GetDependantType()
    {
        return dependantType;
    }
    override public int GetDependantControlID()
    {
        return dependantControlID;
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
        Debug.Log("ADDING STEP WITH control ID " + controlID);
        instructionSteps.Add(new GameStep(controlID));
    }
    public void AddStep(int controlID, string answer)
    {
        instructionSteps.Add(new GameStep(controlID, answer));
    }
    public void AddStep(int controlID, string answer, bool reversible)
    {
        instructionSteps.Add(new GameStep(controlID, answer, reversible));
    }
    public int AddStep(int controlID1, int controlID2, string dependantType)
    {
        instructionSteps.Add(new DependantGameStep(controlID1, controlID2, dependantType));
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
    public void AddDependantSteps(int controlID1, int controlID2, string dependantType)
    {
        int control2Range = ui_controller.uiInstance.GetControlRange(controlID2);

        switch (dependantType)
        {
            case "MAPPING_UP":
                GenerateMappingDependancy(controlID1, controlID2, false);
                break;
            case "MAPPING_DOWN":
                GenerateMappingDependancy(controlID1, controlID2, true);
                break;
            case "RANGE":
                GenerateRangeDependancy(controlID1, controlID2);
                break;
            case "RANDOM":
                GenerateRandomDependancy(controlID1, controlID2);
                break;
            case "REPLACEMENT":
                GenerateReplacemnentDependancy(controlID1, controlID2);
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
    public string GetAnswer(int stepID)
    {
        return instructionSteps[stepID].GetAnswer();
    }
    public bool CheckStep(int stepID, string controlValue)
    {
        return instructionSteps[stepID].CheckStep(controlValue);
    }
    public void AddSuccessTrigger(string successMethod)
    {
        string[] successTrigger = new string[] { successMethod };
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
            foundMethod.Invoke(this, new object[] { });
        }
    }
    public void HideNavTrigger()
    {
        puzzle_controller.puzzleInstance.HideNav();
    }
    public void ShowNavTrigger()
    {
        puzzle_controller.puzzleInstance.ShowNav();
    }
    private void GenerateMappingDependancy(int controlID1, int controlID2)
    {
        int dependantStepID = AddStep(controlID1, controlID2, "MAPPING_UP");

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
            AddDependantAnswer(dependantStepID, answer1.ToString(), answer2.ToString());
        }
    }
    private void GenerateMappingDependancy(int controlID1, int controlID2, bool reverse)
    {
        int dependantStepID = AddStep(controlID1, controlID2, reverse ? "MAPPING_DOWN" : "MAPPING_UP");

        int control1min = ui_controller.uiInstance.GetControlMinValue(controlID1);
        int control1max = ui_controller.uiInstance.GetControlMaxValue(controlID1);
        int control2min = ui_controller.uiInstance.GetControlMinValue(controlID2);
        int control2max = ui_controller.uiInstance.GetControlMaxValue(controlID2);


        for (int value1 = 0; value1 < control1max; value1++)
        {
            int answer1 = control1min + value1;
            int answer2 = 0;

            if (reverse)
            {
                answer2 = control2max - value1;
            }
            else
            {
                answer2 = control2min + value1;
            }
            AddDependantAnswer(dependantStepID, answer1.ToString(), answer2.ToString());
        }
    }
    private void GenerateRangeDependancy(int controlID1, int controlID2)
    {
        int dependantStepID = AddStep(controlID1, controlID2, "RANGE");

        int control1min = ui_controller.uiInstance.GetControlMinValue(controlID1);
        int control1max = ui_controller.uiInstance.GetControlMaxValue(controlID1);
        int control2min = ui_controller.uiInstance.GetControlMinValue(controlID2);
        int control2max = ui_controller.uiInstance.GetControlMaxValue(controlID2);

        int pivot = Random.Range(control2min, control2max) + 1;

        instructionSteps[dependantStepID].SetAnswer(pivot.ToString());

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

            AddDependantAnswer(dependantStepID, answer1.ToString(), value.ToString());
        }
    }
    private void GenerateRandomDependancy(int controlID1, int controlID2)
    {

        int dependantStepID = AddStep(controlID1, controlID2, "RANDOM");

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


            AddDependantAnswer(dependantStepID, answer1, value.ToString());
        }
    }
    private void GenerateReplacemnentDependancy(int controlID1, int controlID2)
    {

        int dependantStepID = AddStep(controlID1, controlID2, "REPLACEMENT");

        int control2min = ui_controller.uiInstance.GetControlMinValue(controlID2);
        int control2max = ui_controller.uiInstance.GetControlMaxValue(controlID2);

        string answer = ui_controller.uiInstance.GetControlRandomAnswer((controlID1));

        int replacementChar = Random.Range(0, answer.Length);
        string frontTest = answer.Substring(0, replacementChar);
        string frontWX = frontTest + "X";
        string rearTest = "";
        if (frontWX.Length <= 2) rearTest = answer.Substring(replacementChar + 1, answer.Length - (replacementChar + 1));

        instructionSteps[dependantStepID].SetAnswer(frontTest + "X" + rearTest);

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
            

            AddDependantAnswer(dependantStepID, answer1, value.ToString());
        }
    }
    public int GetDependantControlID(int stepID)
    {
        return instructionSteps[stepID].GetDependantControlID();
    }
    public GameStep GetInstructionStep(int stepID)
    {
        return instructionSteps[stepID];
    }
}

public struct Engine
{
    public string engineType;
    public int[] engineNo;

    public Engine(string engineType, int[] engineNos)
    {
        this.engineType = engineType;
        this.engineNo = engineNos;
    }

    public int GetEngineNoCount()
    {
        return engineNo.Length;
    }

    public int GetEngineNo(int engineNoIndex)
    {
        return engineNo[engineNoIndex];
    }

    public string GetEngineType()
    {
        return engineType;
    }
}
public struct NavSystem
{
    public string navSystemName;
    public string[] navPlanets;

    public NavSystem(string navSystemName, string[] navPlanets)
    {
        this.navSystemName = navSystemName;
        this.navPlanets = navPlanets;
    }

    public int GetNavPlanetsCount()
    {
        return navPlanets.Length;
    }

    public string GetNavSystemName()
    {
        return navSystemName;
    }

    public string GetNavPlanetName(int navPlanetsID)
    {
        return navPlanets[navPlanetsID];
    }
}
public struct Color
{
    public string color;
    public string warningLevel;
    public string number;
    public Color(string color, string warningLevel, string number)
    {
        this.color = color;
        this.warningLevel = warningLevel;
        this.number = number;
    }

    public string GetWarningLevel()
    {
        return warningLevel;
    }
}

public class puzzle_controller : MonoBehaviour
{
    public static puzzle_controller puzzleInstance;

    private InstructionContainer fileInstructions;
    private List<int> selectedInstructions = new List<int>();

    private List<GameInstruction> gameInstructions = new List<GameInstruction>();

    private string modelNo;

    private List<Engine> engineList = new List<Engine>();
    private int engineID, engineNoID;

    private List<NavSystem> navSystemList = new List<NavSystem>();
    private int navSystemID, navPlanetID;

    private List<Color> colorList = new List<Color>();

    private List<int> controlValueSet = new List<int>();

    private void Awake()
    {
        if (puzzleInstance == null) { puzzleInstance = this; }
    }
    
    private void Start()
    {
        Debug.Log("LOADING XML INSTRUCTION FILE");
        LoadInstructionsXML();

        Debug.Log("CHOOSING INSTRUCTIONS FOR PUZZLE");
        ChooseInstructions();

        Debug.Log("GENERATING MODEL NUMBER");
        modelNo = GenerateModelNo();
        ui_controller.uiInstance.SetScreenModelText(modelNo);

        Debug.Log("GENERATING ENGINE DATA");
        InitializeEngineList();
        InitializeEngines();
        ui_controller.uiInstance.SetScreenEngineText(engineList[engineID].GetEngineType());
        ui_controller.uiInstance.SetScreenEngineNoText(engineList[engineID].GetEngineNo(engineNoID).ToString());

        Debug.Log("GENERATING NAV DATA");
        InitializeNavSystemList();
        InitializeNavSystem();
        ShowNav();

        Debug.Log("GENERATING COLOR DATA");
        InitializeColorList();


        StartCoroutine(CreateInstructions());
    }

    IEnumerator CreateInstructions()
    {
        List<int> controlIDs = new List<int>();

        Debug.Log("CREATING GAME INSTRUCTIONS");
        for (int i = 0; i < selectedInstructions.Count; i++)
        {
            Debug.Log("     CREATING GAME INSTRUCTION " + i);
            int currentInstruction = selectedInstructions[i];
            string instructionTitle = fileInstructions.instructions[currentInstruction].title;
            Debug.Log("         GAME INSTRUCTION - " + instructionTitle);
            gameInstructions.Add(new GameInstruction(instructionTitle));


            if (i >= 2) controlIDs.Clear();

            Debug.Log("         CREATING GAME INSTRUCTION STEPS - " + fileInstructions.instructions[currentInstruction].steps.Count + " TOTAL");
    
            for (int j = 0; j < fileInstructions.instructions[currentInstruction].steps.Count; j++)
            {
                yield return null;

                Debug.Log("             GAME INSTRUCTION STEP - " + j);
                int instructionControlID = ui_controller.uiInstance.GetRandomControlOfType(fileInstructions.instructions[currentInstruction].GetStepControlTypes(j), controlIDs);
                Debug.Log("             GAME INSTRUCTION STEP CONTROL " + instructionControlID);
                string answerType = fileInstructions.instructions[currentInstruction].steps[j].answerType;
                Debug.Log("             GAME INSTRUCTION STEP ANSWER TYPE " + answerType);
                controlIDs.Add(instructionControlID);

                switch (answerType)
                {
                    case "FIXED":
                        Debug.Log("             CREATING FIXED ANSWER TYPE");
                        string insturctionControlAnswer = fileInstructions.instructions[currentInstruction].steps[j].answer;

                        bool reversible = fileInstructions.instructions[currentInstruction].steps[j].reverseControl;
                        Debug.Log("                  IS THIS CONTROL REVERSIBLE? " + reversible);

                        Debug.Log("                 ANSWER - " + insturctionControlAnswer);
                        gameInstructions[i].AddStep(instructionControlID, insturctionControlAnswer, reversible);

                        Debug.Log("                 CHECKING FOR DEFAULT VALUE");
                        if (fileInstructions.instructions[currentInstruction].steps[j].defaultValue != "" && !controlValueSet.Contains(instructionControlID))
                        {
                            string defaultValue = fileInstructions.instructions[currentInstruction].steps[j].defaultValue;
                            ui_controller.uiInstance.SetControlValue(instructionControlID, defaultValue);
                            Debug.Log("                     SETTING CONTROL " + instructionControlID + " TO " + defaultValue);
                            controlValueSet.Add(instructionControlID);
                        }
                        break;
                    case "DEPENDANT":
                        Debug.Log("             CREATING DEPENDANT ANSWER TYPE");
                        int instructionDependantControlID = ui_controller.uiInstance.GetRandomControlOfType(fileInstructions.instructions[currentInstruction].GetDependantStepControlTypes(j), controlIDs);
                        Debug.Log("                 DEPENDANT CONTROL - " + instructionDependantControlID);
                        controlIDs.Add(instructionDependantControlID);
                        string instructionDependantType = fileInstructions.instructions[currentInstruction].steps[j].dependantType;
                        Debug.Log("                 DEPENDANT TYPE - " + instructionDependantType);
                        gameInstructions[i].AddDependantSteps(instructionControlID, instructionDependantControlID, instructionDependantType);
                        break;
                    case "RANDOM":
                        Debug.Log("             CREATING RANDOM ANSWER TYPE");
                        gameInstructions[i].AddStep(instructionControlID);
                        break;
                    default:
                        break;
                }

                yield return null;
            }

            Debug.Log("   CREATING SUCESS TRIGGERS");
            for (int j = 0; j < fileInstructions.instructions[currentInstruction].successTriggers.Count; j++)
            {
                string successFunction = fileInstructions.instructions[currentInstruction].successTriggers[j].function;
                gameInstructions[i].AddSuccessTrigger(successFunction);
            }

            foreach (int x in controlIDs)
            {
                Debug.Log("Control ID Used - " + x);
            }

            yield return null;
        }


        // Set all other controls to random values
        for (int i = 0; i < ui_controller.uiInstance.GetControlsCount(); i++)
        {
            string controlType = ui_controller.uiInstance.GetControlType(i);

            switch (controlType)
            {
                case "button":
                case "switch":
                case "knob":
                case "slider":
                case "light":
                    if (!controlValueSet.Contains(i))
                    {
                        string value = ui_controller.uiInstance.GetControlRandomAnswer(i);
                        ui_controller.uiInstance.SetControlValue(i, value);
                        controlValueSet.Add(i);
                    }
                    break;
                case "meter":
                    ui_controller.uiInstance.SetControlValue(i, "0");
                    break;
                default:
                    break;
            }
        }

        AddFinalStep();

        Debug.Log("     SETTING METER NAME");
        ui_controller.uiInstance.SetControlLabel(2, "JAM LEVELS");

        manual_controller.manualInstance.CreateManual();
    }

    private void AddFinalStep()
    {
        List<int> controlIDs = new List<int>();

        Debug.Log("         CREATING GAME INSTRUCTION FINAL STEP");

        gameInstructions.Add(new GameInstruction("REACTIVATE ENGINES"));

        for(int i = 0; i < 2; i++)
        {
            for (int j = 0; j < gameInstructions[i].GetStepCount(); j++)
            {
                GameStep currentStep = gameInstructions[i].GetInstructionStep(j);

                int controlID = currentStep.GetControlID();
                controlIDs.Add(controlID);
                string depedantType = currentStep.GetDependantType();
                Debug.Log("             GAME INSTRUCTION STEP CONTROL " + controlID);
                Debug.Log("             GAME INSTRUCTION STEP ANSWER TYPE " + depedantType);
                if (depedantType == "NONE")
                {
                    string answer = currentStep.GetAnswer();
                    string controlType = ui_controller.uiInstance.GetControlType(controlID);
                    
                    Debug.Log("             GAME INSTRUCTION STEP CONTROL TYPE  " + controlType);
                    Debug.Log("             ORIGINAL  ANSWER - " + answer);
                    Debug.Log("             REVERSIBLE? - " + currentStep.GetReversible());
                    if (currentStep.GetReversible())
                    {
                        switch (controlType)
                        {
                            case "button":
                            case "switch":
                                if (answer == "0")
                                {
                                    answer = "1";
                                }
                                else
                                {
                                    answer = "0";
                                }
                                break;
                            case "slider":
                            case "knob":
                                answer = (6 - int.Parse(answer)).ToString();
                                break;
                        }
                    }

                    Debug.Log("                 ANSWER - " + answer);
                    gameInstructions[5].AddStep(controlID, answer);
                }
                else
                {
                    int dependantControlID = currentStep.GetDependantControlID();
                    controlIDs.Add(dependantControlID);
                    gameInstructions[5].AddDependantSteps(controlID, dependantControlID, depedantType);
                }
            }
        }

        Debug.Log("      ADDING EMPTY METER STEP");
        gameInstructions[5].AddStep(2, "0");
        controlIDs.Add(2);
        Debug.Log("      ADDING KEYPAD STEP");
        gameInstructions[5].AddStep(6);
        controlIDs.Add(6);

        Debug.Log("      CREATING LIGHT RELATIONSHIP");
        int lightControlRelID = ui_controller.uiInstance.GetRandomControlOfType(new string[] { "switch", "button" });
        Debug.Log("      LIGHT RELATING TO " + lightControlRelID);
        ui_controller.uiInstance.SetConnectedControls(lightControlRelID, 4, "random");

        Debug.Log("      CREATING METER RELATIONSHIP");
        int meterControlRelID = ui_controller.uiInstance.GetRandomControlOfType(new string[] { "slider", "knob" }, controlIDs);
        Debug.Log("      METER RELATING TO " + meterControlRelID);
        ui_controller.uiInstance.SetConnectedControls(meterControlRelID, 2, "mapped");
    }

    private void InitializeEngineList()
    {
        engineList.Add(new Engine("H3 THRUST", new int[] { 2, 3 }));
        engineList.Add(new Engine("PULSE ION", new int[] { 1, 2, 4 }));
        engineList.Add(new Engine("FUSION DRIVE", new int[] { 1, 2, 4 }));
    }

    private void InitializeEngines()
    {
        engineID = Random.Range(0, engineList.Count);
        engineNoID = Random.Range(0, engineList[engineID].GetEngineNoCount());
    }

    private void InitializeNavSystemList()
    {
        navSystemList.Add(new NavSystem("ARCTURUS", new string[] { "GREGORIA", "PRIME", "NOCTURNUS", "FRANCE" }));
        navSystemList.Add(new NavSystem("POLLUX ", new string[] { "ALPHA II", "ALPHA III", "ALPHA IV", "ALPHA VI" }));
        navSystemList.Add(new NavSystem("CENTAURI ", new string[] { "YANCY", "ABOBO", "EARTH 2", "TIBERIUS" }));
    }

    private void InitializeNavSystem()
    {
        navSystemID = Random.Range(0, navSystemList.Count);
        navPlanetID = Random.Range(0, navSystemList[navSystemID].GetNavPlanetsCount());
    }

    private void InitializeColorList()
    {
        colorList.Add(new Color("PURPLE", "NONE", "1"));
        colorList.Add(new Color("BLUE", "LOW", "2"));
        colorList.Add(new Color("GREEN", "MEDIUM", "3"));
        colorList.Add(new Color("YELLOW", "HIGH", "4"));
        colorList.Add(new Color("RED", "CRITICAL", "5"));
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

    public int GetEngineCount()
    {
        return engineList.Count;
    }

    public int GetEngineNoCount(int engineID)
    {
        return engineList[engineID].GetEngineNoCount();
    }

    public string GetEngineNo(int engineID, int engineNoID)
    {
        return engineList[engineID].GetEngineNo(engineNoID).ToString();
    }

    public bool EngineMatch(int engineID, int engineNoID)
    {
        return (this.engineID == engineID && this.engineNoID == engineNoID);
    }

    public string GetCurrentEngineNo()
    {
        return GetEngineNo(engineID, engineNoID);
    }

    public string GenerateModelNo()
    {
        string modelNo = "";
        int letterCount = 0;

        for(int i = 0; i < 5; i++)
        {
            switch(i)
            {
                case 0:
                case 4:
                    modelNo += Random.Range(0, 10).ToString();
                    break;
                case 1:
                case 2:
                case 3:
                    if(letterCount < 2)
                    {
                        modelNo += (char)('A' + Random.Range(0, 26));
                        letterCount++;
                    }
                    else
                    {
                        if(Random.Range(0, 2) == 0)
                        {
                            modelNo += (char)('A' + Random.Range(0, 26));
                            letterCount++;
                        }
                        else
                        {
                            modelNo += Random.Range(0, 10).ToString();
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        return modelNo;
    }

    public string GetModelNo()
    {
        return modelNo;
    }

    public int GetModelNoLetterCount()
    {
        int letterCount = 0;

        for(int i = 0; i < modelNo.Length; i++)
        {
            char modelLetter = modelNo[i];
            if ((int)modelLetter >= (int)'A' && (int)modelLetter <= (int)'Z')
            {
                letterCount++;
            }
        }

        return letterCount;
    }

    public string GetColorWarningLevel(string colorValue)
    {
        return colorList[int.Parse(colorValue) - 1].GetWarningLevel();
    }

    public string GetCurrentNavSystem()
    {
        return navSystemList[navSystemID].GetNavSystemName();
    }

    public int GetNavSystemCount()
    {
        return navSystemList.Count;
    }

    public void ShowNav()
    {
        ui_controller.uiInstance.SetScreenSystemText(puzzle_controller.puzzleInstance.GetCurrentNavSystem());
        ui_controller.uiInstance.SetScreenPlanetText(puzzle_controller.puzzleInstance.GetCurrentNavPlanet());
    }

    public void HideNav()
    {
        ui_controller.uiInstance.SetScreenSystemText("");
        ui_controller.uiInstance.SetScreenPlanetText("");
    }

    public int GetCurrentNavSystemID()
    {
        return navSystemID;
    }

    public int GetCurrentNawSystemPlanetCount(int navSystemID)
    {
        return navSystemList[navSystemID].GetNavPlanetsCount();
    }
    
    public string GetCurrentNavPlanet()
    {
        return navSystemList[navSystemID].GetNavPlanetName(navPlanetID);
    }

    public bool NavSystemMatch(int navSystemID, int navPlanetID)
    {
        return (this.navSystemID == navSystemID && this.navPlanetID == navPlanetID);
    }

    public void LoadInstructionsXML()
    {
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(InstructionContainer));
        FileStream readStream = new FileStream(@".\Assets\XML\/mooc_Instructions.xml", FileMode.Open);
        fileInstructions = xmlSerializer.Deserialize(readStream) as InstructionContainer;
        readStream.Close();
    }

    public void ChooseInstructions()
    {
        for(int i = 0; i < 5; i++)  // 5 here would be replaced by difficulty
        {
            int randomInstructionID = -1;

            do
            {
                randomInstructionID = UnityEngine.Random.Range(0, fileInstructions.instructions.Count);
            } while (fileInstructions.instructions[randomInstructionID].order != i.ToString());

            selectedInstructions.Add(randomInstructionID);
        }
    }

    public int GetSelectedInstructionCount()
    {
        return selectedInstructions.Count;
    }

    public string GetSelectedInstructionTitle(int instructionID)
    {
        return fileInstructions.instructions[selectedInstructions[instructionID]].title;
    }

    public string GetSelectedInstuctionHTML(int instructionID)
    {
        return fileInstructions.instructions[selectedInstructions[instructionID]].instructionHTML;
    }

    public string FindInstructionReverseTitle(string instructionTitle)
    {
        return fileInstructions.FindReverseTitle(instructionTitle);
    }
}
