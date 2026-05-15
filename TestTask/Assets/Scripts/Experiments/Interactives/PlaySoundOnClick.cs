using UnityEngine;

public class PlaySoundOnClick : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    private void OnMouseDown()
    {
        PlaySoundOnClickInternal();
    }

    private void PlaySoundOnClickInternal()
    {
        CheckAudioSourceObject();
        audioSource.Play();
    }

    private void CheckAudioSourceObject()
    {
        if (!audioSource.gameObject.activeSelf)
            audioSource.gameObject.SetActive(true);
    }
}