using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class screen_controller : MonoBehaviour
{
    public string modelText = "";
    public string engineText = "";
    public string noEngineText = "";
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
        output += "  System: " + (systemText == "" ? "UNKNOWN" : systemText) + "\n";
        output += "  Planet: " + (planetText == "" ? "UNKNOWN" : planetText) + "\n";

        screenText.text = output;
    }

    public void SetEngineText(string engineText)
    {
        this.engineText = engineText;
    }

    public void SetNoEngineText(string noEngineText)
    {
        this.noEngineText = noEngineText;
    }

    public void SetSystemText(string systemText)
    {
        this.systemText = systemText;
    }

    public void SetPlanetText(string planetText)
    {
        this.planetText = planetText;
    }

    public void SetModelText(string modelText)
    {
        this.modelText = modelText;
    }
}
