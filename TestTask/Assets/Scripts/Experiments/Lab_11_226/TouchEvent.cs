using UnityEngine;
using UnityEngine.Events;

namespace Lab_11_226
{
    public class TouchEvent : MonoBehaviour
    {
        public UnityEvent OnTouchEvent = new UnityEvent();

        void OnMouseDown()
        {
            OnTouchEvent?.Invoke();
        }
    }
}