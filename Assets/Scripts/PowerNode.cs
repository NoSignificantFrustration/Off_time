using System.Collections;
using UnityEngine;


public class PowerNode : MonoBehaviour, IConnectable
{

    [SerializeField] private NodeType nodeType;
    [SerializeField] private BitArray connectionArray;
    [SerializeField] private bool[] inputs = new bool[4];
    [SerializeField] [Min(0)] private int rotation;
    [SerializeField] private bool isActiated;
    [SerializeField] private bool isLocked;
    [SerializeField] private bool skipInitialPulse;
    [SerializeField] private PowerConnection[] neighbours = new PowerConnection[4];
    [SerializeField] private Sprite[] sprites;
    private SpriteRenderer sr;



    private void Awake()
    {
        if (connectionArray == null)
        {
            SetupConnections();
        }
    }

    public void PreRotate()
    {

        if (nodeType == NodeType.I || nodeType == NodeType.L || nodeType == NodeType.T || nodeType == NodeType.X)
        {
            sr.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            SetupConnections();
            
            for (int i = 0; i < rotation; i++)
            {
                sr.transform.Rotate(Vector3.back * 90);
                BitArray tempArray = new BitArray(4);
                tempArray[0] = connectionArray[3];
                for (int j = 0; j < 3; j++)
                {
                    tempArray[j + 1] = connectionArray[j];
                }
                connectionArray = tempArray;
            }
        }
    }

