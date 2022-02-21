using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIEventHandler : MonoBehaviour
{

    public static bool isPaused = false;
    [SerializeField] private DatabaseManager databaseManager;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject quizMenu;
    [SerializeField] private QuizHandler quizHandler;
    [SerializeField] private GameObject saveLoadMenu;
    [SerializeField] public RecordAdder quizRecordAdder { get; private set; }
    private PlayerInputAsset controls;
    private Stack<GameObject> uiStack;
    private GameObject currentUI;
    private DroneController drone;
    public readonly static string loginRegex = "[^A-Z¡…Õ”÷’⁄‹€a-z·ÈÌÛˆı˙¸˚0-9_]";
    public readonly static string textRegex = "[^A-Z¡…Õ”÷’⁄‹€a-z·ÈÌÛˆı˙¸˚0-9 _!?.]";


    private void Awake()
    {
        uiStack = new Stack<GameObject>();
        currentUI = null;
        controls = new PlayerInputAsset();
        if (!SceneManager.GetActiveScene().name.Equals("MainMenu"))
        {
            controls.UI.Pause.performed += Escape;
        }
        isPaused = false;
    }

    public void Escape(InputAction.CallbackContext obj)
    {

        CloseMenu();
        
  
    }
    public void CloseMenu()
    {
        if (currentUI == null || currentUI == pauseMenu)
        {
            Pause();
            return;
        }
        else
        {
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
            currentUI.SetActive(false);


        }


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

    public void Pause()
    {
        if (pauseMenu == null)
        {
            return;
        }

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

    public void SwitchScene(string scene)
    {
        uiStack = new Stack<GameObject>();
        currentUI = null;
        Time.timeScale = 1f;
        SceneManager.LoadScene(scene);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void ExitToMainMenu()
    {
        PlaySession.saveInfo.fileName = null;
        PlaySession.saveInfo.levelName = null;
        SwitchScene("MainMenu");
    }

    public void OpenSaveLoadMenu(int listType)
    {
        DynamicListManager listManager = saveLoadMenu.transform.GetComponentInChildren<DynamicListManager>();
        OpenMenu(saveLoadMenu);
        listManager.ReloadList((DynamicListManager.DynamicListType)listType);
        
    }

    public void OpenMenu(GameObject menu)
    {
        
        if (currentUI != null)
        {
            
            currentUI.SetActive(false);
            uiStack.Push(currentUI);
        }
        
        currentUI = menu;
        currentUI.SetActive(true);
    }

    public void StartQuiz(DroneController drone)
    {
        
        this.drone = drone;
        if (quizMenu == null)
        {
            Debug.LogWarning("No quiz menu specified!");
            drone.FinishQuiz(false);

        }
        //Debug.Log("Quiz started");
        isPaused = true;
        quizHandler.StartQuiz();
        OpenMenu(quizMenu);
    }
    
    public void FinishQuiz(bool result)
    {
        
        drone.FinishQuiz(result);
        //currentUI.SetActive(false);
        //currentUI = null;
        //isPaused = false;
    }

    public void LoadFromSave()
    {
        // TODO
        SwitchScene("SampleScene");
    }

    public void StartNewGame()
    {
        PlaySession.saveInfo = new SaveGameInfo("SampleScene");
        SwitchScene("SampleScene");
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

   
}
