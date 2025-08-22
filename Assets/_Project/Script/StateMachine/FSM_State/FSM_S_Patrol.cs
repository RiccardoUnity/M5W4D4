using GM;
using UnityEngine;
using UnityEngine.AI;
using GSM = GM.GameStaticManager;

//Enemy in movimento sul suo percorso, sensorialità media
public class FSM_S_Patrol : FSM_BaseState
{
    public override string NameState { get => GSM.GetStatePatrol(); }

    private NavMeshAgent _agent;
    [SerializeField] private Path _path;

    private float _stoppingDistancePoint = 0.1f;
    private bool _hasDestination;
    public bool GetHasDestination() => _hasDestination;
    public void SetHasDestinationFalse() => _hasDestination = false;
    [SerializeField] private EnterPointType _pointType = EnterPointType.None;
    public void SetEnterPointType(EnterPointType value) => _pointType = value;
    private Vector3 _destination;

    [SerializeField] private int _startIndex;
    private int _currentIndex;
    [SerializeField] private bool _inverse;

    protected override void Awake()
    {
        _detectionWidth = 0.5f;
        base.Awake();
        _agent = GetComponentInParent<NavMeshAgent>();
        _useStep = true;
        _currentIndex = _startIndex;
        //Evito che 2 agenti rimangano bloccati sul posto per andare in direzioni opposte
        _agent.avoidancePriority = Random.Range(10, 80);
    }

    protected override void Start()
    {
        base.Start();
        _transitions = new FSM_Transition[3];

        //Transizione: se individuo il Player --> Alert
        _transitions[0] = new FSM_Transition(_fsmController, NameState, 1, _fsmController.GetStateByName(GSM.GetStateAlert()));
        _transitions[0].SetCondition(0, _fsmController.GetDetected, Logic.Equal, Detected.Player);

        //Transizione: se individuo Unknown --> Alert
        _transitions[1] = new FSM_Transition(_fsmController, NameState, 1, _fsmController.GetStateByName(GSM.GetStateAlert()));
        _transitions[1].SetCondition(0, _fsmController.GetDetected, Logic.Equal, Detected.Unknown);

        //Transizione: se arrivo a destinazione --> Idle
        _transitions[2] = new FSM_Transition(_fsmController, NameState, 1, _fsmController.GetStateByName(GSM.GetStateIdle()));
        _transitions[2].SetCondition(0, GetHasDestination, Logic.Equal, false);

        if (_fsmController.debug)
        {
            Debug.Log($"Transizioni create, state {NameState}", this);
        }
    }

    public override void StateEnter()
    {
        base.StateEnter();

        _agent.stoppingDistance = _stoppingDistancePoint;
        if (!_hasDestination)
        {

            switch (_pointType)
            {
                case EnterPointType.None:
                    _destination = _path.GetCurrentPoint(_currentIndex);
                    break;
                case EnterPointType.Next:
                    _destination = _path.GetNextPoint(ref _currentIndex, _inverse);
                    break;
                case EnterPointType.Random:
                    _destination = _path.GetRandomPoint(_currentIndex);
                    break;
                case EnterPointType.Closest:
                    _destination = _path.GetClosestPoint(_agent.transform.position);
                    break;
            }
            _hasDestination = true;
        }
        _agent.SetDestination(_destination);
        if (_fsmController.debug)
        {
            Debug.Log($"Diretto alla destinazione {_destination}, {NameState}", this);
        }
    }

    public override void StateUpdate(float time)
    {
        base.StateUpdate(time);
        if (_agent.remainingDistance <= _agent.stoppingDistance)
        {
            _pointType = EnterPointType.Next;
            SetHasDestinationFalse();
            if (_fsmController.debug)
            {
                Debug.Log($"Giunto a destinazione, {NameState}", this);
            }
        }
    }

    public override void StateExit()
    {
        _agent.ResetPath();
    }

    public void ChangeVerse() => _inverse = !_inverse;


}
