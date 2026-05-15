#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;
using UnityEngine;

public class SoundEventsGenerator : MonoBehaviour
{
    public Dictionary<AnimatorState, AnimationInfo> animationInfos = new Dictionary<AnimatorState, AnimationInfo>();
    private AudioController audioController;
    private GameObject audioHolder;
    private Transform audioHolderTransform;
    
    public void SetAnimation(AnimatorState animatorState, AnimationClip animationClip)
    {
        audioController = gameObject.GetComponent<AudioController>();
        if (audioController == null)
        {
            audioController = gameObject.AddComponent<AudioController>();
        }

        if (!animationInfos.Keys.Contains(animatorState))
            animationInfos.Add(animatorState, new AnimationInfo(animatorState, animationClip, audioController, this));
    }

    public void ClearAllAnimations()
    {
        animationInfos.Clear();
    }

    public Animator GetAnimator()
    {
        return GetComponent<Animator>();
    }

    public void UpdateAudioController()
    {
        var audioTransform = gameObject.transform.Find("Audio");
        if (audioTransform == null)
        {
            audioTransform = new GameObject {name = "Audio"}.transform;
            audioTransform.parent = gameObject.transform;
        }

        audioHolder = audioTransform.gameObject;

        List<AudioSource> oldAudioSources = audioController.oneShootAudios.ToList();
        oldAudioSources.RemoveAll(x => x == null);
        List<string> audioNames = new List<string>();
        
        foreach (var audioSource in oldAudioSources)
        {
            if (audioSource.clip != null)
            {
                audioNames.Add(audioSource.clip.name);
            }
        }
        int index = audioNames.Count;
        foreach (var animationInfo in animationInfos.Values)
        {
            foreach (var audio in animationInfo.audios)
            {
                if(audio.audioClip == null) continue;
                
                if (!audioNames.Contains(audio.audioClip.name))
                {
                    audio.audioControllerIndex = index;
                    index++;
                    var newAudioSource = new GameObject {name = audio.audioClip.name}.AddComponent<AudioSource>();
                    newAudioSource.transform.parent = audioHolder.transform;
                    newAudioSource.clip = audio.audioClip;
                    oldAudioSources.Add(newAudioSource);
                }
                else
                {
                    audio.audioControllerIndex = audioNames.FindIndex(sName => sName.Equals(audio.audioClip.name));
                }
            }
        }

        audioController.oneShootAudios = oldAudioSources.ToArray();
    }

    public void DeleteAudioSource()
    {
        if (UnityEditor.PrefabUtility.IsPartOfPrefabInstance(gameObject.transform.parent.gameObject))
        {
            var prefabPath =
                UnityEditor.PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(
                    gameObject.transform.parent.gameObject);
            var prefabRoot = UnityEditor.PrefabUtility.LoadPrefabContents(prefabPath);
            var childTransform = prefabRoot.transform.GetChild(0);
            audioHolderTransform = childTransform.Find("Audio");
            DestroyImmediate(audioHolderTransform.gameObject, true);
            Instantiate(new GameObject(), childTransform).name = "Audio";
            UnityEditor.PrefabUtility.SaveAsPrefabAsset(prefabRoot, prefabPath);
            UnityEditor.PrefabUtility.UnloadPrefabContents(prefabRoot);
        }
    }
    
}
#endif