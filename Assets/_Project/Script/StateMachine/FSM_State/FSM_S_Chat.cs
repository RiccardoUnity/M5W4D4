using GM;
using UnityEngine;
using GSM = GM.GameStaticManager;

//Simulazione di una Chat e molto alla buona, forse
public class FSM_S_Chat : FSM_BaseState
{
    public override string NameState { get => GSM.GetStateChat(); }

    [SerializeField] private UI_ChatEvent _UI_ChatEvent;
    private CharacterBrain _brain;
    private CharacterBrain _target;
    private bool IsTargetNull() => _target == null;

    private bool _isTargetHuman;   //Serve per distinguere il tipo di chat da avere con il sender
    private bool _hasControlOfUI_ChatEvent;
    [SerializeField] private float _maxTimeAnswer = 10f;
    private float _maxDistanceFromTarget = 1f;
    private float _maxDistanceFromTargetSqr;
    private float _distanceFromTargetSqr;
    private int _random;

    private FSM_S_Chat _targetChat;
    private bool _chatMatch;
    public bool IsChatMatch() => _chatMatch;
    private float _priorityEnemyChat;

    private bool _isWaitingForAnAnswer;
    private bool _isMyRoundToDoQuest;
    private int _questionLevel;
    private AnswerType _answerType = AnswerType.None;

    [SerializeField] private QuestionsLevel _questionsLevel0;
    [SerializeField] private QuestionsLevel _questionsLevel1;
    [SerializeField] private QuestionsLevel _questionsLevel2;
    [SerializeField] private QuestionsLevel _questionsLevelCode;

    [TextArea(1, 3)][SerializeField] private string _answerCodeIdCorrect;
    [TextArea(1, 3)][SerializeField] private string _answerCodeIdWrong;

    protected override void Awake()
    {
        _detectionWidth = 0.4f;
        base.Awake();
        _brain = GetComponentInParent<CharacterBrain>();

        if (_UI_ChatEvent == null)
        {
            Debug.LogError("Manca il riferimento al componente UI_ChatEvent", gameObject);
        }
        if (_questionsLevel0 == null)
        {
            Debug.LogError("Mancano le domande del livello zero", gameObject);
        }
        if (_questionsLevel1 == null)
        {
            Debug.LogError("Mancano le domande di primo livello", gameObject);
        }
        if (_questionsLevel2 == null)
        {
            Debug.LogError("Mancano le domande di secondo livello", gameObject);
        }
        if (_questionsLevelCode == null)
        {
            Debug.LogError("Mancano le domande sul codice identificativo", gameObject);
        }
        if (_answerCodeIdCorrect == null)
        {
            Debug.LogError("Manca la risposta corretta sul codice identificativo", gameObject);
        }
        if (_answerCodeIdWrong == null)
        {
            Debug.LogError("Manca la risposta sbagliata sul codice identificativo", gameObject);
        }

        _priorityEnemyChat = Random.value;
    }

    void Start()
    {
        _brain.SetMyStateChat(_fsmController, this);
        _maxDistanceFromTarget += (_fsmController.GetStateByName(GSM.GetStateAlert()) as FSM_S_Alert).StopFromTarget();
        _maxDistanceFromTargetSqr = _maxDistanceFromTarget * _maxDistanceFromTarget;

        _transitions = new FSM_Transition[5];

        //Transizione: se Player si allontana --> Chase
        _transitions[0] = new FSM_Transition(gameObject, NameState, 1, _fsmController.GetStateByName(GSM.GetStateChase()));
        _transitions[0].SetCondition(0, HumanEscape, Logic.Equal, true);

        //Transizione: se risposta codice negativa --> Chase
        _transitions[1] = new FSM_Transition(gameObject, NameState, 1, _fsmController.GetStateByName(GSM.GetStateChase()));
        _transitions[1].SetCondition(0, HumanAnswerWrong, Logic.Equal, true);

        //Transizione: se risposta codice positiva --> Idle
        _transitions[2] = new FSM_Transition(gameObject, NameState, 1, _fsmController.GetStateByName(GSM.GetStateIdle()));
        _transitions[2].SetCondition(0, HumanAnswerCorrect, Logic.Equal, true);

        //Transizione: se target == null --> Idle
        _transitions[3] = new FSM_Transition(gameObject, NameState, 1, _fsmController.GetStateByName(GSM.GetStateIdle()));
        _transitions[3].SetCondition(0, IsTargetNull, Logic.Equal, true);

        //Transizione: se UI_ChatEvent occupato --> Idle
        _transitions[4] = new FSM_Transition(gameObject, NameState, 1, _fsmController.GetStateByName(GSM.GetStateIdle()));
        _transitions[4].SetCondition(0, HumanHasUI_ChatEvent, Logic.Equal, true);
    }

