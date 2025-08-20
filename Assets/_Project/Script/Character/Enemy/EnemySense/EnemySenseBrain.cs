using GM;
using UnityEngine;

[RequireComponent(typeof(CharacterBrain))]
[RequireComponent(typeof(Enemy_FSM_Controller))]
[RequireComponent(typeof(EnemyHear))]
[RequireComponent(typeof(EnemyView))]
public class EnemySenseBrain : MonoBehaviour
{
    private Detected _detected;

    private CharacterBrain _brain;
    private Enemy_FSM_Controller _fsmController;
    private EnemyHear _enemyHear;
    private EnemyView _enemyView;

    private CharacterBrain _target;
    public CharacterBrain GetTarget() => _target;
    public bool IsTargetNull() => _target == null;
    private Vector3 _targetV3;
    public Vector3 GetTargetV3() => _targetV3;

    private void Awake()
    {
        _brain = GetComponent<CharacterBrain>();
        _fsmController = GetComponent<Enemy_FSM_Controller>();
        _enemyHear = GetComponent<EnemyHear>();
        _enemyView = GetComponent<EnemyView>();
    }

    public Detected Feedback(Enemy_FSM_Controller fsmController)
    {
        if (_fsmController == fsmController)
        {
            _detected = Detected.None;
            _target = null;

            //La vista ha la priorità sull'udito
            CharacterBrain[] characterChats = _enemyView.See(this);

            foreach (CharacterBrain character in characterChats)
            {
                if (character != _brain)
                {
                    if (_brain.IsAlreadyMet(character))
                    {
                        if (character.GetIsPlayer())
                        {
                            //È il Player
                            _detected = Detected.Player;
                            _target = character;
                            _targetV3 = character.transform.position;
                        }
                        else
                        {
                            //Enemy conosciuto
                            if (_detected == Detected.None)
                            {
                                _detected = Detected.Known;
                            }
                        }
                    }
                    else
                    {
                        //Non lo conosco
                        if (_detected < Detected.Unknown)
                        {
                            _detected = Detected.Unknown;
                            _target = character;
                            _targetV3 = character.transform.position;
                        }
                    }
                }
            }

            if (_target == null && _enemyHear.Felt())
            {
                _detected = Detected.Unknown;
                _targetV3 = _enemyHear.GetPositionTarget();
            }

            return _detected;
        }
        return Detected.None;
    }

    public void SetDetectionWidth(Enemy_FSM_Controller fsmController, float value)
    {
        if (fsmController == _fsmController)
        {
            _enemyView.ChangeFocusAngle(this, value);
        }
    }
}
