using Mono.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;

public class DatabaseManager : MonoBehaviour
{

    [SerializeField]
    public string dbName = "GameDB";
    private string filePath;
    public Text outputField;

    private void Awake()
    {
        filePath = "URI=file:" + Application.persistentDataPath + "/" + dbName + ".db";
    }

    private void Start()
    {
        SetupDB();
    }

    private void SetupDB()
    {
        using (SqliteConnection connection = new SqliteConnection(filePath))
        {
            connection.Open();
            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE IF NOT EXISTS questions (id INTEGER PRIMARY KEY AUTOINCREMENT," +
                    "difficulty int(1) NOT NULL," +
                    "question varchar(255) NOT NULL," +
                    "good_answer varchar(100) NOT NULL," +
                    "bad_answer1 varchar(100) NOT NULL," +
                    "bad_answer2 varchar(100) NOT NULL," +
                    "bad_answer3 varchar(100) NOT NULL)";
                command.ExecuteNonQuery();
            }
            connection.Close();
        }
    }

    public void RunQuery(InputField inputField)
    {
        outputField.text = "";
        string query = inputField.text;
        using (SqliteConnection connection = new SqliteConnection(filePath))
        {
            connection.Open();
            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                command.ExecuteNonQuery();

                using (IDataReader reader = command.ExecuteReader())
                {
                    int i = 0;
                    while (reader.Read())
                    {
                        outputField.text += reader.GetValue(i) + "\n";
                        i++;
                    }
                    reader.Close();
                }
                
            }
            connection.Close();
        }

    }


    public void ChangeText(InputField inputField)
    {
        outputField.text = inputField.text;
    }

    
}
