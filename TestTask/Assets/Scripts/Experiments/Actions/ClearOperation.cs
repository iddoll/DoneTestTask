using System.Collections.Generic;
using UnityEngine;

public class ClearOperation : Operation
{
    [SerializeField] private List<PlayerAnswerController> playerAnswerControllers;
    
    public override void DoAction()
    {
        foreach (var playerAnswer in playerAnswerControllers)
        {
            playerAnswer.ClearAllOperations();
        }

        base.DoAction();
    }
}
