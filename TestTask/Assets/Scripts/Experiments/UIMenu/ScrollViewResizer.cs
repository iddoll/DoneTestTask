using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Experiments
{
    public class ScrollViewResizer : MonoBehaviour
    {
        [Header("Scroll view settings")]
        [SerializeField] private RectOptions rectOptions;

        private int _listLength;

        public void SetData(int length)
        {
            _listLength = length;
        }

        private void OnDisable()
        {
            var rectTransformSizeDelta = rectOptions.RectTransform.sizeDelta;
            rectTransformSizeDelta.y = 0f;
            rectOptions.RectTransform.sizeDelta = rectTransformSizeDelta;
            StopAllCoroutines();
        }

        public void OnOpenClick()
        {
            ResizeScrollView(_listLength);
        }

        private void ResizeScrollView(int itemsCount)
        {
            float height = itemsCount * rectOptions.ItemHeight;
            if (height > rectOptions.RectMaxHeight)
            {
                height = rectOptions.RectMaxHeight;
            }

            StartCoroutine(ResizeRectAsync(rectOptions.RectTransform, height));
        }

        private IEnumerator ResizeRectAsync(RectTransform rect, float maxHeight)
        {
            while (rect.sizeDelta.y < maxHeight)
            {
                yield return new WaitForSeconds(1f / rectOptions.FPS);
                var rectSizeDelta = rect.sizeDelta;
                rectSizeDelta.y += rectOptions.StepPerFrame;
                rect.sizeDelta = rectSizeDelta;
            }
        }
    }

    [Serializable]
    public class RectOptions
    {
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private float rectMaxHeight = 500f;
        [SerializeField] private float itemHeight = 55f;
        [SerializeField] private float fps = 60f;
        [SerializeField] private float stepPerFrame = 10f;

        public RectTransform RectTransform => rectTransform;

        public float RectMaxHeight => rectMaxHeight;

        public float ItemHeight => itemHeight;

        public float FPS => fps;

        public float StepPerFrame => stepPerFrame;
    }
}