using System;
using UnityEngine;

public abstract class Operation : MonoBehaviour
{
    [Header("Optional")]
    [SerializeField] protected Operation nextOperation;

    public virtual void DoAction()
    {
        if (nextOperation != null)
        {
            nextOperation.DoAction();
        }
    }

    public virtual void UnDoAction()
    {
        
    }
}
