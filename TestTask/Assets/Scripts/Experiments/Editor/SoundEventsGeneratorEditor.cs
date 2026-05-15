#if UNITY_EDITOR
using System;
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditorInternal;
using UnityEngine;
using AnimatorController = UnityEditor.Animations.AnimatorController;

[CustomEditor(typeof(SoundEventsGenerator))]
public class SoundEventsGeneratorEditor : Editor
{
    private AnimatorController animatorController;
    private SoundEventsGenerator soundEvents;

    private bool isShowHelpBox;
    //private bool isUpdateAnimator;

    public void OnEnable()
    {
        soundEvents = (SoundEventsGenerator) target;
        animatorController = (AnimatorController) soundEvents.GetAnimator().runtimeAnimatorController;
        GetAllAnimatorStates();
        // foreach (var animation in soundEvents.animationInfos.Values)
        // {
        //     animation.UpdateEvents();
        // }
        //EditorApplication.update += EditorUpdate;
    }

    // public void OnDisable()
    // {
    //     EditorApplication.update -= EditorUpdate;
    // }
    //
    // private void EditorUpdate()
    // {
    //     if (isUpdateAnimator)
    //     {
    //         soundEvents.GetAnimator().Update(Time.deltaTime);
    //     }
    // }

    public override void OnInspectorGUI()
    {
        UpdateSerialized();

        EditorGUILayout.HelpBox(
            "Слава Україні!\n\nСкрипт знаходиться на тестуванні, проханя продублювати префаб та анімації перед роботою!\n Не забудь зберегти префаб!",
            MessageType.Warning);
        
        UpdateAllStates();

        // if (GUILayout.Button("Add Events"))
        // {
        //     soundEvents.UpdateAudioController();
        //     foreach (var animation in soundEvents.animationInfos.Keys)
        //     {
        //         soundEvents.animationInfos[animation].AddEvents();
        //     }
        // }

        EditorGUILayout.HelpBox(
            "Змінить налаштування всіх переходів на стандартні. Всі умови (трігери) переходів - збережуться!",
            MessageType.Warning);

        if (GUILayout.Button("Change preset for all transitions"))
        {
            ChangeAllPresets();
            isShowHelpBox = true;
        }
        
        if (isShowHelpBox)
        {
            EditorGUILayout.HelpBox("Все буде Україна!", MessageType.Info);
        }
        
        EditorGUILayout.HelpBox("Увага! Після знищення повернути зміни буде Не можливо!", MessageType.Warning);
        
        if (GUILayout.Button("Destroy all audio source"))
        {
            if (EditorUtility.DisplayDialog("Увага! Повернути зміни буде Не можливо!",
                "В об'єкті Audio будуть видалені всі звуки! Раджу добре подумати про наслідки!",
                "Мені всерівно, погнали!", "Ні ні, в жодному разі!"))
            {
                soundEvents.DeleteAudioSource();
            }
        }

        ApplySerialized();
    }

    private void GetAllAnimatorStates()
    {
        var states = animatorController.layers[0].stateMachine.states;
        foreach (var state in states)
        {
            if (state.state.motion == null) continue;

            var animationClips = animatorController.animationClips.ToList();
            var stateClip = animationClips.Find(clip => clip.name.Equals(state.state.motion.name));
            soundEvents.SetAnimation(state.state, stateClip);

            SerializedObject serialized = new SerializedObject(soundEvents.animationInfos[state.state]);
            soundEvents.animationInfos[state.state].serialized = serialized;

            soundEvents.animationInfos[state.state].reorderableAudios =
                new ReorderableList(serialized, serialized.FindProperty("audios"), true, false, true, true);
            soundEvents.animationInfos[state.state].reorderableAudios.elementHeight = 110f;

            soundEvents.animationInfos[state.state].reorderableAudios.drawElementCallback =
                (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    var element = soundEvents.animationInfos[state.state].reorderableAudios.serializedProperty
                        .GetArrayElementAtIndex(index);

                    var audioClip = element.FindPropertyRelative("audioClip");
                    var startTime = element.FindPropertyRelative("startTime");
                    var endTime = element.FindPropertyRelative("endTime");
                    var offsetTime = element.FindPropertyRelative("offsetTime");

                    EditorGUI.PropertyField(
                        new Rect(rect.x, rect.y + 5, rect.width, EditorGUIUtility.singleLineHeight),
                        audioClip, new GUIContent("Audio Clip"));
                    EditorGUI.PropertyField(
                        new Rect(rect.x, rect.y + 40, rect.width, EditorGUIUtility.singleLineHeight),
                        startTime, new GUIContent("Start At"));
                    
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUI.PropertyField(
                        new Rect(rect.x, rect.y + 60, rect.width, EditorGUIUtility.singleLineHeight),
                        endTime, new GUIContent("Audio length"));
                    EditorGUI.PropertyField(
                        new Rect(rect.x, rect.y + 80, rect.width, EditorGUIUtility.singleLineHeight),
                        offsetTime, new GUIContent("Offset Time"));
                    EditorGUI.EndDisabledGroup();
                    
                    EditorGUI.LabelField(new Rect(rect.x, rect.y + 95, rect.width, EditorGUIUtility.singleLineHeight),
                        "", GUI.skin.horizontalSlider);
                };
        }
    }

