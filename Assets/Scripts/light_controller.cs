using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class light_controller : ParentControl
{
    public GameObject lightObject;
    private Sprite lightSprite;
    private Sprite shadowSprite;

    public Sprite green;
    public Sprite greenShadow;
    public Sprite blue;
    public Sprite blueShadow;
    public Sprite yellow;
    public Sprite yellowShadow;
    public Sprite red;
    public Sprite redShadow;
    public Sprite off;
    public Sprite offShadow;

    private void Awake()
    {
        maxValue = 4;
        minValue = 0;
        dependantSource = false;
        dependantTarget = true;
    }

    private void Start()
    {
        lightSprite = lightObject.GetComponent<Image>().sprite;
        shadowSprite = lightObject.transform.GetChild(0).gameObject.GetComponent<Image>().sprite;

        StartCoroutine(FlashLight());
    }

    IEnumerator FlashLight()
    {
        while (!game_controller.gameInstance.GetGameOver())
        {
            if (lightObject.GetComponent<Image>().sprite == lightSprite)
            {
                lightObject.GetComponent<Image>().sprite = off;
                lightObject.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = offShadow;
            }
            else
            {
                lightObject.GetComponent<Image>().sprite = lightSprite;
                lightObject.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = shadowSprite;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void LateUpdate()
    {
        if(value == "1")
        {
            lightSprite = red;
            shadowSprite = redShadow;
        }
        if(value == "2")
        {
            lightSprite = blue;
            shadowSprite = blueShadow;
        }
        if (value == "3")
        {
            lightSprite = green;
            shadowSprite = greenShadow;
        }
        if (value == "4")
        {
            lightSprite = yellow;
            shadowSprite = yellowShadow;
        }
        if (value == "0")
        {
            lightSprite = off;
            shadowSprite = offShadow;
        }
    }
}
