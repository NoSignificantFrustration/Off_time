using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that stores save data and can be written to disk.
/// </summary>
/// <seealso cref="ISaveable"/>
[System.Serializable]
public class SaveData
{
    /// <summary>Node data list</summary>
    public List<NodeData> nodeDatas = new List<NodeData>();
    /// <summary>Blocker data list</summary>
    public List<BlockerData> blockerDatas = new List<BlockerData>();
    /// <summary>Door data list</summary>
    public List<DoorData> doorDatas = new List<DoorData>();
    /// <summary>Drone data</summary>
    public DroneData droneData;

    /// <summary>
    /// Turns the object into a json string.
    /// </summary>
    /// <returns>The object in json format.</returns>
    public string SaveAsjson()
    {
        return JsonUtility.ToJson(this);
    }

    /// <summary>
    /// Reconstructs the object's contents from a json string.
    /// </summary>
    /// <param name="data">Source json string</param>
    public void LoadFromJson(string data)
    {
        JsonUtility.FromJsonOverwrite(data, this);
    }

    /// <summary>
    /// Gets the object as a generic object.
    /// </summary>
    /// <returns>This SaveData object as a generic object.</returns>
    public object SaveAsObject()
    {
        return this;
    }



    /// <summary>
    /// Struct that contains a PowerNode's save data.
    /// </summary>
    /// <seealso cref="PowerNode"/>
    [System.Serializable]
    public struct NodeData
    {
        /// <summary>Unique ID</summary>
        public string uid;
        /// <summary>Rotation</summary>
        public int rotation;
        /// <summary>Active state</summary>
        public bool isAcrivated;
        /// <summary>Locked state</summary>
        public bool isLocked;
        
    }

    /// <summary>
    /// Struct that contains a NodeBlocker's save data.
    /// </summary>
    /// <seealso cref="NodeBlocker"/>
    [System.Serializable]
    public struct BlockerData
    {
        /// <summary>Unique ID</summary>
        public string uid;
        /// <summary>Active state</summary>
        public bool isAcivated;
        /// <summary>Locked state</summary>
        public bool isLocked;
    }

    /// <summary>
    /// Struct that contains a DroneController's save data.
    /// </summary>
    /// <seealso cref="DroneController"/>
    [System.Serializable]
    public struct DroneData
    {
        /// <summary>The player's position</summary>
        public float[] position;
    }

    /// <summary>
    /// Struct that contains a DoorController's save data.
    /// </summary>
    /// <seealso cref="DoorController"/>
    [System.Serializable]
    public struct DoorData
    {
        /// <summary>Unique ID</summary>
        public string uid;
        /// <summary>Open state</summary>
        public bool open;
    }
}

/// <summary>
/// Interface for saveables.
/// </summary>
/// <seealso cref="SaveData"/>
public interface ISaveable
{
    /// <summary>
    /// Makes the object append it's save information to the Provided SaveData.
    /// </summary>
    /// <param name="saveData">Provided SaveData</param>
    void AddToSave(SaveData saveData);

    /// <summary>
    /// Makes the object get it's save information from the provided SaveData.
    /// </summary>
    /// <param name="saveData">Provided SaveData</param>
    void LoadFromSave(SaveData saveData);

    /// <summary>
    /// Makes the object do it's initial setup.
    /// </summary>
    void Startup();

    /// <summary>
    /// Tries to get the object, can force it too. For internal use.
    /// </summary>
    /// <param name="force">Force?</param>
    /// <returns>The object.</returns>
    Object GetObject(bool force = false);

}
