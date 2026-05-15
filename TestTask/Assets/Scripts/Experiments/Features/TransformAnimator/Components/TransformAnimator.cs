using System.Collections;
using UnityEngine;

namespace Experiments.Features.TransformAnimator.Components
{
    internal enum AnimationType
    {
        Elastic,
        Bounce,
        InOutCubic,
        OutCubic
    }
    public class TransformAnimator : MonoBehaviour
    {
        [SerializeField] private AnimationType animationType = AnimationType.Elastic;
        [SerializeField] private Vector3 endPosition;
        [SerializeField] private Vector3 endRotation;
        [SerializeField] private Vector3 endScale;
        [SerializeField] private float duration = 2f;
        // [SerializeField] [Range(0f, 1f)]private float BackwardEventDelay = 0.5f;
        // public UnityEvent OnAnimationForwardEvent = new UnityEvent();
        // public UnityEvent OnAnimationBackwardEvent = new UnityEvent();
    
    
        private Vector3 _startPosition;
        private Vector3 _startRotation;
        private Vector3 _startScale;
        private bool _isAnimating = false;
        private bool _animCycleFlag;
    

        public void StartAnimation()
        {
            if (_isAnimating) return;
            StartCoroutine(AnimateTransform());
        }

        private void OnDisable()
        {
            _isAnimating = false;
        }

        private IEnumerator AnimateTransform()
        {
            // if(_animCycleFlag==false)OnAnimationForwardEvent?.Invoke();
            _isAnimating = true;
            float elapsedTime = 0f;
            Vector3 initialPosition = transform.localPosition;
            Vector3 initialRotation = transform.localRotation.eulerAngles;
            Vector3 initialScale = transform.localScale;
            bool hasTriggeredBackwardEvent = false;
        
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / duration);
                float easedT = ApplyEasingFunction(t);

                transform.localPosition = Vector3.LerpUnclamped(initialPosition, endPosition, easedT);
                Vector3 rotation = Vector3.LerpUnclamped(initialRotation, endRotation, easedT);
                transform.localRotation = Quaternion.Euler(rotation);
                Vector3 newScale = Vector3.LerpUnclamped(initialScale, endScale, easedT);
                transform.localScale = Vector3.Max(newScale, Vector3.zero);

                // if (_animCycleFlag == true && t >= BackwardEventDelay && !hasTriggeredBackwardEvent)
                // {
                //     OnAnimationBackwardEvent?.Invoke();
                //     hasTriggeredBackwardEvent = true;
                // }
            
                yield return null;
            }

            transform.localPosition = endPosition;
            transform.localScale = endScale;

            endPosition = initialPosition;
            _startPosition = transform.localPosition;
            endRotation = initialRotation;
            _startRotation = transform.localRotation.eulerAngles;
            endScale = initialScale;
            _startScale = transform.localScale;
        
            _animCycleFlag = !_animCycleFlag;
            _isAnimating = false;
        }
    

        private float ApplyEasingFunction(float t)
        {
            switch (animationType)
            {
                case AnimationType.Bounce:
                    return EasingFunctions.EaseOutBounce(t);
                case AnimationType.InOutCubic:
                    return EasingFunctions.EaseInOutCubic(t);
                case AnimationType.OutCubic:
                    return EasingFunctions.EaseOutCubic(t);
                case AnimationType.Elastic:
                default:
                    return EasingFunctions.EaseOutElastic(t);
            }
        }
    

    }
}