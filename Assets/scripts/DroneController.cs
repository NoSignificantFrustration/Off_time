using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DroneController : MonoBehaviour
{

    [SerializeField]
    public float speedMultiplier;
    public UIEventHandler quizHandler;
    private Vector2 movementInput;
    private PlayerInputAsset controls;
    private Rigidbody2D rb;
    private bool isSolvingPuzzle;
    private NodeBlocker blocker;

    private void Awake()
    {
        controls = new PlayerInputAsset();
        rb = GetComponent<Rigidbody2D>();
        controls.Player.Click.performed += Click;
    }

    private void Click(InputAction.CallbackContext ctx)
    {
        if (isSolvingPuzzle || UIEventHandler.isPaused)
        {
            return;
        }


        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
        if (hit)
        {
            IConnectable iFace;
            if (hit.transform.gameObject.TryGetComponent(out iFace))
            {
                iFace.Click(this);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        movementInput = controls.Player.Movement.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {

        rb.velocity = movementInput * speedMultiplier;
    }

    public void StartQuiz(NodeBlocker nodeBlocker)
    {
        if (quizHandler == null)
        {
            Debug.LogWarning("No quizhandler found");
            return;
        }
        Time.timeScale = 0f;
        isSolvingPuzzle = true;
        blocker = nodeBlocker;

        quizHandler.StartQuiz(this);


    }

    public void FinishQuiz(bool result)
    {
        if (result)
        {
            blocker.Unlock();
        }
        isSolvingPuzzle = false;
        blocker = null;
        Time.timeScale = 1f;
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
