using Mono.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class DatabaseManager : MonoBehaviour
{

    [SerializeField] private string dbName = "GameDB";
    private string connectionPath;
    [SerializeField] private Text outputField;

    private void Awake()
    {
        connectionPath = "URI=file:" + Application.persistentDataPath + "/" + dbName + ".db";
    }

    private void Start()
    {
        SetupDB();
    }

    private void SetupDB()
    {

        //Debug.Log(Application.streamingAssetsPath);
        string filePath = Application.persistentDataPath + "/" + dbName + ".db";
        if (!File.Exists(filePath))
        {
            string backupPath = Path.Combine(Application.streamingAssetsPath, dbName + ".db");
            if (File.Exists(backupPath))
            {
                File.Copy(backupPath, filePath);
            }
            else
            {
                Debug.LogError("Database is missing");
                using (SqliteConnection connection = new SqliteConnection(connectionPath))
                {
                    connection.Open();
                    using (SqliteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "CREATE TABLE IF NOT EXISTS questions " +
                            "(id INTEGER PRIMARY KEY AUTOINCREMENT," +
                            "difficulty int(1) NOT NULL," +
                            "question varchar(255) NOT NULL," +
                            "good_answer varchar(100) NOT NULL," +
                            "bad_answer1 varchar(100) NOT NULL," +
                            "bad_answer2 varchar(100) NOT NULL," +
                            "bad_answer3 varchar(100) NOT NULL," +
                            "user_added int(1) NOT NULL)";
                        command.ExecuteNonQuery();
                        command.CommandText = "CREATE TABLE IF NOT EXISTS users " +
                            "(id INTEGER PRIMARY KEY AUTOINCREMENT," +
                            "username varchar(100) NOT NULL," +
                            "passwd varchar(255) NOT NULL)";
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
            }
        }
        
    }

    public void RunQuery(InputField inputField)
    {
        outputField.text = "";
        string query = inputField.text;
        using (SqliteConnection connection = new SqliteConnection(connectionPath))
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
        
        using (SqliteConnection connection = new SqliteConnection(connectionPath))
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

    

    public QuizHandler.QuizData GetQuestion()
    {
        QuizHandler.QuizData dat;

        using (SqliteConnection connection = new SqliteConnection(connectionPath))
        {
            connection.Open();
            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM questions ORDER BY RANDOM() LIMIT 1";
                command.ExecuteNonQuery();

                using (IDataReader reader = command.ExecuteReader())
                {

                    reader.Read();

                    dat.question = reader["question"].ToString();
                    dat.good_answer = reader["good_answer"].ToString();
                    dat.bad_answers = new string[3];
                    dat.bad_answers[0] = reader["bad_answer1"].ToString();
                    dat.bad_answers[1] = reader["bad_answer2"].ToString();
                    dat.bad_answers[2] = reader["bad_answer3"].ToString();
                    reader.Close();

                }

            }
            connection.Close();
        }
        return dat;
    }

    public bool RegisterUser(string username, string password)
    {
        bool alreadyExists;
        using (SqliteConnection connection = new SqliteConnection(connectionPath))
        {
            connection.Open();
            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandText = "SELECT COUNT(*) AS 'count' FROM users WHERE username='" + username +"'" ;
                command.ExecuteNonQuery();

                using (IDataReader reader = command.ExecuteReader())
                {

                    reader.Read();
                    if (Int32.Parse(reader["count"].ToString()) > 0)
                    {
                        alreadyExists = true;
                        reader.Close();
                    }
                    else
                    {
                        reader.Close();
                        alreadyExists = false;
                        command.CommandText = "INSERT INTO users (username, passwd) VALUES " +
                            "('" + username + "', '" + password + "')";
                        command.ExecuteNonQuery();
                    }
                    
                    

                }

            }
            connection.Close();
        }
        return !alreadyExists;
    }

    public bool Login(string username, string password, out int userID, out string uname)
    {

        bool success;
        using (SqliteConnection connection = new SqliteConnection(connectionPath))
        {
            connection.Open();
            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandText = "SELECT id, username, COUNT(*) AS count FROM users WHERE username = '" + username + "' AND passwd = '" + password + "'";
                command.ExecuteNonQuery();

                using (IDataReader reader = command.ExecuteReader())
                {

                    reader.Read();
                    if (Int32.Parse(reader["count"].ToString()) > 0)
                    {
                        success = true;
                        userID = Int32.Parse(reader["id"].ToString());
                        uname = reader["username"].ToString();
                    }
                    else
                    {
                        success = false;
                        userID = 0;
                        uname = "";
                    }
                    reader.Close();
                }

            }
            connection.Close();
        }
        return success;
    }

    //public string[] GetQuestion()
    //{
    //    string[] dat = new string[5];

    //    using (SqliteConnection connection = new SqliteConnection(connectionPath))
    //    {
    //        connection.Open();
    //        using (SqliteCommand command = connection.CreateCommand())
    //        {
    //            command.CommandText = "SELECT * FROM questions ORDER BY RANDOM() LIMIT 1";
    //            command.ExecuteNonQuery();

    //            using (IDataReader reader = command.ExecuteReader())
    //            {

    //                reader.Read();

    //                dat[0] = reader["question"].ToString();
    //                dat[1] = reader["good_answer"].ToString();
    //                dat[2] = reader["bad_answer1"].ToString();
    //                dat[3] = reader["bad_answer2"].ToString();
    //                dat[4] = reader["bad_answer3"].ToString();
    //                reader.Close();

    //            }

    //        }
    //        connection.Close();
    //    }
    //    return dat;
    //}




    public void ChangeText(InputField inputField)
    {
        outputField.text = inputField.text;
    }

    
}
