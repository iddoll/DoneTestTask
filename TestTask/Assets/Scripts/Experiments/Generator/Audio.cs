#if UNITY_EDITOR
using System;
using UnityEngine;

[Serializable]
public class Audio
{
    public AudioClip audioClip;
    public float startTime;
    public float offsetTime;
    public int audioControllerIndex;
    public float endTime;
    public int hashCode;

    public Audio(AudioClip audioClip, float startTime, float offsetTime, int audioControllerIndex, float endTime)
    {
        this.audioClip = audioClip;
        this.startTime = startTime;
        this.offsetTime = offsetTime;
        this.audioControllerIndex = audioControllerIndex;
        this.endTime = endTime;
        hashCode = GetHashCode();
    }

    public Audio(float startTime, float offsetTime, int audioControllerIndex, float endTime)
    {
        this.startTime = startTime;
        this.offsetTime = offsetTime;
        this.audioControllerIndex = audioControllerIndex;
        this.endTime = endTime;
        hashCode = GetHashCode();
    }

    public Audio(Audio audio)
    {
        audioClip = audio.audioClip;
        startTime = audio.startTime;
        offsetTime = audio.offsetTime;
        audioControllerIndex = audio.audioControllerIndex;
        endTime = audio.endTime;
        hashCode = audio.hashCode;
    }
    
    public Audio()
    {
        hashCode = GetHashCode();
    }

    public void UpdateInfo(float startTime)
    {
        this.startTime = startTime;
    }

    public bool CompareWithOtherAudio(Audio audio)
    {
        if (audio.audioClip != null)
        {
            return audioClip.Equals(audio.audioClip) && startTime == audio.startTime && offsetTime == audio.offsetTime &&
                   audioControllerIndex == audio.audioControllerIndex && endTime == audio.endTime;
        }

        return startTime == audio.startTime && offsetTime == audio.offsetTime &&
               audioControllerIndex == audio.audioControllerIndex && endTime == audio.endTime;
    }
}
#endif