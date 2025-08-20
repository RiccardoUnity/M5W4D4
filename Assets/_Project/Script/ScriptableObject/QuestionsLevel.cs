using GM;
using System;
using System.Globalization;
using UnityEngine;
using GSM = GM.GameStaticManager;

[CreateAssetMenu(menuName = "Game/QuestionsLevel")]
public class QuestionsLevel : ScriptableObject
{
    [Serializable]
    public class Question
    {
        [TextArea] [SerializeField] private string _question;
        public string GetQuestion() => _question;

        [TextArea] [SerializeField] private string[] _answers = new string[GSM._numberAnswers];
        public string[] GetAnswers() => _answers;
        public void ResizeAnswers() => Array.Resize(ref _answers, GSM._numberAnswers);

        [TextArea] [SerializeField] private string _afterAnswer;
        public string GetAfterAnswer() => _afterAnswer;
    }

    [SerializeField] private Question[] _questions;
    public int GetQuestionsLenght() => _questions.Length;
    public string GetQuestion(int index) => _questions[index].GetQuestion();
    public string[] GetAnswer(int index) => _questions[index].GetAnswers();
    public string GetAfterAswer(int index) => _questions[index].GetAfterAnswer();

    void OnValidate()
    {
        if (_questions != null)
        {
            foreach (Question question in _questions)
            {
                if (question.GetAnswers().Length != GSM._numberAnswers)
                {
                    question.ResizeAnswers();
                }
            }
        }
    }
}
