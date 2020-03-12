using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ParentControl;

public class button_controller : ParentControl
{

    GameObject button;

    // when the button has been pressed do this.
    void OnMouseDown()
    {
        Debug.Log("buttonClicked");
    }
}
