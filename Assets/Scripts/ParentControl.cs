using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentControl : MonoBehaviour
{
    public class Control
    {
        public string value;

        public Control()
        {
            value = "";
        }

        public string GetValue()
        {
            return value;
        }
    }
}
