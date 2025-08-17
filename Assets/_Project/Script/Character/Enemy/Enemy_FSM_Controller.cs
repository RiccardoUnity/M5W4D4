using System;
using System.Collections;
using UnityEngine;
using GM;

[RequireComponent(typeof(EnemyHear))]
[RequireComponent(typeof(EnemyView))]
public class Enemy_FSM_Controller : MonoBehaviour
{
    private bool _mainSwitch = true;

    private EnemySenseBrain _brain;
    private Detected _detected;
    public Detected GetDetected() => _detected;

    [Range(0f, 1f)]
    [SerializeField] private float _speedAlert = 2f;
    private float _alertMaxKnown = 0f;
    private float _alertMaxNone = 0.25f;
    private float _alertMaxUnknown = 0.5f;
    private float _alertMaxPlayer = 1f;
    private float _alert;
    public Action<Enemy_FSM_Controller, float> onAlertChange;

    private FSM_BaseState[] _states;
    private FSM_BaseState _currentState;
    private FSM_BaseState _nextState;

    private IEnumerator _fsmUpdate;
    private float _internalTime;
    private float _perception = Mathf.Epsilon;   //Esiste per dividere il carico di lavoro della CPU su più frame
    [SerializeField] private float _perceptionMultiplier = 0.5f;

    void Awake()
    {
        _brain = GetComponent<EnemySenseBrain>();
        _states = GetComponentsInChildren<FSM_BaseState>();

        if (_states.Length == 0)
        {
            _mainSwitch = false;
            Debug.LogError("Mancano gli stati all'Enemy", gameObject);
        }

        if (_mainSwitch)
        {
            foreach (FSM_BaseState state in _states)
            {
                if (_currentState == null || state.GetStartWithThisState())
                {
                    _nextState = state;
                }
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
        float delta = 0f;
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

            //Uso i sensi
            _detected = _brain.Feedback(this);

            //Tempo di reazione
            //(in realtà è una scusa per dividere il carico di lavoro e dare la priorità a chi ha un _alert più alto)
            _internalTime = 0f;
            maxAlert = GetCurrentMaxAlert(_detected);
            while (_internalTime < _perception)
            {
                yield return null;
                _internalTime += Time.deltaTime;

                //Ho provato ad usare MoveTorward, ma mi trovo meglio così
                delta = Time.deltaTime / _speedAlert;
                if (Mathf.Abs(maxAlert - _alert) < delta && _alert != maxAlert)
                {
                    _alert = maxAlert;
                }
                else
                {
                    _alert += delta * (maxAlert > _alert ? 1 : -1);
                    onAlertChange(this, _alert);
                }
            }
            yield return null;  //Per sicurezza, ALTO rischio di loop infiniti

            //Aggiorno lo stato
            _currentState.StateUpdate();

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
        _perception = Mathf.Max(Mathf.Epsilon, value * _perceptionMultiplier);
        _brain.SetDetectionWidth(this, value);
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
}
