using UnityEngine;

public class TouchSwing : MonoBehaviour
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
        float sliderValue = 360 - slider.eulerAngles.z;
        int z = (int) transform.localEulerAngles.z;
        if (sliderValue != 360 && !isSwipeEnded)
        {
            transform.localEulerAngles = new Vector3(0f, 0f, sliderValue/2);
        }
        if (z == angle && expAnimator.GetCurrentAnimatorStateInfo(0).IsName(animationName))
        {
            transform.localEulerAngles = Vector3.zero;
            expAnimator.SetTrigger(triggerName);
            isSwipeEnded = true;
        }
    }
}