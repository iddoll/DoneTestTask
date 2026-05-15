using System;
using UnityEngine;

namespace Features.Choice.Components
{
    public class ChoiceComponent : MonoBehaviour
    {
        public event Action<ChoiceComponent> OnObjectClick;

        private void OnMouseDown() => OnObjectClick?.Invoke(this);
    }
}