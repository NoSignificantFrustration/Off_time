using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Quiz record adder logic.
/// </summary>
/// <seealso cref="DatabaseManager"/>
/// <seealso cref="UIEventHandler"/>
/// <seealso cref="QuizHandler.QuizData"/>
public class RecordAdder : MonoBehaviour
{
    /// <summary>Database manager</summary>
    [SerializeField] private DatabaseManager databaseManager;
    /// <summary>U event handler</summary>
    [SerializeField] private UIEventHandler uIEventHandler;
    /// <summary>Difficulty input</summary>
    [SerializeField] private Dropdown difficultyInput;
    /// <summary>Input fields</summary>
    [SerializeField] private InputField[] quesionInputs = new InputField[5];
    /// <summary>Confirm button</summary>
    [SerializeField] private Button confirmButton;
    /// <summary>Back button</summary>
    [SerializeField] private Button backButton;
    /// <summary>Feedback panel</summary>
    [SerializeField] private GameObject feedbackPanel;
    /// <summary>Feedback panel text fields</summary>
    [SerializeField] private Text[] feedbackTexts = new Text[2];
    /// <summary>Feedback panel button</summary>
    [SerializeField] private Button feedbackPanelButton;
    /// <summary>The QuizData we are working with</summary>
    private QuizHandler.QuizData quizData;

    /// <summary>
    /// Reinitialises the UI elements and fills the inputs with the provided QuizHandler.QuizData.
    /// </summary>
    /// <param name="data">Provided QuizHandler.QuizData</param>
    public void Refresh(QuizHandler.QuizData data)
    {
        feedbackPanel.SetActive(false);

        //Store the data
        quizData = data;

        //Fill in the input fields
        difficultyInput.value = data.difficulty;
        quesionInputs[0].text = data.question;
        quesionInputs[1].text = data.good_answer;
        for (int i = 0; i < 3; i++)
        {
            quesionInputs[i + 2].text = data.bad_answers[i];
        }

        //Configure the buttons
        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(delegate {
            uIEventHandler.GetSaveLoadMenu().GetComponentInChildren<DynamicListManager>().LoadButtons();
            Close(); 
        });

        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(delegate { UpdateQuiz(); });
        confirmButton.GetComponentInChildren<Text>().text = "Mentés";
    }

    /// <summary>
    /// Reinitialises the UI elements.
    /// </summary>
    public void Refresh()
    {
        feedbackPanel.SetActive(false);

        foreach (InputField item in quesionInputs)
        {
            item.text = "";

        }

        //Configure the buttons
        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(delegate {
            uIEventHandler.GetSaveLoadMenu().GetComponentInChildren<DynamicListManager>().LoadButtons();
            Close();
        });

        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(delegate { AddQuiz(); });
        confirmButton.GetComponentInChildren<Text>().text = "Hozzáadás";
    }


    /// <summary>
    /// Tries to make a QuizHandler.QuizData with the contents of the inputs.
    /// </summary>
    /// <param name="result">The resulting QuizHandler.QuizData</param>
    /// <returns>True if the operation succeeded, false if it didn't.</returns>
    private bool TryMakeQuizData(out QuizHandler.QuizData result)
    {

        Regex rgx = new Regex(UIEventHandler.textRegex);
        result = new QuizHandler.QuizData();
        result.id = 0;

        //Check if any of the inputs are empty or contain illegal characters
        for (int i = 0; i < quesionInputs.Length; i++)
        {


            if (quesionInputs[i].text.Equals(""))
            {
                feedbackTexts[0].text = "Hiba";
                feedbackTexts[1].text = "Az összes mezõt ki kell tölteni!";
                feedbackPanelButton.GetComponentInChildren<Text>().text = "Ok";
                feedbackPanelButton.onClick.RemoveAllListeners();
                feedbackPanelButton.onClick.AddListener(delegate { feedbackPanel.SetActive(false); });
                feedbackPanel.SetActive(true);

                return false;
            }
            else if (rgx.IsMatch(quesionInputs[i].text))
            {
                feedbackTexts[0].text = "Hiba";
                feedbackTexts[1].text = "Egy vagy több mezõ nem megengedett karaktert tartalmaz.\nMegengedett karakterek: A-Z 0-9 _!?.()-";
                feedbackPanelButton.GetComponentInChildren<Text>().text = "Ok";
                feedbackPanelButton.onClick.RemoveAllListeners();
                feedbackPanelButton.onClick.AddListener(delegate { feedbackPanel.SetActive(false); });
                feedbackPanel.SetActive(true);

                return false;
            }
        }

        //Store the contents
        result.difficulty = difficultyInput.value;
        result.question = quesionInputs[0].text;
        result.good_answer = quesionInputs[1].text;
        result.bad_answers = new string[3];
        for (int i = 0; i < 3; i++)
        {
            result.bad_answers[i] = quesionInputs[i + 2].text;
        }

        return true;
    }

    /// <summary>
    /// Updates the provided quiz.
    /// </summary>
    public void UpdateQuiz()
    {

        if (TryMakeQuizData(out QuizHandler.QuizData data))
        {
            data.id = quizData.id;

            feedbackTexts[0].text = "Sikeres mentés";
            feedbackTexts[1].text = "Kvíz sikeresen módosítva.";
            feedbackPanelButton.GetComponentInChildren<Text>().text = "Ok";
            feedbackPanelButton.onClick.RemoveAllListeners();
            feedbackPanelButton.onClick.AddListener(delegate { 
                feedbackPanel.SetActive(false);
                uIEventHandler.GetSaveLoadMenu().GetComponentInChildren<DynamicListManager>().LoadButtons();
                Close();
            });
            feedbackPanel.SetActive(true);

            databaseManager.UpdateQuiz(data);
        }
    }

    
    /// <summary>
    /// Adds a new quiz.
    /// </summary>
    public void AddQuiz()
    {
        if (TryMakeQuizData(out QuizHandler.QuizData data))
        {
            databaseManager.AddQuiz(data);

            feedbackTexts[0].text = "Sikeres mentés";
            feedbackTexts[1].text = "Kvíz sikeresen hozzáadva.";
            feedbackPanelButton.GetComponentInChildren<Text>().text = "Ok";
            feedbackPanelButton.onClick.RemoveAllListeners();
            feedbackPanelButton.onClick.AddListener(delegate { feedbackPanel.SetActive(false); });
            feedbackPanel.SetActive(true);

            foreach (InputField field in quesionInputs)
            {
                field.text = "";

            }

        }
        
    }

    /// <summary>
    /// Closes the quiz record adder.
    /// </summary>
    public void Close()
    {
        uIEventHandler.CloseMenu();
    }
}
