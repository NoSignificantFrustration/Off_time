using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class PowerConnection : MonoBehaviour, IConnectable
{

    public bool active;
    [SerializeField]
    public PowerNode input;
    [SerializeField]
    public GameObject output;
    [SerializeField]
    public Transform[] joints;
    private LineRenderer lr;
    public IConnectable connectioInterface;

    private void Awake()
    {
       
        lr = GetComponent<LineRenderer>();
        lr.sharedMaterial = new Material(Shader.Find("Standard"));
        connectioInterface = output.GetComponent<IConnectable>();
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

        lr.sharedMaterial.color = new Color(255, 0, 0);

        lr.positionCount = joints.Length + 3;
        lr.SetPosition(0, input.transform.position);
        lr.SetPosition(1, gameObject.transform.position);
        for (int i = 0; i < joints.Length; i++)
        {
            lr.SetPosition(i + 2, joints[i].position);
        }
        lr.SetPosition(joints.Length + 2, connectioInterface.GetTransform().position);
    }

    public void Toggle(bool state)
    {
        active = state;
        if (state)
        {
            lr.sharedMaterial.color = new Color(0, 255, 0);
        }
        else
        {
            lr.sharedMaterial.color = new Color(255, 0, 0);
        }
        connectioInterface.Pulse();
        
    }

    public void Click()
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
}
