using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerScoreEntryMenu : MenuBase
{
    public UnityEngine.UI.Text ScoreField;
    public UnityEngine.UI.InputField NameField;

    private bool Clicked;

    public override void OnStart()
    {
        base.OnStart();

        if (ScoreField)
        {
            TimeSpan span = TimeSpan.FromSeconds(Game.Instance.Score.RealTimePassed);
            ScoreField.text = string.Format(" {0}\n {1:D2}m {2:D2}s", Game.Instance.Score.Score, span.Minutes, span.Seconds);
        }

        Clicked = false;

        Game.Instance.Score.OnScoreUploaded -= ExitGame;
        Game.Instance.Score.OnScoreUploaded += ExitGame;
    }

    public void SubmitScore()
    {
        if(Clicked)
            return;
        
        Clicked = true;
        Canvas.interactable = false;
        
        if (NameField && !string.IsNullOrEmpty(NameField.text))
        {
            Game.Instance.Score.UploadScore(NameField.text);
        }
    }

    public void ExitGame()
    {
        SceneManager.LoadScene(0);
    }

    public override void OnLevelLoaded()
    {
        
    }

    public override void OnLevelCleanUp()
    {

    }
}
