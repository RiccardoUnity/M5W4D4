using GM;
using UnityEngine;
using GSM = GM.GameStaticManager;

public class FSM_S_CallHelp : FSM_BaseState
{
    public override string NameState { get => GSM.GetStateCallHelp(); }

    private CharacterBrain _brain;
    private CharacterBrain _characterCalled;
    private int _count;
    private FSM_S_AnswerCallHelp _answerCallHelp;
    private bool _closeCall;
    private bool GetCloseCall() => _closeCall;

    protected override void Awake()
    {
        _detectionWidth = 0.25f;
        base.Awake();
        _brain = GetComponentInParent<CharacterBrain>();
    }

    protected override void Start()
    {
        base.Start();
        _transitions = new FSM_Transition[1];

        //Transizione: chiamate terminate --> Idle
        _transitions[0] = new FSM_Transition(_fsmController, NameState, 1, _fsmController.GetStateByName(GSM.GetStateIdle()));
        _transitions[0].SetCondition(0, GetCloseCall, Logic.Equal, true);

        if (_fsmController.debug)
        {
            Debug.Log($"Transizioni create, state {NameState}", this);
        }
    }

    public override void StateEnter()
    {
        base.StateEnter();
        _count = 0;
        _answerCallHelp = null;
        _closeCall = false;
    }

    public override void StateUpdate(float time)
    {
        base.StateUpdate(time);
        if (_brain.IsInternalTimerNull())
        {
            //Qualcuno ha risposto
            if (_answerCallHelp != null)
            {
                _closeCall = _answerCallHelp.SetDestination(this, _fsmController.GetSenseBrain().GetTargetV3());
                if (_fsmController.debug)
                {
                    Debug.Log($"Qualcuno ha risposto, inizio conversazione, {NameState}", this);
                }
            }
            //Devo chiamare
            else
            {
                _characterCalled = _brain.GetEnemyKnow(_count);
                ++_count;
                //Non conosco nessun'altro, lascio stare
                if (_characterCalled == null)
                {
                    _closeCall = true;
                    if (_fsmController.debug)
                    {
                        Debug.Log($"Nessun'altro da chiamare, {NameState}", this);
                    }
                }
                //Provo a sentirlo
                else
                {
                    _characterCalled.fsmCallHelp = this;
                    _brain.StartTimer(_brain.GetTimer());
                    if (_fsmController.debug)
                    {
                        Debug.Log($"Chiamo chi conosco, {NameState}", this);
                    }
                }
            }
        }
    }

    public override void StateExit()
    {

    }

    public void AnswerToCall(FSM_S_AnswerCallHelp fsmAnswerCallHelp) => _answerCallHelp = fsmAnswerCallHelp;
}
