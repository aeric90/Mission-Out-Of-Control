using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinLossMessage : MonoBehaviour
{
    public static WinLossMessage winLossInstance;

    public bool won;

    void Awake()
    {
        if (winLossInstance == null) { winLossInstance = this; }

        DontDestroyOnLoad(gameObject);
    }
}
