using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombineTouchController : MonoBehaviour
{
    [Header("Required")]
    [SerializeField] private List<TouchController> touchControllers;
    [SerializeField] private Animator animator;
    [SerializeField] private string triggerName;

    private int _counter;
    
    void Start()
    {
        foreach (var touch in touchControllers)
        {
            touch.OnTouchEvent.AddListener(OnTouch);
        }
    }

    private void OnTouch()
    {
        _counter++;
        if (_counter >= touchControllers.Count)
        {
            animator.SetTrigger(triggerName);
        }
    }
}
