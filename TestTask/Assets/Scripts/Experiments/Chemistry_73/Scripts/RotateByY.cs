using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateByY : MonoBehaviour
{
    public Transform centre;
    public float rotationSpeed = 90f;
    public float radius = 2.0f;
    public float radiusSpeed = 0.5f;
    private void Update()
    {
        transform.RotateAround(centre.position, transform.up, -rotationSpeed * Time.deltaTime);
        Vector3 desiredPosition = (transform.position - centre.position).normalized * 2 + centre.position;
        transform.position = Vector3.MoveTowards(transform.position, desiredPosition, Time.deltaTime * radiusSpeed);
    }
}
