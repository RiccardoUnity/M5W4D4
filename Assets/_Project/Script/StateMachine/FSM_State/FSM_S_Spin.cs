using GM;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using GSM = GM.GameStaticManager;

//Enemy ruota sul posto in base a degli intervalli, sensorialità media
public class FSM_S_Spin : FSM_BaseState
{
    public override string NameState { get => GSM.GetStateSpin(); }

    [Range(45f, 180f)]
    [SerializeField] private float _angleRotation = 180f;
    [SerializeField] private bool _useRandomAngle;
    private float _startAngle;
    private float _currentAngle;
    private float _endAngle;
    [Range(22.5f, 360f)]
    [SerializeField] private float _angularSpeed = 45f;
    private float _currentAngularSpeed;
    [SerializeField] private bool _isClockwise = true;
    [SerializeField] private bool _isPingPong = false;
    private IEnumerator _rotationUpdate;
    private bool _stayInCoroutine = true;
    private bool _isRotationCompleted;
    private bool IsRotationCompleted() => _isRotationCompleted;

    protected override void Awake()
    {
        _detectionWidth = 0.25f;
        base.Awake();
        _startAngle = _fsmController.transform.eulerAngles.y;
    }

    void Start()
    {
        _transitions = new FSM_Transition[2];

        //Transizione: Individuato Player --> Alert
        _transitions[0] = new FSM_Transition(gameObject, NameState, 1, _fsmController.GetStateByName(GSM.GetStateAlert()));
        _transitions[0].SetCondition(0, _fsmController.GetDetected, Logic.Equal, Detected.Player);

        //Transizione: Individuato Unknown --> Alert
        _transitions[1] = new FSM_Transition(gameObject, NameState, 1, _fsmController.GetStateByName(GSM.GetStateAlert()));
        _transitions[1].SetCondition(0, _fsmController.GetDetected, Logic.Equal, Detected.Unknown);

        //Transizione: Fine del movimento --> Idle
        _transitions[1] = new FSM_Transition(gameObject, NameState, 1, _fsmController.GetStateByName(GSM.GetStateIdle()));
        _transitions[1].SetCondition(0, IsRotationCompleted, Logic.Equal, true);
    }

    public override void StateEnter()
    {
        base.StateEnter();
        _currentAngle = _fsmController.transform.eulerAngles.y;
        if (_useRandomAngle)
        {
            _endAngle = Random.Range(0f, _angleRotation);
            _endAngle = _currentAngle + _endAngle * (_isClockwise ? 1 : -1);
        }
        else
        {
            _endAngle = _startAngle + (_isClockwise ? _angleRotation : -_angleRotation);
        }
        StayIn360Degrees(ref _endAngle);
        _isRotationCompleted = false;

        if (_rotationUpdate == null)
        {
            _rotationUpdate = RotationUpdate();
        }
        StartCoroutine(_rotationUpdate);
    }

    private IEnumerator RotationUpdate()
    {
        while (_stayInCoroutine)
        {
            yield return null;

            if (!_isRotationCompleted)
            {
                _currentAngularSpeed = _angularSpeed * Time.deltaTime;
                _currentAngle += _currentAngularSpeed * (_isClockwise ? 1 : -1);
                StayIn360Degrees(ref _currentAngle);
                if (_currentAngle > _endAngle - _currentAngularSpeed && _currentAngle < _endAngle + _currentAngularSpeed)
                {
                    _currentAngle = _endAngle;
                    _isRotationCompleted = true;
                }
                //Posso lasciare gli zeri perchè uso la navMesh ...
                _fsmController.transform.rotation = Quaternion.Euler(0f, _currentAngle, 0f);
            }
        }
        Debug.Log("Sei uscito dalla Coroutine delle rotazioni della macchina a stati", gameObject);
        _rotationUpdate = null;
    }

    public override void StateUpdate(float time)
    {
        base.StateUpdate(time);

    }

    public override void StateExit()
    {
        StopCoroutine(_rotationUpdate);
        if (_isPingPong)
        {
            _isClockwise = !_isClockwise;
        }
        _startAngle = _endAngle;
    }

    //private void StayIn360Degrees(ref float angle) => angle += (angle > 360f ? -360f : (angle < 0f ? 360f : 0f));
    private void StayIn360Degrees(ref float angle)
    {
        if (angle > 360f)
        {
            angle -= 360f;
        }
        else if (angle < 0)
        {
            angle += 360f;
        }
    }
}
