using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerGameStart : MonoBehaviour
{
    public GameObject overlayCanvas;
    public GameObject sliderOne;
    public GameObject sliderTwo;

    public void OnButtonPress()
    {
        game_controller.gameInstance.StartGame();

        overlayCanvas.SetActive(false);
        sliderOne.GetComponent<AudioSource>().enabled = true;
        sliderTwo.GetComponent<AudioSource>().enabled = true;
    }
}
