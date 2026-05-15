using UnityEngine;

public class ObjectMoveAndRotate : MonoBehaviour 
{
    [SerializeField] private bool rotate;
    [SerializeField] private bool move;

    [Space(10)] [Header("Rotation Settings")] [Range(0.0f, 200f)] [SerializeField]
    private float rotationSpeed = 100f;
    
    [Space(10)] [Header("Movement Settings")] [SerializeField]
    private AnimationCurve animationCurve;

    [SerializeField] private float duration = 0.5f;
    [SerializeField] private float movementScale = 2.0f;
    [SerializeField] private bool useCycles;
    [SerializeField] private int maxCycles = 12;

    private Vector3 _initialPosition;
    private float _elapsedTime;
    private float _currentAngle;
    private int _currentCycle;
    private bool _isReversing;
    private bool _previousMove;
    private bool _stopAnim;
    private bool _isPause;
    
    public void SetIsPause(bool state) => _isPause = state;

    private void Start()
    {
        _previousMove = move;
        animationCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
        _initialPosition = transform.localPosition;
    }

    private void Update()
    {
        if (rotate) Rotate();

        if (move)
        {
            if (maxCycles == _currentCycle) 
                _currentCycle = 0;
            
            Move();
        }

        if (_previousMove == move) 
            return;
        
        _previousMove = move;
        ResetMove();
    }

    private void Rotate()
    {
        if (_isPause)
            return;
        
        _currentAngle += rotationSpeed * 10 * Time.deltaTime;
        if (_currentAngle >= 360f) _currentAngle -= 360f;
        transform.localRotation = Quaternion.Euler(0, _currentAngle, 0);
    }

    private void Move()
    {
        if (_isPause || _stopAnim) 
            return;

        _elapsedTime += (_isReversing ? -1 : 1) * Time.deltaTime;

        if (_elapsedTime > duration)
        {
            _elapsedTime = duration;
            _isReversing = true;

            if (useCycles) 
                _currentCycle++;
        }
        else if (_elapsedTime < 0.0f)
        {
            _elapsedTime = 0.0f;
            _isReversing = false;

            if (useCycles) 
                _currentCycle++;
        }

        if (useCycles && _currentCycle >= maxCycles)
        {
            _stopAnim = true;
            return;
        }

        var normalizedTime = _elapsedTime / duration;

        var curveValue = animationCurve.Evaluate(normalizedTime);

        transform.localPosition = _initialPosition + new Vector3(0, curveValue * movementScale, 0);
    }

    private void ResetMove() => _stopAnim = false;
}
