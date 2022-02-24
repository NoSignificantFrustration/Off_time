using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Component for the root canvas that contains the main menu's UI elements.
/// </summary>
/// <seealso cref="UIEventHandler"/>
/// <seealso cref="DatabaseManager"/>
public class MainMenuManager : MonoBehaviour
{
    /// <summary>UI event handler</summary>
    [SerializeField] private UIEventHandler eventHandler;
    /// <summary>Database manager</summary>
    [SerializeField] private DatabaseManager databaseManager;
    /// <summary>Root of the main menu</summary>
    [SerializeField] private GameObject mainMenu;
    /// <summary>Text that displays the name of the currently logged in user</summary>
    [SerializeField] private Text usernameText;
    /// <summary>Root of the login menu</summary>
    [SerializeField] private GameObject loginMenu;
    /// <summary>Feedback rectangles behind the feedback texts on the register menu</summary>
    [SerializeField] private Image[] registerFeedbackRects = new Image[4];
    /// <summary>Register menu input fields</summary>
    [SerializeField] private InputField[] registerFields = new InputField[3];
    /// <summary>Feedback texts under the register input fields</summary>
    private Text[] registerFeedbackTexts = new Text[4];
    /// <summary>Feedback rectangles behind the feedback texts on the login menu</summary>
    [SerializeField] private Image[] loginFeedbackRects = new Image[3];
    /// <summary>Login menu input fields</summary>
    [SerializeField] private InputField[] loginFields = new InputField[2];
    /// <summary>Feedback texts under the login input fields</summary>
    private Text[] loginFeedbackTexts = new Text[3];
    /// <summary>Level selection menu</summary>
    [SerializeField] private GameObject levelSelectMenu;
    /// <summary>Level selection buttons</summary>
    [SerializeField] private Button[] levelSelectButtons;


    /// <summary>
    /// Opens the login menu, or the main menu if a user is logged in and gets references to text fields when the scene loads.
    /// </summary>
    // Start is called before the first frame update
    void Start()
    {
        eventHandler.OpenMenuAsRoot(loginMenu);
        if (PlaySession.userID > 0) //Check if anyone is logged in
        {
            usernameText.text = PlaySession.username; //Display their name
            eventHandler.OpenMenuAsRoot(mainMenu);
        }

        //Get the text field references
        for (int i = 0; i < registerFeedbackTexts.Length; i++)
        {
            registerFeedbackTexts[i] = registerFeedbackRects[i].GetComponentInChildren<Text>();
        }
        for (int i = 0; i < loginFeedbackTexts.Length; i++)
        {
           loginFeedbackTexts[i] = loginFeedbackRects[i].GetComponentInChildren<Text>();
        }
        
    }

