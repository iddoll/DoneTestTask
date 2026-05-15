using System.Collections;
using UnityEngine;
using static Experiments.Features.TransformAnimator.EasingFunctions;

namespace Features.Appearance.Components
{
    public class AppearanceComponent : MonoBehaviour
    {
        [SerializeField] private GameObject Root;
        [SerializeField] private bool AnimateOnStart;
        [SerializeField] private float Duration;
        private Vector3 _initialScale;
        private Vector3 _targetScale;
        private bool _isAnimating;
        private void Awake()
        {
            _targetScale = Root.transform.localScale;
            // _initialScale = new Vector3(0, 0, 0);
        }
        
        private void OnEnable()
        {
            if (AnimateOnStart) ScaleIn();

        }

        public void ScaleIn()
        {
            if (Root!=null)
            {
                if (_isAnimating)StopAllCoroutines();
                Vector3 _inScale = new Vector3(0,0,0);
                Vector3 _outScale = _targetScale;
                StartCoroutine(ScaleOverTime(_inScale,_outScale,Duration,false));
            }
        }

        public void ScaleOut()
        {
            if (Root!=null)
            {
                if (_isAnimating)StopAllCoroutines();
                Vector3 _inScale = Root.transform.localScale;
                Vector3 _outScale = new Vector3(0,0,0);
                StartCoroutine(ScaleOverTime(_inScale,_outScale,Duration,true));
            }
        }
        
        private IEnumerator ScaleOverTime(Vector3 startScale, Vector3 endScale, float duration,bool disableObj)
        {
            Root.SetActive(true);
            _isAnimating = true;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;
                float esedT = EaseOutElastic(t);
                Vector3 newScale = Vector3.LerpUnclamped(startScale, endScale, esedT);
                newScale = Vector3.Max(newScale, Vector3.zero);
                Root.transform.localScale = newScale;
                yield return null;
            }

            Root.transform.localScale = endScale;
            if(disableObj)Root.SetActive(false);
            _isAnimating = false;
        }
    }
}