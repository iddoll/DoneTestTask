using UnityEngine;
using System.Collections;

public class SinCylinders : MonoBehaviour
{
    [SerializeField] private GameObject[] cylinders;
    [SerializeField] private float[] cylindersOffset;
    [SerializeField] private float speed;
    [SerializeField] private float amplitude;
    [SerializeField] private float koefDelta;
    [SerializeField] private float returningBackSpeed;

    private float timer;
    private float k;
    private bool experimentInAction;

    public void StartCylindersExperiment()
    {
        StopAllCoroutines();
        experimentInAction = true;
        StartCoroutine(MoveSpheresSin());
    }

    public void StopCylindersExperiment()
    {
        experimentInAction = false;
    }

    private IEnumerator MoveSpheresSin()
    {
        while (experimentInAction)
        {
            for (int i = 0; i < cylinders.Length; i++)
            {

                var euler = cylinders[i].transform.localRotation.eulerAngles;
                euler.y = Mathf.Sin(Time.time * Mathf.PI * speed + cylindersOffset[i]) * amplitude;
                Quaternion lerped = Quaternion.Lerp(cylinders[i].transform.localRotation, Quaternion.Euler(euler), k);
                cylinders[i].transform.localRotation = lerped;
            }

            k = Mathf.MoveTowards(k, 1f, koefDelta);
            yield return null;
        }

        while (!experimentInAction)
        {
            for (int i = 0; i < cylinders.Length; i++)
            {
                var euler = cylinders[i].transform.localRotation.eulerAngles;
                euler.y = Mathf.Sin(Time.time * Mathf.PI * speed + cylindersOffset[i]) * amplitude;
                Quaternion lerped = Quaternion.Lerp(cylinders[i].transform.localRotation, Quaternion.Euler(Vector3.zero), k);
                cylinders[i].transform.localRotation = lerped;
            }

            yield return null;
        }

        yield break;

    }
}