    /// <summary>
    /// Try to register a user.
    /// </summary>
    public void Register()
    {
        bool ready = true;

        Regex rgx = new Regex(UIEventHandler.loginRegex);

        //Loop through the register text fields
        for (int i = 0; i < 3; i++)
        {
            if (registerFields[i].text.Equals("") || registerFields[i].text == null) //Check if the field is empty
            {
                ready = false;
                registerFeedbackRects[i].gameObject.SetActive(true);
                registerFeedbackRects[i].color = Color.red;
                registerFeedbackTexts[i].text = "Ezt a mezõt kötelezõ kitölteni";

            }
            else if (rgx.IsMatch(registerFields[i].text)) //Check if the field contains illegal characters
            {
                ready = false;
                registerFeedbackRects[i].gameObject.SetActive(true);
                registerFeedbackRects[i].color = Color.red;
                registerFeedbackTexts[i].text = "Nem tartalmazhat speciális karaktert";
                registerFeedbackRects[3].gameObject.SetActive(false);
            }
            else
            {
                registerFeedbackRects[i].gameObject.SetActive(false); //Hide any error feedback messages
                switch (i)
                {
                    case 0:
                        if (registerFields[i].text.Length < 8) //Check the username field text length
                        {
                            ready = false;
                            registerFeedbackRects[i].gameObject.SetActive(true);
                            registerFeedbackRects[i].color = Color.red;
                            registerFeedbackTexts[i].text = "A felhasználónévnek legalább 8 karaker hosszúnak kell lennie";
                        }
                        break;
                    case 1:
                        if (registerFields[i].text.Length < 8) //Check the password field text length
                        {
                            ready = false;
                            registerFeedbackRects[i].gameObject.SetActive(true);
                            registerFeedbackRects[i].color = Color.red;
                            registerFeedbackTexts[i].text = "A jelszónak legalább 8 karaker hosszúnak kell lennie";
                        }
                        break;
                    case 2:
                        if (registerFields[1].text != registerFields[2].text) //Check if the two passwords match
                        {
                            ready = false;
                            registerFeedbackRects[i].gameObject.SetActive(true);
                            registerFeedbackRects[i].color = Color.red;
                            registerFeedbackTexts[i].text = "A két jelszó nem egyezik";
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        if (ready)
        {
            //Hash the password
            Hash128 passHash = new Hash128();
            passHash.Append(registerFields[0].text);
            passHash.Append(registerFields[1].text);
            
            //Try to register the new user
            if (databaseManager.RegisterUser(registerFields[0].text, passHash.ToString()))
            {
                registerFeedbackRects[3].gameObject.SetActive(true);
                registerFeedbackRects[3].color = Color.green;
                registerFeedbackTexts[3].text = "Sikeres regisztráció";

                //Clear register text fields
                for (int i = 0; i < registerFields.Length; i++)
                {
                    registerFields[i].text = "";
                }
            }
            else
            {
                registerFeedbackRects[0].gameObject.SetActive(true);
                registerFeedbackRects[0].color = Color.red;
                registerFeedbackTexts[0].text = "Ez a felhasználónév foglalt";
            }
            
        }
    }

    /// <summary>
    /// Tries to log in a user.
    /// </summary>
    public void Login()
    {

        bool ready = true;

        Regex rgx = new Regex(UIEventHandler.loginRegex);

        //Loop through the login text fields
        for (int i = 0; i < 2; i++)
        {
            if (loginFields[i].text.Equals("")) //Check if the field is empty
            {
                ready = false;
                loginFeedbackRects[i].gameObject.SetActive(true);
                loginFeedbackRects[i].color = Color.red;
                loginFeedbackTexts[i].text = "Ezt a mezõt kötelezõ kitölteni";
            }
            else if (rgx.IsMatch(loginFields[i].text)) //Check if the field contains illegal characters
            {
                ready = false;
                loginFeedbackRects[i].gameObject.SetActive(true);
                loginFeedbackRects[i].color = Color.red;
                loginFeedbackTexts[i].text = "Nem tartalmazhat speciális karaktert";
                loginFeedbackRects[2].gameObject.SetActive(false);
            }
            else
            {
                loginFeedbackRects[i].gameObject.SetActive(false); //Hide any error feedback messages
            }
        }

        //Hash the password
        Hash128 passHash = new Hash128();
        passHash.Append(loginFields[0].text);
        passHash.Append(loginFields[1].text);

        if (ready)
        {
            loginFeedbackRects[2].gameObject.SetActive(false);
            if (databaseManager.Login(loginFields[0].text, passHash.ToString(), out int userID, out string uname, out int currentLevel)) //Check if login credentials are correct
            {
                //Save the userID and name
                PlaySession.userID = userID;
                PlaySession.username = uname;

                PlaySession.currentLevel = currentLevel;

                //Display logged in message and clear login fields
                usernameText.text = "Bejelentkezve " + PlaySession.username + " néven.";
                for (int i = 0; i < loginFields.Length; i++)
                {
                    loginFields[i].text = "";
                }
                eventHandler.OpenMenuAsRoot(mainMenu);

                
            }
            else
            {
                loginFeedbackRects[2].gameObject.SetActive(true);
                loginFeedbackRects[2].color = Color.red;
                loginFeedbackTexts[2].text = "Helytelen felhasználónév vagy jelszó";
            }
            
        }

    }

    /// <summary>
    /// Logs out the current user.
    /// </summary>
    public void Logout()
    {
        PlaySession.userID = new int();
        PlaySession.username = null;
        PlaySession.saveInfo.fileName = null;
        eventHandler.CloseRoot();
    }

    public void OpenLevelSelect()
    {
        for (int i = 0; i < levelSelectButtons.Length; i++)
        {
            if (i + 1 <= PlaySession.currentLevel)
            {
                levelSelectButtons[i].interactable = true;
            }
            else
            {
                levelSelectButtons[i].interactable = false;
            }
        }
        eventHandler.OpenMenu(levelSelectMenu);
    }

}
