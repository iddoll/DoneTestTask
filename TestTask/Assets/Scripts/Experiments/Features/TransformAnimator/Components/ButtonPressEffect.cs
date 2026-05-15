using System.Collections;
using UnityEngine;
using static Experiments.Features.TransformAnimator.EasingFunctions;

namespace Experiments.Features.TransformAnimator.Components
{
    public class ButtonPressEffect : MonoBehaviour
    {
        [SerializeField] private float pressScaleY = 0.6f;
        [SerializeField] private float pressScaleXZ = 1.3f;
        [SerializeField] private float animationDuration = 1f;
        [SerializeField] private AudioSource SoundFX;

        private Vector3 _originalScale;
        private bool _isAnimating = false;
    
        public void Press()
        {
            if (!_isAnimating)
            {
                _originalScale = transform.localScale;
                if(SoundFX!=null)SoundFX.Play();
                StartCoroutine(AnimatePress());
            }
        }

        private IEnumerator AnimatePress()
        {
            _isAnimating = true;
            float elapsedTime = 0;

            while (elapsedTime < animationDuration)
            {
                float scaleY = Mathf.LerpUnclamped(1f, pressScaleY, Mathf.Sin(EaseOutElastic((elapsedTime / animationDuration)) * Mathf.PI));
                float scaleXZ = Mathf.LerpUnclamped(1f, pressScaleXZ,Mathf.Sin(EaseOutElastic((elapsedTime / animationDuration)) * Mathf.PI));
                transform.localScale = new Vector3(_originalScale.x* scaleXZ, _originalScale.y * scaleY, _originalScale.z* scaleXZ);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        
            transform.localScale = _originalScale;
            _isAnimating = false;
        }
    }
}