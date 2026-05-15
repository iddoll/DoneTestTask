using System.Collections.Generic;
using System.Linq;
using Features.DragAndDrop.Models;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Features.DragAndDrop.Controllers
{
    public class DragAndDropController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] 
        private List<DragAndDropModel> dragAndDropModels = new();

        [SerializeField] 
        private Canvas canvas;

        private DragAndDropModel _currentDragAndDropModel;
        private Vector3 _currentInitialPosition;

        private bool _isDragged;

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_isDragged)
                return;

            _currentDragAndDropModel = GetDraggableObjectUnderPointer(eventData);

            if (_currentDragAndDropModel == null) 
                return;

            _isDragged = true;
            _currentInitialPosition = _currentDragAndDropModel.draggableObject.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_currentDragAndDropModel == null || _currentDragAndDropModel.draggableObject == null)
                return;

            _currentDragAndDropModel.draggableObject.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_currentDragAndDropModel == null)
                return;

            if (_currentDragAndDropModel.dropTarget != null &&
                RectTransformUtility.RectangleContainsScreenPoint(_currentDragAndDropModel.dropTarget,
                    Input.mousePosition, null))
            {
                Debug.Log($"Dropped {_currentDragAndDropModel.draggableObject.name} on the target!");
                _currentDragAndDropModel.draggableObject.position = _currentDragAndDropModel.dropTarget.position;
            }
            else
            {
                Debug.Log($"Dropped {_currentDragAndDropModel.draggableObject.name} outside the target.");
                ResetPosition(_currentDragAndDropModel.draggableObject);
            }

            _isDragged = false;
            _currentDragAndDropModel = null;
        }

        private void ResetPosition(RectTransform draggableObject) => draggableObject.position = _currentInitialPosition;

        private DragAndDropModel GetDraggableObjectUnderPointer(PointerEventData eventData) =>
            dragAndDropModels.FirstOrDefault(x =>
                RectTransformUtility.RectangleContainsScreenPoint(x.draggableObject, eventData.position, null));
    }
}