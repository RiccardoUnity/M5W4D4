using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private PlayerController _playerController;
    public PlayerController GetPlayerController() => _playerController;

    private Camera _camera;
    private Quaternion _deltaRotationMove;
    private float _lenght;
    private Vector3 _move;
    public Vector3 GetMove() => _move;

    private bool _hasRay;
    public bool GetHasRay() => _hasRay;
    private Ray _ray;
    public Ray GetRay() => _ray;

    void Awake()
    {
        Debug.Log("Player chiama InteractableSceneManager");
        InteractableSceneManager.Instance.SetPlayer(this);
        _playerController = GetComponentInChildren<PlayerController>();
    }

    void Start()
    {
        _camera = Camera.main;
        _deltaRotationMove = Quaternion.Euler(0f, _camera.transform.eulerAngles.y, 0f);
    }

    void Update()
    {
        InputRead();
    }

    private void InputRead()
    {
        _move = _deltaRotationMove * new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

        if (_move == Vector3.zero)
        {
            if (Input.GetMouseButton(0))
            {
                _ray = _camera.ScreenPointToRay(Input.mousePosition);
                _hasRay = true;
            }
            else
            {
                _hasRay = false;
            }
        }
        else
        {
            _lenght = _move.sqrMagnitude;
            if (_lenght > 1f)
            {
                _lenght = Mathf.Sqrt(_lenght);
                _move.x /= _lenght;
                _move.z /= _lenght;
            }
        }
    }
}
