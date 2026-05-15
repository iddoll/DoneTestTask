#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditorInternal;
using UnityEngine;

public class AnimationInfo : ScriptableObject
{
    public SoundEventsGenerator soundEventsGenerator;
    private AudioController audioController;
    public AnimatorState animationState;
    public AnimationClip animationClip;
    public List<Audio> audios;
    public List<Audio> oldAudios = new List<Audio>();
    public ReorderableList reorderableAudios;
    public SerializedObject serialized;
    public bool showFoldout;

    public AnimationInfo(AnimatorState animationState, AnimationClip animationClip, AudioController audioController, SoundEventsGenerator eventsGenerator)
    {
        this.animationState = animationState;
        this.animationClip = animationClip;
        this.audioController = audioController;
        soundEventsGenerator = eventsGenerator;
        
        audios = new List<Audio>();
        GetEventsFromAnimator();
    }

    public void GetEventsFromAnimator()
    {
        var events = animationClip.events;
        for (int i = 0; i < events.Length; i++)
        {
            if (events[i].functionName.Equals("PlayOneShoot"))
            {
                if (events[i].intParameter < audioController.oneShootAudios.Length &&
                    audioController.oneShootAudios[events[i].intParameter] != null &&
                    audioController.oneShootAudios[events[i].intParameter].clip != null)
                {
                    var clip = audioController.oneShootAudios[events[i].intParameter].clip;
                    if (i < events.Length - 1)
                    {
                        audios.Add(new Audio(clip,
                            events[i].time, events[i + 1].time - events[i].time - clip.length,
                            events[i].intParameter, clip.length + events[i].time));
                    }
                    else
                    {
                        audios.Add(new Audio(clip,
                            events[i].time, 0, events[i].intParameter, clip.length + events[i].time));
                    }
                }
                else
                {
                    audios.Add(i < events.Length - 1
                        ? new Audio(events[i].time, 0.5f, events[i].intParameter, events[i + 1].time)
                        : new Audio(events[i].time, 0.5f, events[i].intParameter, animationClip.length));
                }
            }
        }
        oldAudios.Clear();
        foreach (var audio in audios)
        {
            oldAudios.Add(new Audio(audio));
        }
    }
    
    public void AddEvents()
    {
        List<AnimationEvent> oldAnimationEvents = animationClip.events.ToList();
        
        oldAnimationEvents.RemoveAll(x => x.functionName.Equals("PlayOneShoot"));
        oldAnimationEvents.Remove(oldAnimationEvents.FindLast(e => e.functionName.Equals("StopAllOneShots")));

        for (int i = 0; i < audios.Count; i++)
        {
            AnimationEvent animationEvent = new AnimationEvent();
            if (i == 0)
            {
                if (audios[i].startTime == 0)
                {
                    audios[i].startTime = 0.1f;
                }
                animationEvent.time = audios[i].startTime;
            }
            else
            {
                animationEvent.time =
                    audios[i].startTime;
            }

            animationEvent.functionName = "PlayOneShoot";
            animationEvent.intParameter = audios[i].audioControllerIndex;
            oldAnimationEvents.Add(animationEvent);
        }

        oldAnimationEvents.Remove(oldAnimationEvents.Find(e => e.functionName.Equals("StopAllOneShots")));
        
        AnimationEvent animationStopAtStartEvent = new AnimationEvent();
        animationStopAtStartEvent.functionName = "StopAllOneShots";
        animationStopAtStartEvent.time = 0f;
        
        oldAnimationEvents.Add(animationStopAtStartEvent);
        
        if (audios.Count > 0)
        {
            AnimationEvent animationEventStopAllOneShoot = new AnimationEvent();

            if (audios.Last().audioClip != null)
            {
                animationEventStopAllOneShoot.time = audios.Last().startTime + audios.Last().audioClip.length;
            }
            else
            {
                animationEventStopAllOneShoot.time = animationClip.length; 
            }
            
            animationEventStopAllOneShoot.functionName = "StopAllOneShots";
        
            oldAnimationEvents.Add(animationEventStopAllOneShoot);
        }
        
        
        AnimationUtility.SetAnimationEvents(animationClip, oldAnimationEvents.ToArray());
    }

    public void ClearAudios()
    {
        audios.Clear();
    }

    public void CheckForChanges()
    {
        if (oldAudios.Count != audios.Count)
        {
            if (oldAudios.Count < audios.Count && oldAudios.Find(a => a.Equals(audios.Last())) == null)
            {
                if (oldAudios.Count != 0)
                {
                    audios.Last().startTime = audios[audios.Count - 2].endTime + audios[audios.Count - 2].offsetTime;
                    audios.Last().offsetTime = 0.5f; 
                }
                else
                {
                    audios.Last().startTime = 0f;
                    audios.Last().offsetTime = 0.5f;
                }
            }
            soundEventsGenerator.UpdateAudioController();
            AddEvents();
            ClearAudios();
            GetEventsFromAnimator();
        }
        else
        {
            for (int i = 0; i < audios.Count; i++)
            {
                if (!audios[i].CompareWithOtherAudio(oldAudios[i]))
                {
                    soundEventsGenerator.UpdateAudioController();
                    AddEvents();
                    ClearAudios();
                    GetEventsFromAnimator();
                }
            }
        }
        
        oldAudios.Clear();
        foreach (var audio in audios)
        {
            oldAudios.Add(new Audio(audio));
        }
    }
}
#endif