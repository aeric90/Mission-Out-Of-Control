using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ParentControl;

public class light_controller : ParentControl
{
    public GameObject lightPanel;
    private Color lightColor;

    private void Start()
    {
        numStates = 5;
        minState = 0;
        StartCoroutine(FlashLight());
    }

    IEnumerator FlashLight()
    {
        while (!game_controller.gameInstance.GetGameOver())
        {
            if (lightPanel.GetComponent<Image>().color == lightColor)
            {
                lightPanel.GetComponent<Image>().color = Color.black;
            }
            else
            {
                lightPanel.GetComponent<Image>().color = lightColor;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void LateUpdate()
    {
        if(value == "1")
        {
            lightColor = Color.red;
        }
        if(value == "2")
        {
            lightColor = Color.blue;
        }
        if (value == "3")
        {
            lightColor = Color.green;
        }
        if (value == "4")
        {
            lightColor = Color.yellow;
        }
        if (value == "0")
        {
            lightColor = Color.grey;
        }
    }
}
