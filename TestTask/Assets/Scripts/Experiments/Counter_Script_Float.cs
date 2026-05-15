using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Counter_Script_Float : MonoBehaviour
{
    public TextMeshPro _Text;
    public string _unit;
    public float _rate;
    private Coroutine coroutine;


    void Start()
    {
        _Text.text = _rate.ToString("0.0") + _unit;
    }
    public void StartCounterFloat()
    {
        coroutine = StartCoroutine(CounterRoutine());
    }
    public void StopCounterFloat()
    {
        StopCoroutine(coroutine);
    }
    IEnumerator CounterRoutine()
    {
        while (true)
        {
            _Text.text = _rate.ToString("0.0") + _unit;
            yield return new WaitForSeconds(0.1f);
        }

    }

}
