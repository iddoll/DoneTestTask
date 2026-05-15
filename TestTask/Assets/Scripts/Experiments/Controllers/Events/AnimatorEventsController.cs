using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimatorEventsController : MonoBehaviour
{
    private UnityEvent _OnStartOfExperiment;
    private UnityEvent _OnEndOfExperiment;

    public UnityEvent OnEndOfExperiment => _OnEndOfExperiment;

    public UnityEvent OnStartOfExperiment => _OnStartOfExperiment;

    public void Awake()
    {
        _OnEndOfExperiment = new UnityEvent();
        _OnStartOfExperiment = new UnityEvent();
    }

    public void EndExperiment()
    {
        _OnEndOfExperiment?.Invoke();
    }
    
    public void StartExperiment()
    {
        _OnStartOfExperiment?.Invoke();
    }
}