    public override void StateEnter()
    {
        base.StateEnter();
        _target = _fsmController.GetSenseBrain().GetTarget();
        _targetChat = null;
        _isTargetHuman = _target.GetComponent<PlayerManager>() != null;
        _chatMatch = false;
        _isWaitingForAnAnswer = false;
        _isMyRoundToDoQuest = false;
        _questionLevel = 0;
        _answerType = AnswerType.None;

        if (_isTargetHuman)
        {
            _hasControlOfUI_ChatEvent = _UI_ChatEvent.Enter(gameObject, GetAnswer, _maxTimeAnswer);
        }
        else
        {
            //almeno 2 Enemy sono in State Chat, uno deve iniziare la "conversazione"
            _brain.StartTimer(_priorityEnemyChat * 2f);
        }
    }

    public override void StateUpdate(float time)
    {
        base.StateUpdate(time);
        if (_isTargetHuman)
        {
            _distanceFromTargetSqr = (_target.transform.position - transform.position).sqrMagnitude;
            if (HumanEscape())
            {
                _brain.AddCharacter(this, _target);
            }
            else
            {
                if (_isWaitingForAnAnswer)
                {
                    if (_brain.IsInternalTimerNull())
                    {
                        //Chase
                        _questionLevel = 4;
                        _answerType = AnswerType.Chase;
                        _brain.AddCharacter(this, _target);
                    }
                }
                else
                {
                    if (_isMyRoundToDoQuest)
                    {
                        _UI_ChatEvent.ResetTextAnswer();
                        switch (_questionLevel)
                        {
                            case 0:
                                _random = Random.Range(0, _questionsLevel0.GetQuestionsLenght());
                                _UI_ChatEvent.SetQuestion(_questionsLevel0.GetQuestion(_random), true, _questionsLevel0.GetAnswer(_random));
                                break;
                            case 1:
                                _random = Random.Range(0, _questionsLevel1.GetQuestionsLenght());
                                _UI_ChatEvent.SetQuestion(_questionsLevel1.GetQuestion(_random), true, _questionsLevel1.GetAnswer(_random));
                                break;
                            case 2:
                                _random = Random.Range(0, _questionsLevel2.GetQuestionsLenght());
                                _UI_ChatEvent.SetQuestion(_questionsLevel2.GetQuestion(_random), true, _questionsLevel2.GetAnswer(_random));
                                break;
                            case 3:
                                _random = Random.Range(0, _questionsLevelCode.GetQuestionsLenght());
                                _UI_ChatEvent.SetQuestion(_questionsLevelCode.GetQuestion(_random), true);
                                break;
                        }
                        _isWaitingForAnAnswer = true;
                        _isMyRoundToDoQuest = false;
                        _brain.StartTimer(_maxTimeAnswer);
                    }
                    else
                    {
                        if (_brain.IsInternalTimerNull())
                        {
                            _isMyRoundToDoQuest = true;
                        }
                    }
                }
            }
        }
        //Conversazione tra 2 Enemy
        else
        {
            //Timer di chi deve rispondere
            if (_brain.IsInternalTimerNull())
            {
                if (!_chatMatch)
                {
                    _targetChat = _target.GetFSMStateChat();
                    //Già in conversazione con qualcun'altro
                    if (_targetChat.IsChatMatch())
                    {
                        _target = null;
                    }
                    //Libero di fare un match
                    else
                    {
                        _targetChat.OverrideEnemyTarget(_brain);
                        _chatMatch = true;
                        _target.OverrideStopTimer();
                        _isMyRoundToDoQuest = true;
                    }
                }
                else
                {
                    //"Timer" di chi ha fatto la domanda
                    if (!_isWaitingForAnAnswer)
                    {
                        if (_answerType == AnswerType.None && _isMyRoundToDoQuest)
                        {   
                            switch (_questionLevel)
                            {
                                case 0:
                                    _targetChat.ReceiveQuestion(_brain, AnswerType.ID);
                                    break;
                                case 1:
                                    _targetChat.ReceiveQuestion(_brain, AnswerType.Player);
                                    break;
                                //Tutte le domande hanno ricevuto risposta
                                case 2:
                                    _targetChat.ReceiveQuestion(_brain, AnswerType.Return);
                                    _target = null;
                                    break;
                            }
                            _isWaitingForAnAnswer = true;
                        }
                        else
                        {
                            SendAnswer();
                        }
                    }
                }
            }
        }
    }

