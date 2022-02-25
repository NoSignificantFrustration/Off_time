using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

/// <summary>
/// Object used to connect connectables and visualise said connections.
/// </summary>
/// <seealso cref="IConnectable"/>
/// <seealso cref="PowerNode"/>
/// <seealso cref="IConnectable"/>
public class PowerConnection : MonoBehaviour, IConnectable
{


    /// <summary>State of the connection</summary>
    [SerializeField] private bool active;
    /// <summary>The PowerNode this connection gets input from</summary>
    [SerializeField] private PowerNode input;
    /// <summary>The GameObject this connection will try to output to</summary>
    [SerializeField] private GameObject output;
    /// <summary>Extra points for the line to be drawn along</summary>
    [SerializeField] private Transform[] joints;
    /// <summary>The IConnectable interface of the output</summary>
    [SerializeField] private IConnectable outputInterface;
    /// <summary>The LineRenderer component of the GameObject</summary>
    private LineRenderer lr;
    
   

    /// <summary>
    /// Gets it's LineRenderer component and the connection interface of the output object when the script is loaded.
    /// </summary>
    public void Awake()
    {
        lr = GetComponent<LineRenderer>();
        outputInterface = output.GetComponent<IConnectable>();

    }

    /// <summary>
    /// Calls SetupLines() when the game is started.
    /// </summary>
    // Start is called before the first frame update
    void Start()
    {
        SetUpLines();

    }

    /// <summary>
    /// Draws a line along the input, the GameObject, extra points and finally the output object, and changes it's collor based on the connection state.
    /// </summary>
    public void SetUpLines()
    {
        if (lr == null)
        {
            lr = GetComponent<LineRenderer>();

        }

        //Determime and set the color of the line
        if (active)
        {
            lr.startColor = new Color(0, 255, 0);
            lr.endColor = new Color(0, 255, 0);
        }
        else
        {
            lr.startColor = new Color(255, 0, 0);
            lr.endColor = new Color(255, 0, 0);
        }



        lr.positionCount = joints.Length + 3; //Input + own GameObject + extra joints + output
        lr.SetPosition(0, input.transform.position); //Input's position
        lr.SetPosition(1, gameObject.transform.position); //Own position
        for (int i = 0; i < joints.Length; i++) //Positions of extra joints
        {
            lr.SetPosition(i + 2, joints[i].position);
        }
        lr.SetPosition(joints.Length + 2, outputInterface.GetTransform().position); //Output's position
    }

    /// <summary>
    /// Toggles the state of the connection and calls the output to update too.
    /// </summary>
    /// <param name="state">New state</param>
    public void Toggle(bool state)
    {
        active = state;
        if (state)
        {
            lr.startColor = new Color(0, 255, 0);
            lr.endColor = new Color(0, 255, 0);
        }
        else
        {
            lr.startColor = new Color(255, 0, 0);
            lr.endColor = new Color(255, 0, 0);
        }
        outputInterface.Pulse();
        
    }

    public void Click(DroneController drone)
    {

    }

    public void Pulse()
    {
        Debug.Log(gameObject.name + " Needs to be toggled, not pulsed.");
    }

    /// <summary>
    /// Sends the transform of this GameObject.
    /// </summary>
    /// <returns>The transform of this GameObject.</returns>
    public Transform GetTransform()
    {
        return transform;
    }

    /// <summary>
    /// Gets the active state.
    /// </summary>
    /// <returns>Active state.</returns>
    public bool GetActive()
    {
        return active;
    }

    /// <summary>
    /// Gets the connection's input.
    /// </summary>
    /// <returns>Input PowerNode.</returns>
    public PowerNode GetInput()
    {
        return input;
    }

    /// <summary>
    /// Sets the input.
    /// </summary>
    /// <param name="newInput">New input PowerNode.</param>
    public void SetInput(PowerNode newInput)
    {
        input = newInput;
    }

    /// <summary>
    /// Gets the connection's output.
    /// </summary>
    /// <returns>Output GameObject.</returns>
    public GameObject GetOutput()
    {
        return output;
    }

    /// <summary>
    /// Sets the output.
    /// </summary>
    /// <param name="newOutput">New output GameObject.</param>
    public void SetOutput(GameObject newOutput)
    {
        output = newOutput;
    }

    /// <summary>
    /// Sets the output connection interface.
    /// </summary>
    /// <param name="newIf">New output IConnectable.</param>
    public void SetConnectionInterface(IConnectable newIf)
    {
        outputInterface = newIf;
    }

    /// <summary>
    /// Sets the active state.
    /// </summary>
    /// <param name="state">New state</param>
    public void SetActive(bool state)
    {
        active = state;
    }
}
