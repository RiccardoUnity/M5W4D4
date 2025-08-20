using GM;
using System;
using UnityEngine;

public class FSM_Transition
{
    public class FSM_Condition
    {
        private Condition _condition;

        private Logic _logic;

        private Func<bool> _getValue_b;
        private bool _value_b;

        private Func<float> _getValue_f;
        private float _value_f;

        private Func<Detected> _getValue_detected;
        private Detected _value_detected;

        public FSM_Condition(Func<bool> GetValue, Logic logic, bool value)
        {
            _condition = Condition.Bool;
            _getValue_b = GetValue;
            //Forzo logic ad essere o Equal o NotEqual
            if (logic != Logic.NotEqual)
            {
                logic = Logic.Equal;
            }
            _logic = logic;
            _value_b = value;
        }

        public FSM_Condition(Func<float> GetValue, Logic logic, float value)
        {
            _condition = Condition.Float;
            _getValue_f = GetValue;
            _logic = logic;
            _value_f = value;
        }

        public FSM_Condition(Func<Detected> GetValue, Logic logic, Detected value)
        {
            _condition = Condition.Detected;
            _getValue_detected = GetValue;
            _logic = logic;
            _value_detected = value;
        }

        public bool IsConditionMet()
        {
            switch (_condition)
            {
                case Condition.Bool:
                    switch (_logic)
                    {
                        case Logic.NotEqual:
                            return (_getValue_b.Invoke() != _value_b);
                        case Logic.Equal:
                            return (_getValue_b.Invoke() == _value_b);
                        //Obbligatorio
                        default:
                            return false;
                    }

                case Condition.Float:
                    switch (_logic)
                    {
                        case Logic.NotEqual:
                            return (_getValue_f.Invoke() != _value_f);
                        case Logic.Less:
                            return (_getValue_f.Invoke() < _value_f);
                        case Logic.LessEqual:
                            return (_getValue_f.Invoke() <= _value_f);
                        case Logic.Equal:
                            return (_getValue_f.Invoke() == _value_f);
                        case Logic.GreaterEqual:
                            return (_getValue_f.Invoke() >= _value_f);
                        case Logic.Greater:
                            return (_getValue_f.Invoke() > _value_f);
                        //Obbligatorio
                        default:
                            return false;
                    }

                case Condition.Detected:
                    switch (_logic)
                    {
                        case Logic.NotEqual:
                            return (_getValue_detected.Invoke() != _value_detected);
                        case Logic.Less:
                            return (_getValue_detected.Invoke() < _value_detected);
                        case Logic.LessEqual:
                            return (_getValue_detected.Invoke() <= _value_detected);
                        case Logic.Equal:
                            return (_getValue_detected.Invoke() == _value_detected);
                        case Logic.GreaterEqual:
                            return (_getValue_detected.Invoke() >= _value_detected);
                        case Logic.Greater:
                            return (_getValue_detected.Invoke() > _value_detected);
                        //Obbligatorio
                        default:
                            return false;
                    }
            }
            //Obbligatorio
            return false;
        }
    }

    private GameObject _gameObject;
    private string _myNameState;
    private FSM_Condition[] _conditions;
    private bool[] _valueConditions;

    private FSM_BaseState _nextState;

    private bool _mainSwitch = true;

    public FSM_Transition(GameObject gameObject, string myNameState, int numberOfConditions, FSM_BaseState nextState)
    {
        _gameObject = gameObject;
        _myNameState = myNameState;
        _conditions = new FSM_Condition[numberOfConditions];
        _valueConditions = new bool[numberOfConditions];
        if (nextState != null)
        {
            _nextState = nextState;
        }
        else
        {
            _mainSwitch = false;
            Debug.LogError($"Stato non trovato: {nextState}", _gameObject);
        }
    }

    public void SetCondition(int index, Func<bool> GetValue, Logic logic, bool value)
    {
        if (ConditionExist(index))
        {
            _conditions[index] = new FSM_Condition(GetValue, logic, value);
        }
    }

    public void SetCondition(int index, Func<float> GetValue, Logic logic, float value)
    {
        if (ConditionExist(index))
        {
            _conditions[index] = new FSM_Condition(GetValue, logic, value);
        }
    }

    public void SetCondition(int index, Func<Detected> GetValue, Logic logic, Detected value)
    {
        if (ConditionExist(index))
        {
            _conditions[index] = new FSM_Condition(GetValue, logic, value);
        }
    }

    private bool ConditionExist(int index)
    {
        if (_mainSwitch)
        {
            if (index >= 0 && index < _conditions.Length)
            {
                if (_conditions[index] == null)
                {
                    return true;
                }
                else
                {
                    Debug.LogError($"Condizione già esistente, non sovrascrivibile, {_myNameState}", _gameObject);
                }
            }
            else
            {
                Debug.LogError($"Condizione non registrata, StackOverFlow, {_myNameState}", _gameObject);
            }
        }
        return false;
    }

    public FSM_BaseState IsConditionMet()
    {
        if (_mainSwitch)
        {
            for (int i = 0; i < _conditions.Length; i++)
            {
                _valueConditions[i] = _conditions[i].IsConditionMet();
            }

            foreach (bool value in _valueConditions)
            {
                if (!value)
                {
                    return null;
                }
            }
            Debug.Log($"Transizione {_myNameState} --> {_nextState.NameState}, GameObject {_gameObject.name}");
            return _nextState;
        }
        return null;
    }
}