using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CopyManualLink : MonoBehaviour
{
    public GameObject ManualText;

    public void OnButtonPress()
    {
        TextEditor text = new TextEditor();
        text.content = new GUIContent(ManualText.GetComponent<TMPro.TMP_Text>().text);
        text.SelectAll();
        text.Copy();
    }
}
