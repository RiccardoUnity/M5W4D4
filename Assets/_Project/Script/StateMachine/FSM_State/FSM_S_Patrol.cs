using GM;
using UnityEngine;
using UnityEngine.AI;
using GSM = GM.GameStaticManager;

//Enemy in movimento sul suo percorso, sensorialità media
public class FSM_S_Patrol : FSM_BaseState
{
    public override string NameState { get => NameState; protected set => NameState = GSM.GetStatePatrol(); }

    [SerializeField] private Path _path;
    private NavMeshAgent _agent;

    private float _stopRemaningDistance = 0.2f;
    private bool _hasDestination;
    public bool GetHasDestination() => _hasDestination;
    public void SetHasDestinationFalse() => _hasDestination = false;
    [SerializeField] private EnterPointType _pointType = EnterPointType.None;
    public void SetEnterPointType(EnterPointType value) => _pointType = value;
    private Vector3 _destination;

    protected override void Awake()
    {
        _detectionWidth = 0.5f;
        base.Awake();
        _agent = GetComponentInParent<NavMeshAgent>();
    }

    void Start()
    {
        _transitions = new FSM_Transition[2];

        //Transizione: Individuato Unknown o Player --> Alert
        _transitions[0] = new FSM_Transition(gameObject, 2, _fsmController.GetStateByName(GSM.GetStateAlert()));
        _transitions[0].SetCondition(0, _fsmController.GetDetected, Logic.Equal, Detected.Unknown);
        _transitions[0].SetCondition(1, _fsmController.GetDetected, Logic.Equal, Detected.Player);

        //Transizione: Arrivo a destinazione --> Idle
        _transitions[1] = new FSM_Transition(gameObject, 1, _fsmController.GetStateByName(GSM.GetStateIdle()));
        _transitions[1].SetCondition(0, GetHasDestination, Logic.Equal, false);
    }

    public override void StateEnter()
    {
        base.StateEnter();
        if (!_hasDestination)
        {

            switch (_pointType)
            {
                case EnterPointType.None:
                    _destination = _path.GetCurrentPoint();
                    break;
                case EnterPointType.Random:
                    _destination = _path.GetRandomPoint();
                    break;
                case EnterPointType.Closest:
                    _destination = _path.GetClosestPoint(_agent.transform.position);
                    break;
            }
            _hasDestination = true;
        }
        _agent.SetDestination(_destination);
    }

    public override void StateUpdate()
    {
        if (_agent.remainingDistance < _stopRemaningDistance)
        {
            SetHasDestinationFalse();
        }
    }

    public override void StateExit()
    {

    }
}
