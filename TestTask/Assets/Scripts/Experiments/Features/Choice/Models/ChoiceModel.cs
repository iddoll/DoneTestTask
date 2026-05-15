using System.Collections.Generic;
using Features.Choice.Components;
using UnityEngine;

namespace Features.Choice.Models
{
    [System.Serializable]
    public class ChoiceModel
    {
        public ChoiceComponent ObjectToChoose => objectToChoose;
        public List<GameObject> ObjectsToEnable => objectsToEnable;
        public List<GameObject> ObjectsToDisable => objectsToDisable;
        
        public bool ChoiceCompleted => _choiceCompleted;
        
        [SerializeField] private ChoiceComponent objectToChoose;
        [SerializeField] private List<GameObject> objectsToEnable;
        [SerializeField] private List<GameObject> objectsToDisable; 
        
        private bool _choiceCompleted;

        public void SetChoiceCompleted() => _choiceCompleted = true;

        public void ResetModel() => _choiceCompleted = false;
    }
}