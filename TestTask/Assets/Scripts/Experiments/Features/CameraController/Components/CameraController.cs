using System.Collections;
using UnityEngine;

namespace Features.CameraController.Components
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 0.1f;
        [SerializeField] private float zoomSpeed = 0.1f; 
        [SerializeField] private float rotateSpeed = 0.2f; 
        [SerializeField] private GameObject cameraRoot;
        [SerializeField] private Camera camera;
        [SerializeField] private Vector2 camMinMaxPos;
        [SerializeField] private Vector2 camMinMaxZoom;
        [SerializeField] private float damping = 0.9f;
        [SerializeField] private float speed = 5f;
        [SerializeField] private AudioSource CameraMoveSFX;

        private Vector3 targetPosition;
        private Quaternion targetRotation;
        private float targetFOV;

        private float moveVelocity = 0f;
        private float rotateVelocity = 0f;
        private float lastDistance = 0f;
        private bool isMoving = false;


        private void Start()
        {
            targetPosition = cameraRoot.transform.localPosition;
            targetRotation = cameraRoot.transform.localRotation;
            targetFOV = camera.fieldOfView;
        }

        void Update()
        {

            if (!isMoving)
            {
                if (Input.touchCount == 2)
                {
                    Touch touch1 = Input.GetTouch(0);
                    Touch touch2 = Input.GetTouch(1);

            
                    float deltaY = (touch1.deltaPosition.y + touch2.deltaPosition.y) / 2f;
            
                    moveVelocity = deltaY * moveSpeed * Time.deltaTime * camera.fieldOfView;
            
                    float currentDistance = Vector2.Distance(touch1.position, touch2.position);
            
                    if (lastDistance > 0) 
                    {
                        float deltaDistance = currentDistance - lastDistance;
                        float newZoom = Mathf.Clamp(camera.fieldOfView - deltaDistance * zoomSpeed * Time.deltaTime, camMinMaxZoom.x, camMinMaxZoom.y);
                        camera.fieldOfView = newZoom; 
                    }
            
                    lastDistance = currentDistance;
                }
                else if (Input.touchCount == 1)
                {
                    Touch touch = Input.GetTouch(0);
                    float deltaX = touch.deltaPosition.x;
                    rotateVelocity = deltaX * rotateSpeed * Time.deltaTime* Time.deltaTime;
                }
                else
                {
                    // --- Управление с мыши ---
                    if (Input.GetMouseButton(1))
                    {
                        float deltaY = Input.GetAxis("Mouse Y");
                        moveVelocity = deltaY * moveSpeed * Time.deltaTime * 3000f;
                    }
                    if (Input.GetMouseButton(0))
                    {
                        float deltaX = Input.GetAxis("Mouse X");
                        rotateVelocity = deltaX * rotateSpeed * Time.deltaTime * 5f;
                    }

            
                    float scroll = Input.GetAxis("Mouse ScrollWheel");
                    if (scroll != 0)
                    {
                        float newZoom = Mathf.Clamp(camera.fieldOfView - scroll * zoomSpeed * Time.deltaTime * 300f, camMinMaxZoom.x, camMinMaxZoom.y);
                        camera.fieldOfView = newZoom;
                    }
            
                    moveVelocity *= damping;
                    rotateVelocity *= damping;
                    lastDistance = 0;
                }
        
        
                float newY = Mathf.Clamp(cameraRoot.transform.localPosition.y + (-moveVelocity), camMinMaxPos.x, camMinMaxPos.y);
                cameraRoot.transform.localPosition = new Vector3(transform.localPosition.x, newY, transform.localPosition.z);
                cameraRoot.transform.Rotate(Vector3.up,rotateVelocity);
            }
        }

        public void ResetCamera()
        {
            if (isMoving)StopAllCoroutines();
            StartCoroutine(MoveCamera(targetPosition,targetRotation,targetFOV));
        }
        
        public void FocusCamera(Vector3 FocusPos,Quaternion FocusRot, float FocusFOV)
        {
            if (isMoving)StopAllCoroutines();
            StartCoroutine(MoveCamera(FocusPos,FocusRot,FocusFOV));
            if(CameraMoveSFX!=null)CameraMoveSFX.Play();
        }

        private IEnumerator MoveCamera(Vector3 FocusPos,Quaternion FocusRot, float FocusFOV)
        {
            isMoving = true;
            while (Vector3.Distance(cameraRoot.transform.localPosition, FocusPos) > 0.01f || Quaternion.Angle(cameraRoot.transform.localRotation, FocusRot) > 0.1f)
            {
                cameraRoot.transform.localPosition = Vector3.Lerp(cameraRoot.transform.localPosition, FocusPos, speed * Time.deltaTime);
                cameraRoot.transform.localRotation = Quaternion.Lerp(cameraRoot.transform.localRotation, FocusRot, speed * Time.deltaTime);
                camera.fieldOfView = Mathf.Lerp(camera.fieldOfView,FocusFOV,speed * Time.deltaTime);
                yield return null;
            }
            cameraRoot.transform.localPosition = FocusPos;
            cameraRoot.transform.localRotation = FocusRot;
            isMoving = false;
        }
    }
}