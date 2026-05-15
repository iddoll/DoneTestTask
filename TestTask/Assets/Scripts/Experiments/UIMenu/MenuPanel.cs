using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Experiments
{
    public class MenuPanel : MonoBehaviour
    {
        [SerializeField] private Button openButton;
        [SerializeField] private GameObject openIcon;
        [SerializeField] private GameObject closeIcon;
        [SerializeField] private GameObject scrollView;
        public List<AnimatorEventObject> animatorObjects;

        private bool _isOpened;
        private Action _openButtonClick;

        public Action OpenButtonClick
        {
            get => _openButtonClick;
            set => _openButtonClick = value;
        }

        private void Start()
        {
            openButton.onClick.AddListener(delegate { OnOpenClick(0); });
        }

        private void OnEnable()
        {
            for (int i = 0; i < animatorObjects.Count; i++)
            {
                animatorObjects[i].OnStartKeyEvent.AddListener(OnStartKeyEvent);
            }
        }

        private void OnDisable()
        {
            for (int i = 0; i < animatorObjects.Count; i++)
            {
                animatorObjects[i].OnStartKeyEvent.RemoveListener(OnStartKeyEvent);
            }
        }

        private void OnStartKeyEvent(string key)
        {
            ShowMenu();
        }

        public void OnOpenClick(float animationLength)
        {
            _isOpened = !_isOpened;
            openIcon.SetActive(!_isOpened);
            closeIcon.SetActive(_isOpened);
            scrollView.SetActive(_isOpened);
            if (animationLength != 0f)
            {
                HideMenu();
            }

            if (_isOpened)
                _openButtonClick?.Invoke();
        }

        private void ChangeButtonColor(Button button, float alpha)
        {
            var imageColor = button.image.color;
            imageColor.a = alpha;
            button.image.color = imageColor;
        }

        private void HideMenu()
        {
            ChangeButtonColor(openButton, 0.5f);
            openButton.enabled = false;
        }

        private void ShowMenu()
        {
            ChangeButtonColor(openButton, 1f);
            openButton.enabled = true;
            OnOpenClick(0f);
        }
    }
}