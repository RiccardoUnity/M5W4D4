using UnityEngine;
using GSM = GM.GameStaticManager;

//Enemy ha perso il Player, alterna i sensi per trovare la sua posizione, sensorialità massima in diminuzione
public class FSM_S_Search : FSM_BaseState
{
    public override string NameState { get => NameState; protected set => NameState = GSM.GetStateSearch(); }

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
