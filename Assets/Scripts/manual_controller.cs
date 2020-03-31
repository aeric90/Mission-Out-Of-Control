﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml;
using HtmlAgilityPack;

public struct ManualError
{
	private int instructionID;
	private int stepID;

	public ManualError(int instructionID, int stepID)
	{
		this.instructionID = instructionID;
		this.stepID = stepID;
	}

	public bool ErrorMatch(int instructionID, int stepID)
	{
		return (this.instructionID == instructionID && this.stepID == stepID);
	}
}
public class manual_controller : MonoBehaviour
{
	private List<ManualError> manualErrors = new List<ManualError>();
	private HtmlDocument manualTemplate;

	private void Start()
	{
		GenerateErrorList();

		manualTemplate = new HtmlDocument();
		manualTemplate.Load(@".\Assets\HTML\Beta Test Manual.html");


		string path = @".\Assets\HTML\manual_output.test.html";

		PopulateInstruction0();
		PopulateInstruction1();
		PopulateInstruction2();
		PopulateInstruction3();
		PopulateInstruction4();
		PopulateInstruction5();

		manualTemplate.Save(path);

		/*
		var test = doc.DocumentNode.SelectSingleNode("//div").Attributes["id"].Value;		   
		Debug.Log(test);
		*/
	}

