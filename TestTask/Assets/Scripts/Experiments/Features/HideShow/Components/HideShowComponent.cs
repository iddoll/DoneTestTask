using System.Collections;
using UnityEngine;

namespace Features.HideShow.Components
{
    public class HideShowComponent : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        
        [Header("Hide")][Space]
        [SerializeField] private Transform objectToHide;
        [SerializeField] private float hideDuration = 0.5f;
        
        [Header("Show")][Space]
        [SerializeField] private Transform objectToShow;
        [SerializeField] private float showDuration = 0.5f;
        
        private bool _completed;

        private void OnMouseDown()
        {
            if (_completed)
                return;
            
            _completed = true;
            
            PlayAudio();
            StartCoroutine(HideShowObjects());
        }

        private void PlayAudio()
        {
            if (audioSource == null)
                return;
            
            audioSource.Play();
        }

        private IEnumerator HideShowObjects()
        {
            yield return HideObject();
            yield return ShowObject();
        }

        private IEnumerator HideObject()
        {
            Vector3 startScale = objectToHide.localScale;
            Vector3 endScale = new Vector3(0, 0, 0);

            float elapsedTime = 0f;

            while (elapsedTime < hideDuration)
            {
                objectToHide.localScale = Vector3.Lerp(startScale, endScale, elapsedTime / hideDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.localScale = endScale;
        }

        private IEnumerator ShowObject()
        {
            objectToShow.gameObject.SetActive(true);
            
            Vector3 startScale = new Vector3(0, 0, 0);
            objectToShow.localScale = startScale;
            
            Vector3 endScale = new Vector3(1, 1, 1);
            
            float elapsedTime = 0f;

            while (elapsedTime < showDuration)
            {
                objectToShow.localScale = Vector3.Lerp(startScale, endScale, elapsedTime / showDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            objectToShow.localScale = endScale;
        }
    }
}