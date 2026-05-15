using System;
using UnityEngine;

namespace Features.DragAndDrop.Models
{
    [Serializable]
    public class DragAndDropModel
    {
        public RectTransform draggableObject;
        public RectTransform dropTarget;
    }
}