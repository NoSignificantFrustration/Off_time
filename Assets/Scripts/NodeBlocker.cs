using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Logic for the node blockers.
/// </summary>
/// <seealso cref="IConnectable"/>
/// <seealso cref="ISaveable"/>
/// <seealso cref="PowerConnection"/>
/// <seealso cref="PowerNode"/>
public class NodeBlocker : MonoBehaviour, IConnectable, ISaveable
{
    /// <summary>The input PowerConnection</summary>
    [SerializeField] private PowerConnection input;
    /// <summary>The node this blocker is latched onto</summary>
    [SerializeField] private PowerNode output;
    /// <summary>The range from this blocker can be accessed</summary>
    [SerializeField] private float range;
    /// <summary>The blocker's sprites</summary>
    [SerializeField] private Sprite[] sprites;
    /// <summary>Active state</summary>
    [SerializeField] private bool isActivated = false;
    /// <summary>Locked state</summary>
    [SerializeField] private bool isLocked = true;
    /// <summary>Unique ID</summary>
    [SerializeField] private string uid;
    /// <summary>Is the player in range?</summary>
    private bool in_range;
    /// <summary>SpriteRenderer component</summary>
    private SpriteRenderer sr;

    /// <summary>
    /// Loads the sprite appropriate to the blocker's state.
    /// </summary>
    public void LoadSprite()
    {
        if (isLocked)
        {
            if (isActivated)
            {
                sr.sprite = sprites[1];
            }
            else
            {
                sr.sprite = sprites[0];
            }
        }
        else
        {
            sr.sprite = sprites[2];
        }
    }

    /// <summary>
    /// Gets the trandform of the GameObject.
    /// </summary>
    /// <returns>The GameObject's transform.</returns>
    public Transform GetTransform()
    {
        return transform;
    }

    /// <summary>
    /// Sets the blocker's active state to match it's input's and refreshes the sprite.
    /// </summary>
    public void Pulse()
    {
        isActivated = input.GetActive();
        LoadSprite();
    }

    public void Toggle(bool state)
    {
        
    }

    /// <summary>
    /// Gets a reference to the SpriteRenderer component when the script is loaded.
    /// </summary>
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        
    }

    /// <summary>
    /// Starts a quiz if the node is activated,locked and in range.
    /// </summary>
    /// <param name="drone">Reference to the player</param>
    /// <seealso cref="DroneController"/>
    public void Click(DroneController drone)
    {

        
        if (isActivated && isLocked && in_range)
        {
            PlaySession.saveInfo.moves++;
            drone.StartQuiz(this);
        }
    }

    /// <summary>
    /// Unlocks the blocker and connected node.
    /// </summary>
    public void Unlock()
    {
        isLocked = false;
        output.Unlock();
        output.Pulse();
        LoadSprite();
    }
    
    /// <summary>
    /// Initial setup.
    /// </summary>
    public void Startup()
    {
        LoadSprite();
        if (isLocked)
        {
            sr.color = Color.red;
        }
        //Debug.Log(uid);
    }

    /// <summary>
    /// Checks if the player is in range, and anbles or disables interactability based on that.
    /// </summary>
    // Update is called once per frame
    void Update()
    {
        if (!isLocked)
        {
            return;
        }

        bool tempState;

        //Check if player is in range
        if (Vector3.Distance(GameController.GetPlayer().transform.position, transform.position) <= range)
        {
            tempState = true;
        }
        else
        {
            tempState = false;
        }

        //If the in range state changed change the sprite's color
        if (in_range != tempState)
        {
            in_range = tempState;
            if (in_range)
            {
                sr.color = Color.white;
            }
            else
            {
                sr.color = Color.red;
            }
        }
    }

    /// <summary>
    /// Adds this node's information to the provided SaveData.
    /// </summary>
    /// <param name="saveData">The provided SaveData</param>
    /// <seealso cref="SaveData"/>
    /// <seealso cref="SaveData.BlockerData"/>
    public void AddToSave(SaveData saveData)
    {
        SaveData.BlockerData data = new SaveData.BlockerData();
        data.uid = uid;
        data.isAcivated = isActivated;
        data.isLocked = isLocked;
        saveData.blockerDatas.Add(data);
    }

    /// <summary>
    /// Loads the blocker's information from the provided SaveData.
    /// </summary>
    /// <param name="saveData">The provided SaveData</param>
    public void LoadFromSave(SaveData saveData)
    {
        foreach (SaveData.BlockerData item in saveData.blockerDatas)
        {
            if (item.uid == uid)
            {
                isActivated = item.isAcivated;
                isLocked = item.isLocked;
                LoadSprite();
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
            //uid = Guid.NewGuid().ToString();
            return this;
        }
        return null;
    }
}
