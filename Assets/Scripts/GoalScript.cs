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
            //Make the next level accessible if the current level is the highest level they can access
            if (PlaySession.currentLevel == gameController.levelNumber)
            {
                PlaySession.currentLevel++;
                gameController.databaseManager.UpdateUserCurrentLevel();
            }
            gameController.databaseManager.AddWin();
            GetComponent<SpriteRenderer>().color = Color.green;
            transform.GetChild(0).gameObject.SetActive(false);

            Time.timeScale = 0f;
            gameController.uIEventHandler.OpenWinMenu();
            gameController.uIEventHandler.escapePressedEvent.AddListener(delegate { gameController.uIEventHandler.ExitToMainMenu(false); });
            
        }
        
    }

    public void Toggle(bool state)
    {
        
    }
}
