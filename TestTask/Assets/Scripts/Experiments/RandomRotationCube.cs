using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RandomRotationCube : MonoBehaviour
{    
    [Range(0, 1)]
    [SerializeField] private float _transformStep = 0;
    [Range(0, 1)]
    [SerializeField] private float _rotationStep = 0;
    [SerializeField] private float _forceMin = 2;
    [SerializeField] private float _forceMax = 5;
    [SerializeField] private string _triggerAnimationName = "Jump";
    [SerializeField] private bool _jump;
    
    private Vector3[] _diractionVector = {
        new Vector3(-90,0,0),
        new Vector3(-180,0,0),
        new Vector3(-180,0,90),
        new Vector3(0,0,90),
        new Vector3(0,0,0),
        new Vector3(90,0,0),
    };

    private Animator _anim;
    private Vector3 _rotationStart;
    private Vector3 _rotationLast;
    private Vector3 _positionStart;
    private Vector3 _positionLast;
    private float _forseJump;
    private int _diraction;


    private void Start()
    {
        _positionStart = transform.localPosition;
        _anim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        Bounce();
    }

    private void Bounce()
    {
        if (!_jump) return;
        transform.localPosition = Vector3.Lerp(_positionStart, _positionLast, _transformStep);
        transform.eulerAngles = Vector3.Lerp(_rotationStart, _rotationLast, _rotationStep);
    }

    public void JumpCube()
    {
            _forseJump = Random.Range(_forceMin, _forceMax);
            _diraction = Random.Range(0, 6);
            _rotationLast = _diractionVector[_diraction];
            _rotationStart = transform.eulerAngles;
            _positionLast = new Vector3(_positionStart.x, _positionStart.y + _forseJump, _positionStart.z);
            _anim.SetTrigger(_triggerAnimationName);
            _rotationStep = 0;              
    }

    public int Direction()
    {
        return _diraction;
    }
}