using System.Collections.Generic;
using UnityEngine;

public class AnimatorEventTrigger : MonoBehaviour
{
    public List<AnimatorEventObject> animatorObjects;

    public void ExecuteEventFunction()
    {
        for (int i = 0; i < animatorObjects.Count; i++)
        {
            animatorObjects[i].Execute();
        }
    }

    public void ExecuteEventFunctionByKey(string key)
    {
        for (int i = 0; i < animatorObjects.Count; i++)
        {
            if (animatorObjects[i].key == key)
            {
                animatorObjects[i].Execute();
            }
        }
    }
}