using UnityEngine;
using GSM = GM.GameStaticManager;

public class FSM_S_CallHelp : FSM_BaseState
{
    public override string NameState { get => GSM.GetStateCallHelp(); }

    private CharacterBrain _brain;
    private CharacterBrain _characterCalled;
    private int _count;
    private FSM_S_AnswerCallHelp _answerCallHelp;

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

        //Transizione:
    }

    public override void StateEnter()
    {
        base.StateEnter();
        _count = 0;
        _answerCallHelp = null;
    }

    public override void StateUpdate(float time)
    {
        base.StateUpdate(time);
        if (_brain.IsInternalTimerNull())
        {
            //Qualcuno ha risposto
            if (_answerCallHelp != null)
            {
                
            }
            //Devo chiamare
            else
            {
                _characterCalled = _brain.GetEnemyKnow(_count);
                ++_count;
                if (_characterCalled == null)
                {

                }
                else
                {
                    _characterCalled.fsmCallHelp = this;
                    _brain.StartTimer(_brain.GetTimer());
                }
            }
        }
    }

    public override void StateExit()
    {

    }

    public void AnswerToCall(FSM_S_AnswerCallHelp fsmAnswerCallHelp) => _answerCallHelp = fsmAnswerCallHelp;
}
