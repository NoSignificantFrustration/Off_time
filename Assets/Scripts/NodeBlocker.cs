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
    /// <summary></summary>
    [SerializeField] private bool isActivated = false;
    /// <summary></summary>
    [SerializeField] private bool isLocked = true;
    /// <summary></summary>
    [SerializeField] private string uid;
    /// <summary></summary>
    private bool in_range;
    /// <summary></summary>
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

        
        if (isActivated && isLocked && in_range)
        {
            PlaySession.saveInfo.moves++;
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
        if (isLocked)
        {
            sr.color = Color.red;
        }
        //Debug.Log(uid);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocked)
        {
            return;
        }

        bool tempState;

        if (Vector3.Distance(GameController.GetPlayer().transform.position, transform.position) <= range)
        {
            tempState = true;
        }
        else
        {
            tempState = false;
        }

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
