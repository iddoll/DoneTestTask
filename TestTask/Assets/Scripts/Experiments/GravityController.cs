using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityController : MonoBehaviour
{
    private void Update()
    {
        Physics.gravity = new Vector3(0,0,transform.rotation.y);
    }
}
