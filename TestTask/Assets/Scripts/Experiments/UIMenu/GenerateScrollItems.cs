using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace Experiments
{
    public class GenerateScrollItems : MonoBehaviour
    {
        [Header("Items List")]
        [SerializeField] private List<ScrollItemInfo> scrollItems;

        [Header("Animator Settings")]
        [SerializeField] private Animator animator;
        [SerializeField] private string mainAnimationName;

        [Header("Panel Settings")]
        [SerializeField] private MenuPanel menuPanel;
        [SerializeField] private RectTransform parentObject;
        [SerializeField] private ScrollItem scrollItemPrefab;
        [SerializeField] private ScrollViewResizer scrollViewResizer;

        [Header("Optional Settings")]
        [SerializeField] private List<Operation> operations;
        [SerializeField] private bool isOnlyOneClick;
        
        private Action<float> OnClick;

        private void Start()
        {
            if (menuPanel != null)
                OnClick = menuPanel.OnOpenClick;

            if (scrollViewResizer != null)
            {
                scrollViewResizer.gameObject.SetActive(false);
                scrollViewResizer.SetData(scrollItems.Count);
                menuPanel.OpenButtonClick += scrollViewResizer.OnOpenClick;
            }
            
            foreach (var item in scrollItems)
            {
                ScrollItem scrollItem = Instantiate(scrollItemPrefab, parentObject);
                if (animator != null)
                {
                    scrollItem.SetData(item.Icon, item.Color, item.Description, item.TriggerName, mainAnimationName,
                        animator, OnClick, isOnlyOneClick, item.ItemEvent);
                    scrollItem.Operations = operations;
                }
            }
        }

        
    }

    [Serializable]
    public class ScrollItemInfo
    {
        [SerializeField] private Sprite icon;
        [SerializeField] private Color color;
        [SerializeField] private string description;
        [SerializeField] private string triggerName;
        [SerializeField] private UnityEvent itemEvent = new UnityEvent();


        public Sprite Icon => icon;

        public Color Color => color;

        public string Description => description;

        public string TriggerName => triggerName;

        public UnityEvent ItemEvent => itemEvent;
    }
}