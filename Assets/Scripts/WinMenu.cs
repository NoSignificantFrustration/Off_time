using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinMenu : MonoBehaviour
{
    [SerializeField] Text displayTextField;


    public void UpdateTexts()
    {
        string[] difficultyNames = new string[] { "K�nny�", "K�zepes", "Neh�z" };
        TimeSpan time = TimeSpan.FromSeconds(PlaySession.saveInfo.elapsedTime);

        displayTextField.text = $"Neh�zs�g: {difficultyNames[PlaySession.saveInfo.difficulty]}" +
            $"\n\nEltelt id�: {time.ToString("hh':'mm':'ss")}" +
            $"\n\nL�p�sek sz�ma: {PlaySession.saveInfo.moves}";
    }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
