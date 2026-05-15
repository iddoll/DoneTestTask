using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Counter_Script_Int : MonoBehaviour
{
    public TextMeshPro _Text;
    public string _unit;
    public int _rate;
    private Coroutine coroutine;

    void Start()
    {
        _Text.text = _rate + _unit;
    }
    public void StartCounterInt()
    {
        coroutine = StartCoroutine(CounterRoutine());
    }
    public void StopCounterInt()
    {
        StopCoroutine(coroutine);
    }
    IEnumerator CounterRoutine()
    {
        while (true)
        {
            _Text.text = _rate + _unit;
            yield return new WaitForSeconds(0.1f);
        }

    }

}