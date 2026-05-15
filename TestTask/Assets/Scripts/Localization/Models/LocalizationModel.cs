using System.Collections.Generic;
using Features.Localization.Components;
using JetBrains.Annotations;

namespace Features.Localization.Models
{
    public class LocalizationModel
    {
        [UsedImplicitly]
        public string ExperimentName;

        [UsedImplicitly]
        public List<TextLocalizationModel> Texts = new ();

        [UsedImplicitly]
        public List<TextureLocalizationModel> Textures = new ();

        [UsedImplicitly]
        public List<VoiceLocalizationModel> VoiceOvers = new ();

        [UsedImplicitly]
        public List<MaterialLocalizationModel> Materials = new ();
    }
}