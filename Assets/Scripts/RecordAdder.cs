using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecordAdder : MonoBehaviour
{

    [SerializeField] private DatabaseManager databaseManager;
    [SerializeField] private Dropdown difficultyInput;
    [SerializeField] private InputField[] quesionInputs = new InputField[5];

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
