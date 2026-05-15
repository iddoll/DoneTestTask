using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using Experiments.Features.Localization.Models;
using Features.Localization.Models;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Rendering;

namespace Features.Localization.Components
{
    public interface IResourcesLocalizationController
    {
        UniTask SetResourcesBundle(AssetBundle bundle);
        void ClearBundle();
    }

    public interface ILocalizationTextState
    {
        bool HasTextLocalization { get; }
    }

    public class ExperimentLocalizationController : MonoBehaviour, IResourcesLocalizationController, ILocalizationTextState
    {
        private const string LocalizationDataKey = "LocalizationData";

        public bool HasTextLocalization =>
            _localizationModel?.Texts != null && _localizationModel.Texts.Count > 0;

        public AudioSource[] audioSources;

        private TextLocalizationComponent[] _localizationTextViews;
        private TextureLocalizationComponent[] _textureLocalizationViews;
        private MaterialLocalizationComponent[] _materialLocalizationViews;
        private AssetBundle _bundle;
        private LocalizationModel _localizationModel;
        private string _experimentName;

#if UNITY_EDITOR
        [SerializeField] private bool loadFromStreamingAssets;
        [SerializeField] private string bundleName;
#endif

        private void Awake()
        {
#if UNITY_EDITOR
            if (loadFromStreamingAssets && !string.IsNullOrEmpty(bundleName))
            {
                string bundlePath = Path.Combine(Application.streamingAssetsPath, bundleName);
                AssetBundle loadedBundle = AssetBundle.LoadFromFile(bundlePath);

                if (loadedBundle != null)
                {
                    SetResourcesBundle(loadedBundle).Forget();
                    Debug.Log($"Loaded AssetBundle '{bundleName}' from StreamingAssets.");
                }
                else
                {
                    Debug.LogError($"Failed to load AssetBundle '{bundleName}' from {bundlePath}");
                }
            }
#endif
        }

        public async UniTask SetResourcesBundle(AssetBundle bundle)
        {
            _bundle = bundle;

            if (_bundle == null)
            {
                Debug.LogError($"Localization bundle for {gameObject.name} was not loaded.");
                return;
            }

            LoadLocalizationModel();

            if (_localizationModel == null)
            {
                Debug.LogError("Parsing error. Invalid text localization file data");
                return;
            }

            await Init();
        }

        public void ClearBundle()
        {
            if (_bundle != null)
            {
                _bundle.Unload(true);
                _bundle = null;
                Resources.UnloadUnusedAssets();
                GC.Collect();
            }
        }

        private async UniTask Init()
        {
            _experimentName = gameObject.name;

            LocalizeText();
            await LocalizeAudio();
            await LocalizeTextures();
            await LocalizeMaterials();
        }

        private void LocalizeText()
        {
            if (_localizationModel.Texts == null)
                return;

            Dictionary<string, string> texts = new Dictionary<string, string>();
            foreach (TextLocalizationModel model in _localizationModel.Texts)
            {
                if (string.IsNullOrEmpty(model.Key)) continue;
                if (!texts.TryAdd(model.Key, model.LocalizedText))
                    Debug.LogWarning($"[Localization] Duplicate text key '{model.Key}', skipping.");
            }

            AssignLocalizationTextViews();
            InitLocalizationTextViews(texts);
        }

        private async UniTask LocalizeAudio()
        {
            if (_localizationModel.VoiceOvers == null)
                return;

            AudioClip[] voiceOvers = new AudioClip[audioSources.Length];

            for (int index = 0; index < _localizationModel.VoiceOvers.Count; index++)
            {
                VoiceLocalizationModel localizationModelVoiceAudio = _localizationModel.VoiceOvers[index];

                if (_bundle.Contains(localizationModelVoiceAudio.Key))
                {
                    AudioClip audioClip = _bundle.LoadAsset<AudioClip>(localizationModelVoiceAudio.Key);

                    if (audioClip == null)
                    {
                        Debug.LogError($"Invalid audio localization file data. Skipping audio. Audio is {audioClip}.");
                        continue;
                    }

                    voiceOvers[index] = audioClip;
                }
                else
                {
                    Debug.LogError($"Cant load audio clip from bundle {localizationModelVoiceAudio.Key}.");
                }
            }

            AssignLocalizedAudio(voiceOvers);
        }

