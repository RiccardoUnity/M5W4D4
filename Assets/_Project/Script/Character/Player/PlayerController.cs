using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private PlayerManager _playerManager;
    private NavMeshAgent _agent;

    private RaycastHit _hit;

    void Awake()
    {
        _playerManager = GetComponentInParent<PlayerManager>();
        _agent = GetComponent<NavMeshAgent>();
        _agent.SetDestination(transform.position);
    }

    void Update()
    {
        //Movimento POSSIBILE tramite click del mouse
        if (_playerManager.GetMove() == Vector3.zero)
        {
            //IMPORTANTE !!! - Ricordati di escludere il canvas in overlay
            if (_playerManager.GetHasRay() && !InteractableSceneManager.Instance.GetIsGraphicRayCollider())
            {
                if (Physics.Raycast(_playerManager.GetRay(), out _hit))
                {
                    _agent.SetDestination(_hit.point);
                }
            }
        }
        //Movimento tramite tastiera
        else
        {
            _agent.SetDestination(transform.position + _playerManager.GetMove() * _agent.speed);
        }
    }
}
