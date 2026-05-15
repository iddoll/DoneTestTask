using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [SerializeField] private Transform objToRotate;
    void Update()
    {
        objToRotate.LookAt(Camera.main.transform.position);
    }
}
