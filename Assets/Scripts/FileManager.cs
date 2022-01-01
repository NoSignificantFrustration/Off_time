using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class FileManager
{
    public static string saveDirectory = Path.Combine(Application.persistentDataPath, "Saves\\");

    public static void SetupDirs()
    {
        Directory.CreateDirectory(saveDirectory);
    }

    public static bool WriteToFile(string a_FileName, object a_FileContents)
    {
        var fullPath = Path.Combine(saveDirectory, a_FileName);
        

        try
        {
            //File.WriteAllText(fullPath, a_FileContents);
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fs = new FileStream(fullPath, FileMode.Create);
            formatter.Serialize(fs, a_FileContents);
            fs.Close();
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to write to {fullPath} with exception {e}");
            return false;
        }
    }

    public static bool LoadFromFile(string a_FileName, out object result)
    {
        var fullPath = Path.Combine(saveDirectory, a_FileName);

        try
        {
            if (File.Exists(fullPath))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream fs = new FileStream(fullPath, FileMode.Open);

                result = formatter.Deserialize(fs);
                fs.Close();
                return true;
            }
            else
            {
                result = null;
                return false;
            }
            //result = File.ReadAllText(fullPath);
            
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to read from {fullPath} with exception {e}");
            result = null;
            return false;
        }
    }
}
