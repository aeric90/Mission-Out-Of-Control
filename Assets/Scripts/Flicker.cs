using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Flicker : MonoBehaviour
{
    public GameObject redText;
    public GameObject redCircle;

    private int counter;

    void Start()
    {
        counter = 0;

        StartCoroutine(FlashingText());
    }

    IEnumerator FlashingText()
    {
        while (true)
        {
            if (counter == 0)
            {
                redText.SetActive(true);
                redCircle.SetActive(true);
                counter = 1;
            }
            else if(counter == 1)
            {
                redText.SetActive(false);
                redCircle.SetActive(false);
                counter = 0;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
}


