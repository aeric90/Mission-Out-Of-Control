using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentControl : MonoBehaviour
{
    public class Control
    {
        public int value;

        public Control()
        {
            value = 0;
        }

        public int GetValue()
        {
            return value;
        }
    }
}
