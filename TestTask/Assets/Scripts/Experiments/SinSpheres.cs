using UnityEngine;
using System.Collections;

public class SinSpheres : MonoBehaviour
{
    [SerializeField] private GameObject[] spheres;
    [SerializeField] private float[] spheresOffset;
    [SerializeField] private float speed;
    [SerializeField] private float amplitude;
    [SerializeField] private float koefDelta;
    [SerializeField] private float returningBackSpeed;

    private float timer;
    private float k;
    private bool experimentInAction;

    public void StartSpheresExperiment()
    {
        StopAllCoroutines();
        experimentInAction = true;
        StartCoroutine(MoveSpheresSin());
    }

    public void StopSpheresExperiment()
    {
        experimentInAction = false;
    }

    private IEnumerator MoveSpheresSin()
    {
        while (experimentInAction)
        {
            for (int i = 0; i < spheres.Length; i++)
            {
                spheres[i].transform.localPosition = new Vector3(
                    spheres[i].transform.localPosition.x, 
                    spheres[i].transform.localPosition.y, 
                    Mathf.Lerp(spheres[i].transform.localPosition.z, Mathf.Sin(Time.time * Mathf.PI * speed + spheresOffset[i]) * amplitude, k));                
            }

            k = Mathf.MoveTowards(k, 1f, koefDelta);
            yield return null;
        }

        while (!experimentInAction)
        {
            for (int i = 0; i < spheres.Length; i++)
            {
                spheres[i].transform.localPosition = new Vector3(
                    spheres[i].transform.localPosition.x,
                    spheres[i].transform.localPosition.y,
                    Mathf.MoveTowards(spheres[i].transform.localPosition.z, 0f, returningBackSpeed));
            }
            yield return null;
        }

        yield break;
    }
}