        private async UniTask LocalizeTextures()
        {
            if (_localizationModel.Textures == null)
                return;

            Dictionary<string, TextureMaterialPropertyPair> textures = new Dictionary<string, TextureMaterialPropertyPair>();

            foreach (TextureLocalizationModel localizationModelTexture in _localizationModel.Textures)
            {
                if (_bundle.Contains(localizationModelTexture.Key))
                {
                    Texture2D texture = await _bundle.LoadAssetAsync<Texture2D>(localizationModelTexture.Key) as Texture2D;
                    TextureMaterialPropertyPair textureMaterialPropertyPair = new(texture, localizationModelTexture.MaterialPropertyName);

                    if (texture == null || textureMaterialPropertyPair.MaterialPropertyName == null)
                    {
                        Debug.LogError($"Invalid texture localization file data. Skipping texture. Texture is {texture}," +
                                       $" MaterialPropertyName is {textureMaterialPropertyPair.MaterialPropertyName}.");
                        continue;
                    }

                    if (!textures.TryAdd(localizationModelTexture.Key, textureMaterialPropertyPair))
                        Debug.LogWarning($"[Localization] Duplicate texture key '{localizationModelTexture.Key}', skipping.");
                }
                else
                {
                    Debug.LogError($"Cant load texture from bundle {localizationModelTexture.Key}.");
                }
            }

            AssignLocalizationTextureViews();
            InitLocalizationTextureViews(textures);
        }

        private async UniTask LocalizeMaterials()
        {
            if (_localizationModel.Materials == null || _localizationModel.Materials.Count == 0)
                return;

            // Materials are matched positionally: Materials[0] → _materialLocalizationViews[0].
            List<Material> orderedMaterials = new List<Material>();

            foreach (MaterialLocalizationModel model in _localizationModel.Materials)
            {
                if (_bundle.Contains(model.Key))
                {
                    Material material = await _bundle.LoadAssetAsync<Material>(model.Key) as Material;

                    if (material == null)
                    {
                        Debug.LogError($"[Localization] Failed to load material '{model.Key}' from bundle.");
                    }
                    else
                    {
                        // AssetBundle materials can reference a different shader object than the one
                        // compiled into the project. ResolveShader saves all properties first,
                        // re-links the shader, then restores them so color/floats/textures are preserved.
                        ResolveShader(material, model.Key);
                    }

                    orderedMaterials.Add(material);
                }
                else
                {
                    Debug.LogError($"[Localization] Bundle does not contain material '{model.Key}'.");
                    orderedMaterials.Add(null);
                }
            }

            AssignLocalizationMaterialViews();
            InitLocalizationMaterialViews(orderedMaterials);
        }

