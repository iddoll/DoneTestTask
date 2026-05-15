using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(LineRenderer))]
public class LaserEX16 : MonoBehaviour
{
    [SerializeField] private float laserPartMaxDistance = 10f;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private LineRenderer refractionLR;

    private LineRenderer lineRenderer;
    private float airToGlassModificator = 0.66f;
    private float glassToAirModificator = 1.5f;
    private int defaultPointCount = 2;


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

            var angleToNormal = Vector3.Angle(dir, hit.normal);
            if(lineRenderer.GetPosition(0).z < hit.point.z)
            {
               negativeAngle = true;
            }

            BuildRefractionRay(angleToNormal, hit, lineRenderer.GetPosition(1), negativeAngle);
        }

        else
        {
            lineRenderer.SetPosition(1, transform.position + transform.forward * laserPartMaxDistance);
            lineRenderer.positionCount = 2;
        }
    }

    private void BuildRefractionRay(float angleToNormal, RaycastHit _hit, Vector3 startPoint, bool _negativeAngle)
    {
        Vector3 normal = _hit.normal;

        Vector3 refractionDir = Vector3.up;
        if (_negativeAngle)
        {
            refractionDir = Vector3.down;
        }

        RaycastHit hit;
        refractionLR.SetPosition(0, startPoint);
        refractionLR.positionCount = defaultPointCount;
        if (_hit.collider.tag == "Air")
        {
            Vector3 reversedNormal = Quaternion.AngleAxis((180 - angleToNormal) * airToGlassModificator, refractionDir) * -normal;
            refractionLR.SetPosition(1, refractionLR.GetPosition(0) + reversedNormal * laserPartMaxDistance);
        }

        else if(_hit.collider.tag == "Glass")
        {
            Vector3 reversedNormal = Quaternion.AngleAxis((180 - angleToNormal) * glassToAirModificator, -refractionDir) * -normal;
            refractionLR.SetPosition(1, refractionLR.GetPosition(0) + reversedNormal * laserPartMaxDistance);
        }
    }
}
