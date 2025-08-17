using UnityEngine;
using GSM = GM.GameStaticManager;

//Enemy ha avvistato il Player ma non ne è sicuro
public class FSM_S_Alert : FSM_BaseState
{
    public override string NameState { get => NameState; protected set => NameState = GSM.GetStateAlert(); }

    protected override void Awake()
    {
        _detectionWidth = 0.75f;
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
