using System;
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

    public static bool WriteToFile(string folderPath, string preferredFileName, string extension, object fileContents, bool overWrite, out string actualFileName)
    {
        string fullPath = Path.Combine(folderPath, preferredFileName + extension);

        if (File.Exists(fullPath))
        {
            if (overWrite)
            {
                actualFileName = preferredFileName;
            }
            else
            {
                int i = 0;
                do
                {
                    actualFileName = preferredFileName + $"({i})";
                    fullPath = Path.Combine(folderPath, actualFileName + extension);
                } while (File.Exists(fullPath));
            }
        }
        else
        {
            actualFileName = preferredFileName;
        }

        try
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fs = new FileStream(fullPath, FileMode.Create);
            formatter.Serialize(fs, fileContents);
            fs.Close();
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to write to {fullPath} with exception {e}");
            return false;
        }
    }

    public static bool LoadFromFile(string folderPath, string fileName, string extension, out object result)
    {
        var fullPath = Path.Combine(folderPath, fileName);

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
