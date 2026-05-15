using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TouchSlider : EventValue, IDragHandler 
{
    public Action OnValueChanged;
    [SerializeField] public UnityEvent OnMinEvent;
    [SerializeField] public UnityEvent OnMaxEvent;
    [SerializeField] private int minValue;
    [SerializeField] private int maxValue;
    [SerializeField] private RectTransform slider;
    [SerializeField] private bool isInverse;
    [SerializeField] private RectTransform mainSlider;

    public void OnDrag(PointerEventData eventData)
    {
        float angle = 0f;
        if (mainSlider != null)
        {
            if (isInverse)
            {
                Vector2 sliderPosition = slider.position;
                Vector2 direction = eventData.position - sliderPosition;
                angle = -Vector2.Angle(mainSlider.right, direction) + 30f;
                angle = Mathf.Clamp(angle, maxValue, minValue);
            }
            else
            {
                Vector2 sliderPosition = slider.position;
                Vector2 direction = eventData.position - sliderPosition;
                angle = -Vector2.Angle(mainSlider.right, direction) + 30f;
                angle = Mathf.Clamp(angle, minValue, maxValue);
            }
        }
        else
        {
            if (isInverse)
            {
                Vector2 sliderPosition = slider.position;
                Vector2 direction = eventData.position - sliderPosition;
                angle = -Vector2.Angle(Vector2.right, -direction) + 30f;
                angle = Mathf.Clamp(angle, maxValue, minValue);
            }
            else
            {
                Vector2 sliderPosition = slider.position;
                Vector2 direction = eventData.position - sliderPosition;
                angle = -Vector2.Angle(Vector2.right, direction) + 30f;
                angle = Mathf.Clamp(angle, minValue, maxValue);
            }
        }

        slider.localEulerAngles = new Vector3(0f, 0f, angle);
        OnValueChanged?.Invoke();
        
        float value = CalculationValue(angle);

        CallFinishEvents(value);
    }

    private void CallFinishEvents(float value)
    {
        OnChangedValueEvent?.Invoke(value);
        
        if (Math.Abs(value - 0) < 0.01f)
        {
            OnMinEvent?.Invoke();
        }

        if (Math.Abs(value - 1) < 0.01f)
        {
            OnMaxEvent?.Invoke();
        }
    }

    private float CalculationValue(float angle)
    {
        return (angle - minValue)/(maxValue - minValue);
    }
}