        /// <summary>
        /// Re-links a material's shader to the project-compiled version when the bundle
        /// provides a different shader object. Saves every property before re-linking and
        /// restores it afterwards so color, floats, textures, vectors and keywords are preserved.
        /// </summary>
        private static void ResolveShader(Material material, string assetKey)
        {
            if (material == null || material.shader == null) return;

            string shaderName = material.shader.name;
            Shader found = Shader.Find(shaderName);

            if (found == null)
            {
                Debug.LogWarning($"[Localization] Shader '{shaderName}' not found for material '{assetKey}'. " +
                                 "Add it to Graphics Settings → Always Included Shaders.");
                return;
            }

            // If Unity already resolved to the same object, nothing to do.
            if (found == material.shader) return;

            // ── save ──────────────────────────────────────────────────────────
            string[] keywords    = material.shaderKeywords;
            int      renderQueue = material.renderQueue;

            int propCount = material.shader.GetPropertyCount();
            var floats    = new Dictionary<int, float>(propCount);
            var colors    = new Dictionary<int, Color>(propCount);
            var textures  = new Dictionary<int, Texture>(propCount);
            var vectors   = new Dictionary<int, Vector4>(propCount);

            for (int i = 0; i < propCount; i++)
            {
                int id = material.shader.GetPropertyNameId(i);
                switch (material.shader.GetPropertyType(i))
                {
                    case ShaderPropertyType.Float:
                    case ShaderPropertyType.Range:
                        floats[id] = material.GetFloat(id);
                        break;
                    case ShaderPropertyType.Color:
                        colors[id] = material.GetColor(id);
                        break;
                    case ShaderPropertyType.Texture:
                        textures[id] = material.GetTexture(id);
                        break;
                    case ShaderPropertyType.Vector:
                        vectors[id] = material.GetVector(id);
                        break;
                }
            }

            // ── re-link ───────────────────────────────────────────────────────
            material.shader = found;

            // ── restore ───────────────────────────────────────────────────────
            foreach (var kv in floats)   material.SetFloat(kv.Key, kv.Value);
            foreach (var kv in colors)   material.SetColor(kv.Key, kv.Value);
            foreach (var kv in vectors)  material.SetVector(kv.Key, kv.Value);
            foreach (var kv in textures) if (kv.Value != null) material.SetTexture(kv.Key, kv.Value);

            material.shaderKeywords = keywords;
            material.renderQueue    = renderQueue;
        }

        private void LoadLocalizationModel()
        {
            string[] assetNames = _bundle.GetAllAssetNames();
            string searchKey = LocalizationDataKey;

            foreach (string assetName in assetNames)
            {
                if (ContainsIgnoreCase(assetName, searchKey))
                {
                    TextAsset textAsset = _bundle.LoadAsset<TextAsset>(assetName);
                    if (textAsset != null)
                    {
                        _localizationModel = JsonConvert.DeserializeObject<LocalizationModel>(textAsset.text);
                    }
                    else
                    {
                        Debug.LogWarning($"Found matching name but failed to load as TextAsset: {assetName}");
                    }
                    break;
                }
            }

            if (_localizationModel == null)
            {
                Debug.LogError($"No suitable localization asset found with key: {searchKey}");
            }
        }

        private static bool ContainsIgnoreCase(string source, string fragment)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(fragment))
                return false;

            source   = source.Trim();
            fragment = fragment.Trim();

            return source.IndexOf(fragment, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private void InitLocalizationTextViews(Dictionary<string, string> localizedTexts)
        {
            foreach (TextLocalizationComponent view in _localizationTextViews)
            {
                localizedTexts.TryGetValue(view.TextId, out string localizedText);

                if (localizedText != null)
                    view.Init(localizedText);
            }
        }

        private void InitLocalizationTextureViews(Dictionary<string, TextureMaterialPropertyPair> textures)
        {
            foreach (TextureLocalizationComponent view in _textureLocalizationViews)
            {
                textures.TryGetValue(view.TextureId, out TextureMaterialPropertyPair localizedTexture);

                if (localizedTexture != null)
                    view.Init(localizedTexture.Texture2D, localizedTexture.MaterialPropertyName);
            }
        }

        private void AssignLocalizationTextViews() =>
            _localizationTextViews = GetComponentsInChildren<TextLocalizationComponent>(true);

        private void AssignLocalizationTextureViews() =>
            _textureLocalizationViews = GetComponentsInChildren<TextureLocalizationComponent>(true);

        private void AssignLocalizationMaterialViews() =>
            _materialLocalizationViews = GetComponentsInChildren<MaterialLocalizationComponent>(true);

        private void InitLocalizationMaterialViews(List<Material> materials)
        {
            for (int i = 0; i < materials.Count && i < _materialLocalizationViews.Length; i++)
            {
                if (materials[i] != null)
                    _materialLocalizationViews[i].Init(materials[i]);
            }
        }

        private void AssignLocalizedAudio(AudioClip[] audioClips)
        {
            for (int i = 0; i < audioSources.Length; i++)
            {
                if (audioClips[i] == null)
                {
                    Debug.LogError($"Audio clip {i} is null. Skipping translation");
                    continue;
                }

                audioSources[i].clip = audioClips[i];
            }
        }
    }
}
