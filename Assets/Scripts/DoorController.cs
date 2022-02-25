using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Logic for the nodes that control the doors.
/// </summary>
/// <seealso cref="Door"/>
/// <seealso cref="PowerConnection"/>
/// <seealso cref="IConnectable"/>
/// <seealso cref="ISaveable"/>
public class DoorController : MonoBehaviour, IConnectable, ISaveable
{
    /// <summary>Input PowerConnection</summary>
    [SerializeField] private PowerConnection input;
    /// <summary>The door this controls</summary>
    [SerializeField] private Door door;
    /// <summary>Current state</summary>
    [SerializeField] private bool open;
    /// <summary>Sprites</summary>
    [SerializeField] private Sprite[] sprites;
    /// <summary>SpriteRenderer component</summary>
    private SpriteRenderer sr;
    /// <summary>Unique identifier</summary>
    [SerializeField] private string uid;

    /// <summary>
    /// Gets the SpriteRenderer component when the script is loaded.
    /// </summary>
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    
    public void Click(DroneController drone = null)
    {
        
    }

    /// <summary>
    /// Sends the transform of this GameObject.
    /// </summary>
    /// <returns>The transform of this GameObject</returns>
    public Transform GetTransform()
    {
        return transform;
    }

    /// <summary>
    /// Sets the door's state based on the input's state.
    /// </summary>
    public void Pulse()
    {
        bool tempActive = input.GetActive();

        if (open != tempActive)
        {
            open = tempActive;
            LoadSprite();
            door.ToggleDoor(open);
        }
    }

    /// <summary>
    /// Loads the sprite corresponding with the node's state.
    /// </summary>
    private void LoadSprite()
    {
        if (!open)
        {
            sr.sprite = sprites[0];
        }
        else
        {
            sr.sprite = sprites[1];
        }
    }

    public void Toggle(bool state)
    {
        
    }

    /// <summary>
    /// Adds this node's information to the provided SaveData.
    /// </summary>
    /// <param name="saveData">The provided SaveData</param>
    /// <seealso cref="SaveData"/>
    /// <seealso cref="SaveData.DoorData"/>
    public void AddToSave(SaveData saveData)
    {
        SaveData.DoorData data = new SaveData.DoorData();
        data.uid = uid;
        data.open = open;
        saveData.doorDatas.Add(data);
    }

    /// <summary>
    /// Loads the door's information from the provided SaveData.
    /// </summary>
    /// <param name="saveData">The provided SaveData</param>
    public void LoadFromSave(SaveData saveData)
    {
        foreach (SaveData.DoorData item in saveData.doorDatas)
        {
            if (item.uid == uid)
            {
                open = item.open;
                break;
            }
        }
    }

    /// <summary>
    /// Initial setup for the door.
    /// </summary>
    public void Startup()
    {
        door.ToggleDoor(open);
        LoadSprite();
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
}
