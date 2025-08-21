using GM;
using UnityEngine;
using UnityEngine.AI;
using GSM = GM.GameStaticManager;

//Enemy insegue il Player
public class FSM_S_Chase : FSM_BaseState
{
    public override string NameState { get => GSM.GetStateChase(); }

    private NavMeshAgent _agent;
    private NavMeshPath _path;
    public bool GetPathValid() => _path.status == NavMeshPathStatus.PathComplete;
    [SerializeField] private float _plusSpeed = 1f;

    protected override void Awake()
    {
        _detectionWidth = 1f;
        base.Awake();
        _agent = GetComponentInParent<NavMeshAgent>();
        _path = new NavMeshPath();
        _useStep = true;
    }

    protected override void Start()
    {
        base.Start();
        _transitions = new FSM_Transition[3];

        //Transizione: se raggiungo il Player --> Take
        _transitions[0] = new FSM_Transition(transform.parent.gameObject, NameState, 2, _fsmController.GetStateByName(GSM.GetStateTake()));
        _transitions[0].SetCondition(0, TargetAchieved, Logic.Equal, true);
        _transitions[0].SetCondition(1, TargetLost, Logic.Equal, false);

        //Transizione: se perdo di vista il Player --> Search
        _transitions[1] = new FSM_Transition(transform.parent.gameObject, NameState, 2, _fsmController.GetStateByName(GSM.GetStateSearch()));
        _transitions[1].SetCondition(0, TargetAchieved, Logic.Equal, true);
        _transitions[1].SetCondition(1, TargetLost, Logic.Equal, true);

        //Transizione: se il Player è irraggiungibile --> CallHelp
        _transitions[2] = new FSM_Transition(transform.parent.gameObject, NameState, 1, _fsmController.GetStateByName(GSM.GetStateCallHelp()));
        _transitions[2].SetCondition(0, GetPathValid, Logic.Equal, false);
    }

    public override void StateEnter()
    {
        base.StateEnter();
        _agent.stoppingDistance = _agent.radius * 2f + 0.1f;
        _agent.speed += _plusSpeed;
    }

    public override void StateUpdate(float time)
    {
        base.StateUpdate(time);
        _agent.CalculatePath(_fsmController.GetSenseBrain().GetTargetV3(), _path);
        if (_path.status == NavMeshPathStatus.PathComplete)
        {
            _agent.path = _path;
        }
    }

    public override void StateExit()
    {
        _agent.speed -= _plusSpeed;
    }

    private bool TargetAchieved() => _agent.remainingDistance <= _agent.stoppingDistance;
    private bool TargetLost() => _fsmController.GetSenseBrain().IsTargetNull();
}
