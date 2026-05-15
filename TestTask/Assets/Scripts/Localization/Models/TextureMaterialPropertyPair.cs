using UnityEngine;

namespace Experiments.Features.Localization.Models
{
    public class TextureMaterialPropertyPair
    {
        public Texture2D Texture2D;
        public string MaterialPropertyName;
        
        public TextureMaterialPropertyPair(Texture2D texture2D, string materialPropertyName) => 
            (Texture2D, MaterialPropertyName) = (texture2D, materialPropertyName);
    }
}