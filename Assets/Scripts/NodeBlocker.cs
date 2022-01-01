using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeBlocker : MonoBehaviour, IConnectable, ISaveable
{
    [SerializeField] private PowerConnection input;
    [SerializeField] private PowerNode output;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private bool isActivated = false;
    [SerializeField] private bool isLocked = true;
    [SerializeField] private string uid;
    private SpriteRenderer sr;

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

    public Transform GetTransform()
    {
        return transform;
    }

    public void Pulse()
    {
        isActivated = input.GetActive();
        LoadSprite();
    }

    public void Toggle(bool state)
    {
        
    }

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        
    }

    public void Click(DroneController drone)
    {

        
        if (isActivated && isLocked)
        {
            drone.StartQuiz(this);
        }
    }

    public void Unlock()
    {
        isLocked = false;
        output.Unlock();
        output.Pulse();
        LoadSprite();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    public void Startup()
    {
        LoadSprite();
        //Debug.Log(uid);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddToSave(SaveData saveData)
    {
        SaveData.BlockerData data = new SaveData.BlockerData();
        data.uid = uid;
        data.isAcivated = isActivated;
        data.isLocked = isLocked;
        saveData.blockerDatas.Add(data);
    }

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
