using UnityEngine;

public class OrientToCamera : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private bool lockYAxis = true;
    [SerializeField] private bool reverseDirection = false;
    private bool _hasCamera = false;

    void Start()
    {
        if (targetCamera==null) targetCamera = Camera.main;
        if (targetCamera != null) _hasCamera = true;
    }

    void Update()
    {
        if (_hasCamera)
        {
            Vector3 directionToCamera = targetCamera.transform.position - transform.position;
            
            if (lockYAxis)
            {
                directionToCamera.y = 0;
            }
            
            directionToCamera.Normalize();
            
            if (reverseDirection)
            {
                directionToCamera = -directionToCamera;
            }
            Quaternion targetRotation = Quaternion.LookRotation(directionToCamera);
            transform.rotation = targetRotation;
        }
    }
}