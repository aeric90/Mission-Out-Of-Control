using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentControl : MonoBehaviour
{

    public string value;

    public ParentControl()
    {
        value = "";
    }

    public string GetValue()
    {
        return value;
    }

}
