using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NailController : MonoBehaviour
{
    [SerializeField] private int nailAmount;
    [SerializeField] private Animator expAnimator;

    public Animator ExpAnimator
    {
        get => expAnimator;
        set => expAnimator = value;
    }

    private int nailCounter;

    public static NailController instance;

    private void Start()
    {
        instance = this;
    }

    public void addNail()
    {
        if (nailCounter < nailAmount - 1)
        {
            nailCounter++;
        }
        else
        {
            expAnimator.SetTrigger("Next");
        }
    }
    
    
}
