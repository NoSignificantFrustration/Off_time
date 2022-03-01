using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Class that handles UI events.
/// </summary>
/// <seealso cref="DatabaseManager"/>
/// <seealso cref="QuizHandler"/>
/// <seealso cref="RecordAdder"/>
/// <seealso cref="DroneController"/>
public class UIEventHandler : MonoBehaviour
{
    /// <summary>Pause state</summary>
    public static bool isPaused = false;
    /// <summary>Database manager</summary>
    [SerializeField] private DatabaseManager databaseManager;
    /// <summary>Root of pause menu</summary>
    [SerializeField] private GameObject pauseMenu;
    /// <summary>Root of quiz menu</summary>
    [SerializeField] private GameObject quizMenu;
    /// <summary>Reference to the quiz handler</summary>
    [SerializeField] private QuizHandler quizHandler;
    /// <summary>Root of the dynamic list</summary>
    [SerializeField] private GameObject saveLoadMenu;
    /// <summary>Reference to the quiz record adder</summary>
    [SerializeField] private RecordAdder quizRecordAdder;
    /// <summary>Reference to the win menu</summary>
    [SerializeField] private WinMenu winMenu;
    /// <summary>Root of the confirmation panel</summary>
    [SerializeField] private GameObject confirmPanel;
    /// <summary>Confirmation panel text fields</summary>
    [SerializeField] private Text[] confirmPanelTexts;
    /// <summary>Confirmation panel buttons</summary>
    [SerializeField] private Button[] confirmPanelButtons;
    /// <summary>Text that displays the elapsed time</summary>
    [SerializeField] private Text timeText;
    /// <summary>Input asset</summary>
    public PlayerInputAsset controls { get; private set; }
    /// <summary>Escape pressed event</summary>
    public UnityEvent escapePressedEvent { get; private set; }
    /// <summary>Stack that keeps track of the order menus were opened</summary>
    private Stack<GameObject> uiStack;
    /// <summary>Used to determine which menus should not be closed</summary>
    public int minStackDepth = 0;
    /// <summary>Currently open menu</summary>
    private GameObject currentUI;
    /// <summary>Reference to the player drone</summary>
    private DroneController drone;
    /// <summary>Login regex string</summary>
    public static string loginRegex = "[^A-ZÁÉÍÓÖÕÚÜÛa-záéíóöõúüû0-9_]";
    /// <summary>General regex string</summary>
    public static string textRegex = "[^A-ZÁÉÍÓÖÕÚÜÛa-záéíóöõúüû0-9 _!?.()-]";

    /// <summary>
    /// Gets needed references and subscribes CloseMenu to the escape button event.
    /// </summary>
    private void Awake()
    {
        uiStack = new Stack<GameObject>();
        currentUI = null;
        controls = new PlayerInputAsset();

        controls.UI.Pause.performed += Escape;
        escapePressedEvent = new UnityEvent();
        isPaused = false;
    }

    /// <summary>
    /// Increments the elapsed time counter every frame when the game is not paused.
    /// </summary>
    // Update is called once per frame
    void Update()
    {
        if (!isPaused || currentUI == quizMenu)
        {
            PlaySession.saveInfo.elapsedTime += Time.unscaledDeltaTime;
            timeText.text = TimeSpan.FromSeconds(PlaySession.saveInfo.elapsedTime).ToString("hh':'mm':'ss");
        }
    }

    /// <summary>
    /// This method is always subscribed to the escape button event, and tries to close the current menu.
    /// </summary>
    /// <param name="obj"></param>
    public void Escape(InputAction.CallbackContext obj)
    {
        //Debug.Log(minStackDepth + " " + uiStack.Count);
        CloseMenu();
        escapePressedEvent.Invoke();

    }

    /// <summary>
    /// Closes the current menu.
    /// </summary>
    public void CloseMenu()
    {

        confirmPanel.SetActive(false);

        //Check if the current menu should be closed
        if (uiStack.Count + 1 <= minStackDepth)
        {
            return;
        }
        //Pause and unpause
        if (currentUI == null || currentUI == pauseMenu)
        {
            Pause();
            return;
        }
        else
        {
            //If we are trying to exit out of the quiz menu then fail it
            if (currentUI == quizMenu)
            {
                //drone.FinishQuiz(false);
                drone.isSolvingPuzzle = false;
                currentUI.SetActive(false);
                currentUI = null;
                uiStack.Clear();
                isPaused = false;
                Time.timeScale = 1f;
                return;
            }

            //Close the currently open menu
            currentUI.SetActive(false);


        }

        //Open the next menu on the stack, or set it to null if there is nothing to open.
        if (uiStack.Count > 0)
        {
            currentUI = uiStack.Pop();
            currentUI.SetActive(true);
        }
        else
        {
            currentUI = null;
        }
    }

