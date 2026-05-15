using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatorShell : MonoBehaviour
{
    public float rotationSpeed = 90f;
    public float pauseDuration = 5f;
    private float currentRotation = 0f;
    private bool isPaused = true; 
    private float pauseTimer = 0f;

    private Vector3 vectorRotation = Vector3.forward;

    public void SetVector(Vector3 vec)
    {
        vectorRotation = vec;
    }

    private void Update()
    {
        if (!isPaused)
        {
            transform.Rotate(vectorRotation, rotationSpeed * Time.deltaTime);
            currentRotation += rotationSpeed * Time.deltaTime;
        
            if (currentRotation >= 360f)
            {
                currentRotation = 0f;
                isPaused = true;
            }
        }
        else
        {
            pauseTimer += Time.deltaTime;
            if (pauseTimer >= pauseDuration)
            {
                isPaused = false;
                pauseTimer = 0f;
            }
        }
    }
}
