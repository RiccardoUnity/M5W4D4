using UnityEngine;
using GSM = GM.GameStaticManager;

public class FSM_S_Take : FSM_BaseState
{
    public override string NameState { get => NameState; protected set => NameState = GSM.GetStateTake(); }

    protected override void Awake()
    {
        _detectionWidth = 0f;
        base.Awake();
    }

    void Start()
    {
        _transitions = new FSM_Transition[1];

        //Transizione:
    }

    public override void StateEnter()
    {
        base.StateEnter();
    }

    public override void StateUpdate()
    {

    }

    public override void StateExit()
    {

    }
}