    public override void StateExit()
    {
        if (_hasControlOfUI_ChatEvent)
        {
            _UI_ChatEvent.Exit(gameObject);
        }
    }

    //UI_ChatEvent
    public void GetAnswer(string text)
    {
        if (_isWaitingForAnAnswer)
        {
            switch (_questionLevel)
            {
                case 0:
                    HaveAnAnswerOperations();
                    _UI_ChatEvent.SetQuestion(_questionsLevel0.GetAfterAswer(_random), false);
                    break;
                case 1:
                    HaveAnAnswerOperations();
                    _UI_ChatEvent.SetQuestion(_questionsLevel1.GetAfterAswer(_random), false);
                    break;
                case 2:
                    HaveAnAnswerOperations();
                    _UI_ChatEvent.SetQuestion(_questionsLevel2.GetAfterAswer(_random), false);
                    break;
                case 3:
                    if (text.Length > 2)
                    {
                        HaveAnAnswerOperations();
                        if (EnemySceneManager.Instance.IsEnemyIDValid(text))
                        {
                            //Return
                            _UI_ChatEvent.SetQuestion(_answerCodeIdCorrect, false);
                            _answerType = AnswerType.Return;
                            //Se il player scopre un id valido ha tutto il mio rispetto
                        }
                        else
                        {
                            //Chase
                            _UI_ChatEvent.SetQuestion(_answerCodeIdWrong, false);
                            _answerType = AnswerType.Chase;
                            _brain.AddCharacter(this, _target);
                        }
                    }
                    break;
            }
            
        }
    }

    private void HaveAnAnswerOperations()
    {
        _isWaitingForAnAnswer = false;
        _brain.OverrideStopTimer();
        _brain.StartTimer(_maxTimeAnswer);
        ++_questionLevel;
    }

    //Per le transizioni
    private bool HumanEscape() => _distanceFromTargetSqr > _maxDistanceFromTargetSqr;
    private bool HumanAnswerCorrect() => _answerType == AnswerType.Return && _questionLevel > 3;
    private bool HumanAnswerWrong() => _answerType == AnswerType.Chase && _questionLevel > 3;
    private bool HumanHasUI_ChatEvent() => _isTargetHuman && !_hasControlOfUI_ChatEvent;

    public void OverrideEnemyTarget(CharacterBrain newTarget)
    {
        _target = newTarget;
        _chatMatch = true;
        _targetChat = _target.GetFSMStateChat();
    }

    public void ReceiveQuestion(CharacterBrain sender, AnswerType answerType)
    {
        if (sender == _target)
        {
            if (answerType != AnswerType.Return)
            {
                _answerType = answerType;
                _brain.StartTimer(_priorityEnemyChat * _maxTimeAnswer);
            }
            else
            {
                _target = null;
            }
        }
    }

    public void SendAnswer()
    {
        switch (_answerType)
        {
            case AnswerType.ID:
                _targetChat.ReceiveAnswer(_answerType, _brain.GetID());
                break;
            case AnswerType.Player:
                _targetChat.ReceiveAnswer(_answerType, _brain.FindPlayerInMetCharacter());
                break;
        }
        _answerType = AnswerType.None;
        _isMyRoundToDoQuest = true;
    }

    public void ReceiveAnswer(AnswerType answerType, string id)
    {
        switch (answerType)
        {
            case AnswerType.ID:
                _brain.AddCharacter(this, EnemySceneManager.Instance.GetEnemyByID(id));
                _isWaitingForAnAnswer = false;
                ++_questionLevel;
                break;
        }
        _isMyRoundToDoQuest = false;
    }

    public void ReceiveAnswer(AnswerType answerType, CharacterBrain character)
    {
        switch (answerType)
        {
            case AnswerType.Player:
                _brain.AddCharacter(this, character);
                _isWaitingForAnAnswer = false;
                ++_questionLevel;
                break;
        }
        _isMyRoundToDoQuest = false;
    }
}