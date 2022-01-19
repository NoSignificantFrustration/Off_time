using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private DatabaseManager databaseManager;
    private static GameObject[] nodes;
    private static GameObject[] blockers;
    private static GameObject player;

    private static List<ISaveable> saveables;


    // Start is called before the first frame update
    void Start()
    {
        FileManager.SetupDirs();
        CollectSaveables();

        if (PlaySession.saveInfo.fileName != null)
        {
            Load();
        }

        foreach (ISaveable item in saveables)
        {
            item.Startup();
        }

    }

    public List<ISaveable> GetSaveables()
    {
        return saveables;
    }

    public void CollectSaveables()
    {
        nodes = GameObject.FindGameObjectsWithTag("Node");
        blockers = GameObject.FindGameObjectsWithTag("Blocker");
        player = GameObject.FindGameObjectsWithTag("Player")[0];


        saveables = new List<ISaveable>();
        foreach (GameObject item in nodes)
        {
            saveables.Add(item.GetComponent<ISaveable>());
        }
        foreach (GameObject item in blockers)
        {
            saveables.Add(item.GetComponent<ISaveable>());
        }
        saveables.Add(player.GetComponent<ISaveable>());

    }



    // Update is called once per frame
    void Update()
    {
        if (!UIEventHandler.isPaused)
        {
            PlaySession.saveInfo.elapsedTime += Time.deltaTime;
        }
        
    }

 

    public bool Save(string saveTitle, string fileName, bool overwrite)
    {
        SaveData sd = new SaveData();
        foreach (ISaveable item in saveables)
        {
            item.AddToSave(sd);
        }

        if (overwrite)
        {

            if (FileManager.WriteToFile(FileManager.saveDirectory, fileName, ".save", sd, overwrite, out string actualFilename))
            {
                PlaySession.saveInfo.saveTitle = saveTitle;
                PlaySession.saveInfo.fileName = actualFilename;
                PlaySession.saveInfo.saveTime = System.DateTime.Now;
                databaseManager.OverwriteSave();

                return true;
            }
            else
            {
                return false;
            }
            
        }
        else
        {
            if (FileManager.WriteToFile(FileManager.saveDirectory, fileName, ".save", sd, overwrite, out string actualFilename))
            {
                PlaySession.saveInfo.saveTitle = saveTitle;
                PlaySession.saveInfo.fileName = actualFilename;
                PlaySession.saveInfo.saveTime = System.DateTime.Now;
                databaseManager.AddSave();
                
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public void Load()
    {
        SaveData sd;
        if (FileManager.LoadFromFile(FileManager.saveDirectory, PlaySession.saveInfo.fileName, ".save", out object result))
        {
            sd = (SaveData)result;
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
}
