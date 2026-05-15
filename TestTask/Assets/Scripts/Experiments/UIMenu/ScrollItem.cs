using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;
namespace Experiments
{
    public class ScrollItem : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI description;

        private Animator _animator;
        private string _triggerName;
        private string _choiceAnimationName;
        private Action<float> _action;
        private List<Operation> _operations;
        private bool _isOnlyOneClick, _isClick;
        private UnityEvent _onClickEvent = new UnityEvent();
        public List<Operation> Operations
        {
            get => _operations;
            set => _operations = value;
        }

        public void SetData(Sprite texture2D, Color color, string newDescription, string newTriggerName,
            string choiceAnimationName,
            Animator animator, Action<float> action, bool isOneClick = false, UnityEvent onClickEvent = null)
        {
            icon.sprite = texture2D;
            icon.color = color;
            description.text = newDescription;
            _triggerName = newTriggerName;
            _choiceAnimationName = choiceAnimationName;
            _animator = animator;
            _action = action;
            _isOnlyOneClick = isOneClick;
            _onClickEvent = onClickEvent;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(_isOnlyOneClick && _isClick) return;

            if (_animator != null && _animator.GetCurrentAnimatorStateInfo(0).IsName(_choiceAnimationName))
            {
                _animator.SetTrigger(_triggerName);
                ChangeOpacity(0.5f);
                StartCoroutine(WaitForAnimationAsync());
                CallOperationsAction();
                _isClick = true;

                _onClickEvent.Invoke();
            }
        }

        private void CallOperationsAction()
        {
            foreach (var operation in _operations)
            {
                operation.DoAction();
            }
        }

        private void ChangeOpacity(float opacity)
        {
            var color = description.color;
            color.a = opacity;
            description.color = color;
            color = icon.color;
            color.a = opacity;
            icon.color = color;
        }

        private IEnumerator WaitForAnimationAsync()
        {
            yield return new WaitForSeconds(0.1f);
            _action?.Invoke(_animator.GetCurrentAnimatorStateInfo(0).length);
        }
    }
}