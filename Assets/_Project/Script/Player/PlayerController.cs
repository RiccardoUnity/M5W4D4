using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private Camera _camera;
    private NavMeshAgent _agent;
    private Vector3 _move;
    private Quaternion _deltaRotationMove;

    private RaycastHit _hit;
    private LayerMask _layerMask = (1 << 0) | (1 << 5);

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.SetDestination(transform.position);
    }

    void Start()
    {
        _camera = Camera.main;
        _deltaRotationMove = Quaternion.Euler(0f, _camera.transform.eulerAngles.y, 0f);
    }

    void Update()
    {

        _move = _deltaRotationMove * new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

        //Movimento POSSIBILE tramite click del mouse
        if (_move == Vector3.zero)
        {
            if (Input.GetMouseButton(0))
            {
                Ray ray = _camera.ScreenPointToRay(Input.mousePosition);


                if (Physics.Raycast(ray, out _hit))
                {
                    _agent.SetDestination(_hit.point);
                }
            }
        }
        //Movimento tramite tastiera
        else
        {
            float lenght = _move.sqrMagnitude;
            if (lenght > 1f)
            {
                lenght = Mathf.Sqrt(lenght);
                _move.x /= lenght;
                _move.z /= lenght;
            }

            _agent.SetDestination(transform.position + _move * _agent.speed);
        }
    }
}
