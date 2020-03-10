using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ParentControl;

public class KnobScript : ParentControl
{    public Transform handle;
    public Text valueText;

    public void OnKnobTurn()
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

                this.value = valueText.text = "1";
            }

            else if (r.z < 0.9f && r.z > 0.0f && r.w < -0.4f)
            {
                r.z = 0.7f;
                r.w = -0.8f;

                this.value = valueText.text = "2";
            }

            else if (r.z < 1.0f && r.z > 0.0f && r.w < 0.0f)
            {
                r.z = 0.9f;
                r.w = -0.3f;

                this.value = valueText.text = "3";
            }

            else if (r.z > 0.9f && r.w > 0.0f)
            {
                r.z = 1.0f;
                r.w = 0.2f;

                this.value = valueText.text = "4";
            }

            else if (r.z < 0.0f && r.w < 0.0f)
            {
                r.z = -0.7f;
                r.w = -0.7f;

                this.value = valueText.text = "5";
            }

            handle.rotation = r;

            Debug.Log(r);
            
        }
    }
}
