using UnityEngine;

public class RotationObject : MonoBehaviour
{
    [Header("Rotation limit")]
    [SerializeField] private float minValue;
    [SerializeField] private float maxValue;

    [Header("Setting")]
    [SerializeField] private bool isInverse;
    [SerializeField] private bool isCenter;
    [SerializeField] private bool fromCurrentPosition;

    private void Start()
    {
        if (fromCurrentPosition)
        {
            minValue = minValue + transform.eulerAngles.z;
            
            if(isInverse)
                minValue = -minValue;
        }
    }

    public void OnChangeValue(float value)
    {
        float angle = 0f;
        float maxTemp = maxValue;
        
        //Center
        if (isCenter)
        {
            angle = value * maxValue + minValue;
            var radiusDistance = maxTemp - minValue;
            var centerСalculation = radiusDistance / 2;
            angle = centerСalculation - angle;
            transform.rotation = Quaternion.Euler(0, 0, angle);
            return;
        }
        //==>
        
        //fromCurrentPosition
        if (fromCurrentPosition)
        {
            angle = value * maxValue + minValue;
            maxTemp = maxValue + minValue;
        }
        else
        {
            angle = value * maxValue + minValue;
        }

        angle = Mathf.Clamp(angle, minValue, maxTemp);
        
        transform.rotation = Quaternion.Euler(0, 0, isInverse? -angle: angle);
    }
}
