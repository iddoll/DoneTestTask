using System.Collections;
using Features.OnClickCallback.Components;
using UnityEngine;

namespace Features.CameraFocus.Components
{
    public class CameraFocusComponent : MonoBehaviour
    {
        [SerializeField]
        private Camera targetCamera;

        [SerializeField]
        private float focusSpeed = 1f;

        [SerializeField]
        private Transform targetObject;

        [SerializeField]
        private Vector3 tiltAngle = Vector3.zero;

        [SerializeField]
        private OnClickCallbackComponent focusButton;

        [SerializeField]
        private OnClickCallbackComponent backButton;

        private Vector3 _initialPosition;
        private Quaternion _initialRotation;
        private Coroutine _focusCoroutine;
        private bool _isFocused;

        private void SetCamera()
        {
            if (targetCamera == null)
                targetCamera = Camera.main;
        }

        private void FocusOnTargetObject()
        {
            if (targetObject == null)
            {
                Debug.LogError("Target object is not assigned!");
                return;
            }

            SetCameraInitialPosition();

            float scaleFactor = targetObject.lossyScale.z;
            Vector3 targetPosition = targetObject.position + targetObject.forward * -5f * scaleFactor + Vector3.up * (2f * scaleFactor);
            Quaternion targetRotation = Quaternion.LookRotation(targetObject.position - targetPosition) * Quaternion.Euler(tiltAngle);

            FocusOnPosition(targetPosition, targetRotation);
        }

        private void ResetCameraPosition()
        {
            if (_focusCoroutine != null)
                StopCoroutine(_focusCoroutine);

            _focusCoroutine = StartCoroutine(FocusRoutine(_initialPosition, _initialRotation));
        }

        private void FocusOnPosition(Vector3 position, Quaternion rotation)
        {
            if (_focusCoroutine != null)
                StopCoroutine(_focusCoroutine);

            _focusCoroutine = StartCoroutine(FocusRoutine(position, rotation));
        }

        private IEnumerator FocusRoutine(Vector3 targetPosition, Quaternion targetRotation)
        {
            if (targetCamera == null)
            {
                Debug.LogError("Target camera is not assigned!");
                yield break;
            }

            Vector3 startPosition = targetCamera.transform.position;
            Quaternion startRotation = targetCamera.transform.rotation;

            float elapsed = 0f;

            while (elapsed < 1f)
            {
                elapsed += Time.deltaTime * focusSpeed;
                float t = Mathf.Clamp01(elapsed);

                targetCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
                targetCamera.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);

                yield return null;
            }

            targetCamera.transform.position = targetPosition;
            targetCamera.transform.rotation = targetRotation;

            if (!_isFocused)
                SetMsCameraControllerState(true);
        }

        private void SetCameraInitialPosition()
        {
            if (targetCamera == null)
                return;

            _initialPosition = targetCamera.transform.position;
            _initialRotation = targetCamera.transform.rotation;
        }

        private void OnFocusButtonClick()
        {
            if (_isFocused)
                return;

            _isFocused = true;

            SetMsCameraControllerState(false);
            FocusOnTargetObject();
        }

        private void OnBackButtonClick()
        {
            if (!_isFocused)
                return;

            _isFocused = false;
            ResetCameraPosition();
        }

        private void SetMsCameraControllerState(bool state)
        {
            MSCameraController[] cameraControllers = FindObjectsOfType<MSCameraController>();

            foreach (MSCameraController cameraController in cameraControllers)
                cameraController.enabled = state;
        }

        private void Awake()
        {
            SetCamera();
            
            focusButton.OnClick += OnFocusButtonClick;
            backButton.OnClick += OnBackButtonClick;
        }

        private void OnDestroy()
        {
            focusButton.OnClick -= OnFocusButtonClick;
            backButton.OnClick -= OnBackButtonClick;
        }
    }
}
