using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ui_controller : MonoBehaviour
{
    public static ui_controller uiInstance;

    public TMPro.TextMeshProUGUI countDownText;
    public TMPro.TextMeshProUGUI computerText;

    void Start()
    {
        if(uiInstance == null) { uiInstance = this; }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateCountDown(string timerString)
    {
        countDownText.text = timerString;
    }
}
