using System.Collections.Generic;
using UnityEngine;

//Qui ogni Character può chiedere cose ad un altro Character
//Solo gli Enemy hanno un ID unico, il Player si arrangia
//Gli Enemy non sanno chi è il Player a inizio partita, lo scoprono se fornisce un ID falso
//Qui gli Enemy tengono traccia di chi incontrano/conoscono
public class CharacterChat : MonoBehaviour
{
    private PlayerManager _playerManager;
    public PlayerManager GetPlayerManager() => _playerManager;
    private Enemy_FSM_Controller _enemyFSMController;

    private int _id;
    private List<CharacterChat> _metCharacters = new List<CharacterChat>();

    void Awake()
    {
        _playerManager = GetComponent<PlayerManager>();
        _enemyFSMController = GetComponent<Enemy_FSM_Controller>();

        if (_enemyFSMController != null )
        {
            _id = EnemySceneManager.Instance.AddEnemy(this);
        }
    }

    public bool IsAlreadyMet(CharacterChat character)
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
}
