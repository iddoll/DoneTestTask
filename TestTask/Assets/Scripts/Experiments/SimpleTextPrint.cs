using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class SimpleTextPrint : MonoBehaviour
{
    TMP_Text textGameObject;
    private string textBoofer;

    void Start()
    {
        textGameObject = GetComponent<TMP_Text>();
        textBoofer = textGameObject.text;
        textGameObject.text = "";
        StartCoroutine(PrintText());
    }

    IEnumerator PrintText()
    {
        foreach(char abc in textBoofer)
        {
            textGameObject.text += abc;
            yield return new WaitForSeconds(0.05f);

        }

    }
}
