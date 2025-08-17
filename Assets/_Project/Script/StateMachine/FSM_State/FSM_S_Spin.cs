using UnityEngine;
using GSM = GM.GameStaticManager;

//Enemy ruota sul posto in base a degli intervalli, sensorialità media
public class FSM_S_Spin : FSM_BaseState
{
    public override string NameState { get => NameState; protected set => NameState = GSM.GetStateSpin(); }

    protected override void Awake()
    {
        _detectionWidth = 0.25f;
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
