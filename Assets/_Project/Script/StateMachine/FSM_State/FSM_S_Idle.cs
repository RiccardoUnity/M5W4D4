using UnityEngine;
using GSM = GM.GameStaticManager;

//Enemy sta ferma sul posto
public class FSM_S_Idle : FSM_BaseState
{
    public override string NameState { get => NameState; protected set => NameState = GSM.GetStateIdle(); }

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
