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
        string[] difficultyNames = new string[] { "Könnyû", "Közepes", "Nehéz" };
        TimeSpan time = TimeSpan.FromSeconds(PlaySession.saveInfo.elapsedTime);

        displayTextField.text = $"Nehézség: {difficultyNames[PlaySession.saveInfo.difficulty]}" +
            $"\n\nEltelt idõ: {time.ToString("hh':'mm':'ss")}" +
            $"\n\nLépések száma: {PlaySession.saveInfo.moves}";
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
