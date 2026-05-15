using System;
using JetBrains.Annotations;
using UnityEngine.Serialization;

namespace Features.Localization.Components
{
    [Serializable]
    public class TextureLocalizationModel
    {
        [UsedImplicitly] public string Key;
        [UsedImplicitly] public string MaterialPropertyName;
    }
}