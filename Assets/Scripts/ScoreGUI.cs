using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreGUI : MonoBehaviour, ILevelDependable
{
    public ScoreManager Score;
    public Text Label;
    public string Prefix = "Score : ";

    void Start()
    {
        if (!Score)
            Score = FindObjectOfType<ScoreManager>();
        Label.enabled = false;
    }

    void Update()
    {
        Label.text = string.Format("{0}{1}", Prefix, Score.Score);
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
