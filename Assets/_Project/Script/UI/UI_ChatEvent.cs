using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using GSM = GM.GameStaticManager;

public class UI_ChatEvent : MonoBehaviour
{
    [SerializeField] private Gradient _gradient;
    [SerializeField] private Image _timerBar;
    private Vector3 _scaleTimerBar = Vector3.one;
    private float _time;
    private float _timeInCoroutine;
    [SerializeField] private TMP_Text _question;
    [SerializeField] private TMP_Text _answer;
    [SerializeField] private TMP_Text[] _fourAnswers = new TMP_Text[GSM._numberAnswers];
    [SerializeField] private GameObject _fourAnswersParent;
    [SerializeField] private GameObject _numericKeypad;
    private string _text;
    private Action<string> _callback;
    private GameObject _gameObjectInUse;
    private bool _canAnswer;
    private string[] _answersText = new string[GSM._numberAnswers];

    private IEnumerator _timer;

    void OnValidate()
    {
        if (_fourAnswers.Length != GSM._numberAnswers)
        {
            Array.Resize(ref _fourAnswers, GSM._numberAnswers);
        }
    }

    public void Zero()
    {
        _text += "0";
        SetAnswer();
    }
    public void One()
    {
        _text += "1";
        SetAnswer();
    }
    public void Two()
    {
        _text += "2";
        SetAnswer();
    }
    public void Three()
    {
        _text += "3";
        SetAnswer();
    }
    public void Four()
    {
        _text += "4";
        SetAnswer();
    }
    public void Five()
    {
        _text += "5";
        SetAnswer();
    }
    public void Six()
    {
        _text += "6";
        SetAnswer();
    }
    public void Seven()
    {
        _text += "7";
        SetAnswer();
    }
    public void Eight()
    {
        _text += "8";
        SetAnswer();
    }
    public void Nine()
    {
        _text += "9";
        SetAnswer();
    }
    public void Answer0()
    {
        _text += _answersText[0];
        SetAnswer();
    }
    public void Answer1()
    {
        _text += _answersText[1];
        SetAnswer();
    }
    public void Answer2()
    {
        _text += _answersText[2];
        SetAnswer();
    }
    public void Answer3()
    {
        _text += _answersText[3];
        SetAnswer();
    }

    void Awake()
    {
        if (_gameObjectInUse == null)
        {
            gameObject.SetActive(false);
        }
    }

    public bool Enter(GameObject gameObject, Action<string> callback, float time)
    {
        if (_gameObjectInUse == null)
        {
            _timer = null;
            _question.text = null;
            _answer.text = null;
            _timerBar.gameObject.SetActive(false);
            _gameObjectInUse = gameObject;
            _callback = callback;
            _time = time;
            this.gameObject.SetActive(true);
            return true;
        }
        Debug.Log($"{_gameObjectInUse.name} sta usando ChatEvent, non puoi usarlo", _gameObjectInUse);
        return false;
    }

    public void SetQuestion(string question, bool canAnswer)
    {
        _question.text = question;
        _canAnswer = canAnswer;
        if (_canAnswer)
        {
            _numericKeypad.SetActive(true);
        }
        else
        {
            _numericKeypad.SetActive(false);
            _fourAnswersParent.SetActive(false);
        }
        _timerBar.gameObject.SetActive(true);
        _timer = Timer();
        StartCoroutine(_timer);
    }

    public void SetQuestion(string question, bool canAnswer, string[] texts)
    {
        _question.text = question;
        _canAnswer = canAnswer;
        if (_canAnswer)
        {
            _fourAnswersParent.SetActive(true);
        }
        else
        {
            _numericKeypad.SetActive(false);
            _fourAnswersParent.SetActive(false);
        }
        for (int i = 0; i < GSM._numberAnswers; i++)
        {
            _answersText[i] = null;
            _fourAnswers[i].text = null;
            if (i < texts.Length)
            {
                _answersText[i] = texts[i];
                _fourAnswers[i].text = texts[i];
            }
        }
        _timerBar.gameObject.SetActive(true);
        _timer = Timer();
        StartCoroutine(_timer);
    }

    private IEnumerator Timer()
    {
        _timeInCoroutine = _time;
        while (_timeInCoroutine > 0f)
        {
            yield return null;
            _timeInCoroutine -= Time.deltaTime;
            _scaleTimerBar.x = _timeInCoroutine / _time;
            _timerBar.rectTransform.localScale = _scaleTimerBar;
            _timerBar.color = _gradient.Evaluate(_scaleTimerBar.x);
        }
        _timer = null;
    }

    private void SetAnswer()
    {
        if (_canAnswer)
        {
            _answer.text = _text;
            _callback.Invoke(_text);
        }
    }

    public void ResetTextAnswer()
    {
        _text = null;
        _answer.text = null;
        _timeInCoroutine = 0f;
    }

    public void Exit(GameObject gameObject)
    {
        if (gameObject == _gameObjectInUse)
        {
            _gameObjectInUse = null;
            _callback = null;
            _time = 0;
            if (_timer != null)
            {
                StopCoroutine(_timer);
                _timer = null;
            }
            _scaleTimerBar = Vector3.one;
            _timerBar.rectTransform.localScale = _scaleTimerBar;
            _timerBar.gameObject.SetActive(false);
            _text = null;
            this.gameObject.SetActive(false);
        }
    }
}
