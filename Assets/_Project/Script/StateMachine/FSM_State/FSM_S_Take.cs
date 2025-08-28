using GM;
using UnityEngine;
using UnityEngine.AI;
using GSM = GM.GameStaticManager;

public class FSM_S_Take : FSM_BaseState
{
    public override string NameState { get => GSM.GetStateTake(); }

    [SerializeField] private GameObject _gameOverScreen;
    [SerializeField] private PlayerController _playerController;
    private CharacterBrain _characterBrain;
    private NavMeshAgent _agent;

    private float _distanceSqr;
    private float _agentRadiusSqr;

    protected override void Awake()
    {
        _detectionWidth = 0f;
        base.Awake();
        _characterBrain = GetComponentInParent<CharacterBrain>();
        _agent = GetComponentInParent<NavMeshAgent>();
        _agentRadiusSqr = (_agent.radius * 2f + 0.1f);
        _agentRadiusSqr *= _agentRadiusSqr;
    }

    protected override void Start()
    {
        base.Start();
        _transitions = new FSM_Transition[2];

        //Transizioni di sicurezza
        _transitions[0] = new FSM_Transition(_fsmController, NameState, 1, _fsmController.GetStateByName(GSM.GetStateChase()));
        _transitions[0].SetCondition(0, IsPlayerNear, Logic.Equal, false);
    }

    public override void StateEnter()
    {
        base.StateEnter();
        _distanceSqr = (_playerController.transform.position - _fsmController.transform.position).sqrMagnitude;
        if (IsPlayerNear())
        {
            _gameOverScreen.SetActive(true);
            _playerController.enabled = false;
            EnemySceneManager.Instance.SwitchOffAllEnemy(_fsmController, _characterBrain.GetID());
        }
    }

    public override void StateUpdate(float time)
    {
        base.StateUpdate(time);
    }

    public override void StateExit()
    {

    }

    private bool IsPlayerNear() => _distanceSqr < _agentRadiusSqr;
}
