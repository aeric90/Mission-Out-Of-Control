using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml;
using HtmlAgilityPack;


public class manual_controller : MonoBehaviour
{
	private HtmlDocument manualTemplate;

	private void Start()
	{
		manualTemplate = new HtmlDocument();
		manualTemplate.Load(@".\Assets\HTML\Beta Test Manual.html");


		string path = @".\Assets\HTML\manual_output.test.html";

		PopulateInstruction0();
		PopulateInstruction1();
		PopulateInstruction2();

		manualTemplate.Save(path);

		/*
		var test = doc.DocumentNode.SelectSingleNode("//div").Attributes["id"].Value;		   
		Debug.Log(test);
		*/
	}

	private static void SaveHtmlFile()
	{
		var html =
		@"<!DOCTYPE html>
<html>
<body>
	<h1>This is <b>bold</b> heading</h1>
	<p>This is <u>underlined</u> paragraph</p>
	<h2>This is <i>italic</i> heading</h2>
</body>
</html> ";

		var htmlDoc = new HtmlDocument();
		htmlDoc.LoadHtml(html);

		htmlDoc.Save(@".\Assets\HTML\test.html");
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
				
				HtmlNode engineNode = manualTemplate.DocumentNode.SelectSingleNode("//*[@id=\"e" + i + "n" + engineNo + "\"]/*[@id=\"control\"]");

				int controlID = -1;

				if(puzzle_controller.puzzleInstance.EngineMatch(i, j))
				{
					GameInstruction instruction = puzzle_controller.puzzleInstance.GetGameInstruction(0);
					controlID = instruction.GetStepControl(0);
				}
				else
				{
					controlID = ui_controller.uiInstance.GetRandomControlOfType(new string[] { "switch", "button" });
				}

				engineNode.InnerHtml = ui_controller.uiInstance.GetControlLabel(controlID);
			}
		}
	}
	private void PopulateInstruction1()
	{
		HtmlNode controlNode;

		GameInstruction instruction = puzzle_controller.puzzleInstance.GetGameInstruction(1);
		int controlID1 = instruction.GetStepControl(0);
		int controlID3 = instruction.GetStepControl(1);
		int controlID2 = ui_controller.uiInstance.GetRandomControlOfType(new string[] { "button" }, controlID3);

		controlNode = manualTemplate.DocumentNode.SelectSingleNode("//*[@id=\"stm_c1\"]");
		controlNode.InnerHtml = ui_controller.uiInstance.GetControlLabel(controlID1);

		controlNode = manualTemplate.DocumentNode.SelectSingleNode("//*[@id=\"stm_c2\"]");
		controlNode.InnerHtml = ui_controller.uiInstance.GetControlLabel(controlID2);

		controlNode = manualTemplate.DocumentNode.SelectSingleNode("//*[@id=\"stm_c3\"]");
		controlNode.InnerHtml = ui_controller.uiInstance.GetControlLabel(controlID3);
	}
	private void PopulateInstruction2()
	{
		HtmlNode controlNode;

		GameInstruction instruction = puzzle_controller.puzzleInstance.GetGameInstruction(2);

		int controlID0 = instruction.GetStepControl(0);
		string answer0 = instruction.GetAnswer(0);

		controlNode = manualTemplate.DocumentNode.SelectSingleNode("//*[@id=\"jep_c0\"]");
		controlNode.InnerHtml = ui_controller.uiInstance.GetControlLabel(controlID0);

		controlNode = manualTemplate.DocumentNode.SelectSingleNode("//*[@id=\"jep_a0\"]");
		controlNode.InnerHtml = answer0;

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
}
