using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    private static GameObject[] nodes;
    private static GameObject[] blockers;
    private static GameObject player;

    private static List<ISaveable> saveables;


    // Start is called before the first frame update
    void Start()
    {
        FileManager.SetupDirs();
        CollectSaveables();

        if (PlaySession.saveFileName != null)
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

    }

    public void Save()
    {
        SaveData sd = new SaveData();
        foreach (ISaveable item in saveables)
        {
            item.AddToSave(sd);
        }
        PlaySession.saveFileName = PlaySession.username + ".test";
        //FileManager.WriteToFile(PlaySession.saveFileName, sd);
    }

    public void Load()
    {
        //SaveData sd;
        //PlaySession.saveFileName = PlaySession.username + ".test";
        //if (FileManager.LoadFromFile(PlaySession.saveFileName, out object result))
        //{
        //    //sd.LoadFromJson(result);
        //    sd = (SaveData)result;
        //    foreach (ISaveable item in saveables)
        //    {
        //        item.LoadFromSave(sd);
        //    }
        //}
        //else
        //{
        //    Debug.LogError("Load failed");
        //}

    }
}
