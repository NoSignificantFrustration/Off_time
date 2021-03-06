using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

/// <summary>
/// Handles important game functions like saving and loading. Only one GameController per scene.
/// </summary>
/// <seealso cref="DatabaseManager"/>
/// <seealso cref="SaveData"/>
public class GameController : MonoBehaviour
{
    /// <summary>Database manager</summary>
    [SerializeField] public DatabaseManager databaseManager;
    /// <summary>UI event handler</summary>
    [SerializeField] public UIEventHandler uIEventHandler;
    /// <summary>Level number</summary>
    [SerializeField] public int levelNumber;
    /// <summary>Main camera's CinemachineBrain</summary>
    [SerializeField] public CinemachineBrain cinemachineBrain;
    /// <summary>References to all the nodes</summary>
    private GameObject[] nodes;
    /// <summary>Reference to the source node</summary>
    [SerializeField] private GameObject sourceNode;
    /// <summary>References to all the blockers</summary>
    private GameObject[] blockers;
    /// <summary>Reference to the player</summary>
    private static GameObject player;
    /// <summary>References to all ISaveable interfaces in the scene</summary>
    private static List<ISaveable> saveables;

    /// <summary>
    /// Sets up directories, gets all saveables and loads from a save (if there is one set) when the game starts.
    /// </summary>
    // Start is called before the first frame update
    void Start()
    {
        FileManager.SetupDirs();
        CollectSaveables();

        //if there is a save file specified in the PlaySession load from it
        if (PlaySession.saveInfo.fileName != null) 
        {
            Load();
        }

        //Make all saveables do their initial setup
        foreach (ISaveable item in saveables)
        {
            item.Startup();
        }

        sourceNode.GetComponent<ISaveable>().Startup();
    }

    /// <summary>
    /// Gets the saveables list.
    /// </summary>
    /// <returns>A list of all the ISaveable interfaces</returns>
    public List<ISaveable> GetSaveables()
    {
        return saveables;
    }

    /// <summary>
    /// Collects all GameObject that can be saved and stores references to their ISaveable interfaces.
    /// </summary>
    public void CollectSaveables()
    {
        nodes = GameObject.FindGameObjectsWithTag("Node");
        blockers = GameObject.FindGameObjectsWithTag("Blocker");
        player = GameObject.FindGameObjectsWithTag("Player")[0];


        saveables = new List<ISaveable>();
        foreach (GameObject item in nodes)
        {
            if (item != sourceNode)
            {
                saveables.Add(item.GetComponent<ISaveable>());
            }
            
        }
        foreach (GameObject item in blockers)
        {
            saveables.Add(item.GetComponent<ISaveable>());
        }
        saveables.Add(player.GetComponent<ISaveable>());

    }

 
    /// <summary>
    /// Saves the game and calls for the database to be updated accordingly.
    /// </summary>
    /// <param name="saveTitle">Title of the save</param>
    /// <param name="fileName">Preferred file name</param>
    /// <param name="overwrite">Is the save meant to overwrite another or not</param>
    /// <returns>True if the operation succeeded, false if it didn't.</returns>
    public bool Save(string saveTitle, string fileName, bool overwrite)
    {
        SaveData sd = new SaveData();

        //Make all saveables add their information to the save
        foreach (ISaveable item in saveables)
        {
            item.AddToSave(sd);
        }

        if (overwrite)
        {
            //Try to make an overwriting save
            if (FileManager.WriteToFile(FileManager.saveDirectory, fileName, ".save", sd, overwrite, out string actualFilename))
            {
                //Update the PlaySession's SaveInfo with the new values
                PlaySession.saveInfo.saveTitle = saveTitle;
                PlaySession.saveInfo.fileName = actualFilename;
                PlaySession.saveInfo.saveTime = System.DateTime.Now;
                databaseManager.OverwriteSave(); //Update the database

                return true;
            }
            else
            {
                return false;
            }
            
        }
        else
        {
            //Try to make a non-overwriting save
            if (FileManager.WriteToFile(FileManager.saveDirectory, fileName, ".save", sd, overwrite, out string actualFilename))
            {
                //Update the PlaySession's SaveInfo with the new values
                PlaySession.saveInfo.saveTitle = saveTitle;
                PlaySession.saveInfo.fileName = actualFilename;
                PlaySession.saveInfo.saveTime = System.DateTime.Now;
                databaseManager.AddSave(); //Update the database

                return true;
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Loads from a save.
    /// </summary>
    public void Load()
    {
        SaveData sd;

        //Try to get the object from the disk
        if (FileManager.LoadFromFile(FileManager.saveDirectory, PlaySession.saveInfo.fileName, ".save", out object result))
        {
            sd = (SaveData)result;

            //Make all the saveables get their values from the save
            foreach (ISaveable item in saveables)
            {
                item.LoadFromSave(sd);
            }
        }
        else
        {
            Debug.LogError("Load failed");
        }

    }

    /// <summary>
    /// Gets the player.
    /// </summary>
    /// <returns>The player's GameObject.</returns>
    public static GameObject GetPlayer()
    {
        return player;
    }


}
