using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeBlocker : MonoBehaviour, IConnectable
{
    [SerializeField]
    public PowerConnection input;
    [SerializeField]
    public PowerNode output;
    [SerializeField]
    public Sprite[] sprites;
    [SerializeField]
    public bool isActivated = false;
    [SerializeField]
    public bool isLocked = true;
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
        isActivated = input.active;
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
        output.isLocked = false;
        output.Pulse();
        LoadSprite();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }




}