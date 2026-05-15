using System.Collections;
using UnityEngine;
using static Experiments.Features.TransformAnimator.EasingFunctions;

namespace Experiments.Features.TransformAnimator.Components
{
    public class CameraAnimator : MonoBehaviour
    {
        [SerializeField] private GameObject Camera;
        [SerializeField] private Transform[] Targets;
        [SerializeField] private int CurrentTarget;
        [Space(10)]
        [SerializeField] private float duration = 1f;
        [SerializeField] private bool MoveToTarget;
    

        private GameObject _mainCamera;
        private Transform _TargetPosition;
        private bool _isAnimating;
        private bool _prevMoveToTarget;
        private bool _hasMainCamera;
        private bool _hasCamera;
        private bool _hasTarget;
    
    
        void Start()
        {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            if (Camera != null)
            {
                _hasCamera = true;
                Camera.gameObject.SetActive(false);
            }
            if (_mainCamera != null) _hasMainCamera=true;
        }
        private void OnDestroy()
        {
            if(_mainCamera!=null)_mainCamera.SetActive(true);
        }
        void Update()
        {
            if (MoveToTarget && MoveToTarget!=_prevMoveToTarget)
            {
                MoveCameraIn();
                _prevMoveToTarget = MoveToTarget;
            }
            else if (!MoveToTarget && MoveToTarget!=_prevMoveToTarget)
            {
                MoveCameraOut();
                _prevMoveToTarget = MoveToTarget;
            }
        }
        void SetTarget(int index)
        {
            if (Targets != null && index >= 0 && index < Targets.Length)
            {
                _TargetPosition = Targets[index];
                _hasTarget = true;
            }
            else
            {
                _hasTarget = false;
            }
        }
        public void MoveCameraIn()
        {
            if(_isAnimating || !_hasMainCamera || !_hasCamera) return;
            SetTarget(CurrentTarget);
            if (_hasTarget)
            {
                Camera.gameObject.SetActive(true);
                _mainCamera.SetActive(false);
                StartCoroutine(AnimateTransform(false));
            }
        }
        public void MoveCameraOut()
        {
            if(_isAnimating || !_hasMainCamera || !_hasCamera) return;
            if(_hasTarget) StartCoroutine(AnimateTransform(true));
        }

        private IEnumerator AnimateTransform(bool returnCamera)
        {
            _isAnimating = true;
            float elapsedTime = 0f;
            Vector3 initialPosition = returnCamera ? _TargetPosition.position : _mainCamera.transform.position;
            Quaternion initialRotation = returnCamera ? _TargetPosition.rotation : _mainCamera.transform.rotation;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / duration);
                float easedT = EaseInOutCubic(t);
            
                Vector3 endPosition = returnCamera ? _mainCamera.transform.position : _TargetPosition.position;
                Quaternion endRotation = returnCamera ? _mainCamera.transform.rotation : _TargetPosition.rotation;
            
                Camera.transform.position = Vector3.LerpUnclamped(initialPosition, endPosition, easedT);
                Camera.transform.rotation = Quaternion.Slerp(initialRotation, endRotation, easedT);

                yield return null;
            }

            if (returnCamera)
            {
                Camera.gameObject.SetActive(false);
                _mainCamera.SetActive(true);
            }
            _isAnimating = false;
        }
    }
}
