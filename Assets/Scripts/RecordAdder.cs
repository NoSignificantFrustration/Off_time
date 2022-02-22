using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class RecordAdder : MonoBehaviour
{

    [SerializeField] private DatabaseManager databaseManager;
    [SerializeField] private UIEventHandler uIEventHandler;
    [SerializeField] private Dropdown difficultyInput;
    [SerializeField] private InputField[] quesionInputs = new InputField[5];
    [SerializeField] private Button confirmButton;
    [SerializeField] private GameObject feedbackPanel;
    [SerializeField] private Text[] feedbackTexts = new Text[2];
    [SerializeField] private Button feedbackPanelButton;
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
        TryMakeQuizData(out QuizHandler.QuizData data);
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

    public void AddQuiz()
    {

        TryMakeQuizData(out QuizHandler.QuizData data);



        foreach (InputField field in quesionInputs)
        {
            field.text = "";

        }


    }

    public void Close()
    {
        uIEventHandler.CloseMenu();
    }
}
