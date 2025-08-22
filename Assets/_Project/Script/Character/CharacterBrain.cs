using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

//Qui ogni Character può chiedere cose ad un altro Character
//Solo gli Enemy hanno un ID unico, il Player si arrangia
//Gli Enemy non sanno chi è il Player a inizio partita, lo scoprono se fornisce un ID falso
//Qui gli Enemy tengono traccia di chi incontrano/conoscono
public class CharacterBrain : MonoBehaviour
{
    private bool _isPlayer;
    public bool GetIsPlayer() => _isPlayer;

    private string _id;
    public string GetID() => _id;
    private Enemy_FSM_Controller _fsmController;
    private List<CharacterBrain> _metCharacters = new List<CharacterBrain>();

    private FSM_S_Chat _fsmChat;
    public FSM_S_Chat GetFSMStateChat() => _fsmChat;
    [HideInInspector] public FSM_S_CallHelp fsmCallHelp;
    public bool IsSomeoneCallMe() => fsmCallHelp != null;

    private IEnumerator _internalTimer;
    public bool IsInternalTimerNull() => _internalTimer == null;
    private const float _maxTimer_c = 10f;
    private const float _minTimer_c = 5f;
    private float _timer;
    public float GetTimer() => _timer;
    private float _timerCoroutine;
    public void SetTimerCoroutineZero() => _timerCoroutine = 0f;

    void Awake()
    {
        _fsmController = GetComponent<Enemy_FSM_Controller>();
        if (_fsmController == null )
        {
            _isPlayer = true;
        }
        else
        {
            _id = EnemySceneManager.Instance.AddEnemy(this);
        }

        _timer = Random.Range(_minTimer_c, _maxTimer_c);
    }

    public bool SetMyStateChat(Enemy_FSM_Controller fsmController, FSM_S_Chat fsmStateChat)
    {
        if (_fsmController == fsmController && _fsmChat == null)
        {
            _fsmChat = fsmStateChat;
            return true;
        }
        Debug.LogError("Non è stato possibile settare lo stato Chat nel CharacterBrain", gameObject);
        return false;
    }

    public bool StartTimer(float timer)
    {
        if (_internalTimer == null)
        {
            _internalTimer = InternalTimer(timer);
            StartCoroutine(_internalTimer);
            return true;
        }
        return false;
    }

    private IEnumerator InternalTimer(float timer)
    {
        _timerCoroutine = timer;
        while(_timerCoroutine > 0f)
        {
            yield return null;
            _timerCoroutine -= Time.deltaTime;
        }
        _internalTimer = null;
    }

    public void OverrideStopTimer()
    {
        _timerCoroutine = 0;
        _internalTimer = null;
    }

    public bool AddCharacter(FSM_S_Chat fsmStateChat, CharacterBrain character)
    {
        if (fsmStateChat == _fsmChat && character != null && character != FindPlayerInMetCharacter())
        {
            _metCharacters.Add(character);
            return true;
        }
        return false;
    }

    public bool IsAlreadyMet(CharacterBrain character)
    {
        if (_metCharacters.Contains(character))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public CharacterBrain FindPlayerInMetCharacter()
    {
        foreach (CharacterBrain character in _metCharacters)
        {
            if(character.GetIsPlayer())
            {
                return character;
            }
        }
        return null;
    }

    //È più probabile che i character ad essere aggiunti alla lista siano anche i più vicini fisicamente
    public CharacterBrain GetEnemyKnow(int index)
    {
        if (_metCharacters.Count > 0)
        {
            return _metCharacters[index];
        }
        return null;
    }
}
