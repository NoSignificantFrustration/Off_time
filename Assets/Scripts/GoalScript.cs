using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Logic for the goal node.
/// </summary>
/// <seealso cref="IConnectable"/>
/// <seealso cref="PowerConnection"/>
/// <seealso cref="GameController"/>
public class GoalScript : MonoBehaviour, IConnectable
{
    /// <summary>The input PowerConnection</summary>
    [SerializeField] private PowerConnection input;
    /// <summary>Name of the level that should be loaded</summary>
    [SerializeField] private string nextLevel;
    /// <summary>The scene's GameController</summary>
    [SerializeField] private GameController gameController;

    public void Click(DroneController drone)
    {

    }

    /// <summary>
    /// Gets the object's transform.
    /// </summary>
    /// <returns>The transform.</returns>
    public Transform GetTransform()
    {
        return transform;
    }

    /// <summary>
    /// Loads the next level when the node is powered.
    /// </summary>
    public void Pulse()
    {
        if (input.GetActive())
        {
            if (PlaySession.currentLevel == gameController.levelNumber)
            {
                PlaySession.currentLevel++;
                gameController.databaseManager.UpdateUserCurrentLevel();
            }
            gameController.databaseManager.AddWin();
            SceneManager.LoadScene(nextLevel);
        }
        
    }

    /// <summary>
    /// Loads the next level when the node is powered.
    /// </summary>
    public void Toggle(bool state)
    {
        if (state)
        {
            SceneManager.LoadScene(nextLevel);
        }
    }
}