	private void GenerateErrorList()
	{
		List<int> loadedInstructions = new List<int>();

		int instructionCount = puzzle_controller.puzzleInstance.GetGameInstructionCount();

		do
		{
			int instructionID = UnityEngine.Random.Range(0, instructionCount - 1);

			if(!loadedInstructions.Contains(instructionID))
			{
				loadedInstructions.Add(instructionID);
			}

		} while (loadedInstructions.Count < 3);

		for (int i = 0; i < loadedInstructions.Count; i++)
		{
			int instructionStepCount = puzzle_controller.puzzleInstance.GetGameStepCount(loadedInstructions[i]);

			int stepID = UnityEngine.Random.Range(0, instructionStepCount);

			manualErrors.Add(new ManualError(loadedInstructions[i], stepID));
		}
	}
	private void PopulateInstruction0()
	{
		int engineCount = puzzle_controller.puzzleInstance.GetEngineCount();

		for (int i = 0; i < engineCount; i++)
		{
			int engineNoCount = puzzle_controller.puzzleInstance.GetEngineNoCount(i);

			for (int j = 0; j < engineNoCount; j++)
			{
				string engineNo = puzzle_controller.puzzleInstance.GetEngineNo(i, j);

				int controlID = -1;

				if (puzzle_controller.puzzleInstance.EngineMatch(i, j))
				{
					GameInstruction instruction = puzzle_controller.puzzleInstance.GetGameInstruction(0);
					controlID = instruction.GetStepControl(0);
					ChangeManualTag(0, 0, "//*[@id=\"e" + i + "n" + engineNo + "\"]/*[@id=\"control\"]", controlID);
				}
				else
				{
					controlID = ui_controller.uiInstance.GetRandomControlOfType(new string[] { "switch", "button" });
					ChangeManualTag("//*[@id=\"e" + i + "n" + engineNo + "\"]/*[@id=\"control\"]", controlID);
				}
			}
		}
	}
	private void PopulateInstruction1()
	{
		GameInstruction instruction = puzzle_controller.puzzleInstance.GetGameInstruction(1);
		int controlID1 = instruction.GetStepControl(0);
		int controlID3 = instruction.GetStepControl(1);
		int controlID2 = ui_controller.uiInstance.GetRandomControlOfType(new string[] { "button" }, controlID3);

		ChangeManualTag(1, 0, "//*[@id=\"stm_c1\"]", controlID1);

		ChangeManualTag(1, 1, "//*[@id=\"stm_c2\"]", controlID2);

		ChangeManualTag("//*[@id=\"stm_c3\"]", controlID3);
	}
	private void PopulateInstruction2()
	{
		HtmlNode controlNode;

		GameInstruction instruction = puzzle_controller.puzzleInstance.GetGameInstruction(2);

		int controlID0 = instruction.GetStepControl(0);
		string answer0 = instruction.GetAnswer(0);

		ChangeManualTag(2, 0, "//*[@id=\"jep_c0\"]", controlID0, "//*[@id=\"jep_a0\"]", answer0);

		string answer1 = instruction.GetAnswer(1);
		int controlID1 = instruction.GetStepControl(1);
		int controlID2 = instruction.GetDependantControlID(1);

		controlNode = manualTemplate.DocumentNode.SelectSingleNode("//*[@id=\"jep_v1\"]");
		controlNode.InnerHtml = answer1;

		controlNode = manualTemplate.DocumentNode.SelectSingleNode("//*[@id=\"jep_c1\"]");
		controlNode.InnerHtml = ui_controller.uiInstance.GetControlLabel(controlID1);

		controlNode = manualTemplate.DocumentNode.SelectSingleNode("//*[@id=\"jep_c2\"]");
		controlNode.InnerHtml = ui_controller.uiInstance.GetControlLabel(controlID2);

		int controlID3 = instruction.GetStepControl(2);
		int controlID4 = instruction.GetDependantControlID(2);

		controlNode = manualTemplate.DocumentNode.SelectSingleNode("//*[@id=\"jep_c3\"]");
		controlNode.InnerHtml = ui_controller.uiInstance.GetControlLabel(controlID3);

		controlNode = manualTemplate.DocumentNode.SelectSingleNode("//*[@id=\"jep_c4\"]");
		controlNode.InnerHtml = ui_controller.uiInstance.GetControlLabel(controlID4);

	}
	private void PopulateInstruction3()
	{
		HtmlNode controlNode;

		GameInstruction instruction = puzzle_controller.puzzleInstance.GetGameInstruction(3);

		int controlID0 = instruction.GetStepControl(0);
		int engineNo = int.Parse(puzzle_controller.puzzleInstance.GetCurrentEngineNo());
		int modelNoNum = int.Parse(puzzle_controller.puzzleInstance.GetModelNo().Substring(0, 1));
		int answer0 = int.Parse(instruction.GetAnswer(0));
		int value = answer0 - engineNo - modelNoNum;
		string value0 = "";

		if (value < 0)
		{
			value *= -1;
			value0 = " - " + value.ToString();
		}
		else
		{
			value0 = " + " + value.ToString();
		}

		controlNode = manualTemplate.DocumentNode.SelectSingleNode("//*[@id=\"frt_c0\"]");
		controlNode.InnerHtml = ui_controller.uiInstance.GetControlLabel(controlID0);

		controlNode = manualTemplate.DocumentNode.SelectSingleNode("//*[@id=\"frt_v0\"]");
		controlNode.InnerHtml = value0;

		int controlID1 = instruction.GetStepControl(1);

		controlNode = manualTemplate.DocumentNode.SelectSingleNode("//*[@id=\"frt_c1\"]");
		controlNode.InnerHtml = ui_controller.uiInstance.GetControlLabel(controlID1);

		string answer2 = instruction.GetAnswer(2);
		string value1 = puzzle_controller.puzzleInstance.GetColorWarningLevel(answer2);
		int controlID2 = instruction.GetStepControl(2);

		controlNode = manualTemplate.DocumentNode.SelectSingleNode("//*[@id=\"frt_v1\"]");
		controlNode.InnerHtml = value1;

		controlNode = manualTemplate.DocumentNode.SelectSingleNode("//*[@id=\"frt_c2\"]");
		controlNode.InnerHtml = ui_controller.uiInstance.GetControlLabel(controlID2);
	}
	private void PopulateInstruction4()
	{
		HtmlNode controlNode;

		GameInstruction instruction = puzzle_controller.puzzleInstance.GetGameInstruction(4);


		for (int i = 0; i < puzzle_controller.puzzleInstance.GetNavSystemCount(); i++)
		{
			string answer0 = "";
			string answer1 = "";

			controlNode = manualTemplate.DocumentNode.SelectSingleNode("//*[@id=\"snc_d" + i + "\"]");

			if (i == puzzle_controller.puzzleInstance.GetCurrentNavSystemID())
			{
				answer0 = instruction.GetAnswer(0);
				answer1 = instruction.GetAnswer(1);
			}
			else
			{
				int controlID0 = instruction.GetStepControl(0);
				int controlID1 = instruction.GetStepControl(1);

				answer0 = ui_controller.uiInstance.GetControlRandomAnswer(controlID0);
				answer1 = ui_controller.uiInstance.GetControlRandomAnswer(controlID1);
			}

			controlNode.InnerHtml = answer0 + answer1;

			for (int j = 0; j < puzzle_controller.puzzleInstance.GetCurrentNawSystemPlanetCount(i); j++)
			{
				int answerMain = 0;
				int controlID2 = instruction.GetStepControl(2);
				int answer2 = 0;
				int answer3 = 0;

				controlNode = manualTemplate.DocumentNode.SelectSingleNode("//*[@id=\"snc_s" + i + "b" + j + "\"]");

				if (puzzle_controller.puzzleInstance.NavSystemMatch(i, j))
				{
					answerMain = int.Parse(instruction.GetAnswer(2));
				}
				else
				{
					answerMain = int.Parse(ui_controller.uiInstance.GetControlRandomAnswer(controlID2));
				}

				do
				{
					answer2 = int.Parse(ui_controller.uiInstance.GetControlRandomAnswer(controlID2));
					answer3 = answer2 - answerMain;
				} while (answer3 < 0);

				controlNode.InnerHtml = answer2 + " ' " + answer3;
			}
		}
	}
	private void PopulateInstruction5()
	{
		HtmlNode controlNode;

		GameInstruction instruction = puzzle_controller.puzzleInstance.GetGameInstruction(5);

		int controlID3 = instruction.GetStepControl(3);

		controlNode = manualTemplate.DocumentNode.SelectSingleNode("//*[@id=\"re_c0\"]");
		controlNode.InnerHtml = ui_controller.uiInstance.GetControlLabel(controlID3);

		string answer4 = instruction.GetAnswer(4);

		controlNode = manualTemplate.DocumentNode.SelectSingleNode("//*[@id=\"re_v0\"]");
		controlNode.InnerHtml = answer4;
	}

