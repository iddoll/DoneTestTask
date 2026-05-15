using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventsAnimator : MonoBehaviour
{
    public GameObject btnNext;
    public GameObject btnBack;
    public Rigidbody[] rigidbodyes;
    public Vector3[] posStart;

    public delegate void EndExp();
    public EndExp endExp;

    private void Start()
    {
        if (rigidbodyes.Length > 0)
        {
            posStart = new Vector3[rigidbodyes.Length];
            for (int i = 0; i < rigidbodyes.Length; i++)
            {
                posStart[i] = rigidbodyes[i].transform.position;
            }
        }
    }

    public void ShowBtn()
    {
        if (btnNext != null)
            btnNext.SetActive(true);
        if (btnBack != null)
            btnBack.SetActive(true);
    }

    public void HideBtn()
    {
        if (btnNext != null)
            btnNext.SetActive(false);
        if (btnBack != null)
            btnBack.SetActive(false);
    }

    public void EndExperiment()
    {
        if(endExp!=null)
        {
            endExp.Invoke();
        }
    }
    public void DisableKinematik(int id)
    {
        rigidbodyes[id].isKinematic = false;
        
    }

    public void EnableKinematik(int id)
    {
        rigidbodyes[id].isKinematic = true;
rigidbodyes[id].transform.position = posStart[id];
    }
}
