using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class screen_controller : MonoBehaviour
{
    public string modelText = "3ZF94";
    public string engineText = "Pulse Ion";
    public string noEngineText = "4";
    public string systemText = "";
    public string planetText = "";

    public TMPro.TextMeshProUGUI screenText;

    // Update is called once per frame
    void Update()
    {
        string output = "";

        output += "Model#: " + modelText + "\n";
        output += "Engines: " + engineText + "\n";
        output += "No. of Engines: " + noEngineText + "\n";
        output += "Location:\n";
        output += "\tSystem: " + (systemText == "" ? "UNKNOWN" : systemText) + "\n";
        output += "\tSystem: " + (planetText == "" ? "UNKNOWN" : planetText) + "\n";

        screenText.text = output;
    }

    public void SetSystemText(string systemText)
    {
        this.systemText = systemText;
    }

    public void SetPlanetText(string planetText)
    {
        this.planetText = planetText;
    }
}
