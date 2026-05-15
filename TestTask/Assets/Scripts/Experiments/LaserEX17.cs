using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(LineRenderer))]
public class LaserEX17 : MonoBehaviour
{
    [SerializeField] private float laserPartMaxDistance = 10f;
    [SerializeField] private float laserReflectionFadeMultipl = 1.5f;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private LineRenderer refractionLR;
    [SerializeField] private LineRenderer reflectionLR;
    //[SerializeField] private bool glassToAir = false;

    private LineRenderer lineRenderer;
    //private float airToGlassModificator = 0.66f;
    private float glassToAirModificator = 1.5f;
    private float defaultLaserAlpha = 0.8f;
    //private int reflectionsMaxCount = 1;
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
        bool negativeAngle = false;
        lineRenderer.SetPosition(0, gameObject.transform.position);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, laserPartMaxDistance, layerMask, QueryTriggerInteraction.Ignore))
        {
            lineRenderer.SetPosition(1, hit.point);

            Vector3 dir = lineRenderer.GetPosition(1) - lineRenderer.GetPosition(0);
            Vector3 reflected = lineRenderer.GetPosition(1) + Vector3.Reflect(dir, hit.normal).normalized * laserPartMaxDistance;
            reflectionsCounter++;

            var angleToNormal = Vector3.Angle(dir, hit.normal);
            if (lineRenderer.GetPosition(0).z < hit.point.z)
            {
                negativeAngle = true;
            }

            BuildRefractionRay(angleToNormal, hit, lineRenderer.GetPosition(1), negativeAngle);
            BuildReflectionRay(angleToNormal, reflected, lineRenderer.GetPosition(1));
        }

        else
        {
            lineRenderer.SetPosition(1, transform.position + transform.forward * laserPartMaxDistance);
            lineRenderer.positionCount = 2;
        }
    }

    private void BuildReflectionRay(float angleToNormal, Vector3 _reflected, Vector3 startPoint)
    {
        RaycastHit hit;
        float laserAlpha = Mathf.Clamp((defaultLaserAlpha * ((165f - angleToNormal) / 100f) * laserReflectionFadeMultipl), 0, defaultLaserAlpha);
        var color = reflectionLR.sharedMaterial.color;
        color.a = laserAlpha;
        reflectionLR.sharedMaterial.color = color;

        reflectionLR.SetPosition(0, startPoint);
        reflectionLR.positionCount = defaultPointCount;

        if (Physics.Raycast(reflectionLR.GetPosition(0), _reflected - reflectionLR.GetPosition(0), out hit, laserPartMaxDistance, layerMask, QueryTriggerInteraction.Ignore))
        {
            reflectionLR.SetPosition(1, hit.point);
        }

        else
        {
            reflectionLR.SetPosition(1, _reflected);
        }
    }

    private void BuildRefractionRay(float angleToNormal, RaycastHit _hit, Vector3 startPoint, bool _negativeAngle)
    {
        float laserAlpha = Mathf.Clamp((defaultLaserAlpha - defaultLaserAlpha * ((165f - angleToNormal) / 100f) * laserReflectionFadeMultipl), 0, defaultLaserAlpha);
        var color = refractionLR.sharedMaterial.color;
        color.a = laserAlpha;
        refractionLR.sharedMaterial.color = color;

        Vector3 normal = _hit.normal;

        Vector3 refractionDir = Vector3.up;
        if (_negativeAngle)
        {
            refractionDir = Vector3.down;
        }

        RaycastHit hit;
        refractionLR.SetPosition(0, startPoint);
        refractionLR.positionCount = defaultPointCount;

        Vector3 reversedNormal = Quaternion.AngleAxis((180 - angleToNormal) * glassToAirModificator, refractionDir) * -normal;
        refractionLR.SetPosition(1, refractionLR.GetPosition(0) + reversedNormal * laserPartMaxDistance);

    }
}
