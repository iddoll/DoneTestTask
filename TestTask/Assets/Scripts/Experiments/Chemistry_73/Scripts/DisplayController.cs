using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayController : MonoBehaviour
{
    private float duration = 1f;
    private Coroutine curCoroutine = null;

    public void ShowObject(float size = 1, Action endEvent = null)
    {
        StopCurrentCoroutine();
        curCoroutine = StartCoroutine(ChangeScale(Vector3.one * size,endEvent));
    }

    public void HideObject(Action endEvent = null)
    {
        StopCurrentCoroutine();
        curCoroutine = StartCoroutine(ChangeScale(Vector3.zero,endEvent));
    }

    private void StopCurrentCoroutine()
    {
        if (curCoroutine != null)
        {
            StopCoroutine(curCoroutine);
            curCoroutine = null;
        }
    }
    
    private IEnumerator ChangeScale(Vector3 targetScale, Action endEvent)
    {
        float elapsedTime = 0f;
        Vector3 startingScale = transform.localScale;

        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(startingScale, targetScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
        endEvent?.Invoke();
    }
}
