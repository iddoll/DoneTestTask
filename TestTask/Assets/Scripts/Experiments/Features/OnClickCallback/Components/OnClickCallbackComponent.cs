using System;
using UnityEngine;

namespace Features.OnClickCallback.Components
{
    [RequireComponent(typeof(Collider))]
    public class OnClickCallbackComponent : MonoBehaviour
    {
        public event Action OnClick;

        private void OnMouseDown() => OnClick?.Invoke();
    }
}