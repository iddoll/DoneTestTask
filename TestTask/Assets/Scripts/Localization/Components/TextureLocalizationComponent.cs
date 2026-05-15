using UnityEngine;

namespace Features.Localization.Components
{
    [RequireComponent(typeof(Renderer))]
    public class TextureLocalizationComponent : MonoBehaviour
    {
        //[Required]
        [SerializeField] private string _textureId;

        private Renderer _renderer;
        
        public string TextureId => _textureId;

        public void Init(Texture2D texture, string materialPropertyName)
        {
            _renderer ??= GetComponent<Renderer>();
            if (_renderer == null)
            {
                Debug.LogError($"No Renderer component found. Object name: {gameObject.name}");

                return;
            }

            Material material = _renderer.material;
            material.SetTexture(materialPropertyName, texture);
            _renderer.material = material;
        }
    }
}