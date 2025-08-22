using UnityEngine;
using GSM = GM.GameStaticManager;

public class FSM_S_Take : FSM_BaseState
{
    public override string NameState { get => GSM.GetStateTake(); }

    [SerializeField] private GameObject _gameOverScreen;
    [SerializeField] private PlayerController _playerController;
    private CharacterBrain _characterBrain;

    protected override void Awake()
    {
        _detectionWidth = 0f;
        base.Awake();
        _characterBrain = GetComponentInParent<CharacterBrain>();
    }

    protected override void Start()
    {
        base.Start();
        _transitions = new FSM_Transition[0];

        if (_fsmController.debug)
        {
            Debug.Log($"Qui non ci sono transizioni, state {NameState}", this);
        }
    }

    public override void StateEnter()
    {
        base.StateEnter();
        _gameOverScreen.SetActive(true);
        _playerController.enabled = false;
        EnemySceneManager.Instance.SwitchOffAllEnemy(_fsmController, _characterBrain.GetID());
    }

    public override void StateUpdate(float time)
    {
        base.StateUpdate(time);
    }

    public override void StateExit()
    {

    }
}
