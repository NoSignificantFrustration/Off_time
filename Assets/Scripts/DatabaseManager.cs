using Mono.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;

public class DatabaseManager : MonoBehaviour
{

    [SerializeField] private string dbName = "GameDB";
    private string filePath;
    [SerializeField] private Text outputField;

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

    public void RunQuery(string input)
    {
        
        using (SqliteConnection connection = new SqliteConnection(filePath))
        {
            connection.Open();
            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandText = input;
                command.ExecuteNonQuery();

     

            }
            connection.Close();
        }

    }


    public string[] GetQuestion()
    {
        string[] dat = new string[5];

        using (SqliteConnection connection = new SqliteConnection(filePath))
        {
            connection.Open();
            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM questions ORDER BY RANDOM() LIMIT 1";
                command.ExecuteNonQuery();

                using (IDataReader reader = command.ExecuteReader())
                {

                    reader.Read();

                    dat[0] = reader["question"].ToString();
                    dat[1] = reader["good_answer"].ToString();
                    dat[2] = reader["bad_answer1"].ToString();
                    dat[3] = reader["bad_answer2"].ToString();
                    dat[4] = reader["bad_answer3"].ToString();
                    reader.Close();
                }

            }
            connection.Close();
        }
        return dat;
    }


    public void ChangeText(InputField inputField)
    {
        outputField.text = inputField.text;
    }

    
}
