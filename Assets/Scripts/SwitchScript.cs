using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SwitchScript : MonoBehaviour
{
    public Button Switch;
    public Sprite switchOn;
    public Sprite switchOff;
    private int counter = 0;

    
    // Start is called before the first frame update
    void Start()
    {
        Switch = GetComponent<Button>();
    }

    public void changeButton()
    {
        
        counter++;
        if(counter % 2 == 1)
        {
            Switch.image.overrideSprite = switchOn;
        }
        else
        {
            Switch.image.overrideSprite = switchOff;
        }
    }
    // Update is called once per frame
}
