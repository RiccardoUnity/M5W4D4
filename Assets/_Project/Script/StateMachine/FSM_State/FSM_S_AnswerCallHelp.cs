using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GM;
using GSM = GM.GameStaticManager;


public class FSM_S_AnswerCallHelp : FSM_BaseState
{
    public override string NameState { get => GSM.GetStateAnswerCallHelp(); }

    private CharacterBrain _brain;
    private FSM_S_CallHelp _callHelp;

    protected override void Awake()
    {
        _detectionWidth = 0.4f;
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
        _callHelp = _brain.fsmCallHelp;
        _brain.fsmCallHelp = null;
        _callHelp.AnswerToCall(this);

    }

    public override void StateUpdate(float time)
    {
        base.StateUpdate(time);
    }

    public override void StateExit()
    {

    }
}
