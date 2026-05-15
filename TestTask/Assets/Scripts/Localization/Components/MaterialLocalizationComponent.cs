using UnityEngine;

namespace Features.Localization.Components
{
    [RequireComponent(typeof(Renderer))]
    public class MaterialLocalizationComponent : MonoBehaviour
    {
        [SerializeField] private int _materialIndex = 0;

        private Renderer _renderer;

        public int MaterialIndex => _materialIndex;

        public void Init(Material material)
        {
            _renderer ??= GetComponent<Renderer>();
            if (_renderer == null)
            {
                Debug.LogError($"No Renderer component found. Object name: {gameObject.name}");
                return;
            }

            Material[] materials = _renderer.materials;
            materials[_materialIndex] = material;
            _renderer.materials = materials;
        }
    }
}
