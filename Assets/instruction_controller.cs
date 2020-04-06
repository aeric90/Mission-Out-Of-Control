using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class instruction_controller : MonoBehaviour
{
    public GameObject instructionImage;
    public Sprite[] instructionImages;
    private int currentInstruction = 0;
    public Button prevButton;
    public Button nextButton;

    void Update()
    {
        instructionImage.GetComponent<Image>().sprite = instructionImages[currentInstruction];

        nextButton.interactable = !(currentInstruction == 3);
        prevButton.interactable = !(currentInstruction == 0);

    }

    public void ClickToNext()
    {
        if(currentInstruction < 3)
        {
            currentInstruction++;
        }
    }

    public void ClickToPrev()
    {
        if (currentInstruction > 0)
        {
            currentInstruction--;
        }
    }

}
