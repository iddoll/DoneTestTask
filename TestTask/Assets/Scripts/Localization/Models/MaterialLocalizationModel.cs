using JetBrains.Annotations;

namespace Features.Localization.Models
{
    public class MaterialLocalizationModel
    {
        /// <summary>Asset name to load from the bundle (locale-specific material).</summary>
        [UsedImplicitly] public string Key;

        /// <summary>Renderer material slot index (informational; component uses its own _materialIndex).</summary>
        [UsedImplicitly] public int MaterialIndex;
    }
}