	private void ChangeManualTag(string tag, int controlID)
	{
		HtmlNode engineNode = manualTemplate.DocumentNode.SelectSingleNode(tag); ;
		engineNode.InnerHtml = ui_controller.uiInstance.GetControlLabel(controlID);
	}
	private void ChangeManualTag(int instructionID, int stepID, string tag, int controlID)
	{
		int errorID = -1;

		for(int i = 0; i < manualErrors.Count; i++)
		{
			if(manualErrors[i].ErrorMatch(instructionID, stepID))
			{
				errorID = i;
			}
		}

		if (errorID > -1)
		{
			int errorControlID = GetErrorControl(controlID);

			if (errorControlID > -1)
			{
				SetErrorControlEntry(errorID, instructionID, tag, controlID, errorControlID);
			}
		}
		else
		{
			HtmlNode engineNode = manualTemplate.DocumentNode.SelectSingleNode(tag); ;
			engineNode.InnerHtml = ui_controller.uiInstance.GetControlLabel(controlID);
		}
	}
    private void ChangeManualTag(int instructionID, int stepID, string tag1, int controlID, string tag2, string answer)
	{
		int errorID = -1;

		for (int i = 0; i < manualErrors.Count; i++)
		{
			if (manualErrors[i].ErrorMatch(instructionID, stepID))
			{
				errorID = i;
			}
		}

		if (errorID > -1)
		{
			if (UnityEngine.Random.Range(0, 2) > 0)
			{
				int errorControlID = GetErrorControl(controlID);

				if (errorControlID > -1)
				{
					SetErrorControlEntry(errorID, instructionID, tag1, controlID, errorControlID);

					HtmlNode engineNode = manualTemplate.DocumentNode.SelectSingleNode(tag2); ;
					engineNode.InnerHtml = answer;
				}
			}
			else
			{
				string errorAnswer = GetErrorAnswer(controlID, answer);

				HtmlNode engineNode = manualTemplate.DocumentNode.SelectSingleNode(tag1); ;
				engineNode.InnerHtml = ui_controller.uiInstance.GetControlLabel(controlID);

				SetErrorAnswerEntry(errorID, instructionID, tag2, controlID, answer, errorAnswer);

			}
		}
		else
		{
			HtmlNode engineNode = manualTemplate.DocumentNode.SelectSingleNode(tag1); ;
			engineNode.InnerHtml = ui_controller.uiInstance.GetControlLabel(controlID);

			engineNode = manualTemplate.DocumentNode.SelectSingleNode(tag2); ;
			engineNode.InnerHtml = answer;
		}
	}

	private int GetErrorControl(int controlID)
	{
		string controlType = ui_controller.uiInstance.GetControlType(controlID);
		string[] controlTypes = null;

		switch (controlType)
		{
			case "switch":
			case "button":
				controlTypes = new string[] { "switch", "button" };
				break;
			case "slider":
			case "knob":
				controlTypes = new string[] { "slider", "knob" };
				break;
			case "light":
			case "meter":
				controlTypes = new string[] { "light", "meter" };
				break;
		}

		return ui_controller.uiInstance.GetRandomControlOfType(controlTypes, controlID);
	}

	private string GetErrorAnswer(int controlID, string answer)
	{
		string errorAnswer = "";

		do
		{
			errorAnswer = ui_controller.uiInstance.GetControlRandomAnswer(controlID);

		} while (errorAnswer == answer);

		return errorAnswer;
	}

	private void SetErrorControlEntry(int errorID, int instructionID, string tag, int controlID, int errorControlID)
	{
		HtmlNode engineNode = manualTemplate.DocumentNode.SelectSingleNode(tag); ;
		engineNode.InnerHtml = ui_controller.uiInstance.GetControlLabel(errorControlID);

		HtmlNode errorNode = manualTemplate.DocumentNode.SelectSingleNode("//*[@id=\"error_" + (errorID + 1) + "\"]");
		errorNode.InnerHtml = puzzle_controller.puzzleInstance.GetGameInstructionTitle(instructionID) + " USE " + ui_controller.uiInstance.GetControlLabel(controlID) + " NOT " + ui_controller.uiInstance.GetControlLabel(errorControlID);
	}

	private void SetErrorAnswerEntry(int errorID, int instructionID, string tag, int controlID, string answer, string errorAnswer)
	{
		HtmlNode engineNode = manualTemplate.DocumentNode.SelectSingleNode(tag); ;
		engineNode.InnerHtml = errorAnswer;

		HtmlNode errorNode = manualTemplate.DocumentNode.SelectSingleNode("//*[@id=\"error_" + (errorID + 1) + "\"]");
		errorNode.InnerHtml = puzzle_controller.puzzleInstance.GetGameInstructionTitle(instructionID) + " FOR " + ui_controller.uiInstance.GetControlLabel(controlID) + ", ENTER " + answer + " NOT " + errorAnswer;
	}
}