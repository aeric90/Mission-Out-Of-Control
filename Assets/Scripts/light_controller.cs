using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ParentControl;

public class light_controller : ParentControl
{
    public GameObject light;

    private void Update()
    {
        if(value == "red")
        {
            light.GetComponent<Image>().color = Color.red;
        }
        if(value == "blue")
        {
            light.GetComponent<Image>().color = Color.blue;
        }
        if (value == "green")
        {
            light.GetComponent<Image>().color = Color.green;
        }
        if (value == "yellow")
        {
            light.GetComponent<Image>().color = Color.yellow;
        }
        if (value == "off")
        {
            light.GetComponent<Image>().color = Color.grey;
        }
    }
}
