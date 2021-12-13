using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalScript : MonoBehaviour, IConnectable
{
    [SerializeField]
    public PowerConnection input;
    [SerializeField]
    public string nextLevel;

    public void Click(DroneController drone)
    {

    }

    public Transform GetTransform()
    {
        return transform;
    }

    public void Pulse()
    {
        if (input.active)
        {
            SceneManager.LoadScene(nextLevel);
        }
        
    }

    public void Toggle(bool state)
    {
        if (state)
        {
            SceneManager.LoadScene(nextLevel);
        }
    }
}
