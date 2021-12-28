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
        string[] dats = databaseManager.GetQuestion();

        questionText.text = dats[0];

        List<int> numList = new List<int>() { 0, 1, 2, 3 };

        rightAnswer = Random.Range(0, 4);
        numList.Remove(rightAnswer);
        answerButtons[rightAnswer].GetComponentInChildren<Text>().text = dats[1];
        for (int i = 2; i <= 4; i++)
        {
            int buttonIndex = numList[Random.Range(0, numList.Count)];
            numList.Remove(buttonIndex);
            answerButtons[buttonIndex].GetComponentInChildren<Text>().text = dats[i];
            
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
}
