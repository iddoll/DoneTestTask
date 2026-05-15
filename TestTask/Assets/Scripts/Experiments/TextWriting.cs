using System.Collections;
using UnityEngine;

public enum ModeTextWriting
{
    Word,
    Symbol
}

[HelpURL("https://flexreality.atlassian.net/l/cp/hLiyGaPD")]
[ExecuteInEditMode]
public class TextWriting : MonoBehaviour
{
    [Header("Array of text")]
    [Tooltip("Список слов и фраз, которые нужно будет поэтапно вывести")]
    [MultilineAttribute(3)] public string[] textArray;

    [Header("Output settings")]
    [Tooltip("Измените номер во время записи анимации. " +
    "В TextMeshPro будет " +
    "добавленно слово или фраза под указаным номером")]
    public int wordNumber = -1;

    [Tooltip("Режим ввода текста: по слову или фразе сразу " +
        "или по одному символу за определённый период времени")]
    [SerializeField] private ModeTextWriting mode = ModeTextWriting.Symbol;

    [Tooltip("Скорость посимвольного написания текста")]
    [SerializeField]private float timeOfWriting = 0.02f;

    [Tooltip("Отвечает за замену уже существуещего текста")]
    [SerializeField] private bool replacement = false;

    [Tooltip("Разрешает предпросмотр работы компонента вне запуска игры")]
    [SerializeField] private bool workInInspector = false;

    private int wordSelectedNumber = -1;
    private TMPro.TextMeshPro textMesh;
    

    private void Start()
    {
        textMesh = GetComponent<TMPro.TextMeshPro>();
        workInInspector = false;
        wordSelectedNumber = -1;
    }

    private void FixedUpdate() //Работает в игре
    {
        if (wordNumber != wordSelectedNumber)
            if (wordNumber < textArray.Length && wordNumber >= 0)
                if (replacement)
                    AddReplacement(wordNumber);
                else
                    AddWord(wordNumber);
    }

    private void Update() //Работает в редакторе 
    {
        if(workInInspector)
            if (wordNumber != wordSelectedNumber)
                if (wordNumber < textArray.Length && wordNumber >= 0)
                {
                    if (replacement)                   
                        Clear();                                                                  
                    textMesh.text += textArray[wordNumber];
                    wordSelectedNumber = wordNumber;
                }                    
    }

    public void AddReplacement(int number)
    {
        Clear();
        AddWord(number);
    }

    public void AddWord(int number)
    {
        wordNumber = number;
        wordSelectedNumber = number;     
        if (mode == ModeTextWriting.Word)
        {
            textMesh.text += textArray[number];
        }
        else
        {
            StartCoroutine(AddSymbol(number));
        }
    }

   
    public void Clear() //Работает в игре
    {
        textMesh.text = "";
    }
    
    private IEnumerator AddSymbol(int number)
    {
        for (int i = 0; i < textArray[number].Length; i++)
        {
            textMesh.text += textArray[number][i];
            yield return new WaitForSeconds(timeOfWriting);
        }
        yield return null;
    }

    [ContextMenu("Text Clear")]
    private void ClearInInspector() //Вызывается в редакторе 
    {
        Clear();
        wordNumber = -1;
        wordSelectedNumber = -1;
    }
}
