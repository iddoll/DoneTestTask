using System;
using TMPro;
using UnityEngine;

public class ChangeColorOperation : Operation
{
    [Header("Where to change")]
    [SerializeField] private TextMeshPro tmp;
    [SerializeField] private Renderer renderer;
    
    [Header("Color")]
    [SerializeField] private Color color;
    
    [Header("Optional")]
    [SerializeField] private bool hasChangeFaceColor;
    [SerializeField] private string colorShaderAttribute = "_ColorAlbedo";
    
    private Color _oldColor;
    private bool _isChanged;
    
    public override void DoAction()
    {
        SetChangeColor();
        base.DoAction();
    }

    public override void UnDoAction()
    {
        SetDefaultColor();
    }

    private void SetChangeColor()
    {
        if (tmp != null)
        {
            if (hasChangeFaceColor)
            {
                if (!_isChanged) _oldColor = tmp.faceColor;
                tmp.faceColor = color;
            }
            else
            {
                if (!_isChanged) _oldColor = tmp.color;
                tmp.color = color;
            }
        }

        if (renderer != null)
        {
            if (!_isChanged) _oldColor = renderer.material.GetColor(colorShaderAttribute);
            renderer.material.SetColor(colorShaderAttribute, color);
        }

        _isChanged = true;
    }
    
    private void SetDefaultColor()
    {
        if (!_isChanged) return;
        if (tmp != null)
        {
            if (hasChangeFaceColor)
            {
                tmp.faceColor = _oldColor;
            }
            else
            {
                tmp.color = _oldColor;
            }
        }

        if (renderer != null)
        {
            renderer.material.SetColor(colorShaderAttribute, _oldColor);
        }
    }
}
