using GM;
using System;
using UnityEngine;
using Debug = UnityEngine.Debug;
using GSM = GM.GameStaticManager;

[Serializable]
public abstract class FSM_BaseState : MonoBehaviour
{
    protected Enemy_FSM_Controller _fsmController;

    public abstract string NameState { get; }
    [SerializeField] private bool _startWithThisState;
    public bool GetStartWithThisState() => _startWithThisState;

    protected float _timeState;
    public float GetTimeState() => _timeState;
    protected float _detectionWidth;    //Vista più ampia, suoni minori -> 0; Vista focalizzata, suoni maggiori -> 1;

    protected FSM_Transition[] _transitions;
    private FSM_Transition[] _anyState;
    protected FSM_BaseState _nextState;

    private Func<bool> _isSomeoneCallMe;

    protected bool _useStep;
    protected Noise _noise;
    protected float _timeStep = 1f;
    private float _lastTimeStep;

    protected virtual void Awake()
    {
        _fsmController = GetComponentInParent<Enemy_FSM_Controller>();
        _detectionWidth = Mathf.Clamp01(_detectionWidth);
        _isSomeoneCallMe = GetComponentInParent<CharacterBrain>().IsSomeoneCallMe;
        _noise = GetComponentInParent<Noise>();
    }

    protected virtual void Start()
    {
        _anyState = new FSM_Transition[2];

        //Transizione: ho sentito un altro Enemy --> Chat
        _anyState[0] = new FSM_Transition(_fsmController, NameState, 2, _fsmController.GetStateByName(GSM.GetStateChat()));
        _anyState[0].SetCondition(0, IsChatState, Logic.Equal, false);
        _anyState[0].SetCondition(1, HeardOtherEnemy, Logic.Equal, true);

        //Transizione: se qualcuno mi chiama --> AnswerCallHelp
        _anyState[1] = new FSM_Transition(_fsmController, NameState, 1, _fsmController.GetStateByName(GSM.GetStateAnswerCallHelp()));
        _anyState[1].SetCondition(0, IsSomeoneCallMe, Logic.Equal, true);
    }

    public virtual void StateEnter()
    {
        _fsmController.SetDetectionWidth(_detectionWidth);
        _timeState = 0f;
        _lastTimeStep = 0f;
    }

    public virtual void StateUpdate(float time)
    {
        _timeState += time;
        if (_useStep && _lastTimeStep + _timeStep < _timeState)
        {
            _lastTimeStep = _timeState;
            _noise.Emission();
        }
    }

    public abstract void StateExit();

    public virtual void CheckTransition()
    {
        //Any state
        foreach (FSM_Transition anyState in _anyState)
        {
            _nextState = anyState.IsConditionMet();
            if (_nextState != null)
            {
                _fsmController.SetNextState(this, _nextState);
                return;
            }
        }

        //Current state
        foreach (FSM_Transition transition in _transitions)
        {
            _nextState = transition.IsConditionMet();
            if (_nextState != null)
            {
                _fsmController.SetNextState(this, _nextState);
                return;
            }
        }
    }

    private bool IsChatState() => NameState.Equals(GSM.GetStateChat());
    private bool HeardOtherEnemy() => _fsmController.GetSenseBrain().GetLastNoiseType() == NoiseType.ChatEnemy;
    private bool IsSomeoneCallMe() => _isSomeoneCallMe.Invoke();
}
