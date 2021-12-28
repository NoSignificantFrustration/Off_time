using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalScript : MonoBehaviour, IConnectable
{
    [SerializeField] private PowerConnection input;
    [SerializeField] private string nextLevel;

    public void Click(DroneController drone)
    {

    }

    public Transform GetTransform()
    {
        return transform;
    }

    public void Pulse()
    {
        if (input.GetActive())
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
