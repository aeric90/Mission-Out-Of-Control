using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerGameStart : MonoBehaviour
{
    public GameObject overlayCanvas;

    public void OnButtonPress()
    {
        game_controller.gameInstance.StartGame();

        overlayCanvas.SetActive(false);
    }
}
