using System.Collections;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [Header("Internal")]
    private bool _mainSwitch = true;   //Previene errori se il componente non è configurato correttamente
    private Transform _player;
    [SerializeField] private RectTransform _canvas;
    private Vector3 _startScaleCanvas;
    [SerializeField] private UI_CustomButton _customButton;
    public UI_CustomButton GetCustomButton() => _customButton;

    //Coroutine vars
    private int _awaitFrameCheckPlayer = 10;
    private IEnumerator _checkPlayer;
    private bool _playerIsInRange;
    private IEnumerator _scaleCanvas;
    private bool _isInAnimation;
    private float _fillScale;
    [Range(0.05f, 1f)]
    [SerializeField] private float _timeFillScale = 0.1f;

    [Header("Physics")]
    private Vector3 _offset = new Vector3(0f, 1f, 0f);
    private Vector3 _directionNorm;
    private RaycastHit _hit;
    [SerializeField] private float _maxDistanceRay = 5f;
    private LayerMask _layerMask = (1 << 0) | (1 << 3) | (1 << 6);   //Default 0, Player 3, Enemy 6
    private QueryTriggerInteraction _qti = QueryTriggerInteraction.Ignore;
    public bool _viewGizmos;

    void Awake()
    {
        if (_canvas == null)
        {
            _mainSwitch = false;
            Debug.LogWarning("Manca il riferimento al componente _canvas", gameObject);
        }
        if (_customButton == null)
        {
            _mainSwitch = false;
            Debug.LogWarning("Manca il riferimento al componente _customButton", gameObject);
        }

        if (_mainSwitch)
        {
            InteractableSceneManager.Instance.AddInteractable(this);
            //Distribuisco un minimo il carico di lavoro su frame diversi per ogni istanza
            _awaitFrameCheckPlayer = Random.Range(_awaitFrameCheckPlayer - 5, _awaitFrameCheckPlayer + 5);
            _checkPlayer = CheckPlayer();
        }
    }

    void OnEnable()
    {
        if (_mainSwitch)
        {
            StartCoroutine(_checkPlayer);
        }
    }

    void Start()
    {
        if (_mainSwitch)
        {
            _player = InteractableSceneManager.Instance.GetPlayer().transform;

            _canvas.rotation = Quaternion.Euler(0f, Camera.main.transform.eulerAngles.y, 0f);

            _startScaleCanvas = _canvas.localScale;
            _canvas.localScale = Vector3.zero;
            _canvas.gameObject.SetActive(false);

            //Per buildare la navMesh
            int index = _customButton.onClickComplete.GetPersistentEventCount();
            for (int i = 0; i < index; i++)
            {
                if (_customButton.onClickComplete.GetPersistentTarget(i) is Door target)
                {
                    target.onAnimationComplete += InteractableSceneManager.Instance.GetNavMeshSurface().ReBuildNavMesh;
                }
                break;
            }
        }
    }

    private IEnumerator CheckPlayer()
    {
        while (_mainSwitch)
        {
            for (int i = _awaitFrameCheckPlayer; i > 0; i--)
            {
                yield return null;
            }

            _directionNorm = (_player.position - transform.position).normalized;
            if (Physics.Raycast(transform.position + _offset, _directionNorm, out _hit, _maxDistanceRay, _layerMask, _qti))
            {
                if (_hit.collider.gameObject.layer == _player.gameObject.layer)
                {
                    //PlayerInRange
                    _playerIsInRange = true;
                    _canvas.gameObject.SetActive(true);
                    StartScaleCanvasAnimation();
                }
                else
                {
                    PlayerNotInRange();
                }
            }
            else
            {
                PlayerNotInRange();
            }
        }
    }

    private void StartScaleCanvasAnimation()
    {
        if (_scaleCanvas == null)
        {
            _scaleCanvas = ScaleCanvas();
            StartCoroutine(_scaleCanvas);
        }
    }

    private void PlayerNotInRange()
    {
        _playerIsInRange = false;
        StartScaleCanvasAnimation();
    }

    private IEnumerator ScaleCanvas()
    {
        _isInAnimation = true;
        while (_isInAnimation)
        {
            _fillScale += Time.deltaTime / _timeFillScale * (_playerIsInRange ? 1 : -1);

            if (_playerIsInRange && _fillScale > 1f)
            {
                _fillScale = 1f;
                _canvas.localScale = _startScaleCanvas;
                ExitScaleCanvas();
            }
            else if (!_playerIsInRange && _fillScale < 0f)
            {
                _fillScale = 0f;
                _canvas.localScale = Vector3.zero;
                ExitScaleCanvas();
            }
            else
            {
                _canvas.localScale = Vector3.Lerp(Vector3.zero, _startScaleCanvas, _fillScale);
            }

            yield return null;
        }

        if (!_playerIsInRange)
        {
            //Mi assicuro che la coroutine in _customButton termini prima di spegnere il _canvas
            yield return new WaitUntil(_customButton.AnimationFinish);
            _canvas.gameObject.SetActive(false);
        }
    }

    private void ExitScaleCanvas()
    {
        _isInAnimation = false;
        _scaleCanvas = null;
    }

    public void SwichOff()
    {
        
    }

    void OnDrawGizmos()
    {
        if (_viewGizmos)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position + _offset, _directionNorm * _maxDistanceRay);
        }
    }
}
