using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DroneController : MonoBehaviour
{

    public Camera characterCamera;
    [SerializeField]
    public float speedMultiplier;
    private Vector2 movementInput;
    private PlayerInputAsset controls;
    private CharacterController controller;

    private void Awake()
    {
        controls = new PlayerInputAsset();
        controller = GetComponent<CharacterController>();
        controls.Player.Click.performed += Click;
    }

    private void Click(InputAction.CallbackContext ctx)
    {



        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
        if (hit)
        {
            IConnectable iFace;
            if (hit.transform.gameObject.TryGetComponent(out iFace))
            {
                iFace.Click();
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
        controller.Move(new Vector3(movementInput.x * speedMultiplier, movementInput.y * speedMultiplier, 0));
        
        characterCamera.transform.position = new Vector3(transform.position.x, transform.position.y, characterCamera.transform.position.z);
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
