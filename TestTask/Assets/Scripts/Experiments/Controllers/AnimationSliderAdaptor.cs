using UnityEngine;

/// <summary>
/// If necessary, use a script 'AnimatorParametersController' to work with the slider
/// </summary>
public class AnimationSliderAdaptor : MonoBehaviour
{
    public string param;
    [SerializeField] private AnimatorParametersController _animatorParametersController;
    [SerializeField] private EventValue _changeValueObject;

    private void Start()
    {
        var myFloatEvent = new FloatEvent();
        if (_changeValueObject.OnChangedValueEvent != null)
        {
            myFloatEvent = _changeValueObject.OnChangedValueEvent;
        }
        myFloatEvent.AddListener((v) => { _animatorParametersController.SetFloat(param, v); });
    }
}