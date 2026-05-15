using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Features.MaterialSwitch.Components
{
    internal enum ModeMaterialSwitch
    {
        Once,
        Loop,
    }
    public class MaterialSwitch : MonoBehaviour
    {
        [SerializeField] private ModeMaterialSwitch mode = ModeMaterialSwitch.Once;
        [SerializeField] private bool ResetMaterialOnEnable = true;
        [Space(20)]
        [SerializeField] private List<Renderer> targetRenderers;
        [SerializeField] private Material MaterialSwitchTo;
    
        [Space(10)] [Header("Feedback Settings")]
        [SerializeField] private bool UseFeedback = false;
        [SerializeField] private AudioClip SFXAudio;
        [SerializeField] private ParticleSystem VFXParticles;
        [SerializeField] private float animationDuration = 0.4f;
        [SerializeField] private AnimationCurve scaleCurve = new AnimationCurve(new Keyframe(0, 1),
            new Keyframe(0.16f, 0.75f),
            new Keyframe(0.4f, 1.5f),
            new Keyframe(0.7f, 0.85f),
            new Keyframe(0.85f, 1.1f),
            new Keyframe(1, 1));
    
        private Dictionary<Renderer, Material[]> originalMaterials = new Dictionary<Renderer, Material[]>();
        private AudioSource audioSource;
        private Vector3 initialScale;
        private float animationTime;
        private bool MaterialChanged = false;
        private bool isAnimating = false;
    
        private void Start()
        {
            SaveOriginalMaterials();
            
            if (UseFeedback)
            {
                audioSource = GetComponent<AudioSource>();
                if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
                if (SFXAudio != null) audioSource.clip = SFXAudio;
                audioSource.playOnAwake = false;
            }
        }
        private void OnEnable()
        {
            if (ResetMaterialOnEnable && MaterialChanged)
            {
                ResetMaterials();
            }
        }
        private void OnMouseDown()
        { 
            // if(mode == ModeMaterialSwitch.Off) return; 
            if(MaterialSwitchTo != null && !isAnimating) ChangeMaterial();
        }
        public void ChangeMaterial()
        {
            if (!MaterialChanged)
            {
                SetNewMaterials();
            }
            else if (MaterialChanged && mode == ModeMaterialSwitch.Loop)
            {
                ResetMaterials();
            }
            else
            {
                return;
            }
            if (!isAnimating && UseFeedback) FeedbackOnChange();
        }    
        private void SaveOriginalMaterials()
        {
            if (targetRenderers == null || targetRenderers.Count == 0) return;

            foreach (var renderer in targetRenderers)
            {
                if (renderer != null && !originalMaterials.ContainsKey(renderer))
                {
                    originalMaterials[renderer] = renderer.materials;
                }
            }
        }
        private void SetNewMaterials()
        {
            foreach (var renderer in targetRenderers)
            {
                if (renderer != null)
                {
                    Material[] newMaterials = new Material[renderer.materials.Length];
                    for (int i = 0; i < newMaterials.Length; i++)
                    {
                        newMaterials[i] = MaterialSwitchTo;
                    }

                    renderer.materials = newMaterials;
                }
            }
            MaterialChanged = true;
        }
        private void ResetMaterials()
        {
            foreach (var renderer in targetRenderers)
            {
                if (renderer != null && originalMaterials.ContainsKey(renderer))
                {
                    renderer.materials = originalMaterials[renderer];
                }
                else
                {
                    return;
                }
            }

            MaterialChanged = false;
        }
        private void FeedbackOnChange()
        {
            initialScale = transform.localScale;
            animationTime = 0f;
            isAnimating = true;
            StartCoroutine(Animate());
            if (audioSource != null && audioSource.clip != null) audioSource.Play();
            if (VFXParticles != null) VFXParticles.Play();
        }
        private IEnumerator Animate()
        {
            animationTime = 0f;

            while (animationTime < animationDuration)
            {
                animationTime += Time.deltaTime;
                float progress = Mathf.Clamp01(animationTime / animationDuration);

                float curveValue = scaleCurve.Evaluate(progress);
            
                transform.localScale = initialScale * curveValue;
                if (progress >= 1f)
                {
                    isAnimating = false;
                    transform.localScale = initialScale;
                }
                yield return null;
            }

        }
    
    }
}