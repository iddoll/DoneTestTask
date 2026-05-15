using System;
using UnityEngine;

namespace Features.Experiment.Components
{
    public class ContinueButton : MonoBehaviour
    {
        public event Action OnContinueButtonClick;
        private bool _enable;

        private void OnMouseDown()
        {
            if (!_enable)
                return;

            OnContinueButtonClick?.Invoke();
            DisableButton();
        }

        public void EnableButton()
        {
            _enable = true;
        }
        
        private void DisableButton()
        {
            _enable = false;
        }
    }
}