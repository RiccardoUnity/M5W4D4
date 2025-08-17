using UnityEngine;

public class Path : MonoBehaviour
{
    private bool _mainSwitch = true;

    [SerializeField] private Transform[] _transforms;
    [SerializeField] private int _startIndex;
    private int _currentIndex;
    [SerializeField] private bool _inverse;

    void Awake()
    {
        if (_transforms == null)
        {
            _mainSwitch = false;
            Debug.LogError("Nessuna posizione inserita nello script Path", gameObject);
        }
        else if (_transforms.Length < 2)
        {
            _mainSwitch = false;
            Debug.LogError("2 posizione minime da inserire nello script Path", gameObject);
        }

        if (_mainSwitch)
        {
            if (_startIndex > _transforms.Length || _startIndex < 0)
            {
                _currentIndex = 0;
            }
            _currentIndex = _startIndex;
        }
    }

    public void ChangeVerse() => _inverse = !_inverse;

    public Vector3 GetCurrentPoint() => _transforms[_currentIndex].position;

    private Vector3 GetNext()
    {
        _currentIndex = (_currentIndex + 1 == _transforms.Length) ? 0 : _currentIndex + 1;
        return _transforms[_currentIndex].position;
    }

    private Vector3 GetPrevious()
    {
        _currentIndex = (_currentIndex - 1 > 0) ? _transforms.Length : _currentIndex - 1;
        return _transforms[_currentIndex].position;
    }

    public Vector3 GetNextPoint()
    {
        if (_mainSwitch)
        {
            return (_inverse ? GetPrevious() : GetNext());
        }
        return transform.position;
    }

    public Vector3 GetPreviousPoint()
    {
        if (_mainSwitch)
        {
            return (_inverse ? GetNext() : GetPrevious());
        }
        return transform.position;
    }

    public Vector3 GetRandomPoint()
    {
        int randomIndex = 0;
        if ( _mainSwitch)
        {
            while (randomIndex != _currentIndex)
            {
                randomIndex = Random.Range(0, _transforms.Length);
            }
            _currentIndex = randomIndex;
            return _transforms[_currentIndex].position;
        }
        return transform.position;
    }

    public Vector3 GetClosestPoint(Vector3 position)
    {
        if (_mainSwitch)
        {
            float distanceSqr = float.MaxValue;
            float currentDistance;
            Vector3 closest = transform.position;
            foreach(Transform transform in _transforms)
            {
                currentDistance = (transform.position - position).sqrMagnitude;
                if (currentDistance < distanceSqr)
                {
                    distanceSqr = currentDistance;
                    closest = transform.position;
                }
            }
            return closest;
        }
        return transform.position;
    }
}
