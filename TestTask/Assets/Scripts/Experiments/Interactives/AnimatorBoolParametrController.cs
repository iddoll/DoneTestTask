using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorBoolParametrController : MonoBehaviour
{
    [SerializeField] private Animator expAnimator;
    [SerializeField] private TouchController touchController;
    [SerializeField] private string boolName;
    [SerializeField] private bool boolValue;

    private void Start()
    {
        touchController.OnTouchEvent.AddListener(OnTouch);
    }

    private void OnTouch()
    {
        expAnimator.SetBool(boolName, boolValue);
    }
}