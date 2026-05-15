using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RotateScreen : MonoBehaviour, IRotateUI
{
    public static RotateScreen instance;
    public UnityEvent OnChangeLeft;
    public UnityEvent OnChangeDown;
    bool isDownOrientation = true;
    public bool IsDownOrientation => isDownOrientation;

    public void RotateDown()
    {
        isDownOrientation = true;
    }

    public void RotateLeft()
    {
        isDownOrientation = false;
    }

    private void Awake()
    {
        if (!instance)
            instance = this;
        AddLister(this);
    }

    public void AddLister(IRotateUI rotateUI)
    {
        OnChangeLeft.AddListener(rotateUI.RotateLeft);
        OnChangeDown.AddListener(rotateUI.RotateDown);
    }

    void Update()
    {
        if ((Input.acceleration.x < -.75 || Input.GetKeyDown(KeyCode.I)) && isDownOrientation)
            OnChangeLeft?.Invoke();
        else if ((Input.acceleration.y < -.75 || Input.GetKeyDown(KeyCode.I)) && !isDownOrientation)
            OnChangeDown?.Invoke();
    }
}