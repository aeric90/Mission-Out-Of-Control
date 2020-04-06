using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using HtmlAgilityPack;
using System.IO;
using System.Net;

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
	public static manual_controller manualInstance;

	public TMPro.TextMeshProUGUI manualCodeText;

	private List<ManualError> manualErrors = new List<ManualError>();
	
	private HtmlDocument manualTemplate;

	private int manualCode;
	private string manualFileName;

	private void Awake()
	{
		if (manualInstance == null) { manualInstance = this; }
	}

	private void OnApplicationQuit()
	{
		FTPDelete(manualFileName);
	}

	public void StartManualCreation()
	{
		StartCoroutine(CreateManual());
	}

	IEnumerator CreateManual()
	{
		StreamReader headerText;
		StreamReader instructionText;
		StreamReader manualText;
		MemoryStream inputStream = new MemoryStream();
		StreamWriter inputFile = new StreamWriter(inputStream);
		MemoryStream outputStream = new MemoryStream();

		GenerateErrorList();

		headerText = FTPGet(@"/instructions/mooc_header.html");
		inputFile.Write(headerText.ReadToEnd());
		inputFile.Flush();

		int coffee_stains = UnityEngine.Random.Range(0, 4);

		if(coffee_stains > 0)
		{
			instructionText = FTPGet(@"/instructions/mooc_coffee_" + coffee_stains + ".html");
			inputFile.Write(instructionText.ReadToEnd());
			inputFile.Flush();
		}

		List<int> manualInsturctions = new List<int>();

		do
		{
			int randomInstruction = -1;

			do
			{
				randomInstruction = UnityEngine.Random.Range(0, puzzle_controller.puzzleInstance.GetSelectedInstructionCount());
			} while (manualInsturctions.Contains(randomInstruction));

			string instructionHTML = puzzle_controller.puzzleInstance.GetSelectedInstuctionHTML(randomInstruction);

			instructionText = FTPGet(@"/instructions" + instructionHTML);
			inputFile.Write(instructionText.ReadToEnd());
			inputFile.Flush();

			manualInsturctions.Add(randomInstruction);

			yield return null;
		} while (manualInsturctions.Count < puzzle_controller.puzzleInstance.GetSelectedInstructionCount());

		instructionText = FTPGet(@"/instructions/mooc_re.html");
		inputFile.Write(instructionText.ReadToEnd());
		inputFile.Flush();

		headerText = FTPGet(@"/instructions/mooc_footer.html");
		inputFile.Write(headerText.ReadToEnd());
		inputFile.Flush();

		inputStream.Position = 0;

		manualText = new StreamReader(inputStream);

		manualCode = UnityEngine.Random.Range(1000, 10000);
		manualFileName = @"/Mission_Manual_" + manualCode + ".html";

		manualTemplate = new HtmlDocument();
		manualTemplate.Load(manualText);

		ChangeManualTag("//*[@id=\"mooc_no\"]", manualCode.ToString());

		for (int i = 0; i < puzzle_controller.puzzleInstance.GetSelectedInstructionCount(); i++)
		{
			string instructionHTML = puzzle_controller.puzzleInstance.GetSelectedInstuctionHTML(i);

			switch (instructionHTML)
			{
				case "/mooc_davp.html":
					PopulateMoocDAVP();
					break;
				case "/mooc_adb.html":
					PopulateMoocADB();
					break;
				case "/mooc_dt.html":
					PopulateMoocTR();
					break;
				case "/mooc_astm.html":
					PopulateMoocASTM();
					break;
				case "/mooc_rvmo.html":
					PopulateMoocRVMO();
					break;
				case "/mooc_dlg.html":
					PopulateMoocDLG();
					break;
				case "/mooc_jep.html":
					PopulateMoocJEP();
					break;
				case "/mooc_dabr.html":
					PopulateMoocDABR();
					break;
				case "/mooc_asbatt.html":
					PopulateMoocASBATT();
					break;
				case "/mooc_frt.html":
					PopulateMoocFRT();
					break;
				case "/mooc_rdc.html":
					PopulateMoocRDC();
					break;
				case "/mooc_snc.html":
					PopulateMoocSNC();
					break;
				default:
					break;
			}
			yield return null;
		}

		PopulateMoocRE();

		manualTemplate.Save(outputStream);

		FTPSend(outputStream.ToArray(), manualFileName);

		manual_canvas_controller.manualCanvasInstance.UpdateManualCode(manualCode.ToString());
		ui_controller.uiInstance.UpdateManualCode(manualCode.ToString());
		manual_canvas_controller.manualCanvasInstance.SetManualsReady();
	}

	private void GenerateErrorList()
	{
		manualErrors.Clear();

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

	private void PopulateMoocDAVP()
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
	private void PopulateMoocADB()
	{
		GameInstruction instruction = puzzle_controller.puzzleInstance.GetGameInstruction(0);
		int controlID0 = instruction.GetStepControl(0);

		ChangeManualTag("//*[@id=\"adb_c0\"]", controlID0);
	}
	private void PopulateMoocTR()
	{
		GameInstruction instruction = puzzle_controller.puzzleInstance.GetGameInstruction(0);
		int controlID0 = instruction.GetStepControl(0);
		string answer0 = instruction.GetAnswer(0);
		string answer1 = (6 - int.Parse(answer0)).ToString();

		ChangeManualTag(0, 0, "//*[@id=\"tr_c0\"]", controlID0, "//*[@id=\"tr_v0\"]", answer0);
		ChangeManualTag("//*[@id=\"tr_v1\"]", answer1);
	}
	private void PopulateMoocASTM()
	{
		GameInstruction instruction = puzzle_controller.puzzleInstance.GetGameInstruction(1);
		int controlID1 = instruction.GetStepControl(0);

		ChangeManualTag(1, 0, "//*[@id=\"stm_c1\"]", controlID1);

		int controlID2 = instruction.GetStepControl(1);
		int controlID3 = ui_controller.uiInstance.GetRandomControlOfType(new string[] { "button" }, controlID2);

		int modelLetters = puzzle_controller.puzzleInstance.GetModelNoLetterCount();

		if (modelLetters % 2 > 0)
		{
			ChangeManualTag(1, 1, "//*[@id=\"stm_c2\"]", controlID2);
			ChangeManualTag("//*[@id=\"stm_c3\"]", controlID3);
		}
		else
		{
			ChangeManualTag("//*[@id=\"stm_c2\"]", controlID3);
			ChangeManualTag(1, 1, "//*[@id=\"stm_c3\"]", controlID2);
		}
	}
	private void PopulateMoocRVMO()
	{
		GameInstruction instruction = puzzle_controller.puzzleInstance.GetGameInstruction(1);
		int controlID0 = instruction.GetStepControl(0);
		int controlID1 = instruction.GetDependantControlID(0);

		ChangeManualTag(1, 0, "//*[@id=\"rvmo_c0\"]", controlID0, "//*[@id=\"rvmo_c1\"]", controlID1);

		string answer0 = instruction.GetAnswer(1);
		ChangeManualTag("//*[@id=\"rvmo_a0\"]", answer0);

	}
	private void PopulateMoocDLG()
	{
		GameInstruction instruction = puzzle_controller.puzzleInstance.GetGameInstruction(1);
		int controlID0 = instruction.GetStepControl(0);
		int controlID1 = instruction.GetStepControl(1);

		ChangeManualTag(1, 0, "//*[@id=\"dlg_c0\"]", controlID0);
		ChangeManualTag(1, 1, "//*[@id=\"dlg_c1\"]", controlID1);
	}
	private void PopulateMoocJEP()
	{
		GameInstruction instruction = puzzle_controller.puzzleInstance.GetGameInstruction(2);

		int controlID0 = instruction.GetStepControl(0);
		string answer0 = instruction.GetAnswer(0);

		ChangeManualTag(2, 0, "//*[@id=\"jep_c0\"]", controlID0, "//*[@id=\"jep_a0\"]", answer0);

		string answer1 = instruction.GetAnswer(1);
		int controlID1 = instruction.GetStepControl(1);
		int controlID2 = instruction.GetDependantControlID(1);

		ChangeManualTag("//*[@id=\"jep_v1\"]", answer1);
		ChangeManualTag("//*[@id=\"jep_c1\"]", controlID1);
		ChangeManualTag(2, 1, "//*[@id=\"jep_c2\"]", controlID2);

		int controlID3 = instruction.GetStepControl(2);
		int controlID4 = instruction.GetDependantControlID(2);

		ChangeManualTag(2, 2, "//*[@id=\"jep_c3\"]", controlID3, "//*[@id=\"jep_c4\"]", controlID4);
	}	  
	private void PopulateMoocDABR()
	{
		GameInstruction instruction = puzzle_controller.puzzleInstance.GetGameInstruction(2);

		int controlID0 = instruction.GetStepControl(0);
		string answer0 = instruction.GetAnswer(0);

		ChangeManualTag(2, 0, "//*[@id=\"dabr_c0\"]", controlID0, "//*[@id=\"dabr_a0\"]", answer0);

		int controlID1 = instruction.GetStepControl(1);
		string answer1 = instruction.GetAnswer(1);

		ChangeManualTag(2, 1, "//*[@id=\"dabr_c1\"]", controlID1, "//*[@id=\"dabr_a1\"]", answer1);

		string answer2 = instruction.GetAnswer(2);
		int controlID2 = instruction.GetStepControl(2);
		int controlID3 = instruction.GetDependantControlID(2);

		ChangeManualTag(2, 2, "//*[@id=\"dabr_c2\"]", controlID2, "//*[@id=\"dabr_a2\"]", answer2);
		ChangeManualTag("//*[@id=\"dabr_c3\"]", controlID3);
	}
	private void PopulateMoocASBATT()
	{
		GameInstruction instruction = puzzle_controller.puzzleInstance.GetGameInstruction(2);

		int controlID0 = instruction.GetStepControl(0);
		string answer0 = instruction.GetAnswer(0);

		ChangeManualTag(2, 0, "//*[@id=\"asbatt_c0\"]", controlID0, "//*[@id=\"asbatt_a0\"]", answer0);

		int controlID1 = instruction.GetStepControl(1);
		string answer1 = instruction.GetAnswer(1);

		ChangeManualTag(2, 1, "//*[@id=\"asbatt_c1\"]", controlID1, "//*[@id=\"asbatt_a1\"]", answer1);

		string answer2 = instruction.GetAnswer(2);
		int controlID2 = instruction.GetStepControl(2);
		int controlID3 = instruction.GetDependantControlID(2);

		ChangeManualTag(2, 2, "//*[@id=\"asbatt_c2\"]", controlID2, "//*[@id=\"asbatt_a2\"]", answer2);
		ChangeManualTag("//*[@id=\"asbatt_c3\"]", controlID3);
	}
	private void PopulateMoocFRT()
	{
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

		ChangeManualTag(3, 0, "//*[@id=\"frt_c0\"]", controlID0);
		ChangeManualTag("//*[@id=\"frt_v0\"]", value0);

		int controlID1 = instruction.GetStepControl(1);
		int controlID2 = instruction.GetDependantControlID(1);
		ChangeManualTag(3, 1, "//*[@id=\"frt_c1\"]", controlID1);
		ChangeManualTag(3, 1, "//*[@id=\"frt_c2\"]", controlID2);

		string answer2 = instruction.GetAnswer(2);
		string value1 = puzzle_controller.puzzleInstance.GetColorWarningLevel(answer2);
		int controlID3 = instruction.GetStepControl(2);

		ChangeManualTag("//*[@id=\"frt_v1\"]", value1);
		ChangeManualTag(3, 2, "//*[@id=\"frt_c3\"]", controlID3);
	}
	private void PopulateMoocRDC()
	{
		GameInstruction instruction = puzzle_controller.puzzleInstance.GetGameInstruction(3);

		int controlID0 = instruction.GetStepControl(0);
		int controlID1 = instruction.GetDependantControlID(0);

		ChangeManualTag("//*[@id=\"rdc_c0\"]", controlID0);
		ChangeManualTag("//*[@id=\"rdc_c1\"]", controlID1);

		int controlID2 = instruction.GetStepControl(1);
		int controlID3 = instruction.GetDependantControlID(1);

		ChangeManualTag("//*[@id=\"rdc_c2\"]", controlID2);
		ChangeManualTag("//*[@id=\"rdc_c3\"]", controlID3);

		string answer0 = instruction.GetAnswer(2);
		int controlID4 = instruction.GetStepControl(2);
		int controlID5 = instruction.GetDependantControlID(2);

		ChangeManualTag("//*[@id=\"rdc_v0\"]", answer0);
		ChangeManualTag("//*[@id=\"rdc_c4\"]", controlID4);
		ChangeManualTag(3, 2, "//*[@id=\"rdc_c5\"]", controlID5);
	}
	private void PopulateMoocSNC()
	{
		GameInstruction instruction = puzzle_controller.puzzleInstance.GetGameInstruction(4);
		int controlID0 = instruction.GetStepControl(0);
		int controlID1 = instruction.GetStepControl(1);

		for (int i = 0; i < puzzle_controller.puzzleInstance.GetNavSystemCount(); i++)
		{
			string answer0 = "";
			string answer1 = "";

			if (i == puzzle_controller.puzzleInstance.GetCurrentNavSystemID())
			{
				answer0 = instruction.GetAnswer(0);
				answer1 = instruction.GetAnswer(1);
			}
			else
			{
				answer0 = ui_controller.uiInstance.GetControlRandomAnswer(controlID0);
				answer1 = ui_controller.uiInstance.GetControlRandomAnswer(controlID1);
			}

			ChangeManualTag("//*[@id=\"snc_d" + i + "\"]", answer0 + answer1);

			for (int j = 0; j < puzzle_controller.puzzleInstance.GetCurrentNawSystemPlanetCount(i); j++)
			{
				int answerMain = 0;
				int controlID2 = instruction.GetStepControl(2);
				int answer2 = 0;
				int answer3 = 0;

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

				ChangeManualTag("//*[@id=\"snc_s" + i + "b" + j + "\"]", answer2 + " ' " + answer3);
			}
		}

		ChangeManualTag(4, 0, "//*[@id=\"snc_c0\"]", controlID0);
		ChangeManualTag(4, 1, "//*[@id=\"snc_c1\"]", controlID1);
	}
	private void PopulateMoocRE()
	{
		GameInstruction instruction = puzzle_controller.puzzleInstance.GetGameInstruction(5);

		string instructionTitle0 = puzzle_controller.puzzleInstance.GetGameInstruction(0).GetTitle();
		string reverseTitle0 = puzzle_controller.puzzleInstance.FindInstructionReverseTitle(instructionTitle0);

		ChangeManualTag("//*[@id=\"re_i0\"]", reverseTitle0);

		string instructionTitle1 = puzzle_controller.puzzleInstance.GetGameInstruction(1).GetTitle();
		string reverseTitle1 = puzzle_controller.puzzleInstance.FindInstructionReverseTitle(instructionTitle1);

		ChangeManualTag("//*[@id=\"re_i1\"]", reverseTitle1);

		int controlID3 = instruction.GetStepControl(3);
		ChangeManualTag("//*[@id=\"re_c0\"]", controlID3);
		string answer4 = instruction.GetAnswer(4);
		ChangeManualTag("//*[@id=\"re_v0\"]", answer4);
	}

	private void ChangeManualTag(string tag, int controlID)
	{
		HtmlNode engineNode = manualTemplate.DocumentNode.SelectSingleNode(tag); ;
		engineNode.InnerHtml = ui_controller.uiInstance.GetControlLabel(controlID);
	}
	private void ChangeManualTag(string tag, string answer)
	{
		HtmlNode engineNode = manualTemplate.DocumentNode.SelectSingleNode(tag); ;
		engineNode.InnerHtml = answer;
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
				SetErrorControlEntry(errorID, instructionID, stepID, tag, controlID, errorControlID);
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
					SetErrorControlEntry(errorID, instructionID, stepID, tag1, controlID, errorControlID);

					HtmlNode engineNode = manualTemplate.DocumentNode.SelectSingleNode(tag2); ;
					engineNode.InnerHtml = answer;
				}
			}
			else
			{
				string errorAnswer = GetErrorAnswer(controlID, answer);

				HtmlNode engineNode = manualTemplate.DocumentNode.SelectSingleNode(tag1); ;
				engineNode.InnerHtml = ui_controller.uiInstance.GetControlLabel(controlID);

				SetErrorAnswerEntry(errorID, instructionID, stepID, tag2, controlID, answer, errorAnswer);

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
	private void ChangeManualTag(int instructionID, int stepID, string tag1, int controlID1, string tag2, int controlID2)
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
			int errorControlID = GetErrorControl(controlID2, controlID1);

			HtmlNode engineNode = manualTemplate.DocumentNode.SelectSingleNode(tag1); ;
			engineNode.InnerHtml = ui_controller.uiInstance.GetControlLabel(controlID1);

			SetErrorControlEntry(errorID, instructionID, stepID, tag1, controlID2, errorControlID);
		}
		else
		{
			HtmlNode engineNode = manualTemplate.DocumentNode.SelectSingleNode(tag1); ;
			engineNode.InnerHtml = ui_controller.uiInstance.GetControlLabel(controlID1);

			engineNode = manualTemplate.DocumentNode.SelectSingleNode(tag2); ;
			engineNode.InnerHtml = ui_controller.uiInstance.GetControlLabel(controlID2);
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
	private int GetErrorControl(int controlID, int notControlID)
	{
		int errorControlID = -1;
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

		do
		{
			errorControlID = ui_controller.uiInstance.GetRandomControlOfType(controlTypes, controlID);
		} while (controlID == notControlID);

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
	private void SetErrorControlEntry(int errorID, int instructionID, int stepID, string tag, int controlID, int errorControlID)
	{
		HtmlNode engineNode = manualTemplate.DocumentNode.SelectSingleNode(tag); ;
		engineNode.InnerHtml = ui_controller.uiInstance.GetControlLabel(errorControlID);

		HtmlNode errorNode = manualTemplate.DocumentNode.SelectSingleNode("//*[@id=\"error_" + (errorID + 1) + "\"]");
		errorNode.InnerHtml = puzzle_controller.puzzleInstance.GetGameInstructionTitle(instructionID) + " - STEP " + (stepID + 1) + " - USE " + ui_controller.uiInstance.GetControlLabel(controlID) + " NOT " + ui_controller.uiInstance.GetControlLabel(errorControlID);
	}
	private void SetErrorAnswerEntry(int errorID, int instructionID, int stepID, string tag, int controlID, string answer, string errorAnswer)
	{
		HtmlNode engineNode = manualTemplate.DocumentNode.SelectSingleNode(tag); ;
		engineNode.InnerHtml = errorAnswer;

		HtmlNode errorNode = manualTemplate.DocumentNode.SelectSingleNode("//*[@id=\"error_" + (errorID + 1) + "\"]");
		errorNode.InnerHtml = puzzle_controller.puzzleInstance.GetGameInstructionTitle(instructionID) + " - STEP " + (stepID + 1) + " - FOR " + ui_controller.uiInstance.GetControlLabel(controlID) + ", ENTER " + answer + " NOT " + errorAnswer;
	}

	public StreamReader FTPGet(string manualFileName)
	{
		// Get the object used to communicate with the server.
		FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://213.190.6.173" + manualFileName);
		request.Method = WebRequestMethods.Ftp.DownloadFile;

		// This example assumes the FTP site uses anonymous logon.
		request.Credentials = new NetworkCredential("u590740642", "moocmanuals");

		FtpWebResponse response = (FtpWebResponse)request.GetResponse();
		Debug.Log(response.StatusDescription);
		Stream responseStream = response.GetResponseStream();
		return new StreamReader(responseStream);
	}
	public void FTPSend(byte[] outputBytes, string manualFileName)
	{
		FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://213.190.6.173" + manualFileName);
		request.Method = WebRequestMethods.Ftp.UploadFile;

		request.Credentials = new NetworkCredential("u590740642", "moocmanuals");

		request.ContentLength = outputBytes.Length;

		using (Stream requestStream = request.GetRequestStream())
		{
			requestStream.Write(outputBytes, 0, outputBytes.Length);
		}
	}
	public void FTPDelete(string manualFileName)
	{
		FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://213.190.6.173" + manualFileName);
		request.Credentials = new NetworkCredential("u590740642", "moocmanuals");
		request.Method = WebRequestMethods.Ftp.DeleteFile;
		FtpWebResponse response = (FtpWebResponse)request.GetResponse();
		Debug.Log(response.StatusDescription);
		response.Close();
	}
}