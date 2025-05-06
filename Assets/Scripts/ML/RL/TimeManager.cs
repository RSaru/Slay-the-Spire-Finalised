using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

public class TimeManager : MonoBehaviour
{
    public float slowTimeScale = 0.2f;

    void Update()
    {
        if (Academy.IsInitialized)
        {
            Time.timeScale = slowTimeScale;  // Force slow speed
        }
    }
}

