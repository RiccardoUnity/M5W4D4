using UnityEngine;

public class Path
{
    [SerializeField] private Transform[] _transforms;
    [SerializeField] private int _startIndex;

    private bool _mainSwitch = true;
    private GameObject _gameObject;

    public Path(GameObject gameObject)
    {
        _gameObject = gameObject;
    }

    public Transform SetUp()
    {
        if (_transforms == null)
        {
            _mainSwitch = false;
            Debug.LogError("Nessuna posizione inserita nello script Path", _gameObject);
        }

        if (_mainSwitch)
        {
            if (_startIndex > _transforms.Length || _startIndex < 0)
            {
                _startIndex = 0;
            }
            return _transforms[_startIndex];
        }
        else
        {
            return null;
        }
    }

    public Transform GetNextTransform(Transform current)
    {
        if (_mainSwitch)
        {
            for (int  i = 0; i < _transforms.Length; i++)
            {
                if (_transforms[i] == current)
                {
                    return _transforms[(i + 1 == _transforms.Length) ? 0 : i + 1];
                }
            }
        }
        return null;
    }

    public Transform GetPreviousTransform(Transform current)
    {
        if (_mainSwitch)
        {
            for (int i = _transforms.Length - 1; i >= 0; i--)
            {
                if (_transforms[i] == current)
                {
                    return _transforms[(i - 1 > 0) ? _transforms.Length : i - 1];
                }
            }
        }
        return null;
    }
}
