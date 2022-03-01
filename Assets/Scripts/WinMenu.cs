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
        string[] difficultyNames = new string[] { "K�nny�", "K�zepes", "Neh�z" };
        TimeSpan time = TimeSpan.FromSeconds(PlaySession.saveInfo.elapsedTime);

        displayTextField.text = $"Neh�zs�g: {difficultyNames[PlaySession.saveInfo.difficulty]}" +
            $"\n\nEltelt id�: {time.ToString("hh':'mm':'ss")}" +
            $"\n\nL�p�sek sz�ma: {PlaySession.saveInfo.moves}";
    }

}
