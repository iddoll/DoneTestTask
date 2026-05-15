using System;
using System.Collections.Generic;
using System.Linq;
using Features.Choice.Components;
using Features.Choice.Models;
using UnityEngine;
using UnityEngine.Events;

namespace Features.Choice.Controllers
{
    public abstract class ChoiceController : MonoBehaviour
    {
        [SerializeField] protected List<ChoiceModel> objectsToChoose;
        [SerializeField] private UnityEvent onAllObjectsChosen;

        private ChoiceComponent[] _choiceObjects;

        protected abstract void OnObjectClick(ChoiceComponent obj);
        
        protected void CompleteChoice(ChoiceModel chosenObjectModel)
        {
            chosenObjectModel.SetChoiceCompleted();
            SetActiveState(chosenObjectModel.ObjectsToEnable, true);
            SetActiveState(chosenObjectModel.ObjectsToDisable, false);

            if (objectsToChoose.All(x => x.ChoiceCompleted)) 
                onAllObjectsChosen?.Invoke();
        }

        protected void ProcessWrongChoice(ChoiceComponent obj)
        {
            if (obj is not WrongChoiceComponent wrongChoiceComponent) 
                return;
            
            wrongChoiceComponent.ShowWrongChoice();
        }

        protected void SetActiveState(List<GameObject> objects, bool state)
        {
            for (int i = 0; i < objects.Count; i++) 
                objects[i].SetActive(state);
        }

        private void Awake()
        {
            AssignChoiceObjects();
            SubscribeEvents();
        }

        private void OnDisable()
        {
            UnsubscribeEvents();
            ResetObjectsToChoose();
        }

        private void OnEnable() => SubscribeEvents();

        private void OnDestroy() => UnsubscribeEvents();

        private void AssignChoiceObjects() => _choiceObjects = GetComponentsInChildren<ChoiceComponent>(true);

        private void SubscribeEvents()
        {
            for (int i = 0; i < _choiceObjects.Length; i++) 
                _choiceObjects[i].OnObjectClick += OnObjectClick;
        }
        
        private void UnsubscribeEvents()
        {
            for (int i = 0; i < _choiceObjects.Length; i++) 
                _choiceObjects[i].OnObjectClick -= OnObjectClick;
        }

        private void ResetObjectsToChoose()
        {
            for (int i = 0; i < objectsToChoose.Count; i++)
                objectsToChoose[i].ResetModel();
        }
    }
}