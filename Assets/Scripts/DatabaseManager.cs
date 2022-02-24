using System;
using Mono.Data.Sqlite;
using System.Collections.Generic;
using System.Globalization;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class that handles database operations.
/// </summary>

public class DatabaseManager : MonoBehaviour
{

    /// <summary>The name of the database.</summary>
    [SerializeField] private string dbName = "GameDB";
    /// <summary>Path to the database file.</summary>
    private string connectionPath;
    /// <summary>Text field where results can be outputted.</summary>
    [SerializeField] private Text outputField;

    /// <summary>
    /// Sets the connection path when the script is loaded.
    /// </summary>
    private void Awake()
    {
        connectionPath = "URI=file:" + Application.persistentDataPath + "/" + dbName + ".db";
    }

    /// <summary>
    /// Calls SetupDB() when the game starts.
    /// </summary>
    private void Start()
    {
        SetupDB();
        //Debug.Log(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
    }

    /// <summary>
    /// Sets up the database if it wasn't already, or replaces it if it's missing.
    /// </summary>
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
                        command.CommandText = "CREATE TABLE IF NOT EXISTS saves " +
                            "(id INTEGER PRIMARY KEY AUTOINCREMENT," +
                            "userID INTEGER NOT NULL," +
                            "title VARCHAR(32) NOT NULL," +
                            "difficulty INTEGER(1) NOT NULL," +
                            "moves INTEGER NOT NULL," +
                            "levelName VARCHAR NOT NULL," +
                            "fileName VARCHAR NOT NULL," +
                            "elapsedTime INTEGER NOT NULL," +
                            "savetime DATETIME NOT NULL," +
                            "FOREIGN KEY (userID) REFERENCES users(id))";
                        command.ExecuteNonQuery();
                        command.CommandText = "CREATE TABLE IF NOT EXISTS wins(id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                            "userID INTEGER NOT NULL, " +
                            "difficulty INTEGER(1) NOT NULL, " +
                            "moves INTEGER NOT NULL, " +
                            "levelName VARCHAR NOT NULL, " +
                            "elapsedTime FLOAT NOT NULL, " +
                            "savetime DATETIME NOT NULL, " +
                            "FOREIGN KEY(userID) REFERENCES users(id))";
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
            }
            else
            {
                Debug.LogError("Database is missing");

            }
        }

    }

    /// <summary>
    /// Runs the query from the specified InputField.
    /// </summary>
    /// <param name="inputField">Source InputField</param>
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

    /// <summary>
    /// Runs the specified query.
    /// </summary>
    /// <param name="input">Query text</param>
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


    /// <summary>
    /// Gets a random question from the database.
    /// </summary>
    /// <returns>QuizHandler.QuizData</returns>
    /// <seealso cref="QuizHandler.QuizData"/>
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

                    dat.id = int.Parse(reader["id"].ToString());
                    dat.difficulty = int.Parse(reader["difficulty"].ToString());
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

    /// <summary>
    /// Tries to register a user.
    /// </summary>
    /// <param name="username">Username</param>
    /// <param name="password">Password</param>
    /// <returns>True if successful, false if the username is already taken.</returns>
    public bool RegisterUser(string username, string password)
    {

        username = Regex.Escape(username); //Replace(username, @"[\r\n\x00\x1a\\'""]", @"\$0");
        password = Regex.Escape(password); //Replace(password, @"[\r\n\x00\x1a\\'""]", @"\$0");
        bool alreadyExists;
        using (SqliteConnection connection = new SqliteConnection(connectionPath))
        {
            connection.Open();
            using (SqliteCommand command = connection.CreateCommand())
            {
                //Check if the username is taken
                command.CommandText = "SELECT COUNT(*) AS 'count' FROM users WHERE username='" + username + "'";
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
                        //Register the user
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

    /// <summary>
    /// Checks if a user with the specified username and password exists, and makes their ID available.
    /// </summary>
    /// <param name="username">Username</param>
    /// <param name="password">Password</param>
    /// <param name="userID">UserID</param>
    /// <param name="uname">Username</param>
    /// <param name="currentLevel">Current level</param>
    /// <returns>True if the login values check out, false if they don't.</returns>
    public bool Login(string username, string password, out int userID, out string uname, out int currentLevel)
    {

        username = Regex.Escape(username); //Replace(username, @"[\r\n\x00\x1a\\'""]", @"\$0");
        password = Regex.Escape(password); //Replace(password, @"[\r\n\x00\x1a\\'""]", @"\$0");

        bool success = false;
        using (SqliteConnection connection = new SqliteConnection(connectionPath))
        {
            connection.Open();
            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandText = "SELECT id, username, currentLevel, COUNT(*) AS count FROM users WHERE username = '" + username + "' AND passwd = '" + password + "'";
                command.ExecuteNonQuery();

                using (IDataReader reader = command.ExecuteReader())
                {

                    reader.Read();
                    if (Int32.Parse(reader["count"].ToString()) > 0)
                    {
                        success = true;
                        userID = Int32.Parse(reader["id"].ToString());
                        uname = reader["username"].ToString();
                        currentLevel = Int32.Parse(reader["currentLevel"].ToString());
                    }
                    else
                    {
                        success = false;
                        userID = new int();
                        uname = null;
                        currentLevel = new int();
                    }
                    reader.Close();
                }

            }
            connection.Close();
        }
        return success;
    }

    public void UpdateUserCurrentLevel()
    {
        using (SqliteConnection connection = new SqliteConnection(connectionPath))
        {
            connection.Open();
            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandText = $"UPDATE users SET currentLevel = {PlaySession.currentLevel} WHERE id = {PlaySession.userID}"; 
                command.ExecuteNonQuery();
            }
            connection.Close();
        }
    }

    /// <summary>
    /// Gets the current user's saved games using their ID.
    /// </summary>
    /// <param name="orderByCol">Column the records should be ordered by</param>
    /// <param name="sortingMode">Sorting mode (ASC/DESC)</param>
    /// <returns>A list of SaveGameInfos as generic objects.</returns>
    /// <seealso cref="SaveGameInfo"/>
    public List<object> GetSavedGames(string orderByCol, bool sortingMode)
    {
        List<object> saveGameInfos = new List<object>();

        using (SqliteConnection connection = new SqliteConnection(connectionPath))
        {
            connection.Open();
            using (SqliteCommand command = connection.CreateCommand())
            {
                string mode = sortingMode ? "ASC" : "DESC";
                command.CommandText = $"SELECT id, title, difficulty, levelName, fileName, moves, CAST(savetime AS nvarchar(10)) AS 'savetime', elapsedTime FROM saves WHERE userID = '{PlaySession.userID}' ORDER BY {orderByCol} {mode}";
                command.ExecuteNonQuery();

                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        SaveGameInfo saveInfo = new SaveGameInfo();
                        saveInfo.saveTitle = reader["title"].ToString();
                        saveInfo.difficulty = int.Parse(reader["difficulty"].ToString());
                        saveInfo.levelName = reader["levelName"].ToString();
                        saveInfo.fileName = reader["fileName"].ToString();
                        saveInfo.moves = int.Parse(reader["moves"].ToString());
                        saveInfo.saveTime = DateTime.ParseExact(reader["savetime"].ToString(), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                        saveInfo.elapsedTime = float.Parse(reader["elapsedTime"].ToString()) / 100f;
                        saveGameInfos.Add(saveInfo);
                        //"yyyy-MM-dd HH:mm:ss"
                    }
                    reader.Close();
                }

            }
            connection.Close();
        }

        return saveGameInfos;
    }

    /// <summary>
    /// Checks if the current user already has a save with the specified name.
    /// </summary>
    /// <param name="saveName">The name</param>
    /// <returns>True if it's taken, false if it's not.</returns>
    public bool CheckIfSaveNameTaken(string saveName)
    {

        saveName = Regex.Escape(saveName);

        bool taken = false;
        using (SqliteConnection connection = new SqliteConnection(connectionPath))
        {
            connection.Open();
            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandText = $"SELECT COUNT(*) AS count FROM saves WHERE title = '{saveName}' AND userID = {PlaySession.userID}";
                command.ExecuteNonQuery();

                using (IDataReader reader = command.ExecuteReader())
                {

                    reader.Read();
                    if (Int32.Parse(reader["count"].ToString()) > 0)
                    {
                        taken = true;
                    }
                    else
                    {
                        taken = false;
                    }
                    reader.Close();
                }

            }
            connection.Close();
        }
        return taken;
    }

    /// <summary>
    /// Makes a new save record with the current PlaySession.
    /// </summary>
    /// <seealso cref="PlaySession"/>
    public void AddSave()
    {

        using (SqliteConnection connection = new SqliteConnection(connectionPath))
        {
            connection.Open();
            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandText = $"INSERT INTO saves (userID, title, difficulty, moves, levelName, fileName, elapsedTime, savetime) VALUES" +
                    $"('{PlaySession.userID}', '{PlaySession.saveInfo.saveTitle}', '{PlaySession.saveInfo.difficulty}', '{PlaySession.saveInfo.moves}', '{PlaySession.saveInfo.levelName}', '{PlaySession.saveInfo.fileName}', '{(int)(PlaySession.saveInfo.elapsedTime * 100f)}', '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')";
                command.ExecuteNonQuery();
            }
            connection.Close();
        }
    }

    /// <summary>
    /// Overwrites the save record specified by the current PlaySession save title.
    /// </summary>
    /// <seealso cref="PlaySession"/>
    public void OverwriteSave()
    {
        using (SqliteConnection connection = new SqliteConnection(connectionPath))
        {
            connection.Open();
            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandText = $"UPDATE saves SET " +
                    $"difficulty = '{PlaySession.saveInfo.difficulty}', moves = '{PlaySession.saveInfo.moves}', levelName = '{PlaySession.saveInfo.levelName}', elapsedTime = '{(int)(PlaySession.saveInfo.elapsedTime * 100f)}', savetime = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}'" +
                    $"WHERE userID = '{PlaySession.userID}' AND title = '{PlaySession.saveInfo.saveTitle}'";
                command.ExecuteNonQuery();
            }
            connection.Close();
        }
    }


    /// <summary>
    /// Deletes the user's save record with the specified name.
    /// </summary>
    /// <param name="saveTitle">Save title</param>
    public void DeleteSave(string saveTitle)
    {
        using (SqliteConnection connection = new SqliteConnection(connectionPath))
        {
            connection.Open();
            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandText = $"DELETE FROM saves WHERE" +
                    $"(userID = '{PlaySession.userID}' AND title = '{saveTitle}')";
                command.ExecuteNonQuery();
            }
            connection.Close();
        }
    }

    /// <summary>
    /// Adds a win record to database.
    /// </summary>
    public void AddWin()
    {

        using (SqliteConnection connection = new SqliteConnection(connectionPath))
        {
            connection.Open();
            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandText = $"INSERT INTO wins (userID, difficulty, moves, levelName, elapsedTime, savetime) VALUES" +
                    $"('{PlaySession.userID}', '{PlaySession.saveInfo.difficulty}', '{PlaySession.saveInfo.moves}', '{PlaySession.saveInfo.levelName}', '{(int)(PlaySession.saveInfo.elapsedTime * 100f)}', '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')";
                command.ExecuteNonQuery();
            }
            connection.Close();
        }
    }

    /// <summary>
    /// Gets the leaderboard.
    /// </summary>
    /// <param name="orderByCol">Column the records should be ordered by</param>
    /// <param name="sortingMode">Sorting mode (ASC/DESC)</param>
    /// <returns>A list of WinDatas as gneric objects.</returns>
    public List<object> GetLeaderboard(string orderByCol, bool sortingMode)
    {
        List<object> winInfos = new List<object>();

        using (SqliteConnection connection = new SqliteConnection(connectionPath))
        {
            connection.Open();
            using (SqliteCommand command = connection.CreateCommand())
            {
                string mode = sortingMode ? "ASC" : "DESC";
                command.CommandText = $"SELECT users.username, difficulty, levelName, moves, CAST(savetime AS nvarchar(10)) AS 'savetime', elapsedTime FROM wins INNER JOIN users ON users.id = userID ORDER BY {orderByCol} {mode} ";
                command.ExecuteNonQuery();

                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        WinInfo winInfo = new WinInfo();
                        winInfo.username = reader["username"].ToString();
                        winInfo.difficulty = int.Parse(reader["difficulty"].ToString());
                        winInfo.levelName = reader["levelName"].ToString();
                        winInfo.moves = int.Parse(reader["moves"].ToString());
                        winInfo.saveTime = DateTime.ParseExact(reader["savetime"].ToString(), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                        winInfo.elapsedTime = float.Parse(reader["elapsedTime"].ToString()) / 100f;
                        winInfos.Add(winInfo);
                        //"yyyy-MM-dd HH:mm:ss"
                    }
                    reader.Close();
                }

            }
            connection.Close();
        }

        return winInfos;
    }

    /// <summary>
    /// Gets all user added quizes from database.
    /// </summary>
    /// <param name="orderByCol">Column the records should be ordered by</param>
    /// <param name="sortingMode">Sorting mode (ASC/DESC)</param>
    /// <returns>A generic object list that contains all user added QuizHandler.QuizDatas</returns>
    /// <seealso cref="QuizHandler.QuizData"/>
    public List<object> GetUserQuizes(string orderByCol, bool sortingMode)
    {
        List<object> winInfos = new List<object>();

        using (SqliteConnection connection = new SqliteConnection(connectionPath))
        {
            connection.Open();
            using (SqliteCommand command = connection.CreateCommand())
            {
                string mode = sortingMode ? "ASC" : "DESC";
                command.CommandText = $"SELECT * FROM questions ORDER BY {orderByCol} {mode} ";
                command.ExecuteNonQuery();

                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        QuizHandler.QuizData quizData = new QuizHandler.QuizData();
                        quizData.id = int.Parse(reader["id"].ToString());
                        quizData.difficulty = int.Parse(reader["difficulty"].ToString());
                        quizData.question = reader["question"].ToString();
                        quizData.good_answer = reader["good_answer"].ToString();
                        quizData.bad_answers = new string[3];
                        quizData.bad_answers[0] = reader["bad_answer1"].ToString();
                        quizData.bad_answers[1] = reader["bad_answer2"].ToString();
                        quizData.bad_answers[2] = reader["bad_answer3"].ToString();
                        winInfos.Add(quizData);
                        //"yyyy-MM-dd HH:mm:ss"
                    }
                    reader.Close();
                }

            }
            connection.Close();
        }

        return winInfos;
    }

    /// <summary>
    /// Adds a new quiz with the provided QuizHandler.QuizData.
    /// </summary>
    /// <param name="data">Data to add</param>
    public void AddQuiz(QuizHandler.QuizData data)
    {
        using (SqliteConnection connection = new SqliteConnection(connectionPath))
        {
            connection.Open();
            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandText = $"INSERT INTO questions (difficulty, question, good_answer, bad_answer1, bad_answer2, bad_answer3, user_added) VALUES" +
                    $"('{data.difficulty}', '{data.question}', '{data.good_answer}', '{data.bad_answers[0]}', '{data.bad_answers[1]}', '{data.bad_answers[2]}', '1')";
                command.ExecuteNonQuery();
            }
            connection.Close();
        }
    }

    /// <summary>
    /// Updates the quiz record with the given QuizHandler.QuizData 's id.
    /// </summary>
    /// <param name="data">Replacement QuizHandler.QuizData</param>
    public void UpdateQuiz(QuizHandler.QuizData data)
    {
        using (SqliteConnection connection = new SqliteConnection(connectionPath))
        {
            connection.Open();
            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandText = $"UPDATE questions SET difficulty = '{data.difficulty}', question = '{data.question}', good_answer = '{data.good_answer}', bad_answer1 = '{data.bad_answers[0]}', bad_answer2 = '{data.bad_answers[1]}', bad_answer3 = '{data.bad_answers[2]}' WHERE id = {data.id}";
                command.ExecuteNonQuery();
            }
            connection.Close();
        }
    }

    /// <summary>
    /// Deletes the quiz with the provided id.
    /// </summary>
    /// <param name="id">ID to be deleted</param>
    public void DeleteQuiz(int id)
    {
        using (SqliteConnection connection = new SqliteConnection(connectionPath))
        {
            connection.Open();
            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandText = $"DELETE FROM questions WHERE id = '{id}'";
                command.ExecuteNonQuery();
            }
            connection.Close();
        }
    }

}
