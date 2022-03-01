using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Win menu logic.
/// </summary>
public class WinMenu : MonoBehaviour
{
    /// <summary>Display text field</summary>
    [SerializeField] Text displayTextField;

    /// <summary>
    /// Updates the text field with the current values.
    /// </summary>
    public void UpdateTexts()
    {
        string[] difficultyNames = new string[] { "Könnyû", "Közepes", "Nehéz" };
        TimeSpan time = TimeSpan.FromSeconds(PlaySession.saveInfo.elapsedTime);

        displayTextField.text = $"Nehézség: {difficultyNames[PlaySession.saveInfo.difficulty]}" +
            $"\n\nEltelt idõ: {time.ToString("hh':'mm':'ss")}" +
            $"\n\nLépések száma: {PlaySession.saveInfo.moves}";
    }

}
