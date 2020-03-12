using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ParentControl;

public class light_controller : ParentControl
{
    public GameObject lightPanel;

    private void Start()
    {
        numStates = 5;
    }

    private void Update()
    {
        if(value == "1")
        {
            lightPanel.GetComponent<Image>().color = Color.red;
        }
        if(value == "2")
        {
            lightPanel.GetComponent<Image>().color = Color.blue;
        }
        if (value == "3")
        {
            lightPanel.GetComponent<Image>().color = Color.green;
        }
        if (value == "4")
        {
            lightPanel.GetComponent<Image>().color = Color.yellow;
        }
        if (value == "0")
        {
            lightPanel.GetComponent<Image>().color = Color.grey;
        }
    }
}
