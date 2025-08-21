using GM;
using UnityEngine;
using UnityEngine.AI;
using GSM = GM.GameStaticManager;

//Enemy ha perso il Player, alterna i sensi per trovare la sua posizione, sensorialità massima in diminuzione
public class FSM_S_Search : FSM_BaseState
{
    public override string NameState { get => GSM.GetStateSearch(); }

    private CharacterBrain _brain;
    private NavMeshAgent _agent;
    private NavMeshPath _path;
    [Range(1f, 5f)][SerializeField] private float _timer = 3f;
    private float _stoppingDistancePoint = 0.1f;
    private bool _isGoingToNextPoint;
    private bool _isUnknown;

    [Range(1f, 10f)][SerializeField] private int _randomPoints = 5;
    private int _leghtRandomV3;
    private Vector3[] _randomV3;
    private int _count;
    private NavMeshHit _hitNavMesh;
    [Range(5f, 15f)][SerializeField] private float _searchRadius = 10f;

    protected override void Awake()
    {
        _detectionWidth = 0.25f;
        base.Awake();
        _brain = GetComponentInParent<CharacterBrain>();
        _agent = GetComponentInParent<NavMeshAgent>();
        _randomV3 = new Vector3[_randomPoints];
        _path = new NavMeshPath();
        _useStep = true;
    }

    protected override void Start()
    {
        base.Start();
        _transitions = new FSM_Transition[3];

        //Transizione: se individuo il Player --> Chase
        _transitions[0] = new FSM_Transition(transform.parent.gameObject, NameState, 1, _fsmController.GetStateByName(GSM.GetStateAlert()));
        _transitions[0].SetCondition(0, _fsmController.GetDetected, Logic.Equal, Detected.Player);

        //Transizione: se individuo Unknown --> Alert
        _transitions[1] = new FSM_Transition(transform.parent.gameObject, NameState, 2, _fsmController.GetStateByName(GSM.GetStateAlert()));
        _transitions[1].SetCondition(0, _fsmController.GetDetected, Logic.Equal, Detected.Unknown);
        _transitions[1].SetCondition(1, _fsmController.GetSenseBrain().IsTargetNull, Logic.Equal, false);

        //Transizione: se non trovo nulla --> Idle
        _transitions[2] = new FSM_Transition(transform.parent.gameObject, NameState, 1, _fsmController.GetStateByName(GSM.GetStateIdle()));
        _transitions[2].SetCondition(0, IsVisitedAllPoints, Logic.Equal, true);
    }

    public override void StateEnter()
    {
        base.StateEnter();
        SetRandomPointOnNavMesh();
        _agent.stoppingDistance = _stoppingDistancePoint;
    }

    public override void StateUpdate(float time)
    {
        base.StateUpdate(time);

        if (_isUnknown)
        {
            //Arrivato al Unknown, cerca
            if (_agent.remainingDistance <= _agent.stoppingDistance)
            {
                SetRandomPointOnNavMesh();
            }
            //else non fare niente
        }
        else
        {
            //Ha trovato un Unknown, parte
            if (_fsmController.GetDetected() == Detected.Unknown)
            {
                _agent.SetDestination(_fsmController.GetSenseBrain().GetTargetV3());
                _isUnknown = true;
            }
            else
            {
                if (_brain.IsInternalTimerNull())
                {
                    //Parte
                    if (!_isGoingToNextPoint && _agent.remainingDistance <= _agent.stoppingDistance)
                    {
                        _agent.SetDestination(_randomV3[_count]);
                        _isGoingToNextPoint = true;
                    }
                    //Arriva
                    else if (_agent.remainingDistance <= _agent.stoppingDistance)
                    {
                        _brain.StartTimer(_timer);
                        _isGoingToNextPoint = false;
                        ++_count;
                    }
                }
            }
        }
    }

    public override void StateExit()
    {

    }

    private void SetRandomPointOnNavMesh()
    {
        Vector3 randomV3;
        
        _leghtRandomV3 = 0;
        for (int i = 0; i < _randomPoints; i++)
        {
            _count = 0;
            do
            {
                randomV3 = transform.position + Random.insideUnitSphere * _searchRadius;
                randomV3.y = transform.position.y;
                ++_count;
            }
            while (!NavMesh.SamplePosition(randomV3, out _hitNavMesh, _searchRadius, _agent.areaMask) || _count > 100);
            ++_leghtRandomV3;
            _randomV3[i] = randomV3;
        }
        _count = 0;
        _isGoingToNextPoint = false;
        _isUnknown = false;
    }

    private bool IsVisitedAllPoints() => _count == _leghtRandomV3;
}
