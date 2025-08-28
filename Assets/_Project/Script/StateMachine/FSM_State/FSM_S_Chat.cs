using GM;
using UnityEngine;
using GSM = GM.GameStaticManager;

//Simulazione di una Chat, molto alla buona, forse
//Mentre 2 Enemy chattano, non percepiscono il Player
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
    private float _timeEnemyChat = 2f;

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
    }

    protected override void Start()
    {
        base.Start();
        _brain.SetMyStateChat(_fsmController, this);
        _maxDistanceFromTarget += (_fsmController.GetStateByName(GSM.GetStateAlert()) as FSM_S_Alert).StopFromTarget();
        _maxDistanceFromTargetSqr = _maxDistanceFromTarget * _maxDistanceFromTarget;

        _transitions = new FSM_Transition[6];

        //Transizione: se Player si allontana --> Chase
        _transitions[0] = new FSM_Transition(_fsmController, NameState, 2, _fsmController.GetStateByName(GSM.GetStateChase()));
        _transitions[0].SetCondition(0, IsTargetEscape, Logic.Equal, true);
        _transitions[0].SetCondition(1, IsTargetHuman, Logic.Equal, true);

        //Transizione: se risposta codice negativa --> Chase
        _transitions[1] = new FSM_Transition(_fsmController, NameState, 2, _fsmController.GetStateByName(GSM.GetStateChase()));
        _transitions[1].SetCondition(0, IsHumanAnswerWrong, Logic.Equal, true);
        _transitions[1].SetCondition(1, _brain.IsInternalTimerNull, Logic.Equal, true);

        //Transizione: se risposta codice positiva --> Idle
        _transitions[2] = new FSM_Transition(_fsmController, NameState, 2, _fsmController.GetStateByName(GSM.GetStateIdle()));
        _transitions[2].SetCondition(0, IsHumanAnswerCorrect, Logic.Equal, true);
        _transitions[2].SetCondition(1, _brain.IsInternalTimerNull, Logic.Equal, true);

        //Transizione: se target == null --> Idle
        _transitions[3] = new FSM_Transition(_fsmController, NameState, 1, _fsmController.GetStateByName(GSM.GetStateIdle()));
        _transitions[3].SetCondition(0, IsTargetNull, Logic.Equal, true);

        //Transizione: se target troppo distante --> Idle
        _transitions[4] = new FSM_Transition(_fsmController, NameState, 2, _fsmController.GetStateByName(GSM.GetStateIdle()));
        _transitions[4].SetCondition(0, IsTargetEscape, Logic.Equal, true);
        _transitions[4].SetCondition(1, IsTargetHuman, Logic.Equal, false);

        //Transizione: se UI_ChatEvent occupato --> Idle
        _transitions[5] = new FSM_Transition(_fsmController, NameState, 1, _fsmController.GetStateByName(GSM.GetStateIdle()));
        _transitions[5].SetCondition(0, IsHumanHasUI_ChatEvent, Logic.Equal, true);

        if (_fsmController.debug)
        {
            Debug.Log($"Transizioni create, state {NameState}", this);
        }
    }

    public override void StateEnter()
    {
        base.StateEnter();
        _target = _fsmController.GetSenseBrain().GetTarget();
        //Questa cosa non è il massimo, ma solo per consegnare in tempo
        //Sicuramente è un altro enemy, è solo che non si guardano e quindi EnemySenseBrain non ha un target
        if (_target == null)
        {
            RaycastHit hit;
            if (Physics.Linecast(transform.position + new Vector3(0f, 1f, 0f), _fsmController.GetSenseBrain().GetTargetV3(), out hit, (1 << 6), QueryTriggerInteraction.Ignore))
            {
                _target = hit.collider.GetComponent<CharacterBrain>();
                Debug.LogWarning("Override Target nello state Chat", gameObject);
            }
        }

        if (_target != null)
        {
            _targetChat = null;
            _isTargetHuman = _target.GetComponent<PlayerManager>() != null;
            _chatMatch = false;
            _isWaitingForAnAnswer = false;
            _isMyRoundToDoQuest = false;
            _questionLevel = 0;
            _answerType = AnswerType.None;

            if (!_isTargetHuman)
            {
                //almeno 1 Enemy è in State Chat, il più "veloce" deve iniziare la "conversazione"
                _targetChat = _target.GetFSMStateChat();
                //Già in conversazione con qualcun'altro
                if (_targetChat.IsChatMatch())
                {
                    _target = null;
                    if (_fsmController.debug)
                    {
                        Debug.Log($"Target già impegnato in una conversazione, {NameState}", this);
                    }
                }
                //Libero di fare un match
                else
                {
                    _noise.ChatEnemy();
                    _brain.StartTimer(_timeEnemyChat);
                    if (_fsmController.debug)
                    {
                        Debug.Log($"Chat con un altro Enemy, lo chiamo a 'voce', {NameState}", this);
                    }
                }

            }
        }
    }

    public override void StateUpdate(float time)
    {
        base.StateUpdate(time);

        if (_target != null )
        {
            _distanceFromTargetSqr = (_target.transform.position - transform.position).sqrMagnitude;
            if (_isTargetHuman)
            {
                if (_hasControlOfUI_ChatEvent)
                {
                    if (IsTargetEscape())
                    {
                        _brain.AddCharacter(this, _target);
                        if (_fsmController.debug)
                        {
                            Debug.Log($"Il Player è scappato, lo aggiungo alla lista delle mie conoscenze, {NameState}", this);
                        }
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
                                if (_fsmController.debug)
                                {
                                    Debug.Log($"Ho scoperto che sto conversando con il Player, lo aggiungo alla lista delle mie conoscenze, {NameState}", this);
                                }
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
                                if (_fsmController.debug)
                                {
                                    Debug.Log($"Pongo la domanda {_questionLevel}, {NameState}", this);
                                }
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
                else
                {
                    _hasControlOfUI_ChatEvent = _UI_ChatEvent.Enter(gameObject, GetAnswer, _maxTimeAnswer);
                    if (_fsmController.debug)
                    {
                        Debug.Log($"Chat con il Player, {NameState}", this);
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
                        //Target già impegnato in un match
                        if (_targetChat.IsChatMatch())
                        {
                            _target = null;
                            if (_fsmController.debug)
                            {
                                Debug.Log($"Il target si è messo a parlare con un altro, {NameState}", this);
                            }
                        }
                        //Libero di fare un match
                        else
                        {
                            _targetChat.OverrideChat(_brain, this);
                            _chatMatch = true;
                            _target.OverrideStopTimer();
                            _isMyRoundToDoQuest = true;
                            if (_fsmController.debug)
                            {
                                Debug.Log($"Posso parlare con un Enemy, inizio io con le domande, {NameState}", this);
                            }
                        }
                    }
                    else
                    {
                        //"Timer" di chi ha fatto la domanda
                        if (_isWaitingForAnAnswer)
                        {
                            if (_timeState > _maxTimeAnswer)
                            {
                                _target = null;
                            }
                        }
                        else
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
                //Continuo a chiamarlo, non è detto che mi abbia sentito
                else
                {
                    if (!_chatMatch)
                    {
                        _noise.ChatEnemy();
                    }
                    if (_timeState > _maxTimeAnswer)
                    {
                        _target = null;
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
    private bool IsTargetEscape() => _distanceFromTargetSqr > _maxDistanceFromTargetSqr;
    private bool IsTargetHuman() => _isTargetHuman;
    private bool IsHumanAnswerCorrect() => _answerType == AnswerType.Return && _questionLevel > 3;
    private bool IsHumanAnswerWrong() => _answerType == AnswerType.Chase && _questionLevel > 3;
    private bool IsHumanHasUI_ChatEvent() => _isTargetHuman && !_hasControlOfUI_ChatEvent;

    public void OverrideChat(CharacterBrain target, FSM_S_Chat chat)
    {
        _chatMatch = true;
        _target = target;
        _targetChat = chat;
        if (_fsmController.debug)
        {
            Debug.Log($"Io rispondo alle domande di {_target.gameObject.name}, {NameState}", this);
        }
    }
    
    public void ReceiveQuestion(CharacterBrain sender, AnswerType answerType)
    {
        if (sender == _target)
        {
            if (answerType != AnswerType.Return)
            {
                _answerType = answerType;
                _brain.StartTimer(_timeEnemyChat);
                if (_fsmController.debug)
                {
                    Debug.Log($"'Elaboro' la risposta, {NameState}", this);
                }
            }
            else
            {
                _target = null;
                if (_fsmController.debug)
                {
                    Debug.Log($"Fine conversazione, {NameState}", this);
                }
            }
        }
    }

    public void SendAnswer()
    {
        switch (_answerType)
        {
            case AnswerType.ID:
                if (_fsmController.debug)
                {
                    Debug.Log($"Comunico il mio ID, {NameState}", this);
                }
                _targetChat.ReceiveAnswer(_answerType, _brain.GetID());
                break;
            case AnswerType.Player:
                if (_fsmController.debug)
                {
                    Debug.Log($"Comunico se ho visto il Player, {NameState}", this);
                }
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
        if (_fsmController.debug)
        {
            Debug.Log($"Aggiungo un nuovo amico alla lista, {NameState}", this);
        }
    }

    public void ReceiveAnswer(AnswerType answerType, CharacterBrain character)
    {
        switch (answerType)
        {
            case AnswerType.Player:
                if (_brain.AddCharacter(this, character))
                {
                    if (_fsmController.debug)
                    {
                        Debug.Log($"Ora conosco il Player anch'io senza averlo mai incontrato, {NameState}", this);
                    }
                }
                else
                {
                    if (_fsmController.debug)
                    {
                        Debug.Log($"Non ha visto il Player, {NameState}", this);
                    }
                }
                _isWaitingForAnAnswer = false;
                ++_questionLevel;
                break;
        }
        _isMyRoundToDoQuest = false;
    }
}