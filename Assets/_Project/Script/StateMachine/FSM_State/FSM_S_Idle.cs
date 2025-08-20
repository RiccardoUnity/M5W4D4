using GM;
using UnityEngine;
using GSM = GM.GameStaticManager;

//Enemy sta ferma sul posto
public class FSM_S_Idle : FSM_BaseState
{
    public override string NameState { get => GSM.GetStateIdle(); }

    [Range(1f, 10f)][SerializeField] private float _waiting = 5f;
    
    [SerializeField] private bool _isRandomWaiting;
    [Range(1f, 10f)][SerializeField] private float _maxWaiting = 10f;

    protected override void Awake()
    {
        _detectionWidth = 0f;
        base.Awake();

        if (_isRandomWaiting)
        {
            _waiting = Random.Range(1f, _maxWaiting);
        }
    }

    void Start()
    {
        _transitions = new FSM_Transition[3];

        //Transizione: Individuato Player --> Alert
        _transitions[0] = new FSM_Transition(gameObject, NameState, 1, _fsmController.GetStateByName(GSM.GetStateAlert()));
        _transitions[0].SetCondition(0, _fsmController.GetDetected, Logic.Equal, Detected.Player);

        //Transizione: Individuato Unknown --> Alert
        _transitions[1] = new FSM_Transition(gameObject, NameState, 1, _fsmController.GetStateByName(GSM.GetStateAlert()));
        _transitions[1].SetCondition(0, _fsmController.GetDetected, Logic.Equal, Detected.Unknown);

        //Transizione: Fine del tempo di attesa --> Patrol || Spin
        FSM_BaseState state = _fsmController.GetStateByName(GSM.GetStatePatrol());
        if (state == null || !state.enabled)
        {
            state = _fsmController.GetStateByName(GSM.GetStateSpin());
            if (state == null || !state.enabled)
            {
                Debug.LogError("Ci deve essere almeno uno stato Patrol o Spin attivo in questa state machine", gameObject);
            }
        }
        _transitions[2] = new FSM_Transition(gameObject, NameState, 1, state);
        _transitions[2].SetCondition(0, GetTimeState, Logic.Greater, _waiting);
    }

    public override void StateEnter()
    {
        base.StateEnter();
    }

    public override void StateUpdate(float time)
    {
        base.StateUpdate(time);
    }

    public override void StateExit()
    {

    }
}
