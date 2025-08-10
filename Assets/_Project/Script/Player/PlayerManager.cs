using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private PlayerController _playerController;
    public PlayerController GetPlayerController() => _playerController;

    void Awake()
    {
        Debug.Log("Player chiama InteractableSceneManager");
        InteractableSceneManager.Instance.SetPlayer(transform);
        _playerController = GetComponentInChildren<PlayerController>();
    }

    void Start()
    {
        
    }
}
