using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Logic for the player controlled drone.
/// </summary>
/// <seealso cref="UIEventHandler"/>
/// <seealso cref="NodeBlocker"/>
/// <seealso cref="ISaveable"/>
public class DroneController : MonoBehaviour, ISaveable
{
    /// <summary>Movement speed multiplier</summary>
    [SerializeField] private float speedMultiplier;
    /// <summary>Main UIEventHandler</summary>
    [SerializeField] private UIEventHandler uiEventHandler;
    /// <summary>Movement direction</summary>
    private Vector2 movementInput;
    /// <summary>Input scheme</summary>
    private PlayerInputAsset controls;
    /// <summary>Rigidbody2D component</summary>
    private Rigidbody2D rb;
    /// <summary>SpriteRenderer component</summary>
    private SpriteRenderer sr;
    /// <summary>Quiz solving state</summary>
    public bool isSolvingPuzzle;
    /// <summary>Current NodeBlocker being solved</summary>
    private NodeBlocker blocker;

    /// <summary>
    /// Gets references to components and sets up listener for left mouse click when the script is loaded.
    /// </summary>
    private void Awake()
    {
        controls = new PlayerInputAsset();
        rb = GetComponent<Rigidbody2D>();
        
        sr = GetComponent<SpriteRenderer>();
        controls.Player.Click.performed += Click;
        
    }

    /// <summary>
    /// Handles the clicking on other objects
    /// </summary>
    /// <param name="ctx">Callback context</param>
    private void Click(InputAction.CallbackContext ctx)
    {
        //We don't want to click on anything else when already unlocking a node or when the game is paused
        if (isSolvingPuzzle || UIEventHandler.isPaused)
        {
            return;
        }

        //Click raycast check
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
        //If the raycast hit and the hit object can be clicked do exactly that
        if (hit)
        {
            IConnectable iFace;
            if (hit.transform.gameObject.TryGetComponent(out iFace))
            {
                iFace.Click(this);
            }
        }
    }

    public void Startup()
    {
        
    }

    /// <summary>
    /// Gets the current movement input.
    /// </summary>
    // Update is called once per frame
    void Update()
    {
        if (!UIEventHandler.isPaused)
        {
            movementInput = controls.Player.Movement.ReadValue<Vector2>();
            
        }
        

        
    }

    /// <summary>
    /// Sets the rigidbody's velocity and rotation 50 times per second.
    /// </summary>
    private void FixedUpdate()
    {
        //Calculate the desired velocity
        rb.velocity = movementInput * Time.fixedDeltaTime * speedMultiplier;
        if (movementInput.magnitude > 0)
        {
            //Smoothly rotate the rigidbody
            rb.MoveRotation(Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.forward, movementInput), Time.fixedDeltaTime * 10));

        }
    }


    /// <summary>
    /// Pauses time and calls on the UIEventHandler to start a quiz.
    /// </summary>
    /// <param name="nodeBlocker">The NodeBlocker that was clicked</param>
    public void StartQuiz(NodeBlocker nodeBlocker)
    {
        if (uiEventHandler == null)
        {
            Debug.LogWarning("No quizhandler found");
            return;
        }
        Time.timeScale = 0f;
        isSolvingPuzzle = true;
        blocker = nodeBlocker;

        uiEventHandler.StartQuiz(this);


    }

    /// <summary>
    /// Unlocks the current NodeBlocker if the quiz was successful.
    /// </summary>
    /// <param name="result">The outcome of the quiz</param>
    public void FinishQuiz(bool result)
    {
        if (result)
        {
            blocker.Unlock();
        }
        blocker = null;
    }

    /// <summary>
    /// Enables controls when the script is enabled.
    /// </summary>
    private void OnEnable()
    {
        controls.Enable();
    }

    /// <summary>
    /// Disables controls when the script is Disabled.
    /// </summary>
    private void OnDisable()
    {
        controls.Disable();
    }

    /// <summary>
    /// Saves the drone's information to the specified SaveData.
    /// </summary>
    /// <param name="saveData">The SaveData the drone's DroneData should be added to</param>
    /// <seealso cref="SaveData"/>
    /// <seealso cref="SaveData.DroneData"/>
    public void AddToSave(SaveData saveData)
    {
        SaveData.DroneData data = new SaveData.DroneData();
        data.position = new float[] { transform.position.x, transform.position.y, transform.position.z};
        saveData.droneData = data;
    }

    /// <summary>
    /// Loads the drone's information from the provided SaveData.
    /// </summary>
    /// <param name="saveData">The provided SaveData</param>
    public void LoadFromSave(SaveData saveData)
    {
        transform.position = new Vector3(saveData.droneData.position[0], saveData.droneData.position[1], saveData.droneData.position[2]);
    }

    public UnityEngine.Object GetObject(bool force = false)
    {
        return null;
    }
}
