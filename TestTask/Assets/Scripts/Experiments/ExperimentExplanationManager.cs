using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperimentExplanationManager : MonoBehaviour
{
    private bool explanationActive = true;
    private static ExperimentExplanationManager _instance;

    public static ExperimentExplanationManager Instance
    {
        get
        {
            return _instance;
        }
    }

    public bool ExplanationActive
    {
        get
        {
            return explanationActive;
        }

        set
        {
            explanationActive = value;
        }
    }

    private void Awake()
    {
        _instance = this;
    }
}