    void Start()
    {
        LoadSprite();
        if ((nodeType == NodeType.Source || nodeType == NodeType.NOT || nodeType == NodeType.NAND || nodeType == NodeType.NOR || nodeType == NodeType.XNOR) && !skipInitialPulse )
        {  
            Pulse();
        }
        PreRotate();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Turn()
    {
        if (isLocked)
        {
            return;
        }

        if (nodeType == NodeType.I || nodeType == NodeType.L || nodeType == NodeType.T || nodeType == NodeType.X)
        {
            //Debug.Log(sr.transform.localRotation);
            sr.transform.Rotate(Vector3.back * 90);
            BitArray tempArray = new BitArray(4);
            tempArray[0] = connectionArray[3];
            for (int i = 0; i < 3; i++)
            {
                tempArray[i + 1] = connectionArray[i];
            }
            connectionArray = tempArray;
            if (rotation < 3)
            {
                rotation++;
            }
            else
            {
                rotation = 0;
            }

            if (Application.isPlaying)
            {
                Pulse();
            }
        }

    }

    public void Pulse()
    {

        if (isLocked)
        {
            return;
        }

        bool tempActive;
        switch (nodeType)
        {

            case NodeType.Source:
                for (int i = 0; i < 4; i++)
                {
                    if (neighbours[i] != null)
                    {
                        neighbours[i].Toggle(true);
                    }

                }
                break;
            case NodeType.I:
                TurnEvaluate();
                break;
            case NodeType.L:
                TurnEvaluate();
                break;
            case NodeType.T:
                TurnEvaluate();
                break;
            case NodeType.X:
                TurnEvaluate();
                break;
            case NodeType.NOT:
                if (isActiated == neighbours[0].GetActive())
                {
                    isActiated = !isActiated;
                    for (int i = 0; i < 4; i++)
                    {
                        if (!inputs[i] && neighbours[i] != null)
                        {
                            neighbours[i].Toggle(isActiated);
                        }
                    }
                    LoadSprite();
                }

                break;
            case NodeType.AND:

                if (neighbours[0].GetActive() && neighbours[1].GetActive())
                {
                    tempActive = true;
                }
                else
                {
                    tempActive = false;
                }
                if (tempActive != isActiated)
                {
                    isActiated = tempActive;
                    for (int i = 0; i < 4; i++)
                    {
                        if (!inputs[i] && neighbours[i] != null)
                        {
                            neighbours[i].Toggle(isActiated);
                        }
                    }
                    LoadSprite();
                }
                break;
            case NodeType.OR:
                if (neighbours[0].GetActive() || neighbours[1].GetActive())
                {
                    tempActive = true;
                }
                else
                {
                    tempActive = false;
                }
                if (tempActive != isActiated)
                {
                    isActiated = tempActive;
                    for (int i = 0; i < 4; i++)
                    {
                        if (!inputs[i] && neighbours[i] != null)
                        {
                            neighbours[i].Toggle(isActiated);
                        }
                    }
                    LoadSprite();
                }
                break;
            case NodeType.NAND:
                if (neighbours[0].GetActive() && neighbours[1].GetActive())
                {
                    tempActive = false;
                }
                else
                {
                    tempActive = true;
                }
                if (tempActive != isActiated)
                {
                    isActiated = tempActive;
                    for (int i = 0; i < 4; i++)
                    {
                        if (!inputs[i] && neighbours[i] != null)
                        {
                            neighbours[i].Toggle(isActiated);
                        }
                    }
                    LoadSprite();
                }
                break;
            case NodeType.NOR:
                if (neighbours[0].GetActive() || neighbours[1].GetActive())
                {
                    tempActive = false;
                }
                else
                {
                    tempActive = true;
                }
                if (tempActive != isActiated)
                {
                    isActiated = tempActive;
                    for (int i = 0; i < 4; i++)
                    {
                        if (!inputs[i] && neighbours[i] != null)
                        {
                            neighbours[i].Toggle(isActiated);
                        }
                    }
                    LoadSprite();
                }
                break;
            case NodeType.XOR:
                if (neighbours[0].GetActive() != neighbours[1].GetActive())
                {
                    tempActive = true;
                }
                else
                {
                    tempActive = false;
                }
                if (tempActive != isActiated)
                {
                    isActiated = tempActive;
                    for (int i = 0; i < 4; i++)
                    {
                        if (!inputs[i] && neighbours[i] != null)
                        {
                            neighbours[i].Toggle(isActiated);
                        }
                    }
                    LoadSprite();
                }
                break;
            case NodeType.XNOR:

                if (neighbours[0].GetActive() == neighbours[1].GetActive())
                {
                    tempActive = true;
                }
                else
                {
                    tempActive = false;
                }
                if (tempActive != isActiated)
                {
                    isActiated = tempActive;
                    for (int i = 0; i < 4; i++)
                    {
                        if (!inputs[i] && neighbours[i] != null)
                        {
                            neighbours[i].Toggle(isActiated);
                        }
                    }
                    LoadSprite();
                }
                break;
            default:
                break;
        }
    }

    private void TurnEvaluate()
    {
        bool tempActive = false;
        for (int i = 0; i < 4; i++)
        {
            if (connectionArray[i] && inputs[i])
            {
                if (neighbours[i].GetActive())
                {
                    tempActive = true;
                    break;
                }
            }
        }

        if (tempActive)
        {
            for (int i = 0; i < 4; i++)
            {
                if (neighbours[i] != null && !inputs[i])
                {
                    if (connectionArray[i])
                    {
                        neighbours[i].Toggle(true);
                    }
                    else
                    {
                        neighbours[i].Toggle(false);
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                if (neighbours[i] != null && !inputs[i])
                {
                    neighbours[i].Toggle(false);
                }
            }
        }
        if (isActiated != tempActive)
        {
            isActiated = tempActive;
            LoadSprite();
        }

    }

    public void SetupConnections()
    {
        switch (nodeType)
        {
            case NodeType.Source:

                break;
            case NodeType.I:
                connectionArray = new BitArray(new bool[] { true, false, true, false });

                break;
            case NodeType.L:
                connectionArray = new BitArray(new bool[] { true, true, false, false });

                break;
            case NodeType.T:
                connectionArray = new BitArray(new bool[] { true, true, false, true });

                break;
            case NodeType.X:
                connectionArray = new BitArray(new bool[] { true, true, true, true });

                break;
            case NodeType.NOT:
                inputs = new bool[] { true, false, false, false };
                break;
            case NodeType.AND:
                inputs = new bool[] { true, true, false, false };
                break;
            case NodeType.OR:
                inputs = new bool[] { true, true, false, false };
                break;
            case NodeType.NAND:
                inputs = new bool[] { true, true, false, false };
                break;
            case NodeType.NOR:
                inputs = new bool[] { true, true, false, false };
                break;
            case NodeType.XOR:
                inputs = new bool[] { true, true, false, false };
                break;
            case NodeType.XNOR:
                inputs = new bool[] { true, true, false, false };
                break;
            default:
                break;
        }
        sr = GetComponent<SpriteRenderer>();
    }

    public void LoadSprite()
    {
        if (isLocked)
        {
            isActiated = false;
        }
        if (sr == null)
        {
            sr = GetComponent<SpriteRenderer>();
        }
        switch (nodeType)
        {
            case NodeType.Source:
                if (!isActiated)
                {
                    sr.sprite = sprites[0];
                }
                else
                {
                    sr.sprite = sprites[1];
                }

                break;
            case NodeType.I:
                if (!isActiated)
                {
                    sr.sprite = sprites[2];
                }
                else
                {
                    sr.sprite = sprites[3];
                }
                break;
            case NodeType.L:
                if (!isActiated)
                {
                    sr.sprite = sprites[4];
                }
                else
                {
                    sr.sprite = sprites[5];
                }
                break;
            case NodeType.T:
                if (!isActiated)
                {
                    sr.sprite = sprites[6];
                }
                else
                {
                    sr.sprite = sprites[7];
                }
                break;
            case NodeType.X:
                if (!isActiated)
                {
                    sr.sprite = sprites[8];
                }
                else
                {
                    sr.sprite = sprites[9];
                }
                break;
            case NodeType.NOT:
                if (!isActiated)
                {
                    sr.sprite = sprites[10];
                }
                else
                {
                    sr.sprite = sprites[11];
                }
                break;
            case NodeType.AND:
                if (!isActiated)
                {
                    sr.sprite = sprites[12];
                }
                else
                {
                    sr.sprite = sprites[13];
                }
                break;
            case NodeType.OR:
                if (!isActiated)
                {
                    sr.sprite = sprites[14];
                }
                else
                {
                    sr.sprite = sprites[15];
                }
                break;
            case NodeType.NAND:
                if (!isActiated)
                {
                    sr.sprite = sprites[16];
                }
                else
                {
                    sr.sprite = sprites[17];
                }
                break;
            case NodeType.NOR:
                if (!isActiated)
                {
                    sr.sprite = sprites[18];
                }
                else
                {
                    sr.sprite = sprites[19];
                }
                break;
            case NodeType.XOR:
                if (!isActiated)
                {
                    sr.sprite = sprites[20];
                }
                else
                {
                    sr.sprite = sprites[21];
                }
                break;
            case NodeType.XNOR:
                if (!isActiated)
                {
                    sr.sprite = sprites[22];
                }
                else
                {
                    sr.sprite = sprites[23];
                }
                break;
            default:
                break;
        }
    }



    public void SetNeighbour(int index, PowerConnection connection)
    {
        neighbours[index] = connection;
    }

    public void Click(DroneController drone)
    {
        if (nodeType == NodeType.I || nodeType == NodeType.L || nodeType == NodeType.T)
        {
            Turn();
        }
    }

    public void Unlock()
    {
        isLocked = false;
    }
    public PowerConnection GetNeighbor(int nIndex)
    {
        return neighbours[nIndex];
    }

    public bool GetActive()
    {
        return isActiated;
    }

    public void Toggle(bool state)
    {
        Debug.Log(gameObject.name + " Needs to be pulsed, not toggled.");
    }

    public Transform GetTransform()
    {
        return gameObject.transform;
    }

    public enum NodeType
    {
        Source, I, L, T, X, NOT, AND, OR, NAND, NOR, XOR, XNOR
    }
}
