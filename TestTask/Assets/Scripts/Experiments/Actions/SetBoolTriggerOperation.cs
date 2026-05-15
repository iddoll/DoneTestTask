using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetBoolTriggerOperation : Operation
{
    [Header("Required")]
    [SerializeField] private Animator animator;
    [SerializeField] private string boolName;
    [SerializeField] private int numberOfInteractivity;

    [Header("Optional")] 
    [SerializeField] private Operation onSetTrigger;
    [SerializeField] private bool boolValue;

    private int _counter;

    public override void DoAction()
    {
        _counter++;
        if (_counter == numberOfInteractivity)
        {
            if(onSetTrigger != null) onSetTrigger.DoAction();
            animator.SetBool(boolName, boolValue);
            _counter = 0;
        }

        base.DoAction();
    }
}
