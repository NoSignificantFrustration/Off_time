using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores information about the current play session.
/// </summary>
/// <seealso cref="SaveGameInfo"/>
public static class PlaySession
{
    /// <summary>User ID</summary>
    public static int userID;
    /// <summary>Username</summary>
    public static string username;
    /// <summary>The number of the current level</summary>
    public static int currentLevel;
    /// <summary>Information about the current level</summary>
    public static SaveGameInfo saveInfo;
    
}
