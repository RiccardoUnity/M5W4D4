using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_CustomButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    private enum Status
    {
        None,       //Nessuna interazione
        SlideText,  //Mouse in hover parte 1
        AlphaImage, //Mouse in hover parte 2
        FillMask,   //Mouse premuto
        Execute     //Avvia evento
    }

    [SerializeField] private Image _border;
    [SerializeField] private Image _maskFill;
    [SerializeField] private TMP_Text _text;

    private TMP_FontAsset _fontRegular;
    [SerializeField] private TMP_FontAsset _fontItalic;

    private Status _status = Status.None;
    [SerializeField] private bool _isPositionStartPosition0;
    [SerializeField] private bool _startInPosition0 = true;
    private Vector3 _position0;
    private Vector3 _position1;
    private float _deltaX;
    private float _offsetMultiplierX = 1.2f;   //120%
    [Range(0.05f, 1f)]
    [SerializeField] private float _timeFillText = 0.1f;
    [Range(0.05f, 1f)]
    [SerializeField] private float _timeFillBorder = 0.25f;
    [Range(0.05f, 1f)]
    [SerializeField] private float _timeFillMask = 0.5f;

    private IEnumerator _animation;
    private bool _isPointerEnter;   //true --> _position1, false --> _position0
    public bool GetIsPointerEnter() => _isPointerEnter;
    private bool _isInAnimation;
    private float _fillText;
    private float _fillBorder;
    private float _fillMask;
    private bool _isPointerDown;

    public UnityEvent onClickComplete;

    private bool _mainSwitch = true;   //Previene errori se il componente non è configurato correttamente

    void Awake()
    {
        if (_border == null)
        {
            _mainSwitch = false;
            Debug.LogWarning("Manca l'immagine _border", gameObject);
        }
        if (_maskFill == null)
        {
            _mainSwitch = false;
            Debug.LogWarning("Manca l'immagine _maskFill", gameObject);
        }
        if (_text == null)
        {
            _mainSwitch = false;
            Debug.LogWarning("Manca l'immagine _text", gameObject);
        }
        else
        {
            _fontRegular = _text.font;
            if (_fontItalic == null)
            {
                _fontItalic = _fontRegular;
            }
        }

        if (_mainSwitch)
        {
            _deltaX = _border.rectTransform.sizeDelta.x * _offsetMultiplierX;
            if (_isPositionStartPosition0)
            {
                _position0 = _text.rectTransform.localPosition;
                _position1 = _text.rectTransform.localPosition + new Vector3(_deltaX, 0f, 0f);
            }
            else
            {
                _position0 = _text.rectTransform.localPosition + new Vector3(_deltaX, 0f, 0f);
                _position1 = _text.rectTransform.localPosition;
            }

            SetUp();
        }
    }

    private void SetUp()
    {
        //Text
        if (_startInPosition0)
        {
            _text.rectTransform.localPosition = _position0;
            _fillText = 0f;
        }
        else
        {
            _text.rectTransform.localPosition = _position1;
            _fillText = 1f;
        }

        //Border
        _border.rectTransform.localScale = Vector3.zero;
        _fillBorder = 0f;

        //Mask
        _maskFill.fillAmount = 0f;
        _fillMask = 0f;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_mainSwitch)
        {
            //to _position1;
            _isPointerEnter = true;
            SetUpAnimation();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_mainSwitch)
        {
            //to _position0;
            _isPointerEnter = false;
            _isPointerDown = false;
            SetUpAnimation();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_mainSwitch)
        {
            _isPointerDown = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_mainSwitch)
        {
            _isPointerDown = false;
        }
    }

    private void SetUpAnimation()
    {
        if (_animation == null)
        {
            _animation = Animation();
            StartCoroutine(_animation);
        }
    }

    private IEnumerator Animation()
    {
        _isInAnimation = true;
        _status = Status.SlideText;
        _text.font = _fontItalic;
        while (_isInAnimation)
        {
            switch (_status)
            {
                case Status.None:
                    _isInAnimation = false;
                    break;
                case Status.SlideText:
                    StatusFill(ref _fillText, _timeFillText, _isPointerEnter, StatusSlideTextMax, StatusSlideTextMin, StatusSlideTextStay);
                    break;
                case Status.AlphaImage:
                    StatusFill(ref _fillBorder, _timeFillBorder, _isPointerEnter, StatusAlphaImageMax, StatusAlphaImageMin, StatusAlphaImageStay);
                    break;
                case Status.FillMask:
                    if (_isPointerDown || !_isPointerDown && _fillMask > 0f)
                    {
                        StatusFill(ref _fillMask, _timeFillMask, (_isPointerEnter && _isPointerDown), StatusFillMaskMax, StatusFillMaskMin, StatusFillMaskStay);
                    }
                    else if (!_isPointerEnter)
                    {
                        _status -= 1;
                    }
                    break;
                case Status.Execute:
                    Debug.Log($"Eseguo {gameObject.name}", gameObject);
                    _isInAnimation = false;
                    SetUp();
                    onClickComplete.Invoke();
                    break;
            }

            yield return null;
        }

        _animation = null;
    }

    private void StatusFill(ref float fill, float timeFill, bool direction, Action max, Action min, Action stay)
    {
        fill += Time.deltaTime / timeFill * (direction ? 1 : -1);

        if (_isPointerEnter && fill > 1f)
        {
            fill = 1f;
            _status += 1;
            max.Invoke();
        }
        else if (!_isPointerEnter && fill < 0f)
        {
            fill = 0f;
            _status -= 1;
            min.Invoke();
        }
        else
        {
            stay.Invoke();
        }
    }

    private void StatusSlideTextMax() => _text.rectTransform.localPosition = _position1;
    private void StatusSlideTextMin()
    {
        _text.rectTransform.localPosition = _position0;
        _text.font = _fontRegular;
    }
    private void StatusSlideTextStay() => _text.rectTransform.localPosition = Vector3.Lerp(_position0, _position1, _fillText);

    private void StatusAlphaImageMax() => _border.rectTransform.localScale = Vector3.one;
    private void StatusAlphaImageMin() => _border.rectTransform.localScale = Vector3.zero;
    private void StatusAlphaImageStay() => _border.rectTransform.localScale = Vector3.one * _fillBorder;

    private void StatusFillMaskMax() => _maskFill.fillAmount = 1f;
    private void StatusFillMaskMin() => _maskFill.fillAmount = 0f;
    private void StatusFillMaskStay() => _maskFill.fillAmount = _fillMask;

    public bool AnimationFinish() => _animation == null;
}