    private void ShowAudioSlider(Audio audio, AnimatorState state)
    {
        var clip = soundEvents.animationInfos[state].animationClip;
        EditorGUILayout.BeginHorizontal("Box");
        GUILayout.Label(audio.startTime.ToString(), GUILayout.Width(60));
        EditorGUILayout.MinMaxSlider(ref audio.startTime, ref audio.endTime, 0f, clip.length);
        GUILayout.Label(audio.endTime.ToString(), GUILayout.Width(60));
        EditorGUILayout.EndHorizontal();
    }

    private void UpdateAllStates()
    {
        var animationInfos = soundEvents.animationInfos;
        foreach (var animation in animationInfos.Keys)
        {
            soundEvents.animationInfos[animation].showFoldout =
                EditorGUILayout.Foldout(soundEvents.animationInfos[animation].showFoldout, animation.name);
            if (soundEvents.animationInfos[animation].showFoldout)
            {
                EditorGUILayout.HelpBox("Clip name: " + animationInfos[animation].animationClip.name, MessageType.Info);
                
                 animationInfos[animation].reorderableAudios.DoLayoutList();
                
                soundEvents.animationInfos[animation].CheckForChanges();

                // if (GUILayout.Button("Play animation"))
                // {
                //     soundEvents.GetAnimator().Rebind();
                //     soundEvents.GetAnimator().Play(animation.name);
                //     isUpdateAnimator = true;
                // }

                var clip = soundEvents.animationInfos[animation].animationClip;
                EditorGUILayout.BeginHorizontal("Box");
                GUILayout.Label("0", GUILayout.Width(60));
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                GUILayout.Label(clip.length.ToString(), GUILayout.Width(60));
                EditorGUILayout.EndHorizontal();

                foreach (var audio in animationInfos[animation].audios)
                {
                    ShowAudioSlider(audio, animation);
                }

                foreach (var audio in animationInfos[animation].audios)
                {
                    if (audio.audioClip != null)
                    {
                        if (GUILayout.Button("Play Audio: " + audio.audioClip.name))
                        {
                            EditorSFX.StopAllClips();
                            EditorSFX.PlayClip(audio.audioClip);
                        }
                    }
                }

                if (GUILayout.Button("Stop all audio"))
                {
                    EditorSFX.StopAllClips();
                    //isUpdateAnimator = false;
                }
            }
        }
    }

    private void ChangeAllPresets()
    {
        var states = animatorController.layers[0].stateMachine.states;
        foreach (var state in states)
        {
            foreach (var transition in state.state.transitions)
            {
                transition.hasExitTime = false;
                transition.exitTime = 1f;
                transition.duration = 0f;
                transition.offset = 0f;
            }
        }
    }

    private void UpdateSerialized()
    {
        foreach (var animation in soundEvents.animationInfos.Values)
        {
            animation.serialized.Update();
        }
    }
    
    private void ApplySerialized()
    {
        foreach (var animation in soundEvents.animationInfos.Values)
        {
            animation.serialized.ApplyModifiedProperties();
        }
    }
}
#endif