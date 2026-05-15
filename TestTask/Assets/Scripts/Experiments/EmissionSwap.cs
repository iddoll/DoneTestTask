using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmissionSwap : MonoBehaviour
{
    [Tooltip("Массив текстур, которые будут подменяться")]
    public Texture[] emissionArray;

    [Tooltip("Измените номер во время записи анимации. " +
    "В материале будет изменена карта Emission" +
    "на ту, которая подходит по номеру")]
    public int emissionNumber = -1;

    private int _emissionSelectNumber = -1;
    private MeshRenderer _meshRenderer;

    private void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();        
    }

    private void FixedUpdate()
    {
        if (emissionNumber != _emissionSelectNumber)
            if (emissionNumber < emissionArray.Length && emissionNumber >= 0)               
                    Replacement(emissionNumber);                
    }

    public void Replacement(int number)
    {
        _emissionSelectNumber = number;
        _meshRenderer.material.SetTexture("_EmissionMap", emissionArray[number]);
    }  
}
