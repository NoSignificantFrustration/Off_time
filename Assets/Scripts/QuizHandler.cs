using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuizHandler : MonoBehaviour
{
    [SerializeField]
    private Button[] answerButtons = new Button[4];
    [SerializeField]
    private Text questionText;
    [SerializeField]
    private DatabaseManager databaseManager;
    [SerializeField]
    private UIEventHandler uiEventHandler;
    private int rightAnswer;
    

    public void StartQuiz()
    {
        QuizData dats = databaseManager.GetQuestion();

        questionText.text = dats.question;

        List<int> numList = new List<int>() { 0, 1, 2, 3 };

        rightAnswer = Random.Range(0, 4);
        numList.Remove(rightAnswer);
        answerButtons[rightAnswer].GetComponentInChildren<Text>().text = dats.good_answer;
        for (int i = 0; i < 3; i++)
        {
            int buttonIndex = numList[Random.Range(0, numList.Count)];
            numList.Remove(buttonIndex);
            answerButtons[buttonIndex].GetComponentInChildren<Text>().text = dats.bad_answers[i];
            
        }

    }

    public void AnswerQuiz(Button button)
    {
        if (button == answerButtons[rightAnswer])
        {
            uiEventHandler.FinishQuiz(true);
        }
        else
        {
            uiEventHandler.FinishQuiz(false);
        }
    }

    public struct QuizData
    {
        public string question;
        public string good_answer;
        public string[] bad_answers;
    }
}
