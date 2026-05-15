using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class VerticalTouchSlider : EventValue, IDragHandler
{
    [SerializeField] public UnityEvent OnMinEvent;
    [SerializeField] public UnityEvent OnMaxEvent;
    public Action OnValueChanged;
    public float sensitivity = 1f;
    [SerializeField] private float minValue;
    [SerializeField] private float maxValue;
    [SerializeField] private RectTransform handleAreaSlider;
    [SerializeField] private RectTransform slider;

    public void OnDrag(PointerEventData eventData)
    {
        float inverseX = handleAreaSlider.InverseTransformPoint(eventData.position).x;
        float x = Mathf.Clamp(inverseX * sensitivity, minValue, maxValue);
        slider.localPosition = new Vector3(x, slider.localPosition.y, slider.localPosition.z);
        OnChangedValueEvent?.Invoke(CalculationValue(x));
        
        if (x == maxValue)
        {
            OnMaxEvent?.Invoke();
        }

        if (x == minValue)
        {
            OnMinEvent?.Invoke();
        }
    }

    public void ResetSlider()
    {
        slider.localPosition = new Vector3(maxValue, slider.localPosition.y, slider.localPosition.z);
    }

    private float CalculationValue(float value)
    {
        return (value - minValue) / (maxValue - minValue);
    }
}
