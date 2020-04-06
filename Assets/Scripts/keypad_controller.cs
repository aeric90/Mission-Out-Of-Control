using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ParentControl;

public class keypad_controller : ParentControl
{
    public TMPro.TextMeshProUGUI screenText;

    public AudioClip[] audioSources;
    private void Awake()
    {
        controlType = "keypad";
        maxValue = 999;
        minValue = 0;
        dependantSource = true;
        dependantTarget = false;
    }

    void Start()
    {
        ClearScreen();
    }

    void Update()
    {
        screenText.text = this.value;
    }

    public void KeyClick(string keyName)
    {
        if (this.GetActive())
        {
            GetComponent<AudioSource>().clip = audioSources[Random.Range(0, audioSources.Length)];
            GetComponent<AudioSource>().Play();

            if (keyName == "clear")
            {
                ClearScreen();
            }
            else
            {
                if (this.value.Length < 3)
                {
                    this.value += keyName;
                }
            }
        }
    }

    public void ClearScreen()
    {
        this.value = "";
    }

    override public string GetRandomAnswer()
    {
        int randomValue = Random.Range(minValue, maxValue + 1);

        return randomValue.ToString("D3");
    }
}
