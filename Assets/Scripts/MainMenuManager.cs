using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{

    [SerializeField] private UIEventHandler eventHandler;
    [SerializeField] private DatabaseManager databaseManager; 
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private Text usernameText;
    [SerializeField] private GameObject loginMenu;
    [SerializeField] private Image[] registerFeedbackRects = new Image[4];
    [SerializeField] private InputField[] registerFields = new InputField[3];
    private Text[] registerFeedbackTexts = new Text[4];
    [SerializeField] private Image[] loginFeedbackRects = new Image[3];
    [SerializeField] private InputField[] loginFields = new InputField[2];
    private Text[] loginFeedbackTexts = new Text[3];


    // Start is called before the first frame update
    void Start()
    {
        //eventHandler.OpenMenu(loginMenu);
        if (PlaySession.userID > 0)
        {
            usernameText.text = PlaySession.username;
            eventHandler.OpenMenu(mainMenu);
        }
        for (int i = 0; i < registerFeedbackTexts.Length; i++)
        {
            registerFeedbackTexts[i] = registerFeedbackRects[i].GetComponentInChildren<Text>();
        }
        for (int i = 0; i < loginFeedbackTexts.Length; i++)
        {
           loginFeedbackTexts[i] = loginFeedbackRects[i].GetComponentInChildren<Text>();
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Register()
    {
        bool ready = true;

        Regex rgx = new Regex("[^A-Za-z0-9]");

        for (int i = 0; i < 3; i++)
        {
            if (registerFields[i].text.Equals("") || registerFields[i].text == null)
            {
                ready = false;
                registerFeedbackRects[i].gameObject.SetActive(true);
                registerFeedbackRects[i].color = Color.red;
                registerFeedbackTexts[i].text = "Ezt a mezõt kötelezõ kitölteni";

            }
            else if (rgx.IsMatch(registerFields[i].text))
            {
                ready = false;
                registerFeedbackRects[i].gameObject.SetActive(true);
                registerFeedbackRects[i].color = Color.red;
                registerFeedbackTexts[i].text = "Nem tartalmazhat speciális karaktert";
                registerFeedbackRects[3].gameObject.SetActive(false);
            }
            else
            {
                registerFeedbackRects[i].gameObject.SetActive(false);
                switch (i)
                {
                    case 0:
                        if (registerFields[i].text.Length < 8)
                        {
                            ready = false;
                            registerFeedbackRects[i].gameObject.SetActive(true);
                            registerFeedbackRects[i].color = Color.red;
                            registerFeedbackTexts[i].text = "A felhasználónévnek legalább 8 karaker hosszúnak kell lennie";
                        }
                        break;
                    case 1:
                        if (registerFields[i].text.Length < 8)
                        {
                            ready = false;
                            registerFeedbackRects[i].gameObject.SetActive(true);
                            registerFeedbackRects[i].color = Color.red;
                            registerFeedbackTexts[i].text = "A jelszónak legalább 8 karaker hosszúnak kell lennie";
                        }
                        break;
                    case 2:
                        if (registerFields[1].text != registerFields[2].text)
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
            
            Hash128 passHash = new Hash128();
            passHash.Append(registerFields[0].text);
            passHash.Append(registerFields[1].text);
            
            if (databaseManager.RegisterUser(registerFields[0].text, passHash.ToString()))
            {
                registerFeedbackRects[3].gameObject.SetActive(true);
                registerFeedbackRects[3].color = Color.green;
                registerFeedbackTexts[3].text = "Sikeres regisztráció";
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

    public void Login()
    {

        bool ready = true;

        Regex rgx = new Regex("[^A-Za-z0-9]");

        for (int i = 0; i < 2; i++)
        {
            if (loginFields[i].text.Equals(""))
            {
                ready = false;
                loginFeedbackRects[i].gameObject.SetActive(true);
                loginFeedbackRects[i].color = Color.red;
                loginFeedbackTexts[i].text = "Ezt a mezõt kötelezõ kitölteni";
            }
            else if (rgx.IsMatch(loginFields[i].text))
            {
                ready = false;
                loginFeedbackRects[i].gameObject.SetActive(true);
                loginFeedbackRects[i].color = Color.red;
                loginFeedbackTexts[i].text = "Nem tartalmazhat speciális karaktert";
                loginFeedbackRects[2].gameObject.SetActive(false);
            }
            else
            {
                loginFeedbackRects[i].gameObject.SetActive(false);
            }
        }

        Hash128 passHash = new Hash128();
        passHash.Append(loginFields[0].text);
        passHash.Append(loginFields[1].text);

        if (ready)
        {
            loginFeedbackRects[2].gameObject.SetActive(false);
            if (databaseManager.Login(loginFields[0].text, passHash.ToString(), out int userID, out string uname))
            {
                PlaySession.userID = userID;
                PlaySession.username = uname;
                //PlaySession.currentSave = uname + ".test";
                usernameText.text = "Bejelentkezve " + PlaySession.username + " néven.";
                for (int i = 0; i < loginFields.Length; i++)
                {
                    loginFields[i].text = "";
                }
                eventHandler.OpenMenu(mainMenu);
            }
            else
            {
                loginFeedbackRects[2].gameObject.SetActive(true);
                loginFeedbackRects[2].color = Color.red;
                loginFeedbackTexts[2].text = "Helytelen felhasználónév vagy jelszó";
            }
            
        }

    }

    public void Logout()
    {
        PlaySession.userID = new int();
        PlaySession.username = null;
        PlaySession.saveFileName = null;
        eventHandler.CloseMenu();
    }

}
