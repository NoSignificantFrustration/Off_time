using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIEventHandler : MonoBehaviour
{

    public static bool isPaused = false;
    public DatabaseManager databaseManager;
    public GameObject pauseMenu;
    public GameObject quizMenu;
    private PlayerInputAsset controls;
    private Stack<GameObject> uiStack;
    private GameObject currentUI;
    private DroneController drone;


    private void Awake()
    {
        uiStack = new Stack<GameObject>();
        currentUI = null;
        controls = new PlayerInputAsset();
        controls.UI.Pause.performed += Escape;
        isPaused = false;
    }

    public void Escape(InputAction.CallbackContext obj)
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
                drone.FinishQuiz(false);
                currentUI.SetActive(false);
                currentUI = null;
                uiStack.Clear();
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
        Debug.Log("Quiz started");
        OpenMenu(quizMenu);
    }

    public void FinishQuiz(bool result)
    {
        
        drone.FinishQuiz(result);
        currentUI.SetActive(false);
        currentUI = null;
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