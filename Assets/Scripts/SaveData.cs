using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public List<NodeData> nodeDatas = new List<NodeData>();
    public List<BlockerData> blockerDatas = new List<BlockerData>();
    public List<DoorData> doorDatas = new List<DoorData>();
    public DroneData droneData;

    public string SaveAsjson()
    {
        return JsonUtility.ToJson(this);
    }

    public void LoadFromJson(string data)
    {
        JsonUtility.FromJsonOverwrite(data, this);
    }

    public object SaveAsObject()
    {
        return this;
    }

    


    [System.Serializable]
    public struct NodeData
    {
        public string uid;
        public int rotation;
        public bool isAcrivated;
        public bool isLocked;
        
    }

    [System.Serializable]
    public struct BlockerData
    {
        public string uid;
        public bool isAcivated;
        public bool isLocked;
    }

    [System.Serializable]
    public struct DroneData
    {
        public float[] position;
    }

    [System.Serializable]
    public struct DoorData
    {
        public string uid;
        public bool open;
    }
}

public interface ISaveable
{

    void AddToSave(SaveData saveData);
    void LoadFromSave(SaveData saveData);
    void Startup();
    Object GetObject(bool force = false);

}
