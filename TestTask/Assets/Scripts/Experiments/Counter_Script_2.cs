using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Counter_Script_2 : MonoBehaviour
{

    public TextMeshPro _Text;
    public string _unit;
    public  float _rate;
    private string _rateString;
    
    
    // Start is called before the first frame update
    void Start()
    {

        
    }

    // Update is called once per frame
    void Update()
    {
        _rateString = _rate.ToString("0.0");
        _Text.text = _rateString + _unit;
    }
}
