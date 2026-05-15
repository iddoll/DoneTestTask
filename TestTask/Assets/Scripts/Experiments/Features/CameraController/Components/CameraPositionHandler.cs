using UnityEngine;

namespace Features.CameraController.Components
{
    public class CameraPositionHandler : MonoBehaviour
    {
        public TouchController myButton;
        public CameraController CameraController;

        public Vector3 targetPosition;
        public Quaternion targetRotation;
        public float targetFOV;

        void Start()
        {
            if (myButton != null && CameraController != null)
            {
                myButton.OnTouchEvent.AddListener(() => 
                    CameraController.FocusCamera(targetPosition,targetRotation,targetFOV));
            }
        }
    }
}
