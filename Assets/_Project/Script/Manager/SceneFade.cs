using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SceneFade : MonoBehaviour
{
    #region Singleton
    private static int _count;
    private static SceneFade _instance;

    public static SceneFade Instance
    {
        get
        {
            if ( _instance == null )
            {
                GameObject gameObject = new GameObject("SceneFade - " + _count.ToString());
                Debug.Log($"{gameObject.name} creato");
                gameObject.AddComponent<SceneFade>();
                ++_count;
            }
            return _instance;
        }
    }

    void Awake()
    {
        if ( _instance == null )
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log($"{gameObject.name} assegnato");
        }
        else
        {
            Destroy(gameObject);
            Debug.Log($"{gameObject.name} distrutto");
        }
    }

    void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
            _count = 0;
        }
    }
    #endregion

    private Image _fadeImage;
    public Image FadeImage
    {
        get => _fadeImage;
        set
        {
            //Valido solo al cambio di scena
            if (_fadeImage == null)
            {
                _fadeImage = value;
                _color = _fadeImage.color;
                _color.a = 1f;
                _fadeImage.color = _color;
                StartFade(null);
            }
        }
    }
    [Range(0.5f, 5f)] [SerializeField] private float _fadeSpeed;
    private Color _color;

    private bool _isInAnimation;
    private IEnumerator _fade;
    private bool _isFadeIn = true;

    public void StartFade(Action onExit)
    {
        if (_fade == null)
        {
            _fade = Fade(onExit);
            StartCoroutine(_fade);
        }
    }

    private IEnumerator Fade(Action onExit)
    {
        _isInAnimation = true;
        while (_isInAnimation)
        {
            yield return null;
            _color.a += Time.deltaTime / _fadeSpeed * (_isFadeIn ? -1f : 1f);

            if (_isFadeIn && _color.a < 0f)
            {
                _color.a = 0f;
                _isInAnimation = false;
            }
            else if (!_isFadeIn && _color.a > 1f)
            {
                _color.a = 1f;
                _isInAnimation = false;
            }
            _fadeImage.color = _color;
        }

        _fadeImage.raycastTarget = false;
        _isFadeIn = !_isFadeIn;
        _fade = null;
        onExit?.Invoke();
    }
}
