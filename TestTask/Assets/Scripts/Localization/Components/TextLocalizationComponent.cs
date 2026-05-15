using TMPro;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Features.Localization.Components
{
    [RequireComponent(typeof(TMP_Text))]
    public class TextLocalizationComponent : MonoBehaviour
    {
        [SerializeField] private string _textId;
        [SerializeField] private bool _useCustomId = false;
        
        private TMP_Text _textComponent;
        
        public string TextId => _textId;
        public bool UseCustomId => _useCustomId;

        public void Init(string text)
        {
            _textComponent = GetComponent<TMP_Text>();
            
            if (_textComponent == null)
            {
                Debug.LogError($"{gameObject.name} has no TMP_Text component.");
                return;
            }
            
            _textComponent.text = text;

            string uaText = _textComponent.text;
        }

        #if UNITY_EDITOR
        
        public void UpdateId()
        {
            if (UseCustomId)
            {
                return;
            }
            
            if (!string.IsNullOrWhiteSpace(_textId))
            {
                return;
            }

            _textComponent ??= GetComponent<TMP_Text>();
                
            Undo.RecordObject(this, "Set text id TextLocalizationComponent");
            PrefabUtility.RecordPrefabInstancePropertyModifications(this);
            
            if (_textComponent != null)
                _textId = _textComponent.text;
        }
        
        public string GetUkrainianText()
        {
            _textComponent ??= GetComponent<TMP_Text>();

            return _textComponent != null ? _textComponent.text : "";
        }
#endif
    }
}