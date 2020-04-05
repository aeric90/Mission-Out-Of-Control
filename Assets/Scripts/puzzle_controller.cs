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
}
public class Instruction
{
    public string order;
    public string title;
    public bool reversable;
    public string reverseTitle;
    public string instructionHTML;

    [XmlArray("steps"), XmlArrayItem("step")]
    public List<Step> steps = new List<Step>();

    [XmlArray("successTriggers"), XmlArrayItem("successTrigger")]
    public List<SuccessTrigger> successTriggers = new List<SuccessTrigger>();
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
}
public class StepControl
{
    public string controlType;
}
public class SuccessTrigger
{
    public string function;
}


public class GameStep
{
    protected int controlID;
    protected string answer;

    public GameStep(int controlID)
    {
        this.controlID = controlID;
        this.answer = ui_controller.uiInstance.GetControlRandomAnswer(controlID);
    }
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
    public string GetAnswer()
    {
        return answer;
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
    public void UpdateSystem()
    {
        ui_controller.uiInstance.SetScreenSystemText(puzzle_controller.puzzleInstance.GetCurrentNavSystem());
    }
    public void UpdatePlanet()
    {
        ui_controller.uiInstance.SetScreenPlanetText(puzzle_controller.puzzleInstance.GetCurrentNavPlanet());
    }
    private void GenerateMappingDependancy(int controlID1, int controlID2)
    {
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
            AddDependantAnswer(dependantStepID, answer1.ToString(), answer2.ToString());
        }
    }
    private void GenerateMappingDependancy(int controlID1, int controlID2, bool reverse)
    {
        int dependantStepID = AddStep(controlID1, controlID2);

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
        int dependantStepID = AddStep(controlID1, controlID2);

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


            AddDependantAnswer(dependantStepID, answer1, value.ToString());
        }
    }
    private void GenerateReplacemnentDependancy(int controlID1, int controlID2)
    {

        int dependantStepID = AddStep(controlID1, controlID2);

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

    private void Awake()
    {
        if (puzzleInstance == null) { puzzleInstance = this; }
    }
    
    private void Start()
    {
        LoadInstructionsXML();
        ChooseInstructions();

        modelNo = "3ZF94";  // Need to be able to generate these
        ui_controller.uiInstance.SetScreenModelText(modelNo);

        InitializeEngineList();
        InitializeEngines();
        ui_controller.uiInstance.SetScreenEngineText(engineList[engineID].GetEngineType());
        ui_controller.uiInstance.SetScreenEngineNoText(engineList[engineID].GetEngineNo(engineNoID).ToString());

        InitializeNavSystemList();
        InitializeNavSystem();

        InitializeColorList();

        ui_controller.uiInstance.SetControlValue(1, ui_controller.uiInstance.GetControlRandomAnswer(1));

        // Set METER conrol to a value of 0 and change the label to JAM LEVELS 
        ui_controller.uiInstance.SetControlValue(2, "0");
        ui_controller.uiInstance.SetControlLabel(2, "JAM LEVELS");

        // Sets a random relationship between SWITCH and LIGHT 
        ui_controller.uiInstance.SetControlValue(3, "1");
        ui_controller.uiInstance.SetConnectedControls(3, 4, "random");

        // Sets the defaults value of LIGHT to 3
        ui_controller.uiInstance.SetControlValue(4, ui_controller.uiInstance.GetControlRandomAnswer(4));

        // Sets a mapped relationship between SLIDER 2 and METER
        ui_controller.uiInstance.SetConnectedControls(7, 2, "mapped");

        ui_controller.uiInstance.SetControlValue(9, ui_controller.uiInstance.GetControlRandomAnswer(9));

        for(int i = 0; i < selectedInstructions.Count; i++)
        {

            int currentInstruction = selectedInstructions[i];

            string instructionTitle = fileInstructions.instructions[currentInstruction].title;
            gameInstructions.Add(new GameInstruction(instructionTitle));
                    
            for (int j = 0; j < fileInstructions.instructions[currentInstruction].steps.Count; j++)
            {
                int instructionControlID = ui_controller.uiInstance.GetRandomControlOfType(new string[] { fileInstructions.instructions[currentInstruction].steps[j].stepControls[0].controlType });
                string answerType = fileInstructions.instructions[currentInstruction].steps[j].answerType;

                switch (answerType)
                {

                case "FIXED":
                    string insturctionControlAnswer = fileInstructions.instructions[currentInstruction].steps[j].answer;
                    gameInstructions[i].AddStep(instructionControlID, insturctionControlAnswer);
                    break;
                case "DEPENDANT":
                    int instructionDependantControlID = ui_controller.uiInstance.GetRandomControlOfType(new string[] { fileInstructions.instructions[currentInstruction].steps[j].dependantControls[0].controlType });
                    string instructionDependantType = fileInstructions.instructions[currentInstruction].steps[j].dependantType;
                    gameInstructions[i].AddDependantSteps(instructionControlID, instructionDependantControlID, instructionDependantType);
                    break;
                case "RANDOM":
                    gameInstructions[i].AddStep(instructionControlID);
                    break;
                default:
                    break;
                }
            }

            for (int j = 0; j < fileInstructions.instructions[currentInstruction].successTriggers.Count; j++)
            {
                string successFunction = fileInstructions.instructions[currentInstruction].successTriggers[j].function;
                gameInstructions[i].AddSuccessTrigger(successFunction);
            }
        }

        gameInstructions.Add(new GameInstruction("REACTIVATE ENGINES"));
        gameInstructions[5].AddStep(3, "1");
        gameInstructions[5].AddDependantSteps(5, 4, "mapping_up");
        gameInstructions[5].AddStep(8, "0");
        gameInstructions[5].AddStep(2, "0");
        gameInstructions[5].AddStep(6);

        manual_controller.manualInstance.CreateManual();
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

    public string GetModelNo()
    {
        return modelNo;
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
}
