using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour, IConnectable, ISaveable
{
    
    [SerializeField] private PowerConnection input;
    [SerializeField] private Door door;
    [SerializeField] private bool open;
    [SerializeField] private Sprite[] sprites;
    private SpriteRenderer sr;
    [SerializeField] private string uid;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }



    public void Click(DroneController drone = null)
    {
        
    }

    public Transform GetTransform()
    {
        return transform;
    }

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

    public void AddToSave(SaveData saveData)
    {
        SaveData.DoorData data = new SaveData.DoorData();
        data.uid = uid;
        data.open = open;
        saveData.doorDatas.Add(data);
    }

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

    public void Startup()
    {
        door.ToggleDoor(open);
        LoadSprite();
    }

    public UnityEngine.Object GetObject(bool force = false)
    {
        if (uid == null || uid == "" || force)
        {
            return this;
        }
        return null;
    }
}
