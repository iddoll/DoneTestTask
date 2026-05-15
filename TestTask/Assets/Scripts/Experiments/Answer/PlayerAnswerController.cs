using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerAnswerController : MonoBehaviour
{
    [SerializeField] private Animator expAnimator;
    [SerializeField] private List<PlayerAnswer> playerAnswers;

    private void OnMouseDown()
    {
        foreach (var playerAnswer in playerAnswers)
        {
            if(!expAnimator.GetCurrentAnimatorStateInfo(0).IsName(playerAnswer.AnimationName))
                continue;
            
            if(playerAnswer.hasDisableCollider)
                gameObject.GetComponent<Collider>().enabled = false;
            
            playerAnswer.CallOperation();
        }
    }

    public void ClearAllOperations()
    {
        foreach (var playerAnswer in playerAnswers)
        {
            if(playerAnswer.hasDisableCollider)
                gameObject.GetComponent<Collider>().enabled = true;
            
            playerAnswer.ClearOperations();
        }
    }
}
