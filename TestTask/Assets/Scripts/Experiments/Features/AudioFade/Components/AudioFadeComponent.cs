using System;
using System.Collections;
using UnityEngine;

namespace Features.AudioFade.Components
{
    public class AudioFadeComponent : MonoBehaviour
    {
        [SerializeField] private float fadeDuration = 1f;
        [SerializeField] private AudioSource audioSource;

        private float targetVolume;
        
        private Coroutine _fadeCoroutine;
        private bool _isOn = true;

        private void OnMouseDown() => ToggleMusic();

        private void Start()
        {
            targetVolume = audioSource.volume;
        }

        public void ToggleMusic()
        {
            if (_isOn)
                FadeOut();
            else
                FadeIn();

            _isOn = !_isOn;
        }

        private void FadeIn()
        {
            if (_fadeCoroutine != null)
                StopCoroutine(_fadeCoroutine);

            _fadeCoroutine = StartCoroutine(FadeAudio(targetVolume));
        }
        
        private void FadeOut()
        {
            if (_fadeCoroutine != null)
                StopCoroutine(_fadeCoroutine);

            _fadeCoroutine = StartCoroutine(FadeAudio(0f));
        }

        private IEnumerator FadeAudio(float targetVolume)
        {
            float startVolume = audioSource.volume;
            float elapsedTime = 0f;

            if (targetVolume > 0f && !audioSource.isPlaying)
                audioSource.Play();

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / fadeDuration);
                yield return null;
            }

            audioSource.volume = targetVolume;

            if (targetVolume == 0f)
                audioSource.Stop();
        }
    }
}