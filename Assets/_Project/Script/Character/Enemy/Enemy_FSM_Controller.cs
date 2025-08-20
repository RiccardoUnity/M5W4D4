using System;
using System.Collections;
using UnityEngine;
using GM;

[RequireComponent(typeof(EnemyHear))]
[RequireComponent(typeof(EnemyView))]
public class Enemy_FSM_Controller : MonoBehaviour
{
    private bool _mainSwitch = true;

    private EnemySenseBrain _brainSense;
    public EnemySenseBrain GetSenseBrain() => _brainSense;
    private Detected _detected;
    public Detected GetDetected() => _detected;

    [Range(0f, 1f)]
    [SerializeField] private float _speedAlert = 2f;
    private float _alertMaxKnown = 0f;
    private float _alertMaxNone = 0.25f;
    private float _alertMaxUnknown = 0.5f;
    private float _alertMaxPlayer = 1f;
    private float _alert;
    public event Action<Enemy_FSM_Controller, float> onAlertChange;

    private FSM_BaseState[] _states;
    private FSM_BaseState _currentState;
    private FSM_BaseState _nextState;

    private IEnumerator _fsmUpdate;
    private float _internalTime;
    private float _perception = Mathf.Epsilon;   //Esiste per dividere il carico di lavoro della CPU su più frame
    [Range(1f, 3f)]
    [SerializeField] private float _perceptionMultiplier = 2f;

    private Vector3 _startPosition;
    public Vector3 GetStartPosition() => _startPosition;

    void Awake()
    {
        _brainSense = GetComponent<EnemySenseBrain>();
        _states = GetComponentsInChildren<FSM_BaseState>();

        if (_states.Length == 0)
        {
            _mainSwitch = false;
            Debug.LogError("Mancano gli stati all'Enemy", gameObject);
        }

        if (_mainSwitch)
        {
            _startPosition = transform.position;

            foreach (FSM_BaseState state in _states)
            {
                if (state.GetStartWithThisState() && state.enabled)
                {
                    _nextState = state;
                }
            }
            if (_nextState == null)
            {
                _nextState = _states[0];
            }

            _fsmUpdate = FSMUpdate();
        }
    }

    public FSM_BaseState GetStateByName(string name)
    {
        foreach(FSM_BaseState state in _states)
        {
            if (state.NameState.Equals(name))
            {
                return state;
            }
        }
        Debug.LogError($"Lo stato {name} non è stato trovato", gameObject);
        return null;
    }

    void OnEnable()
    {
        if (_mainSwitch)
        {
            StartCoroutine(_fsmUpdate);
        }
    }

    void OnDisable()
    {
        if (_fsmUpdate != null)
        {
            StopCoroutine(_fsmUpdate);
        }
    }

    private IEnumerator FSMUpdate()
    {
        //Risolve parecchi problemi, non cancellare per favore
        yield return null;
        float maxAlert = 0f;
        float deltaAlertBar = 0f;
        //"Update"
        while (_mainSwitch)
        {
            //Gestione lo cambio stato
            if (_nextState != null)
            {
                //Uscita dallo stato attuale
                if (_currentState != null)
                {
                    Debug.Log($"FSM StateExit {_currentState.NameState}, name {gameObject.name}");
                    _currentState.StateExit();
                }
                //Cambio stato attuale
                _currentState = _nextState;
                _nextState = null;
                //Entro nel nuovo stato attuale
                Debug.Log($"FSM StateEnter {_currentState.NameState}, name {gameObject.name}");
                _currentState.StateEnter();
            }

            //Tempo di reazione
            //(in realtà è una scusa per dividere il carico di lavoro e dare la priorità a chi ha un _alert più alto)
            _internalTime = 0f;
            maxAlert = GetCurrentMaxAlert(_detected);
            do
            {
                yield return null;
                _internalTime += Time.deltaTime;

                //Ho provato ad usare MoveTorward, ma mi trovo meglio così
                deltaAlertBar = Time.deltaTime / _speedAlert;
                if (_alert != maxAlert)
                {
                    if (Mathf.Abs(maxAlert - _alert) < deltaAlertBar)
                    {
                        _alert = maxAlert;
                    }
                    else
                    {
                        _alert += deltaAlertBar * (maxAlert > _alert ? 1 : -1);
                        onAlertChange(this, _alert);
                    }
                }
            }
            while (_internalTime < _perception);
            _internalTime += Time.deltaTime;

            //Aggiorno lo stato
            _currentState.StateUpdate(_internalTime);

            //Uso i sensi
            _detected = _brainSense.Feedback(this);

            //Controllo transizioni
            _currentState.CheckTransition();
        }

        _fsmUpdate = null;
    }

    private float GetCurrentMaxAlert(Detected state)
    {
        switch (state)
        {
            case Detected.None:
                return _alertMaxNone;
            case Detected.Unknown:
                return _alertMaxUnknown;
            case Detected.Known:
                return _alertMaxKnown;
            case Detected.Player:
                return _alertMaxPlayer;
        }
        return 0f;
    }

    public void SetDetectionWidth(float value)
    {
        _perception = Mathf.Lerp(0.5f, Mathf.Epsilon, value) * _perceptionMultiplier;
        _brainSense.SetDetectionWidth(this, value);
    }

    private bool StateInStates(FSM_BaseState checkState)
    {
        bool hasState = false;
        foreach (FSM_BaseState state in _states)
        {
            if (state == checkState)
            {
                hasState = true;
            }
        }
        return hasState;
    }

    public void SetNextState(FSM_BaseState currentState, FSM_BaseState nextState)
    {
        if (currentState == _currentState && StateInStates(nextState))
        {
            _nextState = nextState;
        }
    }

    public bool IsCurrentStateTake()
    {
        if (_currentState is FSM_S_Take)
        {
            return true;
        }
        return false;
    }

    //Anche se chiamabile da tutti, il valore alla base viene cambiato dopo un'attenta procedura di verifica
    public void MainSwitchOff()
    {
        if (_mainSwitch)
        {
            _mainSwitch = EnemySceneManager.Instance.GetGeneralSwich();
        }
    }
}
