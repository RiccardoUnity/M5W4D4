using UnityEngine;
using GSM = GM.GameStaticManager;

//Enemy insegue il Player
public class FSM_S_Chase : FSM_BaseState
{
    public override string NameState { get => NameState; protected set => NameState = GSM.GetStateChase(); }

    protected override void Awake()
    {
        _detectionWidth = 1f;
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
