using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

/// <summary>
/// Object used to connect visualise connection between connectables.
/// </summary>
/// <seealso cref="IConnectable"/>
/// <param name = "active">State of the connection.</param>
/// <param name = "input">The PowerNode this connection gets input from.</param>
/// <seealso cref="PowerNode"/>
/// <param name = "output">The GameObject this connection will try to output to.</param>
/// <param name = "joints">Extra points for the line to be drawn along.</param>
/// <param name = "lr">The LineRenderer component of the GameObject.</param>
/// <param name = "outputInterface">The IConnectable interface of the output.</param>
/// <seealso cref="IConnectable"/>
public class PowerConnection : MonoBehaviour, IConnectable
{

    

    [SerializeField] private bool active;
    [SerializeField] private PowerNode input;
    [SerializeField] private GameObject output;
    [SerializeField] private Transform[] joints;
    private LineRenderer lr;
    [SerializeField] private IConnectable outputInterface;

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

        //Determime the color of the line
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
        lr.SetPosition(0, input.transform.position);
        lr.SetPosition(1, gameObject.transform.position);
        for (int i = 0; i < joints.Length; i++)
        {
            lr.SetPosition(i + 2, joints[i].position);
        }
        lr.SetPosition(joints.Length + 2, outputInterface.GetTransform().position);
    }

    /// <summary>
    /// Toggles the state of the connection
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

    public Transform GetTransform()
    {
        return transform;
    }

    public bool GetActive()
    {
        return active;
    }
    
    public PowerNode GetInput()
    {
        return input;
    }

    public void SetInput(PowerNode newInput)
    {
        input = newInput;
    }

    public GameObject GetOutput()
    {
        return output;
    }

    public void SetOutput(GameObject newOutput)
    {
        output = newOutput;
    }

    public void SetConnectionInterface(IConnectable newIf)
    {
        outputInterface = newIf;
    }

    public void SetActive(bool state)
    {
        active = state;
    }
}
