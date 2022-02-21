using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class RecordAdder : MonoBehaviour
{

    [SerializeField] private DatabaseManager databaseManager;
    [SerializeField] private Dropdown difficultyInput;
    [SerializeField] private InputField[] quesionInputs = new InputField[5];
    [SerializeField] private Button confirmButton;
    private QuizHandler.QuizData quizData;

    public void Refresh(QuizHandler.QuizData data)
    {
        quizData = data;

        difficultyInput.value = data.difficulty;
        quesionInputs[0].text = data.question;
        quesionInputs[1].text = data.good_answer;
        for (int i = 0; i < 3; i++)
        {
            quesionInputs[i + 2].text = data.bad_answers[i];
        }

        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(delegate { UpdateQuiz(); });
        confirmButton.GetComponentInChildren<Text>().text = "Mentés";
    }

    public void Refresh()
    {
        foreach (InputField item in quesionInputs)
        {
            item.text = "";

        }
        
        

        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(delegate { AddQuiz(); });
        confirmButton.GetComponentInChildren<Text>().text = "Hozzáadás";
    }

    public void UpdateQuiz()
    {

    }

    private bool TryMakeQuizData(out QuizHandler.QuizData result)
    {

        Regex rgx = new Regex(UIEventHandler.textRegex);
        result = new QuizHandler.QuizData();
        result.id = 0;
        result.difficulty = difficultyInput.value;

        for (int i = 0; i < quesionInputs.Length; i++)
        {


            if (quesionInputs[i].text.Equals(""))
            {
                return false;
            }
            else if (rgx.IsMatch(quesionInputs[i].text))
            {
                return false;
            }
        }

        return true;
    }

    public void AddQuiz()
    {
        QuizHandler.QuizData data = new QuizHandler.QuizData();

        data.difficulty = difficultyInput.value;
        data.question = quesionInputs[0].text;
        data.good_answer = quesionInputs[1].text;
        for (int i = 0; i < 3; i++)
        {
            data.bad_answers[i] = quesionInputs[i + 2].text;
        }



        foreach (InputField field in quesionInputs)
        {
            field.text = "";

        }


    }


}
