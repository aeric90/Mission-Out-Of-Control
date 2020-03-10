using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KnobScript : MonoBehaviour
{
    public Transform handle;
    public Text valueText;
    public Vector3 mousePos;

    public void OnKnobTurn()
    {
        mousePos = Input.mousePosition;
        Vector2 dir = mousePos - handle.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        angle = (angle <= 0) ? (360 + angle) : angle;

        if (angle <= 225 || angle >= 315)
        {
            Quaternion r = Quaternion.AngleAxis(angle + 135f, Vector3.forward);
            handle.rotation = r;
            angle = ((angle >= 315) ? (angle - 360) : angle) + 45;
            float fillAmount = 0.75f - (angle / 360f);
            valueText.text = Mathf.Round((fillAmount * 100) / 0.75f).ToString();
        }
    }
}
