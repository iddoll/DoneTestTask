using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperimentUIController : MonoBehaviour
{
    [SerializeField] private List<RectTransform> uiElements;
    private bool progress;

    private void Start()
    {
        RotateScreen.instance.OnChangeLeft.AddListener(OnChangeLandscape);
        RotateScreen.instance.OnChangeDown.AddListener(OnChangePortrait);
    }

    private void OnChangeLandscape()
    {
        foreach (var element in uiElements)
        {
            StartCoroutine(RotateTo(-90f, element));
        }
    }

    private void OnChangePortrait()
    {
        foreach (var element in uiElements)
        {
            StartCoroutine(RotateTo(0f, element));
        }
    }
    
    IEnumerator RotateTo(float angle, RectTransform elementRectTransform)
    {
        yield return new WaitUntil(() => progress == false);
        progress = true;
        float time = 0;
        while (time<=0.75)
        {
            float z = Mathf.LerpAngle(elementRectTransform.eulerAngles.z, angle, time);
            elementRectTransform.eulerAngles = new Vector3(0, 0, z);
            yield return new WaitForEndOfFrame();
            time += Time.deltaTime;
            
        }
        progress = false;

    }
}
