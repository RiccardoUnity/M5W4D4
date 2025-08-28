using UnityEngine;
using UnityEngine.AI;

public class AnimatorController : MonoBehaviour
{
    private PlayerManager _playerManager;
    private Animator _animator;
    private NavMeshAgent _agent;
    private string _isWalking = "IsWalking";

    void Awake()
    {
        _playerManager = GetComponentInParent<PlayerManager>();
        _animator = GetComponent<Animator>();
        _agent = GetComponentInParent<NavMeshAgent>();
    }

    void Update()
    {
        if (_playerManager != null)
        {
            if (_playerManager.GetMove() != Vector3.zero)
            {
                SetParameterAnimator(_playerManager.GetMove());
            }
            else
            {
                SetParameterAnimator(_agent.velocity);
            }
        }
        else
        {
            SetParameterAnimator(_agent.velocity);
        }
    }

    private void SetParameterAnimator(Vector3 vector)
    {
        if (vector == Vector3.zero)
        {
            _animator.SetBool(_isWalking, false);
        }
        else
        {
            _animator.SetBool(_isWalking, true);
        }
    }
}
