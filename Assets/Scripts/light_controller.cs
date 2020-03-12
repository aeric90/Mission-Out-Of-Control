using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ParentControl;

public class light_controller : ParentControl
{

    // string lightUI;

    private void Start()
    {

    }

    // Is the light on or off?
    public void LightOnOff()
    {
        // the control has been activated
        if (this.GetActive())
        {
            this.GetValue();

            // If the value string is 0, the light will shut off (turn grey)
            if (this.value == "0")
            {
                Debug.Log(" value is 0, Lights Out!");
                GetComponent<Image>().color = Color.grey;
            }

            // If the value string is 1, the light will turn on (turn white). 
            else if (this.value == "1")
            {
                Debug.Log("value is 1 Lights On!");
                GetComponent<Image>().color = Color.white;
            }
        }
    }
}
