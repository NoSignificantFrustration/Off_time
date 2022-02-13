using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DroneController : MonoBehaviour, ISaveable
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float speedMultiplier;
    [SerializeField] private UIEventHandler quizHandler;
    private Vector2 movementInput;
    private PlayerInputAsset controls;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private bool isSolvingPuzzle;
    private NodeBlocker blocker;

    private void Awake()
    {
        controls = new PlayerInputAsset();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
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

    public void Startup()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!UIEventHandler.isPaused)
        {
            movementInput = controls.Player.Movement.ReadValue<Vector2>();

            //Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            //rb.transform.up = (mouseWorld - (Vector2)transform.position).normalized;
            
            if (movementInput.magnitude > 0)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.forward, movementInput), Time.fixedDeltaTime * 10);
                //float zRot = Vector2.SignedAngle(Vector2.up, movementInput);
                //Vector3 targetRot = new Vector3(0, 0, Vector2.SignedAngle(Vector2.up, movementInput));
                //if (zRot < 0)
                //{
                //    targetRot = new Vector3(0, 0, Mathf.Abs(Vector2.SignedAngle(Vector2.up, movementInput)));
                //}
                //else
                //{
                //    targetRot = new Vector3(0, 0, Vector2.SignedAngle(Vector2.up, movementInput) + 180);
                //}

                //Debug.Log(Vector3.Distance(transform.eulerAngles, targetRot));
                ////rb.transform.up = (Vector3)movementInput;
                //if (Vector3.Distance(transform.eulerAngles, targetRot) > 0.01f)
                //{
                //    transform.eulerAngles = Vector3.Lerp(transform.rotation.eulerAngles, targetRot, Time.deltaTime * 10f);
                //}
                //else
                //{
                //    transform.eulerAngles = targetRot;
                //}
                //transform.rotation.eulerAngles = 0;



                //movementInput.x -= 0.0001f;
                //movementInput.y -= 0.0001f;

            }
            //cameraTransform.position = new Vector3(rb.transform.position.x, rb.position.y, -10);
        }
        

        
    }

    private void FixedUpdate()
    {

        rb.velocity = movementInput * Time.fixedDeltaTime * speedMultiplier;
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

    public void AddToSave(SaveData saveData)
    {
        SaveData.DroneData data = new SaveData.DroneData();
        data.position = new float[] { transform.position.x, transform.position.y, transform.position.z};
        saveData.droneData = data;
    }

    public void LoadFromSave(SaveData saveData)
    {
        transform.position = new Vector3(saveData.droneData.position[0], saveData.droneData.position[1], saveData.droneData.position[2]);
    }

    public UnityEngine.Object GetObject(bool force = false)
    {
        return null;
    }
}
