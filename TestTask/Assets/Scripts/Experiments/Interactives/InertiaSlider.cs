using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InertiaSlider : MonoBehaviour
{
    [SerializeField] private Animator expAnimator;
    [SerializeField] private string triggerName;
    [SerializeField] private string firstAnimationName;
    [SerializeField] private VerticalTouchSlider verticalTouchSlider;
    [SerializeField] private RectTransform slider;
    [SerializeField] private float maxValue;
    private void Start()
    {
        verticalTouchSlider.OnValueChanged += OnSliderValueChange;
    }

    private void OnSliderValueChange()
    {
        if (slider.localPosition.x == maxValue)
        {
            if(expAnimator.GetCurrentAnimatorStateInfo(0).IsName(firstAnimationName))
            {
                expAnimator.SetTrigger(triggerName);
                verticalTouchSlider.sensitivity++;
                verticalTouchSlider.ResetSlider();
            }
            else
            {
                expAnimator.SetTrigger(triggerName);
            }
        }
    }
}
