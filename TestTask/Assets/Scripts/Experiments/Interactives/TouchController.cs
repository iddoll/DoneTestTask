using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TouchController : MonoBehaviour
{
    public UnityEvent OnTouchEvent = new UnityEvent();
    [SerializeField] private Animator expAnimator;
    [SerializeField] private string triggerName;
    [SerializeField] private string animationName;
    [SerializeField] private GameObject objectToDisable;
    [SerializeField] private bool hasToDisableCollider = true;
    [SerializeField] private bool hasToRayCastThrough;
    [SerializeField] private List<GameObject> objectsToDisable;
    [SerializeField] private List<GameObject> objectsToEnable;

    void OnMouseDown()
    {
        if (string.IsNullOrEmpty(animationName))
        {
            expAnimator.SetTrigger(triggerName);
            if(hasToDisableCollider)
                gameObject.GetComponent<Collider>().enabled = false;
            OnTouchEvent?.Invoke();
            DisableObjects();
            EnableObjects();
            return;
        }

        if(expAnimator.GetCurrentAnimatorStateInfo(0).IsName(animationName))
        {
            expAnimator.SetTrigger(triggerName);
            if(hasToDisableCollider)
                gameObject.GetComponent<Collider>().enabled = false;
            OnTouchEvent?.Invoke();
            DisableObjects();
            EnableObjects();
        }
        else if (hasToRayCastThrough)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 50f))
            {
                hit.transform.gameObject.SendMessage("OnMouseDown");
            }
        }
    }

    private void DisableObjects()
    {
        if (objectToDisable != null)
        {
            objectToDisable.SetActive(false);
        }

        if (objectsToDisable != null)
        {
            foreach (var obj in objectsToDisable)
            {
                obj.SetActive(false);
            }
        }
    }

    private void EnableObjects()
    {
        if (objectsToEnable != null)
        {
            foreach (var obj in objectsToEnable)
            {
                obj.SetActive(true);
            }
        }
    }
    
    public void SetAnimationName(string newAnimationName)
    {
        animationName = newAnimationName;
    }
}
