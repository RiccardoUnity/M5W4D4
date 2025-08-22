using GM;
using UnityEngine;
using UnityEngine.AI;
using GSM = GM.GameStaticManager;

//Enemy ha avvistato il Player ma non ne è sicuro
public class FSM_S_Alert : FSM_BaseState
{
    public override string NameState { get => GSM.GetStateAlert(); }

    private NavMeshAgent _agent;
    private NavMeshPath _path;
    public bool GetPathValid() => _path.status == NavMeshPathStatus.PathComplete;
    [SerializeField] private float _stopFromTarget = 4f;
    public float StopFromTarget() => _stopFromTarget;
    private bool _isTargetPlayer;
    public bool GetIsTargetPlayer() => _isTargetPlayer;
    private bool _targetInRange;

    protected override void Awake()
    {
        _detectionWidth = 0.75f;
        base.Awake();
        _agent = GetComponentInParent<NavMeshAgent>();
        _path = new NavMeshPath();
        _useStep = true;
    }

    protected override void Start()
    {
        base.Start();
        _transitions = new FSM_Transition[4];

        //Transizione: Ho percepito il Player --> Chase
        _transitions[0] = new FSM_Transition(_fsmController, NameState, 1, _fsmController.GetStateByName(GSM.GetStateChase()));
        _transitions[0].SetCondition(0, GetIsTargetPlayer, Logic.Equal, true);

        //Transizione: Ha raggiunto il target --> Chat
        _transitions[1] = new FSM_Transition(_fsmController, NameState, 1, _fsmController.GetStateByName(GSM.GetStateChat()));
        _transitions[1].SetCondition(0, TryToChat, Logic.Equal, true);

        //Transizione: Ha raggiunto la posizione del target --> Search
        _transitions[2] = new FSM_Transition(_fsmController, NameState, 1, _fsmController.GetStateByName(GSM.GetStateSearch()));
        _transitions[2].SetCondition(0, TryToSearch, Logic.Equal, true);

        //Transizione: Non ho un percorso valido per indagare --> CallHelp
        _transitions[3] = new FSM_Transition(_fsmController, NameState, 1, _fsmController.GetStateByName(GSM.GetStateCallHelp()));
        _transitions[3].SetCondition(0, GetPathValid, Logic.Equal, false);

        if (_fsmController.debug)
        {
            Debug.Log($"Transizioni create, state {NameState}", this);
        }
    }

    public override void StateEnter()
    {
        base.StateEnter();

        _targetInRange = false;
        if (_fsmController.GetDetected() == Detected.Player)
        {
            _isTargetPlayer = true;
            //In questo caso potevo passare fin da subito da Patrol/Spin/Idle a Chase,
            //ma potrei mettere un passaggio intermedio ...
        }
        else
        {
            _isTargetPlayer = false;
            _agent.stoppingDistance = _stopFromTarget;
            SetPath();
        }
    }

    public override void StateUpdate(float time)
    {
        base.StateUpdate(time);
        SetPath();

        if (_agent.remainingDistance <= _agent.stoppingDistance)
        {
            _targetInRange = true;
            if (_fsmController.debug)
            {
                Debug.Log($"Giunto a destinazione, {NameState}", this);
            }
        }
        else
        {
            _targetInRange = false;
        }
    }

    public override void StateExit()
    {
        _agent.ResetPath();
    }

    private void SetPath()
    {
        _agent.CalculatePath(_fsmController.GetSenseBrain().GetTargetV3(), _path);
        if (_path.status == NavMeshPathStatus.PathComplete)
        {
            _agent.path = _path;
            if (_fsmController.debug)
            {
                Debug.Log($"Qualcosa ha attirato la mia attenzione, {NameState}", this);
            }
        }
        else
        {
            if (_fsmController.debug)
            {
                Debug.Log($"Path {_path.status}, posizione {_fsmController.GetSenseBrain().GetTargetV3()}, {NameState} ", this);
            }
        }
    }

    private bool TryToChat() => _targetInRange && !_fsmController.GetSenseBrain().IsTargetNull();

    private bool TryToSearch() => _targetInRange && _fsmController.GetSenseBrain().IsTargetNull();
}
