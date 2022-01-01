using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class PowerConnection : MonoBehaviour, IConnectable
{

    [SerializeField] private bool active;
    [SerializeField] private PowerNode input;
    [SerializeField] private GameObject output;
    [SerializeField] private Transform[] joints;
    private LineRenderer lr;
    [SerializeField] private IConnectable outputInterface;

    public void Awake()
    {
        lr = GetComponent<LineRenderer>();
        outputInterface = output.GetComponent<IConnectable>();

    }

    // Start is called before the first frame update
    void Start()
    {
        SetUpLines();

    }

 
    // Update is called once per frame
    void Update()
    {

           
    }

    public void SetUpLines()
    {
        if (lr == null)
        {
            lr = GetComponent<LineRenderer>();
            
        }

        if (active)
        {
            lr.material.color = new Color(0, 255, 0);
        }
        else
        {
            lr.material.color = new Color(255, 0, 0);
        }

        

        lr.positionCount = joints.Length + 3;
        lr.SetPosition(0, input.transform.position);
        lr.SetPosition(1, gameObject.transform.position);
        for (int i = 0; i < joints.Length; i++)
        {
            lr.SetPosition(i + 2, joints[i].position);
        }
        lr.SetPosition(joints.Length + 2, outputInterface.GetTransform().position);
    }

    public void Toggle(bool state)
    {
        active = state;
        if (state)
        {
            lr.material.color = new Color(0, 255, 0);
        }
        else
        {
            lr.material.color = new Color(255, 0, 0);
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
