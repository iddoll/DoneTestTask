using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ProbabilityСalculation : MonoBehaviour
{
    [SerializeField] private RandomRotationCube[] _cubes;
    [SerializeField] private TextMeshPro _label;
    [SerializeField] private float animTime = 1.5f;
    [SerializeField][Multiline]private string _defouldText = "Відносна частота випадіння «1» становить " +
        "кількість випадання «1» / кількість підкидань";
    [SerializeField][Multiline] private string _resultText = "Відносна частота випадіння «1» становить ";

    private float _tossing;
    private float _resultDropout;
    private bool _play = true;
   
    private void Start()
    {
        ResetGame();
    }

    public void ClickButton()
    {
        if (!_play) return;
        _play = false;
        _tossing += _cubes.Length;
        StartCoroutine(animUnFrize());
        for (int i = 0; i < _cubes.Length; i++)
        {
            _cubes[i].JumpCube();
            if (_cubes[i].Direction() == 0)
            {
               _resultDropout++;
            }
        }       
        string TemporaryResultText = _resultText + _resultDropout + " / " + _tossing + " = " + _resultDropout / _tossing;
        if (_label) _label.text = TemporaryResultText;
    }

    public void ResetGame()
    {
        _tossing = 0;
        _resultDropout = 0;
       if(_label) _label.text = _defouldText;
    }

    private IEnumerator animUnFrize()
    {
        yield return new WaitForSeconds(animTime);
        _play = true;
    }
}