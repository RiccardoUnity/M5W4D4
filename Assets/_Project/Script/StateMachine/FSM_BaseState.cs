using UnityEngine;

[System.Serializable]
public abstract class FSM_BaseState : MonoBehaviour
{
    protected Enemy_FSM_Controller _fsmController;

    public abstract string NameState { get; }
    [SerializeField] private bool _startWithThisState;
    public bool GetStartWithThisState() => _startWithThisState;

    protected float _timeState;
    public float GetTimeState() => _timeState;
    protected float _detectionWidth;    //Vista più ampia, suoni minori -> 0; Vista focalizzata, suoni maggiori -> 1;

    protected FSM_Transition[] _transitions;
    protected FSM_BaseState _nextState;

    protected virtual void Awake()
    {
        _fsmController = GetComponentInParent<Enemy_FSM_Controller>();
        _detectionWidth = Mathf.Clamp01(_detectionWidth);
    }

    public virtual void StateEnter()
    {
        _fsmController.SetDetectionWidth(_detectionWidth);
        _timeState = 0f;
    }

    public virtual void StateUpdate(float time)
    {
        _timeState += time;
    }

    public abstract void StateExit();

    public virtual void CheckTransition()
    {
        foreach (FSM_Transition transition in _transitions)
        {
            _nextState = transition.IsConditionMet();
            if (_nextState != null)
            {
                _fsmController.SetNextState(this, _nextState);
            }
        }
        //for (int i = 0; i < _transitions.Length; i++)
        //{
        //    Debug.Log(i);
        //    _nextState = _transitions[i].IsConditionMet();
        //    if (_nextState != null)
        //    {
        //        _fsmController.SetNextState(this, _nextState);
        //    }
        //}
    }
}
