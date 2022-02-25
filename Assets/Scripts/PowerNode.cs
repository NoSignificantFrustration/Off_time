using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Power node logic.
/// </summary>
/// <seealso cref="IConnectable"/>
/// <seealso cref="ISaveable"/>
/// <seealso cref="NodeType"/>
/// <seealso cref="PowerConnection"/>
public class PowerNode : MonoBehaviour, IConnectable, ISaveable
{
    /// <summary>Type of the node</summary>
    [SerializeField] private NodeType nodeType;
    /// <summary>Determines which neighbours are connected. [0]: bottom, [1]: left side, [2]: top, [3]: right side</summary>
    [SerializeField] private BitArray connectionArray;
    /// <summary>Determines which neighbours should be considered inputs (The same ordering scheme applies as with connectionArray)</summary>
    [SerializeField] private bool[] inputs = new bool[4];
    /// <summary>Current rotation (Not to be confused with the actual GameObject's rotation)</summary>
    [SerializeField] [Min(0)] private int rotation;
    /// <summary>Active state</summary>
    [SerializeField] private bool isActiated;
    /// <summary>Locked state</summary>
    [SerializeField] private bool isLocked;
    /// <summary>Some NodeTypes will want to do a pulse on startup to set up theit behavior, it can be skipped by setting this true</summary>
    [SerializeField] private bool skipInitialPulse;
    /// <summary>List of neighbouring PowerConnections (The same ordering scheme applies as with connectionArray)</summary>
    [SerializeField] private PowerConnection[] neighbours = new PowerConnection[4];
    /// <summary>List of sprites to be used</summary>
    [SerializeField] private Sprite[] sprites;
    /// <summary>Unique ID</summary>
    [SerializeField] private string uid;
    /// <summary>SpriteRenderer component</summary>
    private SpriteRenderer sr;


    /// <summary>
    /// Gets the connectionArray set up if it isn't already when the script is loaded.
    /// </summary>
    private void Awake()
    {
        if (connectionArray == null)
        {
            SetupConnections();
        }
    }

