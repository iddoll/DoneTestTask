using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class PlayerAnswer
{
    [SerializeField] private bool isRight;
    [SerializeField] public bool hasDisableCollider = false;

    [SerializeField] private string animationName;
    [SerializeField] private List<Operation> rightOperations;
    [SerializeField] private List<Operation> wrongOperations;

    public string AnimationName => animationName;

    public void CallOperation()
    {
        if (isRight)
        {
            foreach (var rightOperation in rightOperations)
            {
                rightOperation.DoAction();
            }
        }
        else
        {
            foreach (var wrongOperation in wrongOperations)
            {
                wrongOperation.DoAction();
            }
        }
    }

    public void ClearOperations()
    {
        foreach (var operation in rightOperations)
        {
            if(operation != null)
                operation.UnDoAction();
        }
        
        foreach (var operation in wrongOperations)
        {
            if(operation != null)
                operation.UnDoAction();
        }
    }
}
