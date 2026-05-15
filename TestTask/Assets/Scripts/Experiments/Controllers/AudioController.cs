using System.Collections;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    private static readonly int AudioTrigger = Animator.StringToHash("Audio");

    [SerializeField] private Animator animator;

    public AudioSource[] oneShootAudios;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }
    public AudioSource[] loopAudios;

    private bool _isPaused;

    private bool Paused => AudioListener.pause;

    #region One-Shots

    public void PlayOneShoot(int numberSound)
    {
        if (!TryGetOneShot(numberSound, out var src, out var clip))
            return;

        src.PlayOneShot(clip);
    }

    public void PlayOneShootWithTrigger(int numberSound) =>
        StartCoroutine(PlayOneShootWithTriggerInternal(numberSound));

    private IEnumerator PlayOneShootWithTriggerInternal(int numberSound)
    {
        if (!TryGetOneShot(numberSound, out var src, out var clip))
            yield break;

        src.PlayOneShot(clip);

        yield return new WaitWhile(() => Paused || src.isPlaying);

        animator.SetTrigger(AudioTrigger);
    }

    public void StopOneShoot(int numberSound)
    {
        if (TryGetOneShot(numberSound, out var src, out _))
            src.Stop();
    }

    public void StopAllOneShots()
    {
        if (oneShootAudios == null) return;
        foreach (var oneShot in oneShootAudios)
            if (oneShot && oneShot.isPlaying)
                oneShot.Stop();
    }

    #endregion

    #region Loops

    public void PlayLoop(int numberSound)
    {
        if (!TryGetLoop(numberSound, out var src))
            return;

        if (!src.isPlaying)
            src.Play();
    }

    public void StopLoop(int numberSound)
    {
        if (TryGetLoop(numberSound, out var src) && src.isPlaying)
            src.Stop();
    }

    #endregion

    #region Pause/Resume

    public void PauseAll()
    {
        _isPaused = true;
    }

    public void ResumeAll()
    {
        _isPaused = false;
    }

    #endregion

    #region Helpers

    private bool TryGetOneShot(int index, out AudioSource src, out AudioClip clip)
    {
        src = null;
        clip = null;

        if (oneShootAudios == null || index < 0 || index >= oneShootAudios.Length)
            return false;

        src = oneShootAudios[index];
        if (!src) return false;

        clip = src.clip;
        if (!clip)
        {
            Debug.LogWarning($"[AudioController] OneShot[{index}] не имеет clip.");
            return false;
        }
        return true;
    }

    private bool TryGetLoop(int index, out AudioSource src)
    {
        src = null;
        if (loopAudios == null || index < 0 || index >= loopAudios.Length)
            return false;

        src = loopAudios[index];
        return src;
    }

    #endregion
}