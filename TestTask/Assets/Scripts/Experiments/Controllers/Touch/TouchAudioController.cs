using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class TouchAudioController : MonoBehaviour
{
    [Header("Public Events")] public UnityEvent afterOnePlayEvent;
    public UnityEvent afterAllPlayEvent;
    [Header("Main elements")] public Animator animator;
    public string param;
    public AudioSource[] audioSources;
    [Header("Setting parameters")] public InteractiveType interactiveType;
    public PlaybackType playbackType;
    public AnimType animType;

    private bool _isInteractive = true;
    private bool _isStop;
    private int _currentAudioIndex = -1;

    public enum InteractiveType
    {
        Always,
        AfterSound,
        AfterAnimation,
        OnlyFinish
    }

    public enum PlaybackType
    {
        DefaultPlay,
        Cycle,
        Random
    }

    public enum AnimType
    {
        AnimPlayWithSound,
        AnimBeforePlaySound,
        AnimPlayAfterSound
    }

    private const string FinishLastClip = "OnFinishLastClipCompleted";

    private const int MediumNumberEventTemp = 3;

    private AnimationClip[] _animationClips;
    private AnimatorClipInfo[] _clipInfo;
    private bool _isEndExp;
    private TouchAudioAnimatorConnector _connector;

    private void Start()
    {
        if (animator != null)
        {
            _animationClips = GetAllAnimationClips();
            _connector = animator.gameObject.AddComponent<TouchAudioAnimatorConnector>();
            _connector.FinishLastClipEvent.AddListener(OnFinishLastClipCompleted);
            AddTempEvent();
        }
    }

    // Case for force end experiment
    private void OnDestroy()
    {
        ClearAnimEvent();
    }

    private void OnMouseDown()
    {
        if (interactiveType != InteractiveType.Always)
        {
            if (!_isInteractive) return;
        }

        _isInteractive = interactiveType == InteractiveType.Always;
        GetComponent<Collider>().enabled = _isInteractive;

        StopAllAudio();
        PlayAudio();
    }

    private void PlayAudio()
    {
        if (audioSources.Length == 0)
        {
            Debug.LogError("No audio sources found!");
            return;
        }

        if (animator != null && animType == AnimType.AnimBeforePlaySound)
        {
            animator.SetTrigger(param);
            return;
        }

        if (animator != null && animType == AnimType.AnimPlayWithSound)
        {
            animator.SetTrigger(param);
        }

        Play();
    }

    private void Play()
    {
        _currentAudioIndex = playbackType == PlaybackType.Random ? GetRandomIndex() : GetNextIndex();

        if (_isStop) return;

        AudioSource audioSource = audioSources[_currentAudioIndex];
        audioSource.Play();

        StartCoroutine(WaitForAudioComplete(audioSource));
    }

    private void FinishPlaySound()
    {
        if (animator != null && animType == AnimType.AnimPlayAfterSound)
        {
            animator.SetTrigger(param);
        }

        if (interactiveType == InteractiveType.AfterSound ||
            interactiveType == InteractiveType.OnlyFinish && animType == AnimType.AnimPlayWithSound)
        {
            _isInteractive = true;
            GetComponent<Collider>().enabled = _isInteractive;
        }

        afterOnePlayEvent?.Invoke();

        if (_currentAudioIndex >= audioSources.Length - 1)
        {
            afterAllPlayEvent?.Invoke();
        }
    }

    private int GetNextIndex()
    {
        switch (playbackType)
        {
            case PlaybackType.Cycle:
                return GetNextCycleIndex();
            case PlaybackType.Random:
                return GetRandomIndex();
        }

        return GetNextSequentialIndex();
    }

    private int GetNextSequentialIndex()
    {
        if (_currentAudioIndex >= audioSources.Length - 1)
        {
            Debug.Log("Reached the end of the song list.");
            _isStop = true;
            return _currentAudioIndex;
        }

        _currentAudioIndex++;

        return _currentAudioIndex;
    }

    private int GetNextCycleIndex()
    {
        var index = (_currentAudioIndex + 1) % audioSources.Length;
        return index;
    }

    private int GetRandomIndex()
    {
        return Random.Range(0, audioSources.Length);
    }

    private IEnumerator WaitForAudioComplete(AudioSource audioSource)
    {
        yield return new WaitForSeconds(audioSource.clip.length);
        FinishPlaySound();
    }

    public void StopAllAudio()
    {
        foreach (AudioSource audioSource in audioSources)
        {
            audioSource.Stop();
        }
    }

    public void ClearAudio()
    {
        StopAllAudio();
        _isInteractive = true;
        _currentAudioIndex = 0;

        if (interactiveType == InteractiveType.Always)
        {
            GetComponent<Collider>().enabled = true;
        }
    }

    private AnimationClip[] GetAllAnimationClips()
    {
        RuntimeAnimatorController runtimeController = animator.runtimeAnimatorController;
        return runtimeController.animationClips;
    }

    private void AddTempEvent()
    {
        var lastClip = _animationClips[_animationClips.Length - 1];
        float eventTime = lastClip.averageDuration;
        AddAnimationEvent(lastClip, FinishLastClip, eventTime * 0.8f);
    }

    private void AddAnimationEvent(AnimationClip clip, string functionName, float time)
    {
        var animationEvent = new AnimationEvent
        {
            time = time,
            functionName = functionName,
            objectReferenceParameter = clip
        };
        clip.AddEvent(animationEvent);
    }

    /// <summary>
    /// Func animation events
    /// </summary>
    private void OnFinishLastClipCompleted(AnimationEvent clip)
    {
        if (interactiveType == InteractiveType.AfterAnimation || interactiveType == InteractiveType.OnlyFinish &&
            animType == AnimType.AnimPlayAfterSound)
        {
            _isInteractive = true;
            GetComponent<Collider>().enabled = _isInteractive;
        }

        if (animType == AnimType.AnimBeforePlaySound)
        {
            Play();
        }
    }

    private void ClearAnimEvent()
    {
        if (animator == null) return;

        foreach (var clip in _animationClips)
        {
            RemoveTempEventByClip(clip);
        }
    }

    private void RemoveTempEventByClip(AnimationClip clip)
    {
        RemoveEvent(clip, FinishLastClip);
    }

    private void RemoveEvent(AnimationClip clip, string functionName)
    {
        AnimationEvent[] events = clip.events;

        List<AnimationEvent> filteredEvents = new List<AnimationEvent>();
        foreach (var ev in events)
        {
            if (ev.functionName != functionName)
            {
                filteredEvents.Add(ev);
            }
        }

        clip.events = new AnimationEvent[0];

        foreach (var ev in filteredEvents)
        {
            clip.AddEvent(ev);
        }
    }
}