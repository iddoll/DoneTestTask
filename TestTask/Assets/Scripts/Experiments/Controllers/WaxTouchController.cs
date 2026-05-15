using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaxTouchController : MonoBehaviour
{
    [SerializeField] private GameObject nail;
    void OnMouseDown()
    {
        NailController.instance.addNail();
        nail.transform.localScale = new Vector3(30f,30f,30f);
        gameObject.transform.localScale = new Vector3(0f,0f,0f);

    }
    
}
