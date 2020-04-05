using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ParentControl;

public class KnobScript : ParentControl
{    
    public Transform handle;
    public TMPro.TextMeshProUGUI screenText;

    private void Awake()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector2 dir = mousePos - handle.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        angle = (angle <= 0) ? (360 + angle) : angle;

        Quaternion r = Quaternion.AngleAxis(angle + 135f, Vector3.forward);

        r.z = 0.1f;
        r.w = -1.0f;

        controlType = "knob";
        maxValue = 5;
        minValue = 1;
        dependantSource = true;
        dependantTarget = true;
    }

    override public void SetValue(string value)
    {
        valueChange = true;
        this.value = screenText.text = value;

        Quaternion r = Quaternion.AngleAxis(135f, Vector3.forward);

        switch (value)
        {
            case "1":
                r.z = 0.1f;
                r.w = -1.0f;
                break;
            case "2":
                r.z = 0.7f;
                r.w = -0.8f;
                break;
            case "3":
                r.z = 0.9f;
                r.w = -0.3f;
                break;
            case "4":
                r.z = 1.0f;
                r.w = 0.2f;
                break;
            case "5":
                r.z = -0.7f;
                r.w = -0.7f;
                break;
        }

        handle.rotation = r;
    }

    public void OnKnobTurn()
    {
        valueChange = true;
        if (GetActive())
        {
            Vector3 mousePos = Input.mousePosition;
            Vector2 dir = mousePos - handle.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            angle = (angle <= 0) ? (360 + angle) : angle;

            if (angle <= 225 || angle >= 315)
            {
                Quaternion r = Quaternion.AngleAxis(angle + 135f, Vector3.forward);

                if (r.z < 0.7f && r.z > 0.0f && r.w < -0.7f)
                {
                    r.z = 0.1f;
                    r.w = -1.0f;

                    this.value = screenText.text = "1";
                }

                else if (r.z < 0.9f && r.z > 0.0f && r.w < -0.4f)
                {
                    r.z = 0.7f;
                    r.w = -0.8f;

                    this.value = screenText.text = "2";
                }

                else if (r.z < 1.0f && r.z > 0.0f && r.w < 0.0f)
                {
                    r.z = 0.9f;
                    r.w = -0.3f;

                    this.value = screenText.text = "3";
                }

                else if (r.z > 0.9f && r.w > 0.0f)
                {
                    r.z = 1.0f;
                    r.w = 0.2f;

                    this.value = screenText.text = "4";
                }

                else if (r.z < 0.0f && r.w < 0.0f)
                {
                    r.z = -0.7f;
                    r.w = -0.7f;

                    this.value = screenText.text = "5";
                }

                handle.rotation = r;
                this.valueChange = true;
            }

        }
    }
}
