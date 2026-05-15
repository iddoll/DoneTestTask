using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTriggerOperation : Operation
{
    [Header("Required")]
    [SerializeField] private Animator animator;
    [SerializeField] private string trigger;
    [SerializeField] private int numberOfInteractivity;

    [Header("Optional")] 
    [SerializeField] private Operation onSetTrigger;
    
    private int _counter;

    public override void DoAction()
    {
        _counter++;
        if (_counter == numberOfInteractivity)
        {
            if(onSetTrigger != null) onSetTrigger.DoAction();
            animator.SetTrigger(trigger);
            _counter = 0;
        }

        base.DoAction();
    }
}
