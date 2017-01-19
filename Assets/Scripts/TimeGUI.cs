using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeGUI : MonoBehaviour, ILevelDependable
{
    public ScoreManager Score;
    public Text Label;
    public string Prefix = "Time : ";

    void Start()
    {
        if (!Score)
            Score = FindObjectOfType<ScoreManager>();
        Label.enabled = false;
    }

    void Update()
    {
        TimeSpan span = TimeSpan.FromSeconds(Score.RealTimePassed);
        Label.text = string.Format("{0}{1:D2}:{2:D2}", Prefix, span.Minutes, span.Seconds);
    }

    public void OnLevelLoaded()
    {
        Label.enabled = true;
    }

    public void OnLevelCleanUp()
    {
        Label.enabled = false;
    }
}
