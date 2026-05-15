using UnityEngine;

/// <summary>
/// If necessary, use a script 'AnimationSliderAdaptor' to work with the slider
/// </summary>

public class AnimatorParametersController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    
    private void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    public void SetTrigger(string value)
    {
        animator.SetTrigger(value);
    }
    
    public void SetFloat(string param, float value)
    {
        animator.SetFloat(param, value);
    }
    
    public void SetInt(string param, int value)
    {
        animator.SetInteger(param, value);
    }
}
