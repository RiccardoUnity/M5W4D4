using System;
using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private bool _isOpen;
    [SerializeField] private float _speedDoor = 2f;
    private IEnumerator _animation;
    private bool _isInAnimation;
    [SerializeField] private Vector3 _closePosition;
    [SerializeField] private bool _setCurrentPositionAsClosePosition;
    [SerializeField] private Vector3 _openPosition;
    [SerializeField] private bool _setCurrentPositionAsOpenPosition;
    private Vector3 _directionToOpen;
    private Vector3 _deltaMove;
    private float _distanceSqr;
    private float _currentDistanceSqr;
    public event Action onAnimationComplete;

    //Sinceramente non avevo voglia e tempo di settare tutto a mano
    void OnValidate()
    {
        if (_setCurrentPositionAsClosePosition)
        {
            _setCurrentPositionAsClosePosition = false;
            _closePosition = transform.position;
        }

        if (_setCurrentPositionAsOpenPosition)
        {
            _setCurrentPositionAsOpenPosition = false;
            _openPosition = transform.position;
        }
    }

    void Awake()
    {
        _directionToOpen = (_closePosition - _openPosition).normalized;
        _distanceSqr = (_closePosition - _openPosition).sqrMagnitude;
    }

    public void MoveDoor()
    {
        _isOpen = !_isOpen;
        if (_animation == null)
        {
            _animation = AnimationDoor();
            StartCoroutine(_animation);
        }
    }

    private IEnumerator AnimationDoor()
    {
        _isInAnimation = true;
        while (_isInAnimation)
        {
            _deltaMove = _directionToOpen * Time.deltaTime / _speedDoor;
            transform.position += _deltaMove * ((_isOpen) ? -1f : 1f);
            yield return null;

            //Si sta aprendo
            if (_isOpen)
            {
                _currentDistanceSqr = (transform.position - _closePosition).sqrMagnitude;
                if (_currentDistanceSqr > _distanceSqr)
                {
                    transform.position = _openPosition;
                    _isInAnimation = false;
                }
            }
            //Si sta chiudendo
            else
            {
                _currentDistanceSqr = (transform.position - _openPosition).sqrMagnitude;
                if (_currentDistanceSqr > _distanceSqr)
                {
                    transform.position = _closePosition;
                    _isInAnimation = false;
                }
            }
        }
        onAnimationComplete?.Invoke();
        _animation = null;
    }
}
