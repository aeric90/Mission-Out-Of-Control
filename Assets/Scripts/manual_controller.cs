using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml;
using HtmlAgilityPack;


public class manual_controller : MonoBehaviour
{
	private void Start()
	{
		//SaveHtmlFile();

		#region example

		string path = @".\Assets\HTML\Beta Test Manual.html";

		var doc = new HtmlDocument();
		doc.Load(path);

		var engineTable = doc.DocumentNode.SelectSingleNode("//body/table");

		HtmlNodeCollection engineTableRows = engineTable.ChildNodes;

		foreach (var row in engineTableRows)
		{
			if (row.NodeType == HtmlNodeType.Element)
			{
				HtmlNodeCollection engingTableEntries = row.ChildNodes;

				foreach (var entry in engineTableRows)
				{
					Debug.Log(entry.OuterHtml);
				}
			}
		}

		#endregion
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


}