    /// <summary>
    /// Handles pausing.
    /// </summary>
    public void Pause()
    {
        //If there is no pause menu don't do anything
        if (pauseMenu == null)
        {
            return;
        }

        //Flip the paused state, open it or close it and set the time scale depending on the state.
        isPaused = !isPaused;
        if (isPaused)
        {
            currentUI = pauseMenu;
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {

            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    /// <summary>
    /// Switches to the specified Scene.
    /// </summary>
    /// <param name="scene">Scene's name</param>
    public void SwitchScene(string scene)
    {
        uiStack = new Stack<GameObject>();
        currentUI = null;
        Time.timeScale = 1f;
        SceneManager.LoadScene(scene);
    }

    /// <summary>
    /// Opens the confirmation panel ands sets the confirm button's click event to quit the game. 
    /// </summary>
    public void QuitButtonPressed()
    {
        //Set the texts
        confirmPanelTexts[0].text = "Kilépés";
        confirmPanelTexts[1].text = "Biztosan kilépsz a játékból?";
        if (!SceneManager.GetActiveScene().name.Equals("MainMenu"))
        {
            confirmPanelTexts[1].text += "\nAz összes mentetlen haladás el fog veszni.";
        }

        //Cancel button actions and text
        confirmPanelButtons[0].onClick.RemoveAllListeners();
        confirmPanelButtons[0].onClick.AddListener(delegate
        {
            confirmPanel.SetActive(false);
        });
        confirmPanelButtons[0].GetComponentInChildren<Text>().text = "Mégsem";

        //Confirm button actions and text
        confirmPanelButtons[1].GetComponentInChildren<Text>().text = "Ok";
        confirmPanelButtons[1].onClick.RemoveAllListeners();
        confirmPanelButtons[1].onClick.AddListener(delegate
        {
            confirmPanel.SetActive(false);
            Debug.Log("Quit");
            Application.Quit();
        });
        confirmPanel.SetActive(true);


    }

    /// <summary>
    /// Goes back to the main menu.
    /// </summary>
    /// <param name="showWarning">Should it show a warning first or not</param>
    public void ExitToMainMenu(bool showWarning)
    {

        if (showWarning) //Show a warning before switching
        {
            //Set up texts
            confirmPanelTexts[0].text = "Vissza a fûmenübe";
            confirmPanelTexts[1].text = "Biztosan visszalépsz a fõmenübe?";
            if (!SceneManager.GetActiveScene().name.Equals("MainMenu"))
            {
                confirmPanelTexts[1].text += "\nAz összes mentetlen haladás el fog veszni.";
            }

            //Set up cancel button
            confirmPanelButtons[0].onClick.RemoveAllListeners();
            confirmPanelButtons[0].onClick.AddListener(delegate
            {
                confirmPanel.SetActive(false);
            });
            confirmPanelButtons[0].GetComponentInChildren<Text>().text = "Mégsem";

            //Set up confirm button
            confirmPanelButtons[1].GetComponentInChildren<Text>().text = "Ok";
            confirmPanelButtons[1].onClick.RemoveAllListeners();
            confirmPanelButtons[1].onClick.AddListener(delegate
            {
                confirmPanel.SetActive(false);
                PlaySession.saveInfo.fileName = null;
                PlaySession.saveInfo.levelName = null;
                SwitchScene("MainMenu");
            });
            confirmPanel.SetActive(true);
        }
        else //Switch outright
        {
            PlaySession.saveInfo.fileName = null;
            PlaySession.saveInfo.levelName = null;
            SwitchScene("MainMenu");
        }


    }

    /// <summary>
    /// Opens the dynamic list.
    /// </summary>
    /// <param name="listType">List type</param>
    /// <seealso cref="DynamicListManager"/>
    public void OpenSaveLoadMenu(int listType)
    {
        DynamicListManager listManager = saveLoadMenu.transform.GetComponentInChildren<DynamicListManager>();
        OpenMenu(saveLoadMenu);
        listManager.ReloadList((DynamicListManager.DynamicListType)listType);

    }

    /// <summary>
    /// Opens the provided menu.
    /// </summary>
    /// <param name="menu">Menu to open</param>
    public void OpenMenu(GameObject menu)
    {

        if (currentUI != null)
        {
            //Close the current menu and save it on the stack
            currentUI.SetActive(false);
            uiStack.Push(currentUI);
        }

        //Open the new menu
        currentUI = menu;
        currentUI.SetActive(true);
    }

    /// <summary>
    /// Opens the provided menu and makes it so it cannot be closed by pressing escape.
    /// </summary>
    /// <param name="menu">Menu to open</param>
    public void OpenMenuAsRoot(GameObject menu)
    {
        if (currentUI != null)
        {
            //Close the current menu and save it on the stack
            currentUI.SetActive(false);
            uiStack.Push(currentUI);
        }

        //Open the new menu
        minStackDepth++;
        currentUI = menu;
        currentUI.SetActive(true);
    }

    /// <summary>
    /// Closes the current "root" menu.
    /// </summary>
    public void CloseRoot()
    {
        minStackDepth--;
        CloseMenu();
    }

    /// <summary>
    /// Starts a quiz.
    /// </summary>
    /// <param name="drone">Player drone</param>
    public void StartQuiz(DroneController drone)
    {

        this.drone = drone;
        if (quizMenu == null)
        {
            Debug.LogWarning("No quiz menu specified!");
            drone.FinishQuiz(false);

        }

        isPaused = true;
        quizHandler.StartQuiz();
        OpenMenu(quizMenu);
    }

    /// <summary>
    /// Finishes the quiz.
    /// </summary>
    /// <param name="result">Quiz result</param>
    public void FinishQuiz(bool result)
    {

        drone.FinishQuiz(result);

    }


    /// <summary>
    /// Opens the win menu.
    /// </summary>
    public void OpenWinMenu()
    {
        winMenu.UpdateTexts();
        OpenMenuAsRoot(winMenu.gameObject);
        timeText.gameObject.SetActive(false);
    }

    /// <summary>
    /// Enables the controls.
    /// </summary>
    private void OnEnable()
    {
        controls.Enable();
    }

    /// <summary>
    /// Disables the controls.
    /// </summary>
    private void OnDisable()
    {
        controls.Disable();
    }

    /// <summary>
    /// Gets the record adder.
    /// </summary>
    /// <returns>Reference to the record adder.</returns>
    public RecordAdder GetRecordAdder()
    {
        return quizRecordAdder;
    }

    /// <summary>
    /// Gets the dynamic list.
    /// </summary>
    /// <returns>Reference to the dynamic list.</returns>
    public GameObject GetSaveLoadMenu()
    {
        return saveLoadMenu;
    }

}