    /// <summary>
    /// Prerotates the node.
    /// </summary>
    public void PreRotate()
    {
        //Only rotate if the nide type is meant to be rotated (Rotating logic nodes fe. would break them)
        if (nodeType == NodeType.I || nodeType == NodeType.L || nodeType == NodeType.T || nodeType == NodeType.X)
        {
            //Reset the SpriteRenderer's rotation
            sr.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            SetupConnections(); //Set up the connection and input array
            
            //Rotate the sprite and shift the connectionArray to match the desired rotation
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


    /// <summary>
    /// Initial setup of the node.
    /// </summary>
    public void Startup()
    {
        LoadSprite();
        PreRotate();
        if (isLocked) //Set the color to red is it's locked
        {
            sr.color = Color.red;
        }
        //Pulse if the current node type demands it and it's not prohibited by skipInitialPulse
        if ((nodeType == NodeType.Source || nodeType == NodeType.NOT || nodeType == NodeType.NAND || nodeType == NodeType.NOR || nodeType == NodeType.XNOR) && !skipInitialPulse)
        {
            Pulse();
            //Debug.Log("Pulse");
        }
        //Loop over all neighbours. If they exist, not an input and the connectionArray says they are connected set their state to the node's state and refresh their lines
        for (int i = 0; i < 4; i++)
        {
            if (!inputs[i] && neighbours[i] != null && connectionArray[i])
            {
                neighbours[i].SetActive(isActiated);
                neighbours[i].SetUpLines();
            }

        }


    }

    /// <summary>
    /// Turns the node 90° clockwise.
    /// </summary>
    void Turn()
    {
        if (isLocked)
        {
            return;
        }

        PlaySession.saveInfo.moves++; //Increment the move counter

        //Only rotate if the nide type is meant to be rotated (Rotating logic nodes fe. would break them)
        if (nodeType == NodeType.I || nodeType == NodeType.L || nodeType == NodeType.T || nodeType == NodeType.X)
        {
            
            sr.transform.Rotate(Vector3.back * 90); //Rotate the sprite 90°

            //Shift the connectionArray
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

    /// <summary>
    /// Reevaluates the node's active state and toggles outputs.
    /// </summary>
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
                //Loop through all existing neighbours and toggle them on
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
                //neighbours[0] is considered the only input, isActiated will be set to it's opposite
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
                //Neighbours[0] and neighbours[1] are inputs, AND operation
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
                //Neighbours[0] and neighbours[1] are inputs, OR operation
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
                //Neighbours[0] and neighbours[1] are inputs, NAND operation
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
                //Neighbours[0] and neighbours[1] are inputs, NOR operation
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
                //Neighbours[0] and neighbours[1] are inputs, XOR operation
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
                //Neighbours[0] and neighbours[1] are inputs, XNOR operation
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

    /// <summary>
    /// Determines the node's active state and toggles neighbors according to it and the connections array.
    /// </summary>
    private void TurnEvaluate()
    {
        bool tempActive = false;
        //Determine active state
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

        //Toggle neighbours accordingly
        if (tempActive)
        {
            for (int i = 0; i < 4; i++)
            {
                //If there is a neighbour on the current index that is not an input toggle it
                if (neighbours[i] != null && !inputs[i]) 
                {
                    //If it's connected toggle it on, if it's not toggle it off
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
            //Toggle all non-input neighbours off
            for (int i = 0; i < 4; i++)
            {
                if (neighbours[i] != null && !inputs[i])
                {
                    neighbours[i].Toggle(false);
                }
            }
        }
        //If the active state changed store it and load the appropriate sprites
        if (isActiated != tempActive)
        {
            isActiated = tempActive;
            LoadSprite();
        }

    }

    /// <summary>
    /// Sets up the connectionArray and inputs arrays according to the NodeType.
    /// </summary>
    public void SetupConnections()
    {
        switch (nodeType)
        {
            case NodeType.Source:
                connectionArray = new BitArray(new bool[] { true, true, true, true });
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
                connectionArray = new BitArray(new bool[] { true, true, true, true });
                break;
            case NodeType.AND:
                inputs = new bool[] { true, true, false, false };
                connectionArray = new BitArray(new bool[] { true, true, true, true });
                break;
            case NodeType.OR:
                inputs = new bool[] { true, true, false, false };
                connectionArray = new BitArray(new bool[] { true, true, true, true });
                break;
            case NodeType.NAND:
                inputs = new bool[] { true, true, false, false };
                connectionArray = new BitArray(new bool[] { true, true, true, true });
                break;
            case NodeType.NOR:
                inputs = new bool[] { true, true, false, false };
                connectionArray = new BitArray(new bool[] { true, true, true, true });
                break;
            case NodeType.XOR:
                inputs = new bool[] { true, true, false, false };
                connectionArray = new BitArray(new bool[] { true, true, true, true });
                break;
            case NodeType.XNOR:
                inputs = new bool[] { true, true, false, false };
                connectionArray = new BitArray(new bool[] { true, true, true, true });
                break;
            default:
                connectionArray = new BitArray(new bool[] { true, true, true, true });
                break;
        }
        sr = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Loads the appropriate sprite according to the isActiated state and NodeType.
    /// </summary>
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


    /// <summary>
    /// Sets the neigbour on the specified index (For internal use).
    /// </summary>
    /// <param name="index">neighbours array index</param>
    /// <param name="connection">The new PowerConnection</param>
    public void SetNeighbour(int index, PowerConnection connection)
    {
        neighbours[index] = connection;
    }

    /// <summary>
    /// Turns the node when the player clicks on it.
    /// </summary>
    /// <param name="drone">Player drone</param>
    /// <seealso cref="DroneController"/>
    public void Click(DroneController drone)
    {
        if (nodeType == NodeType.I || nodeType == NodeType.L || nodeType == NodeType.T)
        {
            Turn();
        }
    }

    /// <summary>
    /// Unlocks the node.
    /// </summary>
    public void Unlock()
    {
        isLocked = false;
        sr.color = Color.white;
    }

    /// <summary>
    /// Gets the neighbour on the specified index.
    /// </summary>
    /// <param name="nIndex">Index</param>
    /// <returns>Requested PowerConnection</returns>
    public PowerConnection GetNeighbor(int nIndex)
    {
        return neighbours[nIndex];
    }

    /// <summary>
    /// Gets the active state.
    /// </summary>
    /// <returns>Active state.</returns>
    public bool GetActive()
    {
        return isActiated;
    }

    public void Toggle(bool state)
    {
        Debug.Log(gameObject.name + " Needs to be pulsed, not toggled.");
    }

    /// <summary>
    /// Gets the transform of this GameObject.
    /// </summary>
    /// <returns>The transform of this GameObject.</returns>
    public Transform GetTransform()
    {
        return gameObject.transform;
    }

    /// <summary>
    /// Adds this node's information to the provided SaveData.
    /// </summary>
    /// <param name="saveData">The provided SaveData</param>
    /// <seealso cref="SaveData"/>
    /// <seealso cref="SaveData.NodeData"/>
    public void AddToSave(SaveData saveData)
    {
        SaveData.NodeData data = new SaveData.NodeData();
        data.uid = uid;
        data.rotation = rotation;
        data.isAcrivated = isActiated;
        data.isLocked = isLocked;
        saveData.nodeDatas.Add(data);
    }

    /// <summary>
    /// Loads the node's information from the provided SaveData.
    /// </summary>
    /// <param name="saveData">The provided SaveData</param>
    public void LoadFromSave(SaveData saveData)
    {
        foreach (SaveData.NodeData item in saveData.nodeDatas)
        {
            if (item.uid == uid)
            {
                rotation = item.rotation;
                isActiated = item.isAcrivated;
                isLocked = item.isLocked;
                skipInitialPulse = true;
                break;
            }
        }
    }

    /// <summary>
    /// For internal use. Returns this object if it has no uid or is forced to.
    /// </summary>
    /// <param name="force">Force state</param>
    /// <returns>The object.</returns>
    public UnityEngine.Object GetObject(bool force = false)
    {
        if (uid == null || uid == "" || force)
        {
            return this;
        }
        return null;
    }

    /// <summary>
    /// Node type enum.
    /// </summary>
    public enum NodeType
    {
        Source, I, L, T, X, NOT, AND, OR, NAND, NOR, XOR, XNOR
    }
}
