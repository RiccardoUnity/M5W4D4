using GM;
using UnityEngine;
using UnityEngine.AI;
using GSM = GM.GameStaticManager;


public class FSM_S_AnswerCallHelp : FSM_BaseState
{
    public override string NameState { get => GSM.GetStateAnswerCallHelp(); }

    private CharacterBrain _brain;
    private FSM_S_CallHelp _callHelp;
    private NavMeshAgent _agent;
    private NavMeshPath _path;
    private bool GetPathStatus() => _path.status == NavMeshPathStatus.PathComplete;
    private bool _targetInRange;
    private bool GetTargetInRange() => _targetInRange;
    private float _stoppingDistancePoint = 0.1f;

    protected override void Awake()
    {
        _detectionWidth = 0.4f;
        base.Awake();
        _brain = GetComponentInParent<CharacterBrain>();
        _agent = GetComponentInParent<NavMeshAgent>();
        _path = new NavMeshPath();
    }

    protected override void Start()
    {
        base.Start();
        _transitions = new FSM_Transition[2];

        //Transizione: se sono arrivato alla posizione --> Search
        _transitions[0] = new FSM_Transition(_fsmController, NameState, 1, _fsmController.GetStateByName(GSM.GetStateSearch()));
        _transitions[0].SetCondition(0, GetTargetInRange, Logic.Equal, true);

        //Transizione: se non posso andare --> Idle
        _transitions[1] = new FSM_Transition(_fsmController, NameState, 1, _fsmController.GetStateByName(GSM.GetStateIdle()));
        _transitions[1].SetCondition(0, GetPathStatus, Logic.Equal, false);

        if (_fsmController.debug)
        {
            Debug.Log($"Transizioni create, state {NameState}", this);
        }
    }

    public override void StateEnter()
    {
        base.StateEnter();
        _callHelp = _brain.fsmCallHelp;
        _brain.fsmCallHelp = null;  //Altrimenti attiva un'altra volta la transizione in _anyState
        _callHelp.AnswerToCall(this);
        _targetInRange = false;
        if (_fsmController.debug)
        {
            Debug.Log($"Rispondo alla chiamata, inizio conversazione, {NameState}", this);
        }
    }

    public override void StateUpdate(float time)
    {
        base.StateUpdate(time);

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

    }

    public bool SetDestination(FSM_S_CallHelp fsmCallHelp, Vector3 position)
    {
        if (fsmCallHelp == _callHelp)
        {
            _agent.CalculatePath(position, _path);
            if (_path.status == NavMeshPathStatus.PathComplete)
            {
                _agent.path = _path;
                _agent.stoppingDistance = _stoppingDistancePoint;
                if (_fsmController.debug)
                {
                    Debug.Log($"Mi dirigo alla posizione segnalata, fine conversazione, {NameState}", this);
                }
                _callHelp = null;
                return true;
            }
            if (_fsmController.debug)
            {
                Debug.Log($"Posizione non raggiungibile, fine conversazione, {NameState}", this);
            }
            return false;
        }
        if (_fsmController.debug)
        {
            Debug.Log($"Chi mi ha chiamato non è lo stesso di chi mi ha passato la posizione, fine conversazione, {NameState}", this);
        }
        return false;
    }
}
