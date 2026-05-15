using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityController : MonoBehaviour
{
    private void Start()
    {
        Physics.gravity = new Vector3(0, 0, 0);
    }
}
