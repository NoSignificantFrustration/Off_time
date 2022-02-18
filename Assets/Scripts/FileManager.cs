using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

/// <summary>
/// Class for handling file operations.
/// </summary>
public static class FileManager
{
    /// <summary>Folder where save files are stored</summary>
    public static string saveDirectory = Path.Combine(Application.persistentDataPath, "Saves\\");

    /// <summary>
    /// Sets up the needed directories.
    /// </summary>
    public static void SetupDirs()
    {
        Directory.CreateDirectory(saveDirectory);
    }

    /// <summary>
    /// Writes an object's contents to disk.
    /// </summary>
    /// <param name="folderPath">Path to the folder we want to write to</param>
    /// <param name="preferredFileName">Preferred file name</param>
    /// <param name="extension">File extension</param>
    /// <param name="fileContents">The object we want to save</param>
    /// <param name="overwrite">Do we want to overwrite the file if it already exists</param>
    /// <param name="actualFileName">Return value of the name the file was actually named in the end</param>
    /// <returns>True if the operation was successful, false if it was not.</returns>
    public static bool WriteToFile(string folderPath, string preferredFileName, string extension, object fileContents, bool overwrite, out string actualFileName)
    {
        string fullPath = Path.Combine(folderPath, preferredFileName + extension);

        if (File.Exists(fullPath))
        {
            if (overwrite)
            {
                actualFileName = preferredFileName;
            }
            else
            {
                //Loop through numbered versions of the preferred file name until we find one that is not taken
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

        //Try to write to the disk
        try
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fs = new FileStream(fullPath, FileMode.Create);
            formatter.Serialize(fs, fileContents); //Serialise our object
            fs.Close();
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to write to {fullPath} with exception {e}");
            return false;
        }
    }

    /// <summary>
    /// Reads an object from the disk.
    /// </summary>
    /// <param name="folderPath">Path to the folder we want to read from</param>
    /// <param name="fileName">Name of the file</param>
    /// <param name="extension">File extension</param>
    /// <param name="result">Return value of the read object</param>
    /// <returns>True if the operation was successful, false if it was not.</returns>
    public static bool LoadFromFile(string folderPath, string fileName, string extension, out object result)
    {
        var fullPath = Path.Combine(folderPath, fileName + extension);

        try
        {
            if (File.Exists(fullPath))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream fs = new FileStream(fullPath, FileMode.Open);

                //Deserialise the object
                result = formatter.Deserialize(fs);
                fs.Close();
                return true;
            }
            else
            {
                result = null;
                return false;
            }


        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to read from {fullPath} with exception {e}");
            result = null;
            return false;
        }
    }

    /// <summary>
    /// Deletes a file from the disk.
    /// </summary>
    /// <param name="folderPath">Path to the folder we want to delete from</param>
    /// <param name="fileName">Name of the file</param>
    /// <param name="extension">File extension</param>
    /// <returns></returns>
    public static bool DeleteFile(string folderPath, string fileName, string extension)
    {
        var fullPath = Path.Combine(folderPath, fileName + extension);

        try
        {
            File.Delete(fullPath);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to delete {fullPath} with exception {e}");
            return false;
        }
    }
}
