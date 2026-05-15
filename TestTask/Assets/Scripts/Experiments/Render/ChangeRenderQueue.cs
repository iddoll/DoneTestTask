using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ChangeRenderQueue : MonoBehaviour
{
    [SerializeField] private Material material;
    [SerializeField] private int renderQueue;

    private void Start()
    {
        material.renderQueue = renderQueue;
    }
}
