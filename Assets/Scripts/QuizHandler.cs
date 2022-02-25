using System.Collections.Generic;

/// <summary>
/// Quiz handdler logic.
/// </summary>
/// <seealso cref="DatabaseManager"/>
/// <seealso cref="UIEventHandler"/>
public class QuizHandler : MonoBehaviour
{
    /// <summary>Answer buttons</summary>
    [SerializeField] private Button[] answerButtons = new Button[4];
    /// <summary>Disabled color</summary>
    [SerializeField] private Color disabledColor;
    /// <summary>Question text field</summary>
    [SerializeField] private Text questionText;
    /// <summary>Database manager</summary>
    [SerializeField] private DatabaseManager databaseManager;
    /// <summary>UI event handler</summary>
    [SerializeField] private UIEventHandler uiEventHandler;
    /// <summary>Default ColorBlock</summary>
    [SerializeField] private ColorBlock cb;
    /// <summary>Index of right answer button</summary>
    private int rightAnswer;

    /// <summary>
    /// Gets a QuizData from the database and starts a quiz with it.
    /// </summary>
    /// <seealso cref="QuizData"/>
    public void StartQuiz()
    {
        //Reset cb
        cb = answerButtons[0].colors;
        cb.disabledColor = disabledColor;


        //Get a quiz from database
        QuizData dats = databaseManager.GetQuestion();

        questionText.text = dats.question;

        List<int> numList = new List<int>() { 0, 1, 2, 3 };

        //Determine which button will have the right answer and set it's text
        rightAnswer = Random.Range(0, 4);
        numList.Remove(rightAnswer);
        answerButtons[rightAnswer].GetComponentInChildren<Text>().text = dats.good_answer;
        answerButtons[rightAnswer].interactable = true;
        answerButtons[rightAnswer].colors = cb;

        //Set up the rest of the buttons
        for (int i = 0; i < 3; i++)
        {
            int buttonIndex = numList[Random.Range(0, numList.Count)];
            numList.Remove(buttonIndex);
            answerButtons[buttonIndex].GetComponentInChildren<Text>().text = dats.bad_answers[i];
            answerButtons[buttonIndex].interactable = true;
            answerButtons[buttonIndex].colors = cb;

        }

    }

    /// <summary>
    /// Determines if the pressed button was the right answer or not.
    /// </summary>
    /// <param name="button">Pressed button</param>
    public void AnswerQuiz(Button button)
    {
        //set answer buttons to not be interactable
        for (int i = 0; i < answerButtons.Length; i++)
        {

            answerButtons[i].interactable = false;
        }

        //Determine if the pressed button is the right one, set it's background color and signal uiEventHandler accordingly
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

    /// <summary>
    /// Struct that contains information about a quiz.
    /// </summary>
    public struct QuizData
    {
        /// <summary>ID in the database</summary>
        public int id;
        /// <summary>Difficulty</summary>
        public int difficulty;
        /// <summary>Question text</summary>
        public string question;
        /// <summary>Good answer</summary>
        public string good_answer;
        /// <summary>Bad answers</summary>
        public string[] bad_answers;
    }
}
