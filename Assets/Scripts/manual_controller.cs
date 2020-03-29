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
				Debug.Log("//*[@id=\"e" + i + "n" + j + "\"]/*[@id=\"control\"]");
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

}
