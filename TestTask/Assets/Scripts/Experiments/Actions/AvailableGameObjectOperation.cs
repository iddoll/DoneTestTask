using System.Collections.Generic;
using UnityEngine;

namespace Actions
{
    public class AvailableGameObjectOperation : Operation
    {
        [SerializeField] private GameObject gameObject;
        [SerializeField] private bool isActive;
        [SerializeField] private List<GameObject> objectsToDisable;
        [SerializeField] private List<GameObject> objectsToEnable;


        public override void DoAction()
        {
            gameObject.SetActive(isActive);
            EnableObjects();
            DisableObjects();
            base.DoAction();
        }

        public override void UnDoAction()
        {
            gameObject.SetActive(!isActive);
        }

        private void EnableObjects()
        {
            if (objectsToEnable != null)
            {
                foreach (var obj in objectsToEnable)
                {
                    obj.SetActive(true);
                }
            }
        }

        private void DisableObjects()
        {
            if (objectsToDisable != null)
            {
                foreach (var obj in objectsToDisable)
                {
                    obj.SetActive(false);
                }
            }
        }
    }
} 