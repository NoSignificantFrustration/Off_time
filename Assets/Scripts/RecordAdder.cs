using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecordAdder : MonoBehaviour
{

    [SerializeField] private DatabaseManager databaseManager;
    [SerializeField] private InputField[] quesionInputs = new InputField[6];

    public void AddQuestion()
    {
        databaseManager.RunQuery("INSERT INTO questions (difficulty, question, good_answer, bad_answer1, bad_answer2, bad_answer3, user_added) VALUES (" + quesionInputs[0].textComponent.text + ", '" + quesionInputs[1].textComponent.text + "', '" + quesionInputs[2].textComponent.text + "', '" + quesionInputs[3].textComponent.text + "', '" + quesionInputs[4].textComponent.text + "', '" + quesionInputs[5].textComponent.text + ", 1"+ "')");
        foreach (InputField field in quesionInputs)
        {
            field.text = "";
            
        }

        
    }

    
}
