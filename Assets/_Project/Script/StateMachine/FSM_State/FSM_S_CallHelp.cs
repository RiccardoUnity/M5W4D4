using UnityEngine;
using GSM = GM.GameStaticManager;

public class FSM_S_CallHelp : FSM_BaseState
{
    public override string NameState { get => GSM.GetStateCallHelp(); }

    private CharacterBrain _character;

    protected override void Awake()
    {
        _detectionWidth = 0.25f;
        base.Awake();
        _character = GetComponentInParent<CharacterBrain>();
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

    public override void StateUpdate(float time)
    {
        base.StateUpdate(time);
    }

    public override void StateExit()
    {

    }
}
