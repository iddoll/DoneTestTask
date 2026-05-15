using System.Collections;
using Features.OnClickCallback.Components;
using UnityEngine;

namespace Features.ObjectToObjectClick.Components
{
    public class ObjectToObjectClickComponent : MonoBehaviour
    {
        [SerializeField] private OnClickCallbackComponent expectedOnClickObject;
        [SerializeField] private GameObject objectToHighlight;
        [SerializeField] private GameObject finalObject;

        private bool _started;
        private bool _expectedObjectClicked;

        private void Awake() => expectedOnClickObject.OnClick += OnExpectedObjectClicked;

        private void OnDestroy() => expectedOnClickObject.OnClick -= OnExpectedObjectClicked;

        private void OnMouseDown()
        {
            if (_started)
                return;

            _started = true;
            objectToHighlight.SetActive(true);
        }

        private void OnExpectedObjectClicked()
        {
            if (!_started || _expectedObjectClicked)
                return;

            _expectedObjectClicked = true;
            objectToHighlight.SetActive(false);
            StartCoroutine(OnExpectedObjectClickedInternal());
        }

        private IEnumerator OnExpectedObjectClickedInternal()
        {
            yield return HideMainObject();
            finalObject.SetActive(true);
        }
        
        private IEnumerator HideMainObject()
        {
            var startScale = transform.localScale;
            var endScale = new Vector3(0, 0, 0);
            float duration = 1f;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                transform.localScale = Vector3.Lerp(startScale, endScale, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.localScale = endScale;
        }
    }
}