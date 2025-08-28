using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Noise))]
public class PlayerController : MonoBehaviour
{
    private bool _mainSwitch = true;

    private PlayerManager _playerManager;
    private NavMeshAgent _agent;

    private RaycastHit _hit;
    private IEnumerator _runDestination;

    private Quaternion _targetRotation;

    [Tooltip("Tempo per compiere un passo")]
    [Range(0.1f, 2f)]
    [SerializeField] private float _timeStep = 1f;
    private float _lastTimeStep;
    public event Action onStep;

    void Awake()
    {
        _playerManager = GetComponentInParent<PlayerManager>();
        _agent = GetComponent<NavMeshAgent>();
        _agent.SetDestination(transform.position);  //L'agente ha per forza una destinazione non null
        _runDestination = RunDestination();
    }

    void OnEnable()
    {
        _lastTimeStep = Time.time;
        StartCoroutine(_runDestination);
    }

    void OnDisable()
    {
        StopCoroutine(_runDestination);
        _agent.isStopped = true;
    }

    private IEnumerator RunDestination()
    {
        while (_mainSwitch)
        {
            yield return null;

            //Movimento POSSIBILE tramite click del mouse
            if (_playerManager.GetMove() == Vector3.zero)
            {
                if (_playerManager.GetHasRay() && !_playerManager.GetCanvasOverload().IsPointerOnOverlay() && !InteractableSceneManager.Instance.GetIsGraphicRayCollider())
                {
                    if (Physics.Raycast(_playerManager.GetRay(), out _hit))
                    {
                        _agent.isStopped = false;
                        _agent.SetDestination(_hit.point);
                    }
                }
            }
            //Movimento tramite tastiera
            else
            {
                _agent.isStopped = true;

                _targetRotation = Quaternion.LookRotation(_playerManager.GetMove(), Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, _targetRotation, _agent.angularSpeed * Time.deltaTime);

                _agent.Move(_playerManager.GetMove() * (_agent.speed * Time.deltaTime));
            }

            //Un evento per ogni passo
            if (_agent.remainingDistance > 0.2f && _lastTimeStep + _timeStep < Time.time)
            {
                _lastTimeStep = Time.time;
                onStep?.Invoke();
            }
        }
    }
}
