using UnityEngine;
using UnityEngine.Events;

public class AnimatorEventObject : MonoBehaviour
{
    [SerializeField] public UnityEvent<string> OnStartKeyEvent;

    public Animator animator;
    [Tooltip("The 'key' to if you need to run a specific animation in the animation event. For example, if there are several events and each of them should start a specific animation")]
    public string key = "NaN";
    [Tooltip("")]
    public string param;
     
    public bool isBool;
    public bool isBoolValue;
    public bool isOnlyEvent;

    public void Execute()
    {
        if (isOnlyEvent)
        {
            OnStartKeyEvent?.Invoke(param);
            return;
        }

        if (isBool)
            animator.SetBool(param, isBoolValue);
        else
            animator.SetTrigger(param);
    }
}