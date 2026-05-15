using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(LineRenderer))]
public class Laser : MonoBehaviour
{
    [SerializeField] private float laserPartMaxDistance = 10f;
    [SerializeField] private int reflectionsMaxCount;
    [SerializeField] private LayerMask layerMask;

    private LineRenderer lineRenderer;
    private int defaultPointCount = 2;
    private int reflectionsCounter;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        UpdateLaser();
    }

    private void UpdateLaser()
    {
        reflectionsCounter = 0;

        lineRenderer.SetPosition(0, gameObject.transform.position);

        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit, laserPartMaxDistance, layerMask, QueryTriggerInteraction.Ignore))
        {
            lineRenderer.SetPosition(1, hit.point);

            if (reflectionsMaxCount != 0)
            {
                Vector3 dir = lineRenderer.GetPosition(1) - lineRenderer.GetPosition(0);
                Vector3 reflected = lineRenderer.GetPosition(1) + Vector3.Reflect(dir, hit.normal).normalized * laserPartMaxDistance;
                reflectionsCounter++;
                SearchReflectionPoint(reflected);
            }

            else
            {
                lineRenderer.positionCount = 2;
            }
        }

        else
        {
            lineRenderer.SetPosition(1, transform.position + transform.forward * laserPartMaxDistance);
            lineRenderer.positionCount = 2;
        }
    }

    private void SearchReflectionPoint(Vector3 _reflected)
    {
        RaycastHit hit;
        if (Physics.Raycast(lineRenderer.GetPosition(reflectionsCounter), _reflected - lineRenderer.GetPosition(reflectionsCounter), out hit, laserPartMaxDistance, layerMask, QueryTriggerInteraction.Ignore))
        {
            lineRenderer.positionCount = reflectionsCounter + defaultPointCount;
            lineRenderer.SetPosition(reflectionsCounter + defaultPointCount - 1, hit.point);

            if (reflectionsCounter < reflectionsMaxCount)
            {
                Vector3 dir = lineRenderer.GetPosition(reflectionsCounter + defaultPointCount - 1) - lineRenderer.GetPosition(reflectionsCounter);
                Vector3 reflected = lineRenderer.GetPosition(1) + Vector3.Reflect(dir, hit.normal).normalized * laserPartMaxDistance;
                reflectionsCounter++;
                SearchReflectionPoint(reflected);
            }
        }

        else
        {
            lineRenderer.positionCount = reflectionsCounter + defaultPointCount;
            lineRenderer.SetPosition(reflectionsCounter + defaultPointCount - 1, _reflected);
        }
    }
}
