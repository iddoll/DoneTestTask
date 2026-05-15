using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GroupDisplayControlller : MonoBehaviour
{
    public List<GroupItem> groups;
    private Material searchMat = null;
    public float blinkDuration = 2f;
    public float blinkSpeed = 1f;

    private bool isBlinking = false;
    private Coroutine currentCoroutine = null;
    private Material[] previosMats = new Material[]{};

    private void Start()
    {
        for (int i = 0; i < groups.Count; i++)
        {
            ResetMetallic(groups[i].mat);
        }
    }

    public void ShowGroups(string name)
    {
        var groupElement = FindModel(name);
        if (groupElement != null)
        {
            if(currentCoroutine!=null)
                StopCoroutine(currentCoroutine);
            ResetMetallic(previosMats);
            currentCoroutine = StartCoroutine(BlinkMetallic(groupElement.mat));
            previosMats = groupElement.mat;
        }
    }
    private IEnumerator BlinkMetallic(Material[] mats)
    {
        yield return new WaitForSeconds(0.01f);
        float elapsedTime = 0f;

        while (elapsedTime < blinkDuration)
        {
            float newMetallicValue = Mathf.Lerp(0f, 1f, Mathf.PingPong(Time.time * blinkSpeed, 1f));
            for (int i = 0; i < mats.Length; i++)
            {
                mats[i].SetFloat("_Metallic", newMetallicValue);
            }

            yield return null;

            elapsedTime += Time.deltaTime;
        }

        foreach (var material in mats)
        {
            material.SetFloat("_Metallic", 0f);
        }
    }

    private void ResetMetallic(Material[] mats)
    {
        foreach (var material in mats)
        {
            material.SetFloat("_Metallic", 0f);
        }
    }
    
    private GroupItem FindModel(string nameGroup)
    {
        return groups.FirstOrDefault(x => x.nameGroup.Contains(nameGroup));
    }
}

[Serializable]
public class GroupItem
{
    public string nameGroup;
    public Material[] mat;
}