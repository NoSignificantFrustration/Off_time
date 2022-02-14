using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuizHandler : MonoBehaviour
{
    [SerializeField]
    private Button[] answerButtons = new Button[4];
    [SerializeField] private Color disabledColor;
    [SerializeField]
    private Text questionText;
    [SerializeField]
    private DatabaseManager databaseManager;
    [SerializeField]
    private UIEventHandler uiEventHandler;
    [SerializeField] private ColorBlock cb;
    private int rightAnswer;

    private void Start()
    {
        
    }

    public void StartQuiz()
    {
        
            cb = answerButtons[0].colors;
            cb.disabledColor = disabledColor;
        
        

        QuizData dats = databaseManager.GetQuestion();

        questionText.text = dats.question;

        List<int> numList = new List<int>() { 0, 1, 2, 3 };

        rightAnswer = Random.Range(0, 4);
        numList.Remove(rightAnswer);
        answerButtons[rightAnswer].GetComponentInChildren<Text>().text = dats.good_answer;
        answerButtons[rightAnswer].interactable = true;
        answerButtons[rightAnswer].colors = cb;
        for (int i = 0; i < 3; i++)
        {
            int buttonIndex = numList[Random.Range(0, numList.Count)];
            numList.Remove(buttonIndex);
            answerButtons[buttonIndex].GetComponentInChildren<Text>().text = dats.bad_answers[i];
            answerButtons[buttonIndex].interactable = true;
            answerButtons[buttonIndex].colors = cb;

        }

    }

    public void AnswerQuiz(Button button)
    {
        for (int i = 0; i < answerButtons.Length; i++)
        {
            
            answerButtons[i].interactable = false;
        }

        ColorBlock chosenColors = cb;
        if (button == answerButtons[rightAnswer])
        {
            chosenColors.disabledColor = Color.blue;
            button.colors = chosenColors;
            uiEventHandler.FinishQuiz(true);
        }
        else
        {
            chosenColors.disabledColor = Color.red;
            button.colors = chosenColors;
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
