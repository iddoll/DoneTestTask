using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exp1Physic9 : MonoBehaviour
{
    [SerializeField] private Animator expAnimator;
    [SerializeField] private TouchSlider touchSlider;
    [SerializeField] private string triggerName;
    [SerializeField] private string animationName;
    [SerializeField] private RectTransform slider;
    [SerializeField] private int angle;
    private bool isSwipeEnded;

    private void Start()
    {
        touchSlider.OnValueChanged += OnSliderValueChange;
    }
    
    private void OnSliderValueChange()
    {
        if(!expAnimator.GetCurrentAnimatorStateInfo(0).IsName(animationName)) return;
        float sliderValue = 360 - slider.localEulerAngles.z;
        int y = (int) transform.localEulerAngles.y - 360;
        if (sliderValue != 360 && !isSwipeEnded)
        {
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, -sliderValue/2 - 30f, transform.localEulerAngles.z);
        }
        if (y == angle)
        {
            expAnimator.SetTrigger(triggerName);
            isSwipeEnded = true;
        }
    }
}
