using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class CircleSlider : EventValue, IDragHandler 
{
    public Action OnValueChanged;
    [SerializeField] public UnityEvent OnMinEvent;
    [SerializeField] public UnityEvent OnMaxEvent;
    [SerializeField] private int minValue;
    [SerializeField] private int maxValue;
    [SerializeField] private RectTransform slider;
    [SerializeField] private bool isInverse;

    private bool isClick;
    private float startValue;
    private float startAngle;

    private void Start()
    {
        startAngle = 360f-slider.localEulerAngles.z;
        startValue = startAngle / 360f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 sliderPosition = slider.position;
        Vector3 direction = eventData.position - sliderPosition;

        float degreesAngle = Vector3.Angle(Vector3.down, direction);
        bool onTheRight = (eventData.position.x) > slider.position.x;
        int angle = !onTheRight ? (int)degreesAngle : 360-(int)degreesAngle;
        
        var sliderAngle = -angle ;
        float angleTemp = 0;
        
        if (onTheRight)
        {
            angleTemp = Math.Abs((int) startAngle + sliderAngle);
            sliderAngle = Mathf.Clamp(sliderAngle, -360, minValue-(int)startAngle);
        }
        else
        {
            angleTemp = 360f - Math.Abs((int) startAngle + sliderAngle);
            sliderAngle = Mathf.Clamp(sliderAngle, -180, 0);
        }

        //__________________
        slider.localEulerAngles = new Vector3(0f, 0f, sliderAngle);
        OnValueChanged?.Invoke();
        
        float value = CalculationValue(angleTemp);
        print("value - " + value);
        CallFinishEvents(value);
        //__________________
    }

    private float prevValue;
    
    private void CallFinishEvents(float value)  
    {
        OnChangedValueEvent?.Invoke(value);
        
        if (value < 0.01f)
        {
            Debug.Log("--> OnMinEvent");
            OnMinEvent?.Invoke();
        }

        if (value > 0.9f)
        {
            Debug.Log("--> OnMaxEvent");
            OnMaxEvent?.Invoke();
        }

        if (prevValue > 0.6f && value < 0.5f)
        {
            Debug.Log("--> OnMaxEvent");
            OnMaxEvent?.Invoke();
        }

        prevValue = value;
    }

    private float CalculationValue(float angle)
    {
        return (angle - minValue)/(maxValue - minValue);
    }
}