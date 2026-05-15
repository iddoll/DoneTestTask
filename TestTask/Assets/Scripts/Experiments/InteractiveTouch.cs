using UnityEngine;

public class InteractiveTouch : MonoBehaviour
{
    [SerializeField] private Animator expAnimator;
    [SerializeField] private string triggerName;
    [SerializeField] private string startAnimationName;
    [SerializeField] private string endAnimationName;
    private bool _isTransparent, _isStart;

    private void OnMouseDown()
    {
        if (_isStart || expAnimator.GetCurrentAnimatorStateInfo(0).IsName(startAnimationName))
        {
            _isStart = true;
            _isTransparent = !_isTransparent;
            expAnimator.SetBool(triggerName, _isTransparent);
        }
        
        if (_isStart && expAnimator.GetCurrentAnimatorStateInfo(0).IsName(endAnimationName))
        {
            gameObject.GetComponent<Collider>().enabled = false;
        }
    }
}